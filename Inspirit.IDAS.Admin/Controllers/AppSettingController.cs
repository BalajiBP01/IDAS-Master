using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data.IDAS;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspirit.IDAS.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppSettingController : ControllerBase
    {
        AppSettingService _service;

        public AppSettingController(AppSettingService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<SettingDataResponse>> SettingDataList([FromBody]DataTableRequest request)
        {
            return await _service.SettingDataList(request);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<SettingCrudResponse>> Insert([FromBody]ApplicationSetting request)
        {
            SettingCrudResponse res = new SettingCrudResponse();

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
        public async Task<ActionResult<SettingCrudResponse>> Update([FromBody]ApplicationSetting request)
        {
            SettingCrudResponse res = new SettingCrudResponse();

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
        public async Task<ActionResult<ApplicationSetting>> View(Guid id)
        {
            ApplicationSetting res = new ApplicationSetting();

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