using Inspirit.IDAS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using XDSTraceService;

namespace Inspirit.IDAS.WebApplication
{
    public class CustomerLogService
    {
        IDASDbContext _context;
        EmailService emailService;
        private IHostingEnvironment _hostingEnvironment;
        private IConfiguration _configuration; public CustomerLogService(IDASDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Krishna update Customer Log
        /// </summary>
        /// <param name="customerUserId"></param>
        /// <param name="customerId"></param>
        /// <param name="logTyp"></param>
        /// <param name="SearchType"></param>
        /// <param name="det"></param>
        /// <param name="IdOrPassportNumber"></param>
        /// <param name="InputType"></param>
        /// <param name="istrailuser"></param>
        /// <returns></returns>
        public async Task<CustomerLog> UpdateCustomerLog(Guid customerUserId, Guid customerId, string logTyp, string SearchType, string det, string IdOrPassportNumber, string InputType, bool istrailuser, string VoucherCode,string CustomerRefNum,bool IsXDS, string EnquiryReason)
        {
            CustomerLog log = new CustomerLog();
            switch (logTyp)
            {

                case "Tracing":
                    if (!string.IsNullOrEmpty(logTyp) && CanUpdate(customerUserId, IdOrPassportNumber, istrailuser))
                    {
                        if (string.IsNullOrEmpty(IdOrPassportNumber))
                            break;
                        log.Id = Guid.NewGuid();
                        log.DateTime = DateTime.Now;
                        log.CompanyId = customerId;
                        log.CompanyUserId = customerUserId;
                        if (SearchType != "lookup" && SearchType != "Search")
                            log.CreditPoints = 1;
                        else
                            log.CreditPoints = 0;

                        log.IdOrPassportNumber = IdOrPassportNumber;
                        log.LogType = logTyp;
                        log.SearchType = SearchType;
                        log.SearchCriteria = det;
                        log.InputType = InputType;

                        //Krishna start new code
                        log.EnquiryReason = EnquiryReason;
                        if (IsXDS == false)
                        {
                            Workorder wo = GetWorkorder(customerId, logTyp, customerUserId);
                            if (wo != null)
                                log.WorkorderId = wo.Id;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(CustomerRefNum))
                            {
                                log.CustomerRefNum = CustomerRefNum;
                            }
                            //else
                            //{
                            //    log.CustomerRefNum = "";
                            //}


                            if (!string.IsNullOrEmpty(VoucherCode))
                            {
                                log.VoucherCode = VoucherCode;
                            }
                            //else
                            //{
                            //    log.VoucherCode = "";
                            //}
                        }

                        // Krishna end

                        _context.CustomerLogs.Add(log);
                        _context.SaveChanges();
                        // krishna start pending
                        try
                        {
                            Customer cust = _context.Customers.Where(c => c.Id == customerId).FirstOrDefault();
                            if ((cust.IsXDS == true) && (log.CreditPoints == 1))
                            {
                                DateTime day = DateTime.Today;
                                DateTime filterday = new DateTime(Convert.ToInt32(day.Year), Convert.ToInt32(day.Month), Convert.ToInt32(day.Day), 23, 59, 59);

                                // xds post api
                                // Guid customerUserId, Guid customerId, string logTyp, string SearchType, string det, string IdOrPassportNumber
                                //CustomerLog customerLog = _context.CustomerLogs.OrderByDescending(x => x.RefNum).Where(t => (t.CompanyId == customerId) && (t.CompanyUserId == customerUserId) && (t.IdOrPassportNumber == IdOrPassportNumber) && ((t.DateTime.Year == day.Year) && (t.DateTime.Month == day.Month) && (t.DateTime.Day == day.Day)) && t.CreditPoints == 1).FirstOrDefault();
                                //Customer cust = _context.Customers.Where(c => c.Id == customerId).FirstOrDefault();
                                CustomerUser user = _context.CustomerUsers.Where(c => c.Id == customerUserId).FirstOrDefault();
                                CustomerLog customerLog = _context.CustomerLogs.Where(t => (t.RefNum == log.RefNum && t.CompanyId == customerId) && (t.CompanyUserId == customerUserId) && (t.IdOrPassportNumber == IdOrPassportNumber) && ((t.DateTime.Year == day.Year) && (t.DateTime.Month == day.Month) && (t.DateTime.Day == day.Day)) && t.CreditPoints == 1).FirstOrDefault();

                                string Passport = null;
                                int CustSubscriberId = int.Parse(cust.SubscriberId);
                                string XDSUsername = user.Email; //"ksulochana";
                                string userName = "Insprt_uat"; // API_USERNAME = "Insprt_uat"
                                string password = "Id@s0522"; // API_PASSWORD = "Id@s0522"

                                string firstName = null;
                                string surName = null;
                                string BirthDate = null;
                                string InspiritRefNum = log.RefNum.ToString();
                                string EnquiryInput = customerLog.InputType.ToString();
                                string XDSRefNum = customerLog.CustomerRefNum.ToString();
                                string XDSVoucherCode = customerLog.VoucherCode.ToString();

                                string xdsSuccessMessage = "Data saved successfully";

                                XDSConnectWSSoapClient xdsClient =
                                    new XDSConnectWSSoapClient
                                    (XDSConnectWSSoapClient.EndpointConfiguration.XDSConnectWSSoap);

                                var result = await xdsClient.ConnectLogResultAsync(
                                                "Insprt_uat", "Id@s0522", CustSubscriberId, XDSUsername,
                                                IdOrPassportNumber, Passport, firstName, surName, BirthDate,
                                                DateTime.Now.ToString(), InspiritRefNum, EnquiryInput,
                                                XDSRefNum, VoucherCode
                                                );

                                if (result.ToString().ToLower() != xdsSuccessMessage.ToLower())
                                {
                                    // pending 
                                    // update completed status false
                                    _context.CustomerLogs.Update(log);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // pending error message 
                            _context.CustomerLogs.Update(log);
                            _context.SaveChanges();
                            return log;
                        }

                    // krishna end


                    // set Id from CustomerLog where CreditPoint = 0 CustomerRefNum, VoucherCode, EnqInput, SearchCriteria
                    return log;
                    }
                    break;
                case "LeadGeneration":
                    ;
                    break;
                case "BatchProcess":
                    ;
                    break;
            }
            return log;
        }


        public async Task<CustomerLog> UpdateCustomerLogPrev(Guid customerUserId, Guid customerId, string logTyp, string SearchType, string det, string IdOrPassportNumber, string InputType, bool istrailuser)
        {
            CustomerLog log = new CustomerLog();
            switch (logTyp)
            {

                case "Tracing":
                    if (!string.IsNullOrEmpty(logTyp) && CanUpdate(customerUserId, IdOrPassportNumber, istrailuser))
                    {
                        if (string.IsNullOrEmpty(IdOrPassportNumber))
                            break;
                        log.Id = Guid.NewGuid();
                        log.DateTime = DateTime.Now;
                        log.CompanyId = customerId;
                        log.CompanyUserId = customerUserId;
                        if (SearchType != "lookup" && SearchType != "Search")
                            log.CreditPoints = 1;
                        else
                            log.CreditPoints = 0;

                        log.IdOrPassportNumber = IdOrPassportNumber;
                        log.LogType = logTyp;
                        log.SearchType = SearchType;
                        log.SearchCriteria = det;
                        log.InputType = InputType;
                        Workorder wo = GetWorkorder(customerId, logTyp, customerUserId);
                        if (wo != null)
                            log.WorkorderId = wo.Id;

                        _context.CustomerLogs.Add(log);
                        _context.SaveChanges();
                        return log;
                    }
                    break;
                case "LeadGeneration":
                    ;
                    break;
                case "BatchProcess":
                    ;
                    break;
            }
            return log;
        }

        public async Task<TrailUserLog> UpdateTrailuserLog(Guid customerUserId, Guid customerId, string logTyp, string SearchType, string det, string IdOrPassportNumber, string InputType, bool istrailuser)
        {
            TrailUserLog log = new TrailUserLog();
            switch (logTyp)
            {
                case "Tracing":
                    if (!string.IsNullOrEmpty(logTyp) && CanUpdate(customerUserId, IdOrPassportNumber, istrailuser))
                    {
                        log.ID = Guid.NewGuid();
                        log.Date = DateTime.Now;
                        log.TrailUserId = customerUserId;
                        if (SearchType != "lookup")
                            log.CreditPoints = 1;
                        else
                            log.CreditPoints = 0;

                        log.Idorpassportnumber = IdOrPassportNumber;
                        log.Logtype = logTyp;
                        log.Searchtype = SearchType;
                        log.SearchCriteria = det;
                        log.InputType = InputType;

                        _context.TrailUserLogs.Add(log);
                        _context.SaveChanges();
                        return log;
                    }
                    break;
                case "LeadGeneration":
                    ;
                    break;
                case "BatchProcess":
                    ;
                    break;
            }
            return log;
        }
        private bool CanUpdate(Guid customerUserId, string IdOrPassportNumber, bool istrailuser)
        {
            bool isUpdate = true;
            if (string.IsNullOrEmpty(IdOrPassportNumber))
                isUpdate = true;
            else
            {

                if (istrailuser == false)
                {
                    DateTime day = DateTime.Today;
                    DateTime filterday = new DateTime(Convert.ToInt32(day.Year), Convert.ToInt32(day.Month), Convert.ToInt32(day.Day), 23, 59, 59);

                    //for tracing same user,IDnumber,date combination - there should be single entry
                    CustomerLog log = _context.CustomerLogs.FirstOrDefault(t => t.CompanyUserId == customerUserId && t.IdOrPassportNumber == IdOrPassportNumber && (t.DateTime < filterday && t.DateTime > day) && t.CreditPoints == 1);

                    if (log != null)
                    {
                        isUpdate = false;
                    }
                }
                else
                {

                    if (istrailuser == true)
                    {
                        TrailUserLog logs = _context.TrailUserLogs.FirstOrDefault(t => t.TrailUserId == customerUserId && t.Idorpassportnumber == IdOrPassportNumber && !(t.Date.Subtract(DateTime.Now).Days > 0) && !(t.Date.Subtract(DateTime.Now).Days < 0));

                        if (logs != null)
                        {
                            isUpdate = false;
                        }
                    }
                }
            }
            return isUpdate;

        }
        IDASDbContext _dbContext;
        public int GetUserCredits(Guid userId, Guid customerId, string serviceType)

        {
            int credits = 0;
            try
            {
                if (serviceType == "Tracing")
                {
                    Workorder wo = GetWorkorder(customerId, "Tracing", userId);

                    var isXDS = _context.Customers.Where(c => c.Id == customerId).Select(x => x.IsXDS).FirstOrDefault();

                    if(isXDS == true)
                    {
                        return 110;
                    }

                    if (wo != null)
                    {
                        if (wo.ProductPackage.Product.UsageType == "Yearly" || wo.ProductPackage.Product.UsageType == "Monthly")
                        {
                            if (IsLicenceAssigned(userId, wo.SubscriptionItemID.Value))
                            {
                                //application settings


                                var app_credits = _context.ApplicationSetting.Where(t => t.SettingName == "Subscription Credit Limit").Select(t => t.SettingValue).FirstOrDefault();
                                credits = Convert.ToInt32(app_credits);
                                var val = _context.CustomerLogs.Where(t => t.CompanyUserId == userId && t.WorkorderId == wo.Id && t.SearchType != "Lookup" && t.DateTime.Date == DateTime.Today).Sum(t => t.CreditPoints);
                                if (val > 0)
                                {
                                    //credits validation per day
                                    credits = credits - val;
                                }
                            }
                        }
                        else if (wo.ProductPackage.Product.UsageType == "Credits")
                        {
                            if (IsLicenceAssigned(userId, wo.SubscriptionItemID.Value))
                            {
                                credits = wo.Credits;
                                var val = _context.CustomerLogs.Where(t => t.CompanyId == customerId && t.WorkorderId == wo.Id && t.SearchType != "Lookup").Sum(t => t.CreditPoints);
                                if (val > 0)
                                {

                                    credits = credits - val;

                                }
                            }
                        }
                        else
                        {
                            var tuser = _context.TrailUsers.Where(m => m.ID == wo.CustomerId).FirstOrDefault();
                            if (tuser != null)
                            {
                                credits = wo.Credits;
                                var val = _context.CustomerLogs.Where(t => t.CompanyId == customerId && t.WorkorderId == wo.Id && t.SearchType != "Lookup").Sum(t => t.CreditPoints);
                                if (val > 0)
                                {
                                    credits = credits - val;
                                }
                            }
                        }
                    }
                    else
                    {
                        var trailuser = _context.TrailUsers.Where(t => t.ID == userId).FirstOrDefault();
                        if (trailuser != null)
                        {
                            credits = trailuser.Credits;
                            var val = _context.TrailUserLogs.Where(t => t.TrailUserId == trailuser.ID && t.Searchtype != "Lookup").Sum(t => t.CreditPoints);
                            if (val > 0)
                            {
                                credits = credits - val;
                            }
                        }
                    }
                }
                else if (serviceType == "BatchProcess")
                {
                    Workorder wo = GetWorkorder(customerId, "BatchProcess", userId);
                    if (wo != null)
                    {
                        if (wo.ProductPackage.Product.UsageType == "Credits")
                        {
                            if (IsLicenceAssigned(userId, wo.SubscriptionItemID.Value))
                            {
                                credits = wo.Credits;
                                var val = _context.CustomerLogs.Where(t => t.CompanyId == customerId && t.WorkorderId == wo.Id && t.SearchType != "Lookup").Sum(t => t.CreditPoints);
                                if (val > 0)
                                {
                                    credits = credits - val;
                                }
                            }
                        }
                    }
                }
                else if (serviceType == "LeadGeneration")
                {

                }
            }
            catch (Exception ex)
            {

            }
            //check credits and send email
            SendEmailIfCreditsLow(credits, customerId);
            //
            return credits;
        }

        private void SendEmailIfCreditsLow(int credits, Guid customerId)
        {
            //string customer = getCustomerName();
            //string user = getUserName(customerId);
            try
            {
                if (credits == 49)
                {
                    //send email 
                    //EmailService emailService = new EmailService();
                    //emailService.SendEmail("oniel@tsar.co.za", "Over Limit Searches", "The User X");
                    SendEmail("data@inspirit.co.za", "Over Limit Searches", "The User " + customerId.ToString() + " has gone below the 50 credit buffer.");
                    //await _emailService.SendEmailAsync(usr.Email, subject, message, null, null, null, false, "Logindetail");
                }
            }
            catch (Exception e)
            {

            }
        }

        private string getUserName(Guid customerId)
        {
            try
            {
                //List<CustomerUser> temp = new List<CustomerUser>();
                //var tempData = from x in _dbContext.CustomerUsers
                //       where x.CustomerId == customerId
                //       select new CustomerUser { FirstName = x.FirstName, LastName = x.LastName };
                //    //string user = _dbContext.CustomerUsers.Select(x => new { .ToString();
                List<CustomerUser> recordlist = _dbContext.CustomerUsers.Where(s => s.Id == customerId).ToList();

            }
            catch (Exception e)
            {

            }
            return "";
        }

        private string getCustomerName()
        {
            throw new NotImplementedException();
        }

        public string SendEmail(string To, string subject, string msgBody, string attachmentFilePath = "", string reply = "", BodyBuilder BodyBuilder = null, bool iscontactus = false, string AttachmentName = "")
        {
            string Message = "";

            MailMessage emailMessage = new MailMessage();
            try
            {
                string serverName = _context.ApplicationSetting.First(t => t.SettingName == "Email Server").SettingValue;
                string hostName = _context.ApplicationSetting.First(t => t.SettingName == "Email Server").SettingValue;
                string Email = string.Empty;
                string Credentials = _context.ApplicationSetting.First(t => t.SettingName == "Email UserID").SettingValue;
                if (iscontactus == false)
                    Email = _context.ApplicationSetting.First(t => t.SettingName == "From Email").SettingValue;
                else
                    Email = _context.ApplicationSetting.First(t => t.SettingName == "Contact Us Email Id").SettingValue;

                string Password = _context.ApplicationSetting.First(t => t.SettingName == "Email Password").SettingValue;
                int Port = 587;
                emailMessage.From = new MailAddress(Email);

                foreach (string strTo in To.Split(';'))
                {
                    if (!string.IsNullOrEmpty(strTo))
                    {
                        emailMessage.To.Add(strTo);
                    }
                }
                emailMessage.Subject = subject;

                if (reply != "")
                {
                    emailMessage.ReplyTo = new MailAddress(reply);
                }
                BodyBuilder builder = null;
                if (BodyBuilder == null)
                {
                    builder = new BodyBuilder();
                    builder.HtmlBody = msgBody;
                }
                else
                    builder = BodyBuilder;

                if (attachmentFilePath != "")
                    emailMessage.Attachments.Add(new Attachment(attachmentFilePath));
                emailMessage.IsBodyHtml = true;
                emailMessage.BodyTransferEncoding = System.Net.Mime.TransferEncoding.Base64;


                string message = string.Empty;
                string regularExpressionPattern1 = @"<img (.*?)>";
                Regex regex = new Regex(regularExpressionPattern1, RegexOptions.Singleline);
                MatchCollection collection = regex.Matches(builder.HtmlBody);
                List<Match> m = collection.ToList();
                emailMessage.Body = msgBody;
                if (m.Count > 0)
                {
                    int i = 1;
                    foreach (var con in m)
                    {
                        if (con.ToString().Length > 0)
                        {
                            string msg = emailService.getImage(con.ToString(), i);


                            string basepath = _configuration.GetSection("ApplicationPath").GetSection("Link").Value;

                            string imgpath = basepath + msg;


                            emailMessage.Body = msgBody;//builder.HtmlBody.Replace(con.ToString(), "<img  style=\"width:100%;height:auto\" src=\"" + imgpath + "\"/>");



                            WriteLog("Method Name: " + "Image log", "" + emailMessage.Body);

                            i++;
                        }
                    }
                }
                else
                {
                    emailMessage.Body = builder.HtmlBody;
                }
                using (SmtpClient client = new SmtpClient())
                {

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = serverName;
                    smtp.Port = 587;
                    smtp.Credentials = new System.Net.NetworkCredential(Credentials, Password);
                    smtp.EnableSsl = false;
                    smtp.Send(emailMessage);
                    Message = "Mail Sent";
                    WriteLog("Method Name:   " + emailMessage.Subject, Message);

                }
            }
            catch (Exception ex)
            {
                WriteLog("Method Name:   " + emailMessage.Subject, ex.Message);
            }
            return Message;
        }

        public void WriteLog(string strLog, string message)
        {
            try
            {
                string folderName = "ErrorLogs";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                string fileName = "EmailLog.txt";
                string logFilePath = Path.Combine(newPath, fileName);

                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);

                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine(strLog);
                    sw.WriteLine(message);
                }
            }
            catch (Exception ex) { }
        }
        private Workorder GetWorkorder(Guid customerId, string usageType, Guid CustomerUserId)
        {

            var lisenceAssigned = _context.SubscriptionLicences.Where(p => p.CustomerUserId == CustomerUserId && p.IsActive == true).FirstOrDefault();

            Workorder wo = null;
            if (lisenceAssigned != null)
            {
                if (usageType == "Tracing")
                {
                    wo = _context.Workorders.Include(t => t.ProductPackage).ThenInclude(t => t.Product).ThenInclude(t => t.Service)
                 .FirstOrDefault(t => DateTime.Now >= t.StartDate && DateTime.Now <= t.EndDate && t.CustomerId == customerId && t.SubscriptionItemID == lisenceAssigned.SubscriptionItemId && t.ProductPackage.Product.Service.Code == "Tracing" && t.isCancelled == false);

                }
                else if (usageType == "BatchProcess")
                {
                    wo = _context.Workorders.Include(t => t.ProductPackage).ThenInclude(t => t.Product).ThenInclude(t => t.Service)
                 .FirstOrDefault(t => t.StartDate <= DateTime.Now && DateTime.Now <= t.EndDate && t.CustomerId == customerId && t.ProductPackage.Product.Service.Code == "Batch" && t.isCancelled == false && t.SubscriptionItemID == lisenceAssigned.ID);
                }
                else if (usageType == "LeadGeneration")
                {
                }
            }

            return wo;
        }
        private bool IsLicenceAssigned(Guid userid, Guid subItemId)
        {
            SubscriptionLicense licence = _context.SubscriptionLicences.FirstOrDefault(t => t.CustomerUserId == userid && t.SubscriptionItemId == subItemId && t.IsActive);

            if (licence != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


}
