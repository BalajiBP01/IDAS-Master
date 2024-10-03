using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Inspirit.IDAS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FullAuditReportController : ControllerBase
    {
        FullAuditReportServices _service;

        public FullAuditReportController(FullAuditReportServices service)
        {
            _service = service;
        }


        [HttpPost("[action]")]
        public async Task<ActionResult<FullAuditDataResponse>> FullAuditDataList([FromBody]FullAuditSearchRequest request)
        {
            return await _service.FullAuditDataList(request);

        }
    
        [HttpPost("[action]")]
        public async Task<ActionResult<CustomerLog>> View(Guid id)
        {

            CustomerLog res = new CustomerLog();

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