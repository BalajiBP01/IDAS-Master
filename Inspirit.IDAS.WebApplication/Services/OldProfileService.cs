using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication.Services
{
    public class OldProfileService
    {
    }
}
//using Inspirit.IDAS.Data;
//using Inspirit.IDAS.Data.IDAS;
//using Inspirit.IDAS.Data.Production;
//using Inspirit.IDAS.ESData;
//using Inspirit.IDAS.WebApplication;
//using Inspirit.IDAS.WebApplication.Model;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;


//namespace Inspirit.IDAS.WebApplication
//{
//    public class ProfileService
//    {
//        ProductionDbContext _productionContext;
//        ESService _eSService;
//        IDASDbContext _IDAScontext;
//        ExcelDataPort<PersonProfile> _export;
//        CustomerLogService _CustomerLogService;
//        private IConfiguration _configuration;
//        private IHostingEnvironment _hostingEnvironment;

//        public ProfileService(ProductionDbContext productionContext, IDASDbContext IDAScontext, ExcelDataPort<PersonProfile> export, CustomerLogService CustomerLogService, IHostingEnvironment IHostingEnvironment, IConfiguration config)
//        {
//            _productionContext = productionContext;
//            _IDAScontext = IDAScontext;
//            _configuration = config;
//            _eSService = new ESService(_configuration.GetSection("ElasticDBIP").GetSection("url").Value);
//            _export = export;
//            _hostingEnvironment = IHostingEnvironment;
//            _CustomerLogService = CustomerLogService;
//        }

//        public PersonProfile GetPersonProfile(ProfileRequest request)

//        {
//            PersonProfile profile = new PersonProfile();
//            //take from cache
//            List<LookupData> lookupLst = _IDAScontext.LookupDatas.ToList();
//            try
//            {
//                if (request.SpouseID != null)
//                {

//                    SqlConnection con = (SqlConnection)_productionContext.Database.GetDbConnection();
//                    using (con)
//                    {
//                        if (con.State != ConnectionState.Open)
//                            con.Open();
//                        DataSet data = new DataSet();
//                        SqlCommand cmd = new SqlCommand("qspConsumerInformation", con);
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.Add(new SqlParameter("@IDNO", request.SpouseID));
//                        cmd.CommandTimeout = 0;

//                        Consumer cons = new Consumer();


//                        SqlDataAdapter sDA = new SqlDataAdapter();
//                        sDA.SelectCommand = cmd;

//                        if (con.State != ConnectionState.Open)
//                            con.Open();

//                        sDA.Fill(data);

//                        profile = GetProfileData(lookupLst, request.istrailuser, data, con);
//                        CustomerLogData logs = new CustomerLogData();

//                        if (request.istrailuser == false)
//                        {
//                            var reslogs = _CustomerLogService.UpdateCustomerLog(request.userId, request.customerId, "Tracing", "Profile", request.SearchCriteria, profile.idNumber, request.InputType, request.istrailuser);
//                        }
//                        else
//                        {
//                            var tlogs = _CustomerLogService.UpdateTrailuserLog(request.userId, request.customerId, "Tracing", "Profile", request.SearchCriteria, profile.idNumber, request.InputType, request.istrailuser);
//                        }
//                        var tabdetails = _IDAScontext.Customers.Where(t => t.Id == request.customerId).FirstOrDefault();
//                        if (tabdetails != null)
//                        {
//                            profile.tabs = tabdetails.TabSelected;
//                        }
//                        if (con.State != ConnectionState.Closed)
//                            con.Close();
//                    }
//                }

//                else
//                {
//                    SqlConnection con = (SqlConnection)_productionContext.Database.GetDbConnection();
//                    using (con)
//                    {
//                        if (con.State != ConnectionState.Open)
//                            con.Open();
//                        DataSet data = new DataSet();
//                        SqlCommand cmd = new SqlCommand("qspConsumerInformation", con);
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.Add(new SqlParameter("@ConsumerId", request.Id));
//                        cmd.CommandTimeout = 0;

//                        Consumer cons = new Consumer();


//                        SqlDataAdapter sDA = new SqlDataAdapter();
//                        sDA.SelectCommand = cmd;

//                        if (con.State != ConnectionState.Open)
//                            con.Open();

//                        sDA.Fill(data);

//                        profile = GetProfileData(lookupLst, request.istrailuser, data, con);
//                        CustomerLogData logs = new CustomerLogData();

//                        if (request.istrailuser == false)
//                        {
//                            var reslogs = _CustomerLogService.UpdateCustomerLog(request.userId, request.customerId, "Tracing", "Profile", request.SearchCriteria, profile.idNumber, request.InputType, request.istrailuser);
//                        }
//                        else
//                        {
//                            var tlogs = _CustomerLogService.UpdateTrailuserLog(request.userId, request.customerId, "Tracing", "Profile", request.SearchCriteria, profile.idNumber, request.InputType, request.istrailuser);
//                        }
//                        var tabs = _IDAScontext.Customers.Where(t => t.Id == request.customerId).FirstOrDefault();
//                        if (tabs != null)
//                        {
//                            profile.tabs = tabs.TabSelected;
//                            if (con.State != ConnectionState.Closed)
//                                con.Close();
//                        }

//                    }

//                }
//            }
//            catch (Exception ex)
//            {
//                var msg = ex;
//                string commrequest = JsonConvert.SerializeObject(msg);
//                WriteLog(commrequest, "Consumer detail");
//            }
//            return profile;
//        }
//        public PersonProfile GetPersonProfileOriginal(ProfileRequest request)

//        {
//            PersonProfile profile = new PersonProfile();
//            //take from cache
//            List<LookupData> lookupLst = _IDAScontext.LookupDatas.ToList();
//            try
//            {
//                if (request.SpouseID != null)
//                {

//                    SqlConnection con = (SqlConnection)_productionContext.Database.GetDbConnection();
//                    using (con)
//                    {
//                        if (con.State != ConnectionState.Open)
//                            con.Open();
//                        DataSet data = new DataSet();
//                        SqlCommand cmd = new SqlCommand("qspConsumerInformation", con);
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.Add(new SqlParameter("@IDNO", request.SpouseID));
//                        cmd.CommandTimeout = 0;

//                        Consumer cons = new Consumer();


//                        SqlDataAdapter sDA = new SqlDataAdapter();
//                        sDA.SelectCommand = cmd;

//                        if (con.State != ConnectionState.Open)
//                            con.Open();

//                        sDA.Fill(data);

//                        profile = GetProfileData(lookupLst, request.istrailuser, data, con);
//                        CustomerLogData logs = new CustomerLogData();

//                        if (request.istrailuser == false)
//                        {
//                            var reslogs = _CustomerLogService.UpdateCustomerLog(request.userId, request.customerId, "Tracing", "Profile", request.SearchCriteria, profile.idNumber, request.InputType, request.istrailuser);
//                        }
//                        else
//                        {
//                            var tlogs = _CustomerLogService.UpdateTrailuserLog(request.userId, request.customerId, "Tracing", "Profile", request.SearchCriteria, profile.idNumber, request.InputType, request.istrailuser);
//                        }
//                        var tabdetails = _IDAScontext.Customers.Where(t => t.Id == request.customerId).FirstOrDefault();
//                        if (tabdetails != null)
//                        {
//                            profile.tabs = tabdetails.TabSelected;
//                        }
//                        if (con.State != ConnectionState.Closed)
//                            con.Close();
//                    }
//                }

//                else
//                {
//                    SqlConnection con = (SqlConnection)_productionContext.Database.GetDbConnection();
//                    using (con)
//                    {
//                        if (con.State != ConnectionState.Open)
//                            con.Open();
//                        DataSet data = new DataSet();
//                        SqlCommand cmd = new SqlCommand("qspConsumerInformation", con);
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.Add(new SqlParameter("@ConsumerId", request.Id));
//                        cmd.CommandTimeout = 0;

//                        Consumer cons = new Consumer();


//                        SqlDataAdapter sDA = new SqlDataAdapter();
//                        sDA.SelectCommand = cmd;

//                        if (con.State != ConnectionState.Open)
//                            con.Open();

//                        sDA.Fill(data);

//                        profile = GetProfileData(lookupLst, request.istrailuser, data, con);
//                        CustomerLogData logs = new CustomerLogData();

//                        if (request.istrailuser == false)
//                        {
//                            var reslogs = _CustomerLogService.UpdateCustomerLog(request.userId, request.customerId, "Tracing", "Profile", request.SearchCriteria, profile.idNumber, request.InputType, request.istrailuser);
//                        }
//                        else
//                        {
//                            var tlogs = _CustomerLogService.UpdateTrailuserLog(request.userId, request.customerId, "Tracing", "Profile", request.SearchCriteria, profile.idNumber, request.InputType, request.istrailuser);
//                        }
//                        var tabs = _IDAScontext.Customers.Where(t => t.Id == request.customerId).FirstOrDefault();
//                        if (tabs != null)
//                        {
//                            profile.tabs = tabs.TabSelected;
//                            if (con.State != ConnectionState.Closed)
//                                con.Close();
//                        }

//                    }

//                }
//            }
//            catch (Exception ex)
//            {
//                var msg = ex;
//                string commrequest = JsonConvert.SerializeObject(msg);
//                WriteLog(commrequest, "Consumer detail");
//            }
//            return profile;
//        }


//        private PersonProfile GetProfileData(List<LookupData> lookupLst, bool istrail, DataSet dataSet, SqlConnection con)
//        {
//            PersonProfile pres = new PersonProfile();

//            List<string> considerTelephones = new List<string>();

//            List<TimeLine> timelines = new List<TimeLine>();
//            try
//            {
//                bool Normal = istrail;
//                if (Normal == false)
//                {

//                    DataTable dtconsumer = dataSet.Tables[0];

//                    if (dtconsumer.Rows.Count > 0)
//                    {
//                        if (dtconsumer.Rows[0]["IDNO"] != DBNull.Value)
//                            pres.idNumber = dtconsumer.Rows[0]["IDNO"].ToString();
//                        else
//                            pres.idNumber = "Unknown";
//                        if (dtconsumer.Rows[0]["ConsumerID"] != DBNull.Value)
//                            pres.id = dtconsumer.Rows[0]["ConsumerID"].ToString();

//                        if (dtconsumer.Rows[0]["PassportNo"] != DBNull.Value)
//                            pres.passportNo = dtconsumer.Rows[0]["PassportNo"].ToString();
//                        else
//                            pres.passportNo = "Unknown";
//                        if (dtconsumer.Rows[0]["TitleCode"] != DBNull.Value)
//                            pres.titleCode = dtconsumer.Rows[0]["TitleCode"].ToString();
//                        else
//                            pres.titleCode = "Unknown";
//                        if (dtconsumer.Rows[0]["FirstInitial"] != DBNull.Value)
//                            pres.firstInitial = dtconsumer.Rows[0]["FirstInitial"].ToString();
//                        else
//                            pres.firstInitial = "Unknown";
//                        if (dtconsumer.Rows[0]["FirstName"] != DBNull.Value)
//                            pres.firstName = Convert.ToString(dtconsumer.Rows[0]["FirstName"]);
//                        else
//                            pres.firstName = "Unknown";

//                        if (dtconsumer.Rows[0]["SecondName"] != DBNull.Value)
//                            pres.secondName = Convert.ToString(dtconsumer.Rows[0]["SecondName"]);
//                        else
//                            pres.surname = "Unknown";

//                        if (dtconsumer.Rows[0]["MaidenName"] != DBNull.Value)
//                            pres.maidenName = dtconsumer.Rows[0]["MaidenName"].ToString();
//                        else
//                            pres.maidenName = "Unknown";
//                        if (dtconsumer.Rows[0]["Surname"] != DBNull.Value)
//                            pres.surname = Convert.ToString(dtconsumer.Rows[0]["Surname"]);
//                        else
//                            pres.surname = "Unknown";
//                        if (dtconsumer.Rows[0]["ThirdName"] != DBNull.Value)
//                            pres.thirdName = Convert.ToString(dtconsumer.Rows[0]["ThirdName"]);
//                        else
//                            pres.thirdName = "Unknown";
//                        if (dtconsumer.Rows[0]["BirthDate"] != DBNull.Value)
//                            pres.birthDate = Convert.ToDateTime(dtconsumer.Rows[0]["BirthDate"]);
//                        if (dtconsumer.Rows[0]["GenderInd"] != DBNull.Value)
//                        {
//                            var data2 = lookupLst.Where(t => t.Type == "Gender Indicator" && t.Value == Convert.ToString(dtconsumer.Rows[0]["GenderInd"])).FirstOrDefault();
//                            if (data2 != null)
//                            {
//                                pres.genderInd = data2.Text;
//                            }
//                        }
//                        else
//                            pres.genderInd = "Unknown";
//                        if (dtconsumer.Rows[0]["RecordStatusInd"] != DBNull.Value)
//                            pres.recordstatusind = Convert.ToString(dtconsumer.Rows[0]["RecordStatusInd"]);
//                        if (dtconsumer.Rows[0]["CreatedOnDate"] != DBNull.Value)
//                            pres.createdOnDate = Convert.ToDateTime(dtconsumer.Rows[0]["CreatedOnDate"]);
//                        if (dtconsumer.Rows[0]["LastUpdatedDate"] != DBNull.Value)
//                            pres.lastUpdatedDate = Convert.ToDateTime(dtconsumer.Rows[0]["LastUpdatedDate"]);
//                        if (dtconsumer.Rows[0]["Alloy"] != DBNull.Value)
//                            pres.contactScore = dtconsumer.Rows[0]["Alloy"].ToString();
//                        if (dtconsumer.Rows[0]["LSM"] != DBNull.Value)
//                            pres.lsm = dtconsumer.Rows[0]["LSM"].ToString();
//                        else
//                            pres.lsm = "Unknown";
//                    }
//                    else
//                    {
//                        pres.ErrorMessage = "No data available for this ID Number";
//                        return pres;

//                    }
//                    //ConsumerHomeAffairs
//                    DataTable dtHomeAffairs = dataSet.Tables[1];

//                    if (dtHomeAffairs.Rows.Count > 0)
//                    {

//                        if (dtHomeAffairs.Rows[0]["NameCombo"] != DBNull.Value)
//                            pres.nameCombo = Convert.ToString(dtHomeAffairs.Rows[0]["NameCombo"]);
//                        else
//                            pres.nameCombo = "Unknown";
//                        if (dtHomeAffairs.Rows[0]["LastUpdatedDate"] != DBNull.Value)
//                            pres.lastupdatesHomeAffire = Convert.ToDateTime(dtHomeAffairs.Rows[0]["LastUpdatedDate"]);
//                        if (dtHomeAffairs.Rows[0]["IDIssuedDate"] != DBNull.Value)
//                            pres.iDIssuedDate = Convert.ToDateTime(dtHomeAffairs.Rows[0]["IDIssuedDate"]);

//                        if (dtHomeAffairs.Rows[0]["MarriageDate"] != DBNull.Value)
//                        {
//                            pres.marriageDate = Convert.ToDateTime(dtHomeAffairs.Rows[0]["MarriageDate"]);
//                            pres.maritalStatus = "MARRIED";
//                            if (dtHomeAffairs.Rows[0]["DivorceDate"] != DBNull.Value)
//                            {
//                                pres.divorceDate = Convert.ToDateTime(dtHomeAffairs.Rows[0]["DivorceDate"]);
//                                if (pres.divorceDate > pres.marriageDate)
//                                { pres.maritalStatus = "DIVORCED"; }
//                            }
//                        }
//                        if (dtHomeAffairs.Rows[0]["PlaceOfMarriage"] != DBNull.Value)
//                            pres.placeOfMarriage = Convert.ToString(dtHomeAffairs.Rows[0]["PlaceOfMarriage"]);
//                        else
//                            pres.placeOfMarriage = "Unknown";
//                        if (dtHomeAffairs.Rows[0]["SpouseIdnoOrDOB"] != DBNull.Value)
//                            pres.spouseIdnoOrDOB = Convert.ToString(dtHomeAffairs.Rows[0]["SpouseIdnoOrDOB"]);
//                        if (dtHomeAffairs.Rows[0]["SpouseSurName"] != DBNull.Value)
//                            pres.spouseSurName = dtHomeAffairs.Rows[0]["SpouseSurName"].ToString();
//                        else
//                            pres.spouseSurName = "Unknown";
//                        if (dtHomeAffairs.Rows[0]["SpouseForeNames"] != DBNull.Value)
//                            pres.spouseForeNames = dtHomeAffairs.Rows[0]["SpouseForeNames"].ToString();
//                        else
//                            pres.spouseForeNames = "Unknown";
//                        if (dtHomeAffairs.Rows[0]["DivorceDate"] != DBNull.Value)
//                            pres.divorceDate = Convert.ToDateTime(dtHomeAffairs.Rows[0]["DivorceDate"]);
//                        if (dtHomeAffairs.Rows[0]["DivorceIssuedCourt"] != DBNull.Value)
//                            pres.divorceIssuedCourt = dtHomeAffairs.Rows[0]["DivorceIssuedCourt"].ToString();
//                        else
//                            pres.divorceIssuedCourt = "Unknown";
//                        if (dtHomeAffairs.Rows[0]["CauseOfDeath"] != DBNull.Value)
//                            pres.causeOfDeath = Convert.ToString(dtHomeAffairs.Rows[0]["CauseOfDeath"]);
//                        else
//                            pres.causeOfDeath = "Unknown";
//                        if (dtHomeAffairs.Rows[0]["DeceasedStatus"] != DBNull.Value)
//                            pres.deceasedStatus = Convert.ToString(dtHomeAffairs.Rows[0]["DeceasedStatus"]);
//                        else
//                            pres.deceasedStatus = "Unknown";
//                        if (dtHomeAffairs.Rows[0]["DeceasedDate"] != DBNull.Value)
//                            pres.deceasedDate = Convert.ToDateTime(dtHomeAffairs.Rows[0]["DeceasedDate"]);
//                        if (dtHomeAffairs.Rows[0]["PlaceOfDeath"] != DBNull.Value)
//                            pres.placeOfDeath = Convert.ToString(dtHomeAffairs.Rows[0]["PlaceOfDeath"]);
//                        else
//                            pres.placeOfDeath = "Unknown";

