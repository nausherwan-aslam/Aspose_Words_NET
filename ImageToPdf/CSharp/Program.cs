﻿//////////////////////////////////////////////////////////////////////////
// Copyright 2001-2011 Aspose Pty Ltd. All Rights Reserved.
//
// This file is part of Aspose.Words. The source code in this file
// is only intended as a supplement to the documentation, and is provided
// "as is", without warranty of any kind, either expressed or implied.
//////////////////////////////////////////////////////////////////////////
//ExStart
//ExId:ImageToPdf
//ExSummary:Converts an image into a PDF document.
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

using Aspose.Words;
using Aspose.Words.Drawing;

namespace ImageToPdf
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Sample infrastructure.
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
            string dataDir = new Uri(new Uri(exeDir), @"../../Data/").LocalPath;

            ConvertImageToPdf(dataDir + "Test.jpg", dataDir + "TestJpg Out.pdf");
            ConvertImageToPdf(dataDir + "Test.png", dataDir + "TestPng Out.pdf");
            ConvertImageToPdf(dataDir + "Test.wmf", dataDir + "TestWmf Out.pdf");
            ConvertImageToPdf(dataDir + "Test.tiff", dataDir + "TestTiff Out.pdf");
        }

        /// <summary>
        /// Converts an image to PDF using Aspose.Words for .NET.
        /// </summary>
        /// <param name="inputFileName">File name of input image file.</param>
        /// <param name="outputFileName">Output PDF file name.</param>
        public static void ConvertImageToPdf(string inputFileName, string outputFileName)
        {
            // Create Aspose.Words.Document and DocumentBuilder. 
            // The builder makes it simple to add content to the document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc); 

            // Read the image from file, ensure it is disposed.
            using (Image image = Image.FromFile(inputFileName))
            {
                // Get the number of frames in the image.
                int framesCount = image.GetFrameCount(FrameDimension.Page);

                // Loop through all frames.
                for (int frameIdx = 0; frameIdx < framesCount; frameIdx++)
                {
                    // Insert a section break before each new page, in case of a multi-frame TIFF.
                    if (frameIdx != 0)
                        builder.InsertBreak(BreakType.SectionBreakNewPage);

                    // Select active frame.
                    image.SelectActiveFrame(FrameDimension.Page, frameIdx);

                    // We want the size of the page to be the same as the size of the image.
                    // Convert pixels to points to size the page to the actual image size.
                    PageSetup ps = builder.PageSetup;
                    ps.PageWidth = ConvertUtil.PixelToPoint(image.Width, image.HorizontalResolution);
                    ps.PageHeight = ConvertUtil.PixelToPoint(image.Height, image.VerticalResolution);

                    // Insert the image into the document and position it at the top left corner of the page.
                    builder.InsertImage(
                        image, 
                        RelativeHorizontalPosition.Page, 
                        0, 
                        RelativeVerticalPosition.Page, 
                        0, 
                        ps.PageWidth, 
                        ps.PageHeight, 
                        WrapType.None);
                }
            }

            // Save the document to PDF.
            doc.Save(outputFileName);
        }
    }
}
//ExEnd