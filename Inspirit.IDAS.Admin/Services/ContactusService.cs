using Inspirit.IDAS.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Inspirit.IDAS.Admin.Services
{
    public class ContactusService
    {
        IDASDbContext _dbContext;
        public ContactusService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActionResult<ContactusResponse>> ContactusList(ContactUsSearchRequest request)
        {
           
            ContactusResponse response = new ContactusResponse();
            try
            {
                DateTime FilterToDate = new DateTime(Convert.ToDateTime(request.todate).Year, Convert.ToDateTime(request.todate).Month, Convert.ToDateTime(request.todate).Day, 23, 59, 59);

                var lst = _dbContext.ContactUs.Where(m=>m.Date >= Convert.ToDateTime(request.fromdate) && m.Date <= FilterToDate).AsQueryable();
                int cnt = lst.Count();
                var flt = lst;

                if ((!String.IsNullOrEmpty(request.fromdate) && !String.IsNullOrEmpty(request.todate)))
                {
                    if(request.isRead.ToLower() == "both")
                    {
                        flt = _dbContext.ContactUs.Where(m => m.Date >= Convert.ToDateTime(request.fromdate) && m.Date <= FilterToDate);
                    }else if(request.isRead.ToLower() == "read")
                    {
                        flt = _dbContext.ContactUs.Where(m => m.Date >= Convert.ToDateTime(request.fromdate) && m.Date <= FilterToDate && (m.IsRead == true));
                    }
                    else if (request.isRead.ToLower() == "unread")
                    {
                        flt = _dbContext.ContactUs.Where(m => m.Date >= Convert.ToDateTime(request.fromdate) && m.Date <= FilterToDate && (m.IsRead == false));
                    }
                    
                }
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();

                var data = (from s in flt
                            select new ContactUsVm
                            {
                                Id = s.Id,
                                ContactName = s.Name,
                                Email = s.Email,
                                Subject = s.Subject,
                                Message = s.Message,
                                Date = s.Date,
                                IsRead = s.IsRead,
                                Business = s.Business,
                                PhoneNumber = s.PhoneNumber
            

                            }).ToList();
                response.data = data;
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<CrudResponseContact> Update(ContactUs data)
        {
            CrudResponseContact response = new CrudResponseContact();

            try
            {

                _dbContext.ContactUs.Update(data);
                _dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
        public ContactUs View(Guid id)
        {
            ContactUs data = new ContactUs();
            try
            {
                data = _dbContext.ContactUs.Find(id);

            }
            catch (Exception ex)
            {

            }
            return data;
        }


        public CrudResponseContact UpdateStatus(ContactUs request)
        {
            CrudResponseContact response = new CrudResponseContact();
            try
            {
                var contact = _dbContext.ContactUs.Where(m => m.Id == request.Id).FirstOrDefault();
                contact.IsRead = request.IsRead;
                _dbContext.ContactUs.Update(contact);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
               
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

    }
    public class ContactusResponse : DataTableResponse<ContactUsVm>
    {

    }
    public class ContactUsSearchRequest
    {
        public string isRead { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }
    public class ContactUsVm
    {
        public Guid Id { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public string PhoneNumber { get; set; }
        public string Business { get; set; }
    }
    public class CrudResponseContact
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

}



