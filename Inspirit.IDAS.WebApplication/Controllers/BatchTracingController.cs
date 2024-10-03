using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Data;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Inspirit.IDAS.Data;
using System;
using Inspirit.IDAS.Data.Production;
using NSwag.Annotations;

namespace Inspirit.IDAS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchTracingController : Controller
    {
        IDASDbContext _IDAScontext;
        ExcelDataPort<ExcelModelResponse> _excelDataImport = new ExcelDataPort<ExcelModelResponse>();
        ExcelDataPort<BatchTracingConsumer> _excelDataExport = new ExcelDataPort<BatchTracingConsumer>();
        private IHostingEnvironment _hostingEnvironment;
        public BatchTraceService _batchTraceService;
        public BatchTracingController(BatchTraceService batchTraceService, IHostingEnvironment hostingEnvironment, IDASDbContext IDASDbContext)
        {
            _batchTraceService = batchTraceService;
            _hostingEnvironment = hostingEnvironment;
            _IDAScontext = IDASDbContext;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<Guid>> AddBatchTraces([FromBody]BatchTrace batchTrace)
        {
            return await _batchTraceService.AddBatchTraces(batchTrace);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> CheckBatchProcessConfiguration(Guid customerUserid)
        {
            return await _batchTraceService.CheckBatchProcessConfiguration(customerUserid);
        }
        public async Task<ActionResult<BatchTrace>> View(Guid ID)
        {
            return await _batchTraceService.View(ID);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> RemoveBatchTrace(Guid ID)
        {
            string Message = "";
            try
            {
                string folderName = "ExportExcel";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                string filename = await _batchTraceService.RemoveBatchTrace(ID);
                string filePath = Path.Combine(newPath, filename);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                Message = "Batch has been cancelled";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Message;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<BatchTraceServices>> ExcelValidation(string filename, Guid UserId, Guid CustId)
        {
            BatchTraceServices batchTraceServices = new BatchTraceServices();
            try
            {
                List<ExcelModelResponse> excelModelResponses = new List<ExcelModelResponse>();
                string importFullPath = _hostingEnvironment.WebRootPath.Replace("wwwroot", @"wwwroot\ImportExcel\");
                filename = UserId + filename;
                importFullPath = importFullPath + filename;



                List<string> idnos = _excelDataImport.GetIdNumberforBatch(importFullPath);

                foreach (var id in idnos)
                {
                    ExcelModelResponse res = new ExcelModelResponse();
                    res.IDNumber = id;
                    excelModelResponses.Add(res);
                }

                batchTraceServices = await _batchTraceService.ExcelValidation(excelModelResponses);
            }
            catch (Exception ex) { }
            return batchTraceServices;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<BatchTraceServices>> FetchingData([FromBody]List<string> consumers, string filename, Guid customerid)
        {
            //list of consumers to the list of strings
            return await _batchTraceService.FetchingData(consumers, filename, customerid);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<BatchTraceServices>> PreparingChart([FromBody]BatchTrace batchTrace)
        {
            return await _batchTraceService.PreparingChart(batchTrace);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<string>>> GetIdNos(string filename, string userid)
        {
            List<string> idnos = new List<string>();
            try
            {
                string fullPath =  _hostingEnvironment.WebRootPath.Replace("wwwroot", @"wwwroot\ImportExcel\");
                filename = userid + filename;
                fullPath = fullPath + filename;
                idnos = _excelDataImport.GetIdNumberforBatch(fullPath);
            }
            catch (System.Exception ex) { }
            return idnos;
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<List<string>>> GetColumns(string filename,string userid)
        {
            List<string> coulmnNames = new List<string>();
            try
            {
                string fullPath = _hostingEnvironment.WebRootPath.Replace("wwwroot", @"wwwroot\ImportExcel\");
                filename = userid + filename;
                fullPath = fullPath + filename;
                coulmnNames = await _excelDataImport.ReadExcelDocuments(fullPath);
                if (coulmnNames.Count == 0)
                    coulmnNames.Add("Column name ID Number is required");
            }
            catch (System.Exception ex) { }
            return coulmnNames;
        }
        [HttpPost("[action]"), DisableRequestSizeLimit]
        public ActionResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                string fileName = "";
                string folderName = "ImportExcel";
                string fullPath = "";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    fileName = fileName + file.Name;
                    fullPath = Path.Combine(newPath, fileName);
                    if (!System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return Json("Uploaded Successfully");
            }
            catch (System.Exception ex) { return Json("Uploaded UnSuccessfully :" + ex.Message.ToString()); }
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<IActionResult>> DownloadExcel(Guid id)
        {
            var filename = _IDAScontext.BatchTraces.Where(t => t.ID == id).FirstOrDefault();
            var memory = new MemoryStream();
            try
            {
                string folderName = "ExportExcel";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                string fileName = filename.ID + "#" + filename.FileName;
                string filepath = newPath + @"\" + fileName;

                using (var stream = new FileStream(filepath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                await _batchTraceService.GetConsumer_Update(id);
            }
            catch (System.Exception ex) { }
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename.FileName);
        }
        [HttpPost("[action]")]
        public async Task<BatchTraceServiceResponse> BatchTraceServiceList([FromBody]BatchTracingServiceRequest request)
        {
            return await _batchTraceService.BatchTraceServiceList(request);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<int>> GetPoints([FromBody]Guid userId, Guid customerId)
        {
            return await _batchTraceService.GetCredits(userId, customerId);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<InvoiceGenResponse>> GenerateProformaInvoice([FromBody]Guid userId, Guid batchId)
        {
            return await _batchTraceService.GenerateInvoice(userId, batchId);
        }
       
    }
}