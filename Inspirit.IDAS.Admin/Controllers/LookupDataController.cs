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
    public class LookupDataController : ControllerBase
    {
        LookupDataService _service;
        public LookupDataController(LookupDataService service)
        {
            _service = service;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<LookupDataResponse>> LookupDataList([FromBody]DataTableRequest request)
        {
            return await _service.LookupDataList(request);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<CrudResponse>> Insert([FromBody]LookupData request)
        {
            CrudResponse res = new CrudResponse();
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
        public async Task<ActionResult<CrudResponse>> Update([FromBody]LookupData request)
        {
            CrudResponse res = new CrudResponse();
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
        [HttpGet("[action]")]
        public async Task<ActionResult<CrudResponse>> Delete(Guid id)
        {
            CrudResponse res = new CrudResponse();
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
        [HttpGet("[action]")]
        public async Task<ActionResult<LookupData>> View(Guid id)
        {
            LookupData res = new LookupData();
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