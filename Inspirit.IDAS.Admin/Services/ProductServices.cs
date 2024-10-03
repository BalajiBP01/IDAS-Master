using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class ProductServices
    {
        IDASDbContext _dbContext;
        public ProductServices(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ActionResult<ProductResponse>> ProductDetailList(DataTableRequest request)
        {
            ProductResponse response = new ProductResponse();
            try
            {
                var lst = _dbContext.Products.AsQueryable();
                var data = await (from s in lst
                                  select new ProductDetails
                                  {
                                      Id = s.Id,
                                      Name = s.Name,
                                      ProductPackageRates = s.PackageRates,
                                      Service = s.Service,
                                      ServiceId = s.ServiceId,
                                      isActive = s.Status,
                                      UsageType = s.UsageType,
                                      ActivatedDate = s.ActivatedDate
                                  }).ToListAsync();
                response.data = data;
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<ProductCrudResponse> ProductsInsert(Product prod)
        {
            ProductCrudResponse response = new ProductCrudResponse();
            try
            {
                if (prod.Status)
                    prod.ActivatedDate = DateTime.Now;
                else
                    prod.DeactivatedDate = DateTime.Now;
                prod.Id = Guid.NewGuid();
                prod.CreatedOn = DateTime.Now;
                prod.LastUpdatedDate = DateTime.MinValue;
                var prName = await _dbContext.Products.FirstOrDefaultAsync(p => p.Name == prod.Name.Trim());
                if (prName != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Product name already exists";
                    return response;
                }
                _dbContext.Products.Add(prod);
                List<ProductPackageRate> productPackageRates = new List<ProductPackageRate>();
                foreach (var f in prod.PackageRates)
                {
                    f.Id = Guid.NewGuid();
                    f.ProductId = prod.Id;
                }
                productPackageRates.AddRange(prod.PackageRates);
                await _dbContext.ProductPackageRates.AddRangeAsync(productPackageRates);
                await _dbContext.SaveChangesAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ProductCrudResponse> ProductsUpdate(Product prod)
        {
            ProductCrudResponse response = new ProductCrudResponse();
            try
            {
                Product product = await _dbContext.Products.Include(t => t.PackageRates).Where(t => t.Id == prod.Id).FirstOrDefaultAsync();
                Product productduplicate = await _dbContext.Products.Include(t => t.PackageRates).Where(t => t.Id != prod.Id && t.Name.ToUpper() == prod.Name.Trim().ToUpper()).FirstOrDefaultAsync();
                if (productduplicate != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Product name already exists";
                }
                else
                {
                    product.Name = prod.Name;
                    product.ServiceId = prod.ServiceId;
                    product.UsageType = prod.UsageType;
                    product.IsPostpaid = prod.IsPostpaid;
                    product.Code = prod.Code;
                    if (product.Status != prod.Status)
                    {
                        product.Status = prod.Status;
                        if (product.Status)
                            product.ActivatedDate = DateTime.Now;
                        else
                            product.DeactivatedDate = DateTime.Now;
                    }
                    product.LastUpdatedDate = DateTime.Now;
                    var _addRate = prod.PackageRates.
                        Where(pid => pid.ProductId != product.Id).ToList();

                    
                    List<ProductPackageRate> _addRange = new List<ProductPackageRate>();
                    List<ProductPackageRate> _updateRange = new List<ProductPackageRate>();

                    foreach (var pr in _addRate)
                    {
                        pr.Id = Guid.NewGuid();
                        pr.ProductId = product.Id;
                        _addRange.Add(pr);
                    }

                    foreach (var pr in prod.PackageRates.Where(t => t.ProductId == product.Id))
                    {
                        var rate = product.PackageRates.Where(t => t.Id == pr.Id).FirstOrDefault();
                        if (rate != null)
                        {
                            rate.MinLimit = pr.MinLimit;
                            rate.MaxLimit = pr.MaxLimit;
                            rate.UnitPrice = pr.UnitPrice;

                            _updateRange.Add(rate);
                        }
                    }

                    _dbContext.Products.Update(product);
                    if (_updateRange.Count > 0)
                        _dbContext.ProductPackageRates.UpdateRange(_updateRange);

                    if (_addRange.Count > 0)
                        await _dbContext.ProductPackageRates.AddRangeAsync(_addRange);

                    await _dbContext.SaveChangesAsync();
                    response.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ProductCrudResponse> ProductsDelete(Guid id)
        {
            ProductCrudResponse response = new ProductCrudResponse();
            try
            {
                var prod = _dbContext.Products.FirstOrDefault(t => t.Id == id);
                _dbContext.Products.Remove(prod);
                if (prod.PackageRates.ToList().Count > 0)
                    _dbContext.ProductPackageRates.RemoveRange(prod.PackageRates);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<List<ProductPackageRate>> ProductPackageRatesRemove(Guid id)
        {
            List<ProductPackageRate> productPackageRates = new List<ProductPackageRate>();
            try
            {
                var pr = _dbContext.ProductPackageRates.FirstOrDefault(t => t.Id == id);
                if (pr != null)
                {
                    pr.IsDeleted = 2;
                    _dbContext.ProductPackageRates.Update(pr);
                    await _dbContext.SaveChangesAsync();
                    productPackageRates = await _dbContext.ProductPackageRates
                        .Where(prate => prate.IsDeleted == 0
                        && prate.ProductId == pr.ProductId)
                        .OrderBy(prate => prate.MinLimit)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {

            }
            return productPackageRates;
        }
        public async Task<ProductDetails> ProductsView(Guid id)
        {
            ProductDetails product = new ProductDetails();
            try
            {
                var prdctlist = _dbContext.Products.AsQueryable();
                var servicelist = _dbContext.Services.AsQueryable();
                product.productlist = await (from p in prdctlist
                                             where p.Id == id
                                             select new Product
                                             {
                                                 Id = p.Id,
                                                 Name = p.Name,
                                                 Service = p.Service,
                                                 Status = p.Status,
                                                 IsPostpaid= p.IsPostpaid,
                                                 ServiceId = p.ServiceId,
                                                 UsageType = p.UsageType,
                                                 Code = p.Code,
                                                 PackageRates = p.PackageRates
                                                                  .Where(pr => pr.IsDeleted == 0)
                                                                  .OrderBy(pr => pr.MinLimit).ToList()
                                             }).ToListAsync();
            }
            catch (Exception ex)
            {

            }
            return product;
        }
        public async Task<ProductDetails> GetServices()
        {
            ProductDetails product = new ProductDetails();
            try
            {
                product.services = await _dbContext.Services.Where(t=>t.IsActive == true).ToListAsync();
            }
            catch (Exception ex) { }
            return product;
        }
    }
    public class ProductResponse : DataTableResponse<ProductDetails>
    {

    }
    public class ServiceResponse : DataTableResponse<Service>
    {

    }
    public class ProductDetails
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime ActivatedDate { get; set; }
        public List<ProductPackageRate> ProductPackageRates { get; set; } = new List<ProductPackageRate>();
        public List<Service> services { get; set; } = new List<Service>();
        public Service Service { get; set; }
        public Guid ServiceId { get; set; }
        public string UsageType { get; set; } // monthly,yearly,credits    
        public bool isActive { get; set; }
        public List<Product> productlist { get; set; } = new List<Product>();
    }
    public class ProductCrudResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}
