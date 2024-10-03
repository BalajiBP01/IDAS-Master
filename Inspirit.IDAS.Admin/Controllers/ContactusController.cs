using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Admin.Services;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inspirit.IDAS.Data;

namespace Inspirit.IDAS.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactusController : ControllerBase
    {
        ContactusService _service;

        public ContactusController(ContactusService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ContactusResponse>> ContactusList([FromBody]ContactUsSearchRequest request)
        {
            return await _service.ContactusList(request);

        }

     
        [HttpPost("[action]")]
        public async Task<ActionResult<ContactUs>> View(Guid id)
        {

            ContactUs res = new ContactUs();

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
        public async Task<ActionResult<ContactusResponse>> UpdateReadStatus([FromBody]ContactUs req)
        {

            ContactusResponse res = new ContactusResponse();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else

            {
            _service.UpdateStatus(req);

            }
            return res;
        }
    }
}
