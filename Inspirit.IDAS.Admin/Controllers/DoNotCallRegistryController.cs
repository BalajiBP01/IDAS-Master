using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NSwag.Annotations;
using Microsoft.AspNetCore.Http;
using Inspirit.IDAS.Data.IDAS;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using Inspirit.IDAS.Data;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;

namespace Inspirit.IDAS.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoNotCallRegistryController : Controller
    {

        private IHostingEnvironment _hostingEnvironment;
        DoNotCallRegistryService _service;
        IDASDbContext _dbContext;
        ExcelDataPort<ExcelModelResponse> _excelDataImport = new ExcelDataPort<ExcelModelResponse>();

        public DoNotCallRegistryController(DoNotCallRegistryService service, IHostingEnvironment hostingEnvironment, IDASDbContext iDASDbContext)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
            _dbContext = iDASDbContext;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<DncrResponse>> GetDoNotCallRegList([FromBody]DonotCallRegistrySearchRequest request)
        {
            return await _service.GetDonotCallRegistry(request);

        }
        [HttpPost("[action]")]
        public async Task<ActionResult<DoNotCallRegistry>> View(Guid id)
        {

            DoNotCallRegistry res = new DoNotCallRegistry();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {
                res = _service.View(id);

            }
            return res;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<CrudResponseDonotCall>> Update([FromBody]DoNotCallRegistry request)
        {

            CrudResponseDonotCall res = new CrudResponseDonotCall();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {
                res = await _service.Update(request);

            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<string>>> GetColumns(string filename)
        {
            List<string> coulmnNames = Excelcolumns(filename);

            return coulmnNames;
        }

        private List<string> Excelcolumns(string filename)
        {
            List<string> coulmnNames = new List<string>();
            try
            {
                string fullPath = _hostingEnvironment.WebRootPath.Replace("wwwroot", @"wwwroot\DonotcallRegExcel\");
                fullPath = fullPath + filename;
                List<string> columnNames = new List<string>();


                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fullPath, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                    string text;
                    foreach (Row r in sheetData.Elements<Row>())
                    {
                        foreach (Cell c in r.Elements<Cell>())
                        {

                            text = (c == null) ? c.InnerText : c.CellValue.Text;
                            if ((c.DataType != null) && (c.DataType == CellValues.SharedString))
                            {
                                text = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(Convert.ToInt32(c.CellValue.Text)).InnerText;

                            }


                            coulmnNames.Add(text);

                        }
                        break;
                    }
                }
            }

            catch (System.Exception ex) { }
            return coulmnNames;
        }



        [HttpPost("[action]")]

        public ActionResult<string> ExcelValidation([FromBody]string filename)
        {
            List<string> lstreqcol = new List<string>() {
                "Name","Surname","EmailId","IdNumber","PhoneNumber","IsApproved"
            };
            List<string> columns = Excelcolumns(filename);



            string strmsg = string.Empty;


            foreach (string str in columns)
            {
                string strreplace = string.Empty;
                strreplace = str.Replace(" ", string.Empty);

                if (lstreqcol.Where(t => t.ToUpper() == strreplace.ToUpper()).Count() == 0)
                {
                    strmsg = "Invalid Excel";
                }
            }

            foreach (string str in lstreqcol)
            {
                string strreplace = string.Empty;
                strreplace = str.Replace(" ", string.Empty);
                if (columns.Where(t => t.ToUpper() == str.ToUpper()).Count() == 0)
                {
                    if (!string.IsNullOrEmpty(strmsg))
                        strmsg += "\n Column:" + str + " required";
                }
            }

            return strmsg;

        }



        [HttpPost("[action]")]

        public ActionResult<List<DoNotCallRegistryVM>> ExcelInsert([FromBody]string filename)
        {

            List<DoNotCallRegistryVM> response = new List<DoNotCallRegistryVM>();

            try
            {
                string fullPath = _hostingEnvironment.WebRootPath.Replace("wwwroot", @"wwwroot\DonotcallRegExcel\");
                fullPath = fullPath + filename;
                List<string> columnNames = new List<string>();
                List<DoNotCallRegistry> lstRegistary = new List<DoNotCallRegistry>();
                int row = 1, col = 1;
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fullPath, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                    string text;
                    DataTable dt = new DataTable();
                    DataRow dr;
                    DoNotCallRegistry Reg;
                    foreach (Row r in sheetData.Elements<Row>())
                    {

                        dr = dt.NewRow();
                        col = 1;
                        foreach (Cell c in r.Elements<Cell>())
                        {
                            if (c.InnerText == null || c.InnerText == "")
                            {
                                break;
                            }
                            else { 
                                text = (c == null) ? c.InnerText : c.CellValue.Text;
                                if ((c.DataType != null) && (c.DataType == CellValues.SharedString))
                                {
                                    text = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(Convert.ToInt32(c.CellValue.Text)).InnerText;

                                }
                                if (row == 1)
                                {
                                    string textupdate = string.Empty;
                                    textupdate = text.Replace(" ", string.Empty);
                                    dt.Columns.Add(textupdate.ToUpper(), typeof(string));
                                }
                                else
                                {
                                    string textupdate = string.Empty;
                                    textupdate = text.Replace(" ", string.Empty);
                                    dr[col - 1] = textupdate;

                                }
                                col++;
                            }
                        }
                        if (row != 1)
                        {

                            Reg = new DoNotCallRegistry();
                            Reg.Id = Guid.NewGuid();
                            Reg.CurrentDate = DateTime.Now;
                            Reg.Name = dr["Name".ToUpper()].ToString();
                            Reg.Surname = dr["Surname".ToUpper()].ToString();
                            Reg.Emailid = dr["Emailid".ToUpper()].ToString();
                            Reg.Idnumber = dr["Idnumber".ToUpper()].ToString();
                            Reg.PhoneNumber = dr["PhoneNumber".ToUpper()].ToString();
                            Reg.IsApproved = true;
                            string strRemarks = IsValid(Reg);
                            if (string.IsNullOrEmpty(strRemarks))
                            {
                                lstRegistary.Add(Reg);
                            }
                            else
                            {
                                DoNotCallRegistryVM d = new DoNotCallRegistryVM();
                                d.EmailId = Reg.Emailid;
                                d.IdNo = Reg.Idnumber;
                                d.IsApproved = Reg.IsApproved.ToString();
                                d.Name = Reg.Name;
                                d.Surname = Reg.Surname;
                                d.PhoneNumber = Reg.PhoneNumber;
                                d.Remarks = strRemarks;
                                response.Add(d);

                            }

                        }
                        row++;
                    }
                }

                if (lstRegistary.Count > 0)
                {
                    var query = lstRegistary.GroupBy(x => x.Idnumber)
                    .Where(g => g.Count() > 1)
                   .Select(y => new { Element = y.Key, Counter = y.Count() })
                   .ToList();

                    if (query != null)
                    {
                        foreach (var id in query)
                        {

                            var found = lstRegistary.FindAll(t => t.Idnumber == id.Element);
                            foreach (var f in found)
                            {
                                DoNotCallRegistryVM d = new DoNotCallRegistryVM();
                                d.EmailId = f.Emailid;
                                d.IdNo = f.Idnumber;
                                d.IsApproved = f.IsApproved.ToString();
                                d.Name = f.Name;
                                d.Surname = f.Surname;
                                d.PhoneNumber = f.PhoneNumber;
                                d.Remarks = "Duplicate Entries Found in Excel For thid Id Number: " + f.Idnumber;
                                response.Add(d);
                            }


                            lstRegistary.RemoveAll(t => t.Idnumber == id.Element);

                        }

                        _dbContext.DoNotCallRegistrys.AddRange(lstRegistary);
                        _dbContext.SaveChanges();
                    }
                }

            }

            catch (System.Exception ex)
            {

            }
            return response;
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                string fileName = "";
                string fullPath = "";
                string folderName = "DonotcallRegExcel";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                newPath = Path.Combine(newPath);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).Name.Trim('"');
                    fileName = file.Name.Trim('"');
                    fullPath = Path.Combine(newPath, fileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return Json("Imported Successfully");
            }
            catch { return Json("Imported UnSuccessfully"); }
        }

        private string IsValid(DoNotCallRegistry d)
        {


            var isadded = _dbContext.DoNotCallRegistrys.Where(m => m.Idnumber == d.Idnumber).FirstOrDefault();

            string message = string.Empty;

            if (isadded != null)
            {
                message = "Id Number exists";
            }
            if (d.Idnumber.Length != 13)
            {
                message += " Invalid Id Number";
            }
            if (d.Name.Length <= 0)
            {
                message += " Invalid Name";
            }
            else
            {
                bool Namevalid = Regex.IsMatch(d.Name, @"^[a-zA-Z0-9 ]+$");
                if (Namevalid == false)
                {
                    message += " Invalid Name";
                }
            }
            if (d.Surname.Length <= 0)
            {
                message += " Invalid Surname";
            }
            else
            {
                bool surnamevalid = Regex.IsMatch(d.Surname, @"^[a-zA-Z0-9 ]+$");
                if (surnamevalid == false)
                {
                    message += " Invalid Surname";
                }
            }
            if (d.PhoneNumber.Length != 10)
            {
                message += " Invalid Phone Number";
            }
            else
            {
                bool phonenumbervalid = Regex.IsMatch(d.PhoneNumber, @"^[0-9]+$");
                if (phonenumbervalid == false)
                {
                    message += " Invalid Phone Number";
                }
            }
            if (d.Emailid.Length > 0)
            {

                bool emailvalid = Regex.IsMatch(d.Emailid, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                if (emailvalid == false)
                {
                    message += " Invalid Email";
                }

            }
            else
            {
                message += " Invalid Email";
            }
            return message;
        }

        private bool VerifyNumericValue(string ValueToCheck)
        {
            Int32 numval;
            bool rslt = false;

            rslt = Int32.TryParse(ValueToCheck, out numval);

            return rslt;
        }

        

    }

    public class DoNotCallRegistryVM
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string IdNo { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public string IsApproved { get; set; }
        public string Remarks { get; set; }
    }
}