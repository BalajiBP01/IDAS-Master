using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Inspirit.IDAS.WebApplication.Controllers
{
    [Route("[controller]")]
  
    public class TrailUserController : Controller
    {
        TrailUserService _trailUserService;
        public TrailUserController(TrailUserService trailUserService)
        {
            _trailUserService = trailUserService;
        }

        [HttpPost("[action]")]
        public ActionResult TrailUser(TrailUser user)
        {
         
            TrailUser tu = new TrailUser
            {
                ID = Guid.NewGuid(),
                FirstName = user.FirstName,
                Surname = user.Surname,
                EmailAddress = user.EmailAddress,
                 BusinessRegisterNumber = user.BusinessRegisterNumber,
                 MobileNumber = user.MobileNumber,
                 Password = "",
                Date = DateTime.Now
            };
            TempData["value"] = _trailUserService.Insert(tu);

            return Redirect("/Home");
        }
    }
}