using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Inspirit.IDAS.Admin
{
    public class ProdservService
    {
        IDASDbContext _dbContext;
        public ProdservService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ActionResult<ServiceDataResponse>> ServiceDataList(DataTableRequest request)
        {
            ServiceDataResponse response = new ServiceDataResponse();
            try
            {

                var lst = _dbContext.Services.AsQueryable();
                var data = await (from s in lst.Skip(request.start).Take(10)
                                  select new ServiceData
                                  {
                                      Id = s.Id,
                                      Name = s.Name,
                                      Code = s.Code,
                                      IsActive = s.IsActive
                                  }).ToListAsync();

                response.data = data;
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<ServiceCrudResponse> Insert(Service data)
        {
            ServiceCrudResponse response = new ServiceCrudResponse();
            try
            {
                var services = await _dbContext.Services.FirstOrDefaultAsync(s => s.Name == data.Name);
                if (services != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Service name already exists";
                    return response;
                }
                data.Id = Guid.NewGuid();
                data.CreatedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                data.CreatedBy = Guid.NewGuid();
               
               
                _dbContext.Services.Add(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ServiceCrudResponse> Update(Service data)
        {
            ServiceCrudResponse response = new ServiceCrudResponse();
            try
            {
                var ser = await _dbContext.Services.Where(m => m.Id == data.Id).FirstOrDefaultAsync();

                ser.Name = data.Name;
                ser.Code = data.Code;
                ser.IsActive = data.IsActive;

                ser.ModifiedBy = Guid.NewGuid();
                ser.ModifiedDate = DateTime.Now;

                _dbContext.Services.Update(ser);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
                
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
                return response;
            }
            return response;
        }
        public async Task<ServiceCrudResponse> Delete(Guid id)
        {
            ServiceCrudResponse response = new ServiceCrudResponse();
            try
            {
                var data = await _dbContext.Services.FirstOrDefaultAsync(t => t.Id == id);
                _dbContext.Services.Remove(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<Service> View(Guid id)
        {
            Service data = new Service();
            try
            {
                data = await _dbContext.Services.FindAsync(id);
            }
            catch (Exception ex)
            {

            }
            return data;
        }
    }
    public class ServiceData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }
    public class ServiceCrudResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
    public class ServiceDataResponse : DataTableResponse<ServiceData> { }
}