//                    }
//                    //lsm
//                    DataTable dtLSM = dataSet.Tables[8];
//                    if (dtLSM.Rows.Count > 0)
//                    {
//                        if (dtLSM.Rows[0]["RiskCategory"] != DBNull.Value)
//                            pres.riskScore = dtLSM.Rows[0]["RiskCategory"].ToString();
//                        else
//                            pres.riskScore = "Unknown";
//                    }


//                    // Consumer Address
//                    DataTable dtAddress = dataSet.Tables[2];
//                    if (dtAddress.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtAddress.Rows)
//                        {
//                            var address = new AddressDetail();
//                            if (dr["AddressTypeInd"] != DBNull.Value)
//                            {
//                                var data5 = lookupLst.Where(t => t.Type == "Address Type Indicator" && t.Value == Convert.ToString(dr["AddressTypeInd"])).FirstOrDefault();
//                                if (data5 != null)
//                                {
//                                    address.addressTypeInd = data5.Text;

//                                }
//                            }
//                            if (dr["OriginalAddress1"] != DBNull.Value)
//                                address.originalAddress1 = Convert.ToString(dr["OriginalAddress1"]);
//                            if (dr["OriginalAddress2"] != DBNull.Value)
//                                address.originalAddress2 = Convert.ToString(dr["OriginalAddress2"]);
//                            if (dr["OriginalAddress3"] != DBNull.Value)
//                                address.originalAddress3 = Convert.ToString(dr["OriginalAddress3"]);
//                            if (dr["OriginalAddress4"] != DBNull.Value)
//                                address.originalAddress4 = Convert.ToString(dr["OriginalAddress4"]);
//                            if (dr["OriginalPostalCode"] != DBNull.Value)
//                                address.originalPostalCode = Convert.ToString(dr["OriginalPostalCode"]);
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                address.lastUpdatedDate = Convert.ToDateTime(dr["LastUpdatedDate"]);
//                            if (dr["CreatedOnDate"] != DBNull.Value)
//                                address.createdOnDate = Convert.ToDateTime(dr["CreatedOnDate"]);
//                            if (dr["ConsumerAddressID"] != DBNull.Value)
//                                address.id = Convert.ToString(dr["ConsumerAddressID"]);
//                            if (dr["OccupantTypeInd"] != DBNull.Value)
//                            {
//                                var data1 = lookupLst.Where(t => t.Type == "Occupant Type Indicator" && t.Value == Convert.ToString(dr["OccupantTypeInd"])).FirstOrDefault();
//                                if (data1 != null)
//                                {
//                                    address.occupantTypeInd = data1.Text;

//                                }
//                            }
//                            address.fullAddress = "";
//                            if (!string.IsNullOrEmpty(address.originalAddress1))
//                                address.fullAddress += address.originalAddress1;

//                            if (!string.IsNullOrEmpty(address.originalAddress2))
//                            {
//                                if (!string.IsNullOrEmpty(address.fullAddress))
//                                    address.fullAddress += ", ";
//                                address.fullAddress += address.originalAddress2;
//                            }
//                            if (!string.IsNullOrEmpty(address.originalAddress3))
//                            {
//                                if (!string.IsNullOrEmpty(address.fullAddress))
//                                    address.fullAddress += ", ";
//                                address.fullAddress += address.originalAddress3;
//                            }

//                            if (!string.IsNullOrEmpty(address.originalAddress4))
//                            {
//                                if (!string.IsNullOrEmpty(address.fullAddress))
//                                    address.fullAddress += ", ";
//                                address.fullAddress += address.originalAddress4;
//                            }
//                            if (!string.IsNullOrEmpty(address.originalPostalCode))
//                            {
//                                if (!string.IsNullOrEmpty(address.fullAddress))
//                                    address.fullAddress += ", ";
//                                address.fullAddress += address.originalPostalCode;
//                            }
//                            if (dr["Province"] != DBNull.Value)
//                                address.province = Convert.ToString(dr["Province"]);
//                            if (dr["Town"] != DBNull.Value)
//                                address.town = Convert.ToString(dr["Town"]);
//                            if (dr["Region"] != DBNull.Value)
//                                address.region = Convert.ToString(dr["Region"]);
//                            var addrpresent = pres.addresses.Find(m => m.originalAddress1 == address.originalAddress1 && m.originalAddress2 == address.originalAddress2 && m.originalAddress3 == address.originalAddress3
//                            && m.originalAddress4 == address.originalAddress4 && m.addressTypeInd == address.addressTypeInd);
//                            if (addrpresent == null)
//                            {
//                                pres.addresses.Add(address);
//                            }

//                        }
//                        pres.addresses = pres.addresses.OrderByDescending(t => t.lastUpdatedDate).ToList();
//                        foreach (var timeadd in pres.addresses)
//                        {
//                            if (timeadd.lastUpdatedDate != null)
//                            {
//                                //timelines
//                                var addresspresent = timelines.FindAll(p => p.Type == timeadd.addressTypeInd).ToList();

//                                if (addresspresent.Count == 0)
//                                {
//                                    TimeLine line = new TimeLine();
//                                    line.LastupdatedDate = timeadd.lastUpdatedDate.Value;
//                                    line.Text = timeadd.originalAddress1 + " " + timeadd.originalAddress2 + " " + timeadd.originalAddress3 + " " + timeadd.originalAddress4;
//                                    line.TableName = "Address";
//                                    line.Type = timeadd.addressTypeInd;
//                                    timelines.Add(line);
//                                }

//                            }
//                        }

//                    }
//                    //Consumer Employment
//                    DataTable dtEmployment = dataSet.Tables[3];

//                    if (dtEmployment.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtEmployment.Rows)
//                        {
//                            var employee = new Employeement();
//                            if (dr["Employer"] != DBNull.Value)
//                                employee.employer = Convert.ToString(dr["Employer"]);
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                employee.date = Convert.ToDateTime(dr["LastUpdatedDate"]);
//                            if (dr["CreatedOnDate"] != DBNull.Value)
//                                employee.CreatedDate = Convert.ToDateTime(dr["CreatedOnDate"]);
//                            if (dr["Occupation"] != DBNull.Value)
//                                employee.occupation = Convert.ToString(dr["Occupation"]);
//                            if (dr["ConsumerID"] != DBNull.Value)
//                                employee.id = Convert.ToString(dr["ConsumerID"]);
//                            var empExists = pres.employees.Find(m => m.occupation == employee.occupation && m.employer == employee.employer);
//                            if (empExists == null)
//                            {
//                                pres.employees.Add(employee);
//                            }
//                        }
//                    }

//                    // Telephones
//                    List<string> telephoneinf = new List<string>();
//                    DataTable dtTelephone = dataSet.Tables[4];
//                    DataTable telcount = dataSet.Tables[18];
//                    if (dtTelephone.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtTelephone.Rows)
//                        {
//                            var contact = new ContactDetail();


//                            if (dr["TelephoneTypeInd"] != DBNull.Value)
//                            {
//                                var data10 = lookupLst.Where(t => t.Type == "Telephone Type Indicator" && t.Value == Convert.ToString(dr["TelephoneTypeInd"])).FirstOrDefault();
//                                if (data10 != null)
//                                {
//                                    contact.type = data10.Text;
//                                }
//                            }


//                            if (dr["ConsumerTelephoneID"] != DBNull.Value)

//                                contact.id = Convert.ToInt64(dr["ConsumerTelephoneID"]);
//                            if (dr["TelephoneID"] != DBNull.Value)
//                                contact.id = Convert.ToInt64(dr["TelephoneID"]);
//                            if (dr["InternationalDialingCode"] != DBNull.Value)
//                                contact.internationalDialingCode = Convert.ToString(dr["InternationalDialingCode"]);
//                            if (dr["TelephoneCode"] != DBNull.Value)
//                                contact.telephoneCode = Convert.ToString(dr["TelephoneCode"]);
//                            if (dr["TelephoneNo"] != DBNull.Value)
//                                contact.telephoneNo = Convert.ToString(dr["TelephoneNo"]);
//                            contact.ExistingTelephoneNo = contact.telephoneNo;


//                            if (dr["CreatedonDate"] != DBNull.Value)
//                                contact.createdonDate = Convert.ToDateTime(dr["CreatedonDate"]);
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                contact.lastUpdatedDate = Convert.ToDateTime(dr["LastUpdatedDate"]);
//                            if (dr["ChangedonDate"] != DBNull.Value)
//                                contact.ChangedonDate = Convert.ToDateTime(dr["ChangedonDate"]);
//                            if (dr["CreatedonDate"] != DBNull.Value)
//                                contact.linkedDate = Convert.ToDateTime(dr["CreatedonDate"]);
//                            if (contact.internationalDialingCode == "27")
//                            {
//                                contact.telephoneNo = "0" + contact.telephoneNo;
//                            }
//                            var telephoneexists = pres.contacts.Find(t => t.telephoneNo == contact.telephoneNo && t.type == contact.type);
//                            if (telephoneexists != null)
//                            {
//                                if (contact.createdonDate > telephoneexists.createdonDate)
//                                {
//                                    pres.contacts.Remove(telephoneexists);
//                                    pres.contacts.Add(contact);
//                                }
//                            }
//                            else
//                            {

//                                if (contact.telephoneNo.Length == 10 || contact.internationalDialingCode != "27")
//                                {
//                                    if (contact.internationalDialingCode != "27")
//                                    {
//                                        contact.telephoneNo = contact.internationalDialingCode + contact.telephoneNo;
//                                    }
//                                    if (contact.createdonDate.Value != null)
//                                    {

//                                        if (telcount.Rows.Count > 0)
//                                        {
//                                            foreach (DataRow drtellinked in telcount.Select("TelephoneNo = " + contact.ExistingTelephoneNo.ToString()))
//                                            {
//                                                if (drtellinked["countphones"] != DBNull.Value)
//                                                    contact.peopleLinked = Convert.ToInt32(drtellinked["countphones"]);
//                                            }
//                                        }


//                                        considerTelephones.Add(contact.ExistingTelephoneNo);
//                                        pres.contacts.Add(contact);
//                                    }
//                                    if (!telephoneinf.Contains(contact.telephoneCode))
//                                    {
//                                        telephoneinf.Add(contact.telephoneCode);
//                                    }
//                                }
//                            }
//                        }


//                    }


//                    DataTable dtTelephoneCode = dataSet.Tables[9];


//                    if (dtTelephoneCode.Rows.Count > 0)
//                    {
//                        foreach (ContactDetail t in pres.contacts)
//                        {
//                            DataRow[] dr = dtTelephoneCode.Select("Code = '" + t.telephoneCode + "'");
//                            if (dr.Count() > 0)
//                            {
//                                if (dr[0]["Region"] != DBNull.Value)
//                                    t.CodeRegion = Convert.ToString(dr[0]["Region"]);
//                                if (dr[0]["Type"] != DBNull.Value)
//                                    t.CodeType = Convert.ToString(dr[0]["Type"]);
//                            }
//                            else
//                            {
//                                t.CodeRegion = "UnKnown";
//                                t.CodeType = "UnKnown";
//                            }
//                        }
//                    }

//                    // Consumer Emails
//                    DataTable dtEmails = dataSet.Tables[5];
//                    DataTable dtEmailCount = dataSet.Tables[17];
//                    if (dtEmails.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtEmails.Rows)
//                        {
//                            var contact = new ContactDetail();
//                            if (dr["ID"] != DBNull.Value)
//                                contact.id = Convert.ToInt64(dr["ID"]);
//                            if (dr["EmailID"] != DBNull.Value)
//                                contact.telephoneNo = Convert.ToString(dr["EmailID"]);
//                            if (dr["CreatedOnDate"] != DBNull.Value)
//                                contact.createdonDate = Convert.ToDateTime(dr["CreatedOnDate"]);
//                            contact.type = "Email";
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                contact.lastUpdatedDate = Convert.ToDateTime(dr["LastUpdatedDate"]);

//                            if (dtEmailCount.Rows.Count > 0)
//                            {
//                                int drcount = dtEmailCount.Select("EmailID = '" + dr["EmailID"] + "'").Count();
//                                contact.peopleLinked = drcount;
//                            }
//                            var emailexists = pres.contacts.Find(m => m.telephoneNo == contact.telephoneNo);
//                            if (emailexists == null)
//                            {
//                                pres.contacts.Add(contact);
//                            }
//                        }
//                        //both contact detail and email 
//                        pres.contacts = pres.contacts.OrderByDescending(t => t.lastUpdatedDate).ToList();
//                        foreach (var contact in pres.contacts)
//                        {
//                            if (contact.lastUpdatedDate != null)
//                            {
//                                var contactpresent = timelines.FindAll(p => p.Type == contact.type).ToList();

//                                if (contactpresent.Count == 0)
//                                {
//                                    TimeLine line = new TimeLine();
//                                    line.LastupdatedDate = contact.lastUpdatedDate.Value;
//                                    line.Text = contact.telephoneNo;
//                                    line.TableName = "Contacts";
//                                    line.Type = contact.type;
//                                    timelines.Add(line);
//                                }
//                            }
//                        }


//                    }

//                    //Directors
//                    DataTable dtDirectors = dataSet.Tables[10];
//                    DataTable dtCommercialDirectors = dataSet.Tables[11];
//                    DataTable dtCommercials = dataSet.Tables[12];

//                    if (dtCommercialDirectors.Rows.Count > 0)
//                        foreach (DataRow drCommercialDirector in dtCommercialDirectors.Rows)
//                        {
//                            var director = new DirectorShip();
//                            foreach (DataRow drCommercial in dtCommercials.Select("CommercialID = " + drCommercialDirector["CommercialID"].ToString()))
//                            {
//                                if (drCommercial["CommercialName"] != DBNull.Value)
//                                    director.companyname = Convert.ToString(drCommercial["CommercialName"]);
//                                if (drCommercial["CommercialID"] != DBNull.Value)
//                                    director.companyid = Convert.ToInt32(drCommercial["CommercialID"]);
//                                if (drCommercial["RegistrationNo"] != DBNull.Value)
//                                    director.companyno = Convert.ToString(drCommercial["RegistrationNo"]);
//                            }
//                            if (drCommercialDirector["DirectorID"] != DBNull.Value)
//                                director.id = Convert.ToString(drCommercialDirector["DirectorID"]);
//                            if (drCommercialDirector["AppointmentDate"] != DBNull.Value)
//                                director.appointmentDate = Convert.ToDateTime(drCommercialDirector["AppointmentDate"]);
//                            if (drCommercialDirector["CreatedOnDate"] != DBNull.Value)
//                                director.createdOnDate = Convert.ToDateTime(drCommercialDirector["CreatedOnDate"]);
//                            if (drCommercialDirector["CommercialDirectorID"] != DBNull.Value)
//                                director.CommDirId = drCommercialDirector["CommercialDirectorID"].ToString();
//                            if (drCommercialDirector["DirectorDesignationCode"] != DBNull.Value)
//                            {
//                                var data1 = lookupLst.Where(t => t.Type == "Director Designation Code" && t.Value == Convert.ToString(drCommercialDirector["DirectorDesignationCode"])).FirstOrDefault();
//                                if (data1 != null)
//                                {
//                                    director.directorDesignationCode = data1.Text;
//                                }
//                            }
//                            if (drCommercialDirector["DirectorSetDate"] != DBNull.Value)
//                                director.directorSetDate = Convert.ToDateTime(drCommercialDirector["DirectorSetDate"]);
//                            if (drCommercialDirector["DirectorStatusDate"] != DBNull.Value)
//                                director.directorStatusDate = Convert.ToDateTime(drCommercialDirector["DirectorStatusDate"]);
//                            if (drCommercialDirector["DirectorStatusCode"] != DBNull.Value)
//                            {
//                                var data3 = lookupLst.Where(t => t.Type == "Director Status Code" && t.Value == Convert.ToString(drCommercialDirector["DirectorStatusCode"])).FirstOrDefault();
//                                if (data3 != null)
//                                {
//                                    director.directorStatusCode = data3.Text;
//                                }
//                            }
//                            if (drCommercialDirector["DirectorTypeCode"] != DBNull.Value)
//                            {
//                                var data5 = lookupLst.Where(t => t.Type == "Director Type Code" && t.Value == Convert.ToString(drCommercialDirector["DirectorTypeCode"])).FirstOrDefault();
//                                if (data5 != null)
//                                {
//                                    director.directorTypeCode = data5.Text;
//                                }
//                            }
//                            if (drCommercialDirector["Estate"] != DBNull.Value)
//                                director.estate = Convert.ToString(drCommercialDirector["Estate"]);
//                            if (drCommercialDirector["IsRSAResidentYN"] != DBNull.Value)
//                            {
//                                var data6 = lookupLst.Where(t => t.Type == "Is RSA Resident YN" && t.Value == Convert.ToString(drCommercialDirector["IsRSAResidentYN"])).FirstOrDefault();
//                                if (data6 != null)
//                                {
//                                    director.isRSAResidentYN = data6.Text;
//                                }
//                            }
//                            if (drCommercialDirector["MemberControlPerc"] != DBNull.Value)
//                                director.memberControlPerc = Convert.ToDecimal(drCommercialDirector["MemberControlPerc"]);
//                            if (drCommercialDirector["MemberSize"] != DBNull.Value)
//                                director.memberSize = Convert.ToDecimal(drCommercialDirector["MemberSize"]);
//                            if (drCommercialDirector["Profession"] != DBNull.Value)
//                                director.profession = Convert.ToString(drCommercialDirector["Profession"]);
//                            if (drCommercialDirector["RegisterNo"] != DBNull.Value)
//                                director.registerNo = Convert.ToString(drCommercialDirector["RegisterNo"]);
//                            if (drCommercialDirector["TrusteeOf"] != DBNull.Value)
//                                director.trusteeOf = Convert.ToString(drCommercialDirector["TrusteeOf"]);

