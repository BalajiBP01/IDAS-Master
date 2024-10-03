using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UserDataResponse>> UserDataList([FromBody]DataTableRequest request)
        {
            return await _service.UserDataList(request);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UserCrudResponse>> Insert([FromBody]User request)
        {
            UserCrudResponse res = new UserCrudResponse();

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
        public async Task<ActionResult<UserCrudResponse>> Update([FromBody]User request)
        {
            UserCrudResponse res = new UserCrudResponse();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {
                res = await _service.Update(request);

            }
            return res;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<UserCrudResponse>> Delete(Guid id)
        {
            UserCrudResponse res = new UserCrudResponse();

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
        public async Task<ActionResult<User>> View(Guid id)
        {
            User res = new User();

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
        public async Task<ActionResult<List<Menu>>> GetUserMenu()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {

                var res = await _service.GetDropdownMenu();
                return res;
            }

        }

        
            [HttpGet("[action]")]
        public async Task<ActionResult<UserPermission>> GetPermission([FromQuery] Guid id, string FormName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {

                var res = await _service.GetUserPermission(id, FormName);
                return res;
            }

        }


        [HttpPost("[action]")]
        public async Task<ActionResult<List<UserPermission>>> RemoveUserperission(Guid id)
        {
            List<UserPermission> res = new List<UserPermission>();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {
                res = await _service.PermissionRemove(id);

            }
            return res;
        }



    }
}