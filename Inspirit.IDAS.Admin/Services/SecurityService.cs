
using Inspirit.IDAS.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Inspirit.IDAS.Admin
{
    public class SecurityService
    {
        IConfiguration _config;
        IDASDbContext _context;
        public SecurityService(IDASDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<LoginReponse> AuthUser(LoginRequest request)
        {
            LoginReponse reponse = new LoginReponse();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(t => t.Emailid == request.userName && t.Password == request.password && (t.IsActive == true));

                if (user == null)
                {
                    reponse.errorMessage = "Username or password entered does not exist.";
                    reponse.isSucsess = false;
                }
                else
                {
                    reponse.FullName = user.FirstName + " " + user.LastName;
                    reponse.userid = user.Id;
                    reponse.isSucsess = true;
                }

            }
            catch (Exception ex)
            {
                reponse.errorMessage = ex.Message;
                reponse.isSucsess = false;
            }
            return reponse;
        }
        public async Task<List<SearchCustomerResponse>> GetSearchCustomers(SearchCustomerRequest request)
        {
            List<SearchCustomerResponse> response = new List<SearchCustomerResponse>();
            var Customers = _context.Customers.Where(c => c.Status == "Active");
            try
            {
                if (!String.IsNullOrEmpty(request.CustomerName)
                    && String.IsNullOrEmpty(request.CustomerCode)
                    && String.IsNullOrEmpty(request.RegistrationNumber))
                {
                    response = await (from cs in Customers
                                      where cs.TradingName.StartsWith(request.CustomerName)
                                      select new SearchCustomerResponse
                                      {
                                          CustomerId = cs.Id,
                                          CustomerName = cs.TradingName,
                                          CustomerCode = cs.Code,
                                          RegisterNumber = cs.RegistrationNumber
                                      }).ToListAsync();
                }
                else if (String.IsNullOrEmpty(request.CustomerName)
                    && !String.IsNullOrEmpty(request.CustomerCode)
                    && String.IsNullOrEmpty(request.RegistrationNumber))
                {
                    response = await (from cs in Customers
                                      where cs.Code.StartsWith(request.CustomerCode)
                                      select new SearchCustomerResponse
                                      {
                                          CustomerId = cs.Id,
                                          CustomerName = cs.TradingName,
                                          CustomerCode = cs.Code,
                                          RegisterNumber = cs.RegistrationNumber
                                      }).ToListAsync();
                }
                else if (String.IsNullOrEmpty(request.CustomerName)
                    && String.IsNullOrEmpty(request.CustomerCode)
                    && !String.IsNullOrEmpty(request.RegistrationNumber))
                {
                    response = await (from cs in Customers
                                      where cs.RegistrationNumber.StartsWith(request.RegistrationNumber)
                                      select new SearchCustomerResponse
                                      {
                                          CustomerId = cs.Id,
                                          CustomerName = cs.TradingName,
                                          CustomerCode = cs.Code,
                                          RegisterNumber = cs.RegistrationNumber
                                      }).ToListAsync();
                }
                else if (!String.IsNullOrEmpty(request.CustomerName)
                    && !String.IsNullOrEmpty(request.CustomerCode)
                    && String.IsNullOrEmpty(request.RegistrationNumber))
                {
                    response = await (from cs in Customers
                                      where cs.TradingName.StartsWith(request.CustomerName)
                                      && cs.Code.StartsWith(request.CustomerCode)
                                      select new SearchCustomerResponse
                                      {
                                          CustomerId = cs.Id,
                                          CustomerName = cs.TradingName,
                                          CustomerCode = cs.Code,
                                          RegisterNumber = cs.RegistrationNumber
                                      }).ToListAsync();
                }
                else if (!String.IsNullOrEmpty(request.CustomerName)
                    && String.IsNullOrEmpty(request.CustomerCode)
                    && !String.IsNullOrEmpty(request.RegistrationNumber))
                {
                    response = await (from cs in Customers
                                      where cs.TradingName.StartsWith(request.CustomerName)
                                      && cs.RegistrationNumber.StartsWith(request.RegistrationNumber)
                                      select new SearchCustomerResponse
                                      {
                                          CustomerId = cs.Id,
                                          CustomerName = cs.TradingName,
                                          CustomerCode = cs.Code,
                                          RegisterNumber = cs.RegistrationNumber
                                      }).ToListAsync();
                }
                else if (String.IsNullOrEmpty(request.CustomerName)
                    && !String.IsNullOrEmpty(request.CustomerCode)
                    && !String.IsNullOrEmpty(request.RegistrationNumber))
                {
                    response = await (from cs in Customers
                                      where cs.Code.StartsWith(request.CustomerCode)
                                      && cs.RegistrationNumber.StartsWith(request.RegistrationNumber)
                                      select new SearchCustomerResponse
                                      {
                                          CustomerId = cs.Id,
                                          CustomerName = cs.TradingName,
                                          CustomerCode = cs.Code,
                                          RegisterNumber = cs.RegistrationNumber
                                      }).ToListAsync();
                }
                else if (!String.IsNullOrEmpty(request.CustomerName)
                    && !String.IsNullOrEmpty(request.CustomerCode)
                    && !String.IsNullOrEmpty(request.RegistrationNumber))
                {
                    response = await (from cs in Customers
                                      where cs.TradingName.StartsWith(request.CustomerName)
                                      && cs.Code.StartsWith(request.CustomerCode)
                                      && cs.RegistrationNumber.StartsWith(request.CustomerName)
                                      select new SearchCustomerResponse
                                      {
                                          CustomerId = cs.Id,
                                          CustomerName = cs.TradingName,
                                          CustomerCode = cs.Code,
                                          RegisterNumber = cs.RegistrationNumber
                                      }).ToListAsync();
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        private string BuildToken(User user)
        {
            var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Email, user.Emailid),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<List<Menu>> GetUserMenu(Guid id)
        {

            var usermenus = _context.Users.Include(m => m.UserPermissionslist).Where(m => m.Id == id).FirstOrDefault();
            var isAdmin = usermenus.IsAdmin;
            List<string> menus = usermenus.UserPermissionslist.Select(m => m.FormName).ToList();
            List<string> templist = new List<string>();

            List<Menu> lst = new List<Menu>();

            menus.ForEach(t => templist.Add(t.ToUpper()));

            if (templist.Contains("Dashboard".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Dashboard", Url = "dashboard" });
            }
            if (templist.Contains("Customers".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Customers", Url = "customerlist" });
            }
            if (templist.Contains("Data Service Agreement".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Data Service Agreement", Url = "dsalist" });
            }
            if (templist.Contains("Lookup Data".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Lookup Data", Url = "lookuplist" });
            }
            if (templist.Contains("Email Templates".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Email Templates", Url = "emailtemplatelist" });
            }
            if (templist.Contains("Donot Call Registry".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Donot Call Registry", Url = "dncrlist" });
            }
            if (templist.Contains("Invoice".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Invoice", Url = "invoicelist" });
            }
            if (templist.Contains("Proforma Invoice".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Proforma Invoice", Url = "proformainvoicelist" });
            }
            if (templist.Contains("Payment".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Payment", Url = "paymentlist" });
            }
            if (templist.Contains("Application Message".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Application Message", Url = "applicationmessagelist" });
            }
            if (templist.Contains("Application Settings".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Application Settings", Url = "appsettinglist" });
            }
            if (templist.Contains("Manage Users".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Manage Users", Url = "userlist" });
            }
            if (templist.Contains("Subscriptions".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Subscriptions", Url = "subscribtions" });
            }
            if (templist.Contains("Contact Us".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Contact Us", Url = "contactus" });
            }
            if (templist.Contains("Products".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Products", Url = "productlist" });
            }
            if (templist.Contains("Product Services".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "Product Services", Url = "productservicelist" });
            }
            if (templist.Contains("BatchTracing".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "BatchTracing", Url = "batchtrace" });
            }
            if (templist.Contains("News Blog".ToUpper()) || isAdmin)
            {
                lst.Add(new Menu() { Caption = "News Blog", Url = "news" });
            }
            if(templist.Contains("LeadGeneration".ToUpper()) || isAdmin){
                lst.Add(new Menu() { Caption = "LeadGeneration", Url = "leadlist" });
            }

            return lst;
        }
    }
    public class LoginRequest
    {
        public string userName { get; set; }
        public string password { get; set; }
    }
    public class LoginReponse
    {
        public string FullName { get; set; }
        public Guid userid { get; set; }
        public bool isSucsess { get; set; }
        public string errorMessage { get; set; }
        public string token { get; set; }
    }
    public class SearchCustomerRequest
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string RegistrationNumber { get; set; }
    }
    public class SearchCustomerResponse
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string RegisterNumber { get; set; }
        public bool IsChecked { get; set; }
        public string Message { get; set; }
    }
    public class Menu
    {
        public string Caption { get; set; }

        public string Image { get; set; }

        public string Url { get; set; }

        public bool IsSelected { get; set; }

        public List<Menu> SubMenus { get; set; } = new List<Menu>();
    }
}
