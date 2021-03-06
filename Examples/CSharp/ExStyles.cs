﻿//////////////////////////////////////////////////////////////////////////
// Copyright 2001-2011 Aspose Pty Ltd. All Rights Reserved.
//
// This file is part of Aspose.Words. The source code in this file
// is only intended as a supplement to the documentation, and is provided
// "as is", without warranty of any kind, either expressed or implied.
//////////////////////////////////////////////////////////////////////////
using System;
using Aspose.Words;
using NUnit.Framework;

namespace Examples
{
    [TestFixture]
    public class ExStyles : ExBase
    {
        [Test]
        public void GetStyles()
        {
            //ExStart
            //ExFor:DocumentBase.Styles
            //ExFor:Style.Name
            //ExId:GetStyles
            //ExSummary:Shows how to get access to the collection of styles defined in the document.
            Document doc = new Document();
            StyleCollection styles = doc.Styles;

            foreach (Style style in styles)
                Console.WriteLine(style.Name);
            //ExEnd
        }

        [Test]
        public void SetAllStyles()
        {
            //ExStart
            //ExFor:Style.Font
            //ExFor:Style
            //ExSummary:Shows how to change the font formatting of all styles in a document.
            Document doc = new Document();
            foreach (Style style in doc.Styles)
            {
                if (style.Font != null)
                {
                    style.Font.ClearFormatting();
                    style.Font.Size = 20;
                    style.Font.Name = "Arial";
                }
            }
            //ExEnd
        }

        [Test]
        public void ChangeStyleOfTOCLevel()
        {
            Document doc = new Document();
            //ExStart
            //ExId:ChangeTOCStyle
            //ExSummary:Changes a formatting property used in the first level TOC style.
            // Retrieve the style used for the first level of the TOC and change the formatting of the style.
            doc.Styles[StyleIdentifier.Toc1].Font.Bold = true;
            //ExEnd
        }

        [Test]
        public void ChangeTOCTabStops()
        {
            //ExStart
            //ExFor:TabStop
            //ExFor:ParagraphFormat.TabStops
            //ExFor:Style.StyleIdentifier
            //ExFor:TabStopCollection.RemoveByPosition
            //ExFor:TabStop.Alignment
            //ExFor:TabStop.Position
            //ExFor:TabStop.Leader
            //ExId:ChangeTOCTabStops
            //ExSummary:Shows how to modify the position of the right tab stop in TOC related paragraphs.
            Document doc = new Document(MyDir + "Document.TableOfContents.doc");

            // Iterate through all paragraphs in the document
            foreach (Paragraph para in doc.GetChildNodes(NodeType.Paragraph, true))
            {
                // Check if this paragraph is formatted using the TOC result based styles. This is any style between TOC and TOC9.
                if (para.ParagraphFormat.Style.StyleIdentifier >= StyleIdentifier.Toc1 && para.ParagraphFormat.Style.StyleIdentifier <= StyleIdentifier.Toc9)
                {
                    // Get the first tab used in this paragraph, this should be the tab used to align the page numbers.
                    TabStop tab = para.ParagraphFormat.TabStops[0];
                    // Remove the old tab from the collection.
                    para.ParagraphFormat.TabStops.RemoveByPosition(tab.Position);
                    // Insert a new tab using the same properties but at a modified position. 
                    // We could also change the separators used (dots) by passing a different Leader type
                    para.ParagraphFormat.TabStops.Add(tab.Position - 50, tab.Alignment, tab.Leader);
                }
            }

            doc.Save(MyDir + "Document.TableOfContentsTabStops Out.doc");
            //ExEnd
        }
    }
}
