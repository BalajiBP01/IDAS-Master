using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class ApplicationMessageService
    {

        IDASDbContext _dbContext;
        public ApplicationMessageService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActionResult<ApplicationmessageResponse>> ApplicationMessageList(DataTableRequest request)
        {
            ApplicationmessageResponse response = new ApplicationmessageResponse();
            try
            {
                var lst = _dbContext.ApplicationMessages.AsQueryable();
                int cnt = _dbContext.ApplicationMessages.Count();
                var flt = lst;
                
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();



                var data = (from s in flt
                            select new Applicationmessage
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Message = s.Message,
                                Showmessage = s.Showmessage,
                            }).ToList();


                response.data = data;

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public CrudResponseMessage Insert(ApplicationMessage data)
        {
            CrudResponseMessage response = new CrudResponseMessage();

            try
            {
                var msg = _dbContext.ApplicationMessages.Where(m => m.Name == data.Name).FirstOrDefault();
                if (msg == null)
                {

                    data.Id = Guid.NewGuid();
                    var message = Convert.ToString(data.Id);
                    if (data.Showmessage == true)
                    {
                       
                        List<CustomerUser> users = new List<CustomerUser>();
                        users = _dbContext.CustomerUsers.ToList();
                        foreach (var usr in users)
                        {
                            if (data.Showmessage == true)
                            {
                                if (string.IsNullOrEmpty(usr.Message))
                                {
                                    usr.Message = message;
                                }

                                else if (!usr.Message.Contains(message))
                                {
                                    usr.Message = usr.Message + "," + msg;
                                    usr.Message = usr.Message.TrimStart(',');
                                    usr.Message = usr.Message.TrimEnd(',');
                                }
                            }
                          
                        }
                        _dbContext.CustomerUsers.UpdateRange(users);
                    }
                    _dbContext.ApplicationMessages.Add(data);
                    _dbContext.SaveChanges();
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Name is exists";
                    return response;
                }


            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }

        public CrudResponseMessage Update(ApplicationMessage data)
        {
            CrudResponseMessage response = new CrudResponseMessage();
            try
            {

                List<CustomerUser> users = new List<CustomerUser>();
                users = _dbContext.CustomerUsers.ToList();

                foreach (var usr in users)
                {
                    var msg = Convert.ToString(data.Id);
                    if (data.Showmessage == true)
                    {
                        if (string.IsNullOrEmpty(usr.Message))
                        {
                            usr.Message = msg;
                        }

                        else if (!usr.Message.Contains(msg))
                        {
                            usr.Message = usr.Message + "," + msg;
                            usr.Message = usr.Message.TrimStart(',');
                            usr.Message = usr.Message.TrimEnd(',');
                        }
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(usr.Message))
                        {
                            usr.Message = usr.Message.Replace(msg, "");
                            usr.Message = usr.Message.Replace(",,", ",");
                            usr.Message = usr.Message.TrimStart(',');
                            usr.Message = usr.Message.TrimEnd(',');
                        }

                    }

                }
                _dbContext.CustomerUsers.UpdateRange(users);

                _dbContext.ApplicationMessages.Update(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;

        }
        public CrudResponseMessage Delete(Guid id)
        {
            CrudResponseMessage response = new CrudResponseMessage();
            try
            {
                var data = _dbContext.ApplicationMessages.FirstOrDefault(t => t.Id == id);
                List<CustomerUser> users = new List<CustomerUser>();
                users = _dbContext.CustomerUsers.ToList();
                string msg = Convert.ToString(data.Id);
                foreach (var usr in users)
                {
                    var isexists = usr.Message.Contains("," + msg);
                    if (isexists == true)
                    {
                        usr.Message = usr.Message.Replace("," + msg, "");
                    }
                    else
                    {
                        var exist = usr.Message.Contains(msg);
                        if (exist == true)
                            usr.Message = usr.Message.Replace(msg, "");
                    }
                }

                _dbContext.CustomerUsers.UpdateRange(users);
                _dbContext.ApplicationMessages.Remove(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public ApplicationMessage View(Guid id)
        {
            ApplicationMessage data = new ApplicationMessage();

            try
            {
                data = _dbContext.ApplicationMessages.Find(id);

            }
            catch (Exception ex)
            {

            }
            return data;
        }

    }

    public class ApplicationmessageResponse : DataTableResponse<Applicationmessage>
    {

    }




    public class Applicationmessage
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }

        public bool Showmessage { get; set; }


    }


    public class CrudResponseMessage
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}