//                            DataRow[] drDirector = dtDirectors.Select("DirectorID = " + drCommercialDirector["DirectorID"].ToString());
//                            if (drDirector[0]["DirectorStatusDate"] != DBNull.Value)
//                                director.directorStatusDate = Convert.ToDateTime(drDirector[0]["DirectorStatusDate"]);
//                            if (drDirector[0]["LastUpdatedDate"] != DBNull.Value)
//                                director.lastUpdatedDate = Convert.ToDateTime(drDirector[0]["LastUpdatedDate"]);
//                            if (drDirector[0]["FirstInitial"] != DBNull.Value)
//                                director.firstInitial = drDirector[0]["FirstInitial"].ToString();
//                            if (drDirector[0]["SecondInitial"] != DBNull.Value)
//                                director.secondInitial = drDirector[0]["SecondInitial"].ToString();
//                            if (drDirector[0]["FirstName"] != DBNull.Value)
//                                director.fullName = drDirector[0]["FirstName"].ToString();
//                            if (drDirector[0]["SecondName"] != DBNull.Value)
//                                director.fullName = string.IsNullOrEmpty(director.fullName) ? drDirector[0]["SecondName"].ToString() : director.fullName + drDirector[0]["SecondName"].ToString();
//                            if (drDirector[0]["Surname"] != DBNull.Value)
//                                director.surname = drDirector[0]["Surname"].ToString();
//                            if (drDirector[0]["SurnamePrevious"] != DBNull.Value)
//                                director.surnamePrevious = drDirector[0]["SurnamePrevious"].ToString();
//                            if (drDirector[0]["BirthDate"] != DBNull.Value)
//                                director.birthDate = Convert.ToDateTime(drDirector[0]["BirthDate"]);
//                            if (drDirector[0]["IDNo"] != DBNull.Value)
//                                director.idNumber = drDirector[0]["IDNo"].ToString();


//                            var direxists = pres.directorShips.Find(t => t.appointmentDate == director.appointmentDate && t.companyno == director.companyno);
//                            if (direxists == null)
//                            {
//                                pres.directorShips.Add(director);
//                            }

//                        }

//                    //Property deeds Buyer
//                    DataTable dtBuyer = dataSet.Tables[13];
//                    DataTable dtdeedBuyer = dataSet.Tables[14];
//                    DataTable dtBuyerExt = dataSet.Tables[22];

//                    if (dtBuyer.Rows.Count > 0)
//                        foreach (DataRow dr in dtBuyer.Rows)
//                        {
//                            var property1 = new PropertyDeedDetail();


//                            foreach (DataRow drBuyer in dtBuyerExt.Rows)
//                            {
//                                if (drBuyer["PropertyDeedId"].ToString() == dr["PropertyDeedId"].ToString())
//                                {

//                                    //BuyerIDNO, BuyerName
//                                    property1.sellerIDNO = property1.sellerIDNO + " - " + drBuyer["SellerIDNO"].ToString();
//                                    property1.sellerName = property1.sellerName + " - " + drBuyer["SellerName"].ToString();
//                                }
//                            }
//                            foreach (DataRow drdeed in dtdeedBuyer.Select("PropertyDeedId = " + dr["PropertyDeedId"].ToString()))
//                            {
//                                if (drdeed["DeedsOfficeId"] != DBNull.Value)
//                                {
//                                    var data11 = lookupLst.Where(t => t.Type == "Deeds Office Identifier" && t.Value == drdeed["DeedsOfficeId"].ToString()).FirstOrDefault();
//                                    if (data11 != null)
//                                    {
//                                        property1.deedsOfficeId = data11.Text;
//                                    }
//                                }
//                                if (drdeed["TitleDeedNo"] != DBNull.Value)
//                                    property1.titleDeedNo = drdeed["TitleDeedNo"].ToString();
//                                if (drdeed["TitleDeedNoOLD"] != DBNull.Value)
//                                    property1.titleDeedNoOLD = drdeed["TitleDeedNoOLD"].ToString();
//                                if (drdeed["TitleDeedFee"] != DBNull.Value)
//                                    property1.titleDeedFee = (Convert.ToInt32(drdeed["TitleDeedFee"]));
//                                if (drdeed["DatePurchase"] != DBNull.Value)
//                                    property1.datePurchase = Convert.ToDateTime(drdeed["DatePurchase"]);
//                                if (drdeed["DateRegister"] != DBNull.Value)
//                                    property1.dateRegister = Convert.ToDateTime(drdeed["DateRegister"]);
//                                if (drdeed["PurchaseAmount"] != DBNull.Value)
//                                    property1.purchaseAmount = Convert.ToDecimal(drdeed["PurchaseAmount"]);
//                                if (drdeed["StreetAddress"] != DBNull.Value)
//                                    property1.streetAddress = drdeed["StreetAddress"].ToString();
//                                if (drdeed["StreetNumber"] != DBNull.Value)
//                                    property1.streetNumber = drdeed["StreetNumber"].ToString();
//                                if (drdeed["StreetName"] != DBNull.Value)
//                                    property1.streetName = drdeed["StreetName"].ToString();
//                                if (drdeed["StreetType"] != DBNull.Value)
//                                    property1.streetType = drdeed["StreetType"].ToString();
//                                if (drdeed["X"] != DBNull.Value)
//                                    property1.x = Convert.ToDecimal(drdeed["X"]);
//                                if (drdeed["Y"] != DBNull.Value)
//                                    property1.y = Convert.ToDecimal(drdeed["Y"]);
//                                if (drdeed["SuburbCode"] != DBNull.Value)
//                                    property1.suburbCode = drdeed["SuburbCode"].ToString();
//                                if (drdeed["SuburbDeeds"] != DBNull.Value)
//                                    property1.suburbDeeds = drdeed["SuburbDeeds"].ToString();
//                                if (drdeed["Town"] != DBNull.Value)
//                                    property1.town = drdeed["Town"].ToString();
//                                if (drdeed["Authority"] != DBNull.Value)
//                                    property1.authority = drdeed["Authority"].ToString();
//                                if (drdeed["MunicipalityName"] != DBNull.Value)
//                                    property1.municipalityName = drdeed["MunicipalityName"].ToString();
//                                if (drdeed["ProvinceId"] != DBNull.Value)
//                                {

//                                    var data = lookupLst.Where(t => t.Type == "Province Identifier" && t.Value == drdeed["ProvinceId"].ToString()).FirstOrDefault();
//                                    if (data != null)
//                                    {
//                                        property1.provinceId = data.Text;

//                                    }
//                                }
//                                if (drdeed["IsCurrOwnerUpdated"] != DBNull.Value)
//                                {
//                                    var data7 = lookupLst.Where(t => t.Type == "Is Current Owner Updated" && t.Value == drdeed["IsCurrOwnerUpdated"].ToString()).FirstOrDefault();
//                                    if (data7 != null)
//                                    {
//                                        property1.isCurrOwnerUpdated = data7.Text;
//                                    }
//                                }
//                                if (drdeed["Extent"] != DBNull.Value)
//                                    property1.extent = drdeed["Extent"].ToString();
//                                if (drdeed["AttorneyFirmNumber"] != DBNull.Value)
//                                    property1.attorneyFirmNumber = drdeed["AttorneyFirmNumber"].ToString();
//                                if (drdeed["AttorneyFileNumber"] != DBNull.Value)
//                                    property1.attorneyFileNumber = drdeed["AttorneyFileNumber"].ToString();
//                                if (drdeed["TransferSeqNo"] != DBNull.Value)
//                                    property1.transferSeqNo = Convert.ToInt32(drdeed["TransferSeqNo"]);
//                                if (drdeed["DateCaptured"] != DBNull.Value)
//                                    property1.dateCaptured = Convert.ToDateTime(drdeed["DateCaptured"]);
//                                if (drdeed["BondNumber"] != DBNull.Value)
//                                    property1.bondNumber = drdeed["BondNumber"].ToString();
//                                if (drdeed["BondHolder"] != DBNull.Value)
//                                    property1.bondHolder = drdeed["BondHolder"].ToString();
//                                if (drdeed["BondAmount"] != DBNull.Value)
//                                    property1.bondAmount = Convert.ToInt64(drdeed["BondAmount"]);
//                                if (drdeed["PropertyType"] != DBNull.Value)
//                                {
//                                    var data5 = lookupLst.Where(t => t.Type == "Property Type" && t.Value == drdeed["PropertyType"].ToString()).FirstOrDefault();
//                                    if (data5 != null)
//                                    {
//                                        property1.propertyType = data5.Text;

//                                    }
//                                }
//                                if (drdeed["PropertyName"] != DBNull.Value)
//                                    property1.propertyName = drdeed["PropertyName"].ToString();
//                                if (drdeed["SchemeId"] != DBNull.Value)
//                                    property1.schemeId = drdeed["SchemeId"].ToString();
//                                if (drdeed["SuburbId"] != DBNull.Value)
//                                    property1.suburbId = Convert.ToInt16(drdeed["SuburbId"]);
//                                if (drdeed["Erf"] != DBNull.Value)
//                                    property1.erf = drdeed["Erf"].ToString();
//                                if (drdeed["Portion"] != DBNull.Value)
//                                    property1.portion = Convert.ToInt32(drdeed["Portion"]);
//                                if (drdeed["Unit"] != DBNull.Value)
//                                    property1.unit = Convert.ToInt32(drdeed["Unit"]);
//                                if (drdeed["CreatedOndate"] != DBNull.Value)
//                                    property1.createdOndate = Convert.ToDateTime(drdeed["CreatedOndate"]);
//                                if (drdeed["ErfSize"] != DBNull.Value)
//                                    property1.erfSize = drdeed["ErfSize"].ToString();
//                                if (drdeed["StandNo"] != DBNull.Value)
//                                    property1.standNo = drdeed["StandNo"].ToString();
//                                if (drdeed["PortionNo"] != DBNull.Value)
//                                    property1.portionNo = drdeed["PortionNo"].ToString();
//                                if (drdeed["TownShipNo"] != DBNull.Value)
//                                    property1.townShipNo = Convert.ToInt32(drdeed["TownShipNo"]);
//                                if (drdeed["PrevExtent"] != DBNull.Value)
//                                    property1.prevExtent = drdeed["PrevExtent"].ToString();
//                                if (drdeed["IsCurrentOwner"] != DBNull.Value)
//                                    property1.isCurrentOwner = Convert.ToBoolean(drdeed["IsCurrentOwner"]);
//                                if (drdeed["PropertyDeedID"] != DBNull.Value)
//                                    property1.propertyDeedId = Convert.ToInt32(drdeed["PropertyDeedID"]);
//                                if (drdeed["StreetAddress"] != DBNull.Value)
//                                    property1.address = drdeed["StreetAddress"].ToString();

//                            }


//                            if (dr["BuyerID"] != DBNull.Value)
//                                property1.buyerid = dr["BuyerID"].ToString();
//                            if (dr["BuyerIDNO"] != DBNull.Value)
//                                property1.buyerIDNO = dr["BuyerIDNO"].ToString();
//                            if (dr["BuyerName"] != DBNull.Value)
//                                property1.buyerName = dr["BuyerName"].ToString();
//                            if (dr["BuyerType"] != DBNull.Value)
//                            {

//                                var data1 = lookupLst.Where(t => t.Type == "Buyer Type" && t.Value == dr["BuyerType"].ToString()).FirstOrDefault();
//                                if (data1 != null)
//                                {
//                                    property1.buyerType = data1.Text;

//                                }
//                            }
//                            property1.fullAddress = "";
//                            if (!string.IsNullOrEmpty(property1.streetNumber))
//                                property1.fullAddress += property1.streetNumber;

//                            if (!string.IsNullOrEmpty(property1.streetName))
//                            {
//                                if (!string.IsNullOrEmpty(property1.fullAddress))
//                                    property1.fullAddress += ", ";
//                                property1.fullAddress += property1.streetName;
//                            }
//                            if (!string.IsNullOrEmpty(property1.streetType))
//                            {
//                                if (!string.IsNullOrEmpty(property1.fullAddress))
//                                    property1.fullAddress += ", ";
//                                property1.fullAddress += property1.streetType;
//                            }

//                            if (!string.IsNullOrEmpty(property1.town))
//                            {
//                                if (!string.IsNullOrEmpty(property1.fullAddress))
//                                    property1.fullAddress += ", ";
//                                property1.fullAddress += property1.town;
//                            }
//                            if (dr["BuyerStatus"] != DBNull.Value)
//                                property1.buyerStatus = dr["BuyerStatus"].ToString();
//                            if (dr["Share"] != DBNull.Value)
//                                property1.share = dr["Share"].ToString();
//                            property1.type = "Buyer";

//                            var propertybuyerexists = pres.propertyOwners.Find(t => t.type == property1.type && t.datePurchase == property1.datePurchase && t.fullAddress == property1.fullAddress && t.isCurrentOwner == property1.isCurrentOwner);
//                            if (propertybuyerexists == null)
//                            {
//                                pres.propertyOwners.Add(property1);
//                            }
//                        }

//                    //Property deeds Sellar
//                    DataTable dtSellar = dataSet.Tables[15];
//                    DataTable dtdeedSeller = dataSet.Tables[16];


//                    if (dtSellar.Rows.Count > 0)
//                        foreach (DataRow dr1 in dtSellar.Rows)
//                        {
//                            var property1 = new PropertyDeedDetail();
//                            foreach (DataRow drdeed1 in dtdeedSeller.Select("PropertyDeedId = " + dr1["PropertyDeedId"].ToString()))
//                            {
//                                if (drdeed1["DeedsOfficeId"] != DBNull.Value)
//                                {
//                                    var data11 = lookupLst.Where(t => t.Type == "Deeds Office Identifier" && t.Value == drdeed1["DeedsOfficeId"].ToString()).FirstOrDefault();
//                                    if (data11 != null)
//                                    {
//                                        property1.deedsOfficeId = data11.Text;
//                                    }
//                                }
//                                if (drdeed1["TitleDeedNo"] != DBNull.Value)
//                                    property1.titleDeedNo = drdeed1["TitleDeedNo"].ToString();
//                                if (drdeed1["TitleDeedNoOLD"] != DBNull.Value)
//                                    property1.titleDeedNoOLD = drdeed1["TitleDeedNoOLD"].ToString();
//                                if (drdeed1["TitleDeedFee"] != DBNull.Value)
//                                    property1.titleDeedFee = (Convert.ToInt32(drdeed1["TitleDeedFee"]));
//                                if (drdeed1["DatePurchase"] != DBNull.Value)
//                                    property1.datePurchase = Convert.ToDateTime(drdeed1["DatePurchase"]);
//                                if (drdeed1["DateRegister"] != DBNull.Value)
//                                    property1.dateRegister = Convert.ToDateTime(drdeed1["DateRegister"]);
//                                if (drdeed1["PurchaseAmount"] != DBNull.Value)
//                                    property1.purchaseAmount = Convert.ToDecimal(drdeed1["PurchaseAmount"]);
//                                if (drdeed1["StreetAddress"] != DBNull.Value)
//                                    property1.streetAddress = drdeed1["StreetAddress"].ToString();
//                                if (drdeed1["StreetNumber"] != DBNull.Value)
//                                    property1.streetNumber = drdeed1["StreetNumber"].ToString();
//                                if (drdeed1["StreetName"] != DBNull.Value)
//                                    property1.streetName = drdeed1["StreetName"].ToString();
//                                if (drdeed1["StreetType"] != DBNull.Value)
//                                    property1.streetType = drdeed1["StreetType"].ToString();
//                                if (drdeed1["X"] != DBNull.Value)
//                                    property1.x = Convert.ToDecimal(drdeed1["X"]);
//                                if (drdeed1["Y"] != DBNull.Value)
//                                    property1.y = Convert.ToDecimal(drdeed1["Y"]);
//                                if (drdeed1["SuburbCode"] != DBNull.Value)
//                                    property1.suburbCode = drdeed1["SuburbCode"].ToString();
//                                if (drdeed1["SuburbDeeds"] != DBNull.Value)
//                                    property1.suburbDeeds = drdeed1["SuburbDeeds"].ToString();
//                                if (drdeed1["Town"] != DBNull.Value)
//                                    property1.town = drdeed1["Town"].ToString();
//                                if (drdeed1["Authority"] != DBNull.Value)
//                                    property1.authority = drdeed1["Authority"].ToString();
//                                if (drdeed1["MunicipalityName"] != DBNull.Value)
//                                    property1.municipalityName = drdeed1["MunicipalityName"].ToString();
//                                if (drdeed1["ProvinceId"] != DBNull.Value)
//                                {

//                                    var data = lookupLst.Where(t => t.Type == "Province Identifier" && t.Value == drdeed1["ProvinceId"].ToString()).FirstOrDefault();
//                                    if (data != null)
//                                    {
//                                        property1.provinceId = data.Text;

//                                    }
//                                }
//                                if (drdeed1["IsCurrOwnerUpdated"] != DBNull.Value)
//                                {
//                                    var data7 = lookupLst.Where(t => t.Type == "Is Current Owner Updated" && t.Value == drdeed1["IsCurrOwnerUpdated"].ToString()).FirstOrDefault();
//                                    if (data7 != null)
//                                    {
//                                        property1.isCurrOwnerUpdated = data7.Text;
//                                    }
//                                }
//                                if (drdeed1["Extent"] != DBNull.Value)
//                                    property1.extent = drdeed1["Extent"].ToString();
//                                if (drdeed1["AttorneyFirmNumber"] != DBNull.Value)
//                                    property1.attorneyFirmNumber = drdeed1["AttorneyFirmNumber"].ToString();
//                                if (drdeed1["AttorneyFileNumber"] != DBNull.Value)
//                                    property1.attorneyFileNumber = drdeed1["AttorneyFileNumber"].ToString();
//                                if (drdeed1["TransferSeqNo"] != DBNull.Value)
//                                    property1.transferSeqNo = Convert.ToInt32(drdeed1["TransferSeqNo"]);
//                                if (drdeed1["DateCaptured"] != DBNull.Value)
//                                    property1.dateCaptured = Convert.ToDateTime(drdeed1["DateCaptured"]);
//                                if (drdeed1["BondNumber"] != DBNull.Value)
//                                    property1.bondNumber = drdeed1["BondNumber"].ToString();
//                                if (drdeed1["BondHolder"] != DBNull.Value)
//                                    property1.bondHolder = drdeed1["BondHolder"].ToString();
//                                if (drdeed1["BondAmount"] != DBNull.Value)
//                                    property1.bondAmount = Convert.ToInt64(drdeed1["BondAmount"]);
//                                if (drdeed1["PropertyType"] != DBNull.Value)
//                                {
//                                    var data5 = lookupLst.Where(t => t.Type == "Property Type" && t.Value == drdeed1["PropertyType"].ToString()).FirstOrDefault();
//                                    if (data5 != null)
//                                    {
//                                        property1.propertyType = data5.Text;

