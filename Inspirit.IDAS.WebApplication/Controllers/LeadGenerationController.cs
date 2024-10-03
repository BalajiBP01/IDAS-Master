using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data.Production;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Inspirit.IDAS.WebApplication
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadGenerationController : Controller
    {
        public LeadGenerationServices _leadsService;
        private IHostingEnvironment _hostingEnvironment;
        public LeadGenerationController(LeadGenerationServices servce, IHostingEnvironment hosting)
        {
            _leadsService = servce;
            _hostingEnvironment = hosting;
        }

        [HttpPost("[action]")]
        public async Task<LeadsDatatableResponse> GetLeadList([FromBody] DatatableLeadRequest req)
        {
            return await _leadsService.LeadList(req);
        }
       

        [HttpPost("[action]")]
        public async Task<LeadGenerationResponse> GetLeadsCount([FromBody]LeadsRequest request)
        {
            LeadGenerationResponse message = await _leadsService.GetLeads(request);
            return message;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> CancelLead(Guid ID)
        {
            string Message = "";
            try
            {
                string folderName = "ExportExcel";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                string filename = await _leadsService.RemoveLead(ID);
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

        [HttpPost("[action]")]
        public async Task<ActionResult<InvoiceGenResponse>> GenerateProformaInvoice([FromBody]Guid userId, Guid LeadId)
        {
            return await _leadsService.GenerateInvoice(userId, LeadId);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> CheckLeadConfig(Guid customerUserid)
        {
            return await _leadsService.CheckLeadProcessConfig(customerUserid);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<LeadGenerationResponse>> GetLeadInformation(Guid LeadId)
        {
            return await _leadsService.GetLeadsFullData(LeadId);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<LeadGenerationResponse>> GetLeadtablevalue(Guid LeadId)
        {
            return await _leadsService.getLeadTable(LeadId);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<LeadsMessage>> UpdateLeads([FromBody] LeadGenerationResponse req)
        {
            return await _leadsService.Update(req);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<LeadGenerationResponse>> GetLeadsIdNumber(Guid Id)
        {
            return await _leadsService.GetIdno(Id);
        }
    }
}
