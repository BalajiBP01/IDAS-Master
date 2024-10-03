using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inspirit.IDAS.Data;


namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdservController : ControllerBase
    {
        ProdservService _service;
        public ProdservController(ProdservService service)
        {
            _service = service;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ServiceDataResponse>> ServiceDataList([FromBody]DataTableRequest request)
        {
            return await _service.ServiceDataList(request);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ServiceCrudResponse>> Insert([FromBody]Service request)
        {
            ServiceCrudResponse res = new ServiceCrudResponse();
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
        public async Task<ActionResult<ServiceCrudResponse>> Update([FromBody]Service request)
        {
            ServiceCrudResponse res = new ServiceCrudResponse();
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
        public async Task<ActionResult<ServiceCrudResponse>> Delete(Guid id)
        {
            ServiceCrudResponse res = new ServiceCrudResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.Delete(id);
            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<Service>> View(Guid id)
        {
            Service res = new Service();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _service.View(id);
            }
            return res;
        }
    }
}