//                                    }
//                                }
//                                if (drdeed1["PropertyName"] != DBNull.Value)
//                                    property1.propertyName = drdeed1["PropertyName"].ToString();
//                                if (drdeed1["SchemeId"] != DBNull.Value)
//                                    property1.schemeId = drdeed1["SchemeId"].ToString();
//                                if (drdeed1["SuburbId"] != DBNull.Value)
//                                    property1.suburbId = Convert.ToInt16(drdeed1["SuburbId"]);
//                                if (drdeed1["Erf"] != DBNull.Value)
//                                    property1.erf = drdeed1["Erf"].ToString();
//                                if (drdeed1["Portion"] != DBNull.Value)
//                                    property1.portion = Convert.ToInt32(drdeed1["Portion"]);
//                                if (drdeed1["Unit"] != DBNull.Value)
//                                    property1.unit = Convert.ToInt32(drdeed1["Unit"]);
//                                if (drdeed1["CreatedOndate"] != DBNull.Value)
//                                    property1.createdOndate = Convert.ToDateTime(drdeed1["CreatedOndate"]);
//                                if (drdeed1["ErfSize"] != DBNull.Value)
//                                    property1.erfSize = drdeed1["ErfSize"].ToString();
//                                if (drdeed1["StandNo"] != DBNull.Value)
//                                    property1.standNo = drdeed1["StandNo"].ToString();
//                                if (drdeed1["PortionNo"] != DBNull.Value)
//                                    property1.portionNo = drdeed1["PortionNo"].ToString();
//                                if (drdeed1["TownShipNo"] != DBNull.Value)
//                                    property1.townShipNo = Convert.ToInt32(drdeed1["TownShipNo"]);
//                                if (drdeed1["PrevExtent"] != DBNull.Value)
//                                    property1.prevExtent = drdeed1["PrevExtent"].ToString();
//                                if (drdeed1["IsCurrentOwner"] != DBNull.Value)
//                                    property1.isCurrentOwner = Convert.ToBoolean(drdeed1["IsCurrentOwner"]);
//                                if (drdeed1["PropertyDeedID"] != DBNull.Value)
//                                    property1.propertyDeedId = Convert.ToInt32(drdeed1["PropertyDeedID"]);
//                                if (drdeed1["StreetAddress"] != DBNull.Value)
//                                    property1.address = drdeed1["StreetAddress"].ToString();




//                                if (dr1["SellerID"] != DBNull.Value)
//                                    property1.sellarid = dr1["SellerID"].ToString();
//                                if (dr1["SellerIDNO"] != DBNull.Value)
//                                    property1.sellerIDNO = dr1["SellerIDNO"].ToString();
//                                if (dr1["SellerName"] != DBNull.Value)
//                                    property1.sellerName = dr1["SellerName"].ToString();
//                                if (dr1["SellerType"] != DBNull.Value)
//                                {

//                                    var data1 = lookupLst.Where(t => t.Type == "Buyer Type" && t.Value == dr1["SellerType"].ToString()).FirstOrDefault();
//                                    if (data1 != null)
//                                    {
//                                        property1.sellerType = data1.Text;

//                                    }
//                                }
//                                property1.fullAddress = "";
//                                if (!string.IsNullOrEmpty(property1.streetNumber))
//                                    property1.fullAddress += property1.streetNumber;

//                                if (!string.IsNullOrEmpty(property1.streetName))
//                                {
//                                    if (!string.IsNullOrEmpty(property1.fullAddress))
//                                        property1.fullAddress += ", ";
//                                    property1.fullAddress += property1.streetName;
//                                }
//                                if (!string.IsNullOrEmpty(property1.streetType))
//                                {
//                                    if (!string.IsNullOrEmpty(property1.fullAddress))
//                                        property1.fullAddress += ", ";
//                                    property1.fullAddress += property1.streetType;
//                                }

//                                if (!string.IsNullOrEmpty(property1.town))
//                                {
//                                    if (!string.IsNullOrEmpty(property1.fullAddress))
//                                        property1.fullAddress += ", ";
//                                    property1.fullAddress += property1.town;
//                                }

//                                property1.sellerStatus = dr1["SellerStatus"].ToString();
//                                property1.type = "Seller";


//                                var propertybuyerexists = pres.propertyOwners.Find(t => t.type == property1.type && t.datePurchase == property1.datePurchase && t.fullAddress == property1.fullAddress && t.isCurrentOwner == property1.isCurrentOwner);
//                                if (propertybuyerexists == null)
//                                {
//                                    pres.propertyOwners.Add(property1);
//                                }
//                            }
//                        }
//                    // Judgements

//                    DataTable dtJudgement = dataSet.Tables[6];

//                    if (dtJudgement.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtJudgement.Rows)
//                        {
//                            var judgement = new Consumerjudgement();
//                            if (dr["ConsumerJudgementID"] != DBNull.Value)
//                                judgement.id = Convert.ToString(dr["ConsumerJudgementID"]);
//                            if (dr["IDNo"] != DBNull.Value)
//                                judgement.idno = Convert.ToString(dr["IDNo"]);
//                            if (dr["CaseNumber"] != DBNull.Value)
//                                judgement.casenumber = Convert.ToString(dr["CaseNumber"]);
//                            if (dr["CaseFilingDate"] != DBNull.Value)
//                                judgement.casefilingdate = Convert.ToDateTime(dr["CaseFilingDate"]);
//                            if (dr["CaseReason"] != DBNull.Value)
//                                judgement.casereason = Convert.ToString(dr["CaseReason"]);
//                            if (dr["CaseType"] != DBNull.Value)
//                                judgement.casetype = Convert.ToString(dr["CaseType"]);
//                            if (dr["DisputeAmt"] != DBNull.Value)
//                                judgement.disputeamt = Convert.ToDecimal(dr["DisputeAmt"]);
//                            if (dr["CourtName"] != DBNull.Value)
//                                judgement.courtname = Convert.ToString(dr["CourtName"]);
//                            if (dr["CourtCity"] != DBNull.Value)
//                                judgement.courtcity = Convert.ToString(dr["CourtCity"]);
//                            if (dr["CourtType"] != DBNull.Value)
//                                judgement.courttype = Convert.ToString(dr["CourtType"]);
//                            if (dr["PlaintiffName"] != DBNull.Value)
//                                judgement.plaintiffname = Convert.ToString(dr["PlaintiffName"]);
//                            if (dr["PlaintiffAddress1"] != DBNull.Value)
//                                judgement.plaintiffaddress1 = Convert.ToString(dr["PlaintiffAddress1"]);
//                            if (dr["PlaintiffAddress2"] != DBNull.Value)
//                                judgement.plaintiffaddress2 = Convert.ToString(dr["PlaintiffAddress2"]);
//                            if (dr["PlaintiffAddress3"] != DBNull.Value)
//                                judgement.plaintiffaddress3 = Convert.ToString(dr["PlaintiffAddress3"]);
//                            if (dr["PlaintiffAddress4"] != DBNull.Value)
//                                judgement.plaintiffaddress4 = Convert.ToString(dr["PlaintiffAddress4"]);
//                            if (dr["PlaintiffPostalCode"] != DBNull.Value)
//                                judgement.plaintifpostalcode = Convert.ToString(dr["PlaintiffPostalCode"]);
//                            if (dr["AttorneyName"] != DBNull.Value)
//                                judgement.attorneyname = Convert.ToString(dr["AttorneyName"]);
//                            if (dr["AttorneyTelephoneCode"] != DBNull.Value)
//                                judgement.attorneytelephonecode = Convert.ToString(dr["AttorneyTelephoneCode"]);
//                            if (dr["AttorneyTelephoneNo"] != DBNull.Value)
//                                judgement.attorneytelephoneno = Convert.ToString(dr["AttorneyTelephoneNo"]);
//                            if (dr["AttorneyFaxCode"] != DBNull.Value)
//                                judgement.attorneyfaxcode = Convert.ToString(dr["AttorneyFaxCode"]);
//                            if (dr["AttorneyFaxNo"] != DBNull.Value)
//                                judgement.attorneyfaxno = Convert.ToString(dr["AttorneyFaxNo"]);
//                            if (dr["AttorneyAddress1"] != DBNull.Value)
//                                judgement.attorneyaddress1 = Convert.ToString(dr["AttorneyAddress1"]);
//                            if (dr["AttorneyAddress2"] != DBNull.Value)
//                                judgement.attorneyaddress2 = Convert.ToString(dr["AttorneyAddress2"]);
//                            if (dr["AttorneyAddress3"] != DBNull.Value)
//                                judgement.attorneyaddress3 = Convert.ToString(dr["AttorneyAddress3"]);
//                            if (dr["AttorneyAddress4"] != DBNull.Value)
//                                judgement.attorneyaddress4 = Convert.ToString(dr["AttorneyAddress4"]);
//                            if (dr["AttorneyPostalCode"] != DBNull.Value)
//                                judgement.attorneypostalcode = Convert.ToString(dr["AttorneyPostalCode"]);
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                judgement.lastupdateddate = Convert.ToDateTime(dr["LastUpdatedDate"]);
//                            if (dr["CreatedOnDate"] != DBNull.Value)
//                                judgement.createdodate = Convert.ToDateTime(dr["CreatedOnDate"]);
//                            if (dr["JudgementTypeCode"] != DBNull.Value)
//                            {

//                                var data11 = lookupLst.FirstOrDefault(t => t.Type == "Judgement Type Code" && t.Value == Convert.ToString(dr["JudgementTypeCode"]));
//                                if (data11 != null)
//                                {
//                                    judgement.judgementtypecode = data11.Text;
//                                }
//                            }
//                            if (dr["DisputeDate"] != DBNull.Value)
//                                judgement.disputedate = Convert.ToDateTime(dr["DisputeDate"]);
//                            if (dr["DisputeResolvedDate"] != DBNull.Value)
//                                judgement.disputeresolveddate = Convert.ToDateTime(dr["DisputeResolvedDate"]);
//                            if (dr["Rescinded"] != DBNull.Value)
//                                judgement.rescinded = Convert.ToBoolean(dr["Rescinded"]); // Need Clarity
//                            if (dr["RescissionDate"] != DBNull.Value)
//                                judgement.rescissiondate = Convert.ToDateTime(dr["RescissionDate"]);
//                            if (dr["RescissionReason"] != DBNull.Value)
//                                judgement.rescissionreason = Convert.ToString(dr["RescissionReason"]);
//                            if (dr["RescindedAmount"] != DBNull.Value)
//                                judgement.rescindedamount = Convert.ToString(dr["RescindedAmount"]);
//                            if (dr["JudgementTypeCode"] != DBNull.Value)
//                                judgement.judgementtypecode = Convert.ToString(dr["JudgementTypeCode"]).ToUpper();
//                            if (dr["CaseFilingDate"] != DBNull.Value)
//                                judgement.casefilingdate = Convert.ToDateTime(dr["CaseFilingDate"]);
//                            if (dr["Rescinded"] == DBNull.Value)
//                            {
//                                var judgeexists = pres.consumerjudgements.Find(t => t.casenumber == judgement.casenumber && t.plaintiffname == judgement.plaintiffname);
//                                if (judgeexists == null)
//                                {
//                                    pres.consumerjudgements.Add(judgement);
//                                }
//                            }
//                        }
//                    }



//                    // Debit Reviews

//                    DataTable dtDebt = dataSet.Tables[7];

//                    if (dtDebt.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtDebt.Rows)
//                        {
//                            var consdeb = new consumerdebtreview();
//                            if (dr["DebtCounsellorRegistrationNo"] != DBNull.Value)
//                                consdeb.debtCounsellorRegistrationNo = Convert.ToString(dr["DebtCounsellorRegistrationNo"]);
//                            if (dr["DebtCounsellorFirstName"] != DBNull.Value)
//                                consdeb.debtCounsellorFirstName = Convert.ToString(dr["DebtCounsellorFirstName"]);
//                            if (dr["DebtCounsellorSurname"] != DBNull.Value)
//                                consdeb.debtCounsellorSurname = Convert.ToString(dr["DebtCounsellorSurname"]);
//                            if (dr["DebtReviewStatusDate"] != DBNull.Value)
//                                consdeb.applicationCreationDate = Convert.ToDateTime(dr["ApplicationCreationDate"]);
//                            if (dr["DebtReviewStatusDate"] != DBNull.Value)
//                                consdeb.debtReviewStatusDate = Convert.ToDateTime(dr["DebtReviewStatusDate"]);
//                            if (dr["ConsumerDebtReviewID"] != DBNull.Value)
//                                consdeb.consumerDebtReviewID = Convert.ToInt64(dr["ConsumerDebtReviewID"]);
//                            if (dr["ConsumerID"] != DBNull.Value)
//                                consdeb.consumerID = Convert.ToInt64(dr["ConsumerID"]);
//                            if (dr["DebtCounsellorTelephoneCode"] != DBNull.Value)
//                                consdeb.debtCounsellorTelephoneCode = Convert.ToString(dr["DebtCounsellorTelephoneCode"]);
//                            if (dr["DebtCounsellorTelephoneNo"] != DBNull.Value)
//                                consdeb.debtCounsellorTelephoneNo = Convert.ToString(dr["DebtCounsellorTelephoneNo"]);
//                            consdeb.telephoneCodeandNumber = string.Empty;

//                            if (!string.IsNullOrEmpty(consdeb.debtCounsellorTelephoneCode))
//                                consdeb.telephoneCodeandNumber += consdeb.debtCounsellorTelephoneCode;

//                            if (!string.IsNullOrEmpty(consdeb.debtCounsellorTelephoneNo))
//                            {
//                                if (!string.IsNullOrEmpty(consdeb.telephoneCodeandNumber))
//                                    consdeb.telephoneCodeandNumber += ", ";
//                                consdeb.telephoneCodeandNumber += consdeb.debtCounsellorTelephoneNo;
//                            }
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                consdeb.lastUpdatedDate = Convert.ToDateTime(dr["LastUpdatedDate"]);
//                            if (dr["DebtReviewStatusCode"] != DBNull.Value)
//                            {
//                                var data1 = lookupLst.FirstOrDefault(t => t.Type == "Debt Review Status Code" && t.Value == Convert.ToString(dr["DebtReviewStatusCode"]));//todo
//                                if (data1 != null)
//                                {
//                                    consdeb.debtReviewStatusCode = data1.Text;

//                                }
//                            }

//                            var debtexists = pres.consumerDebtReview.Find(t => t.debtCounsellorRegistrationNo == consdeb.debtCounsellorRegistrationNo && t.debtCounsellorFirstName == consdeb.debtCounsellorFirstName && t.debtCounsellorSurname == consdeb.debtCounsellorSurname
//                            && t.debtReviewStatusCode == consdeb.debtReviewStatusCode);
//                            if (debtexists == null)
//                            {
//                                if (dr["DebtReviewStatusCode"] != DBNull.Value)
//                                {
//                                    string Code = dr["DebtReviewStatusCode"].ToString();

//                                    if (!string.IsNullOrEmpty(Code))
//                                    {
//                                        pres.consumerDebtReview.Add(consdeb);
//                                    }
//                                }
//                            }
//                        }
//                    }

//                    List<ReletionshipLinkVm> lstvm = new List<ReletionshipLinkVm>();
//                    int relationshipcount = Convert.ToInt32(_IDAScontext.ApplicationSetting.Where(t => t.SettingName.ToUpper() == "RelationshipCount").Select(t => t.SettingValue).FirstOrDefault());



//                    DataTable dtdirelink = dataSet.Tables[20];


//                    if (dtdirelink.Rows.Count > 0)
//                    {
//                        foreach (DataRow reldrlink in dtdirelink.Rows)
//                        {
//                            ReletionshipLinkVm rel = new ReletionshipLinkVm();
//                            string fname = string.Empty;
//                            string sname = string.Empty;
//                            if (reldrlink["FirstName"] != DBNull.Value)
//                                fname = reldrlink["FirstName"].ToString();
//                            if (reldrlink["Surname"] != DBNull.Value)
//                                sname = reldrlink["Surname"].ToString();
//                            rel.FullName = fname + " " + sname;
//                            if (reldrlink["IDNo"] != DBNull.Value)
//                                rel.IdNumber = reldrlink["IDNo"].ToString();
//                            if (reldrlink["CommercialName"] != DBNull.Value)
//                                rel.KeyMatch = reldrlink["CommercialName"].ToString();
//                            rel.Type = "Director";
//                            rel.MatchCriteria = "Director";
//                            if (rel.IdNumber != pres.idNumber && pres.relationships.Count < relationshipcount)
//                            {
//                                var linkpres = lstvm.Find(t => t.IdNumber == rel.IdNumber);
//                                if (linkpres == null)
//                                {
//                                    lstvm.Add(rel);
//                                }
//                            }
//                        }
//                    }

//                    if (lstvm.Count < relationshipcount)
//                    {
//                        DataTable dtpropertylink = dataSet.Tables[21];
//                        if (dtpropertylink.Rows.Count > 0)
//                        {
//                            foreach (DataRow drprop in dtpropertylink.Rows)
//                            {
//                                ReletionshipLinkVm rel = new ReletionshipLinkVm();

//                                if (drprop["BuyerName"] != DBNull.Value)
//                                    rel.FullName = drprop["BuyerName"].ToString();
//                                if (drprop["BuyerIDNO"] != DBNull.Value)
//                                    rel.IdNumber = drprop["BuyerIDNO"].ToString();
//                                rel.Type = "Property";
//                                rel.MatchCriteria = "Property";
//                                if (rel.IdNumber != pres.idNumber && lstvm.Count < relationshipcount)
//                                {
//                                    var linkpres = lstvm.Find(t => t.IdNumber == rel.IdNumber);
//                                    if (linkpres == null)
//                                    {
//                                        lstvm.Add(rel);
//                                    }
//                                }
//                            }
//                        }
//                    }

