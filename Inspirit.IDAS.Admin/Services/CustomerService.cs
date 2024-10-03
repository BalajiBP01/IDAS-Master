using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Inspirit.IDAS.Admin
{
    public class CustomerService
    {
        IDASDbContext _dbContext;
        CustomerUserService _CustomerUserService;
        public CustomerService(IDASDbContext dbContext, CustomerUserService CustomerUserService)
        {
            _dbContext = dbContext;
            _CustomerUserService = CustomerUserService;
        }
        public async Task<ActionResult<CustomerDetailsResponse>> CustomerList(DataTableRequest request)
        {
            CustomerDetailsResponse response = new CustomerDetailsResponse();
            try
            {
                var lst = _dbContext.Customers.AsQueryable();
                int cnt = _dbContext.Customers.Count();
                var flt = lst;

                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();

                var data = (from s in flt
                            select new CustomerData
                            {
                                Id = s.Id,
                                TradingName = s.TradingName,
                                RegistrationName = s.RegistrationName,
                                RegistrationNumber = s.RegistrationNumber,
                                VATNumber = s.VATNumber,
                                BranchLocation = s.BranchLocation,
                                PhysicalAddress = s.PhysicalAddress,
                                TypeOfBusiness = s.TypeOfBusiness,
                                TelephoneNumber = s.TelephoneNumber,
                                FaxNumber = s.FaxNumber,
                                BillingEmail = s.BillingEmail,
                                Status = s.Status,
                                BillingType = s.BillingType,
                                Code = s.Code,
                                IsRestricted = s.IsRestricted
                                

                            }).ToList();

                response.data = data;

            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public string GetCustomerName(Guid Id)
        {
            string name = "";
            try
            {
                name = _dbContext.Customers.Where(m => m.Id == Id).Select(m => m.TradingName).FirstOrDefault();
                return name;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return name;
        }
        public async Task<List<Product>> GetProducts()
        {
            List<Product> productlist = await _dbContext.Products.Where(a => a.Status == true).ToListAsync();
            return productlist;
        }
        public async Task<List<LookupData>> GetLookupDatas()
        {
            List<LookupData> lookupDatas = await _dbContext.LookupDatas
                .Where(l => l.IsActive == true && l.Type == "Type of Business").ToListAsync();
            return lookupDatas;
        }
        // new krishna start
        /// <summary>
        /// Added by Krishna
        /// </summary>
        /// <returns>list of Enquiry Reason LookupData</returns>
        public async Task<List<LookupData>> GetEnquiryReasonLookupDatas()
        {
            List<LookupData> lookupDatas = await _dbContext.LookupDatas
                .Where(l => l.IsActive == true && l.Type == "Enquiry Reason").ToListAsync();
            return lookupDatas;
        }
        // new krishna end

        public async Task<CustomerProdcutVm> GetCustomerProducts()
        {
            CustomerProdcutVm customerProdcutVm = new CustomerProdcutVm();
            customerProdcutVm.customerProductProperties = new List<CustomerProductProperty>();
            return customerProdcutVm;
        }
        public CustomerCrudResponse Insert(CustomersVm cust)
        {
            CustomerCrudResponse response = new CustomerCrudResponse();
            // pending validation duplicate subscriberId for XDS Customer
            try
            {
                var custs = _dbContext.Customers.Where(m => m.RegistrationNumber.Replace(" ", "") == cust.customer.RegistrationNumber.Replace(" ", "")).FirstOrDefault();
                string strCode = string.Empty;
                string strTemp = cust.customer.TradingName;
                strTemp = strTemp.Replace(" ", "");
                if (custs == null)
                {
                    cust.customer.Id = Guid.NewGuid();
                    cust.customer.Status = "Pending";
                    cust.customer.CreatedBy = cust.IDASuserId;
                    cust.customer.CreatedDate = DateTime.Now;
                    cust.customer.IsRestricted = false;
                    int count = 0;
                    if (strTemp.Length < 6)
                    {
                        strCode = AddZero(strTemp, "Right", 6);
                        count = _dbContext.Customers.Where(t => strTemp.Contains(t.TradingName.Substring(0, strTemp.Length))).Count();
                    }
                    else
                    {
                        strCode = strTemp.Substring(0, 6);
                        count = _dbContext.Customers.Where(t => strTemp.Substring(0, 6).StartsWith(t.TradingName.Substring(0, 6))).Count();
                    }

                    count += 1;

                    if (count.ToString().Length != 3)
                        strCode = strCode + AddZero(count.ToString(), "Left", 3);

                    cust.customer.Code = strCode;
                    cust.customer.TabSelected = "ConsumerTimeline,ConsumerProfile,ConsumerTelephone,ConsumerAddress,ConsumerEmployment,ConsumerProperty,ConsumerJudgement,ConsumerDebtReview,ConsumerDirector,ConsumerRelationship,CommercialTimeline,CommercialProfile,CommercialTelephone,CommercialAddress,CommercialJudgement,CommercialProperty,CommercialAuditor,CommercialDirector";


                    // krishna new start
                    //if (cust.customer.IsXDS == false)
                    //{
                    //    cust.customer.SubscriberId = "";
                    //}
                    //
                    if (cust.customer.IsXDS == true)
                    {
                        if(string.IsNullOrEmpty(cust.customer.SubscriberId))
                        {
                            response.IsSuccess = false;
                            response.Message = "Company SubscriberId is required";
                            return response;
                        }

                        bool subscriptionExists = _dbContext.Customers.Any(c => c.SubscriberId == cust.customer.SubscriberId.Trim());
                        if (subscriptionExists == true)
                        {
                            response.IsSuccess = false;
                            response.Message = "Company with entered SubscriberId already exists";
                            return response;
                        }
                    }
                    // krishna new end
                    _dbContext.Customers.Add(cust.customer);
                    _dbContext.SaveChanges();
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Company with entered registration number already exists";
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
        public async Task<CustomerCrudResponse> InsertCustomerProduct(CustomerProdcutVm customerProdcutVm)
        {
            CustomerCrudResponse response = new CustomerCrudResponse();
            try
            {
                List<CustomerProduct> customerProducts = new List<CustomerProduct>();
                foreach (var custPrvm in customerProdcutVm.customerProductProperties)
                {
                    var data = customerProdcutVm.customerProductProperties
                                               .Where(e => e.ProductId == custPrvm.ProductId).ToList();
                    if (data.Count() > 1)
                    {
                        response.Message = "Products can not be entered more than once.";
                        break;
                    }
                    CustomerProduct customerProduct = new CustomerProduct
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customerProdcutVm.CustomerId,
                        ProductId = custPrvm.ProductId,
                        Active = custPrvm.IsActive,
                        CreatedDate = DateTime.Now
                    };
                    customerProducts.Add(customerProduct);
                }
                if (customerProducts.Count > 0)
                {
                    await _dbContext.CustomerProducts.AddRangeAsync(customerProducts);
                    await _dbContext.SaveChangesAsync();
                    response.Message = "";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<CustomerCrudResponse> UpdateCustomerProduct(CustomerProdcutVm customerProdcutVm)
        {
            CustomerCrudResponse response = new CustomerCrudResponse();
            try
            {
                List<CustomerProduct> customerProducts = new List<CustomerProduct>();

                var existsproducts = _dbContext.CustomerProducts.Where(m => m.CustomerId == customerProdcutVm.CustomerId).ToList();

                List<CustomerProductProperty> _addCustomerProducts = new List<CustomerProductProperty>();
                List<CustomerProduct> _updateProduct = new List<CustomerProduct>();
                foreach (var prod in customerProdcutVm.customerProductProperties)
                {
                    CustomerProduct custprod = existsproducts.Find(t => t.Id == prod.Id);
                    if(custprod == null)
                        _addCustomerProducts.Add(prod);
                    else
                    {
                        custprod.ProductId = prod.ProductId;
                        custprod.Active = prod.IsActive;
                        custprod.ModifiedDate = DateTime.Now;
                        _updateProduct.Add(custprod);
                    }
                }
                

                foreach (var d in customerProdcutVm.customerProductProperties)
                {
                    var data = customerProdcutVm.customerProductProperties
                                                .Where(e => e.ProductId == d.ProductId).ToList();
                    if (data.Count() > 1)
                    {
                        response.Message = "Products can not be entered more than once.";
                        return response;
                    }
                }
                customerProducts = new List<CustomerProduct>();
                foreach (var custPrvm in _addCustomerProducts)
                {
                    CustomerProduct customerProduct = new CustomerProduct
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customerProdcutVm.CustomerId,
                        ProductId = custPrvm.ProductId,
                        Active = custPrvm.IsActive,
                        CreatedDate = DateTime.Now
                    };
                    customerProducts.Add(customerProduct);
                }
                if (customerProducts.Count > 0)
                {
                    await _dbContext.CustomerProducts.AddRangeAsync(customerProducts);
                }
               
                if (_updateProduct.Count > 0)
                {
                    _dbContext.CustomerProducts.UpdateRange(_updateProduct);
                }
               
                await _dbContext.SaveChangesAsync();
                response.Message = "";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public CustomerCrudResponse Update(CustomersVm cust)
        {
            CustomerCrudResponse response = new CustomerCrudResponse();
            try
            {
              
                var custs = _dbContext.Customers.Where(m => m.Id != cust.customer.Id && m.RegistrationNumber.Replace(" ", "") == cust.customer.RegistrationNumber.Replace(" ", "")).FirstOrDefault();
                if (cust.customer.IsXDS == true)
                {
                    if (string.IsNullOrEmpty(cust.customer.SubscriberId))
                    {
                        response.IsSuccess = false;
                        response.Message = "Company SubscriberId is required";
                        return response;
                    }

                    bool subscriptionExists = _dbContext.Customers.Any(c => c.Id != cust.customer.Id && c.SubscriberId == cust.customer.SubscriberId.Trim());
                    if (subscriptionExists == true)
                    {
                        response.IsSuccess = false;
                        response.Message = "Company with entered SubscriberId already exists";
                        return response;
                    }
                }
                if (custs == null)
                {
                    cust.customer.ModifiedBy = cust.IDASuserId;
                    cust.customer.ModifiedDate = DateTime.Now;
                    _dbContext.Customers.Update(cust.customer);
                    _dbContext.SaveChanges();
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Company with entered registration number already exists";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
        public CustomerCrudResponse Delete(Guid id)
        {
            CustomerCrudResponse response = new CustomerCrudResponse();
            try
            {
                var cust = _dbContext.Customers.FirstOrDefault(t => t.Id == id);
                _dbContext.Customers.Remove(cust);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public CustomersVm View(Guid id, string hostingEnvironment)
        {
            CustomersVm cust = new CustomersVm();
            try
            {
                cust.customer = _dbContext.Customers.Find(id);
                string webRootPath = hostingEnvironment;
                string filepath = Path.Combine(webRootPath, "Customerdocuments", id.ToString());
                if (filepath != null)
                {
                    
                    foreach (var names in Directory.GetFiles(filepath, "", SearchOption.AllDirectories))
                    {
                        string path = names;
                        string name = names.Split('\\').Last();
                        cust.filenames.Add(new FileNames
                        {
                            Path = path,
                            Name = name
                        });
                    }
                }

            }

            catch (Exception ex)
            {

            }
            return cust;
        }
        public async Task<CustomerProdcutVm> ViewCustomerProdcut(Guid id)
        {
            CustomerProdcutVm customerProdcutVm = new CustomerProdcutVm();
            try
            {
                var data = await _dbContext.CustomerProducts
                                  .Where(v => v.CustomerId == id).ToListAsync();
                foreach (var i in data)
                {
                    customerProdcutVm.CustomerId = i.CustomerId;
                    CustomerProductProperty customerProductProperty = new CustomerProductProperty
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        IsActive = i.Active.Value
                    };
                    customerProdcutVm.customerProductProperties.Add(customerProductProperty);
                }
            }
            catch (Exception ex)
            {

            }
            return customerProdcutVm;
        }
        public CustomerCrudResponse UpdateStatus(CustomersVm cust)
        {


            cust.customer.ActivatedDate = DateTime.Now;
            cust.customer.ActivatedBy = cust.IDASuserId;
            CustomerCrudResponse resp = Update(cust);
            if (resp.IsSuccess)
            {
                List<CustomerUser> lst = _dbContext.CustomerUsers.Where(t => t.CustomerId == cust.customer.Id).ToList();
                foreach (var user in lst)
                {
                    CustomerUserVM userVM = new CustomerUserVM();
                    user.ActivatedBy = cust.IDASuserId;
                    user.ActivatedDate = DateTime.Now;
                    userVM.customeruser = user;
                    userVM.IDASuserId = cust.IDASuserId;
                    _CustomerUserService.UpdateStatusAsync(userVM, cust.customer.Status);
                  
                }
                
            }
            if (resp.IsSuccess && cust.customer.Status.ToUpper() == "ACTIVE")
            {
                var activeproduct = _dbContext.CustomerProducts.Where(t => t.CustomerId == cust.customer.Id && t.Active == true).FirstOrDefault();
                if (activeproduct == null)
                    resp.Message = "Customer does not have the active product.";
            }
            return resp;
        }
        public string FileDelete(string FilePath)
        {
            if (FilePath != null)
            {
                try
                {
                    if (System.IO.File.Exists(FilePath))
                    {
                        System.IO.File.Delete(FilePath);
                        return "File deleted";
                    }
                }
                catch (Exception EX)
                {
                    return EX.Message.ToString();
                }
            }
            return "File Not deleted";

        }

        public async Task<string> GetTabs(Guid Id)
        {
            string res = string.Empty;
            var custdetails = await _dbContext.Customers.Where(t => t.Id == Id).FirstOrDefaultAsync();
            if (custdetails != null)
            {
                res = custdetails.TabSelected;
            }
            return res;
        }

        public async Task<string> AddorUpdateTabs(string Tabs, Guid CustomerId)
        {
            string res = string.Empty;
            Customer cust = new Customer();
            try
            {
                Tabs = Tabs.Replace("undefined", "");
                Tabs = Tabs.Replace(",,", ",");
                Tabs = Tabs.TrimEnd(',');
                Tabs = Tabs.TrimStart(',');
                var customerdetails = await _dbContext.Customers.Where(t => t.Id == CustomerId).FirstOrDefaultAsync();
                if (customerdetails != null)
                {
                    cust = customerdetails;
                    cust.TabSelected = Tabs;
                    _dbContext.Customers.Update(cust);
                    _dbContext.SaveChanges();
                    res = "Saved Successfully";
                }
                else
                {
                    res = "Faied";
                }
            }
            catch (Exception ex)
            {
            }
            return res;
        }
    



    public string AddZero(string strInput, string strWhere, int length)
        {
            string strOutput = strInput;

            while(strOutput.Length != length)
            {
                if (strWhere.ToUpper() == "LEFT")
                    strOutput = "0" + strOutput;
                else
                    strOutput += "0";
            }


            return strOutput;

        }
    }
    public class CustomerData
    {
        public Guid Id { get; set; }
        public string TradingName { get; set; }
        public string RegistrationName { get; set; }
        public string RegistrationNumber { get; set; }
        public string VATNumber { get; set; }
        public string BranchLocation { get; set; }
        public string PhysicalAddress { get; set; }
        public string TypeOfBusiness { get; set; }
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string BillingEmail { get; set; }
        public string Status { get; set; }
        public string Code { get; set; }
        public string BillingType { get; set; }
        public bool? IsRestricted { get; set; }


    }
    public class CustomersVm
    {
        public Customer customer { get; set; }
        public Guid IDASuserId { get; set; }
        public List<FileNames> filenames { get; set; } = new List<FileNames>();
    }
    public class CustomerProdcutVm
    {
        public Guid CustomerId { get; set; }
        public List<CustomerProductProperty> customerProductProperties = new List<CustomerProductProperty>();
    }
    public class CustomerProductProperty
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public bool IsActive { get; set; }
    }
    public class FileNames
    {
        public string Path { get; set; }
        public string Name { get; set; }
    }
    public class CustomerCrudResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
    public class CustomerDetailsResponse : DataTableResponse<CustomerData>
    {

    }
}

