using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Admin.Services;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inspirit.IDAS.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        ProductServices _productServices;
        public ProductController(ProductServices ProductServices)
        {
            _productServices = ProductServices;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ProductResponse>> ProductDetailList([FromBody]DataTableRequest request)
        {
            return await _productServices.ProductDetailList(request);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ProductCrudResponse>> ProductsInsert([FromBody]Product prod)
        {
            ProductCrudResponse res = new ProductCrudResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _productServices.ProductsInsert(prod);
            }
            return res;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<ProductCrudResponse>> ProductsUpdate([FromBody]Product prod)
        {
            ProductCrudResponse res = new ProductCrudResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _productServices.ProductsUpdate(prod);
            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<ProductCrudResponse>> ProductsDelete(Guid id)
        {
            ProductCrudResponse res = new ProductCrudResponse();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _productServices.ProductsDelete(id);
            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<ProductDetails>> ProductsView(Guid id)
        {
            ProductDetails res = new ProductDetails();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                res = await _productServices.ProductsView(id);
            }
            return res;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<List<ProductPackageRate>>> ProductPackageRatesRemove(Guid id)
        {
            List<ProductPackageRate> productPackageRates = new List<ProductPackageRate>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                productPackageRates = await _productServices.ProductPackageRatesRemove(id);
            }
            return productPackageRates;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<Product>> ProductsAdd()
        {
            Product pr = new Product();
            pr.Name = "";
            pr.PackageRates = new List<ProductPackageRate>();
            pr.UsageType = "";
            return pr;
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<ProductDetails>> GetServices()
        {
            ProductDetails product = new ProductDetails();
            product = await _productServices.GetServices();
            return product;
        }
    }
}