using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inspirit.IDAS.Data.IDAS;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Inspirit.IDAS.Data;
using System.Linq;

namespace Inspirit.IDAS.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProFormaInvoiceController : ControllerBase
    {
        ProFormaInvoiceService _service;
        private IHostingEnvironment _hostingEnvironment;
        IDASDbContext _IdasContext;
        public ProFormaInvoiceController(ProFormaInvoiceService service, IHostingEnvironment hostingEnvironment, IDASDbContext IdasContext)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
            _IdasContext = IdasContext;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ProFormaInvoiceDataResponse>> ProFormaInvoiceDataList([FromBody]ProFormaInvoiceDataTableRequest request)
        {
            return await _service.ProFormaInvoiceDataList(request);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ProFormaInvoiceCrudResponses>> Insert([FromBody] ProFormaInvoice request)
        {
            ProFormaInvoiceCrudResponses res = new ProFormaInvoiceCrudResponses();
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
        public bool FinalInvoiceSave([FromBody]ProFormaInvoice data)
        {
            return _service.FinalInvoiceSave(data);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ProFormaInvoiceCrudResponses>> Update([FromBody]ProFormaInvoice request)
        {
            ProFormaInvoiceCrudResponses res = new ProFormaInvoiceCrudResponses();
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
        public async Task<ActionResult<ProFormaInvoiceCrudResponses>> Cancel([FromBody]ProFormaInvoice request)
        {
            ProFormaInvoiceCrudResponses res = new ProFormaInvoiceCrudResponses();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.Cancel(request);
            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<ProFormaInvoiceCrudResponses>> Delete(Guid id)
        {
            ProFormaInvoiceCrudResponses res = new ProFormaInvoiceCrudResponses();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.Delete(id);
            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<ProformaInvoiceLineItem>>> ProformaProductsRemove(Guid id)
        {
            List<ProformaInvoiceLineItem> proformaInvoiceLineItems = new List<ProformaInvoiceLineItem>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                proformaInvoiceLineItems = await _service.ProformaProductsRemove(id);
            }
            return proformaInvoiceLineItems;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<decimal>> ApplicationSetting()
        {
            return await _service.ApplicationSetting();
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<ProFormaReport>> View(Guid id)
        {
            ProFormaReport res = new ProFormaReport();
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
        [HttpGet("[action]")]
        public async Task<ActionResult<List<ProFormaInvoiceBulkEmail>>> GetProFormaInvoiceBulkEmail(string styear, string stmonth)
        {
            List<ProFormaInvoiceBulkEmail> res = new List<ProFormaInvoiceBulkEmail>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.GetProFormaInvoiceBulkEmail(styear, stmonth);
            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<ProFormaInvoice>> RatesAdd()
        {
            return await _service.RatesAdd();
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            return await _service.GetCustomers();
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<Customer>> GetCustomerById(Guid customerid)
        {
            return await _service.GetCustomerById(customerid);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<ProductVmodel>>> GetProducts()
        {
            return await _service.GetProducts();
        }
        [HttpGet("[action]")]
        public string CreatePayment(Guid invoiceId)
        {
            _service.CreatePayment(invoiceId, "");
            return "Payment Done";
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<string>> GetPDFFile([FromBody]EmailProperty emailProperty)
        {
            string Message = "";
            try
            {
                string folderName = "ImagesforMail";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);

                var attach = _IdasContext.InvoiceAttachments.Where(t => t.InvoiceId == emailProperty.id).FirstOrDefault();

                string time = string.Empty;

                double utctime = (DateTime.Now.ToUniversalTime() - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).TotalSeconds;
                if (utctime.ToString().Contains('.'))
                {
                    time = utctime.ToString().Replace(".", "");
                }
                else
                    time = utctime.ToString();

                string fullPath = Path.Combine(newPath, emailProperty.fileName.Replace(".pdf", "_") + time + ".pdf");
               
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                else
                {
                    if (System.IO.File.Exists(fullPath))
                    {

                        System.IO.File.Delete(fullPath);
                    }
                }
                emailProperty.base64 = attach.Attachment;
                byte[] imageBytes = Convert.FromBase64String(emailProperty.base64);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                using (FileStream fs = new FileStream(fullPath, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 5f, 6f, 28f, 2f);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    image.ScaleToFit(600, 950);
                    document.Open();
                    //document.Add(image); document.Add(image);
                    document.Add(image);
                    document.Close();
                }
                Message = await _service.SendMail(emailProperty.toMail, fullPath, emailProperty.id);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Message;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<string>> SavePdfFile([FromBody]EmailProperty emailProperty)
        {
            string Message = "";
            try
            {
                InvoiceAttachment attachment = new InvoiceAttachment();
                var attexists = _IdasContext.InvoiceAttachments.Where(t => t.InvoiceId == emailProperty.id).FirstOrDefault();
                if (attexists == null)
                {
                    attachment.Id = Guid.NewGuid();
                    attachment.InvoiceId = emailProperty.id;
                    attachment.InvoiceNumber = emailProperty.InvoiceNumber;
                    attachment.Attachment = emailProperty.base64;
                    Message = "Mail has been saved";
                    _IdasContext.InvoiceAttachments.Add(attachment);
                }
                else
                {
                    attexists.Attachment = emailProperty.base64;
                    _IdasContext.InvoiceAttachments.Update(attexists);
                }
                Message = "Mail has been saved";
                _IdasContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Message;
        }

    }
}