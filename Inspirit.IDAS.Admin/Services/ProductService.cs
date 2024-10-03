using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin.Services
{
    public class ProductService
    {

        IDASDbContext _dbContext;
        public ProductService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<ActionResult<ProductResponse>> ProductDataList(DataTableRequest request)
        {
            ProductResponse response = new ProductResponse();
            try
            {
                //var lst = _dbContext.Products.AsQueryable();
                //int cnt = _dbContext.Products.Count();
                //var flt = lst;
                //if (request.search != null && !string.IsNullOrEmpty(request.search.value))
                //{
                //    flt = _dbContext.Products.Where(t => t.Name == request.search.value);
                //}

                //response.recordsTotal = cnt;
                //response.recordsFiltered = flt.Count();



                //var data = (from s in flt.Skip(request.start).Take(10)
                //            select new ProductDetails
                //            {
                //                Id = s.Id,
                //                 Name = s.Name,                              
                //                 EffectiveDate = DateTime.Today.Date,
                //                 BillingType=s.BillingType,
                //                 UsageType=s.UsageType,
                //                 SearchLimit=s.SearchLimit,
                //                 TimeLimit=s.TimeLimit,


                //            }).ToList();

                //response.data = data;

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public CrudResponse Insert(Product data)
        {
            CrudResponse response = new CrudResponse();

            try
            {
                data.Id = Guid.NewGuid();
                _dbContext.Products.Add(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }

        public CrudResponse Update(Product data)
        {
            CrudResponse response = new CrudResponse();
            try
            {
                _dbContext.Products.Update(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }

        public CrudResponse Delete(Guid id)
        {
            CrudResponse response = new CrudResponse();
            try
            {
                // var data = _dbContext.Products.FirstOrDefault(t => t.Id == id);
                var dat = _dbContext.Products.FirstOrDefault(s => s.Id == id);

                _dbContext.Products.Remove(dat);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public Product View(Guid id)
        {
            Product data = new Product();
            try
            {
                data = _dbContext.Products.Find(id);

            }
            catch (Exception ex)
            {

            }
            return data;
        }

    }
    public class ProductResponse : DataTableResponse<ProductDetails>
    {

    }
    public class ProductDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public DateTime EffectiveDate { get; set; }

        public string BillingType { get; set; } //Credit,Invoice

        public string UsageType { get; set; } // per user/ per search

        public int SearchLimit { get; set; }// For per user --  Limited to 250 searches per user per day 

        public int TimeLimit { get; set; }// For p         
        //public string Text { get; set; }
    }
    //public class ProductCrudResponse
    //{
    //    public string Message { get; set; }

    //    public bool IsSuccess { get; set; }
    //}
}
