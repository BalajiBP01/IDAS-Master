using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        NewsService _service;
        IDASDbContext _context;
        public NewsController(NewsService service, IDASDbContext context)
        {
            _service = service;
        }
        
        [HttpPost("[action]")]
        public async Task<ActionResult<NewsDataTableResponse>> GetNewsList([FromBody]DataTableRequest request)
        {
            return await _service.NewsList(request);

        }
        [HttpGet("[action]")]
        public async Task<ActionResult<News>> View(Guid Id)
        {
            return await _service.View(Id);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<NewsCrudResponse>> Insert([FromBody]News req)
        {
            return await _service.Insert(req);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<NewsCrudResponse>> Update([FromBody]News req)
        {
            return await _service.Update(req);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<NewsCrudResponse>> Detele(Guid Id)
        {
            return await _service.Delete(Id);
        }

    }
}