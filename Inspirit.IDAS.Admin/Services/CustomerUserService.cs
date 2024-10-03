
using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.Production;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class CustomerUserService
    {
        IDASDbContext _dbContext;
        EmailService _emailService;
        DAL dAL = new DAL();
        public CustomerUserService(IDASDbContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }
        public async Task<ActionResult<CustomerUserResponse>> CustomerUserDataList(CustomerUserRequest request)
        {
            CustomerUserResponse response = new CustomerUserResponse();
            try
            {

                var lst = _dbContext.CustomerUsers.Where(m => m.CustomerId == request.Id).AsQueryable();
                int cnt = lst.Count();
                var flt = lst;
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();
                var data = (from s in flt
                            select new customerusr
                            {
                                Id = s.Id,
                                FirstName = s.FirstName,
                                LastName = s.LastName,
                                Title = s.Title,
                                IDNumber = s.IDNumber,
                                Email = s.Email,
                                Password = s.Password,
                                IsAdmin = s.IsAdmin,
                                Status = s.Status,
                                CustomerId = s.CustomerId,
                                IsRestricted = s.IsRestricted,
                                LastPasswordResetDate= s.LastPasswordResetDate


                            }).ToList();

                response.data = data;

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public CrudUserResponse Insert(CustomerUserVM customr)
        {
            CrudUserResponse response = new CrudUserResponse();
            if (customr.customeruser.MAchAddressCHK is null)
            {
                customr.customeruser.MAchAddressCHK = false;
            }
            try
            {
                var custuser = _dbContext.CustomerUsers.Where(m => m.Email == customr.customeruser.Email).FirstOrDefault();

                if (custuser == null)
                {
                    customr.customeruser.Id = Guid.NewGuid();
                    customr.customeruser.CreatedBy = customr.IDASuserId;
                    customr.customeruser.CreatedDate = DateTime.Now;
                    customr.customeruser.Status = "Pending";
                    customr.customeruser.IsRestricted = false;
                    customr.customeruser.LastPasswordResetDate = DateTime.Now;
                    _dbContext.CustomerUsers.Add(customr.customeruser);
                    _dbContext.SaveChanges();
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "User already exists";
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
   
        public CrudUserResponse Update(CustomerUserVM customr)
        {
            if (customr.customeruser.MAchAddressCHK is null)
            {
                customr.customeruser.MAchAddressCHK = false;
            }
            CrudUserResponse response = new CrudUserResponse();
            try
            {
                var userDate = _dbContext.CustomerUsers.Where(m => m.Email == customr.customeruser.Email).FirstOrDefault();
                customr.customeruser.ModifiedBy = customr.IDASuserId;
                customr.customeruser.ModifiedDate = DateTime.Now;
                var custuser = _dbContext.CustomerUsers.Where(m => m.Id != customr.customeruser.Id && m.Email == customr.customeruser.Email).FirstOrDefault();

                if (custuser == null)
                {
                    customr.customeruser.LastPasswordResetDate = userDate.LastPasswordResetDate;
                    _dbContext.Entry(userDate).State = EntityState.Detached;
                    _dbContext.CustomerUsers.Update(customr.customeruser);
                    _dbContext.SaveChanges();
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "User already exists";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }

        public CrudUserResponse Delete(Guid id)
        {
            CrudUserResponse response = new CrudUserResponse();
            try
            {
                var data = _dbContext.CustomerUsers.FirstOrDefault(t => t.Id == id);
                _dbContext.CustomerUsers.Remove(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public CustomerUserVM View(Guid id)
        {
            CustomerUserVM data = new CustomerUserVM();
            try
            {
                data.customeruser = _dbContext.CustomerUsers.Find(id);

            }
            catch (Exception ex)
            {

            }
            return data;
        }


        public async Task<CustomerCrudResponse> UpdateStatusAsync(CustomerUserVM cust, string status)
        {
            CustomerCrudResponse response = new CustomerCrudResponse();
            try
            {
                var user = _dbContext.CustomerUsers.Find(cust.customeruser.Id);
                string code = "";
                user.ActivatedBy = cust.IDASuserId;
                user.ActivatedDate = DateTime.Now;
                if (user.Status == "Pending") // when user is activated for first time
                {
                    code = GenerateCode();
                    user.Password = code;
                }
                user.Status = status;

                _dbContext.CustomerUsers.Update(user);
                _dbContext.SaveChanges();

                if (!string.IsNullOrEmpty(code))
                {
                    response.Message = await EmailLoginDetialAsync(cust.customeruser, code);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        private string GenerateCode()
        {
            string code = string.Empty;
            Random random = new Random();
            do
            {
                int num = random.Next(48, 90); // 0-9,A-Z
                if (!(num > 57 && num < 65))
                    code += Convert.ToChar(num);
            }
            while (code.Length < 10);
            return code;
        }
        public async Task<string> EmailLoginDetialAsync(CustomerUser usr, string code)
        {
            var temp = _dbContext.EmailTemplates.First(t => t.Type == "LoginDetial");
            string subject = temp.Subject;
            string message = temp.MailContent;

            message = message.Replace("&lt;email&gt;", usr.Email);

            message = message.Replace("&lt;passwordcode&gt;", code);



            var builder = new BodyBuilder();
            builder.HtmlBody += "<br />Greetings, <br />";
            builder.HtmlBody += message;
            builder.HtmlBody += "<br />Regards<br />";
            builder.HtmlBody += "<br />IDAS Team <br />";

            string returnmessage = await _emailService.SendEmailAsync(usr.Email, subject, "", "", "", builder, "Customer");

            return returnmessage;
        }

    }
    public class customerusr
    {

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }


        public string Title { get; set; } //choices - Mr, Miss, Mrs, Ms

        public string IDNumber { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsAdmin { get; set; }
        public bool? IsRestricted { get; set; }
        public DateTime? LastPasswordResetDate { get; set; }

        public string Status { get; set; } //Created,Active,Inactive

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    public class CustomerVm
    {
        public Guid Id { get; set; }

        //Company Information

        public string TradingName { get; set; }

        public string RegistrationName { get; set; }

        public string RegistrationNumber { get; set; }

        public string VATNumber { get; set; }

        public string BranchLocation { get; set; }

        public string PhysicalAddress { get; set; }


        public string TypeOfBusiness { get; set; }// choices - ref. to  doc

        public string TelephoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public string BillingEmail { get; set; }

        public string Status { get; set; } //Pending,Active,Inactive

        public string BillingType { get; set; } //Credit,Invoice
    }

    public class CustomerUserVM
    {
        public Guid IDASuserId { get; set; }
        public CustomerUser customeruser { get; set; }
    }






    public class CustomerUserResponse : DataTableResponse<customerusr>
    {

    }

    public class CustomerUserRequest
    {

        public Guid Id { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }


    public class CrudUserResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }


}
