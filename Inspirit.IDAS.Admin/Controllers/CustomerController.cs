using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Inspirit.IDAS.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        CustomerService _service;
        private IHostingEnvironment _hostingEnvironment;
        public CustomerController(CustomerService service, IHostingEnvironment hostingEnvironment)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CustomerDetailsResponse>> CustomerDetailList([FromBody]DataTableRequest request)
        {
            return await _service.CustomerList(request);
        }
        [HttpGet("[action]")]
        public string GetName(Guid Id)
        {
            return _service.GetCustomerName(Id);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CustomerCrudResponse>> Insert([FromBody]CustomersVm cust)
        {
            CustomerCrudResponse res = new CustomerCrudResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = _service.Insert(cust);
            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CustomerCrudResponse>> InsertCustomerProduct([FromBody]CustomerProdcutVm cust)
        {
            CustomerCrudResponse res = new CustomerCrudResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.InsertCustomerProduct(cust);
            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CustomerCrudResponse>> UpdateCustomerProduct([FromBody]CustomerProdcutVm cust)
        {
            CustomerCrudResponse res = new CustomerCrudResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.UpdateCustomerProduct(cust);
            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CustomerCrudResponse>> Update([FromBody]CustomersVm cust)
        {
            CustomerCrudResponse res = new CustomerCrudResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = _service.Update(cust);
            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CustomerCrudResponse>> UpdateStatus([FromBody]CustomersVm cust)
        {
            CustomerCrudResponse res = new CustomerCrudResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = _service.UpdateStatus(cust);
            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CustomerCrudResponse>> Delete(Guid id)
        {
            CustomerCrudResponse res = new CustomerCrudResponse();
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
        public async Task<ActionResult<CustomersVm>> View(Guid id)
        {
            CustomersVm res = new CustomersVm();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {

                res = _service.View(id, _hostingEnvironment.WebRootPath.ToString());
            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<CustomerProdcutVm>> ViewCustomerProdcut(Guid id)
        {
            CustomerProdcutVm res = new CustomerProdcutVm();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.ViewCustomerProdcut(id);
            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            return await _service.GetProducts();
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<LookupData>>> GetLookupDatas()
        {
            return await _service.GetLookupDatas();
        }
        // new krishna start
        /// <summary>
        /// Added by Krishna
        /// </summary>
        /// <returns>list of Enquiry Reason LookupData</returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<List<LookupData>>> GetEnquiryReasonLookupDatas()
        {
            return await _service.GetEnquiryReasonLookupDatas();
        }
        // new krishna end

        [HttpGet("[action]")]
        public async Task<ActionResult<CustomerProdcutVm>> GetCustomerProducts()
        {
            return await _service.GetCustomerProducts();
        }
        [HttpPost("[action]")]
        public string deleteFile(string FilePath)
        {
            return _service.FileDelete(FilePath);
        }
        [HttpPost]
        public ActionResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                string fileName = "";
                string fullPath = "";
                string folderName = "Customerdocuments";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                newPath = Path.Combine(newPath, file.FileName.ToString());
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
        [HttpGet("[action]")]
        public async Task<ActionResult<IActionResult>> DownloadPdf(string fileName, Guid Id)
        {
            string folderName = "Customerdocuments";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName, Id.ToString());
            string filepath = newPath + @"\" + fileName;
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
            return File(memory, "application/pdf", fileName);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<string>> GetTabs(Guid Id)
        {
            return await _service.GetTabs(Id);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<string>> AddTabs(string tabs, Guid CustomerId)
        {
            return await _service.AddorUpdateTabs(tabs, CustomerId);
        }

    }
}