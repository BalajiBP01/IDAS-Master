using System;
using Inspirit.IDAS.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Inspirit.IDAS.Data.Production;
using Inspirit.IDAS.Data.IDAS;
using NSwag.Annotations;

namespace Inspirit.IDAS.WebApplication.Controllers
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

        [HttpPost("[action]")]
        public async Task<ActionResult<List<ProductPackageRate>>> GetServices()
        {

            List<ProductPackageRate> services = new List<ProductPackageRate>();
            services = await _SubscriptionService.GetServices();

            return services;

        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<SubscriptionLicenceVm>>> GetSubscribtionUsers(Guid guid)
        {
            List<SubscriptionLicenceVm> response = new List<SubscriptionLicenceVm>();
            response = await _SubscriptionService.GetSubscribtionUsers(guid);
            return response;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<string>> AssignLicensetoUsers([FromBody]SubscriptionLicenceRequest subscriptionLicenceRequest)
        {
            List<CustomerUser> response = new List<CustomerUser>();
            string message = await _SubscriptionService.AssignLicencetoUsers(subscriptionLicenceRequest);
            return message;
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



        [HttpPost("[action]")]

        public void AddSubscription([FromBody] ProductsVm products, Guid customerId)
        {

            _SubscriptionService.CreateSubscription(products, customerId);
        }

        [HttpPost("[action]")]

        public async Task<ActionResult<SubscriptionDataTableRespone>> GetSubscriptionList([FromBody] SubDataTableRequest request)
        {
           
            return await _SubscriptionService.SubscriptionList(request);
           // return res;

        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<ProductPackageRate>>> GetProductList(Guid id)
        {
            List<ProductPackageRate> res = new List<ProductPackageRate>();
            res =await _SubscriptionService.GetProducts(id);
            return res;
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<SubscriptionVm>> GetSubscription(Guid id)
        {
            SubscriptionVm subsc = new SubscriptionVm();
            subsc = await _SubscriptionService.GetSubscriptionDetail(id);
            return subsc;
        }
    }
}