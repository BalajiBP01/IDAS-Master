using Inspirit.IDAS.Data;
using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Inspirit.IDAS.Admin
{
    public class DsaService
    {
        IDASDbContext _dbContext;
        private IHostingEnvironment _hostingEnvironment;
        public DsaService(IDASDbContext dbContext, IHostingEnvironment hostingEnvironment)
        {
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
        }
        public DsaResponse GetDsaTableList(DataTableRequest request)
        {
            DsaResponse response = new DsaResponse();
            try
            {
                var lst = _dbContext.DataServicesAgreements.AsQueryable();
                var flt = lst;
                var data = (from s in flt
                            select new DsaList
                            {
                                id = s.Id,
                                version = s.Version,
                                effectiveDate = s.EffectiveDate,
                                fileName = !String.IsNullOrEmpty(s.FilePath) ?
                                         Path.GetFileName(s.FilePath) : "",
                                isPublished = s.IsPublished
                            }).ToList();

                response.data = data;
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<CrudDsaResponse> Insert(DataServicesAgreement data)
        {
            CrudDsaResponse response = new CrudDsaResponse();
            try
            {
                data.Id = Guid.NewGuid();
                var published = _dbContext.DataServicesAgreements
                       .Where(m => m.IsPublished == true).FirstOrDefault();
                if (published != null)
                {
                    if (data.IsPublished)
                    {
                        published.IsPublished = false;
                        _dbContext.DataServicesAgreements.Update(published);
                    }
                }
                else if (!data.IsPublished)
                {
                    response.IsSuccess = false;
                    response.Message = "Atleast one version should be publish";
                    return response;
                }
                _dbContext.DataServicesAgreements.Add(data);
                await _dbContext.SaveChangesAsync();
                response.IsSuccess = true;
                response.Message = "";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<CrudDsaResponse> Update(DataServicesAgreement data)
        {
            CrudDsaResponse response = new CrudDsaResponse();
            try
            {
                if (data.IsPublished)
                {
                    var published = _dbContext.DataServicesAgreements
                        .Where(m => m.IsPublished == true && m.Id != data.Id).FirstOrDefault();
                    if (published != null)
                    {
                        published.IsPublished = false;
                        _dbContext.DataServicesAgreements.Update(published);
                    }
                }
                else if (!data.IsPublished)
                {
                    var published = _dbContext.DataServicesAgreements
                        .Where(m => m.IsPublished == true && m.Id == data.Id).FirstOrDefault();
                    if (published != null)
                    {
                        response.IsSuccess = false;
                        response.Message = "Atleast one version should be publish";
                        return response;
                    }
                }
                _dbContext.DataServicesAgreements.Update(data);
                await _dbContext.SaveChangesAsync();
                response.IsSuccess = true;
                response.Message = "";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public CrudDsaResponse Delete(Guid id)
        {
            CrudDsaResponse response = new CrudDsaResponse();
            try
            {
                var data = _dbContext.DataServicesAgreements.FirstOrDefault(t => t.Id == id);
                if (data.IsPublished)
                {
                    response.IsSuccess = false;
                    response.Message = "Atleast one version should be publish";
                    return response;
                }
                _dbContext.DataServicesAgreements.Remove(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<DataServicesAgreement> View(Guid id)
        {
            DataServicesAgreement data = new DataServicesAgreement();
            try
            {
                data = await _dbContext.DataServicesAgreements
                    .FirstOrDefaultAsync(d => d.Id == id);
            }
            catch (Exception ex) { }
            return data;
        }
    }
    public class DsaResponse : DataTableResponse<DsaList>
    {

    }
    public class DsaList
    {
        public Guid id { get; set; }
        public DateTime effectiveDate { get; set; }
        public int version { get; set; }
        public string termsAndCondition { get; set; }
        public string filepath { get; set; }
        public string fileName { get; set; }
        public bool isPublished { get; set; }
    }
    public class CrudDsaResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
