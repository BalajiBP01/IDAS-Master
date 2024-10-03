using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.Production;
using Inspirit.IDAS.ESData;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Xml;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Inspirit.IDAS.WebApplication
{
    public class TracingService
    {
        ProductionDbContext _ProductionDbContext;
        ESService _eSService;
        CustomerLogService _CustomerLogService;
        IDASDbContext _IDASDbContext;
        private IHostingEnvironment _hostingEnvironment;
        private IConfiguration _configuration;
        DAL dAL = new DAL();

        public TracingService(CustomerLogService CustomerLogService, ProductionDbContext ProductionDbContext, IDASDbContext IDASDbContext, IHostingEnvironment IHostingEnvironment, IConfiguration iconfig)
        {
            _CustomerLogService = CustomerLogService;
            _ProductionDbContext = ProductionDbContext;
            _IDASDbContext = IDASDbContext;
            _hostingEnvironment = IHostingEnvironment;
            _configuration = iconfig;
            _eSService = new ESService(_configuration.GetSection("ElasticDBIP").GetSection("url").Value);

        }

        public async Task<ConsumerSearchResponse> ConsumerSearch(ConsumerSearchRequest request)
        {
            ConsumerSearchResponse response = new ConsumerSearchResponse();

            ESSearchConsumerRequest req = new ESSearchConsumerRequest();

            try
            {
                bool istrail = false;
                if (request.isTrailuser == true)
                {
                    istrail = true;
                }
                string searchInp = string.Empty;
                string InputType = string.Empty;
                if (request.globalSearch != "undefined" && request.globalSearch != null)
                {
                    if (request.globalSearch.Contains(" "))
                    {
                        var glo = request.globalSearch.Replace(" ", ",");
                        req.GlobalSearch = glo;
                        searchInp = glo;
                    }
                    else
                        req.GlobalSearch = request.globalSearch;
                    searchInp = request.globalSearch;
                    InputType = "Global Search";
                }
                if (request.iDNumber != "undefined" && request.iDNumber != null)
                {
                    req.IDNumber = request.iDNumber;
                    searchInp = request.iDNumber;
                    InputType = "ID Number";
                }
                // krishna start pending
                if (request.iDNumber == "undefined" && request.iDNumber == null)
                {
                    if (request.Passport != "undefined" && request.Passport != null)
                    {
                        req.Passport = request.Passport;
                        searchInp = request.Passport;
                        InputType = "Passport Number";
                    }
                }
                // krishna end
                if (request.firstname != "undefined" && request.firstname != null)
                {
                    req.Firstname = request.firstname;
                    if (!string.IsNullOrEmpty(searchInp))
                        searchInp += " ";
                    searchInp += req.Firstname;
                    InputType = "First Name";
                }

                if (request.dateOfBirth != null && request.dateOfBirth != "undefined")
                {
                    req.DateOfBirth = DateTime.Parse(request.dateOfBirth, CultureInfo.GetCultureInfo("en-gb"));
                    if (!string.IsNullOrEmpty(searchInp))
                        searchInp += " ";
                    searchInp += req.DateOfBirth;
                    InputType = "Date of Birth";
                }
                if (request.fromDate != null && request.fromDate != "undefined")
                {
                    req.DateOfBirthFromDate = DateTime.Parse(request.fromDate, CultureInfo.GetCultureInfo("en-gb"));
                    if (!string.IsNullOrEmpty(searchInp))
                        searchInp += " ";
                    searchInp += req.DateOfBirthFromDate;
                    InputType = "Date of Birth range";
                }
                if (request.toDate != null && request.toDate != "undefined")
                {
                    req.DateOfBirthToDate = DateTime.Parse(request.toDate, CultureInfo.GetCultureInfo("en-gb"));
                    if (!string.IsNullOrEmpty(searchInp))
                        searchInp += " ";
                    searchInp += req.DateOfBirthToDate;
                }
                if (request.phoneNumber != "undefined" && request.phoneNumber != null)
                {
                    req.PhoneNumber = request.phoneNumber;
                    if (!string.IsNullOrEmpty(searchInp))
                        searchInp += " ";
                    searchInp += req.PhoneNumber;
                    InputType = "Phone Number";
                }
                if (request.address != "undefined" && request.address != null)
                {
                    var add = request.address.Replace(",", " ");
                    req.Address = add;
                    if (!string.IsNullOrEmpty(searchInp))
                        searchInp += " ";
                    searchInp += add;
                    InputType = "Address";
                    req.Address = req.Address.TrimEnd();
                }

                if (request.surname != "undefined" && request.surname != null)
                {
                    req.Surname = request.surname;
                    if (!string.IsNullOrEmpty(searchInp))
                        searchInp += " ";
                    searchInp += req.Surname;
                    InputType = "Surname";
                }
                if (request.emailaddress != "undefined")
                {
                    req.Email = request.emailaddress;
                }

                // krishna start 
                response.SearchInput = searchInp;
                response.CustomerRefNum = request.CustomerRefNum;
                // krishna end

                req.LogEs = _configuration.GetSection("ElasticSearchLog").GetSection("LogEs").Value;
                var res = await _eSService.SearchConsumerAsync(req);

                var idnos = res.data.Select(t => t.IDNumber).ToList();
                var dlist = _IDASDbContext.DoNotCallRegistrys.Where(t => t.IsApproved == true && idnos.Contains(t.Idnumber)).Select(t => t.Idnumber).ToList();
                if (istrail == false)
                {
                    response.recordsTotal = Convert.ToInt32(res.TotalCount);
                    response.recordsFiltered = Convert.ToInt32(res.TotalCount);
                    response.totalTime = res.TotalTime;
                    
                    
                    foreach (var rec in res.data)
                    {
                        var rec1 = new PersonProfileData();

                        if (!dlist.Contains(rec.IDNumber))
                        {
                            if (rec.consumerid != null)
                                rec1.consumerId = rec.consumerid;
                            if (rec.DateOfBirth != null)
                                rec1.age = DateTime.Today.Year - rec.DateOfBirth.Value.Year;
                            if (rec.DateOfBirth != null)
                                rec1.dateOfBirth = rec.DateOfBirth.Value;
                            if (rec.Firstname != null)
                                rec1.fullName = rec.Surname + " " + rec.Firstname;
                            if (rec.Gender != null)
                                rec1.gender = rec.Gender;
                            if (rec.IDNumber != null)
                                rec1.iDNumber = rec.IDNumber;
                            rec1.totalCount = response.recordsTotal;
                            rec1.totalTime = res.TotalTime;
                            rec1.isdeceased = rec.isDeceased;
                            response.data.Add(rec1);
                        }
                    }

                }
                else
                {
                    response.recordsTotal = Convert.ToInt32(res.TotalCount);
                    response.recordsFiltered = Convert.ToInt32(res.TotalCount);
                    response.totalTime = res.TotalTime;

                    foreach (var rec in res.data)
                    {
                        var rec1 = new PersonProfileData();

                        if (!dlist.Contains(rec.IDNumber))
                        {
                            rec1.consumerId = rec.consumerid;
                            rec1.age = 00;
                            rec1.dateOfBirth = DateTime.MinValue;
                            rec1.fullName = GetMaskedData(rec.Surname + " " + rec.Firstname);
                            rec1.gender = rec.Gender;
                            rec1.iDNumber = GetMaskedData(rec.IDNumber);
                            rec1.totalCount = response.recordsTotal;
                            rec1.totalTime = res.TotalTime;
                            rec1.isdeceased = rec.isDeceased;
                            response.data.Add(rec1);
                        }
                    }
                }
                if (istrail == false)
                {
                    var reslogs = _CustomerLogService.UpdateCustomerLog(request.userId, request.CustId, "Tracing", "Search", searchInp, searchInp, InputType, istrail, request.VoucherCode,request.CustomerRefNum, request.IsXDS,request.EnquiryReason);
                    // krishna start new
                    response.RefNum = reslogs.Result.RefNum;
                    // krishna end new
                }
            }
            catch (Exception ex)

            {
                WriteLog(ex.Message + "\n" + ex.Source, "ConsumerTracingError");
                var msg = ex.Message;
            }

            
            

            return response;
        }
        // krishna start
        /// <summary>
        /// Added by Krishna
        /// </summary>
        /// <param name="consumerid"></param>
        /// <returns>Enquiry Reason</returns>
        public async Task<string> GetCustomerEnquiryReason(Guid customerId)
        {
            string enquiryReason = await _IDASDbContext.Customers.Where(x=> x.Id == customerId).Select(x=>x.EnquiryReason).FirstOrDefaultAsync();
            return enquiryReason;
        }

        /// <summary>
        /// Added by Krishna
        /// </summary>
        /// <returns>list of Enquiry Reason LookupData</returns>
        public async Task<List<LookupData>> GetEnquiryReasonLookupDatas()
        {
            List<LookupData> lookupDatas = await _IDASDbContext.LookupDatas
                .Where(l => l.IsActive == true && l.Type == "Enquiry Reason").ToListAsync();
            return lookupDatas;
        }

        // krishna end

        public async Task<CompanySearchResponse> CompanySearch(CompanySearchRequest request)
        {
            List<LookupData> lookupLst = _IDASDbContext.LookupDatas.ToList();
            CompanySearchResponse response = new CompanySearchResponse();
            ESSearchCommercialRequest req = new ESSearchCommercialRequest();

            try
            {
                string searchInp = string.Empty;
                string input = string.Empty;

                if (request.companyName != "undefined" && request.companyName != null)
                {
                    req.CompanyName = request.companyName;
                    searchInp = req.CompanyName;
                    input = "Company Name";
                }
                if (request.companyRegNumber != "undefined" && request.companyRegNumber != null)
                {
                    req.CompanyRegNumber = request.companyRegNumber;
                    if (!string.IsNullOrEmpty(searchInp))
                        searchInp += " ";
                    searchInp += req.CompanyRegNumber;
                    input = "Company Register Number";
                }

                if (request.globalSearch != "undefined" && request.globalSearch != null)
                {
                    if (request.globalSearch.Contains(" "))
                    {
                        var glo = request.globalSearch.Replace(" ", ",");
                        req.GlobalSearch = glo;
                        searchInp = glo;
                    }
                    else
                        req.GlobalSearch = request.globalSearch;
                    searchInp = request.globalSearch;
                    input = "Global Search";
                }
                if (request.commercialAddress != "undefined" && request.commercialAddress != null)
                {
                    if (request.commercialAddress.Contains(","))
                    {
                        var add = request.commercialAddress.Replace(",", " ");
                        if (!string.IsNullOrEmpty(searchInp))
                            searchInp += " ";
                        searchInp += add;
                        input = "Company Address";
                    }
                    else
                    {
                        req.CommercialAddress = request.commercialAddress;
                        if (!string.IsNullOrEmpty(searchInp))
                            searchInp += " ";
                        searchInp += req.CommercialAddress;
                        input = "Company Address";
                    }

                }
                if (request.commercialTelephone != "undefined" && request.commercialTelephone != null)
                {
                    req.CommercialTelephone = request.commercialTelephone;
                    if (!string.IsNullOrEmpty(searchInp))
                        searchInp += " ";
                    searchInp += req.CommercialTelephone;
                    input = "Company Telephone";
                }
                req.EsLog = _configuration.GetSection("ElasticSearchLog").GetSection("LogEs").Value;

                var res = await _eSService.SearchCommercialAsync(req);

                if (!string.IsNullOrEmpty(res.ErrorMessage))
                {
                    WriteLog(res.ErrorMessage, "Commercial Search");
                }


                bool istrail = false;
                if (request.isTrailuser == true)
                {
                    istrail = true;
                }


                if (istrail == false)
                {
                    response.recordsTotal = Convert.ToInt32(res.TotalCount);
                    response.recordsFiltered = Convert.ToInt32(res.TotalCount);

                    if (res.data != null)
                    {
                        foreach (var rec in res.data)
                        {
                            var rec1 = new CompanyData();
                            rec1.companyid = rec.CommercialID;
                            rec1.companyName = rec.CompanyName;
                            rec1.CompanyRegNumber = rec.CompanyRegNumber;
                            var data5 = lookupLst.FirstOrDefault(t => t.Type == "Status Code of Company" && t.Value == rec.CommercialStatusCode);
                            if (data5 != null)
                            {
                                rec1.commercialStatusCode = data5.Text;
                            }
                            rec1.businessStartDate = rec.BusinessStartDate.Value;

                            response.data.Add(rec1);
                        }
                    }
                    else if (res.data2 != null)
                    {
                        foreach (var rec in res.data2)
                        {
                            var rec1 = new CompanyData();
                            rec1.companyid = rec.CommercialID;
                            rec1.companyName = rec.CompanyName;
                            rec1.CompanyRegNumber = rec.CompanyRegNumber;
                            var data5 = lookupLst.FirstOrDefault(t => t.Type == "Status Code of Company" && t.Value == rec.CommercialStatusCode);
                            if (data5 != null)
                            {
                                rec1.commercialStatusCode = data5.Text;
                            }
                            rec1.businessStartDate = rec.BusinessStartDate.Value;

                            response.data.Add(rec1);
                        }
                    }
                }
                else
                {
                    response.recordsTotal = Convert.ToInt32(res.TotalCount);
                    response.recordsFiltered = Convert.ToInt32(res.TotalCount);

                    foreach (var rec in res.data)
                    {
                        var rec1 = new CompanyData();
                        rec1.companyid = rec.CommercialID;
                        rec1.companyName = GetMaskedData(rec.CompanyName);
                        rec1.CompanyRegNumber = GetMaskedData(rec.CompanyRegNumber);
                        var data5 = lookupLst.FirstOrDefault(t => t.Type == "Status Code of Company" && t.Value == rec.CommercialStatusCode);
                        if (data5 != null)
                        {
                            rec1.commercialStatusCode = GetMaskedData(data5.Text);
                        }
                        rec1.businessStartDate = DateTime.MinValue;

                        response.data.Add(rec1);
                    }
                }

                if (request.isTrailuser == false)
                {
                    // krishna pending
                    //var reslogs = _CustomerLogService.UpdateCustomerLog(request.userId, request.CustId, "Tracing", "Search", searchInp, searchInp, input, request.isTrailuser);
                    var reslogs = _CustomerLogService.UpdateCustomerLog(request.userId, request.CustId, "Tracing", "Search", searchInp, searchInp, input, request.isTrailuser, null, null, request.isXDS,null);
                }

            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, "Commercial Search");
            }
            return response;
        }

        public async Task<AddressSearchResponse> AddressSearch(AddressSearchRequest request)
        {
            AddressSearchResponse response = new AddressSearchResponse();
            ESSearchAddressRequest Esreq = new ESSearchAddressRequest();
            ESSearchAddressResponse Esres = new ESSearchAddressResponse();

            string FullAddress = string.Empty;
            try
            {

                if (!string.IsNullOrEmpty(request.Address1) && request.Address1 != "undefined")
                {
                    FullAddress = request.Address1;
                }
                if (!string.IsNullOrEmpty(request.Address2) && request.Address2 != "undefined")
                {
                    if (!string.IsNullOrEmpty(FullAddress))
                    {
                        FullAddress += " " + request.Address2;
                    }
                    else
                        FullAddress = request.Address2;
                }
                if (!string.IsNullOrEmpty(request.Address3) && request.Address3 != "undefined")
                {
                    if (!string.IsNullOrEmpty(FullAddress))
                    {
                        FullAddress += " " + request.Address3;
                    }
                    else
                        FullAddress = request.Address3;
                }
                if (!string.IsNullOrEmpty(request.Address4) && request.Address4 != "undefined")
                {
                    if (!string.IsNullOrEmpty(FullAddress))
                    {
                        FullAddress += " " + request.Address4;
                    }
                    else
                        FullAddress = request.Address4;
                }
                if (!string.IsNullOrEmpty(request.PostalCode) && request.PostalCode != "undefined")
                {
                    if (!string.IsNullOrEmpty(FullAddress))
                    {
                        FullAddress += " " + request.PostalCode;
                    }
                    else
                        FullAddress = request.PostalCode;
                }

                Esreq.Address = FullAddress;

                Esres = await _eSService.SearchAddressAsync(Esreq);

                response.recordsTotal = Convert.ToInt32(Esres.TotalCount);
                response.recordsFiltered = Convert.ToInt32(Esres.TotalCount);
                foreach (var res in Esres.data)
                {
                    var rec = new AddressData();
                    rec.Address = res.Address.Replace(" ", "_");
                    rec.AddressId = res.consumeraddressid;
                    var isfound = response.data.Find(t => t.Address == rec.Address);
                    if (isfound == null)
                    {
                        response.data.Add(rec);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return response;
        }

        public string GetMaskedData(string str)
        {
            if (str != null)
            {
                if (str.Length > 2)
                {
                    return str[0] + new string('X', str.Length - 2) + str.Substring(str.Length - 1);
                }
                else
                    return str;
            }
            return string.Empty;
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

        public int GetIsRestrictedCustomer(Guid id)
        {
            try
            {
                return dAL.getIsRestrictedCustomer(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetIsRestrictedCustomerUser(Guid id)
        {
            try
            {
                return dAL.getIsRestrictedCustomerUser(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }



    public class ConsumerSearchRequest
    {
        public string iDNumber { get; set; }

        public string surname { get; set; }
        public string firstname { get; set; }
        public string address { get; set; }
        public string dateOfBirth { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string phoneNumber { get; set; }
        public string type { get; set; }
        public string globalSearch { get; set; }

        public string emailaddress { get; set; }
        public string searchTimereq { get; set; }
        public Guid userId { get; set; }
        public string consumerId { get; set; }
        public bool isTrailuser { get; set; }
        public Guid CustId { get; set; }

        // Krishna start 
        public string Passport { get; set; }
        public string CustomerRefNum { get; set; }
        public string VoucherCode { get; set; }

        public bool IsXDS { get; set; }

        public string UserName { get; set; }

        public string EnquiryReason { get; set; }

        // Krishna End



    }

    public class ConsumerSearchResponse : DataTableResponse<PersonProfileData>
    {

    }
    public class CompanySearchRequest
    {
        public string companyName { get; set; }
        public string companyRegNumber { get; set; }
        public string type { get; set; }
        public string commercialAddress { get; set; }
        public string commercialTelephone { get; set; }
        public string globalSearch { get; set; }

        public Guid userId { get; set; }
        public string commercialId { get; set; }
        public bool isTrailuser { get; set; }
        public Guid CustId { get; set; }
        public string searchTimereq { get; set; }

        public bool isXDS { get; set; }
    }




    public class CompanySearchResponse : DataTableResponse<CompanyData>
    {

    }

    public class AddressData
    {
        public string AddressId { get; set; }
        public string Address { get; set; }
    }

    public class AddressSearchResponse : DataTableResponse<AddressData>
    {

    }

    public class AddressSearchRequest
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PostalCode { get; set; }
        public bool isTrailuser { get; set; }
    }
    public class CompanyData
    {
        public string CompanyRegNumber { get; set; }
        public string companyName { get; set; }
        public int companyid { get; set; }
        public string commercialStatusCode { get; set; }
        public DateTime businessStartDate { get; set; }
    }
    public class PersonProfileData
    {
        //grid columns
        public int totalCount { get; set; }
        public string consumerId { get; set; }
        public string gender { get; set; }
        public string iDNumber { get; set; }
        public string fullName { get; set; }
        public int age { get; set; }
        public DateTime dateOfBirth { get; set; }
        public bool isdeceased { get; set; }
        public long totalTime { get; set; }

    }


    public class CustomerLogData
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public string LogType { get; set; }

        public Guid CompanyId { get; set; }

        public Guid? CompanyUserId { get; set; }

        public int CreditPoints { get; set; }

        public int DebitPoints { get; set; }

        public string TransactionDetial { get; set; }

        public string RefNo { get; set; }

        public string IdOrPassportNumber { get; set; }

        public bool IsFree { get; set; }


        public string SearchType { get; set; } //Search,Lookup,SocialNetwork

        public string SearchCriteria { get; set; }
    }
}
