
using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{

    public class EmailTemplateService
    {
        IDASDbContext _dbContext;
        public EmailTemplateService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActionResult<EmailTemplateResponse>> EmailTemplateList(DataTableRequest request)
        {
            EmailTemplateResponse response = new EmailTemplateResponse();
            try
            {
                var lst = _dbContext.EmailTemplates.AsQueryable();
                int cnt = _dbContext.EmailTemplates .Count();
                var flt = lst;
               
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();



                var data = (from s in flt
                            select new EmailTemplateList
                            {
                                Id = s.Id,
                                Subject = s.Subject,
                                MailContent = s.MailContent,
                                Type = s.Type
                            }).ToList();

                response.data = data;

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public CrudResponseemail Insert(EmailTemplate data)
        {
            CrudResponseemail response = new CrudResponseemail();

            try
            {
                data.Id = Guid.NewGuid();
                _dbContext.EmailTemplates.Add(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }

        public CrudResponseemail Update(EmailTemplate data)
        {
            CrudResponseemail response = new CrudResponseemail();
            try
            {
                _dbContext.EmailTemplates.Update(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
        public CrudResponseemail Delete(Guid id)
        {
            CrudResponseemail response = new CrudResponseemail();
            try
            {
                var data = _dbContext.EmailTemplates.FirstOrDefault(t => t.Id == id);
                _dbContext.EmailTemplates.Remove(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public EmailTemplate View(Guid id)
        {
            EmailTemplate data = new EmailTemplate();
            try
            {
                data = _dbContext.EmailTemplates.Find(id);

            }
            catch (Exception ex)
            {

            }
            return data;
        }

    }

    public class EmailTemplateResponse : DataTableResponse<EmailTemplateList>
    {

    }




    public class EmailTemplateList
    {
        public Guid Id { get; set; }

        public string Subject { get; set; }

        public string MailContent { get; set; }

        public string Type { get; set; }
    }


    public class CrudResponseemail
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }

}
