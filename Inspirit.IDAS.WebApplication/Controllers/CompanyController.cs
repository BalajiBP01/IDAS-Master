using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data.Production;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using static Inspirit.IDAS.Data.Production.CompanyProfile;

namespace Inspirit.IDAS.WebApplication
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        ComapnyService _service;
        public CompanyController(ComapnyService service)
        {
            _service = service;
        }
        
           [HttpPost("action")]
           public async Task<ActionResult<CompanyProfile>> GetCompanyDetails([FromBody]CompanyRequest request)
           {
                return _service.GetComapnyProfile(request);
           }

        [HttpGet("action")]

        public async Task<ActionResult<CommercialAuditorVm>> GetAuditorDetail([FromQuery] int AuditorId)
        {
            return _service.GetAuditorinfo(AuditorId);
            
        }
        
    }
}

        