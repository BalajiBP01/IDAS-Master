using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inspirit.IDAS.Data;

namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationMessageController : ControllerBase
    {
        ApplicationMessageService _service;

        public ApplicationMessageController(ApplicationMessageService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ApplicationmessageResponse>> ApplicationMessageList([FromBody]DataTableRequest request)
        {
            return await _service.ApplicationMessageList(request);

        }

        [HttpPost("[action]")]
        public async Task<ActionResult<CrudResponseMessage>> Insert([FromBody]ApplicationMessage request)
        {

            CrudResponseMessage res = new CrudResponseMessage();

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
        public async Task<ActionResult<CrudResponseMessage>> Update([FromBody]ApplicationMessage request)
        {

            CrudResponseMessage res = new CrudResponseMessage();

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
        public async Task<ActionResult<CrudResponseMessage>> Delete(Guid id)
        {

            CrudResponseMessage res = new CrudResponseMessage();

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
        public async Task<ActionResult<ApplicationMessage>> View(Guid id)
        {

            ApplicationMessage res = new ApplicationMessage();

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
