using Inspirit.IDAS.Data;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Linq;
using Inspirit.IDAS.Data.IDAS;
using MimeKit;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Data;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Inspirit.IDAS.WebApplication
{
    public class SecurityService
    {
        IDASDbContext _context;
        IConfiguration _config;
        EmailService _emailService;

        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;


        [DllImport("Iphlapi.dll")]
        private static extern int SendARP(int dest, int ScrIP, byte[] pMacAddr, ref int PhyAddrLen);

        public SecurityService(IDASDbContext context, IConfiguration config, EmailService emailService, IConfiguration iconfig, IHostingEnvironment ihostingEnvironment)
        {
            _context = context;
            _configuration = iconfig;
            _emailService = emailService;
            _hostingEnvironment = ihostingEnvironment;

        }
        public async Task<string> GetAdminUrl()
        {
            string url = string.Empty;
            url = _configuration.GetSection("Admin").GetSection("url").Value;
            return url;
        }
        public static PhysicalAddress GetMacAddress(IPAddress ipaddress)
        {
            const int MacAddressLength = 6;
            int length = MacAddressLength;
            var macBytes = new byte[MacAddressLength];
            SendARP(BitConverter.ToInt32(ipaddress.GetAddressBytes(), 0), 0, macBytes, ref length);
            return new PhysicalAddress(macBytes);
        }
        public async Task<LoginReponse> AuthUser(LoginRequest request, IPAddress iP)
        {
            LoginReponse response = new LoginReponse();
            try
            {

                var user = await _context.CustomerUsers.Include(t => t.Customer).FirstOrDefaultAsync(t => t.Email == request.userName && t.Password == request.password);
                //var machAddress = GetMacAddress(iP);

                //get to test by ip address
                //bool verified = checkMacAddress(iP.ToString(), user.MAchAddressCHK, user.MacAddresses);
                if (user == null)
                {
                    var tuser = _context.TrailUsers.Where(t => t.EmailAddress == request.userName && t.Password == request.password).FirstOrDefault();
                    if (tuser != null)
                    {
                        DateTime ExDate = tuser.Date.AddHours(48);
                        if (DateTime.Now < ExDate)
                        {
                            response.UserID = tuser.ID;
                            response.CustomerID = tuser.ID;
                            response.IsSucsess = true;
                            response.Token = tuser.ID.ToString();
                            response.UserName = tuser.FirstName + " " + tuser.Surname;
                            response.isTrialuser = true;
                        }
                        else
                        {
                            response.ErrorMessage = "Username or password expired.";
                            response.IsSucsess = false;
                        }
                    }
                    else
                    {
                        response.ErrorMessage = "Username or password entered does not exist.";
                        response.IsSucsess = false;
                    }
                }
                else if (user.Status.ToUpper().Trim() != "ACTIVE" || user.Customer.Status.Trim().ToUpper() != "ACTIVE")
                {
                    response.ErrorMessage = "User is locked. Contact IDAS Admin.";
                    response.IsSucsess = false;
                }
                else
                {
                    bool verified = checkMacAddress(iP.ToString(), user.MAchAddressCHK, user.MacAddresses);
                    if (verified == true)
                    {
                        int Timestamp = 0;
                        if (user.LastLoginDate != null)
                        {
                            Timestamp = (Int32)(DateTime.Now.Subtract(user.LastLoginDate.Value)).TotalMinutes;
                        }
                        else
                        {
                            Timestamp = 6;
                        }

                        if (user.IsUserLoggedIn != true || Timestamp > 5)
                        {
                            var adminuser = _context.Users.Where(t => t.Emailid == request.userName).FirstOrDefault();
                            if (adminuser != null)
                                response.isIdasUser = true;
                            else
                            response.isIdasUser = false;
                            response.UserID = user.Id;
                            response.CustomerID = user.CustomerId;
                            response.IsSucsess = true;
                            response.Token = user.Id.ToString();
                            response.UserName = user.FirstName + " " + user.LastName;
                            response.isTrialuser = false;
                            // Revert back to false
                            // krishna start

                            response.isXDS = false;
                            //response.isXDS = true;
                            // krishna end

                            user.LastLoginDate = DateTime.Now;
                            user.IsUserLoggedIn = true;
                            _context.CustomerUsers.Update(user);
                            _context.SaveChanges();
                        }
                        else if (Timestamp <= 5 && user.IsUserLoggedIn == true)
                        {
                            response.ErrorMessage = "Found another active session, kindly try after 5 minutes or contact administrator.";
                            response.IsSucsess = false;
                        }
                        else
                        {
                            response.ErrorMessage = "User Already Logged In";
                            response.IsSucsess = false;
                        }
                    }
                    else
                    {
                        response.ErrorMessage = "" + iP.ToString() + " : IP Address does not match username. Contact your Administrator";
                        response.IsSucsess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// krishna xds token authentication
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LoginReponse> AuthXdsUser(string request)
        {
            LoginReponse response = new LoginReponse();
            // get token details
            XdsTokenData xdsTokenData = ValidateJwtToken(request);
            if(xdsTokenData == null)
            {
                response.IsSucsess = false;
                response.ErrorMessage = "Invalid token";
                return response;
            }

            try
            {
                bool userExists = await _context.CustomerUsers.AllAsync(u => u.Email == xdsTokenData.Username);
                if(userExists == false)
                {
                    // create user
                    Customer customerExists = await _context.Customers.Where(c => c.SubscriberId == xdsTokenData.SubscriberId).FirstOrDefaultAsync();
                    if(customerExists == null) 
                    {
                        response.IsSucsess = false;
                        response.ErrorMessage = "Invalid customer subscriberid";
                        return response;
                    }

                    // create user pending
                    // code here
                    CustomerUser customeruser = new CustomerUser();
                    customeruser.FirstName = xdsTokenData.Firstname;
                    customeruser.LastName = xdsTokenData.Surname;
                    customeruser.Title = xdsTokenData.Title;

                    customeruser.IDNumber = Guid.NewGuid().ToString();
                    customeruser.Email = xdsTokenData.Username;
                    customeruser.Password = "Xdsuserpassword@123";
                    customeruser.IsAdmin = false;
                    customeruser.CustomerId = customerExists.Id;

                    // issue guid = string
                    //public Guid? SubscriptionId { get; set; }
                    //customeruser.SubscriptionId = xdsTokenData.SubscriberId.ToString();
                    customeruser.Code = "XDS";
                    customeruser.ActivatedBy = Guid.NewGuid();
                    customeruser.ActivatedDate = DateTime.Now;
                    customeruser.MacAddresses = Guid.NewGuid().ToString();
                    customeruser.Message = "XDS";
                    customeruser.PhoneNumber = "";
                    //
                    customeruser.Id = Guid.NewGuid();
                    customeruser.CreatedBy = Guid.NewGuid();
                    customeruser.CreatedDate = DateTime.Now;
                    customeruser.Status = "ACTIVE";
                    customeruser.IsRestricted = false;
                    customeruser.LastPasswordResetDate = DateTime.Now;
                    _context.CustomerUsers.Add(customeruser);
                    _context.SaveChanges();
                }
                else
                {
                    bool customerExists = await _context.Customers.AllAsync(c => c.SubscriberId == xdsTokenData.SubscriberId);
                    if (customerExists == false)
                    {
                        response.IsSucsess = false;
                        response.ErrorMessage = "Invalid customer subscriberid";
                        return response;
                    }
                }

                var user = await _context.CustomerUsers.Include(t => t.Customer).FirstOrDefaultAsync(t => t.Email == xdsTokenData.Username);
                //var user = await _context.CustomerUsers.Include(t => t.Customer).FirstOrDefaultAsync(t => t.Email == request.userName && t.Password == request.password);
                
                // krishna ip check commented
                //var machAddress = GetMacAddress(iP);

                //get to test by ip address
                //bool verified = checkMacAddress(iP.ToString(), user.MAchAddressCHK, user.MacAddresses);
                if (user == null)
                {
                    //var tuser = _context.TrailUsers.Where(t => t.EmailAddress == request.userName && t.Password == request.password).FirstOrDefault();
                    var tuser = _context.TrailUsers.Where(t => t.EmailAddress == xdsTokenData.Username).FirstOrDefault();
                    if (tuser != null)
                    {
                        DateTime ExDate = tuser.Date.AddHours(48);
                        if (DateTime.Now < ExDate)
                        {
                            response.UserID = tuser.ID;
                            response.CustomerID = tuser.ID;
                            response.IsSucsess = true;
                            response.Token = tuser.ID.ToString();
                            response.UserName = tuser.FirstName + " " + tuser.Surname;
                            response.isTrialuser = true;
                        }
                        else
                        {
                            response.ErrorMessage = "Username or password expired.";
                            response.IsSucsess = false;
                        }
                    }
                    else
                    {
                        response.ErrorMessage = "Username or password entered does not exist.";
                        response.IsSucsess = false;
                    }
                }
                else if (user.Status.ToUpper().Trim() != "ACTIVE" || user.Customer.Status.Trim().ToUpper() != "ACTIVE")
                {
                    response.ErrorMessage = "User is locked. Contact IDAS Admin.";
                    response.IsSucsess = false;
                }
                else
                {
                    //bool verified = checkMacAddress(iP.ToString(), user.MAchAddressCHK, user.MacAddresses);
                    bool verified = true;
                    if (verified == true)
                    {
                        int Timestamp = 0;
                        if (user.LastLoginDate != null)
                        {
                            Timestamp = (Int32)(DateTime.Now.Subtract(user.LastLoginDate.Value)).TotalMinutes;
                        }
                        else
                        {
                            Timestamp = 6;
                        }

                        if (user.IsUserLoggedIn != true || Timestamp > 5)
                        {
                            var adminuser = _context.Users.Where(t => t.Emailid == xdsTokenData.Username).FirstOrDefault();
                            if (adminuser != null)
                                response.isIdasUser = true;
                            else
                                response.isIdasUser = false;
                            response.UserID = user.Id;
                            response.CustomerID = user.CustomerId;
                            response.IsSucsess = true;
                            response.Token = user.Id.ToString();
                            response.UserName = user.FirstName + " " + user.LastName;
                            response.isTrialuser = false;

                            response.isXDS = true;

                            user.LastLoginDate = DateTime.Now;
                            user.IsUserLoggedIn = true;
                            _context.CustomerUsers.Update(user);
                            _context.SaveChanges();
                        }
                        else if (Timestamp <= 5 && user.IsUserLoggedIn == true)
                        {
                            response.ErrorMessage = "Found another active session, kindly try after 5 minutes or contact administrator.";
                            response.IsSucsess = false;
                        }
                        else
                        {
                            response.ErrorMessage = "User Already Logged In";
                            response.IsSucsess = false;
                        }
                    }
                    else
                    {
                        // krishna commented
                        //response.ErrorMessage = "" + iP.ToString() + " : IP Address does not match username. Contact your Administrator";
                        response.IsSucsess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }
            return response;
        }


        public XdsTokenData ValidateJwtToken(string token)
        {
            if (token == null)
                return null;
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWTToken:Secret"]); //xwry2432340DFGS0345hfsaWOPUIY156740

            //var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidAudience = _configuration["JWT:ValidAudience"],

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                //var subscriberId = jwtToken.Claims.First(x => x.Type == "SubscriberId").Value;
                //var userName = jwtToken.Claims.First(x => x.Type == "Username").Value;

                XdsTokenData xdsTokenData = new XdsTokenData();
                xdsTokenData.Username = jwtToken.Claims.First(x => x.Type == "Username").Value;
                xdsTokenData.SubscriberId = jwtToken.Claims.First(x => x.Type == "SubscriberId").Value;
                xdsTokenData.Firstname = jwtToken.Claims.First(x => x.Type == "Firstname").Value;
                xdsTokenData.Surname = jwtToken.Claims.First(x => x.Type == "Surname").Value;
                xdsTokenData.EmailAddress = jwtToken.Claims.First(x => x.Type == "EmailAddress").Value;
                xdsTokenData.Title = jwtToken.Claims.First(x => x.Type == "Title").Value;
                return xdsTokenData;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }

        private string getMachAddress()
        {
            //string userip = this.HttpContext.Connection;//Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.
            return "";
        }

        private bool checkMacAddress(string v, bool? mAchAddressCHK, string macAddresses)
        {
            bool verifiedMac = false;
            if (mAchAddressCHK == false)
            {
                return true;
            }
            else
            {
                return CheckMacAddressList(v, macAddresses);
            }

            return verifiedMac;
        }

        private bool CheckMacAddressList(string v, string macAddresses)
        {
            bool verifiedMAc = false;
            string[] Macaddresses_List = macAddresses.Split(';');

            for (int i = 0; i < Macaddresses_List.Length - 1; i++)
            {
                if (v == Macaddresses_List[i])
                {
                    return true;
                }
            }
            return verifiedMAc;
        }

        public string Logout(Guid id, bool istrailuser)
        {
            CustomerUser user = new CustomerUser();
            if (istrailuser == false)
            {
                user = _context.CustomerUsers.Where(t => t.Id == id).FirstOrDefault();

            }
            user.IsUserLoggedIn = false;
            _context.CustomerUsers.Update(user);
            _context.SaveChanges();
            return "User successfully logged off";

        }
        public async Task<LoginReponse> TrailuserAuth(LoginRequest request)
        {
            LoginReponse response = new LoginReponse();
            try
            {



                var user = await _context.TrailUsers.FirstOrDefaultAsync(t => t.EmailAddress == request.userName && t.Password == request.password);



                if (user == null)
                {
                    response.ErrorMessage = "Username or password entered does not exist.";
                    response.IsSucsess = false;
                }
                else
                {
                    DateTime ExDate = user.Date.AddHours(48);
                    if (DateTime.Now < ExDate)
                    {

                        response.UserID = user.ID;
                        response.CustomerID = user.ID;
                        response.IsSucsess = true;
                        response.Token = user.ID.ToString();
                        response.UserName = user.FirstName + " " + user.Surname;

                    }
                    else
                    {
                        response.ErrorMessage = "Username and password expired";
                        response.IsSucsess = false;
                    }

                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
        private void SaveCustDSA(Guid userid, int dsaVersion)
        {
            Customer cust = _context.CustomerUsers.Include(t => t.Customer).FirstOrDefaultAsync(t => t.Id == userid).Result.Customer;
            DataServicesAgreement dsa = _context.DataServicesAgreements.FirstOrDefault(t => t.Version == dsaVersion);
            CustomerDSA custDsa = new CustomerDSA();
            custDsa.Id = Guid.NewGuid();
            custDsa.CustomerId = cust.Id;
            custDsa.DataServicesAgreementId = dsa.Id;
            _context.CustomerDSAs.Add(custDsa);
            _context.SaveChanges();

        }
        private bool IsDSAUpdated(Guid userid)
        {
            bool isUpdated = false;

            Customer cust = _context.CustomerUsers.Include(t => t.Customer).FirstOrDefaultAsync(t => t.Id == userid).Result.Customer;
            DataServicesAgreement dsa = _context.DataServicesAgreements.OrderBy(t => t.Version).FirstOrDefault();

            CustomerDSA custDsa = _context.CustomerDSAs.FirstOrDefault(t => t.CustomerId == cust.Id && t.DataServicesAgreementId == dsa.Id);
            if (custDsa == null)
            {
                isUpdated = false;
            }
            else
            {
                isUpdated = true;
            }

            return isUpdated;
        }
        public async Task<List<LookupData>> GetLookupDatas()
        {
            List<LookupData> lookupDatas = new List<LookupData>();
            try
            {
                lookupDatas = await _context.LookupDatas
                    .Where(l => l.IsActive == true && l.Type == "Type of Business").ToListAsync();
            }
            catch (Exception ex) { }
            return lookupDatas;
        }
        public async Task<List<CustomerLog>> GetLogs(string userid)
        {
            List<CustomerLog> customerLogs = new List<CustomerLog>();
            try
            {

                DateTime filterFromDate = new DateTime(Convert.ToDateTime(DateTime.Now).Year, Convert.ToDateTime(DateTime.Now).Month, Convert.ToDateTime(DateTime.Now).Day, 0, 0, 0);

                DateTime FilterToDate = new DateTime(Convert.ToDateTime(DateTime.Now).Year, Convert.ToDateTime(DateTime.Now).Month, Convert.ToDateTime(DateTime.Now).Day, 23, 59, 59);



                if (!String.IsNullOrEmpty(userid))
                {
                    //customerLogs = await _context.CustomerLogs.
                    //    Where(l => l.CompanyUserId == (Guid.Parse(userid)) && (l.DateTime >= filterFromDate && l.DateTime <= FilterToDate) && l.CreditPoints == 1).
                    //    OrderByDescending(l => l.DateTime).Take(10).ToListAsync();
                    customerLogs = await _context.CustomerLogs.
                    Where(l => l.CompanyUserId == (Guid.Parse(userid)) && (l.DateTime >= filterFromDate && l.DateTime <= FilterToDate) && l.CreditPoints == 1).
                    OrderByDescending(l => l.DateTime).Take(20).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return customerLogs;
        }
        public async Task<List<TrailUserLog>> GetTrailUserLogs(string userid)
        {
            List<TrailUserLog> userLogs = new List<TrailUserLog>();
            try
            {

                DateTime filterFromDate = new DateTime(Convert.ToDateTime(DateTime.Now).Year, Convert.ToDateTime(DateTime.Now).Month, Convert.ToDateTime(DateTime.Now).Day, 0, 0, 0);

                DateTime FilterToDate = new DateTime(Convert.ToDateTime(DateTime.Now).Year, Convert.ToDateTime(DateTime.Now).Month, Convert.ToDateTime(DateTime.Now).Day, 23, 59, 59);



                if (!String.IsNullOrEmpty(userid))
                {
                    userLogs = await _context.TrailUserLogs.
                        Where(l => l.TrailUserId == (Guid.Parse(userid)) && (l.Date >= filterFromDate && l.Date <= FilterToDate)).
                        OrderByDescending(l => l.Date).Take(10).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return userLogs;
        }
        public async Task<DataServicesAgreement> GetDSA(string userid)
        {
            DataServicesAgreement dsa = null;
            if (String.IsNullOrEmpty(userid))
            {
                dsa = await _context.DataServicesAgreements
                    .Where(t => t.IsPublished == true)
                    .FirstOrDefaultAsync();
            }
            else if (IsDSAUpdated(Guid.Parse(userid)))
            {
                dsa = await _context.DataServicesAgreements
                    .Where(t => t.IsPublished == true)
                    .FirstOrDefaultAsync();
            }
            return dsa;
        }
        public async Task<SignUpResponse> RegistrationNumberVerify(SignUpRequest request)
        {
            SignUpResponse response = new SignUpResponse();

            Customer customer = _context.Customers.FirstOrDefault(t => t.RegistrationNumber == request.customer.RegistrationNumber);

            if (customer != null)
            {
                response.Message = "Registration number already exists";
                response.isSucsess = false;
            }
            else
            {
                response.Message = "";
                response.isSucsess = true;
            }
            return response;
        }
        private string BuildToken(CustomerUser user)
        {
            var claims = new[] {

        new Claim(JwtRegisteredClaimNames.Email, user.Email),
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
        public async Task<SignUpResponse> Register(SignUpRequest request)
        {
            SignUpResponse reponse = new SignUpResponse();
            try
            {
                Customer customer = _context.Customers
                    .FirstOrDefault(t => t.RegistrationNumber == request.customer.RegistrationNumber);
                if (customer != null)
                {
                    reponse.isSucsess = false;
                    reponse.Message = "Registration number already exists";
                }
                else
                {
                    request.customer.Status = "Pending";
                    request.customer.CreatedDate = DateTime.Now;
                    string strCode = string.Empty;
                    string strTemp = request.customer.TradingName;
                    strTemp = strTemp.Replace(" ", "");
                    int count = 0;
                    if (strTemp.Length < 6)
                    {
                        strCode = AddZero(strTemp, "Right", 6);
                        count = _context.Customers.Where(t => strTemp.Contains(t.TradingName.Substring(0, strTemp.Length))).Count();
                    }
                    else
                    {
                        strCode = strTemp.Substring(0, 6);
                        count = _context.Customers.Where(t => strTemp.Substring(0, 6).StartsWith(t.TradingName.Substring(0, 6))).Count();
                    }
                    count += 1;
                    if (count.ToString().Length != 3)
                        strCode = strCode + AddZero(count.ToString(), "Left", 3);
                    request.customer.Code = strCode;
                    CustomerUser user = new CustomerUser();
                    user.Id = Guid.NewGuid();
                    user.CreatedDate = DateTime.Now;
                    user.CustomerId = request.customer.Id;
                    user.Email = request.emailid;
                    user.FirstName = request.firstName;
                    user.IDNumber = request.iDNumber;
                    user.IsAdmin = true;

                    user.LastName = request.lastName;
                    user.Status = "Pending";
                    user.Title = request.title;
                    user.LastLoginDate = DateTime.MinValue;
                    user.ActivatedDate = DateTime.MinValue;
                    user.PhoneNumber = request.contactNumber;
                    request.customer.TabSelected = "ConsumerTimeline,ConsumerProfile,ConsumerTelephone,ConsumerAddress,ConsumerEmployment,ConsumerProperty,ConsumerJudgement,ConsumerDebtReview,ConsumerDirector,ConsumerRelationship,CommercialTimeline,CommercialProfile,CommercialTelephone,CommercialAddress,CommercialJudgement,CommercialProperty,CommercialAuditor,CommercialDirector";
                    _context.Customers.Add(request.customer);
                    _context.CustomerUsers.Add(user);
                    await _context.SaveChangesAsync();
                    SaveCustDSA(user.Id, Convert.ToInt32(request.dsaVersion));
                    reponse.isSucsess = true;
                    reponse.Message = "successfully registered.";
                }
            }
            catch (Exception ex)
            {
                reponse.isSucsess = false;
                reponse.Message = ex.Message;
            }
            return reponse;
        }
        public async Task<string> SendMail(string toEmail,  string attachmentFilePath)
        {
            EmailTemplate email = _context.EmailTemplates.FirstOrDefault(t => t.Type == "SignUp");
            string Message = "";
            if (email != null)
            {
                var builder = new BodyBuilder();
                builder.HtmlBody += email.MailContent;
                Message = await _emailService.SendEmailAsync(toEmail, "New Customer", "", attachmentFilePath, "", builder, false, "Register");
            }
            else
            {
                Message = "No contents founds on Email Template";
            }
            return Message;
        }
        public async Task<DonoCallRegistryResponse> callregistry(DonotCallRegistryRequest request)
        {
            DonoCallRegistryResponse reponse = new DonoCallRegistryResponse();
            MailMessage mail = new MailMessage();
            SmtpClient sc = new SmtpClient();
            try
            {
                EmailTemplate email = _context.EmailTemplates.FirstOrDefault(t => t.Type == "Donot call Registry Request");

                DoNotCallRegistry reg = new DoNotCallRegistry();
                reg.Id = Guid.NewGuid();
                reg.CurrentDate = DateTime.Today;
                reg.Emailid = request.emailaddress;
                reg.Idnumber = request.iDNumber;
                reg.PhoneNumber = request.phonenumber;
                reg.Surname = request.surname;
                reg.Name = request.name;
                _context.DoNotCallRegistrys.Add(reg);
                await _context.SaveChangesAsync();
                var builder = new BodyBuilder();
                builder.HtmlBody += email.MailContent;

                await _emailService.SendEmailAsync(request.emailaddress, email.Subject, "", "", "", builder, false, "DonotCall");


                reponse.errorMessage = "Saved Successfully";
                reponse.isSucsess = true;
            }
            catch (Exception ex)
            {
                reponse.isSucsess = false;
                reponse.errorMessage = ex.Message;

            }
            return reponse;
        }
        public async Task<LoginReponse> ForgotPassword(string email)
        {
            LoginReponse reponse = new LoginReponse();
            MailMessage mail = new MailMessage();//new MailMessage(from, to);  this is one method
            SmtpClient sc = new SmtpClient();//new SmtpClient("smtp.gmail.com", 587); this is one method
            try
            {
                var username = _context.CustomerUsers.Include(t => t.Customer).FirstOrDefault(t => t.Email == email);
                if (username == null)
                {
                    reponse.ErrorMessage = "Username entered does not exist.";
                    reponse.IsSucsess = false;
                }
                else
                {
                    string code = GenerateCode();
                    username.Password = code;
                    username.IsUserLoggedIn = false;
                    _context.Update(username);
                    _context.SaveChanges();
                   

                    EmailTemplate emails = _context.EmailTemplates.FirstOrDefault(t => t.Type == "Reset Password");
                    string message = emails.MailContent;

                    message = message.Replace("&lt;username&gt;", username.FirstName + " " + username.LastName);
                    message = message.Replace("&lt;email&gt;", username.Email);
                    message = message.Replace("&lt;password&gt;", code);

                    var builder = new BodyBuilder();
                    builder.HtmlBody += message;
                    var admin = _context.CustomerUsers.FirstOrDefault(m => m.Customer.Id == username.CustomerId && m.IsAdmin == true);
                  

                    if (admin != null)
                    {
                        await _emailService.SendEmailAsync(admin.Email, "IDAS Password reset", "", "", "", builder, false, "ResetPassword");
                        await _emailService.SendEmailAsync(username.Email, "IDAS Password reset", "", "", "", builder, false, "ResetPassword");

                

                        reponse.ErrorMessage = "Password has been sent to your Admin.";
                        reponse.IsSucsess = true;
                    }
                    else
                    {
                        reponse.ErrorMessage = "Please contact your Administrator.";
                        reponse.IsSucsess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                reponse.IsSucsess = false;
                reponse.ErrorMessage = ex.Message;
            }
            return reponse;
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
        private string GetToken()
        {
            string token = string.Empty;

            return token;
        }
        public async Task<List<Menu>> GetUserMenu(Guid id, int windowsize)
        {

            List<Menu> lst = new List<Menu>();

            lst.Add(new Menu() { Caption = "Dashboard", Url = "dashboard" });


            SqlConnection con = (SqlConnection)_context.Database.GetDbConnection();
            using (con)
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                DataTable dataTable = new DataTable();
                SqlCommand cmd = new SqlCommand("Retrieve_user_menu", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CustUserID", id));
                cmd.CommandTimeout = 0;
                try
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    SqlDataAdapter sDA = new SqlDataAdapter();
                    sDA.SelectCommand = cmd;
                    sDA.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            bool isbatchsub = false;
                            if (dr["BatchwithoutSub"] != DBNull.Value)
                            {
                                isbatchsub = (bool)dr["BatchwithoutSub"];
                            }
                            bool isleadsub = false;
                            if (dr["LeadswithoutSub"] != DBNull.Value)
                            {
                                isleadsub = (bool)dr["LeadswithoutSub"];
                            }

                            if (dr["Name"] != DBNull.Value)
                            {
                                string servicename = (string)dr["Name"];
                                if (servicename == "Tracing")
                                {
                                    lst.Add(new Menu() { Caption = "Tracing", Url = "tracingSearch" });
                                }
                                if ((servicename == "Batch" || isbatchsub == true) && windowsize > 768)
                                {
                                    lst.Add(new Menu() { Caption = "Batch Generation", Url = "batchprocess" });
                                }
                                if ((servicename == "Lead Generation" || isleadsub == true) && windowsize > 768)
                                {
                                    lst.Add(new Menu() { Caption = "Lead Generation", Url = "leadGeneration" });
                                }

                            }

                            if (dr["IsAdmin"] != DBNull.Value)
                            {
                                bool isadmin = (bool)dr["IsAdmin"];
                                if (isadmin == true && windowsize > 768)
                                {
                                    lst.Add(new Menu() { Caption = "Users", Url = "userlist" });
                                    lst.Add(new Menu() { Caption = "Subscriptions", Url = "Subscriptions" });
                                    lst.Add(new Menu() { Caption = "Invoices", Url = "invoicelist" });
                                }
                            }
                        }
                    }

                    else
                    {
                        var trailuser = _context.TrailUsers.Where(t => t.ID == id).FirstOrDefault();
                        if (trailuser != null)
                        {
                            lst.Add(new Menu() { Caption = "Tracing", Url = "tracingSearch" });
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return lst;
        }
        public LoginReponse ResetPassword(string username, string oldpassword, string password)
        {
            LoginReponse res = new LoginReponse();
            if (string.IsNullOrEmpty(username))
            {

            }
            else
            {
                var usr = _context.CustomerUsers.FirstOrDefault(t => t.Email == username && t.Password == oldpassword);
                if (usr != null)
                {
                    usr.Password = password;
                    usr.Code = "";
                    usr.IsUserLoggedIn = false;
                    _context.CustomerUsers.Update(usr);
                    _context.SaveChanges();

                    res.ErrorMessage = "Password reset";
                    res.IsSucsess = true;
                }
                else
                {
                    res.ErrorMessage = "Reset failed";
                    res.IsSucsess = false;
                }

            }
            return res;
        }
        public List<ApplicationMessages> GetApplicationMessage(Guid id)
        {
            List<ApplicationMessages> msgs = new List<ApplicationMessages>();

            try
            {
                var usrmessages = _context.CustomerUsers.Where(m => m.Id == id).FirstOrDefault();

                string messages = usrmessages.Message;

                if (!string.IsNullOrEmpty(messages))
                {
                    string[] values = messages.Split(',');
                    foreach (var val in values)
                    {
                        var msg = _context.ApplicationMessages.Where(m => m.Id == new Guid(val) && m.Showmessage == true).FirstOrDefault();

                        var message = new ApplicationMessages();
                        message.id = msg.Id;
                        message.message = msg.Message;
                        msgs.Add(message);
                    }
                }


            }
            catch (Exception ex)
            {
                var m = ex.Message;
            }
            return msgs;
        }
        public string RemoveMessage(Guid userId)
        {
            string Message = string.Empty;
            try
            {
                var usrmessages = _context.CustomerUsers.Where(t => t.Id == userId).FirstOrDefault();
                usrmessages.Message = null;
                _context.CustomerUsers.Update(usrmessages);
                _context.SaveChanges();
                Message = "success";
            }
            catch (Exception ex)
            {

            }
            return Message;
        }
        public string AddZero(string strInput, string strWhere, int length)
        {
            string strOutput = strInput;
            while (strOutput.Length != length)
            {
                if (strWhere.ToUpper() == "LEFT")
                    strOutput = "0" + strOutput;
                else
                    strOutput += "0";
            }
            return strOutput;
        }

        public string GetElasticIp()
        {
            return _context.ApplicationSetting.Where(t => t.SettingName == "Elastic DB IP").Select(t => t.SettingValue).FirstOrDefault();
        }
        public void WriteLog(string strLog, string errorName)
        {
            try
            {
                string folderName = "ErrorLogs";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                string fileName = errorName + ".txt";
                string logFilePath = Path.Combine(newPath, fileName);

                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);

                using (StreamWriter OurStream = File.CreateText(logFilePath))
                {
                    OurStream.WriteLine(strLog);
                    OurStream.Close();
                }
            }
            catch (Exception ex) { }
        }
    }
    public class LoginRequest
    {
        public string userName { get; set; }
        public string password { get; set; }
    }

    public class XdsLoginRequest
    {
        public string token { get; set; }
    }

    public class LoginReponse
    {
        public bool IsSucsess { get; set; }
        public string ErrorMessage { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }

        public bool ShowDSA { get; set; }

        public Guid UserID { get; set; }

        public Guid CustomerID { get; set; }

        public int Credits { get; set; }
        public bool isIdasUser { get; set; }
        public string UserIpaddress { get; set; }
        public bool isTrialuser { get; set; }
        // krishna start
        public bool isXDS { get; set; }
        // krishna end
    }
    public class SignUpRequest
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string title { get; set; }
        public string iDNumber { get; set; }
        public string emailid { get; set; }
        public string billEmailadress { get; set; }
        public string contactNumber { get; set; }
        public string dsaVersion { get; set; }
        public string htmlString { get; set; }
        public string[] base64Array { get; set; }
        public float[] floats { get; set; }
        public Customer customer { get; set; }
        public string toMail { get; set; }
        public string displayBusinessName { get; set; }
    }
    public class SignUpResponse
    {
        public bool isSucsess { get; set; }
        public string Message { get; set; }
        public string token { get; set; }

    }
    public class Menu
    {
        public string Caption { get; set; }

        public string Image { get; set; }

        public string Url { get; set; }

        public bool IsSelected { get; set; }

        public List<Menu> SubMenus { get; set; } = new List<Menu>();
    }
    public class DataServiceResponse
    {
        public int Version { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
    public class DonotCallRegistryRequest
    {
        public string iDNumber { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string phonenumber { get; set; }
        public string emailaddress { get; set; }

        public DateTime currentdate { get; set; }
    }
    public class DonoCallRegistryResponse
    {
        public bool isSucsess { get; set; }
        public string errorMessage { get; set; }
        public string token { get; set; }
    }
    public class ApplicationMessages
    {
        public Guid id { get; set; }
        public string message { get; set; }
        public bool showmessage { get; set; }
    }

    public class XdsTokenData
    {
        public string SubscriberId { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }
    }

}
