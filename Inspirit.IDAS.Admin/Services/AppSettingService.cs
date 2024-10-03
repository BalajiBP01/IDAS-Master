using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class AppSettingService
    {
        IDASDbContext _dbContext;
        public AppSettingService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActionResult<SettingDataResponse>> SettingDataList(DataTableRequest request)
        {
            SettingDataResponse response = new SettingDataResponse();
            try
            {
                var lst = _dbContext.ApplicationSetting.AsQueryable();
                int cnt = lst.Count();
                var flt = lst;
               
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();



                var data = (from s in flt
                            select new Setting
                            {
                                Id = s.Id,
                                SettingName = s.SettingName,
                                SettingValue = s.SettingValue,
                                Remarks = s.Remarks
                            }).ToList();

                response.data = data;

            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public SettingCrudResponse Insert(ApplicationSetting data)
        {
            SettingCrudResponse response = new SettingCrudResponse();

            try
            {
                data.Id = Guid.NewGuid();

                _dbContext.ApplicationSetting.Add(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }

        public SettingCrudResponse Update(ApplicationSetting data)
        {
            SettingCrudResponse response = new SettingCrudResponse();
            try
            {
                _dbContext.ApplicationSetting.Update(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }

        public SettingCrudResponse Delete(Guid id)
        {
            SettingCrudResponse response = new SettingCrudResponse();
            try
            {
                var data = _dbContext.ApplicationSetting.FirstOrDefault(t => t.Id == id);
                _dbContext.ApplicationSetting.Remove(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public ApplicationSetting View(Guid id)
        {
            ApplicationSetting data = new ApplicationSetting();
            try
            {
                data = _dbContext.ApplicationSetting.Find(id);

            }
            catch (Exception ex)
            {

            }
            return data;
        }
    }

    public class SettingDataResponse : DataTableResponse<Setting>
    {

    }

    public class Setting
    {
        public Guid Id { get; set; }

        public string SettingName { get; set; }

        public string SettingValue { get; set; }
        public string Remarks { get; set; }

    }

    public class SettingCrudResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}
