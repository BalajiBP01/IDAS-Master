using System;
using System.IO;
using System.Threading.Tasks;
using Inspirit.IDAS.Data.Production;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NSwag.Annotations;

namespace Inspirit.IDAS.WebApplication
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonProfileController : ControllerBase
    {
        ProfileService _service;
        private IHostingEnvironment _hostingEnvironment;
        public PersonProfileController(ProfileService service, IHostingEnvironment hostingEnvironment)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<PersonProfile>> GetProfileDetils([FromBody] ProfileRequest request)
        {
            return _service.GetPersonProfile(request);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<IActionResult>> DownloadPdf([FromBody]string base64)
        {
            var memory = new MemoryStream();
            string fileName = "";
            try
            {
                string folderName = "ExportPdf";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                fileName = "PersonalDetail.pdf";
                string filepath = Path.Combine(newPath, fileName);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                using (FileStream fsstream = new FileStream(filepath, FileMode.Open))
                {
                    await fsstream.CopyToAsync(memory);
                }
                memory.Position = 0;
            }
            catch (System.Exception ex) { }
            return File(memory, "application/pdf", fileName);
        }

        [HttpGet("[action]")]

        public async Task<ActionResult<long>> GetConsumerID([FromQuery] string Idno)
        {
            return _service.GetConsumerId(Idno);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<DirectorShip>> GetDirectorInfo([FromQuery] int directorid)
        {
            return _service.GetDirectorDetail(directorid);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<Endorsement>> GetEndorsementdetail([FromQuery] int Propertydeedid)
        {
            return _service.GetEndorsement(Propertydeedid);
        }
    }
}