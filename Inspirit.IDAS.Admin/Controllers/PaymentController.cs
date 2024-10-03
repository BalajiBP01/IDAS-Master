using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspirit.IDAS.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        PaymentService _service;
        public PaymentController(PaymentService service)
        {
            _service = service;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<PaymentResponse>> PaymentList([FromBody]PaymentDataTableRequest request)
        {
            return await _service.PaymentList(request);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CrudResponsePayment>> Insert([FromBody]Payment request)
        {
            CrudResponsePayment res = new CrudResponsePayment();
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
        public async Task<ActionResult<CrudResponsePayment>> Update([FromBody]Payment request)
        {
            CrudResponsePayment res = new CrudResponsePayment();
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
        public async Task<ActionResult<CrudResponsePayment>> Delete(Guid id)
        {
            CrudResponsePayment res = new CrudResponsePayment();
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
        public async Task<ActionResult<List<LookupData>>> GetPaymentLookupvalues()
        {
            List<LookupData> res = new List<LookupData>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.GetPaymentLookupvalues();
            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<Payment>> View(Guid id)
        {
            Payment res = new Payment();
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
    }
}
