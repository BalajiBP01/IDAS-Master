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

namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadGenerationController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        LeadGenerationService _service;
        IDASDbContext _dbContext;
        private IConfiguration _configuration;
        ExcelDataPort<ExcelModelResponse> _excelDataImport = new ExcelDataPort<ExcelModelResponse>();
        public LeadGenerationController(IHostingEnvironment hostingEnvironment, IDASDbContext iDASDbContext, LeadGenerationService leadservice, IConfiguration config)
        {

            _hostingEnvironment = hostingEnvironment;
            _dbContext = iDASDbContext;
            _service = leadservice;
            _configuration = config;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<LeadDataTableResponse>> GetLeadList([FromBody]LeadDataTableRequest request)
        {
            return await _service.GetLeadList(request);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<LeadResponse>> UpdateLead(Guid LeadId, Guid AdminId)
        {
            return await _service.ApproveLead(LeadId, AdminId);

        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> RemoveLeads(Guid ID)
        {
            string Message = "";
            try
            {
                string newPath = _configuration.GetSection("BatchTracePath").GetSection("BatchExcelPath").Value;
                string filename = await _service.RemoveLead(ID);
                string filePath = Path.Combine(newPath, filename);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                Message = "Lead has been cancelled";
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
            var filename = _dbContext.LeadsGenaration.Where(t => t.ID == id).FirstOrDefault();
            var memory = new MemoryStream();
            try
            {
                string newPath = _configuration.GetSection("BatchTracePath").GetSection("BatchExcelPath").Value;
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