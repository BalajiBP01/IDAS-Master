using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.WebApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Inspirit.IDAS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //userSevirce
        UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<UserResponse>> UserList([FromBody]UserDataTableRequest request)
        {
            return await _service.UserDataList(request);

        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CrudUserResponse>> Insert([FromBody]CustomerUser request)
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
        public async Task<ActionResult<CrudUserResponse>> Update([FromBody]CustomerUser request)
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
        public async Task<ActionResult<CustomerUser>> View(Guid id)
        {

            CustomerUser res = new CustomerUser();

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