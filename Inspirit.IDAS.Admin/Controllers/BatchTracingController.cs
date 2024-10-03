using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inspirit.IDAS.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchTracingController : ControllerBase
    {

        private IHostingEnvironment _hostingEnvironment;
        BatchTracingService _service;
        IDASDbContext _dbContext;
        private IConfiguration _configuration;

        ExcelDataPort<ExcelModelResponse> _excelDataImport = new ExcelDataPort<ExcelModelResponse>();
        public BatchTracingController(IHostingEnvironment hostingEnvironment, IDASDbContext iDASDbContext, BatchTracingService batchTracingService, IConfiguration config )
        {

            _hostingEnvironment = hostingEnvironment;
            _dbContext = iDASDbContext;
            _service = batchTracingService;
            _configuration = config;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<BatchTracingDataTableResponse>> GetBatchTrace([FromBody]BatchDataTableRequest request)
        {
            return await _service.GetBatchTracingList(request);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<BatchtraceResponse>> UpdateBatchTrace(Guid BatchId, Guid c)
        {
            return await _service.Update(BatchId, BatchId);

        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> RemoveBatchTrace(Guid ID)
        {
            string Message = "";
            try
            {
                string newPath = _configuration.GetSection("BatchTracePath").GetSection("BatchExcelPath").Value;
                string filename = await _service.RemoveBatchTrace(ID);
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
        public async Task<ActionResult<IActionResult>> DownloadExcel(Guid id)
        {
            var filename = _dbContext.BatchTraces.Where(t => t.ID == id).FirstOrDefault();
            var memory = new MemoryStream();
            try
            {
                string newPath =  _configuration.GetSection("BatchTracePath").GetSection("BatchExcelPath").Value;
                string filepath = Path.Combine(newPath, filename.OutPutFileName);
                using (var stream = new FileStream(filepath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
            }
            catch (System.Exception ex) { }
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename.OutPutFileName);
        }
    }
}