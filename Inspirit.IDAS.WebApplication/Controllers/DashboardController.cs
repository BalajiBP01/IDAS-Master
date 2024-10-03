using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Inspirit.IDAS.WebApplication
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        DashboardService _service;
        public DashboardController(DashboardService service)
        {
            _service = service;
        }
        
        [HttpGet("action")]
        public async Task<ActionResult<List<DashboardVm>>> GetData(){
            return _service.GetDashboardData();
        }
    }
}