//                    DataTable dtTelRel = dataSet.Tables[19];
//                    DataTable daatable1 = dataSet.Tables[19];
//                    List<DataRow> rows = dtTelRel.Select("Surname = '" + pres.surname + "'").ToList();

//                    List<DataRow> row1 = daatable1.Select("Surname <> '" + pres.surname + "'").ToList();

//                    if (lstvm.Count < relationshipcount)
//                    {
//                        if (rows.Count > 0)
//                        {

//                            foreach (DataRow reldr in rows)
//                            {
//                                if (lstvm.Count < relationshipcount)
//                                {
//                                    if (reldr["IDNO"] != DBNull.Value && reldr["TelephoneNo"] != DBNull.Value)
//                                        if (reldr["IDNO"].ToString() != pres.idNumber)
//                                        {
//                                            ReletionshipLinkVm rel = new ReletionshipLinkVm();


//                                            if (reldr["ConsumerID"] != DBNull.Value)
//                                                rel.ConsumerId = Convert.ToInt64(reldr["ConsumerID"]);

//                                            rel.IdNumber = reldr["IDNO"].ToString();
//                                            string fname = string.Empty;
//                                            string sname = string.Empty;
//                                            if (reldr["FirstName"] != DBNull.Value)
//                                                fname = reldr["FirstName"].ToString();
//                                            if (reldr["Surname"] != DBNull.Value)
//                                                sname = reldr["Surname"].ToString();
//                                            rel.FullName = fname + " " + sname;

//                                            rel.KeyMatch = reldr["InternationalDialingCode"].ToString() == "27" ? "0" + reldr["TelephoneNo"].ToString() : reldr["InternationalDialingCode"].ToString() + reldr["TelephoneNo"].ToString();
//                                            rel.Type = "Telephone";
//                                            rel.MatchCriteria = "TelephoneSurname";
//                                            var linkpresent = lstvm.Find(t => t.IdNumber == rel.IdNumber);
//                                            if (linkpresent == null)
//                                            {
//                                                lstvm.Add(rel);
//                                            }


//                                        }
//                                }
//                            }
//                        }

//                        if (lstvm.Count < relationshipcount)
//                        {
//                            if (row1.Count > 0)
//                                foreach (DataRow reldr in row1)
//                                {
//                                    if (lstvm.Count < relationshipcount)
//                                    {
//                                        if (reldr["IDNO"] != DBNull.Value && reldr["TelephoneNo"] != DBNull.Value)
//                                            if (reldr["IDNO"].ToString() != pres.idNumber)
//                                            {
//                                                ReletionshipLinkVm rel = new ReletionshipLinkVm();


//                                                if (reldr["ConsumerID"] != DBNull.Value)
//                                                    rel.ConsumerId = Convert.ToInt64(reldr["ConsumerID"]);
//                                                if (reldr["IDNO"] != DBNull.Value)
//                                                    rel.IdNumber = reldr["IDNO"].ToString();
//                                                string fname = string.Empty;
//                                                string sname = string.Empty;
//                                                if (reldr["FirstName"] != DBNull.Value)
//                                                    fname = reldr["FirstName"].ToString();
//                                                if (reldr["Surname"] != DBNull.Value)
//                                                    sname = reldr["Surname"].ToString();
//                                                rel.FullName = fname + " " + sname;
//                                                if (reldr["TelephoneNo"] != DBNull.Value)
//                                                    rel.KeyMatch = reldr["InternationalDialingCode"].ToString() == "27" ? "0" + reldr["TelephoneNo"].ToString() : reldr["InternationalDialingCode"].ToString() + reldr["TelephoneNo"].ToString();
//                                                rel.Type = "Telephone";
//                                                rel.MatchCriteria = "Telephone";
//                                                var linkpresent = lstvm.Find(t => t.IdNumber == rel.IdNumber);
//                                                if (linkpresent == null)
//                                                {
//                                                    lstvm.Add(rel);
//                                                }


//                                            }
//                                    }
//                                }
//                        }
//                    }

//                    if (lstvm.Count > 0)
//                    {
//                        var order1 = lstvm.Where(t => t.MatchCriteria == "Director").OrderBy(t => t.IdNumber).ToList();
//                        foreach (var o1 in order1)
//                        {
//                            int number = 1;
//                            if (pres.relationships.Count > 0)
//                            {
//                                number = pres.relationships.Count() + 1;
//                            }
//                            o1.DisplayNo = number.ToString();
//                            pres.relationships.Add(o1);
//                        }
//                        var order2 = lstvm.Where(t => t.MatchCriteria == "Property").OrderBy(t => t.IdNumber).ToList();
//                        foreach (var o2 in order2)
//                        {
//                            int number = 1;
//                            if (pres.relationships.Count > 0)
//                            {
//                                number = pres.relationships.Count() + 1;
//                            }
//                            o2.DisplayNo = number.ToString();
//                            pres.relationships.Add(o2);
//                        }
//                        var order3 = lstvm.Where(t => t.MatchCriteria == "TelephoneSurname").OrderBy(t => t.IdNumber).ToList();
//                        foreach (var o3 in order3)
//                        {
//                            int number = 1;
//                            if (pres.relationships.Count > 0)
//                            {
//                                number = pres.relationships.Count() + 1;
//                            }
//                            o3.DisplayNo = number.ToString();
//                            pres.relationships.Add(o3);
//                        }
//                        var order4 = lstvm.Where(t => t.MatchCriteria == "Telephone").OrderBy(t => t.IdNumber).ToList();
//                        foreach (var o4 in order4)
//                        {
//                            int number = 1;
//                            if (pres.relationships.Count > 0)
//                            {
//                                number = pres.relationships.Count() + 1;
//                            }
//                            o4.DisplayNo = number.ToString();
//                            pres.relationships.Add(o4);
//                        }
//                    }

//                    pres.timelines = timelines.OrderByDescending(t => t.LastupdatedDate).ToList();


//                }
//                else
//                {

//                    DataTable dtconsumer = dataSet.Tables[0];

//                    if (dtconsumer.Rows.Count > 0)
//                    {
//                        if (dtconsumer.Rows[0]["IDNO"] != DBNull.Value)
//                            pres.idNumber = GetMaskedData(dtconsumer.Rows[0]["IDNO"].ToString());
//                        else
//                            pres.idNumber = GetMaskedData("Unknown");
//                        if (dtconsumer.Rows[0]["PassportNo"] != DBNull.Value)
//                        {
//                            pres.passportNo = GetMaskedData(dtconsumer.Rows[0]["PassportNo"].ToString());
//                        }
//                        else
//                        {
//                            pres.passportNo = GetMaskedData("Unknown");
//                        }
//                        if (dtconsumer.Rows[0]["TitleCode"] != DBNull.Value)
//                        {
//                            pres.titleCode = GetMaskedData(dtconsumer.Rows[0]["TitleCode"].ToString());
//                        }
//                        else
//                        {
//                            pres.titleCode = GetMaskedData("Unknown");
//                        }
//                        if (dtconsumer.Rows[0]["FirstInitial"] != DBNull.Value)
//                        {
//                            pres.firstInitial = GetMaskedData(dtconsumer.Rows[0]["FirstInitial"].ToString());
//                        }
//                        else
//                        {
//                            pres.firstInitial = GetMaskedData("Unknown");
//                        }
//                        if (dtconsumer.Rows[0]["FirstName"] != DBNull.Value)
//                            pres.firstName = GetMaskedData(Convert.ToString(dtconsumer.Rows[0]["FirstName"]));
//                        else
//                            pres.firstName = GetMaskedData("Unknown");
//                        if (dtconsumer.Rows[0]["SecondName"] != DBNull.Value)
//                            pres.secondName = GetMaskedData(Convert.ToString(dtconsumer.Rows[0]["SecondName"]));
//                        else
//                            pres.surname = GetMaskedData("Unknown");
//                        if (dtconsumer.Rows[0]["MaidenName"] != DBNull.Value)
//                        {
//                            pres.maidenName = GetMaskedData(dtconsumer.Rows[0]["MaidenName"].ToString());
//                        }
//                        else
//                        {
//                            pres.maidenName = GetMaskedData("Unknown");
//                        }
//                        if (dtconsumer.Rows[0]["Surname"] != DBNull.Value)
//                            pres.surname = GetMaskedData(Convert.ToString(dtconsumer.Rows[0]["Surname"]));
//                        else
//                            pres.surname = GetMaskedData("Unknown");
//                        if (dtconsumer.Rows[0]["ThirdName"] != DBNull.Value)
//                            pres.thirdName = GetMaskedData(Convert.ToString(dtconsumer.Rows[0]["ThirdName"]));
//                        else
//                            pres.thirdName = GetMaskedData("Unknown");
//                        if (dtconsumer.Rows[0]["BirthDate"] != DBNull.Value)
//                            pres.birthDate = DateTime.MinValue;
//                        if (dtconsumer.Rows[0]["GenderInd"] != DBNull.Value)
//                        {
//                            var data2 = lookupLst.Where(t => t.Type == "Gender Indicator" && t.Value == Convert.ToString(dtconsumer.Rows[0]["GenderInd"])).FirstOrDefault();
//                            if (data2 != null)
//                            {
//                                pres.genderInd = GetMaskedData(data2.Text);
//                            }
//                        }
//                        if (dtconsumer.Rows[0]["RecordStatusInd"] != DBNull.Value)
//                            pres.recordstatusind = Convert.ToString(dtconsumer.Rows[0]["RecordStatusInd"]);
//                        if (dtconsumer.Rows[0]["CreatedOnDate"] != DBNull.Value)
//                            pres.createdOnDate = DateTime.MinValue;
//                        if (dtconsumer.Rows[0]["LastUpdatedDate"] != DBNull.Value)
//                            pres.lastUpdatedDate = DateTime.MinValue;

//                    }
//                    else
//                    {
//                        pres.ErrorMessage = "No data available";
//                        return pres;
//                    }

//                    //ConsumerHomeAffairs
//                    DataTable dtHomeAffairs = dataSet.Tables[1];
//                    if (dtHomeAffairs.Rows.Count > 0)
//                    {

//                        if (dtHomeAffairs.Rows[0]["NameCombo"] != DBNull.Value)
//                            pres.nameCombo = GetMaskedData(Convert.ToString(dtHomeAffairs.Rows[0]["NameCombo"]));
//                        else
//                            pres.nameCombo = GetMaskedData("Unknown");
//                        if (dtHomeAffairs.Rows[0]["LastUpdatedDate"] != DBNull.Value)
//                            pres.lastupdatesHomeAffire = DateTime.MinValue;
//                        if (dtHomeAffairs.Rows[0]["IDIssuedDate"] != DBNull.Value)
//                            pres.iDIssuedDate = DateTime.MinValue;

//                        if (dtHomeAffairs.Rows[0]["MarriageDate"] != DBNull.Value)
//                        {
//                            pres.marriageDate = DateTime.MinValue;
//                            pres.maritalStatus = "MARRIED";
//                            if (dtHomeAffairs.Rows[0]["DivorceDate"] != DBNull.Value)
//                            {
//                                pres.divorceDate = DateTime.MinValue;
//                                if (pres.divorceDate > pres.marriageDate)
//                                { pres.maritalStatus = GetMaskedData("DIVORCED"); }
//                            }
//                        }
//                        if (dtHomeAffairs.Rows[0]["PlaceOfMarriage"] != DBNull.Value)
//                            pres.placeOfMarriage = GetMaskedData(Convert.ToString(dtHomeAffairs.Rows[0]["PlaceOfMarriage"]));
//                        if (pres.placeOfMarriage == null) { pres.placeOfMarriage = GetMaskedData("Unknown"); }
//                        if (dtHomeAffairs.Rows[0]["SpouseIdnoOrDOB"] != DBNull.Value)
//                            pres.spouseIdnoOrDOB = GetMaskedData(Convert.ToString(dtHomeAffairs.Rows[0]["SpouseIdnoOrDOB"]));
//                        if (dtHomeAffairs.Rows[0]["SpouseSurName"] != DBNull.Value)
//                            pres.spouseSurName = GetMaskedData(Convert.ToString(dtHomeAffairs.Rows[0]["SpouseSurName"]));
//                        else
//                            pres.spouseSurName = GetMaskedData("Unknown");
//                        if (dtHomeAffairs.Rows[0]["SpouseForeNames"] != DBNull.Value)
//                            pres.spouseForeNames = GetMaskedData(Convert.ToString(dtHomeAffairs.Rows[0]["SpouseForeNames"]));
//                        else
//                            pres.spouseForeNames = GetMaskedData("Unknown");
//                        if (dtHomeAffairs.Rows[0]["DivorceDate"] != DBNull.Value)
//                            pres.divorceDate = DateTime.MinValue;
//                        if (dtHomeAffairs.Rows[0]["DivorceIssuedCourt"] != DBNull.Value)
//                            pres.divorceIssuedCourt = GetMaskedData(Convert.ToString(dtHomeAffairs.Rows[0]["DivorceIssuedCourt"]));
//                        else
//                            pres.divorceIssuedCourt = GetMaskedData("Unknown");
//                        if (dtHomeAffairs.Rows[0]["CauseOfDeath"] != DBNull.Value)
//                            pres.causeOfDeath = GetMaskedData(Convert.ToString(dtHomeAffairs.Rows[0]["CauseOfDeath"]));
//                        if (pres.causeOfDeath == null)
//                        {
//                            pres.causeOfDeath = GetMaskedData("Unknown");
//                        }
//                        if (dtHomeAffairs.Rows[0]["DeceasedStatus"] != DBNull.Value)
//                            pres.deceasedStatus = GetMaskedData(Convert.ToString(dtHomeAffairs.Rows[0]["DeceasedStatus"]));
//                        if (pres.deceasedStatus == null)
//                        {
//                            pres.deceasedStatus = GetMaskedData("Unknown");
//                        }
//                        if (dtHomeAffairs.Rows[0]["DeceasedDate"] != DBNull.Value)
//                            pres.deceasedDate = DateTime.MinValue;
//                        if (dtHomeAffairs.Rows[0]["PlaceOfDeath"] != DBNull.Value)
//                            pres.placeOfDeath = GetMaskedData(Convert.ToString(dtHomeAffairs.Rows[0]["PlaceOfDeath"]));
//                        if (pres.placeOfDeath == null)
//                        {
//                            pres.placeOfDeath = GetMaskedData("Unknown");
//                        }

//                    }

//                    // Consumer Address
//                    DataTable dtAddress = dataSet.Tables[2];
//                    if (dtAddress.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtAddress.Rows)
//                        {
//                            var address = new AddressDetail();
//                            if (dr["AddressTypeInd"] != DBNull.Value)
//                            {
//                                var data5 = lookupLst.Where(t => t.Type == "Address Type Indicator" && t.Value == Convert.ToString(dr["AddressTypeInd"])).FirstOrDefault();
//                                if (data5 != null)
//                                {
//                                    address.addressTypeInd = data5.Text;

//                                }
//                            }
//                            if (dr["OriginalAddress1"] != DBNull.Value)
//                                address.originalAddress1 = GetMaskedData(Convert.ToString(dr["OriginalAddress1"]));
//                            if (dr["OriginalAddress2"] != DBNull.Value)
//                                address.originalAddress2 = GetMaskedData(Convert.ToString(dr["OriginalAddress2"]));
//                            if (dr["OriginalAddress3"] != DBNull.Value)
//                                address.originalAddress3 = GetMaskedData(Convert.ToString(dr["OriginalAddress3"]));
//                            if (dr["OriginalAddress4"] != DBNull.Value)
//                                address.originalAddress4 = GetMaskedData(Convert.ToString(dr["OriginalAddress4"]));
//                            if (dr["OriginalPostalCode"] != DBNull.Value)
//                                address.originalPostalCode = GetMaskedData(Convert.ToString(dr["OriginalPostalCode"]));
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                address.lastUpdatedDate = DateTime.MinValue;
//                            if (dr["CreatedOnDate"] != DBNull.Value)
//                                address.createdOnDate = DateTime.MinValue;
//                            if (dr["ConsumerAddressID"] != DBNull.Value)
//                                address.id = Convert.ToString(dr["ConsumerAddressID"]);
//                            if (dr["OccupantTypeInd"] != DBNull.Value)
//                            {
//                                var data1 = lookupLst.Where(t => t.Type == "Occupant Type Indicator" && t.Value == Convert.ToString(dr["OccupantTypeInd"])).FirstOrDefault();
//                                if (data1 != null)
//                                {
//                                    address.occupantTypeInd = data1.Text;

//                                }
//                            }
//                            address.fullAddress = "";
//                            if (!string.IsNullOrEmpty(address.originalAddress1))
//                                address.fullAddress += address.originalAddress1;

//                            if (!string.IsNullOrEmpty(address.originalAddress2))
//                            {
//                                if (!string.IsNullOrEmpty(address.fullAddress))
//                                    address.fullAddress += ", ";
//                                address.fullAddress += address.originalAddress2;
//                            }
//                            if (!string.IsNullOrEmpty(address.originalAddress3))
//                            {
//                                if (!string.IsNullOrEmpty(address.fullAddress))
//                                    address.fullAddress += ", ";
//                                address.fullAddress += address.originalAddress3;
//                            }

//                            if (!string.IsNullOrEmpty(address.originalAddress4))
//                            {
//                                if (!string.IsNullOrEmpty(address.fullAddress))
//                                    address.fullAddress += ", ";
//                                address.fullAddress += address.originalAddress4;
//                            }
//                            if (!string.IsNullOrEmpty(address.originalPostalCode))
//                            {
//                                if (!string.IsNullOrEmpty(address.fullAddress))
//                                    address.fullAddress += ", ";
//                                address.fullAddress += address.originalPostalCode;
//                            }
//                            if (dr["Province"] != DBNull.Value)
//                                address.province = GetMaskedData(Convert.ToString(dr["Province"]));
//                            if (dr["Town"] != DBNull.Value)
//                                address.town = GetMaskedData(Convert.ToString(dr["Town"]));
//                            if (dr["Region"] != DBNull.Value)
//                                address.region = GetMaskedData(Convert.ToString(dr["Region"]));

//                            if (address.lastUpdatedDate != null)
//                            {
//                                //timelines
//                                var addresspresent = timelines.FindAll(p => p.Type == address.addressTypeInd).ToList();

