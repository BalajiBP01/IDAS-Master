using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.Production;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class ExcelDataPort<TModel> where TModel : class, new()
    {
        public void ExportData(List<TModel> entities, string filepath)
        {
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = spreadsheet.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                Sheets sheets = new Sheets();
                Sheet Sheet = CreateSheet(entities, workbookPart, spreadsheet);
                sheets.Append(Sheet);
                spreadsheet.WorkbookPart.Workbook.AppendChild<Sheets>(sheets);
                workbookPart.Workbook.Save();
                spreadsheet.Close();
            }
        }
        public Sheet CreateSheet(IList entities, WorkbookPart workbookPart, SpreadsheetDocument spreadsheet)
        {
            var typ = entities.GetType().GetGenericArguments()[0];

            WorksheetPart childworksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            SheetData childsheetData = new SheetData();

            Sheet childsheet = new Sheet();
            childsheet.Id = spreadsheet.WorkbookPart.GetIdOfPart(childworksheetPart);
            childsheet.SheetId = 1;
            childsheet.Name = typ.Name;

            var props = GetProperties(typ);
            Row r = new Row { RowIndex = 1 };
            for (int j = 0; j < props.Count(); j++)
            {
                string childHeader = ExcelColumnIndexToName(j);
                Cell cell1 = CreateTextCell(childHeader, 1, props[j].Name);
                r.Append(cell1);
            }
            childsheetData.Append(r);

            int i = 0;
            foreach (var ent in entities)
            {
                Row row = new Row();
                row.RowIndex = (uint)i + 2;

                //loop column by column depending on the entity properties
                int column = 0;
                foreach (var prop in GetProperties(typ))
                {
                    Cell cell = new Cell();

                    cell.CellReference = CreateCellReference(column + 1) + row.RowIndex.ToString();

                    cell.DataType = CellValues.InlineString;
                    InlineString inlineString = new InlineString();
                    Text text = new Text();
                    text.Text = Convert.ToString(prop.GetValue(entities[i]));
                    inlineString.Text = text; //The value you want to put into cell
                    cell.AppendChild(inlineString);

                    //Keep appending cell (column) into the same row
                    row.AppendChild(cell);
                    column++;
                }
                i++;
                //Keep appending row into the data sheet
                childsheetData.Append(row);
            }
            childworksheetPart.Worksheet = new Worksheet(childsheetData);
            return childsheet;
        }
        public List<TModel> ImportData(string fileName)
        {
            List<TModel> lst = new List<TModel>();
            var results = GetAllWorksheets(fileName);
            lst = GetDataToList(fileName, results[0].Name);
            return lst;
        }
        private List<TModel> GetDataToList(string filePath, string sheetName)
        {
            List<TModel> resultList = new List<TModel>();

            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                Sheet sheet = null;
                WorksheetPart wsPart = null;

                // Find the worksheet.                
                if (sheetName == "")
                {
                    sheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();
                }
                else
                {
                    sheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();
                }
                if (sheet != null)
                {
                    // Retrieve a reference to the worksheet part.
                    wsPart = (WorksheetPart)(wbPart.GetPartById(sheet.Id));
                }
                if (wsPart == null)
                {
                    throw new Exception("No worksheet.");
                }
                else
                {
                    //List to hold custom column names for mapping data to columns (index-free).
                    var columnNames = new List<string>();

                    //List to hold column address letters for handling empty cell issue (handle empty cell issue).
                    var columnLetters = new List<string>();

                    //Iterate cells of custom header row.
                    foreach (Cell cell in wsPart.Worksheet.Descendants<Row>().ElementAt(0))
                    {
                        //Get custom column names.
                        //Remove spaces, symbols (except underscore), and make lower cases and for all values in columnNames list.                    
                        columnNames.Add(Regex.Replace(GetCellValue(document, cell), @"[^A-Za-z0-9_]", ""));

                        //Get built-in column names by extracting letters from cell references.
                        columnLetters.Add(GetColumnAddress(cell.CellReference));
                    }

                    //Used for sheet row data to be added through delegation.                
                    var rowData = new List<object>();

                    //Do data in rows
                    string cellLetter = string.Empty;

                    foreach (var row in GetUsedRows(document, wsPart))
                    {
                        rowData.Clear();

                        //Iterate through prepared enumerable.
                        foreach (var cell in GetCellsForRow(row, columnLetters))
                        {
                            rowData.Add(GetCellValue(document, cell));
                        }

                        TModel resultObject = ConvertToObject(rowData, columnNames);
                        //Calls the delegated function to add it to the collection.
                        resultList.Add(resultObject);
                    }
                }
            }
            return resultList;
        }
        public List<Sheet> GetAllWorksheets(string fileName)
        {
            Sheets theSheets = null;

            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                theSheets = wbPart.Workbook.Sheets;
                document.Close();
            }
            return theSheets.OfType<Sheet>().ToList();
        }
        private List<PropertyInfo> GetProperties()
        {
            List<PropertyInfo> propList = new List<PropertyInfo>();
            foreach (var prop in typeof(TModel).GetProperties())
            {
                if (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(Guid?))
                {
                    continue;
                }
                if (prop.PropertyType.GetTypeInfo().IsValueType || prop.PropertyType == typeof(string))
                {
                    propList.Add(prop);
                }
            }
            return propList;
        }
        private List<PropertyInfo> GetProperties(Type model)
        {
            List<PropertyInfo> propList = new List<PropertyInfo>();
            foreach (var prop in model.GetProperties())
            {
                if (prop.PropertyType == typeof(Guid) || prop.PropertyType == typeof(Guid?))
                {
                    continue;
                }
                if (prop.PropertyType.GetTypeInfo().IsValueType || prop.PropertyType == typeof(string))
                {
                    propList.Add(prop);
                }
            }
            return propList;
        }
        private string ExcelColumnIndexToName(int Index)
        {
            string range = string.Empty;
            if (Index < 0) return range;
            int a = 26;
            int x = (int)Math.Floor(Math.Log((Index) * (a - 1) / a + 1, a));
            Index -= (int)(Math.Pow(a, x) - 1) * a / (a - 1);
            for (int i = x + 1; Index + i > 0; i--)
            {
                range = ((char)(65 + Index % a)).ToString() + range;
                Index /= a;
            }
            return range;
        }
        private Cell CreateTextCell(string header, UInt32 index, string text)
        {
            Cell c = new Cell { DataType = CellValues.InlineString, CellReference = header + index };
            InlineString istring = new InlineString();
            Text t = new Text { Text = text };
            istring.Append(t);
            c.Append(istring);
            return c;
        }
        private string CreateCellReference(int column)
        {
            string result = string.Empty;
            //First is A
            //After Z, is AA
            //After ZZ, is AAA
            char firstRef = 'A';
            uint firstIndex = (uint)firstRef;
            int mod = 0;

            while (column > 0)
            {
                mod = (column - 1) % 26;
                result += (char)(firstIndex + mod);
                column = (column - mod) / 26;
            }

            return result;
        }
        private TModel ConvertToObject(List<object> rowData, IList<string> columnNames)
        {
            TModel obj = new TModel();

            foreach (var item in columnNames)
            {
                var pi = typeof(TModel).GetProperty(item);
                if (pi != null)
                {
                    string stringVal = Convert.ToString(rowData[columnNames.IndexOf(item)]);
                    if (IsNullableType(pi.PropertyType) && string.IsNullOrEmpty(stringVal) || string.IsNullOrEmpty(stringVal))
                    {
                        continue;
                    }

                    var propertyType = IsNullableType(pi.PropertyType) ? Nullable.GetUnderlyingType(pi.PropertyType) : pi.PropertyType;
                    if (!(propertyType.GetTypeInfo().IsValueType || propertyType == typeof(string)))
                    {
                        continue;
                    }

                    object propVal = TypeDescriptor.GetConverter(propertyType).ConvertFromInvariantString(stringVal);
                    pi.SetValue(obj, propVal);
                }
            }
            return obj;
        }
        private string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            if (cell == null) return null;
            string value = cell.InnerText;

            //Process values particularly for those data types.
            if (cell.DataType != null)
            {
                switch (cell.DataType.Value)
                {
                    //Obtain values from shared string table.
                    case CellValues.SharedString:
                        var sstPart = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                        value = sstPart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                        break;

                    //Optional boolean conversion.
                    case CellValues.Boolean:
                        var booleanToBit = "Y";
                        if (booleanToBit != "Y")
                        {
                            value = value == "0" ? "FALSE" : "TRUE";
                        }
                        break;
                }
            }
            return value;
        }
        private IEnumerable<Row> GetUsedRows(SpreadsheetDocument document, WorksheetPart wsPart)
        {
            bool hasValue;
            //Iterate all rows except the first one.
            foreach (var row in wsPart.Worksheet.Descendants<Row>().Skip(1))
            {
                hasValue = false;
                foreach (var cell in row.Descendants<Cell>())
                {
                    //Find at least one cell with value for a row
                    if (!string.IsNullOrEmpty(GetCellValue(document, cell)))
                    {
                        hasValue = true;
                        break;
                    }
                }
                if (hasValue)
                {
                    //Return the row and keep iteration state.
                    yield return row;
                }
            }
        }
        private IEnumerable<Cell> GetCellsForRow(Row row, List<string> columnLetters)
        {
            int workIdx = 0;
            foreach (var cell in row.Descendants<Cell>())
            {
                //Get letter part of cell address.
                var cellLetter = GetColumnAddress(cell.CellReference);

                //Get column index of the matched cell.  
                int currentActualIdx = columnLetters.IndexOf(cellLetter);

                //Add empty cell if work index smaller than actual index.
                for (; workIdx < currentActualIdx; workIdx++)
                {
                    var emptyCell = new Cell() { DataType = null, CellValue = new CellValue(string.Empty) };
                    yield return emptyCell;
                }

                //Return cell with data from Excel row.
                yield return cell;
                workIdx++;

                //Check if it's ending cell but there still is any unmatched columnLetters item.   
                if (cell == row.LastChild)
                {
                    //Append empty cells to enumerable. 
                    for (; workIdx < columnLetters.Count(); workIdx++)
                    {
                        var emptyCell = new Cell() { DataType = null, CellValue = new CellValue(string.Empty) };
                        yield return emptyCell;
                    }
                }
            }
        }
        private string GetColumnAddress(string cellReference)
        {
            //Create a regular expression to get column address letters.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }
        private bool IsNullableType(Type type)
        {
            return type != null && type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        public async Task<List<string>> ReadExcelDocuments(string fileName)
        {
            List<string> columnNames = new List<string>();
            try
            {


                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fs, false))
                    {
                        WorkbookPart workbookPart = doc.WorkbookPart;
                        SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                        SharedStringTable sst = sstpart.SharedStringTable;

                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        Worksheet sheet = worksheetPart.Worksheet;

                        var cells = sheet.Descendants<Cell>();
                        var rows = sheet.Descendants<Row>();


                        //Or...via each row
                        foreach (Row row in rows)
                        {
                            if (row.InnerText == "")
                                break;
                            foreach (Cell c in row.Elements<Cell>())
                            {
                                if ((c.DataType != null) && (c.DataType == CellValues.SharedString))
                                {
                                    int ssid = int.Parse(c.CellValue.Text);
                                    string str = sst.ChildElements[ssid].InnerText;
                                    if (row.RowIndex == "1")
                                        columnNames.Add(str);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return columnNames;

        }
    }
    public class ExcelModelRequest
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string IdNumber { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public List<string> columnNames { get; set; } = new List<string>();
        public List<ExcelModelRequest> excelModelRequests { get; set; } = new List<ExcelModelRequest>();
        public List<DoNotCallRegistry> consumers { get; set; } = new List<DoNotCallRegistry>();
    }
    public class ExcelModelResponse
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string IdNumber { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public bool isApproved { get; set; }
    }
}
