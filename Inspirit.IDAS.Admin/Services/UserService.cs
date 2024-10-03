using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class UserService
    {
        IDASDbContext _dbContext;

        public UserService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActionResult<UserDataResponse>> UserDataList(DataTableRequest request)
        {
            UserDataResponse response = new UserDataResponse();
            try
            {
                var lst = _dbContext.Users.AsQueryable();
                int cnt = _dbContext.Users.Count();
                var flt = lst;
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();

                var data = (from s in flt
                            select new UsersData
                            {
                                Id = s.Id,
                                LoginName = s.LoginName,
                                FirstName = s.FirstName,
                                LastName = s.LastName,
                                Password = s.Password,
                                Emailid = s.Emailid,
                                IsActive = s.IsActive
                            }).ToList();

                response.data = data;

            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<UserCrudResponse> Insert(User data)
        {
            UserCrudResponse response = new UserCrudResponse();
            try
            {
                bool valid = FormNameValid(data);
                if (valid == true)
                {
                    var userexists = _dbContext.Users.Where(t => t.Emailid == data.Emailid).FirstOrDefault();
                    if (userexists == null)
                    {
                        data.Id = Guid.NewGuid();
                        foreach (var pr in data.UserPermissionslist)
                        {
                            pr.UserId = data.Id;
                            pr.Id = Guid.NewGuid();
                        }
                        _dbContext.Users.Add(data);
                        await _dbContext.UserPermissions.AddRangeAsync(data.UserPermissionslist);
                        await _dbContext.SaveChangesAsync();
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "User already exists";
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Form Name can not be entered more than once.";
                }


            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }

        private bool FormNameValid(User data)
        {
            bool isSucess = false;
            foreach (var usr in data.UserPermissionslist)
            {
                var cnt = data.UserPermissionslist
                      .Where(t => t.FormName == usr.FormName).ToList();
                if (cnt.Count > 1)
                {
                    isSucess = false;
                    break;
                }
                else
                {
                    isSucess = true;
                }
            }
            return isSucess;
        }
        public async Task<List<Menu>> GetDropdownMenu()
        {
            List<Menu> lst = new List<Menu>();
            lst.Add(new Menu() { Caption = "Dashboard", Url = "dashboard" });
            lst.Add(new Menu() { Caption = "Customers", Url = "customerlist" });
            lst.Add(new Menu() { Caption = "Data Service Agreement", Url = "dsalist" });
            lst.Add(new Menu() { Caption = "Lookup Data", Url = "lookuplist" });
            lst.Add(new Menu() { Caption = "Email Templates", Url = "emailtemplatelist" });
            lst.Add(new Menu() { Caption = "Donot Call Registry", Url = "dncrlist" });
            lst.Add(new Menu() { Caption = "Invoice", Url = "invoicelist" });
            lst.Add(new Menu() { Caption = "Proforma Invoice", Url = "proformainvoicelist" });
            lst.Add(new Menu() { Caption = "Payment", Url = "paymentlist" });
            lst.Add(new Menu() { Caption = "Application Message", Url = "applicationmessagelist" });
            lst.Add(new Menu() { Caption = "Application Settings", Url = "appsettinglist" });
            lst.Add(new Menu() { Caption = "Manage Users", Url = "userlist" });
            lst.Add(new Menu() { Caption = "Subscriptions", Url = "subscribtions" });
            lst.Add(new Menu() { Caption = "Contact Us", Url = "contactus" });
            lst.Add(new Menu() { Caption = "Products", Url = "productlist" });
            lst.Add(new Menu() { Caption = "Product Services", Url = "productservicelist" });
            lst.Add(new Menu() { Caption = "BatchTracing", Url = "batchtrace" });
            lst.Add(new Menu() { Caption = "News Blog", Url = "news" });
            lst.Add(new Menu() { Caption = "LeadGeneration", Url = "leadlist" });
            return lst;
        }
        public async Task<UserCrudResponse> Update(User data)
        {

            UserCrudResponse response = new UserCrudResponse();


            try
            {
                bool valid = FormNameValid(data);
                if (valid == true)
                {

                    var _addRange = data.UserPermissionslist.
                 Where(t => t.UserId != data.Id).ToList();

                    var _updateRange = data.UserPermissionslist.
                       Where(t => t.UserId == data.Id).ToList();

                    List<UserPermission> _addpermissions = new List<UserPermission>();
                    foreach (var usr in _addRange)
                    {
                        usr.Id = Guid.NewGuid();
                        usr.UserId = data.Id;
                    }
                    _dbContext.Users.Update(data);
                    _dbContext.UserPermissions.UpdateRange(_updateRange);

                    if (_addRange.Count > 0)
                        await _dbContext.UserPermissions.AddRangeAsync(_addRange);

                    await _dbContext.SaveChangesAsync();
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Form Name can not be entered more than once.";

                }
            }

            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;

        }

        public async Task<UserPermission> GetUserPermission(Guid id, string FormName)
        {
            UserPermission permission = new UserPermission();
            try
            {
                permission = _dbContext.UserPermissions.Where(m => m.UserId == id && m.FormName.ToLower() == FormName.ToLower()).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }

            return permission;
        }

        public async Task<List<UserPermission>> PermissionRemove(Guid id)
        {
            List<UserPermission> lstpermissions =
                new List<UserPermission>();
            try
            {
                var permisssion = await _dbContext.UserPermissions
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (permisssion != null)
                {
                    _dbContext.UserPermissions.Remove(permisssion);
                    await _dbContext.SaveChangesAsync();
                    lstpermissions = await _dbContext.UserPermissions
                        .Where(i => i.UserId == permisssion.UserId).ToListAsync();
                }
            }
            catch (Exception ex)
            {

            }
            return lstpermissions;
        }

        public UserCrudResponse Delete(Guid id)
        {
            UserCrudResponse response = new UserCrudResponse();
            try
            {
                var data = _dbContext.Users.FirstOrDefault(t => t.Id == id);
                _dbContext.Users.Remove(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public User View(Guid id)
        {
            User data = new User();
            try
            {
                data = _dbContext.Users.Include(t => t.UserPermissionslist).Where(t => t.Id == id).FirstOrDefault();

            }
            catch (Exception ex)
            {

            }
            return data;
        }
    }


    public class UserDataResponse : DataTableResponse<UsersData>
    {

    }

    public class UsersData
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Emailid { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserCrudResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}
