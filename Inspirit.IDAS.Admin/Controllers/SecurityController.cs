using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspirit.IDAS.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        SecurityService _securityService;
        public SecurityController(SecurityService securityService)
        {
            _securityService = securityService;
        }
        [HttpPost("[action]")]

        public async Task<ActionResult<LoginReponse>> Login([FromBody]LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                LoginReponse res = await _securityService.AuthUser(request);
                return res;
            }

        }

        [HttpGet("[action]")]
        public async Task<ActionResult<List<Menu>>> GetUserMenu(Guid Id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {

                var res = await _securityService.GetUserMenu(Id);
                return res;
            }

        }
        [HttpPost("[action]")]
        public async Task<ActionResult<List<SearchCustomerResponse>>> GetSearchCustomers([FromBody]SearchCustomerRequest request)
        {
            return await _securityService.GetSearchCustomers(request);
        }
    }
}