//                                if (addresspresent.Count == 0)
//                                {
//                                    TimeLine line = new TimeLine();
//                                    line.LastupdatedDate = address.lastUpdatedDate.Value;
//                                    line.Text = address.originalAddress1 + " " + address.originalAddress2 + " " + address.originalAddress3 + " " + address.originalAddress4;
//                                    line.TableName = "Address";
//                                    line.Type = address.addressTypeInd;
//                                    timelines.Add(line);
//                                }

//                            }
//                            var addrpresent = pres.addresses.Find(m => m.originalAddress1 == address.originalAddress1 && m.originalAddress2 == address.originalAddress2 && m.originalAddress3 == address.originalAddress3
//                            && m.originalAddress4 == address.originalAddress4 && m.addressTypeInd == address.addressTypeInd);
//                            if (addrpresent == null)
//                            {
//                                pres.addresses.Add(address);
//                            }

//                        }
//                    }

//                    //Consumer Employment
//                    DataTable dtEmployment = dataSet.Tables[3];

//                    if (dtEmployment.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtEmployment.Rows)
//                        {
//                            var employee = new Employeement();
//                            if (dr["Employer"] != DBNull.Value)
//                                employee.employer = GetMaskedData(Convert.ToString(dr["Employer"]));
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                employee.date = DateTime.MinValue;
//                            if (dr["CreatedOnDate"] != DBNull.Value)
//                                employee.CreatedDate = DateTime.MinValue;
//                            if (dr["Occupation"] != DBNull.Value)
//                                employee.occupation = GetMaskedData(Convert.ToString(dr["Occupation"]));
//                            if (dr["ConsumerID"] != DBNull.Value)
//                                employee.id = Convert.ToString(dr["ConsumerID"]);
//                            var empExists = pres.employees.Find(m => m.occupation == employee.occupation && m.employer == employee.employer);

//                            if (empExists == null)
//                            {
//                                pres.employees.Add(employee);
//                            }
//                        }
//                    }
//                    // Consumer Emails


//                    DataTable dtEmails = dataSet.Tables[5];

//                    if (dtEmails.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtEmails.Rows)
//                        {
//                            var contact = new ContactDetail();
//                            if (dr["ID"] != DBNull.Value)
//                                contact.id = Convert.ToInt64(dr["ID"]);
//                            if (dr["EmailID"] != DBNull.Value)
//                                contact.telephoneNo = GetMaskedData(Convert.ToString(dr["EmailID"]));
//                            if (dr["CreatedOnDate"] != DBNull.Value)
//                                contact.createdonDate = DateTime.MinValue;
//                            contact.type = "Email";
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                contact.lastUpdatedDate = DateTime.MinValue;
//                            contact.peopleLinked = 0;
//                            if (contact.lastUpdatedDate != null)
//                            {
//                                var Emailpresent = timelines.FindAll(p => p.Type == contact.type).ToList();
//                                if (Emailpresent.Count == 0 && (contact.createdonDate != null))
//                                {
//                                    TimeLine line = new TimeLine();
//                                    line.LastupdatedDate = contact.lastUpdatedDate.Value;
//                                    line.Text = contact.telephoneNo;
//                                    line.TableName = "Contacts";
//                                    line.Type = contact.type;
//                                    timelines.Add(line);
//                                }
//                            }
//                            var emailexists = pres.contacts.Find(m => m.telephoneNo == contact.telephoneNo);
//                            if (emailexists == null)
//                            {
//                                pres.contacts.Add(contact);
//                            }
//                        }
//                    }



//                    // Telephones
//                    List<string> telephoneinf = new List<string>();
//                    DataTable dtTelephone = dataSet.Tables[4];

//                    if (dtTelephone.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtTelephone.Rows)
//                        {
//                            var contact = new ContactDetail();
//                            if (dr["TelephoneTypeInd"] != DBNull.Value)
//                            {
//                                var data10 = lookupLst.Where(t => t.Type == "Telephone Type Indicator" && t.Value == Convert.ToString(dr["TelephoneTypeInd"])).FirstOrDefault();
//                                if (data10 != null)
//                                {
//                                    contact.type = data10.Text;
//                                }
//                            }
//                            if (dr["TelephoneID"] != DBNull.Value)
//                                contact.id = Convert.ToInt64(dr["TelephoneID"]);
//                            if (dr["InternationalDialingCode"] != DBNull.Value)
//                                contact.internationalDialingCode = GetMaskedData(Convert.ToString(dr["InternationalDialingCode"]));
//                            if (dr["TelephoneCode"] != DBNull.Value)
//                                contact.telephoneCode = GetMaskedData(Convert.ToString(dr["TelephoneCode"]));
//                            if (dr["TelephoneNo"] != DBNull.Value)
//                                contact.telephoneNo = GetMaskedData(Convert.ToString(dr["TelephoneNo"]));
//                            if (dr["CreatedonDate"] != DBNull.Value)
//                                contact.createdonDate = DateTime.MinValue;
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                contact.lastUpdatedDate = DateTime.MinValue;

//                            if (dr["CreatedonDate"] != DBNull.Value)
//                                contact.linkedDate = DateTime.MinValue;
//                            if (contact.internationalDialingCode == "27")
//                            {
//                                contact.telephoneNo = GetMaskedData("0" + contact.telephoneNo);
//                            }
//                            if (contact.lastUpdatedDate != null)
//                            {
//                                var contactpresent = timelines.FindAll(p => p.Type == contact.type).ToList();

//                                if (contactpresent.Count == 0)
//                                {
//                                    TimeLine line = new TimeLine();
//                                    line.LastupdatedDate = contact.lastUpdatedDate.Value;
//                                    line.Text = contact.telephoneNo;
//                                    line.TableName = "Contacts";
//                                    line.Type = contact.type;
//                                    timelines.Add(line);
//                                }
//                            }
//                            var telephoneexists = pres.contacts.Find(t => t.telephoneNo == contact.telephoneNo);
//                            if (telephoneexists == null)
//                            {

//                                if (contact.telephoneNo.Length == 10 || contact.internationalDialingCode != "27")
//                                {
//                                    if (contact.internationalDialingCode != "27")
//                                    {
//                                        contact.telephoneNo = contact.internationalDialingCode + contact.telephoneNo;
//                                    }
//                                    if (contact.createdonDate.Value != null)
//                                    {
//                                        contact.peopleLinked = 0;
//                                        pres.contacts.Add(contact);
//                                    }
//                                    if (!telephoneinf.Contains(contact.telephoneCode))
//                                    {
//                                        telephoneinf.Add(contact.telephoneCode);
//                                    }
//                                }
//                            }
//                        }
//                    }


//                    DataTable dtTelephoneCode = dataSet.Tables[9];

//                    if (dtTelephoneCode.Rows.Count > 0)
//                    {
//                        foreach (ContactDetail t in pres.contacts)
//                        {
//                            DataRow[] dr = dtTelephoneCode.Select("Code = '" + t.telephoneCode + "'");
//                            if (dr.Count() > 0)
//                            {
//                                if (dr[0]["Region"] != DBNull.Value)
//                                    t.CodeRegion = GetMaskedData(Convert.ToString(dr[0]["Region"]));
//                                if (dr[0]["Type"] != DBNull.Value)
//                                    t.CodeType = GetMaskedData(Convert.ToString(dr[0]["Type"]));
//                            }
//                            else
//                            {
//                                t.CodeRegion = GetMaskedData("UnKnown");
//                                t.CodeType = GetMaskedData("UnKnown");
//                            }
//                        }
//                    }



//                    //Directors
//                    DataTable dtDirectors = dataSet.Tables[10];
//                    DataTable dtCommercialDirectors = dataSet.Tables[11];
//                    DataTable dtCommercials = dataSet.Tables[12];

//                    if (dtCommercialDirectors.Rows.Count > 0)
//                        foreach (DataRow drCommercialDirector in dtCommercialDirectors.Rows)
//                        {
//                            var director = new DirectorShip();
//                            foreach (DataRow drCommercial in dtCommercials.Select("CommercialID = " + drCommercialDirector["CommercialID"].ToString()))
//                            {
//                                if (drCommercial["CommercialName"] != DBNull.Value)
//                                    director.companyname = GetMaskedData(Convert.ToString(drCommercial["CommercialName"]));
//                                if (drCommercial["CommercialID"] != DBNull.Value)
//                                    director.companyid = Convert.ToInt32(drCommercial["CommercialID"]);
//                                if (drCommercial["RegistrationNo"] != DBNull.Value)
//                                    director.companyno = GetMaskedData(Convert.ToString(drCommercial["RegistrationNo"]));
//                            }
//                            if (drCommercialDirector["DirectorID"] != DBNull.Value)
//                                director.id = Convert.ToString(drCommercialDirector["DirectorID"]);
//                            if (drCommercialDirector["AppointmentDate"] != DBNull.Value)
//                                director.appointmentDate = DateTime.MinValue;
//                            if (drCommercialDirector["CreatedOnDate"] != DBNull.Value)
//                                director.createdOnDate = DateTime.MinValue;
//                            if (drCommercialDirector["DirectorDesignationCode"] != DBNull.Value)
//                            {
//                                var data1 = lookupLst.Where(t => t.Type == "Director Designation Code" && t.Value == Convert.ToString(drCommercialDirector["DirectorDesignationCode"])).FirstOrDefault();
//                                if (data1 != null)
//                                {
//                                    director.directorDesignationCode = GetMaskedData(data1.Text);
//                                }
//                            }
//                            if (drCommercialDirector["DirectorSetDate"] != DBNull.Value)
//                                director.directorSetDate = DateTime.MinValue;
//                            if (drCommercialDirector["DirectorStatusDate"] != DBNull.Value)
//                                director.directorStatusDate = DateTime.MinValue;
//                            if (drCommercialDirector["DirectorStatusCode"] != DBNull.Value)
//                            {
//                                var data3 = lookupLst.Where(t => t.Type == "Director Status Code" && t.Value == Convert.ToString(drCommercialDirector["DirectorStatusCode"])).FirstOrDefault();
//                                if (data3 != null)
//                                {
//                                    director.directorStatusCode = GetMaskedData(data3.Text);
//                                }
//                            }
//                            if (drCommercialDirector["DirectorTypeCode"] != DBNull.Value)
//                            {
//                                var data5 = lookupLst.Where(t => t.Type == "Director Type Code" && t.Value == Convert.ToString(drCommercialDirector["DirectorTypeCode"])).FirstOrDefault();
//                                if (data5 != null)
//                                {
//                                    director.directorTypeCode = GetMaskedData(data5.Text);
//                                }
//                            }
//                            if (drCommercialDirector["Estate"] != DBNull.Value)
//                                director.estate = GetMaskedData(Convert.ToString(drCommercialDirector["Estate"]));
//                            if (drCommercialDirector["IsRSAResidentYN"] != DBNull.Value)
//                            {
//                                var data6 = lookupLst.Where(t => t.Type == "Is RSA Resident YN" && t.Value == Convert.ToString(drCommercialDirector["IsRSAResidentYN"])).FirstOrDefault();
//                                if (data6 != null)
//                                {
//                                    director.isRSAResidentYN = GetMaskedData(data6.Text);
//                                }
//                            }
//                            if (drCommercialDirector["MemberControlPerc"] != DBNull.Value)
//                                director.memberControlPerc = 0;
//                            if (drCommercialDirector["MemberSize"] != DBNull.Value)
//                                director.memberSize = 0;
//                            if (drCommercialDirector["Profession"] != DBNull.Value)
//                                director.profession = GetMaskedData(Convert.ToString(drCommercialDirector["Profession"]));
//                            if (drCommercialDirector["RegisterNo"] != DBNull.Value)
//                                director.registerNo = GetMaskedData(Convert.ToString(drCommercialDirector["RegisterNo"]));
//                            if (drCommercialDirector["TrusteeOf"] != DBNull.Value)
//                                director.trusteeOf = GetMaskedData(Convert.ToString(drCommercialDirector["TrusteeOf"]));

//                            DataRow[] drDirector = dtDirectors.Select("DirectorID = " + drCommercialDirector["DirectorID"].ToString());
//                            if (drDirector[0]["DirectorStatusDate"] != DBNull.Value)
//                                director.directorStatusDate = DateTime.MinValue;
//                            if (drDirector[0]["LastUpdatedDate"] != DBNull.Value)
//                                director.lastUpdatedDate = DateTime.MinValue;
//                            if (drDirector[0]["FirstInitial"] != DBNull.Value)
//                                director.firstInitial = GetMaskedData(drDirector[0]["FirstInitial"].ToString());
//                            if (drDirector[0]["SecondInitial"] != DBNull.Value)
//                                director.secondInitial = GetMaskedData(drDirector[0]["SecondInitial"].ToString());
//                            if (drDirector[0]["FirstName"] != DBNull.Value)
//                                director.fullName = GetMaskedData(drDirector[0]["FirstName"].ToString());
//                            if (drDirector[0]["SecondName"] != DBNull.Value)
//                                director.fullName = GetMaskedData(string.IsNullOrEmpty(director.fullName) ? drDirector[0]["SecondName"].ToString() : director.fullName + drDirector[0]["SecondName"].ToString());
//                            if (drDirector[0]["Surname"] != DBNull.Value)
//                                director.surname = GetMaskedData(drDirector[0]["Surname"].ToString());
//                            if (drDirector[0]["SurnamePrevious"] != DBNull.Value)
//                                director.surnamePrevious = GetMaskedData(drDirector[0]["SurnamePrevious"].ToString());
//                            if (drDirector[0]["BirthDate"] != DBNull.Value)
//                                director.birthDate = DateTime.MinValue;
//                            if (drDirector[0]["IDNo"] != DBNull.Value)
//                                director.idNumber = GetMaskedData(drDirector[0]["IDNo"].ToString());


//                            var direxists = pres.directorShips.Find(t => t.appointmentDate == director.appointmentDate && t.registerNo == director.registerNo);
//                            if (direxists == null)
//                            {
//                                pres.directorShips.Add(director);
//                            }

//                        }

//                    //Property deeds Buyer
//                    DataTable dtBuyer = dataSet.Tables[13];
//                    DataTable dtdeedBuyer = dataSet.Tables[14];

//                    if (dtBuyer.Rows.Count > 0)
//                        foreach (DataRow dr in dtBuyer.Rows)
//                        {
//                            var property1 = new PropertyDeedDetail();
//                            foreach (DataRow drdeed in dtdeedBuyer.Select("PropertyDeedId = " + dr["PropertyDeedId"].ToString()))
//                            {
//                                if (drdeed["DeedsOfficeId"] != DBNull.Value)
//                                {
//                                    var data11 = lookupLst.Where(t => t.Type == "Deeds Office Identifier" && t.Value == drdeed["DeedsOfficeId"].ToString()).FirstOrDefault();
//                                    if (data11 != null)
//                                    {
//                                        property1.deedsOfficeId = GetMaskedData(data11.Text);
//                                    }
//                                }
//                                if (drdeed["TitleDeedNo"] != DBNull.Value)
//                                    property1.titleDeedNo = GetMaskedData(drdeed["TitleDeedNo"].ToString());
//                                if (drdeed["TitleDeedNoOLD"] != DBNull.Value)
//                                    property1.titleDeedNoOLD = GetMaskedData(drdeed["TitleDeedNoOLD"].ToString());
//                                if (drdeed["TitleDeedFee"] != DBNull.Value)
//                                    property1.titleDeedFee = 00;
//                                if (drdeed["DatePurchase"] != DBNull.Value)
//                                    property1.datePurchase = DateTime.MinValue;
//                                if (drdeed["DateRegister"] != DBNull.Value)
//                                    property1.dateRegister = DateTime.MinValue;
//                                if (drdeed["PurchaseAmount"] != DBNull.Value)
//                                    property1.purchaseAmount = 000;
//                                if (drdeed["StreetAddress"] != DBNull.Value)
//                                    property1.streetAddress = GetMaskedData(drdeed["StreetAddress"].ToString());
//                                if (drdeed["StreetNumber"] != DBNull.Value)
//                                    property1.streetNumber = GetMaskedData(drdeed["StreetNumber"].ToString());
//                                if (drdeed["StreetName"] != DBNull.Value)
//                                    property1.streetName = GetMaskedData(drdeed["StreetName"].ToString());
//                                if (drdeed["StreetType"] != DBNull.Value)
//                                    property1.streetType = GetMaskedData(drdeed["StreetType"].ToString());
//                                if (drdeed["X"] != DBNull.Value)
//                                    property1.x = 00;
//                                if (drdeed["Y"] != DBNull.Value)
//                                    property1.y = 00;
//                                if (drdeed["SuburbCode"] != DBNull.Value)
//                                    property1.suburbCode = GetMaskedData(drdeed["SuburbCode"].ToString());
//                                if (drdeed["SuburbDeeds"] != DBNull.Value)
//                                    property1.suburbDeeds = GetMaskedData(drdeed["SuburbDeeds"].ToString());
//                                if (drdeed["Town"] != DBNull.Value)
//                                    property1.town = GetMaskedData(drdeed["Town"].ToString());
//                                if (drdeed["Authority"] != DBNull.Value)
//                                    property1.authority = GetMaskedData(drdeed["Authority"].ToString());
//                                if (drdeed["MunicipalityName"] != DBNull.Value)
//                                    property1.municipalityName = drdeed["MunicipalityName"].ToString();
//                                if (drdeed["ProvinceId"] != DBNull.Value)
//                                {

//                                    var data = lookupLst.Where(t => t.Type == "Province Identifier" && t.Value == drdeed["ProvinceId"].ToString()).FirstOrDefault();
//                                    if (data != null)
//                                    {
//                                        property1.provinceId = GetMaskedData(data.Text);

