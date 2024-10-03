using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication
{
    public class ProductService
    {

        IDASDbContext _IDAScontext;
        public ProductService(IDASDbContext IDAScontext)
        {
            _IDAScontext = IDAScontext;
        }
        public async Task<ProductResponse> GetProducts()
        {
            ProductResponse productResponse = new ProductResponse();
            try
            {
                                   
            }
            catch (Exception ex)
            {

            }
            return productResponse;

        }

        public class ProductResponse
        {
            public List<ProductPackageRate> _productPackageRate = new List<ProductPackageRate>();
            public List<Product> _productlist = new List<Product>();
        }
        public class Product
        {
            public string CategoryName { get; set; }
            public string ProductName { get; set; }
            public string ProductDataTypename { get; set; }
            public int DayLimit { get; set; }

        }
    }
}