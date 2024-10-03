using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using Inspirit.IDAS.Data.IDAS;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Inspirit.IDAS.WebApplication
{
    public class FullAuditReportServices
    {
        IDASDbContext _dbContext;

        public FullAuditReportServices(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ActionResult<FullAuditDataResponse>> FullAuditDataList(FullAuditSearchRequest request)
        {
            FullAuditDataResponse response = new FullAuditDataResponse();
            CustomerLogvms req = new CustomerLogvms();
            try
            {
                req.fromdate = DateTime.Parse(request.fromdate, CultureInfo.GetCultureInfo("en-gb"));
                req.todate = DateTime.Parse(request.todate, CultureInfo.GetCultureInfo("en-gb"));
                

                var isAdmin = _dbContext.CustomerUsers.Where(m => m.Id == request.CustomerUserId).FirstOrDefault();

                var users = _dbContext.CustomerLogs.Where(m => m.CompanyId == request.CustomerId).ToList();

                
                var lst = new List<CustomerLog>();
                var userlst = new List<CustomerUser>();                
                int cnt = users.Count();
                if (isAdmin.IsAdmin)
                    lst = _dbContext.CustomerLogs.Where(m => m.CompanyId == request.CustomerId).ToList();
                else
                    lst = _dbContext.CustomerLogs.Where(m => m.CompanyId == request.CustomerId && m.CompanyUserId == isAdmin.Id).ToList();

                var flt = lst;

                


                userlst = _dbContext.CustomerUsers.Where(m => m.CustomerId == isAdmin.CustomerId).ToList();

                if (!String.IsNullOrEmpty(request.fromdate) && !String.IsNullOrEmpty(request.todate))
                {
                    DateTime FilterToDate = new DateTime(Convert.ToDateTime(request.todate).Year, Convert.ToDateTime(request.todate).Month, Convert.ToDateTime(request.todate).Day, 23, 59, 59);
                    
                    flt = lst.AsQueryable().Where(m => m.DateTime >= Convert.ToDateTime(request.fromdate) && m.DateTime <= FilterToDate).ToList();
                }
                
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();
                var data = (from s in flt
                            join c in _dbContext.CustomerUsers
                            on s.CompanyUserId equals c.Id
                            select new CustomerLogvms
                            {
                                Id = s.Id,
                                DateTime =Convert.ToString(s.DateTime),
                                CreditPoints = s.CreditPoints,
                                LogType = s.LogType,
                                SearchCriteria = s.SearchCriteria,
                                SearchType = s.SearchType,
                                CustomerUserId = s.CompanyUserId,
                                userEmail = c.Email,
                                Name = userlst.Where(m => m.Id == s.CompanyUserId).FirstOrDefault().FirstName + " " + userlst.Where(m => m.Id == s.CompanyUserId).FirstOrDefault().LastName 
                            }).ToList();

                response.data = data;
            }
            catch (Exception ex)
            {

            }
            return response;
        }     

        public CustomerLog View(Guid id)
        {
            CustomerLog data = new CustomerLog();
            try
            {
                data = _dbContext.CustomerLogs.Find(id);

            }
            catch (Exception ex)
            {

            }
            return data;
        }
    }

    public class FullAuditDataResponse : DataTableResponse<CustomerLogvms>
    {

    }
    public class CustomerLogvms
    {
        public Guid Id { get; set; }
        public string DateTime { get; set; }
        public int CreditPoints { get; set; }
        public DateTime fromdate { get; set; }
        public DateTime todate { get; set; }
        public string LogType { get; set; }
        public string SearchCriteria { get; set; }
        public string SearchType { get; set; }
        public string Name { get; set; }
        public Guid CustomerUserId { get; set; }
        public string userEmail { get; set; }
    }

    public class FullAuditSearchRequest
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerUserId { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
       
        public DataTableRequest dtRequest { get; set; }
    }
}




