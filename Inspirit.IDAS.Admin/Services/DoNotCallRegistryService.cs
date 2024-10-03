using Inspirit.IDAS.Admin;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using MimeKit;

namespace Inspirit.IDAS.Admin
{
    public class DoNotCallRegistryService
    {
        IDASDbContext _dbContext;
        EmailService _emailService;
        
        public DoNotCallRegistryService(IDASDbContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }




        public async Task<ActionResult<DncrResponse>> GetDonotCallRegistry(DonotCallRegistrySearchRequest request)
        {

            DonotCallRegistryVm req = new DonotCallRegistryVm();
            DncrResponse response = new DncrResponse();
            try
            {
                req.fromdate = DateTime.Parse(request.fromdate, CultureInfo.GetCultureInfo("en-gb"));

                DateTime FilterToDate = new DateTime(Convert.ToDateTime(request.todate).Year, Convert.ToDateTime(request.todate).Month, Convert.ToDateTime(request.todate).Day, 23, 59, 59);

                var lst = _dbContext.DoNotCallRegistrys.AsQueryable();
                int cnt = lst.Count();
                var flt = lst;
               
                if (!String.IsNullOrEmpty(request.fromdate) && !String.IsNullOrEmpty(request.todate))
                {
                    flt = _dbContext.DoNotCallRegistrys.Where(m => m.CurrentDate >= req.fromdate && m.CurrentDate <= FilterToDate);
                }
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();

                var data = (from s in flt
                            select new DncrList
                            {
                                id = s.Id,
                                name = s.Name,
                                currentDate = s.CurrentDate,
                                emailId = s.Emailid,
                                surname = s.Surname,
                                idNumber = s.Idnumber,
                                phoneNumber = s.PhoneNumber,
                                IsApproved = s.IsApproved

                            }).ToList();

                response.data = data;

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<CrudResponseDonotCall> Update(DoNotCallRegistry data)
        {
            CrudResponseDonotCall response = new CrudResponseDonotCall();
          
            try
            {
                _dbContext.DoNotCallRegistrys.Update(data);
                _dbContext.SaveChanges();


                if (data.Emailid != null)
                {
                    var emailtemp = _dbContext.EmailTemplates.Where(t => t.Type == "Donot call Registry Approve").FirstOrDefault();

                    var builder = new BodyBuilder();
                   
                    builder.HtmlBody += emailtemp.MailContent;
                   
                    await _emailService.SendEmailAsync(data.Emailid, emailtemp.Subject, "", "", "", builder,"Donotcall");
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
     

        public DoNotCallRegistry View(Guid id)
        {
            DoNotCallRegistry data = new DoNotCallRegistry();
            try
            {
                data = 
                (from s in _dbContext.DoNotCallRegistrys
                 where s.Id == id
                 select new DoNotCallRegistry
                 {
                      Id= s.Id ,
                      Name=s.Name,
                      CurrentDate=s.CurrentDate,
                      Emailid=s.Emailid,
                      Surname=s.Surname,
                      Idnumber=s.Idnumber,
                      PhoneNumber=s.PhoneNumber,
                      IsApproved=s.IsApproved
                    

                 }).ToList().FirstOrDefault();

            }
            catch (Exception ex)
            {

            }
            return data;
        }




    }

    public class DncrResponse : DataTableResponse<DncrList>
    {

    }









    public class DncrList
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string emailId { get; set; }
        public string idNumber { get; set; }
        public string phoneNumber { get; set; }
        public DateTime currentDate { get; set; }
        public bool IsApproved { get; set; }
    }
    public class DonotCallRegistrySearchRequest
    {
        public string fromdate { get; set; }
        public string todate { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }

    public class DonotCallRegistryVm
    {

        public DateTime fromdate { get; set; }

        public DateTime todate { get; set; }



    }
    public class CrudResponseDonotCall
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }


}


