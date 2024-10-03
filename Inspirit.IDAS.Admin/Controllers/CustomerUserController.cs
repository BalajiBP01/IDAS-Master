using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Admin.Services;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerUserController : ControllerBase
    {
        CustomerUserService _service;

        public CustomerUserController(CustomerUserService service)
        {
            _service = service;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CustomerUserResponse>> CustomerUserList([FromBody]CustomerUserRequest request)
        {
            return await _service.CustomerUserDataList(request);

        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CrudUserResponse>> Insert([FromBody]CustomerUserVM request)
        {
            CrudUserResponse res = new CrudUserResponse();

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
        public async Task<ActionResult<CrudUserResponse>> Update([FromBody]CustomerUserVM request)
        {

            CrudUserResponse res = new CrudUserResponse();

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
        public async Task<ActionResult<CrudUserResponse>> Delete(Guid id)
        {

            CrudUserResponse res = new CrudUserResponse();

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
        public async Task<ActionResult<CustomerUserVM>> View(Guid id)
        {

            CustomerUserVM res = new CustomerUserVM();

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
        public async Task<ActionResult<CustomerCrudResponse>> UpdateStatus([FromBody]CustomerUserVM cust)
        {

            CustomerCrudResponse res = new CustomerCrudResponse();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {
                res = await _service.UpdateStatusAsync(cust, cust.customeruser.Status);

            }
            return res;
        }
      
    }
}