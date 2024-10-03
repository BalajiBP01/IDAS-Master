using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using Inspirit.IDAS.Data.IDAS;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Inspirit.IDAS.WebApplication
{
    public class SummaryFullAuditService
    {
        IDASDbContext _dbContext;

        public SummaryFullAuditService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ActionResult<SummaryAuditDataResponse>> SummaryAuditDataList(SummaryAuditSearchRequest request)
        {
            SummaryAuditDataResponse response = new SummaryAuditDataResponse();
            CustomerLogvm req = new CustomerLogvm();

            try
            {
                req.Fromdate = DateTime.Parse(request.fromdate, CultureInfo.GetCultureInfo("en-gb"));
                req.Todate = DateTime.Parse(request.todate, CultureInfo.GetCultureInfo("en-gb"));
              
                var lst = _dbContext.CustomerLogs.Include(cu => cu.CustomerUser).AsQueryable();
                int cnt = lst.Count();
                var flt = lst;
                int SumofCreditPoints = 0;


                var Result = new List<Result>();
                var isAdmin = _dbContext.CustomerUsers.Where(m => m.Id == request.CustomerUserId).FirstOrDefault();               

                var users = _dbContext.CustomerUsers;
                if (isAdmin.IsAdmin)
                    lst = _dbContext.CustomerLogs.Where(m => m.CompanyId == request.CustomerId);
                else
                    lst = _dbContext.CustomerLogs.Where(m => m.CompanyId == request.CustomerId && m.CompanyUserId == isAdmin.Id);



                if (!String.IsNullOrEmpty(request.fromdate) && !String.IsNullOrEmpty(request.todate))
                {
                    DateTime FilterToDate = new DateTime(Convert.ToDateTime(request.todate).Year, Convert.ToDateTime(request.todate).Month, Convert.ToDateTime(request.todate).Day, 23, 59, 59);

                    flt = lst.Where(m => m.DateTime >= Convert.ToDateTime(request.fromdate)
                    && m.DateTime <= FilterToDate);
                    SumofCreditPoints = flt.Select(c => c.CreditPoints).Sum();
                }

                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();

                var data = (from s in flt

                            join u in users on s.CompanyUserId equals u.Id
                            select new { Name = u.FirstName + " " + u.LastName, s.CreditPoints, u.Id, u.IsAdmin, u.Email,u.ActivatedDate, u.LastLoginDate } into u
                            group u by u.Name into UserGroup


                            select new CustomerLogvm
                            {
                                Firstname = UserGroup.Key,
                                UserType = UserGroup.Select(t => t.IsAdmin).FirstOrDefault(),
                                LoginName = UserGroup.Select(t => t.Email).FirstOrDefault(),
                                CreditPoints = UserGroup.Sum(i=> i.CreditPoints),
                                UserActiveDate = UserGroup.Select(t => t.ActivatedDate.Value).FirstOrDefault(),
                                UserLastActiveDate = UserGroup.Select(t => t.LastLoginDate.Value).FirstOrDefault()
                            }).Distinct().ToList();
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

    public class SummaryAuditDataResponse : DataTableResponse<CustomerLogvm>
    {

    }
    public class CustomerLogvm
    {
        public DateTime Fromdate { get; set; }

        public DateTime Todate { get; set; }

        public Guid Id { get; set; }

        public DateTime UserActiveDate { get; set; }
        public DateTime UserLastActiveDate { get; set; }
        public int CreditPoints { get; set; }
        public string LoginName { get; set; }
        public string FullName { get; set; }
        public bool UserType { get; set; }
        public string Firstname { get; set; }
       

    }

    public class SummaryAuditSearchRequest
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerUserId { get; set; }
        public Guid companyuserid { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }

    public class Result
    {
        public string Name { get; set; }
        public int CreditPoints { get; set; }
    }
}



