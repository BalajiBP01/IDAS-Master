using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication
{
    public class UserService
    {
        IDASDbContext _dbContext;

        EmailService _emailService;
        public UserService(IDASDbContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
        }
        public async Task<ActionResult<UserResponse>> UserDataList(UserDataTableRequest request)
        {
            UserResponse response = new UserResponse();
            try
            {

                var users = _dbContext.CustomerUsers.Where(m => m.CustomerId == request.CustomerId).ToList();
                    var lst = _dbContext.CustomerUsers.Where(m => m.CustomerId == request.CustomerId).AsQueryable();
                    int cnt = users.Count();
                    var flt = lst;


                    if (request.dtRequest.search != null && !string.IsNullOrEmpty(request.dtRequest.search.value))
                    {
                        foreach (var user in users)
                        {
                            flt = _dbContext.CustomerUsers.Where(t => t.CustomerId == request.CustomerId && (t.FirstName == request.dtRequest.search.value || t.LastName == request.dtRequest.search.value));

                        }

                    }

                    response.recordsTotal = cnt;
                    response.recordsFiltered = flt.Count();


                    var data = (from s in flt.Skip(request.dtRequest.start).Take(10)
                                select new Users
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
                                    CustomerId = s.CustomerId

                                }).ToList();

                    response.data = data;

                }
            catch (Exception ex)
            {

            }
            return response;


       
            
        }

        public CrudUserResponse Insert(CustomerUser customr)
        {
            CrudUserResponse response = new CrudUserResponse();

            try
            {
                customr.Id = Guid.NewGuid();
               
                customr.Status = "Pending";
                customr.CustomerId = customr.CustomerId;
                _dbContext.CustomerUsers.Add(customr);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }

        public CrudUserResponse Update(CustomerUser customr)
        {
            CrudUserResponse response = new CrudUserResponse();
            try
            {
                _dbContext.CustomerUsers.Update(customr);
                _dbContext.SaveChanges();
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

        public CustomerUser View(Guid id)
        {
            CustomerUser data = new CustomerUser();
            try
            {
                data = _dbContext.CustomerUsers.Find(id);

            }
            catch (Exception ex)
            {

            }
            return data;
        }

        public CrudUserResponse UpdateStatus(CustomerUser cust, string status)
        {
            CrudUserResponse response = new CrudUserResponse();
            try
            {
                var user = _dbContext.CustomerUsers.Find(cust.Id);
                string code = "";
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
                    EmailLoginDetial(cust, code);
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

        public async void EmailLoginDetial(CustomerUser usr, string code)
        {
            var temp = _dbContext.EmailTemplates.First(t => t.Type == "LoginDetial");
            string subject = temp.Subject;
            string message = temp.MailContent;

            message = message.Replace("<email>", usr.Email);

            message = message.Replace("<passwordcode>", code);

           await _emailService.SendEmailAsync(usr.Email, subject, message,null,null,null,false,"Logindetail");
        }



    }
    public class UserResponse : DataTableResponse<Users>
    {

    }
    public class Users
    {

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }


        public string Title { get; set; } //choices - Mr, Miss, Mrs, Ms

        public string IDNumber { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        public string Status { get; set; } //Created,Active,Inactive

        public Guid CustomerId { get; set; }
       
    }

    public class CrudUserResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
     
    public class UserDataTableRequest
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerUserId { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }

}
