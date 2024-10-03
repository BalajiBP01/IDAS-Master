
using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.Production;
using Inspirit.IDAS.ESData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication
{
    [Route("api/[controller]")]
    [ApiController]
    public class TracingController : ControllerBase
    {
        CustomerLogService _CustomerLogService;
        TracingService _Service;
        EmailService _emailService;
        IDASDbContext _dbcontext;
        DAL dAL = new DAL();
 

   
        public TracingController(TracingService Service, CustomerLogService CustomerLogService, IConfiguration configuration)
        {
            _Service = Service;
            _CustomerLogService = CustomerLogService;

        }

       
        [HttpPost("[action]")]
        public async Task<ActionResult<ConsumerSearchResponse>> TracingConsumerSearch([FromBody]ConsumerSearchRequest request)
        {

            ConsumerSearchResponse res = new ConsumerSearchResponse();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {

                res = await _Service.ConsumerSearch(request);
            }
            return res;
        }

       
        [HttpGet]
        public String GetProvinceOfBirth(string fetch)
        {
            //var query = (from c in AddressData.
            //             where c.Comune == fetch
            //             select c.Provincia).ToList();

            return "";
        }

      
        [HttpPost("[action]")]
        public async Task<ActionResult<CompanySearchResponse>> TracingCommercialSearch([FromBody]CompanySearchRequest request)
        {
            CompanySearchResponse res = new CompanySearchResponse();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {

                res = await _Service.CompanySearch(request);


            }
            return res;
        }

       
        [HttpPost("[action]")]
        public async Task<ActionResult<AddressSearchResponse>> TracingAddressSearch([FromBody]AddressSearchRequest request)
        {
            AddressSearchResponse res = new AddressSearchResponse();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {

                res = await _Service.AddressSearch(request);


            }
            return res;
        }
        //[HttpGet("~/tracingSearchTest/addressSearchResultTest")]
        //url: '/tracingSearchTest/addressSearchResultTest',
        [Route("tracingSearchTest/addressSearchResultTest")]

        
        [HttpGet]

        public async Task<ActionResult<AddressSearchResponse>> TracingAutocomplete([FromBody]AddressSearchRequest request)
        {
            AddressSearchResponse res = new AddressSearchResponse();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {

                res = await _Service.AddressSearch(request);


            }
            return res;
        }

        //[HttpGet("[action]")]
        //public async Task<ActionResult<int>> GetPoints(Guid userId, Guid customerId, bool IsXDS)
        //{
        //    if (IsXDS == false)
        //    {
        //        return _CustomerLogService.GetUserCredits(userId, customerId, "Tracing");
        //    }
        //    else
        //    {
        //        return 100;
        //    }
        //}


        [HttpGet("[action]")]
        public async Task<ActionResult<int>> GetPoints(Guid userId, Guid customerId)
        {
            return _CustomerLogService.GetUserCredits(userId, customerId, "Tracing");
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<int>> GetCustomerIsRestricted(Guid Id)
        {
            return  _Service.GetIsRestrictedCustomer(Id);
            //return dAL.getIsRestrictedCustomer(Id);
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<int>> GetCustomerUserIsRestricted(Guid Id)
        {
            //return dAL.getIsRestrictedCustomerUser(Id);
            return _Service.GetIsRestrictedCustomerUser(Id);
        }


        // Krishna start
        /// <summary>
        /// Added by Krishna
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns> Enquiry Reason </returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<string>> GetCustomerEnquiryReason(Guid id)
        {
            return await _Service.GetCustomerEnquiryReason(id);
        }

        /// <summary>
        /// Added by Krishna
        /// </summary>
        /// <returns>list of Enquiry Reason LookupData</returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<List<LookupData>>> GetEnquiryReasonLookupDatas()
        {
            return await _Service.GetEnquiryReasonLookupDatas();
        }

        // Krishna end

        //public async Task<ActionResult<int>> GetPoints(Guid userId, Guid customerId)
        //{
        //    var x = _CustomerLogService.GetUserCredits(userId, customerId, "Tracing");
        //    if (x < 250)
        //    {
        //        //send email 
        //        //EmailService emailService = new EmailService();
        //        await _emailService.SendEmailAsync("oniel@tsar.co.za", "Over Limit Searches", "The User X" + HttpContext.User.ToString(), null, null, null, false, "test");
        //        //await _emailService.SendEmailAsync(usr.Email, subject, message, null, null, null, false, "Logindetail");
        //    }
        //    return x;
        //}

    }
}
