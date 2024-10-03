using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inspirit.IDAS.Data;
using NSwag.Annotations;

namespace Inspirit.IDAS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        InvoiceService _service;

        public InvoiceController(InvoiceService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<InvoiceDataResponse>> InvoiceDataList([FromBody]InvDataTableRequest request)
        {
            return await _service.InvoiceDataList(request);

        }
        [HttpGet("[action]")]
        public async Task<ActionResult<InvoiceVm>> InvoiceDataListdata(Guid id)
        {
            return await _service.InvoiceDataListdata(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<Invoice>> View(Guid id)
        {

            Invoice res = new Invoice();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {
                res = _service.View(id);

            }
            return res;
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
        public PaymentResponse CreatePayment([FromBody] Guid invoiceId)
        {
            PaymentResponse res = new PaymentResponse();
            res =  _service.CreatePayment(invoiceId, "");
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



        [HttpGet("[action]")]
        public ProductPackageRate productRate([FromQuery] Guid id)
        {
            ProductPackageRate rate = new ProductPackageRate();
            rate = _service.rate(id);
            return rate;

        }
    }
}