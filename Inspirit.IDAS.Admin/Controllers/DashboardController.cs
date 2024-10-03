using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        DashboardService _service;
        public DashboardController(DashboardService service)
        {
            _service = service;
        }
       
        [HttpGet("[action]")]
        public async Task<ActionResult<CustomerCount>> GetCustCount()
        {
            return  await _service.GetCustomerData();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<CustomerUserCount>> GetCustuserCount()
        {
            return await _service.GetCustomerUserData();
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<DonotCallRegCount>> GetDonotCallCount()
        {
            return await _service.GetDonotCallregCount();
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<CustomerLog>>> CustomerLog()
        {
            return await _service.GetCustomerLogs();
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<ContactUsCount>> GetContactus()
        {
            return await _service.GetContactusCount();
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<PaymentCount>> GetPaymentdetail()
        {
            return await _service.GetPaymentDet();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<InvoiceCount>> GetInvoice()
        {
            return await _service.GetInvoiceDetails();
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<SubscriptionCount>> GetSubscriptiondet()
        {
            return await _service.GetSubActiveCount();
        }

    }
}