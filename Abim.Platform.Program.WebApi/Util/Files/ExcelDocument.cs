using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.WebApi.Util.Files
{
    /// <summary>
    /// ExcelDocument class. Generates a simple XLSX spreadsheet, with auto-sized cells and a bold top row.
    /// </summary>
    public class ExcelDocument
    {
        #region Fields

        #region Settings

        /// <summary>
        /// Whether to use borders
        /// </summary>
        protected const bool UseBorders = true;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sheet names.
        /// </summary>
        /// <value>
        /// The sheet names.
        /// </value>
        protected List<string> SheetNames { get; set; }

        /// <summary>
        /// Gets or sets the sheet text.
        /// </summary>
        /// <value>
        /// The sheet text.
        /// </value>
        protected Dictionary<string, string[,]> SheetText { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelDocument"/> class.
        /// </summary>
        public ExcelDocument()
        {
            SheetNames = new List<string>();
            SheetText = new Dictionary<string, string[,]>();
        }

        /// <summary>
        /// Sets all the data for a worksheet.
        /// </summary>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="text">The text, grouped primarily by ROW, not by column</param>
        public void SetData(string sheetName, string[,] text)
        {
            if(!SheetNames.Contains(sheetName))
                SheetNames.Add(sheetName);
            int width = text.GetLength(0), height = text.GetLength(1);
            var copy = new string[width, height];
            for(int i = 0; i < width; i++)
                for(int j = 0; j < height; j++)
                    copy[i, j] = text[i, j] ?? "";
            SheetText[sheetName] = copy;
        }

        /// <summary>
        /// Saves the document to the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Save(string path)
        {
            //Create the document
            var document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();
            
            //Create the worksheets
            var worksheetsPerSheet = new Dictionary<Sheet, Worksheet>();
            var sheets = new List<Sheet>();
            var sheetsResource = workbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            var sheetCollection = sheetsResource.Elements<Sheet>().ToList();
            for(int i = 0; i < SheetNames.Count; i++)
            {
                WorksheetPart newWorksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = new Worksheet();
                string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);
                uint sheetId = 1;
                if(sheetCollection.Count() > 0)
                {
                    sheetId = sheetCollection.Select(s => s.SheetId.Value).Max() + 1;
                }
                Sheet sheet = new Sheet()
                {
                    Id = relationshipId,
                    SheetId = sheetId,
                    Name = SheetNames[i]
                };
                sheetsResource.Append(sheet);
                sheets.Add(sheet);
                worksheetsPerSheet[sheet] = newWorksheetPart.Worksheet;
            }
            
            //Set Column Widths
            for(int i = 0; i < SheetNames.Count; i++)
            {
                var sheet = sheets[i];
                var text = SheetText[SheetNames[i]];
                var worksheetData = worksheetsPerSheet[sheet];
                var columnsData = worksheetData.GetFirstChild<Columns>();
                if(columnsData == null)
                {
                    columnsData = new Columns();
                    worksheetData.AppendChild(columnsData);
                }
                for(int column = 0; column < text.GetLength(1); column++)
                {
                    int maxCharacterCount = 0;
                    for(int row = 1; row < text.GetLength(0); row++)
                    {
                        if(maxCharacterCount < text[row, column].Length)
                            maxCharacterCount = text[row, column].Length;
                    }
                    int letterAccommodation = Math.Max(text[0, column].Length, maxCharacterCount / 2);
                    int pointWidth = (int)(Math.Ceiling(letterAccommodation * 1.5));
                    columnsData.Append(
                        new Column()
                        {
                            Min = (uint)column + 1,
                            Max = (uint)column + 1,
                            Width = new DoubleValue((double)pointWidth),
                            CustomWidth = true
                        }
                    );
                }
            }
            
            //Set SheetData
            for(int i = 0; i < SheetNames.Count; i++)
            {
                var sheet = sheets[i];
                var worksheetData = worksheetsPerSheet[sheet];
                worksheetData.Append(new SheetData());
            }
            
            //Write the data
            var stringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            for(int i = 0; i < SheetNames.Count; i++)
            {
                var sheet = sheets[i];
                var text = SheetText[SheetNames[i]];
                for(int row = 0; row < text.GetLength(0); row++)
                {
                    for(int column = 0; column < text.GetLength(1); column++)
                    {
                        AddCell(worksheetsPerSheet[sheet], stringTablePart, row, column, text[row, column]);
                    }
                }
            }

            //Style the cells: create a Stylesheet
            WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
            Stylesheet stylesheet = new Stylesheet();
            stylePart.Stylesheet = stylesheet;
            
            //Style the cells: create a Border
            int borderID = 0;
            if(UseBorders)
            {
                stylesheet.Borders = new Borders();
                stylesheet.Borders.Count = 1;
                stylesheet.Borders.AppendChild(new Border());    //default border
                var border = new Border()
                {
                    TopBorder = new TopBorder()
                    {
                        Style = BorderStyleValues.Thin,
                        Color = new Color()
                        {
                            Rgb = ToHex(new[]{0, 0, 0})
                        }
                    },
                    LeftBorder = new LeftBorder()
                    {
                        Style = BorderStyleValues.Thin,
                        Color = new Color()
                        {
                            Rgb = ToHex(new[]{0, 0, 0})
                        }
                    },
                    RightBorder = new RightBorder()
                    {
                        Style = BorderStyleValues.Thin,
                        Color = new Color()
                        {
                            Rgb = ToHex(new[]{0, 0, 0})
                        }
                    },
                    BottomBorder = new BottomBorder()
                    {
                        Style = BorderStyleValues.Thin,
                        Color = new Color()
                        {
                            Rgb = ToHex(new[]{0, 0, 0})
                        }
                    }
                };
                stylesheet.Borders.Append(border);
                borderID = (int)(stylesheet.Borders.Count.Value);
                stylesheet.Borders.Count++;
            }
            
            //Style the cells: create a Font
            stylesheet.Fonts = new Fonts();
            stylesheet.Fonts.Count = 1;
            stylesheet.Fonts.AppendChild(new Font());    //default font
            Font font = new Font()
            {
                FontName = new FontName(){ Val = "Calibri" },
                Bold = new Bold(),
                Color = new Color
                {
                    Rgb = ToHex(new[]{ 0, 0, 0 })
                },
                FontSize = new FontSize()
                {
                    Val = new DoubleValue((double)(11))
                }
            };
            stylesheet.Fonts.Append(font);
            var fontID = (int)(stylesheet.Fonts.Count.Value);
            stylesheet.Fonts.Count++;
            
            //Style the cells: create a Fill
            stylesheet.Fills = new Fills();
            stylesheet.Fills.Count = 1;
            stylesheet.Fills.AppendChild(new Fill());    //default fill
            Fill fill = new Fill()
            {
                PatternFill = new PatternFill()
            };
            stylesheet.Fills.Append(fill);
            var fillID = (int)(stylesheet.Fills.Count.Value);
            stylesheet.Fills.Count++;
            
            //Style the cells: create 2 CellFormats
            stylesheet.CellFormats = new CellFormats();
            stylesheet.CellFormats.Count = 1;
            stylesheet.CellFormats.AppendChild(    //default CellFormat
                new CellFormat()
                {
                    FormatId = 0
                }
            );
            var cellFormat1 = new CellFormat()
            {
                FormatId = 1
            };
            if(UseBorders)
                cellFormat1.BorderId = new UInt32Value((uint)borderID);
            stylesheet.CellFormats.AppendChild(cellFormat1);
            UInt32Value styleIndex1 = stylesheet.CellFormats.Count;
            stylesheet.CellFormats.Count++;
            var cellFormat2 = new CellFormat()
            {
                FormatId = 2,
                FontId = new UInt32Value((uint)fontID)
            };
            if(UseBorders)
                cellFormat2.BorderId = new UInt32Value((uint)borderID);
            stylesheet.CellFormats.AppendChild(cellFormat2);
            UInt32Value styleIndex2 = stylesheet.CellFormats.Count;
            stylesheet.CellFormats.Count++;
            
            //Style the cells: apply the StyleIndex
            for(int i = 0; i < SheetNames.Count; i++)
            {
                var sheet = sheets[i];
                var text = SheetText[SheetNames[i]];
                for(int column = 0; column < text.GetLength(1); column++)
                {
                    Cell cell = GetCell(worksheetsPerSheet[sheet], 0, column);
                    cell.StyleIndex = styleIndex2;
                }
                for(int row = 1; row < text.GetLength(0); row++)
                {
                    for(int column = 0; column < text.GetLength(1); column++)
                    {
                        Cell cell = GetCell(worksheetsPerSheet[sheet], row, column);
                        cell.StyleIndex = styleIndex1;
                    }
                }
            }
            
            //Save
            foreach(var worksheetPart in document.WorkbookPart.Parts.Where(p => p.OpenXmlPart is WorksheetPart)
                .Select(w => w.OpenXmlPart as WorksheetPart))
            {
                worksheetPart.Worksheet.Save();
            }
            workbookPart.Workbook.Save();
            document.Close();
        }

        /// <summary>
        /// Gets a cell.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected Cell GetCell(Worksheet worksheet, int row, int column)
        {
            SheetData sheetData = worksheet.Elements<SheetData>().First();
            Row rowObject = sheetData.Elements<Row>().Where(r => r.RowIndex.Value == row + 1).FirstOrDefault();
            if(rowObject != null)
            {
                var cellFound = rowObject.Elements<Cell>().Where(c => GetColumnNumber(c) == column).FirstOrDefault();
                return cellFound;
            }
            return null;
        }

        /// <summary>
        /// Gets a row, adding a new one if necessary.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected Row GetRow(Worksheet worksheet, int row)
        {
            SheetData sheetData = worksheet.Elements<SheetData>().First();
            Row rowObject = sheetData.Elements<Row>().Where(r => r.RowIndex.Value == row + 1).FirstOrDefault();
            if(rowObject == null)
            {
                Row refRow = null;
                foreach(Row r in sheetData.Elements<Row>())
                {
                    if(r.RowIndex.Value > (row + 1))
                    {
                        refRow = r;
                        break;
                    }
                }
                rowObject = new Row()
                {
                    RowIndex = new UInt32Value((uint)(row + 1)),
                    Height = new DoubleValue(40.0),
                    CustomHeight = true
                };
                if(refRow == null) sheetData.Append(rowObject);
                else sheetData.InsertBefore(rowObject, refRow);
            }
            return rowObject;
        }

        /// <summary>
        /// Adds a cell, if it doesn't already exist.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="sharedStringTable">The sharedStringTable.</param>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        protected void AddCell(Worksheet worksheet, SharedStringTablePart sharedStringTable, int row, int column, string value)
        {
            Row rowObject = GetRow(worksheet, row);
            
            string cellReference = TranslateColumnBackToString(column) + (row + 1);
            Cell refCell = null;
            foreach (Cell cell in rowObject.Elements<Cell>())
            {
                if(GetColumnNumber(cell.CellReference.Value) == column) return;     //already existed
                if(GetColumnNumber(cell.CellReference.Value) > column)
                {
                    refCell = cell;
                    break;
                }
            }
            Cell newCell = new Cell()
            {
                CellReference = cellReference
            };
            if(refCell != null)
            {
                rowObject.InsertBefore(newCell, refCell);
            }
            else
            {
                Cell last = rowObject.Elements<Cell>().LastOrDefault();
                if(last != null)
                {
                    rowObject.InsertAfter(newCell, last);
                }
                else
                {
                    rowObject.InsertAt(newCell, 0);
                }
            }
            
            newCell.DataType = CellValues.String;
            newCell.CellValue = new CellValue(value);
        }

        /// <summary>
        /// Inserts a shared string.
        /// </summary>
        /// <param name="sharedStringTable">The shared string table.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected int InsertSharedString(SharedStringTablePart stringTablePart, string value)
        {
            var stringTable = stringTablePart.SharedStringTable;
            if(stringTable == null)
            {
                stringTable = new SharedStringTable();
                stringTablePart.SharedStringTable = stringTable;
            }
            SharedStringItem item = CreateSharedStringItem(value);
            stringTable.AppendChild(item);
            return stringTable.Elements<SharedStringItem>().Count() - 1;        //return the index
        }

        /// <summary>
        /// Creates a shared string item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected SharedStringItem CreateSharedStringItem(string value)
        {
            SharedStringItem item = new SharedStringItem(new Text(value));
            return item;
        }
        
        /// <summary>
        /// RGB to Hex
        /// </summary>
        protected static string ToHex(int[] rgb)
        {
            return HexBinaryValue.FromString(rgb[0].ToString("X2") + rgb[1].ToString("X2") + rgb[2].ToString("X2"));
        }

        /// <summary>
        /// Checks if a color matches RGB, by its hex.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <param name="rgb">The RGB.</param>
        /// <returns></returns>
        protected static bool ColorMatches(HexBinaryValue hex, int[] rgb)
        {
            if(hex == null) return (rgb == null);
            string hexString = hex.Value;   //RRGGBBAA format
            int[] hexRGB = new int[3];
            for(int index = hexString.Length == 8/*ARGB*/ ? 2 : 0, count = 0; count < 3; index += 2, count++)
            {
                hexRGB[count] = int.Parse(hexString.Substring(index, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return (hexRGB[0] == rgb[0] && hexRGB[1] == rgb[1] && hexRGB[2] == rgb[2]);
        }
     
        /// <summary>
        /// Translates a column from e.g. 'Z' to 25 or 'AA' to 26
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        protected static int TranslateColumn(string columnName)
        {
            columnName = columnName.ToUpper();
            int total = 0;
            for(int i = 0, placeValue = 1; i < columnName.Length; i++, placeValue *= 26)
            {
                total += (Val(columnName[columnName.Length - 1 - i]) + (i > 0 ? 1 : 0)) * placeValue;
            }
            return total;
        }
 
        /// <summary>
        /// Translates a column back to a string, e.g. 25 to 'Z' or 26 to 'AA'
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected static string TranslateColumnBackToString(int column)
        {
            string ret = "";
            int A = 'A';
            bool firstDigit = true;
            while(column > 0)
            {
                ret = (char)(A + (column % 26) - (firstDigit ? 0 : 1)) + ret;
                column /= 26;
                firstDigit = false;
            }
            return ret;
        }
    
        /// <summary>
        /// Gets a row number from a cell reference. For example "AA12" will return 12
        /// </summary>
        /// <param name="columnRowReference">The column row reference.</param>
        /// <returns></returns>
        protected static int GetRowNumber(string columnRowReference)
        {
            return int.Parse(new string(columnRowReference.ToCharArray().Where(c => Char.IsDigit(c)).ToArray())) - 1;
        }

        /// <summary>
        /// Gets a column number from a cell reference. For example "AA12" will return 26 (since AA is column 26)
        /// </summary>
        /// <param name="columnRowReference">The column row reference.</param>
        /// <returns></returns>
        protected static int GetColumnNumber(string columnRowReference)
        {
            return TranslateColumn(new string(columnRowReference.ToCharArray().Where(c => Char.IsLetter(c)).ToArray()));
        }

        /// <summary>
        /// Gets the column number for a Cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        protected int GetColumnNumber(Cell cell)
        {
            return TranslateColumn(new string(cell.CellReference.ToString().ToCharArray().Where(c => Char.IsLetter(c)).ToArray()));
        }

        /// <summary>
        /// Gets the ordinal integer for a letter. For example, 'A' returns 1 and 'B' returns 2
        /// </summary>
        /// <param name="letter">The letter.</param>
        /// <returns></returns>
        protected static int Val(char letter)
        {
            return letter - 'A';
        }
    }
}
