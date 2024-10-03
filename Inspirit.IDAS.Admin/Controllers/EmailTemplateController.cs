
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
    public class EmailTemplateController : ControllerBase
    {
        EmailTemplateService _service;

        public EmailTemplateController(EmailTemplateService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<EmailTemplateResponse>> EmailTemplateList([FromBody]DataTableRequest request)
        {
            return await _service.EmailTemplateList(request);

        }

        [HttpPost("[action]")]
        public async Task<ActionResult<CrudResponseemail>> Insert([FromBody]EmailTemplate request)
        {

            CrudResponseemail res = new CrudResponseemail();

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
        public async Task<ActionResult<CrudResponseemail>> Update([FromBody]EmailTemplate request)
        {

            CrudResponseemail res = new CrudResponseemail();

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
        public async Task<ActionResult<CrudResponseemail>> Delete(Guid id)
        {

            CrudResponseemail res = new CrudResponseemail();

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
        public async Task<ActionResult<EmailTemplate>> View(Guid id)
        {

            EmailTemplate res = new EmailTemplate();

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