//                                    }
//                                }
//                                if (drdeed["IsCurrOwnerUpdated"] != DBNull.Value)
//                                {
//                                    var data7 = lookupLst.Where(t => t.Type == "Is Current Owner Updated" && t.Value == drdeed["IsCurrOwnerUpdated"].ToString()).FirstOrDefault();
//                                    if (data7 != null)
//                                    {
//                                        property1.isCurrOwnerUpdated = GetMaskedData(data7.Text);
//                                    }
//                                }
//                                if (drdeed["Extent"] != DBNull.Value)
//                                    property1.extent = GetMaskedData(drdeed["Extent"].ToString());
//                                if (drdeed["AttorneyFirmNumber"] != DBNull.Value)
//                                    property1.attorneyFirmNumber = GetMaskedData(drdeed["AttorneyFirmNumber"].ToString());
//                                if (drdeed["AttorneyFileNumber"] != DBNull.Value)
//                                    property1.attorneyFileNumber = GetMaskedData(drdeed["AttorneyFileNumber"].ToString());
//                                if (drdeed["TransferSeqNo"] != DBNull.Value)
//                                    property1.transferSeqNo = 00;
//                                if (drdeed["DateCaptured"] != DBNull.Value)
//                                    property1.dateCaptured = DateTime.MinValue;
//                                if (drdeed["BondNumber"] != DBNull.Value)
//                                    property1.bondNumber = GetMaskedData(drdeed["BondNumber"].ToString());
//                                if (drdeed["BondHolder"] != DBNull.Value)
//                                    property1.bondHolder = GetMaskedData(drdeed["BondHolder"].ToString());
//                                if (drdeed["BondAmount"] != DBNull.Value)
//                                    property1.bondAmount = Convert.ToInt64(drdeed["BondAmount"]);
//                                if (drdeed["PropertyType"] != DBNull.Value)
//                                {
//                                    var data5 = lookupLst.Where(t => t.Type == "Property Type" && t.Value == drdeed["PropertyType"].ToString()).FirstOrDefault();
//                                    if (data5 != null)
//                                    {
//                                        property1.propertyType = GetMaskedData(data5.Text);

//                                    }
//                                }
//                                if (drdeed["PropertyName"] != DBNull.Value)
//                                    property1.propertyName = GetMaskedData(drdeed["PropertyName"].ToString());
//                                //todo

//                                if (drdeed["SchemeId"] != DBNull.Value)
//                                    property1.schemeId = GetMaskedData(drdeed["SchemeId"].ToString());
//                                if (drdeed["SuburbId"] != DBNull.Value)
//                                    property1.suburbId = 0;
//                                if (drdeed["Erf"] != DBNull.Value)
//                                    property1.erf = GetMaskedData(drdeed["Erf"].ToString());
//                                if (drdeed["Portion"] != DBNull.Value)
//                                    property1.portion = 0;
//                                if (drdeed["Unit"] != DBNull.Value)
//                                    property1.unit = 0;
//                                if (drdeed["CreatedOndate"] != DBNull.Value)
//                                    property1.createdOndate = DateTime.MinValue;
//                                if (drdeed["ErfSize"] != DBNull.Value)
//                                    property1.erfSize = GetMaskedData(drdeed["ErfSize"].ToString());
//                                if (drdeed["StandNo"] != DBNull.Value)
//                                    property1.standNo = GetMaskedData(drdeed["StandNo"].ToString());
//                                if (drdeed["PortionNo"] != DBNull.Value)
//                                    property1.portionNo = GetMaskedData(drdeed["PortionNo"].ToString());
//                                if (drdeed["TownShipNo"] != DBNull.Value)
//                                    property1.townShipNo = 0;
//                                if (drdeed["PrevExtent"] != DBNull.Value)
//                                    property1.prevExtent = GetMaskedData(drdeed["PrevExtent"].ToString());
//                                if (drdeed["IsCurrentOwner"] != DBNull.Value)
//                                    property1.isCurrentOwner = false;
//                                if (drdeed["PropertyDeedID"] != DBNull.Value)
//                                    property1.propertyDeedId = Convert.ToInt32(drdeed["PropertyDeedID"]);
//                                if (drdeed["StreetAddress"] != DBNull.Value)
//                                    property1.address = GetMaskedData(drdeed["StreetAddress"].ToString());

//                            }


//                            if (dr["BuyerID"] != DBNull.Value)
//                                property1.buyerid = dr["BuyerID"].ToString();
//                            if (dr["BuyerIDNO"] != DBNull.Value)
//                                property1.buyerIDNO = GetMaskedData(dr["BuyerIDNO"].ToString());
//                            if (dr["BuyerName"] != DBNull.Value)
//                                property1.buyerName = GetMaskedData(dr["BuyerName"].ToString());
//                            if (dr["BuyerType"] != DBNull.Value)
//                            {

//                                var data1 = lookupLst.Where(t => t.Type == "Buyer Type" && t.Value == dr["BuyerType"].ToString()).FirstOrDefault();
//                                if (data1 != null)
//                                {
//                                    property1.buyerType = GetMaskedData(data1.Text);

//                                }
//                            }
//                            property1.fullAddress = "";
//                            if (!string.IsNullOrEmpty(property1.streetNumber))
//                                property1.fullAddress += property1.streetNumber;

//                            if (!string.IsNullOrEmpty(property1.streetName))
//                            {
//                                if (!string.IsNullOrEmpty(property1.fullAddress))
//                                    property1.fullAddress += ", ";
//                                property1.fullAddress += property1.streetName;
//                            }
//                            if (!string.IsNullOrEmpty(property1.streetType))
//                            {
//                                if (!string.IsNullOrEmpty(property1.fullAddress))
//                                    property1.fullAddress += ", ";
//                                property1.fullAddress += property1.streetType;
//                            }

//                            if (!string.IsNullOrEmpty(property1.town))
//                            {
//                                if (!string.IsNullOrEmpty(property1.fullAddress))
//                                    property1.fullAddress += ", ";
//                                property1.fullAddress += property1.town;
//                            }
//                            if (dr["BuyerStatus"] != DBNull.Value)
//                                property1.buyerStatus = GetMaskedData(dr["BuyerStatus"].ToString());
//                            if (dr["Share"] != DBNull.Value)
//                                property1.share = GetMaskedData(dr["Share"].ToString());

//                            property1.type = GetMaskedData("Buyer");

//                            var propertybuyerexists = pres.propertyOwners.Find(t => t.type == property1.type && t.datePurchase == property1.datePurchase && t.fullAddress == property1.fullAddress && t.isCurrentOwner == property1.isCurrentOwner);
//                            if (propertybuyerexists == null)
//                            {
//                                pres.propertyOwners.Add(property1);
//                            }
//                        }

//                    //Property deeds Sellar
//                    DataTable dtSellar = dataSet.Tables[15];
//                    DataTable dtdeedSeller = dataSet.Tables[16];

//                    if (dtSellar.Rows.Count > 0)
//                        foreach (DataRow dr1 in dtSellar.Rows)
//                        {
//                            var property1 = new PropertyDeedDetail();
//                            foreach (DataRow drdeed1 in dtdeedSeller.Select("PropertyDeedId = " + dr1["PropertyDeedId"].ToString()))
//                            {
//                                if (drdeed1["DeedsOfficeId"] != DBNull.Value)
//                                {
//                                    var data11 = lookupLst.Where(t => t.Type == "Deeds Office Identifier" && t.Value == drdeed1["DeedsOfficeId"].ToString()).FirstOrDefault();
//                                    if (data11 != null)
//                                    {
//                                        property1.deedsOfficeId = GetMaskedData(data11.Text);
//                                    }
//                                }
//                                if (drdeed1["TitleDeedNo"] != DBNull.Value)
//                                    property1.titleDeedNo = GetMaskedData(drdeed1["TitleDeedNo"].ToString());
//                                if (drdeed1["TitleDeedNoOLD"] != DBNull.Value)
//                                    property1.titleDeedNoOLD = GetMaskedData(drdeed1["TitleDeedNoOLD"].ToString());
//                                if (drdeed1["TitleDeedFee"] != DBNull.Value)
//                                    property1.titleDeedFee = 0;
//                                if (drdeed1["DatePurchase"] != DBNull.Value)
//                                    property1.datePurchase = DateTime.MinValue;
//                                if (drdeed1["DateRegister"] != DBNull.Value)
//                                    property1.dateRegister = DateTime.MinValue;
//                                if (drdeed1["PurchaseAmount"] != DBNull.Value)
//                                    property1.purchaseAmount = 0000;
//                                if (drdeed1["StreetAddress"] != DBNull.Value)
//                                    property1.streetAddress = GetMaskedData(drdeed1["StreetAddress"].ToString());
//                                if (drdeed1["StreetNumber"] != DBNull.Value)
//                                    property1.streetNumber = GetMaskedData(drdeed1["StreetNumber"].ToString());
//                                if (drdeed1["StreetName"] != DBNull.Value)
//                                    property1.streetName = GetMaskedData(drdeed1["StreetName"].ToString());
//                                if (drdeed1["StreetType"] != DBNull.Value)
//                                    property1.streetType = GetMaskedData(drdeed1["StreetType"].ToString());
//                                if (drdeed1["X"] != DBNull.Value)
//                                    property1.x = 0;
//                                if (drdeed1["Y"] != DBNull.Value)
//                                    property1.y = 0;
//                                if (drdeed1["SuburbCode"] != DBNull.Value)
//                                    property1.suburbCode = GetMaskedData(drdeed1["SuburbCode"].ToString());
//                                if (drdeed1["SuburbDeeds"] != DBNull.Value)
//                                    property1.suburbDeeds = GetMaskedData(drdeed1["SuburbDeeds"].ToString());
//                                if (drdeed1["Town"] != DBNull.Value)
//                                    property1.town = GetMaskedData(drdeed1["Town"].ToString());
//                                if (drdeed1["Authority"] != DBNull.Value)
//                                    property1.authority = GetMaskedData(drdeed1["Authority"].ToString());
//                                if (drdeed1["MunicipalityName"] != DBNull.Value)
//                                    property1.municipalityName = GetMaskedData(drdeed1["MunicipalityName"].ToString());
//                                if (drdeed1["ProvinceId"] != DBNull.Value)
//                                {

//                                    var data = lookupLst.Where(t => t.Type == "Province Identifier" && t.Value == drdeed1["ProvinceId"].ToString()).FirstOrDefault();
//                                    if (data != null)
//                                    {
//                                        property1.provinceId = GetMaskedData(data.Text);

//                                    }
//                                }
//                                if (drdeed1["IsCurrOwnerUpdated"] != DBNull.Value)
//                                {
//                                    var data7 = lookupLst.Where(t => t.Type == "Is Current Owner Updated" && t.Value == drdeed1["IsCurrOwnerUpdated"].ToString()).FirstOrDefault();
//                                    if (data7 != null)
//                                    {
//                                        property1.isCurrOwnerUpdated = GetMaskedData(data7.Text);
//                                    }
//                                }
//                                if (drdeed1["Extent"] != DBNull.Value)
//                                    property1.extent = GetMaskedData(drdeed1["Extent"].ToString());
//                                if (drdeed1["AttorneyFirmNumber"] != DBNull.Value)
//                                    property1.attorneyFirmNumber = GetMaskedData(drdeed1["AttorneyFirmNumber"].ToString());
//                                if (drdeed1["AttorneyFileNumber"] != DBNull.Value)
//                                    property1.attorneyFileNumber = GetMaskedData(drdeed1["AttorneyFileNumber"].ToString());
//                                if (drdeed1["TransferSeqNo"] != DBNull.Value)
//                                    property1.transferSeqNo = 00;
//                                if (drdeed1["DateCaptured"] != DBNull.Value)
//                                    property1.dateCaptured = DateTime.MinValue;
//                                if (drdeed1["BondNumber"] != DBNull.Value)
//                                    property1.bondNumber = GetMaskedData(drdeed1["BondNumber"].ToString());
//                                if (drdeed1["BondHolder"] != DBNull.Value)
//                                    property1.bondHolder = GetMaskedData(drdeed1["BondHolder"].ToString());
//                                if (drdeed1["BondAmount"] != DBNull.Value)
//                                    property1.bondAmount = 00;
//                                if (drdeed1["PropertyType"] != DBNull.Value)
//                                {
//                                    var data5 = lookupLst.Where(t => t.Type == "Property Type" && t.Value == drdeed1["PropertyType"].ToString()).FirstOrDefault();
//                                    if (data5 != null)
//                                    {
//                                        property1.propertyType = GetMaskedData(data5.Text);

//                                    }
//                                }
//                                if (drdeed1["PropertyName"] != DBNull.Value)
//                                    property1.propertyName = GetMaskedData(drdeed1["PropertyName"].ToString());
//                                if (drdeed1["SchemeId"] != DBNull.Value)
//                                    property1.schemeId = GetMaskedData(drdeed1["SchemeId"].ToString());
//                                if (drdeed1["SuburbId"] != DBNull.Value)
//                                    property1.suburbId = 0;
//                                if (drdeed1["Erf"] != DBNull.Value)
//                                    property1.erf = GetMaskedData(drdeed1["Erf"].ToString());
//                                if (drdeed1["Portion"] != DBNull.Value)
//                                    property1.portion = 0;
//                                if (drdeed1["Unit"] != DBNull.Value)
//                                    property1.unit = 0;
//                                if (drdeed1["CreatedOndate"] != DBNull.Value)
//                                    property1.createdOndate = DateTime.MinValue;
//                                if (drdeed1["ErfSize"] != DBNull.Value)
//                                    property1.erfSize = GetMaskedData(drdeed1["ErfSize"].ToString());
//                                if (drdeed1["StandNo"] != DBNull.Value)
//                                    property1.standNo = GetMaskedData(drdeed1["StandNo"].ToString());
//                                if (drdeed1["PortionNo"] != DBNull.Value)
//                                    property1.portionNo = GetMaskedData(drdeed1["PortionNo"].ToString());
//                                if (drdeed1["TownShipNo"] != DBNull.Value)
//                                    property1.townShipNo = 0;
//                                if (drdeed1["PrevExtent"] != DBNull.Value)
//                                    property1.prevExtent = GetMaskedData(drdeed1["PrevExtent"].ToString());
//                                if (drdeed1["IsCurrentOwner"] != DBNull.Value)
//                                    property1.isCurrentOwner = false;
//                                if (drdeed1["PropertyDeedID"] != DBNull.Value)
//                                    property1.propertyDeedId = Convert.ToInt32(drdeed1["PropertyDeedID"]);
//                                if (drdeed1["StreetAddress"] != DBNull.Value)
//                                    property1.address = GetMaskedData(drdeed1["StreetAddress"].ToString());




//                                if (dr1["SellerID"] != DBNull.Value)
//                                    property1.sellarid = dr1["SellerID"].ToString();
//                                if (dr1["SellerIDNO"] != DBNull.Value)
//                                    property1.sellerIDNO = GetMaskedData(dr1["SellerIDNO"].ToString());
//                                if (dr1["SellerName"] != DBNull.Value)
//                                    property1.sellerName = GetMaskedData(dr1["SellerName"].ToString());
//                                if (dr1["SellerType"] != DBNull.Value)
//                                {

//                                    var data1 = lookupLst.Where(t => t.Type == "Buyer Type" && t.Value == dr1["SellerType"].ToString()).FirstOrDefault();
//                                    if (data1 != null)
//                                    {
//                                        property1.sellerType = GetMaskedData(data1.Text);

//                                    }
//                                }
//                                property1.fullAddress = "";
//                                if (!string.IsNullOrEmpty(property1.streetNumber))
//                                    property1.fullAddress += property1.streetNumber;

//                                if (!string.IsNullOrEmpty(property1.streetName))
//                                {
//                                    if (!string.IsNullOrEmpty(property1.fullAddress))
//                                        property1.fullAddress += ", ";
//                                    property1.fullAddress += property1.streetName;
//                                }
//                                if (!string.IsNullOrEmpty(property1.streetType))
//                                {
//                                    if (!string.IsNullOrEmpty(property1.fullAddress))
//                                        property1.fullAddress += ", ";
//                                    property1.fullAddress += property1.streetType;
//                                }

//                                if (!string.IsNullOrEmpty(property1.town))
//                                {
//                                    if (!string.IsNullOrEmpty(property1.fullAddress))
//                                        property1.fullAddress += ", ";
//                                    property1.fullAddress += property1.town;
//                                }

//                                property1.sellerStatus = dr1["SellerStatus"].ToString();
//                                property1.type = "Seller";


//                                var propertybuyerexists = pres.propertyOwners.Find(t => t.type == property1.type && t.datePurchase == property1.datePurchase && t.fullAddress == property1.fullAddress && t.isCurrentOwner == property1.isCurrentOwner);
//                                if (propertybuyerexists == null)
//                                {
//                                    pres.propertyOwners.Add(property1);
//                                }
//                            }
//                        }

//                    // Judgements

//                    DataTable dtJudgement = dataSet.Tables[6];

