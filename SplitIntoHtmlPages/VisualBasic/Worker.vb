'////////////////////////////////////////////////////////////////////////
' Copyright 2001-2011 Aspose Pty Ltd. All Rights Reserved.
'
' This file is part of Aspose.Words. The source code in this file
' is only intended as a supplement to the documentation, and is provided
' "as is", without warranty of any kind, either expressed or implied.
'////////////////////////////////////////////////////////////////////////

Imports Microsoft.VisualBasic
Imports System.Collections
Imports System.IO
Imports System.Text
Imports Aspose.Words
Imports Aspose.Words.Reporting
Imports Aspose.Words.Saving

Namespace SplitIntoHtmlPages
	''' <summary>
	''' This class takes a Microsoft Word document, splits it into topics at paragraphs formatted
	''' with the Heading 1 style and saves every topic as an HTML file.
	''' 
	''' Also generates contents.html file that provides links to all saved topics.
	''' </summary>
	Friend Class Worker
		''' <summary>
		''' Performs the Word to HTML conversion.
		''' </summary>
		''' <param name="srcFileName">The MS Word file to convert.</param>
		''' <param name="tocTemplate">An MS Word file that is used as a template to build
		''' a table of contents. This file needs to have a mail merge region called "TOC" defined
		''' and one mail merge field called "TocEntry".</param>
		''' <param name="dstDir">The output directory where to write HTML files. Must exist.</param>
		Friend Sub Execute(ByVal srcFileName As String, ByVal tocTemplate As String, ByVal dstDir As String)
			mDoc = New Document(srcFileName)
			mTocTemplate = tocTemplate
			mDstDir = dstDir

			Dim topicStartParas As ArrayList = SelectTopicStarts()
			InsertSectionBreaks(topicStartParas)
			Dim topics As ArrayList = SaveHtmlTopics()
			SaveTableOfContents(topics)
		End Sub

		''' <summary>
		''' Selects heading paragraphs that must become topic starts.
		''' We can't modify them in this loop, we have to remember them in an array first.
		''' </summary>
		Private Function SelectTopicStarts() As ArrayList
			Dim paras As NodeCollection = mDoc.GetChildNodes(NodeType.Paragraph, True, False)
			Dim topicStartParas As New ArrayList()

			For Each para As Paragraph In paras
				Dim style As StyleIdentifier = para.ParagraphFormat.StyleIdentifier
				If style = StyleIdentifier.Heading1 Then
					topicStartParas.Add(para)
				End If
			Next para

			Return topicStartParas
		End Function

		''' <summary>
		''' Inserts section breaks before the specified paragraphs.
		''' </summary>
		Private Sub InsertSectionBreaks(ByVal topicStartParas As ArrayList)
			Dim builder As New DocumentBuilder(mDoc)
			For Each para As Paragraph In topicStartParas
				Dim section As Section = para.ParentSection

				' Insert section break if the paragraph is not at the beginning of a section already.
				If para IsNot section.Body.FirstParagraph Then
					builder.MoveTo(para.FirstChild)
					builder.InsertBreak(BreakType.SectionBreakNewPage)

					' This is the paragraph that was inserted at the end of the now old section.
					' We don't really need the extra paragraph, we just needed the section.
					section.Body.LastParagraph.Remove()
				End If
			Next para
		End Sub

		''' <summary>
		''' Splits the current document into one topic per section and saves each topic
		''' as an HTML file. Returns a collection of Topic objects.
		''' </summary>
		Private Function SaveHtmlTopics() As ArrayList
			Dim topics As New ArrayList()
			For sectionIdx As Integer = 0 To mDoc.Sections.Count - 1
				Dim section As Section = mDoc.Sections(sectionIdx)

				Dim paraText As String = section.Body.FirstParagraph.GetText()

				' The text of the heading paragaph is used to generate the HTML file name.
				Dim fileName As String = MakeTopicFileName(paraText)
				If fileName = "" Then
					fileName = "UNTITLED SECTION " & sectionIdx
				End If

				fileName = Path.Combine(mDstDir, fileName & ".html")

				' The text of the heading paragraph is also used to generate the title for the TOC.
				Dim title As String = MakeTopicTitle(paraText)
				If title = "" Then
					title = "UNTITLED SECTION " & sectionIdx
				End If

				Dim topic As New Topic(title, fileName)
				topics.Add(topic)

				SaveHtmlTopic(section, topic)
			Next sectionIdx

			Return topics
		End Function

		''' <summary>
		''' Leaves alphanumeric characters, replaces white space with underscore
		''' and removes all other characters from a string.
		''' </summary>
		Private Shared Function MakeTopicFileName(ByVal paraText As String) As String
			Dim b As New StringBuilder()
			For Each c As Char In paraText
				If Char.IsLetterOrDigit(c) Then
					b.Append(c)
				ElseIf c = " "c Then
					b.Append("_"c)
				End If
			Next c
			Return b.ToString()
		End Function

		''' <summary>
		''' Removes the last character (which is a paragraph break character from the given string).
		''' </summary>
		Private Shared Function MakeTopicTitle(ByVal paraText As String) As String
			Return paraText.Substring(0, paraText.Length - 1)
		End Function

		''' <summary>
		''' Saves one section of a document as an HTML file.
		''' Any embedded images are saved as separate files in the same folder as the HTML file.
		''' </summary>
		Private Shared Sub SaveHtmlTopic(ByVal section As Section, ByVal topic As Topic)
			Dim dummyDoc As New Document()
			dummyDoc.RemoveAllChildren()
			dummyDoc.AppendChild(dummyDoc.ImportNode(section, True, ImportFormatMode.KeepSourceFormatting))

			dummyDoc.BuiltInDocumentProperties.Title = topic.Title

			Dim saveOptions As New HtmlSaveOptions()
			saveOptions.PrettyFormat = True
			' This is to allow headings to appear to the left of main text.
			saveOptions.AllowNegativeLeftIndent = True
			saveOptions.ExportHeadersFootersMode = ExportHeadersFootersMode.None

			dummyDoc.Save(topic.FileName, saveOptions)
		End Sub

		''' <summary>
		''' Generates a table of contents for the topics and saves to contents.html.
		''' </summary>
		Private Sub SaveTableOfContents(ByVal topics As ArrayList)
			Dim tocDoc As New Document(mTocTemplate)

			' We use a custom mail merge even handler defined below.
			tocDoc.MailMerge.FieldMergingCallback = New HandleTocMergeField()
			' We use a custom mail merge data source based on the collection of the topics we created.
			tocDoc.MailMerge.ExecuteWithRegions(New TocMailMergeDataSource(topics))

			tocDoc.Save(Path.Combine(mDstDir, "contents.html"))
		End Sub

		Private Class HandleTocMergeField
			Implements IFieldMergingCallback
			Private Sub IFieldMergingCallback_FieldMerging(ByVal e As FieldMergingArgs) Implements IFieldMergingCallback.FieldMerging
				If mBuilder Is Nothing Then
					mBuilder = New DocumentBuilder(e.Document)
				End If

				' Our custom data source returns topic objects.
				Dim topic As Topic = CType(e.FieldValue, Topic)

				' We use the document builder to move to the current merge field and insert a hyperlink.
				mBuilder.MoveToMergeField(e.FieldName)
				mBuilder.InsertHyperlink(topic.Title, topic.FileName, False)

				' Signal to the mail merge engine that it does not need to insert text into the field
				' as we've done it already.
				e.Text = ""
			End Sub

			Private Sub ImageFieldMerging(ByVal args As ImageFieldMergingArgs) Implements IFieldMergingCallback.ImageFieldMerging
				' Do nothing.
			End Sub

			Private mBuilder As DocumentBuilder
		End Class

		Private mDoc As Document
		Private mTocTemplate As String
		Private mDstDir As String
	End Class
End Namespace
