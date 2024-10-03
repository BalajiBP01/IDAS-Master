using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{

    public class LookupDataService
    {
        IDASDbContext _dbContext;
        public LookupDataService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<LookupDataResponse> LookupDataList(DataTableRequest request)
        {
            LookupDataResponse response = new LookupDataResponse();
            try
            {
                var lst = _dbContext.LookupDatas.AsQueryable();
                int cnt = _dbContext.LookupDatas.Count();
                var flt = lst;
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();

                var data = await (from s in flt
                                  select new Lookup
                                  {
                                      Id = s.ID,
                                      Text = s.Text,
                                      Type = s.Type,
                                      Value = s.Value,
                                      IsActive = s.IsActive
                                  }).ToListAsync();

                response.data = data;
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public CrudResponse Insert(LookupData data)
        {
            CrudResponse response = new CrudResponse();
            try
            {
                var lookup = _dbContext.LookupDatas.Where(l => l.Type == data.Type && l.Value == data.Value);
                if (lookup.Count() > 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Already Exists";
                    return response;
                }
                data.ID = Guid.NewGuid();
                _dbContext.LookupDatas.Add(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public CrudResponse Update(LookupData data)
        {
            CrudResponse response = new CrudResponse();
            try
            {
                _dbContext.LookupDatas.Update(data);
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
                var data = _dbContext.LookupDatas.FirstOrDefault(t => t.ID == id);
                _dbContext.LookupDatas.Remove(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public LookupData View(Guid id)
        {
            LookupData data = new LookupData();
            try
            {
                data = _dbContext.LookupDatas.Find(id);

            }
            catch (Exception ex)
            {

            }
            return data;
        }
    }
    public class LookupDataResponse : DataTableResponse<Lookup>
    {

    }
    public class Lookup
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
    }
    public class CrudResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
