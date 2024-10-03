using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class DashboardService
    {
        IDASDbContext _dbContext;
        public DashboardService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActionResult<CustomerCount>> GetCustomerData()
        {
            CustomerCount data = new CustomerCount();
            try
            {

                DateTime todayDate = DateTime.Now;
                DateTime fromdate = new DateTime(todayDate.Year, todayDate.Month, 1);
                DateTime todate = new DateTime(fromdate.Year, fromdate.Month + 1, 1, 23, 59, 59).AddDays(-1);

                var lst = _dbContext.Customers.ToList();
                var monthlst = lst.Where(t => t.CreatedDate >= fromdate && t.CreatedDate <= todate).ToList();
                data.ActiveMonthtTotal = monthlst.FindAll(m => m.Status.ToUpper() == "ACTIVE").ToList().Count();
                data.PendingMonthTotal = monthlst.FindAll(m => m.Status.ToUpper() == "PENDING").ToList().Count();
                data.MonthTotal = monthlst.Count();
                data.InactiveMonthTotal = monthlst.FindAll(m => m.Status.ToUpper() == "INACTIVE").ToList().Count();



                data.ActiveTotalCount = lst.FindAll(m => m.Status.ToUpper() == "ACTIVE").ToList().Count();
                data.PendingTotalCount = lst.FindAll(m => m.Status.ToUpper() == "PENDING").ToList().Count();
                data.Total = lst.Count();
                data.InActiveTotalCount = lst.FindAll(m => m.Status.ToUpper() == "INACTIVE").ToList().Count();

            }
            catch (Exception ex)
            {

            }
            return data;
        }

        public async Task<ActionResult<CustomerUserCount>> GetCustomerUserData()
        {
            CustomerUserCount data = new CustomerUserCount();
            try
            {

                DateTime todayDate = DateTime.Now;
                DateTime fromdate = new DateTime(todayDate.Year, todayDate.Month, 1);
                DateTime todate = new DateTime(fromdate.Year, fromdate.Month + 1, 1, 23, 59, 59).AddDays(-1);

                var lst = _dbContext.CustomerUsers.ToList();
                var monthlst = lst.Where(t => t.CreatedDate >= fromdate && t.CreatedDate <= todate).ToList();
                data.ActiveMonthtTotal = monthlst.FindAll(m => m.Status.ToUpper() == "ACTIVE").ToList().Count();
                data.PendingMonthTotal = monthlst.FindAll(m => m.Status.ToUpper() == "PENDING").ToList().Count();
                data.MonthTotal = monthlst.Count();
                data.InactiveMonthTotal = monthlst.FindAll(m => m.Status.ToUpper() == "INACTIVE").ToList().Count();



                data.ActiveTotalCount = lst.FindAll(m => m.Status.ToUpper() == "ACTIVE").ToList().Count();
                data.PendingTotalCount = lst.FindAll(m => m.Status.ToUpper() == "PENDING").ToList().Count();
                data.Total = lst.Count();
                data.InActiveTotalCount = lst.FindAll(m => m.Status.ToUpper() == "INACTIVE").ToList().Count();

            }
            catch (Exception ex)
            {

            }
            return data;
        }

        public async Task<ActionResult<DonotCallRegCount>> GetDonotCallregCount()
        {
            DonotCallRegCount data = new DonotCallRegCount();
            try
            {

                DateTime todayDate = DateTime.Now;
                DateTime fromdate = new DateTime(todayDate.Year, todayDate.Month, 1);
                DateTime todate = new DateTime(fromdate.Year, fromdate.Month + 1, 1, 23, 59, 59).AddDays(-1);

                var lst = _dbContext.DoNotCallRegistrys.ToList();
                var monthlst = lst.Where(t => t.CurrentDate >= fromdate && t.CurrentDate <= todate).ToList();
                data.MonthTotal = monthlst.Count();
                data.Total = lst.Count();

            }
            catch (Exception ex)
            {

            }
            return data;
        }
        public async Task<ActionResult<List<CustomerLog>>> GetCustomerLogsOld_Month()
        {
            List<CustomerLog> data = new List<CustomerLog>();
            try
            {
                int SumofCreditPoints = 0;
                DateTime todayDate = DateTime.Now;
                DateTime fromdate = new DateTime(todayDate.Year, todayDate.Month, 1);
                DateTime todate = new DateTime(fromdate.Year, fromdate.Month + 1, 1, 23, 59, 59).AddDays(-1);
                DateTime yesterday = DateTime.Today; //DateTime.Today.AddDays(-1);
                //yesterday = yesterday.Date;
                var logs = _dbContext.CustomerLogs.Where(m => m.DateTime > yesterday).ToList();
                var users = _dbContext.CustomerUsers.Include(t => t.Customer).ToList();

                List<CustomerLog> datalst = (from s in logs

                                             join u in users on s.CompanyUserId equals u.Id
                                             select new { Name = u.FirstName, u.LastName, s.CreditPoints, u.Id, u.IsAdmin, u.Email, u.Customer.TradingName } into u
                                             group u by u.Name into UserGroup

                                             select new CustomerLog
                                             {
                                                 FirstName = UserGroup.Key,
                                                 LastName = UserGroup.Select(t => t.LastName).FirstOrDefault(),
                                                 CustomerName = UserGroup.Select(t => t.TradingName).FirstOrDefault(),
                                                 CreditsUsage = UserGroup.Sum(i => i.CreditPoints)
                                             }).Distinct().OrderByDescending(m => m.CreditsUsage).ToList();
                data = datalst.Take(10).ToList();
            }
            catch (Exception ex)
            {

            }

            return data;
        }

        public async Task<ActionResult<List<CustomerLog>>> GetCustomerLogs()
        {
            List<CustomerLog> data = new List<CustomerLog>();
            try
            {
                int SumofCreditPoints = 0;
                DateTime todayDate = DateTime.Now;
                DateTime fromdate = new DateTime(todayDate.Year, todayDate.Month, 1);
                DateTime todate = new DateTime(fromdate.Year, fromdate.Month + 1, 1, 23, 59, 59).AddDays(-1);


                var logs = _dbContext.CustomerLogs.Where(m => m.DateTime >= fromdate && m.DateTime <= todate).ToList();
                var users = _dbContext.CustomerUsers.Include(t => t.Customer).ToList();

                List<CustomerLog> datalst = (from s in logs

                                             join u in users on s.CompanyUserId equals u.Id
                                             select new { Name = u.FirstName, u.LastName, s.CreditPoints, u.Id, u.IsAdmin, u.Email, u.Customer.TradingName } into u
                                             group u by u.Name into UserGroup

                                             select new CustomerLog
                                             {
                                                 FirstName = UserGroup.Key,
                                                 LastName = UserGroup.Select(t => t.LastName).FirstOrDefault(),
                                                 CustomerName = UserGroup.Select(t => t.TradingName).FirstOrDefault(),
                                                 CreditsUsage = UserGroup.Sum(i => i.CreditPoints)
                                             }).Distinct().OrderByDescending(m => m.CreditsUsage).ToList();
                data = datalst.Take(10).ToList();
            }
            catch (Exception ex)
            {

            }

            return data;
        }

        public async Task<ActionResult<ContactUsCount>> GetContactusCount()
        {

            ContactUsCount data = new ContactUsCount();
            try
            {
                DateTime todayDate = DateTime.Now;
                DateTime fromdate = new DateTime(todayDate.Year, todayDate.Month, 1);
                DateTime todate = new DateTime(fromdate.Year, fromdate.Month + 1, 1, 23, 59, 59).AddDays(-1);

                var lst = _dbContext.ContactUs.ToList();
                var lstmonth = lst.Where(t => t.Date >= fromdate && t.Date <= todate).ToList();

                data.ActionedMonth = lstmonth.Where(m => m.IsRead == true).ToList().Count();
                data.MonthTotal = lstmonth.Count();
                data.NotActionedMonth = lstmonth.Where(m => m.IsRead == false).ToList().Count();

                data.ActionedTotal = lst.Where(m => m.IsRead == true).ToList().Count();
                data.NotActionedTotal = lst.Where(m => m.IsRead == false).ToList().Count();
                data.Total = lst.Count();
            }
            catch (Exception ex)
            {

            }
            return data;
        }

        public async Task<ActionResult<PaymentCount>> GetPaymentDet()
        {

            PaymentCount data = new PaymentCount();
            try
            {
                DateTime todayDate = DateTime.Now;

                DateTime fromdate = new DateTime(todayDate.Year, todayDate.Month, 1);
                DateTime todate = new DateTime(fromdate.Year, fromdate.Month + 1, 1, 23, 59, 59).AddDays(-1);

                DateTime prevfromdate = new DateTime(fromdate.Year, fromdate.Month - 1, 1);
                DateTime prevtodate = new DateTime(fromdate.Year, fromdate.Month, 1, 23, 59, 59).AddDays(-1);

                var invlst = _dbContext.Invoices.Include(t => t.InvoiceLineItems).ToList();
                var invlistmonth = invlst.Where(t => t.InvoiceDate >= fromdate && t.InvoiceDate <= todate).ToList();
                var invprevmonthlist = invlst.Where(t => t.InvoiceDate >= prevfromdate && t.InvoiceDate <= prevtodate).ToList();

                var paymentlst = _dbContext.Payments.ToList();
                var paymentlstmonth = paymentlst.Where(t => t.Date >= fromdate && t.Date <= todate).ToList();
                var paymentprevmonth = paymentlst.Where(t => t.Date >= prevfromdate && t.Date <= prevtodate).ToList();

                data.NotPaidMonth = invlistmonth.Where(t => t.ispaid == false).ToList().Count();
                data.NotPaidTotal = invlst.Where(t => t.ispaid == false).ToList().Count();
                data.NotPaidPrevmonth = invprevmonthlist.Where(t => t.ispaid == false).Count();

                data.PaidMonth = paymentlstmonth.Where(t => t.Amount == t.PaymentAmountReceive).ToList().Count();
                data.PaidTotal = paymentlst.Where(t => t.Amount == t.PaymentAmountReceive).ToList().Count();
                data.Paidprevmonth = paymentprevmonth.Where(t => t.Amount == t.PaymentAmountReceive).ToList().Count();

                data.ShortPaidMonth = paymentlstmonth.Where(t => t.Amount != t.PaymentAmountReceive).ToList().Count();
                data.ShortPaidTotal = paymentlst.Where(t => t.Amount != t.PaymentAmountReceive).ToList().Count();
                data.ShortPaidprevmonth = paymentprevmonth.Where(t => t.Amount != t.PaymentAmountReceive).ToList().Count();


            }
            catch (Exception ex)
            {

            }
            return data;
        }

        public async Task<ActionResult<InvoiceCount>> GetInvoiceDetails()
        {

            InvoiceCount data = new InvoiceCount();
            try
            {
                DateTime todayDate = DateTime.Now;
                DateTime fromdate = new DateTime(todayDate.Year, todayDate.Month, 1);
                DateTime todate = new DateTime(fromdate.Year, fromdate.Month + 1, 1, 23, 59, 59).AddDays(-1);

                DateTime prevfromdate = new DateTime(fromdate.Year, fromdate.Month - 1, 1);
                DateTime prevtodate = new DateTime(fromdate.Year, fromdate.Month, 1, 23, 59, 59).AddDays(-1);

                var lst = _dbContext.Invoices.Include(m => m.InvoiceLineItems).ToList();
                data.Total = lst.Count();
                data.SentCustTotal = lst.Where(t => t.IsEmailSend == true).ToList().Count();
                data.NotSentTotal = lst.Where(t => t.IsEmailSend == false).ToList().Count();
                data.CancelTotal = lst.Where(t => t.isCancelled == true).ToList().Count();
                data.TaxinvTotal = lst.Where(t => t.IsTaxinvSent == true).ToList().Count();

                var lstmonth = lst.Where(m => m.InvoiceDate >= fromdate && m.InvoiceDate <= todate).ToList();
                data.MonthTotal = lstmonth.Count();
                data.NotSentMonth = lstmonth.Where(t => t.IsEmailSend == false).ToList().Count();
                data.SentCustMonth = lstmonth.Where(t => t.IsEmailSend == true).ToList().Count();
                data.CancelMonth = lstmonth.Where(t => t.isCancelled == true).ToList().Count();
                data.TaxinvMonth = lstmonth.Where(t => t.IsTaxinvSent == true).ToList().Count();

                var prevlst = lst.Where(m => m.InvoiceDate >= prevfromdate && m.InvoiceDate <= prevtodate).ToList();
                data.CancelPrevMonth = prevlst.Where(t => t.isCancelled == true).ToList().Count();

            }
            catch (Exception ex)
            {

            }
            return data;
        }

        public async Task<ActionResult<SubscriptionCount>> GetSubActiveCount()
        {
            SubscriptionCount data = new SubscriptionCount();
            try
            {
                DateTime todayDate = DateTime.Now;
                DateTime fromdate = new DateTime(todayDate.Year, todayDate.Month, 1);
                DateTime todate = new DateTime(fromdate.Year, fromdate.Month + 1, 1, 23, 59, 59).AddDays(-1);

                var lstmonth = _dbContext.Subscriptions.Include(t => t.SubscriptionItems)
                                                              .Where(t => t.SubscriptionItems.Any(i => i.StartDate <= todate && i.StartDate >= fromdate && i.isBilled == false)).ToList();
                data.MonthTotal = lstmonth.Count();
                var lst = _dbContext.Subscriptions.Include(t => t.SubscriptionItems).ToList();
                data.Total = lst.Count();
            }
            catch (Exception ex)
            {

            }

            return data;
        }






    }
    public class CustomerCount
    {
        public int Total { get; set; }
        public int MonthTotal { get; set; }
        public int ActiveTotalCount { get; set; }
        public int PendingTotalCount { get; set; }
        public int InActiveTotalCount { get; set; }
        public int ActiveMonthtTotal { get; set; }
        public int InactiveMonthTotal { get; set; }
        public int PendingMonthTotal { get; set; }
    }
    public class CustomerUserCount
    {
        public int Total { get; set; }
        public int MonthTotal { get; set; }
        public int ActiveTotalCount { get; set; }
        public int PendingTotalCount { get; set; }
        public int InActiveTotalCount { get; set; }
        public int ActiveMonthtTotal { get; set; }
        public int InactiveMonthTotal { get; set; }
        public int PendingMonthTotal { get; set; }
    }
    public class DonotCallRegCount
    {
        public int Total { get; set; }
        public int MonthTotal { get; set; }
    }
    public class CustomerLog
    {
        public Guid CustomerUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Surname { get; set; }
        public int CreditsUsage { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }

    }
    public class ContactUsCount
    {
        public int Total { get; set; }
        public int MonthTotal { get; set; }
        public int NotActionedTotal { get; set; }
        public int NotActionedMonth { get; set; }
        public int ActionedTotal { get; set; }
        public int ActionedMonth { get; set; }

    }

    public class PaymentCount
    {
        public int NotPaidTotal { get; set; }
        public int NotPaidMonth { get; set; }
        public int PaidTotal { get; set; }
        public int PaidMonth { get; set; }
        public int ShortPaidTotal { get; set; }
        public int ShortPaidMonth { get; set; }
        public int NotPaidPrevmonth { get; set; }
        public int Paidprevmonth { get; set; }
        public int ShortPaidprevmonth { get; set; }
    }
    public class InvoiceCount
    {
        public int Total { get; set; }
        public int MonthTotal { get; set; }
        public int SentCustTotal { get; set; }
        public int SentCustMonth { get; set; }
        public int NotSentTotal { get; set; }
        public int NotSentMonth { get; set; }
        public int CancelTotal { get; set; }
        public int CancelMonth { get; set; }
        public int CancelPrevMonth { get; set; }
        public int TaxinvTotal { get; set; }
        public int TaxinvMonth { get; set; }
    }

    public class SubscriptionCount
    {
        public int MonthTotal { get; set; }
        public int Total { get; set; }
    }

}
