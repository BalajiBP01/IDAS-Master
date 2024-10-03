using Inspirit.IDAS.Data;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication.Services
{
    public class ComplaintsService
    {
        IDASDbContext _IDAScontext;
        EmailService _emailService;
        public ComplaintsService(IDASDbContext IDAScontext, EmailService emailService)
        {
            _IDAScontext = IDAScontext;
            _emailService = emailService;
        }
        public string Insert(ContactUs contact)
        {
            try
            {
                if (contact.PhoneNumber.Length != 10)
                {
                    if (contact.PhoneNumber.StartsWith("27") && contact.PhoneNumber.Length == 11)
                    {
                        var p = contact.PhoneNumber.Substring(0, 2);
                        var valid = contact.PhoneNumber.Replace(p, "");
                        contact.PhoneNumber = valid;
                    }
                    else
                    {
                        return "Complaints - Invalid phone number ";

                    }
                }
                _IDAScontext.ContactUs.Add(contact);
                _IDAScontext.SaveChanges();
                SendMail(contact.Email);
            }
            catch (Exception ex)
            {

            }
            return "Saved Successfully - Complaint";
        }
        private async void SendMail(string toEmail)
        {
            EmailTemplate email = _IDAScontext.EmailTemplates.FirstOrDefault(t => t.Type == "ContactUs");
            if (email != null)
            {


                string to = toEmail;
                string message = email.MailContent;
                string subject = email.Subject;

                var builder = new BodyBuilder();

                builder.HtmlBody += message;




                await _emailService.SendEmailAsync(to, subject, "", "", "", builder, true, "ContactUs");
            }
        }
    }
}
