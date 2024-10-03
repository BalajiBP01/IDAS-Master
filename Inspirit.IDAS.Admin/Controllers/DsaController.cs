using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using Inspirit.IDAS.Data;
using System.Collections.Generic;

namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DsaController : Controller
    {
        DsaService _service;
        private IHostingEnvironment _hostingEnvironment;
        public DsaController(DsaService service, IHostingEnvironment hostingEnvironment)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CrudDsaResponse>> Insert([FromBody]DataServicesAgreement request)
        {
            CrudDsaResponse res = new CrudDsaResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.Insert(request);
            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CrudDsaResponse>> Update([FromBody]DataServicesAgreement request)
        {
            CrudDsaResponse res = new CrudDsaResponse();
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
        [HttpPost("[action]")]
        public async Task<ActionResult<CrudDsaResponse>> Delete(Guid id)
        {
            CrudDsaResponse res = new CrudDsaResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = _service.Delete(id);
            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<DataServicesAgreement>> View(Guid id)
        {
            DataServicesAgreement res = new DataServicesAgreement();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.View(id);
            }
            return res;
        }
        [HttpPost]
        public ActionResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = webRootPath.Replace(webRootPath, @"D:\DSADocuments");

                if (file.Length > 0)
                {
                    string fullPath = Path.Combine(newPath, file.Name);
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return Json("Imported Successfully");
            }
            catch { return Json("Imported UnSuccessfully"); }

        }
        [HttpPost("[action]")]
        public async Task<ActionResult<DsaResponse>> GetDsaTableList([FromBody]DataTableRequest request)
        {
            return _service.GetDsaTableList(request);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<IActionResult>> DownloadPdf(string fileName)
        {
            string folderName = "uploads";
            string webRootPath = _hostingEnvironment.WebRootPath
                .Replace("Admin", "WebApplication");
            string newPath = Path.Combine(webRootPath, folderName);
            string filepath = Path.Combine(newPath, fileName);
            var memory = new MemoryStream();
            try
            {
                using (var stream = new FileStream(filepath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
            }
            catch (System.Exception ex) { }
            return File(memory, "application/octet-stream", fileName);
        }
    }
}


