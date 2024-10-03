using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Inspirit.IDAS.Data;

namespace Inspirit.IDAS.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        InvoiceService _service;
        private IHostingEnvironment _hostingEnvironment;
        public InvoiceController(InvoiceService service, IHostingEnvironment hostingEnvironment)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<InvoiceDataResponse>> InvoiceDataList([FromBody]InvoiceDataTableRequest request)
        {
            return await _service.InvoiceDataList(request);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<InvoiceCrudResponses>> Insert([FromBody] Invoice request)
        {

            InvoiceCrudResponses res = new InvoiceCrudResponses();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {
                res = _service.Insert(request);

            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<InvoiceCrudResponses>> Update([FromBody]Invoice request)
        {

            InvoiceCrudResponses res = new InvoiceCrudResponses();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {
                res = _service.Update(request);

            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<InvoiceCrudResponses>> Delete(Guid id)
        {

            InvoiceCrudResponses res = new InvoiceCrudResponses();

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
        [HttpGet("[action]")]
        public async Task<ActionResult<InvoiceCrudResponses>> Cancel(Guid id)
        {
            InvoiceCrudResponses res = new InvoiceCrudResponses();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = _service.Cancel(id);
            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<TaxInvoiceReport>> View(Guid id)
        {
            TaxInvoiceReport res = new TaxInvoiceReport();
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
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            List<Customer> list = new List<Customer>();
            list = _service.GetCustomerList();
            return list;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<Customer>> GetCustomerById(Guid customerid)
        {
            return await _service.GetCustomerById(customerid);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<CustomerVModel>>> getCustlist()
        {
            List<CustomerVModel> list = new List<CustomerVModel>();
            list = _service.GetCustomers();
            return list;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<ProductVm>>> GetProducts()
        {
            List<ProductVm> list = new List<ProductVm>();
            list = _service.GetProductList();
            return list;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<ProductPackageRate>>> GetProductRates(Guid Id)
        {
            List<ProductPackageRate> productlist = new List<ProductPackageRate>();
            productlist = _service.PackageRates(Id);
            return productlist;
        }
        [HttpPost("[action]")]
        public SubscriptionResponse CreatePayment([FromBody] Guid invoiceId, string paymentType,
            string referance, string comments,
            decimal PaymentAmountReceive, DateTime PaymentReceievedDate)
        {

            return _service.CreatePayment(invoiceId, paymentType,
                referance, comments, PaymentAmountReceive, PaymentReceievedDate);

        }

        [HttpPost("[action]")]
        public SubscriptionResponse CreateCreditNote([FromBody] Creditnote note)
        {

            return _service.CreateCreditNote(note);

        }
        [HttpPost("[action]")]
        public InvoiceCrudResponses CheckExistingInvoiceId([FromBody] Guid invoiceId)
        {
            InvoiceCrudResponses vrify = _service.CheckExistingInvoiceId(invoiceId);
            if (vrify.IsSuccess == false)
            {
                vrify.IsSuccess = false;

                return vrify;
            }
            else
            {
                vrify.IsSuccess = true;
                return vrify;
            }


        }
        [HttpPost("[action]")]
        public InvoiceCrudResponses CheckExistingCreditNote([FromBody] Guid invoiceId)
        {
            InvoiceCrudResponses vrify = _service.CheckExistingCreditNote(invoiceId);
            if (vrify.IsSuccess == false)
            {
                vrify.IsSuccess = false;

                return vrify;
            }
            else
            {
                vrify.IsSuccess = true;
                return vrify;
            }


        }
        [HttpGet("[action]")]
        public ProductPackageRate productRate([FromQuery] Guid id)
        {
            ProductPackageRate rate = new ProductPackageRate();
            rate = _service.rate(id);
            return rate;

        }
        [HttpPost("[action]")]
        public string MonthlyInvoice([FromBody] InvoiceGenerationVm req)
        {
            string res = _service.GenerateMonthlyInvoices(req);
            return res;
        }
        [HttpGet("[action]")]
        public string GetValtAmount()
        {
            return _service.GetVat();

        }
        [HttpGet("[action]")]
        public List<CustomerVModel> GetValidCustomerlist([FromQuery]string year, string month)
        {
            return _service.getValidCustomers(year, month);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<InvoiceBulkEmail>>> GetInvoiceBulkEmail(string styear, string stmonth)
        {
            List<InvoiceBulkEmail> res = new List<InvoiceBulkEmail>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.GetInvoiceBulkEmail(styear, stmonth);
            }
            return res;
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


                string time = string.Empty;

                double utctime = (DateTime.Now.ToUniversalTime() - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).TotalSeconds;
                if (utctime.ToString().Contains('.'))
                {
                    time = utctime.ToString().Replace(".","");
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
                byte[] imageBytes = Convert.FromBase64String(emailProperty.base64);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                using (FileStream fs = new FileStream(fullPath, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 5f, 6f, 28f, 2f);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    image.ScaleToFit(600, 950);
                    document.Open();
                    document.Add(image);
                    document.Close();
                }
                Message = await _service.SendMail(emailProperty.toMail, fullPath, emailProperty.ispaid, emailProperty.id, emailProperty.istaxinv);
              
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Message;
        }
    }
}