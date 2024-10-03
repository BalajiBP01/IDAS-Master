using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.WebApplication;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using static Inspirit.IDAS.WebApplication.ProductService;

namespace Inspirit.IDAS.WebApplication.Controllers
{
    public class ProductsController : Controller
    {
        ProductService _productservice;
        public ProductsController(ProductService productservice)
        {
            _productservice = productservice;
        }
        public IActionResult Home()
        {
            return Redirect("/Home");
        }
   
        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            ProductResponse productResponse = await _productservice.GetProducts();
            return Json(productResponse);
        }
    }
}