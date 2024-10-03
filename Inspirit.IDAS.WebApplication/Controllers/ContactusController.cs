using System;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.WebApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Inspirit.IDAS.WebApplication.Controllers
{

    [Route("[controller]")]
    public class ContactusController : Controller
    {
        ContactusService _contactservice;
        private readonly GoogleRecaptchaService _googlerecaptchaservice;
        public ContactusController(ContactusService contactservice, GoogleRecaptchaService googleRecaptchaService)
        {
            _contactservice = contactservice;
            _googlerecaptchaservice = googleRecaptchaService;

        }

        [SwaggerIgnore]
        [HttpPost("[action]")]
        public async Task<ActionResult> ContactUs(ContactUsVM contactVM)
        {
            string token = contactVM.Token2;

            var googleRecaptcha = await _googlerecaptchaservice.VerifyResponse(token);
            //googleRecaptcha.score = 0.4;
            if (googleRecaptcha.success == true && googleRecaptcha.score > 0.5)
            {
                ContactUs cs = new ContactUs
                {
                    Id = Guid.NewGuid(),
                    Name = contactVM.Name,
                    Email = contactVM.Email,
                    Subject = contactVM.Subject,
                    Message = contactVM.Message,
                    PhoneNumber = contactVM.PhoneNumber,
                    Business = contactVM.Business,
                    Date = DateTime.Now              

                };
                TempData["valuecontactus"] = _contactservice.Insert(cs);
                if (TempData["valuecontactus"] != null)
                {
                    return Redirect("/ContactSubmitted");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please Contact Administrator";

                return Redirect("/ContactUs");
            }
            return Redirect("/ContactSubmitted");
        }
        [SwaggerIgnore]
        [HttpPost("[action]")]
        public async Task<ActionResult> Complaints(ContactUsVM contactVM)
        {
            string token = contactVM.Token2;
            var googleRecaptcha = await _googlerecaptchaservice.VerifyResponse(token);
            //googleRecaptcha.score = 0.4;
            if (googleRecaptcha.success == true && googleRecaptcha.score > 0.5)
            {
                ContactUs cps = new ContactUs
                {
                    Id = Guid.NewGuid(),
                    Name = contactVM.Name,
                    Email = contactVM.Email,
                    Subject = contactVM.Subject,
                    Message = contactVM.Message,
                    PhoneNumber = contactVM.PhoneNumber,
                    Business = contactVM.Business,
                    Date = DateTime.Now,
                };
                TempData["valuecontactus"] = _contactservice.InsertComplaint(cps, " ");
                if (TempData["valuecontactus"] != null)
                {
                    return Redirect("/ContactSubmitted");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please Contact Administrator";
                return Redirect("/Complaints");
            }
            return Redirect("/ContactSubmitted");
        }
        [SwaggerIgnore]
        [HttpGet("download")]
        public IActionResult GetBlobDownload([FromQuery] string link)
        {
            var net = new System.Net.WebClient();
            var data = net.DownloadData(link);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = "something.bin";
            return File(content, contentType, fileName);
        }
    }
}