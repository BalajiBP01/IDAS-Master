using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;


namespace Inspirit.IDAS.WebApplication.Controllers
{
    [Route("[controller]")]
    public class NewsController : Controller
    {
        NewsService _service;
        public NewsController(NewsService service)
        {
            _service = service;
        }
        
        [SwaggerIgnore]
        [HttpGet("[action]")]
        public ActionResult NewsBlog()
        {
          IEnumerable<NewsBlog>  newsvm = _service.GetNewsData();
            ViewBag.key = newsvm;
           
            return View();
        }
        [SwaggerIgnore]
        [HttpGet("[action]")]
        public ActionResult NewsBlogDetail([FromQuery] Guid Id)
        {
            NewsBlog blog = new NewsBlog();
            blog = _service.GetBlogDetail(Id);
            ViewBag.key = blog;
            return View();
        }
        [SwaggerIgnore]
        [HttpGet("[action]")]
        public ActionResult GetKeywordDetails([FromQuery] string key)
        {
            List<NewsBlog> blog = new List<NewsBlog>();
            blog = _service.GetKeywords(key);
            ViewBag.key = blog;
            return View("NewsBlog");
        }
       
    }


}