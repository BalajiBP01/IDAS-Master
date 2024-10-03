using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inspirit.IDAS.Admin.Services;
using Inspirit.IDAS.Data;

namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {

        SubscriptionService _SubscriptionService;
        public SubscriptionController(SubscriptionService SubscriptionService)
        {
            _SubscriptionService = SubscriptionService;
        }
        [HttpGet("[action]")]
        public SubscriptionResponse CreateInvoice(Guid subscriptionId)
        {
            SubscriptionResponse res = new SubscriptionResponse();
            res = _SubscriptionService.CreateInvoice(subscriptionId);
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<SubscriptionDataTableRespone>> GetSubscriptionList([FromBody] SubscriptionDataTableRequest req)
        {
            return await _SubscriptionService.SubscriptionList(req);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<SubscriptionVm>> GetSubscription(Guid id)
        {
            SubscriptionVm subsc = new SubscriptionVm();
            subsc = await _SubscriptionService.GetSubscriptionDetail(id);
            return subsc;
        }         
        [HttpPost("[action]")]
        public SubscriptionResponse AddSubscription([FromBody] ProductsVm products)
        {
            return _SubscriptionService.CreateSubscription(products);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            List<Customer> list = new List<Customer>();
            list = await _SubscriptionService.GetCustomerList();
            return list;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<Customer>> GetCustomerById(Guid customerid)
        {
            return await _SubscriptionService.GetCustomerById(customerid);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<ProductPackageRate>>> GetProductList(Guid id)
        {
            List<ProductPackageRate> res = new List<ProductPackageRate>();
            res = await _SubscriptionService.GetProductsRates(id);
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<Product>>> GetProducts(Guid id)
        {
            List<Product> res = new List<Product>();
            res = await _SubscriptionService.GetProducts(id);
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<List<ProductsVm>>> GetData()
        {
            List<ProductsVm> response = new List<ProductsVm>();
            try
            {
                response = await _SubscriptionService.GetSubscription();
            }
            catch (Exception ex) { }
            return response;
        }
    }
}