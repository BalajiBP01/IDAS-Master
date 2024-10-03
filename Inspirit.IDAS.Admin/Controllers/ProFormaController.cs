using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Mvc;
using Inspirit.IDAS.Data;

namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProFormaController : ControllerBase
    {
        ProFormaInvoiceService _service;
        public ProFormaController(ProFormaInvoiceService service)
        {
            _service = service;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ProFormaInvoiceDataResponse>> ProFormaDataList([FromBody]DataTableRequest request)
        {
            return await _service.ProFormaInvoiceDataList(request);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ProFormaInvoiceCrudResponses>> Insert([FromBody]ProFormaInvoice request)
        {

            ProFormaInvoiceCrudResponses res = new ProFormaInvoiceCrudResponses();

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
        public async Task<ActionResult<ProFormaInvoiceCrudResponses>> Update([FromBody]ProFormaInvoice request)
        {

            ProFormaInvoiceCrudResponses res = new ProFormaInvoiceCrudResponses();

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
                res = _service.Delete(id);

            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<ProFormaInvoice>> View(Guid id)
        {

            ProFormaInvoice res = new ProFormaInvoice();

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
        [HttpGet("[action]")]
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            List<Customer> list = new List<Customer>();
            list = _service.GetCustomerList();
            return list;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<ProductVmodel>>> GetProducts()
        {
            List<ProductVmodel> list = new List<ProductVmodel>();
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
        public string CreatePayment(Guid invoiceId)
        {
            _service.CreatePayment(invoiceId, "");
            return "Payment Done";
        }
    }
}