//                    if (dtJudgement.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtJudgement.Rows)
//                        {
//                            var judgement = new Consumerjudgement();
//                            if (dr["ConsumerJudgementID"] != DBNull.Value)
//                                judgement.id = Convert.ToString(dr["ConsumerJudgementID"]);
//                            if (dr["IDNo"] != DBNull.Value)
//                                judgement.idno = GetMaskedData(Convert.ToString(dr["IDNo"]));
//                            if (dr["CaseNumber"] != DBNull.Value)
//                                judgement.casenumber = GetMaskedData(Convert.ToString(dr["CaseNumber"]));
//                            if (dr["CaseFilingDate"] != DBNull.Value)
//                                judgement.casefilingdate = DateTime.MinValue;
//                            if (dr["CaseReason"] != DBNull.Value)
//                                judgement.casereason = GetMaskedData(Convert.ToString(dr["CaseReason"]));
//                            if (dr["CaseType"] != DBNull.Value)
//                                judgement.casetype = GetMaskedData(Convert.ToString(dr["CaseType"]));
//                            if (dr["DisputeAmt"] != DBNull.Value)
//                                judgement.disputeamt = 00000;
//                            if (dr["CourtName"] != DBNull.Value)
//                                judgement.courtname = GetMaskedData(Convert.ToString(dr["CourtName"]));
//                            if (dr["CourtCity"] != DBNull.Value)
//                                judgement.courtcity = GetMaskedData(Convert.ToString(dr["CourtCity"]));
//                            if (dr["CourtType"] != DBNull.Value)
//                                judgement.courttype = GetMaskedData(Convert.ToString(dr["CourtType"]));
//                            if (dr["PlaintiffName"] != DBNull.Value)
//                                judgement.plaintiffname = GetMaskedData(Convert.ToString(dr["PlaintiffName"]));
//                            if (dr["PlaintiffAddress1"] != DBNull.Value)
//                                judgement.plaintiffaddress1 = GetMaskedData(Convert.ToString(dr["PlaintiffAddress1"]));
//                            if (dr["PlaintiffAddress2"] != DBNull.Value)
//                                judgement.plaintiffaddress2 = GetMaskedData(Convert.ToString(dr["PlaintiffAddress2"]));
//                            if (dr["PlaintiffAddress3"] != DBNull.Value)
//                                judgement.plaintiffaddress3 = GetMaskedData(Convert.ToString(dr["PlaintiffAddress3"]));
//                            if (dr["PlaintiffAddress4"] != DBNull.Value)
//                                judgement.plaintiffaddress4 = GetMaskedData(Convert.ToString(dr["PlaintiffAddress4"]));
//                            if (dr["PlaintiffPostalCode"] != DBNull.Value)
//                                judgement.plaintifpostalcode = GetMaskedData(Convert.ToString(dr["PlaintiffPostalCode"]));
//                            if (dr["AttorneyName"] != DBNull.Value)
//                                judgement.attorneyname = GetMaskedData(Convert.ToString(dr["AttorneyName"]));
//                            if (dr["AttorneyTelephoneCode"] != DBNull.Value)
//                                judgement.attorneytelephonecode = GetMaskedData(Convert.ToString(dr["AttorneyTelephoneCode"]));
//                            if (dr["AttorneyTelephoneNo"] != DBNull.Value)
//                                judgement.attorneytelephoneno = GetMaskedData(Convert.ToString(dr["AttorneyTelephoneNo"]));
//                            if (dr["AttorneyFaxCode"] != DBNull.Value)
//                                judgement.attorneyfaxcode = GetMaskedData(Convert.ToString(dr["AttorneyFaxCode"]));
//                            if (dr["AttorneyFaxNo"] != DBNull.Value)
//                                judgement.attorneyfaxno = GetMaskedData(Convert.ToString(dr["AttorneyFaxNo"]));
//                            if (dr["AttorneyAddress1"] != DBNull.Value)
//                                judgement.attorneyaddress1 = GetMaskedData(Convert.ToString(dr["AttorneyAddress1"]));
//                            if (dr["AttorneyAddress2"] != DBNull.Value)
//                                judgement.attorneyaddress2 = GetMaskedData(Convert.ToString(dr["AttorneyAddress2"]));
//                            if (dr["AttorneyAddress3"] != DBNull.Value)
//                                judgement.attorneyaddress3 = GetMaskedData(Convert.ToString(dr["AttorneyAddress3"]));
//                            if (dr["AttorneyAddress4"] != DBNull.Value)
//                                judgement.attorneyaddress4 = GetMaskedData(Convert.ToString(dr["AttorneyAddress4"]));
//                            if (dr["AttorneyPostalCode"] != DBNull.Value)
//                                judgement.attorneypostalcode = GetMaskedData(Convert.ToString(dr["AttorneyPostalCode"]));
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                judgement.lastupdateddate = DateTime.MinValue;
//                            if (dr["CreatedOnDate"] != DBNull.Value)
//                                judgement.createdodate = DateTime.MinValue;
//                            if (dr["JudgementTypeCode"] != DBNull.Value)
//                            {

//                                var data11 = lookupLst.Where(t => t.Type == "Judgement Type Code" && t.Value == Convert.ToString(dr["JudgementTypeCode"])).FirstOrDefault();
//                                if (data11 != null)
//                                {
//                                    judgement.judgementtypecode = GetMaskedData(data11.Text);
//                                }
//                            }
//                            if (dr["DisputeDate"] != DBNull.Value)
//                                judgement.disputedate = DateTime.MinValue;
//                            if (dr["DisputeResolvedDate"] != DBNull.Value)
//                                judgement.disputeresolveddate = DateTime.MinValue;
//                            if (dr["Rescinded"] != DBNull.Value)
//                                judgement.rescinded = false;// Need Clarity
//                            if (dr["RescissionDate"] != DBNull.Value)
//                                judgement.rescissiondate = DateTime.MinValue;
//                            if (dr["RescissionReason"] != DBNull.Value)
//                                judgement.rescissionreason = GetMaskedData(Convert.ToString(dr["RescissionReason"]));
//                            if (dr["RescindedAmount"] != DBNull.Value)
//                                judgement.rescindedamount = "000000";
//                            if (dr["JudgementTypeCode"] != DBNull.Value)
//                                judgement.judgementtypecode = GetMaskedData(Convert.ToString(dr["JudgementTypeCode"]).ToUpper());
//                            if (dr["CaseFilingDate"] != DBNull.Value)
//                                judgement.casefilingdate = DateTime.MinValue;

//                            pres.consumerjudgements.Add(judgement);

//                        }
//                    }




//                    // Debit Reviews

//                    DataTable dtDebt = dataSet.Tables[7];

//                    if (dtDebt.Rows.Count > 0)
//                    {
//                        foreach (DataRow dr in dtDebt.Rows)
//                        {
//                            var consdeb = new consumerdebtreview();
//                            if (dr["DebtCounsellorRegistrationNo"] != DBNull.Value)
//                                consdeb.debtCounsellorRegistrationNo = GetMaskedData(Convert.ToString(dr["DebtCounsellorRegistrationNo"]));
//                            if (dr["DebtCounsellorFirstName"] != DBNull.Value)
//                                consdeb.debtCounsellorFirstName = GetMaskedData(Convert.ToString(dr["DebtCounsellorFirstName"]));
//                            if (dr["DebtCounsellorSurname"] != DBNull.Value)
//                                consdeb.debtCounsellorSurname = GetMaskedData(Convert.ToString(dr["DebtCounsellorSurname"]));
//                            if (dr["DebtReviewStatusDate"] != DBNull.Value)
//                                consdeb.applicationCreationDate = DateTime.MinValue;
//                            if (dr["DebtReviewStatusDate"] != DBNull.Value)
//                                consdeb.debtReviewStatusDate = DateTime.MinValue;
//                            if (dr["ConsumerDebtReviewID"] != DBNull.Value)
//                                consdeb.consumerDebtReviewID = Convert.ToInt64(dr["ConsumerDebtReviewID"]);
//                            if (dr["ConsumerID"] != DBNull.Value)
//                                consdeb.consumerID = Convert.ToInt64(dr["ConsumerID"]);
//                            if (dr["DebtCounsellorTelephoneCode"] != DBNull.Value)
//                                consdeb.debtCounsellorTelephoneCode = GetMaskedData(Convert.ToString(dr["DebtCounsellorTelephoneCode"]));
//                            if (dr["DebtCounsellorTelephoneNo"] != DBNull.Value)
//                                consdeb.debtCounsellorTelephoneNo = GetMaskedData(Convert.ToString(dr["DebtCounsellorTelephoneNo"]));
//                            consdeb.telephoneCodeandNumber = string.Empty;

//                            if (!string.IsNullOrEmpty(consdeb.debtCounsellorTelephoneCode))
//                                consdeb.telephoneCodeandNumber += consdeb.debtCounsellorTelephoneCode;

//                            if (!string.IsNullOrEmpty(consdeb.debtCounsellorTelephoneNo))
//                            {
//                                if (!string.IsNullOrEmpty(consdeb.telephoneCodeandNumber))
//                                    consdeb.telephoneCodeandNumber += ", ";
//                                consdeb.telephoneCodeandNumber += consdeb.debtCounsellorTelephoneNo;
//                            }
//                            if (dr["LastUpdatedDate"] != DBNull.Value)
//                                consdeb.lastUpdatedDate = DateTime.MinValue;
//                            if (dr["DebtReviewStatusCode"] != DBNull.Value)
//                            {
//                                var data1 = lookupLst.Where(t => t.Type == "Debt Review Status Code" && t.Value == Convert.ToString(dr["DebtReviewStatusCode"])).FirstOrDefault();//todo
//                                if (data1 != null)
//                                {
//                                    consdeb.debtReviewStatusCode = GetMaskedData(data1.Text);

//                                }
//                            }


//                            pres.consumerDebtReview.Add(consdeb);

//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var msg = ex;
//                string commrequest = JsonConvert.SerializeObject(msg);
//                WriteLog(commrequest, "Consumer detail");
//            }
//            return pres;
//        }



//        public string GetMaskedData(string str)
//        {
//            if (str != null)
//            {
//                if (str.Length > 2)
//                {
//                    return str[0] + new string('X', str.Length - 2) + str.Substring(str.Length - 1);
//                }
//                else
//                    return str;
//            }
//            return string.Empty;
//        }

//        public void WriteLog(string strLog, string errorName)
//        {
//            try
//            {
//                string folderName = "ErrorLogs";
//                string webRootPath = _hostingEnvironment.WebRootPath;
//                string newPath = Path.Combine(webRootPath, folderName);
//                string fileName = errorName + ".txt";
//                string logFilePath = Path.Combine(newPath, fileName);

//                if (!Directory.Exists(newPath))
//                    Directory.CreateDirectory(newPath);

//                using (StreamWriter OurStream = File.CreateText(logFilePath))
//                {
//                    OurStream.WriteLine(strLog);
//                    OurStream.Close();
//                }
//            }
//            catch (Exception ex) { }
//        }

//        public long GetConsumerId(string IdNumber)
//        {
//            long id = _productionContext.Consumers.Where(t => t.IDNO == IdNumber).Select(t => t.ConsumerID).FirstOrDefault();
//            return id;
//        }

//        public DirectorShip GetDirectorDetail(int DirectorId)
//        {
//            DirectorShip director = new DirectorShip();
//            try
//            {
//                SqlConnection con = (SqlConnection)_productionContext.Database.GetDbConnection();
//                using (con)
//                {
//                    if (con.State != ConnectionState.Open)
//                        con.Open();
//                    DataSet data = new DataSet();
//                    SqlCommand cmd = new SqlCommand("qspDirectordetail", con);
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.Add(new SqlParameter("@CommercialDirId", DirectorId));
//                    cmd.CommandTimeout = 0;
//                    List<LookupData> lookuplst = _IDAScontext.LookupDatas.ToList();

//                    SqlDataAdapter sDA = new SqlDataAdapter();
//                    sDA.SelectCommand = cmd;

//                    if (con.State != ConnectionState.Open)
//                        con.Open();

//                    sDA.Fill(data);

//                    DataTable dtdirAddress = data.Tables[0];
//                    if (dtdirAddress.Rows.Count > 0)
//                    {
//                        foreach (DataRow diraddres in dtdirAddress.Rows)
//                        {
//                            var address = new DirectorAddressVm();
//                            if (diraddres["addressTypeInd"] != DBNull.Value)
//                            {
//                                var data19 = lookuplst.FirstOrDefault(t => t.Type == "Address Type Indicator" && t.Value == diraddres["addressTypeInd"].ToString());
//                                if (data19 != null)
//                                {
//                                    address.addressTypeInd = data19.Text;

//                                }
//                            }
//                            if (diraddres["createdOnDate"] != DBNull.Value)
//                                address.createdOnDate = Convert.ToDateTime(diraddres["createdOnDate"]);
//                            if (diraddres["originalAddress1"] != DBNull.Value)
//                                address.originalAddress1 = diraddres["originalAddress1"].ToString();
//                            if (diraddres["originalAddress2"] != DBNull.Value)
//                                address.originalAddress2 = diraddres["originalAddress2"].ToString();
//                            if (diraddres["originalAddress3"] != DBNull.Value)
//                                address.originalAddress3 = diraddres["originalAddress3"].ToString();
//                            if (diraddres["originalAddress4"] != DBNull.Value)
//                                address.originalAddress4 = diraddres["originalAddress4"].ToString();
//                            if (diraddres["originalPostalCode"] != DBNull.Value)
//                                address.originalPostalCode = diraddres["originalPostalCode"].ToString();
//                            if (diraddres["lastUpdatedDate"] != DBNull.Value)
//                                address.lastUpdatedDate = Convert.ToDateTime(diraddres["lastUpdatedDate"]);

//                            address.directorFullAddress = "";
//                            if (!string.IsNullOrEmpty(address.originalAddress1) && address.originalAddress1 != " ")
//                                address.directorFullAddress += address.originalAddress1;

//                            if (!string.IsNullOrEmpty(address.originalAddress2) && address.originalAddress2 != " ")
//                            {
//                                if (!string.IsNullOrEmpty(address.directorFullAddress))
//                                    address.directorFullAddress += ", ";
//                                address.directorFullAddress += address.originalAddress2;
//                            }
//                            if (!string.IsNullOrEmpty(address.originalAddress3) && address.originalAddress3 != " ")
//                            {
//                                if (!string.IsNullOrEmpty(address.directorFullAddress))
//                                    address.directorFullAddress += ", ";
//                                address.directorFullAddress += address.originalAddress3;
//                            }

//                            if (!string.IsNullOrEmpty(address.originalAddress4) && address.originalAddress4 != " ")
//                            {
//                                if (!string.IsNullOrEmpty(address.directorFullAddress))
//                                    address.directorFullAddress += ", ";
//                                address.directorFullAddress += address.originalAddress4;
//                            }
//                            if (!string.IsNullOrEmpty(address.originalPostalCode) && address.originalPostalCode != " ")
//                            {
//                                if (!string.IsNullOrEmpty(address.directorFullAddress))
//                                    address.directorFullAddress += ", ";
//                                address.directorFullAddress += address.originalPostalCode;
//                            }

//                            var directoraddresspre = director.directoraddresses.Find(m => m.addressTypeInd == address.addressTypeInd && m.originalAddress1 == address.originalAddress1
//                      && m.originalAddress2 == address.originalAddress2 && m.originalAddress3 == address.originalAddress3 && m.originalAddress4 == address.originalAddress4 &&
//                      m.originalPostalCode == address.originalPostalCode);

//                            if (directoraddresspre == null)
//                            {
//                                director.directoraddresses.Add(address);
//                            }

//                        }
//                    }

//                    DataTable dtdirTelephone = data.Tables[1];
//                    if (dtdirTelephone.Rows.Count > 0)
//                    {
//                        foreach (DataRow telephone in dtdirTelephone.Rows)
//                        {
//                            var phone = new DirectorTelephoneVM();
//                            if (telephone["TelephoneTypeInd"] != DBNull.Value)
//                            {
//                                var data11 = lookuplst.FirstOrDefault(t => t.Type == "Telephone Type Indicator" && t.Value == telephone["TelephoneTypeInd"].ToString());
//                                if (data11 != null)
//                                {
//                                    phone.TelephoneTypeInd = data11.Text;
//                                }
//                            }
//                            if (telephone["TelephoneCode"] != DBNull.Value)
//                                phone.TelephoneCode = telephone["TelephoneCode"].ToString();
//                            if (telephone["TelephoneNo"] != DBNull.Value)
//                                phone.TelephoneNo = telephone["TelephoneNo"].ToString();
//                            if (telephone["LastUpdatedDate"] != DBNull.Value)
//                                phone.LastUpdatedDate = Convert.ToDateTime(telephone["LastUpdatedDate"]);
//                            if (telephone["CreatedOnDate"] != DBNull.Value)
//                                phone.CreatedOnDate = Convert.ToDateTime(telephone["CreatedOnDate"]);
//                            if (phone.TelephoneNo.Length != 10)
//                            {
//                                phone.DirectorTelephone = phone.TelephoneCode + phone.TelephoneNo;
//                            }
//                            else
//                            {
//                                phone.DirectorTelephone = phone.TelephoneNo;
//                            }
//                            if (phone.DirectorTelephone.Length == 10)
//                            {
//                                director.directortelephones.Add(phone);
//                            }

//                        }

//                    }
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//            return director;
//        }

//        public Endorsement GetEndorsement(int Propertydeedid)
//        {
//            Endorsement property1 = new Endorsement();
//            try
//            {
//                SqlConnection con = (SqlConnection)_productionContext.Database.GetDbConnection();
//                using (con)
//                {
//                    if (con.State != ConnectionState.Open)
//                        con.Open();
//                    DataSet data = new DataSet();
//                    SqlCommand cmd = new SqlCommand("qspEndorsementDetail", con);
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.Add(new SqlParameter("@PropertyDeedId", Propertydeedid));
//                    cmd.CommandTimeout = 0;
//                    List<LookupData> lookuplst = _IDAScontext.LookupDatas.ToList();

//                    SqlDataAdapter sDA = new SqlDataAdapter();
//                    sDA.SelectCommand = cmd;

//                    if (con.State != ConnectionState.Open)
//                        con.Open();

//                    sDA.Fill(data);

//                    DataTable endr = data.Tables[0];
//                    if (endr.Rows.Count > 0)
//                    {
//                        if (endr.Rows[0]["EndorsementAmount"] != DBNull.Value)
//                            property1.EndorsementAmount = Convert.ToInt32(endr.Rows[0]["EndorsementAmount"]);
//                        if (endr.Rows[0]["EndorsementHolder"] != DBNull.Value)
//                            property1.EndorsementHolder = endr.Rows[0]["EndorsementHolder"].ToString();
//                        if (endr.Rows[0]["EndorsementNumber"] != DBNull.Value)
//                            property1.EndorsementNumber = endr.Rows[0]["EndorsementNumber"].ToString();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//            return property1;
//        }
//    }
//}
//public class ReletionshipLinkVm
//{
//    public string Type { get; set; }
//    public string IdNumber { get; set; }
//    public string FullName { get; set; }
//    public long ConsumerId { get; set; }
//    public string KeyMatch { get; set; }
//    public string MatchCriteria { get; set; }
//    public string DisplayNo { get; set; }
//    public string DateOfBirth { get; set; }//june
//}



//public class ProfileRequest
//{
//    public long Id { get; set; }
//    public Guid userId { get; set; }
//    public Guid customerId { get; set; }
//    public string SearchType { get; set; }
//    public string InputType { get; set; }
//    public string SpouseID { get; set; }
//    public string SearchCriteria { get; set; }
//    public bool istrailuser { get; set; }
//}


//public class TimeLine
//{
//    public DateTime LastupdatedDate { get; set; }
//    public string Type { get; set; }
//    public string Text { get; set; }
//    public string TableName { get; set; }
//}

