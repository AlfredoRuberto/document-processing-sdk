﻿using System;
using System.IO;
using Telerik.Documents.SpreadsheetStreaming;

namespace GenerateDocuments
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter file name: ");
            string fileName = Console.ReadLine();
            if (String.IsNullOrEmpty(fileName))
            {
                Console.WriteLine("Invalid file name (empty or null). Press any key to exit.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Generating and saving document...");

            string filePath = String.Format("../../Files/{0}.xlsx", fileName);
            GenerateDocument(filePath);

            Console.Write("Want to open the document? (Y/N)");
            var readKey = Console.ReadKey();
            if (readKey.KeyChar == 'Y' || readKey.KeyChar == 'y')
            {
                string absoluteFilePath = Path.GetFullPath(filePath);
                System.Diagnostics.Process.Start(absoluteFilePath);
            }
        }

        private static void GenerateDocument(string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            using (FileStream stream = File.OpenWrite(filePath))
            {
                using (IWorkbookExporter workbook = SpreadExporter.CreateWorkbookExporter(SpreadDocumentFormat.Xlsx, stream))
                {
                    // Creating a style which would be used later in the code.
                    SpreadCellStyle style = workbook.CellStyles.Add("MyStyle");
                    style.Underline = SpreadUnderlineType.DoubleAccounting;
                    style.VerticalAlignment = SpreadVerticalAlignment.Top;

                    using (IWorksheetExporter worksheet = workbook.CreateWorksheetExporter("My sheet"))
                    {
                        // It is mandatory to export the worksheet view state before filling the worksheet with data.
                        using (IWorksheetViewExporter worksheetView = worksheet.CreateWorksheetViewExporter())
                        {
                            worksheetView.SetFirstVisibleCell(3, 0);

                            worksheetView.AddSelectionRange(9, 0, 13, 6);
                            worksheetView.SetActiveSelectionCell(11, 3);
                        }

                        // It is mandatory to export the column setting before exporting the row and cell data.
                        worksheet.SkipColumns(1);
                        using (IColumnExporter column = worksheet.CreateColumnExporter())
                        {
                            column.SetWidthInPixels(80);
                        }

                        worksheet.SkipRows(3);
                        using (IRowExporter row = worksheet.CreateRowExporter())
                        {
                            row.SkipCells(3);
                            using (ICellExporter cell = row.CreateCellExporter())
                            {
                                cell.SetValue("Merged cell.");
                                cell.SetFormat(new SpreadCellFormat()
                                {
                                    CellStyle = style,
                                    HorizontalAlignment = SpreadHorizontalAlignment.Center,
                                    VerticalAlignment = SpreadVerticalAlignment.Center
                                });
                            }
                        }

                        using (IRowExporter row = worksheet.CreateRowExporter())
                        {
                            row.SetHeightInPixels(200);
                            using (ICellExporter cell = row.CreateCellExporter())
                            {
                                cell.SetValue(123.456);
                            }

                            using (ICellExporter cell = row.CreateCellExporter())
                            {
                                SpreadCellFormat format = new SpreadCellFormat()
                                {
                                    NumberFormat = "dd/mm/yyyy",
                                    IsBold = true
                                };
                                cell.SetFormat(format);
                                cell.SetValue(42370);
                            }
                        }

                        worksheet.MergeCells(3, 3, 6, 6);
                    }
                }
            }
        }
    }
}
