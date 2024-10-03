using Inspirit.IDAS.Data;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication
{
    public class TrailUserService
    {
        IDASDbContext _IDAScontext;
        EmailService _emailService;
        public TrailUserService(IDASDbContext IDAScontext, EmailService emailService)
        {

            _IDAScontext = IDAScontext;
            _emailService = emailService;
        }
        public string Insert(TrailUser tuser)
        {
            try
            {
                var userExists = _IDAScontext.TrailUsers.Where(t => t.EmailAddress == tuser.EmailAddress).FirstOrDefault();
                if (userExists != null)
                {
                    return "Email address has already been used for the free trial";
                }
                var code = GenerateCode();
                tuser.Password = code;
                tuser.Credits = 5;

                ContactUs contact = new ContactUs();

                contact.Id = Guid.NewGuid();
                contact.Email = tuser.EmailAddress;
                contact.Date = DateTime.Now;
                contact.IsRead = false;
                contact.Message = "Trail User";
                contact.Name = tuser.FirstName;
                contact.PhoneNumber = tuser.MobileNumber;
                contact.Subject = "Trail User";
                contact.Business = tuser.BusinessRegisterNumber;
                var userExist = _IDAScontext.TrailUsers.Where(t => t.EmailAddress == tuser.EmailAddress).FirstOrDefault();
                if (userExist != null)
                {
                    return "Login details sent by Email";
                }
                else
                {
                    _IDAScontext.ContactUs.Add(contact);
                    _IDAScontext.SaveChanges();
                    _IDAScontext.TrailUsers.Add(tuser);
                    _IDAScontext.SaveChanges();
                }

                if (!string.IsNullOrEmpty(code))
                {
                    EmailLoginDetial(tuser, code);
                }
            }
            catch (Exception ex)
            {

            }
            return "Login details sent by Email";
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
        public async void EmailLoginDetial(TrailUser usr, string code)
        {
            var temp = _IDAScontext.EmailTemplates.First(t => t.Type == "Trial User");
            string subject = temp.Subject;
            string message = temp.MailContent;

            message = message.Replace("&lt;username&gt;", usr.EmailAddress);

            message = message.Replace("&lt;password&gt;", code);



            var builder = new BodyBuilder();
            builder.HtmlBody += message;

           await _emailService.SendEmailAsync(usr.EmailAddress, subject, "", "", "", builder,false,"TrialUser");
        }

    }
}
