using Inspirit.IDAS.Data;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Inspirit.IDAS.Admin
{
    public class EmailService
    {
        IDASDbContext _dbContext;
        private IHostingEnvironment _hostingEnvironment;
        private IConfiguration _configuration;

        public EmailService(IDASDbContext dbContext, IHostingEnvironment hostingEnvironment, IConfiguration config)
        {
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
            _configuration = config;
        }
        public async Task<string> SendEmailAsync(string To, string subject, string msgBody, string attachmentFilePath = "", string reply = "", BodyBuilder BodyBuilder = null, string AttachmentName = "")
        {
            string Message = "";
            MailMessage emailMessage = new MailMessage();
           
            try
            {
                string serverName = _dbContext.ApplicationSetting.First(t => t.SettingName == "Email Server").SettingValue;
                string hostName = _dbContext.ApplicationSetting.First(t => t.SettingName == "Email Server").SettingValue;
                string Email = string.Empty;
                string Credentials = _dbContext.ApplicationSetting.First(t => t.SettingName == "Email UserID").SettingValue;
                Email = _dbContext.ApplicationSetting.First(t => t.SettingName == "From Email").SettingValue;
            
                string Password = _dbContext.ApplicationSetting.First(t => t.SettingName == "Email Password").SettingValue;
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
                if (m.Count > 0)
                {
                    int i = 1;
                    foreach (var con in m)
                    {
                        if (con.ToString().Length > 0)
                        {
                            string msg = getImage(con.ToString(), i, AttachmentName);


                            string basepath = _configuration.GetSection("AdminPath").GetSection("Link").Value;

                            string imgpath = basepath + msg;


                            emailMessage.Body = builder.HtmlBody.Replace(con.ToString(), "<img   src=\"" + imgpath + "\" style=\"width:100%;height:auto\"  />");



                            WriteLog("Method Name: " + "Image log", imgpath + emailMessage.Body);

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

                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(Credentials, Password);
                    smtp.Port = 587;
                    smtp.Host = serverName;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = true;
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
        public string getImage(string htmlcontent, int i,string AttachmentName)
        {
            string message = string.Empty;
            int index1 = htmlcontent.ToString().IndexOf(",");
            index1 = index1 + 1;
            string str = htmlcontent.ToString().Remove(0, index1);

            string str2 = str.Replace("\">", "");

            var base64string = str2;
            var base64array = Convert.FromBase64String(base64string);

            string folderName = "AdminImageForMail";
            string webRootPath = _hostingEnvironment.WebRootPath.Replace("Admin", "WebApplication");
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);
            string fileName = AttachmentName + i + ".png";
            string logFilePath = Path.Combine(newPath, fileName);
            System.IO.File.WriteAllBytes(logFilePath, base64array);
            message = folderName + "/" + fileName;


            return message;
        }
    }
}
