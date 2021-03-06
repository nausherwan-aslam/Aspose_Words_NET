﻿//////////////////////////////////////////////////////////////////////////
// Copyright 2001-2011 Aspose Pty Ltd. All Rights Reserved.
//
// This file is part of Aspose.Words. The source code in this file
// is only intended as a supplement to the documentation, and is provided
// "as is", without warranty of any kind, either expressed or implied.
//////////////////////////////////////////////////////////////////////////
//ExStart
//ExId:XMLMailMerge
//ExSummary:Simple Mail Merge from XML using DataSet.
using System;
using System.Data;
using System.IO;
using System.Reflection;

using Aspose.Words;

namespace XMLMailMerge
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Sample infrastructure.
            string exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
            string dataDir = new Uri(new Uri(exeDir), @"../../Data/").LocalPath;

            // Create the Dataset and read the XML.
            DataSet customersDs = new DataSet();
            customersDs.ReadXml(dataDir + "Customers.xml");

            // Open a template document.
            Document doc = new Document(dataDir + "TestFile.doc");

            // Execute mail merge to fill the template with data from XML using DataTable.
            doc.MailMerge.Execute(customersDs.Tables["Customer"]);

            // Save the output document.
            doc.Save(dataDir + "TestFile Out.doc");
        }
    }
}
//ExEnd