using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.Production;
using Inspirit.IDAS.ESData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Inspirit.IDAS.Data.Production.CompanyProfile;

namespace Inspirit.IDAS.WebApplication
{
    public class ComapnyService
    {
        ProductionDbContext _productionContext;
        CustomerLogService _CustomerLogService;
        IDASDbContext _IDAScontext;
        ESService _eSService;
        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;
        public ComapnyService(ProductionDbContext productionContext, CustomerLogService CustomerLogService, IDASDbContext IDAScontext, IHostingEnvironment IHostingEnvironment, IConfiguration config)
        {
            _productionContext = productionContext;
            _IDAScontext = IDAScontext;
            _hostingEnvironment = IHostingEnvironment;
            _configuration = config;
            _eSService = new ESService(_configuration.GetSection("ElasticDBIP").GetSection("url").Value);
            _CustomerLogService = CustomerLogService;
        }

        /*public string getRegistrationNo(string registrationNo)
        {
            SqlConnection con = (SqlConnection)_productionContext.Database.GetDbConnection();
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.GetRegistrationNo", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@registrationNo", registrationNo);
            try
            {
                string a = (string)cmd.ExecuteScalar();
                return a;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            con.Close();

        }*/
        public CompanyProfile GetComapnyProfile(CompanyRequest request)
        {
            CompanyProfile profile = null;
            try
            {
                //string r = "1970/006206/07";
                //string reg = getRegistrationNo(r);
                List<LookupData> lookupLst = _IDAScontext.LookupDatas.ToList();
                SqlConnection con = (SqlConnection)_productionContext.Database.GetDbConnection();
                using (con)
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    DataSet data = new DataSet();
                    SqlCommand cmd = new SqlCommand("qspCommercialInformation", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CommercialId", Convert.ToInt32(request.Id)));
                    cmd.CommandTimeout = 0;

                    Commercial commercial = new Commercial();


                    SqlDataAdapter sDA = new SqlDataAdapter();
                    sDA.SelectCommand = cmd;

                    if (con.State != ConnectionState.Open)
                        con.Open();
                    sDA.Fill(data);
                    profile = ConvertCompanyProfile(lookupLst, request.istrailuser, data);
                    CustomerLogData logs = new CustomerLogData();

                    if (request.istrailuser == false)
                    {
                        // krishna pending isXDS
                        var reslogs = _CustomerLogService.UpdateCustomerLog(request.userId, request.customerId, "Tracing", request.SearchType, request.SearchCriteria, profile.commercialName, request.InputType,request.istrailuser,null,null,false,null);
                    }
                    else
                    {
                        var tlogs = _CustomerLogService.UpdateTrailuserLog(request.userId, request.customerId, "Tracing", request.SearchType, request.SearchCriteria, profile.commercialName, request.InputType,request.istrailuser);
                    }
                    var tabdetails = _IDAScontext.Customers.Where(t => t.Id == request.customerId).FirstOrDefault();
                    if (tabdetails != null)
                    {
                        profile.tabSelected = tabdetails.TabSelected;
                    }
                    if (con.State != ConnectionState.Closed)
                        con.Close();
                }
            }

            catch (Exception ex)
            {
                var msg = ex;
                string commrequest = JsonConvert.SerializeObject(msg);
                WriteLog(commrequest, "Company detail");
            }

            return profile;
        }
        public CompanyProfile ConvertCompanyProfile( List<LookupData> lookupLst, bool isTrailuser, DataSet dataSet)
        {
            //TODO conversion code
            CompanyProfile profile = new CompanyProfile();
            List<TimeLine> timelines = new List<TimeLine>();

            if (isTrailuser == false)
            {

                DataTable dtcommercial = dataSet.Tables[0];
                if (dtcommercial.Rows.Count > 0)
                {
                    if (dtcommercial.Rows[0]["RegistrationNo"] != DBNull.Value)
                        profile.registrationNo = dtcommercial.Rows[0]["RegistrationNo"].ToString();
                    else
                        profile.registrationNo = "Unknown";
                    if (dtcommercial.Rows[0]["RegistrationNoOld"] != DBNull.Value)
                        profile.registrationNoOld = dtcommercial.Rows[0]["RegistrationNoOld"].ToString();
                    else
                        profile.registrationNoOld = "Unknown";
                    if (dtcommercial.Rows[0]["CommercialName"] != DBNull.Value)
                        profile.commercialName = dtcommercial.Rows[0]["CommercialName"].ToString();
                    else
                        profile.commercialName = "Unknown";
                    if (dtcommercial.Rows[0]["CommercialShortName"] != DBNull.Value)
                        profile.commercialShortName = dtcommercial.Rows[0]["CommercialShortName"].ToString();
                    else
                        profile.commercialShortName = "Unknown";
                    if (dtcommercial.Rows[0]["CommercialTranslatedName"] != DBNull.Value)
                        profile.commercialTranslatedName = dtcommercial.Rows[0]["CommercialTranslatedName"].ToString();
                    else
                        profile.commercialShortName = "Unknown";
                    if (dtcommercial.Rows[0]["PreviousBusinessname"] != DBNull.Value)
                        profile.previousBusinessname = dtcommercial.Rows[0]["PreviousBusinessname"].ToString();
                    else
                        profile.previousBusinessname = "Unknown";
                    if (dtcommercial.Rows[0]["RegistrationDate"] != DBNull.Value)
                        profile.registrationDate = Convert.ToDateTime(dtcommercial.Rows[0]["RegistrationDate"]);
                    if (dtcommercial.Rows[0]["BusinessStartDate"] != DBNull.Value)
                        profile.businessStartDate = Convert.ToDateTime(dtcommercial.Rows[0]["BusinessStartDate"]);
                    if (dtcommercial.Rows[0]["FinancialYearEnd"] != DBNull.Value)
                        profile.financialYearEnd = Convert.ToInt32(dtcommercial.Rows[0]["FinancialYearEnd"]);
                    if (dtcommercial.Rows[0]["FinancialEffectiveDate"] != DBNull.Value)
                        profile.financialEffectiveDate = Convert.ToDateTime(dtcommercial.Rows[0]["FinancialEffectiveDate"]);
                    if (dtcommercial.Rows[0]["LastUpdatedDate"] != DBNull.Value)
                        profile.lastUpdatedDate = Convert.ToDateTime(dtcommercial.Rows[0]["LastUpdatedDate"]);

                    if (dtcommercial.Rows[0]["SICCode"] != DBNull.Value)
                    {
                        var data = lookupLst.FirstOrDefault(t => t.Type == "SIC Code" && t.Value == dtcommercial.Rows[0]["SICCode"].ToString());
                        if (data != null)
                        {
                            profile.sICCode = data.Text;
                        }
                    }
                    else
                        profile.sICCode = "Unknown";
                    if (dtcommercial.Rows[0]["BusinessDesc"] != DBNull.Value)
                        profile.businessDesc = dtcommercial.Rows[0]["BusinessDesc"].ToString();
                    else
                        profile.businessDesc = "Unknown";
                    if (dtcommercial.Rows[0]["CommercialStatusCode"] != DBNull.Value)
                    {
                        var data5 = lookupLst.FirstOrDefault(t => t.Type == "Status Code of Company" && t.Value == dtcommercial.Rows[0]["CommercialStatusCode"].ToString());
                        if (data5 != null)
                        {
                            profile.commercialStatusCode = data5.Text;
                        }
                    }
                    else
                        profile.commercialStatusCode = "Unknown";
                    if (dtcommercial.Rows[0]["CommercialTypeCode"] != DBNull.Value)
                    {
                        var data6 = lookupLst.FirstOrDefault(t => t.Type == "Type Code of Company" && t.Value == dtcommercial.Rows[0]["CommercialTypeCode"].ToString());
                        if (data6 != null)
                        {
                            profile.commercialTypeCode = data6.Text;
                        }
                    }
                    else
                        profile.commercialTypeCode = "Unknown";
                    if (dtcommercial.Rows[0]["VATNo"] != DBNull.Value)
                        profile.vATNo = Convert.ToInt64(dtcommercial.Rows[0]["VATNo"]);
                    if (dtcommercial.Rows[0]["bussEmail"] != DBNull.Value)
                        profile.bussEmail = dtcommercial.Rows[0]["bussEmail"].ToString();
                    else
                        profile.bussEmail = "Unknown";
                    if (dtcommercial.Rows[0]["BussWebsite"] != DBNull.Value)
                        profile.bussWebsite = dtcommercial.Rows[0]["BussWebsite"].ToString();
                    else
                        profile.bussWebsite = "Unknown";
                    if (dtcommercial.Rows[0]["CreatedOnDate"] != DBNull.Value)
                        profile.createdOnDate = Convert.ToDateTime(dtcommercial.Rows[0]["CreatedOnDate"]);
                    if (dtcommercial.Rows[0]["AmountPerShare"] != DBNull.Value)
                        profile.amountPerShare = Convert.ToDecimal(dtcommercial.Rows[0]["AmountPerShare"]);
                    if (dtcommercial.Rows[0]["NoOfShares"] != DBNull.Value)
                        profile.noOfShares = Convert.ToDecimal(dtcommercial.Rows[0]["NoOfShares"]);
                    if (dtcommercial.Rows[0]["Premium"] != DBNull.Value)
                        profile.premium = Convert.ToInt32(dtcommercial.Rows[0]["Premium"]);
                }
                // Address

                DataTable dtaddress = dataSet.Tables[1];
                if (dtaddress.Rows.Count > 0)
                {
                    foreach (DataRow dtadd in dtaddress.Rows)
                    {
                        var address = new AddressDetail();

                        if (dtadd["AddressTypeInd"] != DBNull.Value)
                        {
                            var data1 = lookupLst.Where(t => t.Type == "Address Type Indicator" && t.Value == dtadd["AddressTypeInd"].ToString()).FirstOrDefault();
                            if (data1 != null)
                            {
                                address.addressTypeInd = data1.Text;

                            }
                        }
                        if (dtadd["CommercialAddressID"] != DBNull.Value)
                            address.id = dtadd["CommercialAddressID"].ToString();
                        if (dtadd["OriginalAddress1"] != DBNull.Value)
                            address.originalAddress1 = dtadd["OriginalAddress1"].ToString();
                        if (dtadd["OriginalAddress2"] != DBNull.Value)
                            address.originalAddress2 = dtadd["OriginalAddress2"].ToString();
                        if (dtadd["OriginalAddress3"] != DBNull.Value)
                            address.originalAddress3 = dtadd["OriginalAddress3"].ToString();
                        if (dtadd["OriginalAddress4"] != DBNull.Value)
                            address.originalAddress4 = dtadd["OriginalAddress4"].ToString();
                        if (dtadd["OriginalPostalCode"] != DBNull.Value)
                            address.originalPostalCode = dtadd["OriginalPostalCode"].ToString();
                        if (dtadd["CreatedOnDate"] != DBNull.Value)
                            address.createdOnDate = Convert.ToDateTime(dtadd["CreatedOnDate"]);
                        if (dtadd["LastUpdatedDate"] != DBNull.Value)
                            address.lastUpdatedDate = Convert.ToDateTime(dtadd["LastUpdatedDate"]);

                        //Check & Delete

                        if (dtadd["occupantTypeInd"] != DBNull.Value)
                        {
                            var data1 = lookupLst.Where(t => t.Type == "Occupant Type Indicator" && t.Value == Convert.ToString(dtadd["OccupantTypeInd"])).FirstOrDefault();
                            if (data1 != null)
                            {
                                address.occupantTypeInd = data1.Text;

                            }
                        }

                        address.fullAddress = "";
                        if (!string.IsNullOrEmpty(address.originalAddress1) && (address.originalAddress1 != " "))
                            address.fullAddress += address.originalAddress1;

                        if (!string.IsNullOrEmpty(address.originalAddress2) && (address.originalAddress2 != " "))
                        {
                            if (!string.IsNullOrEmpty(address.fullAddress))
                                address.fullAddress += ", ";
                            address.fullAddress += address.originalAddress2;
                        }
                        if (!string.IsNullOrEmpty(address.originalAddress3) && (address.originalAddress3 != " "))
                        {
                            if (!string.IsNullOrEmpty(address.fullAddress))
                                address.fullAddress += ", ";
                            address.fullAddress += address.originalAddress3;
                        }

                        if (!string.IsNullOrEmpty(address.originalAddress4) && (address.originalAddress3 != " "))
                        {
                            if (!string.IsNullOrEmpty(address.fullAddress))
                                address.fullAddress += ", ";
                            address.fullAddress += address.originalAddress4;
                        }
                        if (!string.IsNullOrEmpty(address.originalPostalCode) && (address.originalPostalCode != " "))
                        {
                            if (!string.IsNullOrEmpty(address.fullAddress))
                                address.fullAddress += ", ";
                            address.fullAddress += address.originalPostalCode;
                        }


                        var addrpresent = profile.addresses.Find(m => m.originalAddress1 == address.originalAddress1 && m.originalAddress2 == address.originalAddress2 && m.originalAddress3 == address.originalAddress3
                        && m.originalAddress4 == address.originalAddress4 && m.addressTypeInd == address.addressTypeInd);
                        if (addrpresent == null)
                        {
                            profile.addresses.Add(address);
                        }
                    }

                    profile.addresses = profile.addresses.OrderByDescending(t => t.lastUpdatedDate).ToList();
                    foreach (var address in profile.addresses)
                    {
                        if (address.addressTypeInd != null && address.lastUpdatedDate != null)
                        {
                            var addresspresent = timelines.FindAll(p => p.Type == address.addressTypeInd).ToList();

                            if (addresspresent.Count == 0)
                            {
                                TimeLine line = new TimeLine();
                                line.LastupdatedDate = address.lastUpdatedDate.Value;
                                line.Text = address.originalAddress1 + " " + address.originalAddress2 + " " + address.originalAddress3 + " " + address.originalAddress4;
                                line.TableName = "Address";
                                line.Type = address.addressTypeInd;
                                timelines.Add(line);
                            }
                        }
                    }
                }


                DataTable dtphone = dataSet.Tables[2];
                if (dtphone.Rows.Count > 0)
                {
                    foreach (DataRow drphone in dtphone.Rows)
                    {

                        var detail = new ContactDetail();
                        if (drphone["CommercialTelephoneID"] != DBNull.Value)
                            detail.id = Convert.ToInt64(drphone["CommercialTelephoneID"]);
                        if (drphone["TelephoneTypeInd"] != DBNull.Value)
                        {
                            var data2 = lookupLst.Where(t => t.Type == "Telephone Type Indicator" && t.Value == drphone["TelephoneTypeInd"].ToString()).FirstOrDefault();
                            if (data2 != null)
                            {
                                detail.type = data2.Text;
                            }
                        }
                        if (drphone["TelephoneCode"] != DBNull.Value)
                            detail.telephoneCode = drphone["TelephoneCode"].ToString();
                        if (drphone["CreatedOnDate"] != DBNull.Value)
                            detail.createdonDate = Convert.ToDateTime(drphone["CreatedOnDate"]);
                        if (drphone["LastUpdatedDate"] != DBNull.Value)
                            detail.lastUpdatedDate = Convert.ToDateTime(drphone["LastUpdatedDate"]);
                        if (drphone["TelephoneNo"] != DBNull.Value)
                            detail.telephoneNo = drphone["TelephoneNo"].ToString();
                        detail.peopleLinked = GetCount(drphone["TelephoneNo"].ToString());


                       


                        if (detail.telephoneNo.Length != 10)
                        {
                            detail.telephoneNo = detail.telephoneCode + detail.telephoneNo;
                        }
                        else
                        {
                            detail.telephoneNo = detail.telephoneNo;
                        }
                        if (detail.telephoneNo.Length == 10)
                        {
                          
                            profile.contacts.Add(detail);
                        }
                    }

                    profile.addresses = profile.addresses.OrderByDescending(t => t.lastUpdatedDate).ToList();
                    foreach (var detail in profile.contacts)
                    {
                        if (detail.createdonDate != null)
                        {
                            var contactpresent = timelines.Find(p => p.Type == detail.type);
                            if (contactpresent == null)
                            {
                                TimeLine line = new TimeLine();
                                line.LastupdatedDate = detail.lastUpdatedDate.Value;
                                line.Text = detail.telephoneNo;
                                line.TableName = "Contacts";
                                line.Type = detail.type;
                                timelines.Add(line);
                            }
                        }
                    }
                }

                DataTable dtTelephoneCode = dataSet.Tables[3];
                if (dtTelephoneCode.Rows.Count > 0)
                {
                    foreach (ContactDetail t in profile.contacts)
                    {
                        DataRow[] dr = dtTelephoneCode.Select("Code = '" + t.telephoneCode + "'");
                        if (dr.Count() > 0)
                        {
                            if (dr[0]["Region"] != DBNull.Value)
                                t.CodeRegion = Convert.ToString(dr[0]["Region"]);
                            if (dr[0]["Type"] != DBNull.Value)
                                t.CodeType = Convert.ToString(dr[0]["Type"]);
                        }
                        else
                        {
                            t.CodeRegion = "UnKnown";
                            t.CodeType = "UnKnown";
                        }
                    }
                }

                DataTable dtjudge = dataSet.Tables[4];
                if (dtjudge.Rows.Count > 0)
                {
                    foreach (DataRow drjudge in dtjudge.Rows)
                    {
                        var commercialJudgement = new CommercialJudgement();
                        if (drjudge["CommercialName"] != DBNull.Value)
                            commercialJudgement.CommercialName = drjudge["CommercialName"].ToString();
                        if (drjudge["CommercialJudgmentID"] != DBNull.Value)
                            commercialJudgement.CommercialJudgmentID = Convert.ToInt32(drjudge["CommercialJudgmentID"]);
                        if (drjudge["Address1"] != DBNull.Value)
                            commercialJudgement.Address1 = drjudge["Address1"].ToString();
                        if (drjudge["Address2"] != DBNull.Value)
                            commercialJudgement.Address2 = drjudge["Address2"].ToString();
                        if (drjudge["Address3"] != DBNull.Value)
                            commercialJudgement.Address3 = drjudge["Address3"].ToString();
                        if (drjudge["Address4"] != DBNull.Value)
                            commercialJudgement.Address4 = drjudge["Address4"].ToString();
                        if (drjudge["PostalCode"] != DBNull.Value)
                            commercialJudgement.PostalCode = drjudge["PostalCode"].ToString();
                        if (drjudge["HomeTelephoneCode"] != DBNull.Value)
                            commercialJudgement.HomeTelephoneCode = drjudge["HomeTelephoneCode"].ToString();
                        if (drjudge["HomeTelephoneNo"] != DBNull.Value)
                            commercialJudgement.HomeTelephoneNo = drjudge["HomeTelephoneNo"].ToString();
                        if (drjudge["WorkTelephoneCode"] != DBNull.Value)
                            commercialJudgement.WorkTelephoneCode = drjudge["WorkTelephoneCode"].ToString();
                        if (drjudge["WorkTelephoneNo"] != DBNull.Value)
                            commercialJudgement.WorkTelephoneNo = drjudge["WorkTelephoneNo"].ToString();
                        if (drjudge["CellularNo"] != DBNull.Value)
                            commercialJudgement.CellularNo = drjudge["CellularNo"].ToString();
                        if (drjudge["FaxCode"] != DBNull.Value)
                            commercialJudgement.FaxCode = drjudge["FaxCode"].ToString();
                        if (drjudge["FaxNo"] != DBNull.Value)
                            commercialJudgement.FaxNo = drjudge["FaxNo"].ToString();
                        if (drjudge["CaseFilingDate"] != DBNull.Value)
                            commercialJudgement.CaseFilingDate = Convert.ToDateTime(drjudge["CaseFilingDate"]);
                        if (drjudge["CaseNumber"] != DBNull.Value)
                            commercialJudgement.CaseNumber = drjudge["CaseNumber"].ToString();
                        if (drjudge["CaseFilingDate"] != DBNull.Value)
                            commercialJudgement.CaseFilingDate = Convert.ToDateTime(drjudge["CaseFilingDate"]);
                        if (drjudge["CaseType"] != DBNull.Value)
                            commercialJudgement.CaseType = drjudge["CaseType"].ToString();
                        if (drjudge["CaseReason"] != DBNull.Value)
                            commercialJudgement.CaseReason = drjudge["CaseReason"].ToString();
                        if (drjudge["DisputeAmt"] != DBNull.Value)
                            commercialJudgement.DisputeAmt = Convert.ToDecimal(drjudge["DisputeAmt"]);
                        if (drjudge["CourtName"] != DBNull.Value)
                            commercialJudgement.CourtName = drjudge["CourtName"].ToString();
                        if (drjudge["CourtCity"] != DBNull.Value)
                            commercialJudgement.CourtCity = drjudge["CourtCity"].ToString();
                        if (drjudge["CourtType"] != DBNull.Value)
                            commercialJudgement.CourtType = drjudge["CourtType"].ToString();
                        if (drjudge["CourtCaseID"] != DBNull.Value)
                            commercialJudgement.CourtCaseID = Convert.ToInt32(drjudge["CourtCaseID"]);
                        if (drjudge["PlaintiffName"] != DBNull.Value)
                            commercialJudgement.PlaintiffName = drjudge["PlaintiffName"].ToString();
                        if (drjudge["Plaintiff1Address"] != DBNull.Value)
                            commercialJudgement.Plaintiff1Address = drjudge["Plaintiff1Address"].ToString();
                        if (drjudge["AttorneyName"] != DBNull.Value)
                            commercialJudgement.AttorneyName = drjudge["AttorneyName"].ToString();
                        if (drjudge["AttorneyTelephoneCode"] != DBNull.Value)
                            commercialJudgement.AttorneyTelephoneCode = drjudge["AttorneyTelephoneCode"].ToString();
                        if (drjudge["AttorneyTelephoneNo"] != DBNull.Value)
                            commercialJudgement.AttorneyTelephoneNo = drjudge["AttorneyTelephoneNo"].ToString();
                        if (drjudge["AttorneyFaxCode"] != DBNull.Value)
                            commercialJudgement.AttorneyFaxCode = drjudge["AttorneyFaxCode"].ToString();
                        if (drjudge["AttorneyFaxNo"] != DBNull.Value)
                            commercialJudgement.AttorneyFaxNo = drjudge["AttorneyFaxNo"].ToString();
                        if (drjudge["AttorneyAddress1"] != DBNull.Value)
                            commercialJudgement.AttorneyAddress1 = drjudge["AttorneyAddress1"].ToString();
                        if (drjudge["AttorneyAddress2"] != DBNull.Value)
                            commercialJudgement.AttorneyAddress2 = drjudge["AttorneyAddress2"].ToString();
                        if (drjudge["AttorneyAddress3"] != DBNull.Value)
                            commercialJudgement.AttorneyAddress3 = drjudge["AttorneyAddress3"].ToString();
                        if (drjudge["AttorneyAddress4"] != DBNull.Value)
                            commercialJudgement.AttorneyAddress4 = drjudge["AttorneyAddress4"].ToString();
                        if (drjudge["AttorneyPostalCode"] != DBNull.Value)
                            commercialJudgement.AttorneyPostalCode = drjudge["AttorneyPostalCode"].ToString();
                        if (drjudge["ReferenceNo"] != DBNull.Value)
                            commercialJudgement.ReferenceNo = drjudge["ReferenceNo"].ToString();
                        if (drjudge["LastUpdatedDate"] != DBNull.Value)
                            commercialJudgement.LastUpdatedDate = Convert.ToDateTime(drjudge["LastUpdatedDate"]);
                        if (drjudge["CreatedOnDate"] != DBNull.Value)
                            commercialJudgement.CreatedOnDate = Convert.ToDateTime(drjudge["CreatedOnDate"]);
                        if (drjudge["DisputeDate"] != DBNull.Value)
                            commercialJudgement.DisputeDate = Convert.ToDateTime(drjudge["DisputeDate"]);
                        if (drjudge["DisputeResolvedDate"] != DBNull.Value)
                            commercialJudgement.DisputeResolvedDate = Convert.ToDateTime(drjudge["DisputeResolvedDate"]);
                        if (drjudge["Rescinded"] != DBNull.Value)
                            commercialJudgement.Rescinded = Convert.ToBoolean(drjudge["Rescinded"]);
                        if (drjudge["RescissionDate"] != DBNull.Value)
                            commercialJudgement.RescissionDate = Convert.ToDateTime(drjudge["RescissionDate"]);
                        if (drjudge["RescissionReason"] != DBNull.Value)
                            commercialJudgement.RescissionReason = drjudge["RescissionReason"].ToString();
                        if (drjudge["RescindedAmount"] != DBNull.Value)
                            commercialJudgement.RescindedAmount = drjudge["RescindedAmount"].ToString();


                        var judgepresent = profile.commercialJudgements.Find(t => t.CaseNumber == commercialJudgement.CaseNumber && t.PlaintiffName == commercialJudgement.PlaintiffName);
                        if (judgepresent == null)
                        {
                            profile.commercialJudgements.Add(commercialJudgement);
                        }

                    }
                }


                DataTable dtcommdir = dataSet.Tables[5];
                DataTable dtdire = dataSet.Tables[6];
              
                if (dtcommdir.Rows.Count > 0)
                {
                    foreach (DataRow drCommercialDirector in dtcommdir.Rows)
                    {

                        var directors = new DirectorShip();

                        if (drCommercialDirector["AppointmentDate"] != DBNull.Value)
                            directors.appointmentDate = Convert.ToDateTime(drCommercialDirector["AppointmentDate"]);
                        if (drCommercialDirector["DirectorSetDate"] != DBNull.Value)
                            directors.directorSetDate = Convert.ToDateTime(drCommercialDirector["DirectorSetDate"]);
                        if (drCommercialDirector["CommercialID"] != DBNull.Value)
                            directors.companyid = Convert.ToInt32(drCommercialDirector["CommercialID"]);
                      
                        if (drCommercialDirector["CommercialDirectorID"] != DBNull.Value)
                            directors.CommDirId = drCommercialDirector["CommercialDirectorID"].ToString();
                        if (drCommercialDirector["CreatedOnDate"] != DBNull.Value)
                            directors.createdOnDate = Convert.ToDateTime(drCommercialDirector["CreatedOnDate"]);
                        
                        if (drCommercialDirector["MemberSize"] != DBNull.Value)
                            directors.memberSize = Convert.ToDecimal(drCommercialDirector["MemberSize"]);
                        if (drCommercialDirector["MemberControlPerc"] != DBNull.Value)
                            directors.memberControlPerc = Convert.ToDecimal(drCommercialDirector["MemberControlPerc"]);
                        if (drCommercialDirector["Profession"] != DBNull.Value)
                            directors.profession = drCommercialDirector["Profession"].ToString();
                        if (drCommercialDirector["DirectorDesignationCode"] != DBNull.Value)
                        {
                            var data1 = lookupLst.FirstOrDefault(t => t.Type == "Director Designation Code" && t.Value == drCommercialDirector["DirectorDesignationCode"].ToString());
                            if (data1 != null)
                            {
                                directors.directorDesignationCode = data1.Text;

                            }
                        }
                        if (drCommercialDirector["DirectorStatusCode"] != DBNull.Value)
                        {
                            var data2 = lookupLst.FirstOrDefault(t => t.Type == "Director Status Code" && t.Value == drCommercialDirector["DirectorStatusCode"].ToString());
                            if (data2 != null)
                            {
                                directors.directorStatusCode = data2.Text;

                            }
                        }
                        if (drCommercialDirector["DirectorStatusDate"] != DBNull.Value)
                            directors.directorStatusDate = Convert.ToDateTime(drCommercialDirector["DirectorStatusDate"]);

                        if (drCommercialDirector["DirectorTypeCode"] != DBNull.Value)
                        {
                            var data3 = lookupLst.FirstOrDefault(t => t.Type == "Director Type Code" && t.Value == drCommercialDirector["DirectorTypeCode"].ToString());
                            if (data3 != null)
                            {
                                directors.directorTypeCode = data3.Text;

                            }
                        }
                        if (drCommercialDirector["IsRSAResidentYN"] != DBNull.Value)
                        {

                            var data4 = lookupLst.FirstOrDefault(t => t.Type == "Is RSA Resident YN" && t.Value == drCommercialDirector["IsRSAResidentYN"].ToString());
                            if (data4 != null)
                            {
                                directors.isRSAResidentYN = data4.Text;

                            }
                        }
                        if (dtcommdir.Rows.Count > 0)
                        {
                            DataRow[] drDirector = dtdire.Select("DirectorID = " + drCommercialDirector["DirectorID"].ToString());

                            if (drDirector[0]["IDNo"] != DBNull.Value)
                                directors.idNumber = drDirector[0]["IDNo"].ToString();
                            if (drDirector[0]["FirstInitial"] != DBNull.Value)
                                directors.firstInitial = drDirector[0]["FirstInitial"].ToString();
                            if (drDirector[0]["SecondInitial"] != DBNull.Value)
                                directors.secondInitial = drDirector[0]["SecondInitial"].ToString();

                            string fname = string.Empty;
                            string sname = string.Empty;
                            string surname = string.Empty;
                            if (drDirector[0]["FirstName"] != DBNull.Value)
                                fname = drDirector[0]["FirstName"].ToString();
                            if (drDirector[0]["SecondName"] != DBNull.Value)
                                sname =" " + drDirector[0]["SecondName"].ToString();
                           
                            if (drDirector[0]["Surname"] != DBNull.Value)
                                directors.surname = drDirector[0]["Surname"].ToString();
                            surname = " " + directors.surname;
                            directors.fullName = fname + sname + surname;
                            if (drDirector[0]["SurnamePrevious"] != DBNull.Value)
                                directors.surnamePrevious = drDirector[0]["SurnamePrevious"].ToString();
                            if (drDirector[0]["BirthDate"] != DBNull.Value)
                                directors.birthDate = Convert.ToDateTime(drDirector[0]["BirthDate"]);
                            if (drDirector[0]["DirectorID"] != DBNull.Value)
                                directors.id = drDirector[0]["DirectorID"].ToString();
                            if (drDirector[0]["LastUpdatedDate"] != DBNull.Value)
                                directors.lastUpdatedDate = Convert.ToDateTime(drDirector[0]["LastUpdatedDate"]);
                        }
                        var directorpresent = profile.directorShips.Find(t => t.idNumber == directors.idNumber && t.appointmentDate == directors.appointmentDate);
                        if (directorpresent == null)
                        {
                            profile.directorShips.Add(directors);
                        }

                    }
                }


                DataTable dtAuditor = dataSet.Tables[7];
              
                if (dtAuditor.Rows.Count > 0)
                {
                    foreach (DataRow draud in dtAuditor.Rows)
                    {
                        var aud = new CommercialAuditorVm();

                        if (draud["AuditorName"] != DBNull.Value)
                            aud.AuditorName = draud["AuditorName"].ToString();
                        if (draud["ActStartDate"] != DBNull.Value)
                            aud.ActStartDate = Convert.ToDateTime(draud["ActStartDate"]);
                        if (draud["ActEndDate"] != DBNull.Value)
                            aud.ActEndDate = Convert.ToDateTime(draud["ActEndDate"]);
                        if (draud["CreatedOnDate"] != DBNull.Value)
                            aud.CreatedOnDate = Convert.ToDateTime(draud["CreatedOnDate"]);
                        if (draud["LastUpdatedDate"] != DBNull.Value)
                            aud.LastUpdatedDate = Convert.ToDateTime(draud["LastUpdatedDate"]);
                        if (draud["ProfessionCode"] != DBNull.Value)
                        {
                            var data3 = lookupLst.Where(t => t.Type == "Profession Code" && t.Value == draud["ProfessionCode"].ToString()).FirstOrDefault();
                            if (data3 != null)
                            {
                                aud.ProfessionCode = data3.Text;

                            }
                        }
                        if (draud["AuditorTypeCode"] != DBNull.Value)
                        {
                            var data2 = lookupLst.Where(t => t.Type == "Auditor Type Code" && t.Value == draud["AuditorTypeCode"].ToString()).FirstOrDefault();
                            if (data2 != null)
                            {
                                aud.AuditorTypeCode = data2.Text;

                            }
                        }
                        if (draud["ProfessionNo"] != DBNull.Value)
                            aud.ProfessionNo = draud["ProfessionNo"].ToString();
                        if (draud["AuditorStatusCode"] != DBNull.Value)
                        {
                            var data1 = lookupLst.Where(t => t.Type == "Auditor Status Code" && t.Value == draud["AuditorStatusCode"].ToString()).FirstOrDefault();
                            {
                                aud.AuditorStatusCode = data1.Text;

                            }
                        }
                        if (draud["AuditorID"] != DBNull.Value)
                            aud.AuditorID = Convert.ToInt32(draud["AuditorID"]);
                        if (draud["CommercialAuditorID"] != DBNull.Value)
                            aud.CommercialAuditorID = Convert.ToInt32(draud["CommercialAuditorID"]);
                        if (draud["CommercialID"] != DBNull.Value)
                            aud.CommercialID = Convert.ToInt32(draud["CommercialID"]);
                       
                        var auditorpresent = profile.commercialAuditors.Find(t => t.AuditorName == aud.AuditorName && t.ProfessionNo == aud.ProfessionNo);
                        if (auditorpresent == null)
                        {
                            profile.commercialAuditors.Add(aud);
                        }
                    }
                }

                //Property deeds Buyer
                DataTable dtBuyer = dataSet.Tables[8];
                DataTable dtdeedBuyer = dataSet.Tables[9];
                
                if (dtBuyer.Rows.Count > 0)
                    foreach (DataRow dr in dtBuyer.Rows)
                    {
                        var property1 = new PropertyDeedDetail();
                        foreach (DataRow drdeed in dtdeedBuyer.Select("PropertyDeedId = " + dr["PropertyDeedId"].ToString()))
                        {
                            if (drdeed["DeedsOfficeId"] != DBNull.Value)
                            {
                                var data11 = lookupLst.Where(t => t.Type == "Deeds Office Identifier" && t.Value == drdeed["DeedsOfficeId"].ToString()).FirstOrDefault();
                                if (data11 != null)
                                {
                                    property1.deedsOfficeId = data11.Text;
                                }
                            }
                            if (drdeed["TitleDeedNo"] != DBNull.Value)
                                property1.titleDeedNo = drdeed["TitleDeedNo"].ToString();
                            if (drdeed["TitleDeedNoOLD"] != DBNull.Value)
                                property1.titleDeedNoOLD = drdeed["TitleDeedNoOLD"].ToString();
                            if (drdeed["TitleDeedFee"] != DBNull.Value)
                                property1.titleDeedFee = (Convert.ToInt32(drdeed["TitleDeedFee"]));
                            if (drdeed["DatePurchase"] != DBNull.Value)
                                property1.datePurchase = Convert.ToDateTime(drdeed["DatePurchase"]);
                            if (drdeed["DateRegister"] != DBNull.Value)
                                property1.dateRegister = Convert.ToDateTime(drdeed["DateRegister"]);
                            if (drdeed["PurchaseAmount"] != DBNull.Value)
                                property1.purchaseAmount = Convert.ToDecimal(drdeed["PurchaseAmount"]);
                            if (drdeed["StreetAddress"] != DBNull.Value)
                                property1.streetAddress = drdeed["StreetAddress"].ToString();
                            if (drdeed["StreetNumber"] != DBNull.Value)
                                property1.streetNumber = drdeed["StreetNumber"].ToString();
                            if (drdeed["StreetName"] != DBNull.Value)
                                property1.streetName = drdeed["StreetName"].ToString();
                            if (drdeed["StreetType"] != DBNull.Value)
                                property1.streetType = drdeed["StreetType"].ToString();
                            if (drdeed["X"] != DBNull.Value)
                                property1.x = Convert.ToDecimal(drdeed["X"]);
                            if (drdeed["Y"] != DBNull.Value)
                                property1.y = Convert.ToDecimal(drdeed["Y"]);
                            if (drdeed["SuburbCode"] != DBNull.Value)
                                property1.suburbCode = drdeed["SuburbCode"].ToString();
                            if (drdeed["SuburbDeeds"] != DBNull.Value)
                                property1.suburbDeeds = drdeed["SuburbDeeds"].ToString();
                            if (drdeed["Town"] != DBNull.Value)
                                property1.town = drdeed["Town"].ToString();
                            if (drdeed["Authority"] != DBNull.Value)
                                property1.authority = drdeed["Authority"].ToString();
                            if (drdeed["MunicipalityName"] != DBNull.Value)
                                property1.municipalityName = drdeed["MunicipalityName"].ToString();
                            if (drdeed["ProvinceId"] != DBNull.Value)
                            {

                                var data = lookupLst.Where(t => t.Type == "Province Identifier" && t.Value == drdeed["ProvinceId"].ToString()).FirstOrDefault();
                                if (data != null)
                                {
                                    property1.provinceId = data.Text;

                                }
                            }
                            if (drdeed["IsCurrOwnerUpdated"] != DBNull.Value)
                            {
                                var data7 = lookupLst.Where(t => t.Type == "Is Current Owner Updated" && t.Value == drdeed["IsCurrOwnerUpdated"].ToString()).FirstOrDefault();
                                if (data7 != null)
                                {
                                    property1.isCurrOwnerUpdated = data7.Text;
                                }
                            }
                            if (drdeed["Extent"] != DBNull.Value)
                                property1.extent = drdeed["Extent"].ToString();
                            if (drdeed["AttorneyFirmNumber"] != DBNull.Value)
                                property1.attorneyFirmNumber = drdeed["AttorneyFirmNumber"].ToString();
                            if (drdeed["AttorneyFileNumber"] != DBNull.Value)
                                property1.attorneyFileNumber = drdeed["AttorneyFileNumber"].ToString();
                            if (drdeed["TransferSeqNo"] != DBNull.Value)
                                property1.transferSeqNo = Convert.ToInt32(drdeed["TransferSeqNo"]);
                            if (drdeed["DateCaptured"] != DBNull.Value)
                                property1.dateCaptured = Convert.ToDateTime(drdeed["DateCaptured"]);
                            if (drdeed["BondNumber"] != DBNull.Value)
                                property1.bondNumber = drdeed["BondNumber"].ToString();
                            if (drdeed["BondHolder"] != DBNull.Value)
                                property1.bondHolder = drdeed["BondHolder"].ToString();
                            if (drdeed["BondAmount"] != DBNull.Value)
                                property1.bondAmount = Convert.ToInt64(drdeed["BondAmount"]);
                            if (drdeed["PropertyType"] != DBNull.Value)
                            {
                                var data5 = lookupLst.Where(t => t.Type == "Property Type" && t.Value == drdeed["PropertyType"].ToString()).FirstOrDefault();
                                if (data5 != null)
                                {
                                    property1.propertyType = data5.Text;

                                }
                            }
                            if (drdeed["PropertyName"] != DBNull.Value)
                                property1.propertyName = drdeed["PropertyName"].ToString();
                            if (drdeed["SchemeId"] != DBNull.Value)
                                property1.schemeId = drdeed["SchemeId"].ToString();
                            if (drdeed["SuburbId"] != DBNull.Value)
                                property1.suburbId = Convert.ToInt16(drdeed["SuburbId"]);
                            if (drdeed["Erf"] != DBNull.Value)
                                property1.erf = drdeed["Erf"].ToString();
                            if (drdeed["Portion"] != DBNull.Value)
                                property1.portion = Convert.ToInt32(drdeed["Portion"]);
                            if (drdeed["Unit"] != DBNull.Value)
                                property1.unit = Convert.ToInt32(drdeed["Unit"]);
                            if (drdeed["CreatedOndate"] != DBNull.Value)
                                property1.createdOndate = Convert.ToDateTime(drdeed["CreatedOndate"]);
                            if (drdeed["ErfSize"] != DBNull.Value)
                                property1.erfSize = drdeed["ErfSize"].ToString();
                            if (drdeed["StandNo"] != DBNull.Value)
                                property1.standNo = drdeed["StandNo"].ToString();
                            if (drdeed["PortionNo"] != DBNull.Value)
                                property1.portionNo = drdeed["PortionNo"].ToString();
                            if (drdeed["TownShipNo"] != DBNull.Value)
                                property1.townShipNo = Convert.ToInt32(drdeed["TownShipNo"]);
                            if (drdeed["PrevExtent"] != DBNull.Value)
                                property1.prevExtent = drdeed["PrevExtent"].ToString();
                            if (drdeed["IsCurrentOwner"] != DBNull.Value)
                                property1.isCurrentOwner = Convert.ToBoolean(drdeed["IsCurrentOwner"]);
                            if (drdeed["PropertyDeedID"] != DBNull.Value)
                                property1.propertyDeedId = Convert.ToInt32(drdeed["PropertyDeedID"]);
                            if (drdeed["StreetAddress"] != DBNull.Value)
                                property1.address = drdeed["StreetAddress"].ToString();

                        }


                        if (dr["BuyerID"] != DBNull.Value)
                            property1.buyerid = dr["BuyerID"].ToString();
                        if (dr["BuyerIDNO"] != DBNull.Value)
                            property1.buyerIDNO = dr["BuyerIDNO"].ToString();
                        if (dr["BuyerName"] != DBNull.Value)
                            property1.buyerName = dr["BuyerName"].ToString();
                        if (dr["BuyerType"] != DBNull.Value)
                        {

                            var data1 = lookupLst.Where(t => t.Type == "Buyer Type" && t.Value == dr["BuyerType"].ToString()).FirstOrDefault();
                            if (data1 != null)
                            {
                                property1.buyerType = data1.Text;

                            }
                        }
                        property1.fullAddress = "";
                        if (!string.IsNullOrEmpty(property1.streetNumber))
                            property1.fullAddress += property1.streetNumber;

                        if (!string.IsNullOrEmpty(property1.streetName))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.streetName;
                        }
                        if (!string.IsNullOrEmpty(property1.streetType))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.streetType;
                        }

                        if (!string.IsNullOrEmpty(property1.town))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.town;
                        }

                        property1.buyerStatus = dr["BuyerStatus"].ToString();
                        property1.share = dr["Share"].ToString();
                        property1.type = "Buyer";



                        var propertybuyerexists = profile.propertyOwners.Find(t => t.type == property1.type && t.datePurchase == property1.datePurchase && t.fullAddress == property1.fullAddress && t.isCurrentOwner == property1.isCurrentOwner);
                        if (propertybuyerexists == null)
                        {
                            profile.propertyOwners.Add(property1);
                        }
                    }

                //Property deeds Sellar
                DataTable dtSellar = dataSet.Tables[10];
                DataTable dtdeedSeller = dataSet.Tables[11];
               
                if (dtSellar.Rows.Count > 0)
                    foreach (DataRow dr1 in dtSellar.Rows)
                    {
                        var property1 = new PropertyDeedDetail();
                        foreach (DataRow drdeed1 in dtdeedSeller.Select("PropertyDeedId = " + dr1["PropertyDeedId"].ToString()))
                        {
                            if (drdeed1["DeedsOfficeId"] != DBNull.Value)
                            {
                                var data11 = lookupLst.Where(t => t.Type == "Deeds Office Identifier" && t.Value == drdeed1["DeedsOfficeId"].ToString()).FirstOrDefault();
                                if (data11 != null)
                                {
                                    property1.deedsOfficeId = data11.Text;
                                }
                            }
                            if (drdeed1["TitleDeedNo"] != DBNull.Value)
                                property1.titleDeedNo = drdeed1["TitleDeedNo"].ToString();
                            if (drdeed1["TitleDeedNoOLD"] != DBNull.Value)
                                property1.titleDeedNoOLD = drdeed1["TitleDeedNoOLD"].ToString();
                            if (drdeed1["TitleDeedFee"] != DBNull.Value)
                                property1.titleDeedFee = (Convert.ToInt32(drdeed1["TitleDeedFee"]));
                            if (drdeed1["DatePurchase"] != DBNull.Value)
                                property1.datePurchase = Convert.ToDateTime(drdeed1["DatePurchase"]);
                            if (drdeed1["DateRegister"] != DBNull.Value)
                                property1.dateRegister = Convert.ToDateTime(drdeed1["DateRegister"]);
                            if (drdeed1["PurchaseAmount"] != DBNull.Value)
                                property1.purchaseAmount = Convert.ToDecimal(drdeed1["PurchaseAmount"]);
                            if (drdeed1["StreetAddress"] != DBNull.Value)
                                property1.streetAddress = drdeed1["StreetAddress"].ToString();
                            if (drdeed1["StreetNumber"] != DBNull.Value)
                                property1.streetNumber = drdeed1["StreetNumber"].ToString();
                            if (drdeed1["StreetName"] != DBNull.Value)
                                property1.streetName = drdeed1["StreetName"].ToString();
                            if (drdeed1["StreetType"] != DBNull.Value)
                                property1.streetType = drdeed1["StreetType"].ToString();
                            if (drdeed1["X"] != DBNull.Value)
                                property1.x = Convert.ToDecimal(drdeed1["X"]);
                            if (drdeed1["Y"] != DBNull.Value)
                                property1.y = Convert.ToDecimal(drdeed1["Y"]);
                            if (drdeed1["SuburbCode"] != DBNull.Value)
                                property1.suburbCode = drdeed1["SuburbCode"].ToString();
                            if (drdeed1["SuburbDeeds"] != DBNull.Value)
                                property1.suburbDeeds = drdeed1["SuburbDeeds"].ToString();
                            if (drdeed1["Town"] != DBNull.Value)
                                property1.town = drdeed1["Town"].ToString();
                            if (drdeed1["Authority"] != DBNull.Value)
                                property1.authority = drdeed1["Authority"].ToString();
                            if (drdeed1["MunicipalityName"] != DBNull.Value)
                                property1.municipalityName = drdeed1["MunicipalityName"].ToString();
                            if (drdeed1["ProvinceId"] != DBNull.Value)
                            {

                                var data = lookupLst.Where(t => t.Type == "Province Identifier" && t.Value == drdeed1["ProvinceId"].ToString()).FirstOrDefault();
                                if (data != null)
                                {
                                    property1.provinceId = data.Text;

                                }
                            }
                            if (drdeed1["IsCurrOwnerUpdated"] != DBNull.Value)
                            {
                                var data7 = lookupLst.Where(t => t.Type == "Is Current Owner Updated" && t.Value == drdeed1["IsCurrOwnerUpdated"].ToString()).FirstOrDefault();
                                if (data7 != null)
                                {
                                    property1.isCurrOwnerUpdated = data7.Text;
                                }
                            }
                            if (drdeed1["Extent"] != DBNull.Value)
                                property1.extent = drdeed1["Extent"].ToString();
                            if (drdeed1["AttorneyFirmNumber"] != DBNull.Value)
                                property1.attorneyFirmNumber = drdeed1["AttorneyFirmNumber"].ToString();
                            if (drdeed1["AttorneyFileNumber"] != DBNull.Value)
                                property1.attorneyFileNumber = drdeed1["AttorneyFileNumber"].ToString();
                            if (drdeed1["TransferSeqNo"] != DBNull.Value)
                                property1.transferSeqNo = Convert.ToInt32(drdeed1["TransferSeqNo"]);
                            if (drdeed1["DateCaptured"] != DBNull.Value)
                                property1.dateCaptured = Convert.ToDateTime(drdeed1["DateCaptured"]);
                            if (drdeed1["BondNumber"] != DBNull.Value)
                                property1.bondNumber = drdeed1["BondNumber"].ToString();
                            if (drdeed1["BondHolder"] != DBNull.Value)
                                property1.bondHolder = drdeed1["BondHolder"].ToString();
                            if (drdeed1["BondAmount"] != DBNull.Value)
                                property1.bondAmount = Convert.ToInt64(drdeed1["BondAmount"]);
                            if (drdeed1["PropertyType"] != DBNull.Value)
                            {
                                var data5 = lookupLst.Where(t => t.Type == "Property Type" && t.Value == drdeed1["PropertyType"].ToString()).FirstOrDefault();
                                if (data5 != null)
                                {
                                    property1.propertyType = data5.Text;

                                }
                            }
                            if (drdeed1["PropertyName"] != DBNull.Value)
                                property1.propertyName = drdeed1["PropertyName"].ToString();
                            if (drdeed1["SchemeId"] != DBNull.Value)
                                property1.schemeId = drdeed1["SchemeId"].ToString();
                            if (drdeed1["SuburbId"] != DBNull.Value)
                                property1.suburbId = Convert.ToInt16(drdeed1["SuburbId"]);
                            if (drdeed1["Erf"] != DBNull.Value)
                                property1.erf = drdeed1["Erf"].ToString();
                            if (drdeed1["Portion"] != DBNull.Value)
                                property1.portion = Convert.ToInt32(drdeed1["Portion"]);
                            if (drdeed1["Unit"] != DBNull.Value)
                                property1.unit = Convert.ToInt32(drdeed1["Unit"]);
                            if (drdeed1["CreatedOndate"] != DBNull.Value)
                                property1.createdOndate = Convert.ToDateTime(drdeed1["CreatedOndate"]);
                            if (drdeed1["ErfSize"] != DBNull.Value)
                                property1.erfSize = drdeed1["ErfSize"].ToString();
                            if (drdeed1["StandNo"] != DBNull.Value)
                                property1.standNo = drdeed1["StandNo"].ToString();
                            if (drdeed1["PortionNo"] != DBNull.Value)
                                property1.portionNo = drdeed1["PortionNo"].ToString();
                            if (drdeed1["TownShipNo"] != DBNull.Value)
                                property1.townShipNo = Convert.ToInt32(drdeed1["TownShipNo"]);
                            if (drdeed1["PrevExtent"] != DBNull.Value)
                                property1.prevExtent = drdeed1["PrevExtent"].ToString();
                            if (drdeed1["IsCurrentOwner"] != DBNull.Value)
                                property1.isCurrentOwner = Convert.ToBoolean(drdeed1["IsCurrentOwner"]);
                            if (drdeed1["PropertyDeedID"] != DBNull.Value)
                                property1.propertyDeedId = Convert.ToInt32(drdeed1["PropertyDeedID"]);
                            if (drdeed1["StreetAddress"] != DBNull.Value)
                                property1.address = drdeed1["StreetAddress"].ToString();

                        }


                        if (dr1["SellerID"] != DBNull.Value)
                            property1.buyerid = dr1["SellerID"].ToString();
                        if (dr1["SellerIDNO"] != DBNull.Value)
                            property1.buyerIDNO = dr1["SellerIDNO"].ToString();
                        if (dr1["SellerName"] != DBNull.Value)
                            property1.buyerName = dr1["SellerName"].ToString();
                        if (dr1["SellerType"] != DBNull.Value)
                        {

                            var data1 = lookupLst.Where(t => t.Type == "Buyer Type" && t.Value == dr1["SellerType"].ToString()).FirstOrDefault();
                            if (data1 != null)
                            {
                                property1.buyerType = data1.Text;

                            }
                        }
                        property1.fullAddress = "";
                        if (!string.IsNullOrEmpty(property1.streetNumber))
                            property1.fullAddress += property1.streetNumber;

                        if (!string.IsNullOrEmpty(property1.streetName))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.streetName;
                        }
                        if (!string.IsNullOrEmpty(property1.streetType))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.streetType;
                        }

                        if (!string.IsNullOrEmpty(property1.town))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.town;
                        }

                        property1.buyerStatus = dr1["SellerStatus"].ToString();
                        property1.type = "Seller";

                        var propertybuyerexists = profile.propertyOwners.Find(t => t.type == property1.type && t.datePurchase == property1.datePurchase && t.fullAddress == property1.fullAddress && t.isCurrentOwner == property1.isCurrentOwner);
                        if (propertybuyerexists == null)
                        {
                            profile.propertyOwners.Add(property1);
                        }
                    }
                profile.timelines = timelines.OrderByDescending(t => t.LastupdatedDate).ToList();


            }
            else
            {

                DataTable dtcommercial = dataSet.Tables[0];
                if (dtcommercial.Rows.Count > 0)
                {
                    if (dtcommercial.Rows[0]["RegistrationNo"] != DBNull.Value)
                        profile.registrationNo =GetMaskedData(dtcommercial.Rows[0]["RegistrationNo"].ToString());
                    else
                        profile.registrationNo = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["RegistrationNoOld"] != DBNull.Value)
                        profile.registrationNoOld = GetMaskedData(dtcommercial.Rows[0]["RegistrationNoOld"].ToString());
                    else
                        profile.registrationNoOld = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["CommercialName"] != DBNull.Value)
                        profile.commercialName = GetMaskedData(dtcommercial.Rows[0]["CommercialName"].ToString());
                    else
                        profile.commercialName = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["CommercialShortName"] != DBNull.Value)
                        profile.commercialShortName = GetMaskedData(dtcommercial.Rows[0]["CommercialShortName"].ToString());
                    else
                        profile.commercialShortName = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["CommercialTranslatedName"] != DBNull.Value)
                        profile.commercialTranslatedName = GetMaskedData(dtcommercial.Rows[0]["CommercialTranslatedName"].ToString());
                    else
                        profile.commercialShortName = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["PreviousBusinessname"] != DBNull.Value)
                        profile.previousBusinessname = GetMaskedData(dtcommercial.Rows[0]["PreviousBusinessname"].ToString());
                    else
                        profile.previousBusinessname = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["RegistrationDate"] != DBNull.Value)
                        profile.registrationDate = DateTime.MinValue;
                    if (dtcommercial.Rows[0]["BusinessStartDate"] != DBNull.Value)
                        profile.businessStartDate = DateTime.MinValue;
                    if (dtcommercial.Rows[0]["FinancialYearEnd"] != DBNull.Value)
                        profile.financialYearEnd = 0;
                    if (dtcommercial.Rows[0]["FinancialEffectiveDate"] != DBNull.Value)
                        profile.financialEffectiveDate = DateTime.MinValue;
                    if (dtcommercial.Rows[0]["LastUpdatedDate"] != DBNull.Value)
                        profile.lastUpdatedDate = DateTime.MinValue;
                    //todo
                    if (dtcommercial.Rows[0]["SICCode"] != DBNull.Value)
                    {
                        var data = lookupLst.FirstOrDefault(t => t.Type == "SIC Code" && t.Value == dtcommercial.Rows[0]["SICCode"].ToString());
                        if (data != null)
                        {
                            profile.sICCode = GetMaskedData(data.Text);
                        }
                    }
                    else
                        profile.sICCode = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["BusinessDesc"] != DBNull.Value)
                        profile.businessDesc = GetMaskedData(dtcommercial.Rows[0]["BusinessDesc"].ToString().Substring(0, 10));
                    else
                        profile.businessDesc = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["CommercialStatusCode"] != DBNull.Value)
                    {
                        var data5 = lookupLst.FirstOrDefault(t => t.Type == "Status Code of Company" && t.Value == dtcommercial.Rows[0]["CommercialStatusCode"].ToString());
                        if (data5 != null)
                        {
                            profile.commercialStatusCode = GetMaskedData(data5.Text);
                        }
                    }
                    else
                        profile.commercialStatusCode = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["CommercialTypeCode"] != DBNull.Value)
                    {
                        var data6 = lookupLst.FirstOrDefault(t => t.Type == "Type Code of Company" && t.Value == dtcommercial.Rows[0]["CommercialTypeCode"].ToString());
                        if (data6 != null)
                        {
                            profile.commercialTypeCode = GetMaskedData(data6.Text);
                        }
                    }
                    else
                        profile.commercialTypeCode = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["VATNo"] != DBNull.Value)
                        profile.vATNo =000;
                    if (dtcommercial.Rows[0]["bussEmail"] != DBNull.Value)
                        profile.bussEmail = GetMaskedData(dtcommercial.Rows[0]["bussEmail"].ToString());
                    else
                        profile.bussEmail = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["BussWebsite"] != DBNull.Value)
                        profile.bussWebsite = GetMaskedData(dtcommercial.Rows[0]["BussWebsite"].ToString());
                    else
                        profile.bussWebsite = GetMaskedData("Unknown");
                    if (dtcommercial.Rows[0]["CreatedOnDate"] != DBNull.Value)
                        profile.createdOnDate = DateTime.MinValue;
                    if (dtcommercial.Rows[0]["AmountPerShare"] != DBNull.Value)
                        profile.amountPerShare = 0;
                    if (dtcommercial.Rows[0]["NoOfShares"] != DBNull.Value)
                        profile.noOfShares = 0;
                    if (dtcommercial.Rows[0]["Premium"] != DBNull.Value)
                        profile.premium = 0;
                }
                // Address

                DataTable dtaddress = dataSet.Tables[1];
                if (dtaddress.Rows.Count > 0)
                {
                    foreach (DataRow dtadd in dtaddress.Rows)
                    {
                        var address = new AddressDetail();

                        if (dtadd["AddressTypeInd"] != DBNull.Value)
                        {
                            var data1 = lookupLst.Where(t => t.Type == "Address Type Indicator" && t.Value == dtadd["AddressTypeInd"].ToString()).FirstOrDefault();
                            if (data1 != null)
                            {
                                address.addressTypeInd = data1.Text;

                            }
                        }
                        if (dtadd["CommercialAddressID"] != DBNull.Value)
                            address.id = dtadd["CommercialAddressID"].ToString();
                        if (dtadd["OriginalAddress1"] != DBNull.Value)
                            address.originalAddress1 = GetMaskedData(dtadd["OriginalAddress1"].ToString());
                        if (dtadd["OriginalAddress2"] != DBNull.Value)
                            address.originalAddress2 = GetMaskedData(dtadd["OriginalAddress2"].ToString());
                        if (dtadd["OriginalAddress3"] != DBNull.Value)
                            address.originalAddress3 = GetMaskedData(dtadd["OriginalAddress3"].ToString());
                        if (dtadd["OriginalAddress4"] != DBNull.Value)
                            address.originalAddress4 = GetMaskedData(dtadd["OriginalAddress4"].ToString());
                        if (dtadd["OriginalPostalCode"] != DBNull.Value)
                            address.originalPostalCode = GetMaskedData(dtadd["OriginalPostalCode"].ToString());
                        if (dtadd["CreatedOnDate"] != DBNull.Value)
                            address.createdOnDate = DateTime.MinValue;
                        if (dtadd["LastUpdatedDate"] != DBNull.Value)
                            address.lastUpdatedDate = DateTime.MinValue;

                        //Check & Delete

                        if (dtadd["occupantTypeInd"] != DBNull.Value)
                        {
                            var data1 = lookupLst.Where(t => t.Type == "Occupant Type Indicator" && t.Value == Convert.ToString(dtadd["OccupantTypeInd"])).FirstOrDefault();
                            if (data1 != null)
                            {
                                address.occupantTypeInd = GetMaskedData(data1.Text);

                            }
                        }

                        if (address.addressTypeInd != null && address.createdOnDate != null)
                        {
                            var addresspresent = timelines.FindAll(p => p.Type == address.addressTypeInd).ToList();

                            if (addresspresent.Count == 0)
                            {
                                TimeLine line = new TimeLine();
                                line.LastupdatedDate = address.lastUpdatedDate.Value;
                                line.Text = address.originalAddress1 + " " + address.originalAddress2 + " " + address.originalAddress3 + " " + address.originalAddress4;
                                line.TableName = "Address";
                                line.Type = address.addressTypeInd;
                                timelines.Add(line);
                            }
                        }

                        address.fullAddress = "";
                        if (!string.IsNullOrEmpty(address.originalAddress1) && (address.originalAddress1 != " "))
                            address.fullAddress += address.originalAddress1;

                        if (!string.IsNullOrEmpty(address.originalAddress2) && (address.originalAddress2 != " "))
                        {
                            if (!string.IsNullOrEmpty(address.fullAddress))
                                address.fullAddress += ", ";
                            address.fullAddress += address.originalAddress2;
                        }
                        if (!string.IsNullOrEmpty(address.originalAddress3) && (address.originalAddress3 != " "))
                        {
                            if (!string.IsNullOrEmpty(address.fullAddress))
                                address.fullAddress += ", ";
                            address.fullAddress += address.originalAddress3;
                        }

                        if (!string.IsNullOrEmpty(address.originalAddress4) && (address.originalAddress3 != " "))
                        {
                            if (!string.IsNullOrEmpty(address.fullAddress))
                                address.fullAddress += ", ";
                            address.fullAddress += address.originalAddress4;
                        }
                        if (!string.IsNullOrEmpty(address.originalPostalCode) && (address.originalPostalCode != " "))
                        {
                            if (!string.IsNullOrEmpty(address.fullAddress))
                                address.fullAddress += ", ";
                            address.fullAddress += address.originalPostalCode;
                        }


                        var addrpresent = profile.addresses.Find(m => m.originalAddress1 == address.originalAddress1 && m.originalAddress2 == address.originalAddress2 && m.originalAddress3 == address.originalAddress3
                        && m.originalAddress4 == address.originalAddress4 && m.addressTypeInd == address.addressTypeInd);
                        if (addrpresent == null)
                        {
                            profile.addresses.Add(address);
                        }
                    }
                }


                DataTable dtphone = dataSet.Tables[2];
                if (dtphone.Rows.Count > 0)
                {
                    foreach (DataRow drphone in dtphone.Rows)
                    {

                        var detail = new ContactDetail();
                        if (drphone["CommercialTelephoneID"] != DBNull.Value)
                            detail.id = Convert.ToInt64(drphone["CommercialTelephoneID"]);
                        if (drphone["TelephoneTypeInd"] != DBNull.Value)
                        {
                            var data2 = lookupLst.Where(t => t.Type == "Telephone Type Indicator" && t.Value == drphone["TelephoneTypeInd"].ToString()).FirstOrDefault();
                            if (data2 != null)
                            {
                                detail.type = data2.Text;
                            }
                        }
                        if (drphone["TelephoneCode"] != DBNull.Value)
                            detail.telephoneCode = GetMaskedData(drphone["TelephoneCode"].ToString());
                        if (drphone["CreatedOnDate"] != DBNull.Value)
                            detail.createdonDate = DateTime.MinValue;
                        if (drphone["LastUpdatedDate"] != DBNull.Value)
                            detail.lastUpdatedDate = DateTime.MinValue;
                        if (drphone["TelephoneNo"] != DBNull.Value)
                            detail.telephoneNo = GetMaskedData(drphone["TelephoneNo"].ToString());
                        detail.peopleLinked = 0;


                        var contactpresent = timelines.FindAll(p => p.Type == detail.type).ToList();


                        if (detail.telephoneNo.Length != 10)
                        {
                            detail.telephoneNo = GetMaskedData(detail.telephoneCode + detail.telephoneNo);
                        }
                        else
                        {
                            detail.telephoneNo = GetMaskedData(detail.telephoneNo);
                        }
                        if (detail.telephoneNo.Length == 10)
                        {
                            if (contactpresent.Count == 0 && detail.createdonDate != null)
                            {
                                TimeLine line = new TimeLine();
                                line.LastupdatedDate = detail.lastUpdatedDate.Value;
                                line.Text = detail.telephoneNo;
                                line.TableName = "Contacts";
                                line.Type = detail.type;
                                timelines.Add(line);
                            }
                            profile.contacts.Add(detail);
                        }
                    }
                }

                DataTable dtTelephoneCode = dataSet.Tables[3];
                if (dtTelephoneCode.Rows.Count > 0)
                {
                    foreach (ContactDetail t in profile.contacts)
                    {
                        DataRow[] dr = dtTelephoneCode.Select("Code = '" + t.telephoneCode + "'");
                        if (dr.Count() > 0)
                        {
                            if (dr[0]["Region"] != DBNull.Value)
                                t.CodeRegion = GetMaskedData(Convert.ToString(dr[0]["Region"]));
                            if (dr[0]["Type"] != DBNull.Value)
                                t.CodeType = GetMaskedData(Convert.ToString(dr[0]["Type"]));
                        }
                        else
                        {
                            t.CodeRegion = GetMaskedData("UnKnown");
                            t.CodeType = GetMaskedData("UnKnown");
                        }
                    }
                }

                DataTable dtjudge = dataSet.Tables[4];
                if (dtjudge.Rows.Count > 0)
                {
                    foreach (DataRow drjudge in dtjudge.Rows)
                    {
                        var commercialJudgement = new CommercialJudgement();
                        if (drjudge["CommercialName"] != DBNull.Value)
                            commercialJudgement.CommercialName = GetMaskedData(drjudge["CommercialName"].ToString());
                        if (drjudge["CommercialJudgmentID"] != DBNull.Value)
                            commercialJudgement.CommercialJudgmentID = Convert.ToInt32(drjudge["CommercialJudgmentID"]);
                        if (drjudge["Address1"] != DBNull.Value)
                            commercialJudgement.Address1 = GetMaskedData(drjudge["Address1"].ToString());
                        if (drjudge["Address2"] != DBNull.Value)
                            commercialJudgement.Address2 = GetMaskedData(drjudge["Address2"].ToString());
                        if (drjudge["Address3"] != DBNull.Value)
                            commercialJudgement.Address3 = GetMaskedData(drjudge["Address3"].ToString());
                        if (drjudge["Address4"] != DBNull.Value)
                            commercialJudgement.Address4 = GetMaskedData(drjudge["Address4"].ToString());
                        if (drjudge["PostalCode"] != DBNull.Value)
                            commercialJudgement.PostalCode = GetMaskedData(drjudge["PostalCode"].ToString());
                        if (drjudge["HomeTelephoneCode"] != DBNull.Value)
                            commercialJudgement.HomeTelephoneCode = GetMaskedData(drjudge["HomeTelephoneCode"].ToString());
                        if (drjudge["HomeTelephoneNo"] != DBNull.Value)
                            commercialJudgement.HomeTelephoneNo = GetMaskedData(drjudge["HomeTelephoneNo"].ToString());
                        if (drjudge["WorkTelephoneCode"] != DBNull.Value)
                            commercialJudgement.WorkTelephoneCode = GetMaskedData(drjudge["WorkTelephoneCode"].ToString());
                        if (drjudge["WorkTelephoneNo"] != DBNull.Value)
                            commercialJudgement.WorkTelephoneNo = GetMaskedData(drjudge["WorkTelephoneNo"].ToString());
                        if (drjudge["CellularNo"] != DBNull.Value)
                            commercialJudgement.CellularNo = GetMaskedData(drjudge["CellularNo"].ToString());
                        if (drjudge["FaxCode"] != DBNull.Value)
                            commercialJudgement.FaxCode = drjudge["FaxCode"].ToString();
                        if (drjudge["FaxNo"] != DBNull.Value)
                            commercialJudgement.FaxNo = GetMaskedData(drjudge["FaxNo"].ToString());
                        if (drjudge["CaseFilingDate"] != DBNull.Value)
                            commercialJudgement.CaseFilingDate = DateTime.MinValue ;
                        if (drjudge["CaseNumber"] != DBNull.Value)
                            commercialJudgement.CaseNumber = GetMaskedData(drjudge["CaseNumber"].ToString());
                        if (drjudge["CaseFilingDate"] != DBNull.Value)
                            commercialJudgement.CaseFilingDate = DateTime.MinValue;
                        if (drjudge["CaseType"] != DBNull.Value)
                            commercialJudgement.CaseType = GetMaskedData(drjudge["CaseType"].ToString());
                        if (drjudge["CaseReason"] != DBNull.Value)
                            commercialJudgement.CaseReason = GetMaskedData(drjudge["CaseReason"].ToString());
                        if (drjudge["DisputeAmt"] != DBNull.Value)
                            commercialJudgement.DisputeAmt = 0;
                        if (drjudge["CourtName"] != DBNull.Value)
                            commercialJudgement.CourtName = GetMaskedData(drjudge["CourtName"].ToString());
                        if (drjudge["CourtCity"] != DBNull.Value)
                            commercialJudgement.CourtCity = GetMaskedData(drjudge["CourtCity"].ToString());
                        if (drjudge["CourtType"] != DBNull.Value)
                            commercialJudgement.CourtType = GetMaskedData(drjudge["CourtType"].ToString());
                        if (drjudge["CourtCaseID"] != DBNull.Value)
                            commercialJudgement.CourtCaseID = 0;
                        if (drjudge["PlaintiffName"] != DBNull.Value)
                            commercialJudgement.PlaintiffName = GetMaskedData(drjudge["PlaintiffName"].ToString());
                        if (drjudge["Plaintiff1Address"] != DBNull.Value)
                            commercialJudgement.Plaintiff1Address = GetMaskedData(drjudge["Plaintiff1Address"].ToString());
                        if (drjudge["AttorneyName"] != DBNull.Value)
                            commercialJudgement.AttorneyName = GetMaskedData(drjudge["AttorneyName"].ToString());
                        if (drjudge["AttorneyTelephoneCode"] != DBNull.Value)
                            commercialJudgement.AttorneyTelephoneCode = GetMaskedData(drjudge["AttorneyTelephoneCode"].ToString());
                        if (drjudge["AttorneyTelephoneNo"] != DBNull.Value)
                            commercialJudgement.AttorneyTelephoneNo = GetMaskedData(drjudge["AttorneyTelephoneNo"].ToString());
                        if (drjudge["AttorneyFaxCode"] != DBNull.Value)
                            commercialJudgement.AttorneyFaxCode = GetMaskedData(drjudge["AttorneyFaxCode"].ToString());
                        if (drjudge["AttorneyFaxNo"] != DBNull.Value)
                            commercialJudgement.AttorneyFaxNo = GetMaskedData(drjudge["AttorneyFaxNo"].ToString());
                        if (drjudge["AttorneyAddress1"] != DBNull.Value)
                            commercialJudgement.AttorneyAddress1 = GetMaskedData(drjudge["AttorneyAddress1"].ToString());
                        if (drjudge["AttorneyAddress2"] != DBNull.Value)
                            commercialJudgement.AttorneyAddress2 = GetMaskedData(drjudge["AttorneyAddress2"].ToString());
                        if (drjudge["AttorneyAddress3"] != DBNull.Value)
                            commercialJudgement.AttorneyAddress3 = GetMaskedData(drjudge["AttorneyAddress3"].ToString());
                        if (drjudge["AttorneyAddress4"] != DBNull.Value)
                            commercialJudgement.AttorneyAddress4 = GetMaskedData(drjudge["AttorneyAddress4"].ToString());
                        if (drjudge["AttorneyPostalCode"] != DBNull.Value)
                            commercialJudgement.AttorneyPostalCode = GetMaskedData(drjudge["AttorneyPostalCode"].ToString());
                        if (drjudge["ReferenceNo"] != DBNull.Value)
                            commercialJudgement.ReferenceNo = GetMaskedData(drjudge["ReferenceNo"].ToString());
                        if (drjudge["LastUpdatedDate"] != DBNull.Value)
                            commercialJudgement.LastUpdatedDate =DateTime.MinValue;
                        if (drjudge["CreatedOnDate"] != DBNull.Value)
                            commercialJudgement.CreatedOnDate = DateTime.MinValue;
                        if (drjudge["DisputeDate"] != DBNull.Value)
                            commercialJudgement.DisputeDate = DateTime.MinValue;
                        if (drjudge["DisputeResolvedDate"] != DBNull.Value)
                            commercialJudgement.DisputeResolvedDate =DateTime.MinValue;
                        if (drjudge["Rescinded"] != DBNull.Value)
                            commercialJudgement.Rescinded =false;
                        if (drjudge["RescissionDate"] != DBNull.Value)
                            commercialJudgement.RescissionDate = DateTime.MinValue;
                        if (drjudge["RescissionReason"] != DBNull.Value)
                            commercialJudgement.RescissionReason = GetMaskedData(drjudge["RescissionReason"].ToString());
                        if (drjudge["RescindedAmount"] != DBNull.Value)
                            commercialJudgement.RescindedAmount = GetMaskedData(drjudge["RescindedAmount"].ToString());


                        var judgepresent = profile.commercialJudgements.Find(t => t.CaseNumber == commercialJudgement.CaseNumber && t.PlaintiffName == commercialJudgement.PlaintiffName);
                        if (judgepresent == null)
                        {
                            profile.commercialJudgements.Add(commercialJudgement);
                        }

                    }
                }


                DataTable dtcommdir = dataSet.Tables[5];
                DataTable dtdire = dataSet.Tables[6];
              

                if (dtcommdir.Rows.Count > 0)
                {
                    foreach (DataRow drCommercialDirector in dtcommdir.Rows)
                    {

                        var directors = new DirectorShip();

                        if (drCommercialDirector["AppointmentDate"] != DBNull.Value)
                            directors.appointmentDate = DateTime.MinValue;
                        if (drCommercialDirector["DirectorSetDate"] != DBNull.Value)
                            directors.directorSetDate = DateTime.MinValue;
                        if (drCommercialDirector["CommercialID"] != DBNull.Value)
                            directors.companyid = Convert.ToInt32(drCommercialDirector["CommercialID"]);
                        if (drCommercialDirector["CreatedOnDate"] != DBNull.Value)
                            directors.createdOnDate = DateTime.MinValue;
                        if (drCommercialDirector["MemberSize"] != DBNull.Value)
                            directors.memberSize = 0;
                        if (drCommercialDirector["MemberControlPerc"] != DBNull.Value)
                            directors.memberControlPerc = 0;
                        if (drCommercialDirector["Profession"] != DBNull.Value)
                            directors.profession = GetMaskedData(drCommercialDirector["Profession"].ToString());
                        if (drCommercialDirector["DirectorDesignationCode"] != DBNull.Value)
                        {
                            var data1 = lookupLst.FirstOrDefault(t => t.Type == "Director Designation Code" && t.Value == drCommercialDirector["DirectorDesignationCode"].ToString());
                            if (data1 != null)
                            {
                                directors.directorDesignationCode = GetMaskedData(data1.Text);

                            }
                        }
                        if (drCommercialDirector["DirectorStatusCode"] != DBNull.Value)
                        {
                            var data2 = lookupLst.FirstOrDefault(t => t.Type == "Director Status Code" && t.Value == drCommercialDirector["DirectorStatusCode"].ToString());
                            if (data2 != null)
                            {
                                directors.directorStatusCode = GetMaskedData(data2.Text);

                            }
                        }
                        if (drCommercialDirector["DirectorStatusDate"] != DBNull.Value)
                            directors.directorStatusDate = DateTime.MinValue;

                        if (drCommercialDirector["DirectorTypeCode"] != DBNull.Value)
                        {
                            var data3 = lookupLst.FirstOrDefault(t => t.Type == "Director Type Code" && t.Value == drCommercialDirector["DirectorTypeCode"].ToString());
                            if (data3 != null)
                            {
                                directors.directorTypeCode = GetMaskedData(data3.Text);

                            }
                        }
                        if (drCommercialDirector["IsRSAResidentYN"] != DBNull.Value)
                        {

                            var data4 = lookupLst.FirstOrDefault(t => t.Type == "Is RSA Resident YN" && t.Value == drCommercialDirector["IsRSAResidentYN"].ToString());
                            if (data4 != null)
                            {
                                directors.isRSAResidentYN = GetMaskedData(data4.Text);

                            }
                        }
                        if (dtcommdir.Rows.Count > 0)
                        {
                            DataRow[] drDirector = dtdire.Select("DirectorID = " + drCommercialDirector["DirectorID"].ToString());

                            if (drDirector[0]["IDNo"] != DBNull.Value)
                                directors.idNumber = GetMaskedData(drDirector[0]["IDNo"].ToString());
                            if (drDirector[0]["FirstInitial"] != DBNull.Value)
                                directors.firstInitial = GetMaskedData(drDirector[0]["FirstInitial"].ToString());
                            if (drDirector[0]["SecondInitial"] != DBNull.Value)
                                directors.secondInitial = GetMaskedData(drDirector[0]["SecondInitial"].ToString());

                            string fname = string.Empty;
                            string sname = string.Empty;

                            if (drDirector[0]["FirstName"] != DBNull.Value)
                                fname = GetMaskedData(drDirector[0]["FirstName"].ToString());
                            if (drDirector[0]["SecondName"] != DBNull.Value)
                                sname = GetMaskedData(drDirector[0]["SecondName"].ToString());
                            directors.fullName = fname + "" + sname;
                            if (drDirector[0]["Surname"] != DBNull.Value)
                                directors.surname = GetMaskedData(drDirector[0]["Surname"].ToString());
                            if (drDirector[0]["SurnamePrevious"] != DBNull.Value)
                                directors.surnamePrevious = GetMaskedData(drDirector[0]["SurnamePrevious"].ToString());
                            if (drDirector[0]["BirthDate"] != DBNull.Value)
                                directors.birthDate = DateTime.MinValue;
                            if (drDirector[0]["DirectorID"] != DBNull.Value)
                                directors.id = drDirector[0]["DirectorID"].ToString();
                            if (drDirector[0]["LastUpdatedDate"] != DBNull.Value)
                                directors.lastUpdatedDate = DateTime.MinValue;
                        }

                    
                        var directorpresent = profile.directorShips.Find(t => t.idNumber == directors.idNumber && t.appointmentDate == directors.appointmentDate);
                        if (directorpresent == null)
                        {
                            profile.directorShips.Add(directors);
                        }

                    }
                }


                DataTable dtAuditor = dataSet.Tables[7];
                
                if (dtAuditor.Rows.Count > 0)
                {
                    foreach (DataRow draud in dtAuditor.Rows)
                    {
                        var aud = new CommercialAuditorVm();

                        if (draud["AuditorName"] != DBNull.Value)
                            aud.AuditorName = GetMaskedData(draud["AuditorName"].ToString());
                        if (draud["ActStartDate"] != DBNull.Value)
                            aud.ActStartDate = DateTime.MinValue;
                        if (draud["ActEndDate"] != DBNull.Value)
                            aud.ActEndDate =DateTime.MinValue;
                        if (draud["CreatedOnDate"] != DBNull.Value)
                            aud.CreatedOnDate = DateTime.MinValue;
                        if (draud["LastUpdatedDate"] != DBNull.Value)
                            aud.LastUpdatedDate = DateTime.MinValue;
                        if (draud["ProfessionCode"] != DBNull.Value)
                        {
                            var data3 = lookupLst.Where(t => t.Type == "Profession Code" && t.Value == draud["ProfessionCode"].ToString()).FirstOrDefault();
                            if (data3 != null)
                            {
                                aud.ProfessionCode = GetMaskedData(data3.Text);

                            }
                        }
                        if (draud["AuditorTypeCode"] != DBNull.Value)
                        {
                            var data2 = lookupLst.Where(t => t.Type == "Auditor Type Code" && t.Value == draud["AuditorTypeCode"].ToString()).FirstOrDefault();
                            if (data2 != null)
                            {
                                aud.AuditorTypeCode = GetMaskedData(data2.Text);

                            }
                        }
                        if (draud["ProfessionNo"] != DBNull.Value)
                            aud.ProfessionNo = GetMaskedData(draud["ProfessionNo"].ToString());
                        if (draud["AuditorStatusCode"] != DBNull.Value)
                        {
                            var data1 = lookupLst.Where(t => t.Type == "Auditor Status Code" && t.Value == draud["AuditorStatusCode"].ToString()).FirstOrDefault();
                            {
                                aud.AuditorStatusCode = GetMaskedData(data1.Text);

                            }
                        }
                        if (draud["AuditorID"] != DBNull.Value)
                            aud.AuditorID = Convert.ToInt32(draud["AuditorID"]);
                        if (draud["CommercialAuditorID"] != DBNull.Value)
                            aud.CommercialAuditorID = Convert.ToInt32(draud["CommercialAuditorID"]);
                        if (draud["CommercialID"] != DBNull.Value)
                            aud.CommercialID = Convert.ToInt32(draud["CommercialID"]);
                     

                        var auditorpresent = profile.commercialAuditors.Find(t => t.AuditorName == aud.AuditorName && t.ProfessionNo == aud.ProfessionNo);
                        if (auditorpresent == null)
                        {
                            profile.commercialAuditors.Add(aud);
                        }
                    }
                }

                //Property deeds Buyer
                DataTable dtBuyer = dataSet.Tables[8];
                DataTable dtdeedBuyer = dataSet.Tables[9];
               
                if (dtBuyer.Rows.Count > 0)
                    foreach (DataRow dr in dtBuyer.Rows)
                    {
                        var property1 = new PropertyDeedDetail();
                        foreach (DataRow drdeed in dtdeedBuyer.Select("PropertyDeedId = " + dr["PropertyDeedId"].ToString()))
                        {
                            if (drdeed["DeedsOfficeId"] != DBNull.Value)
                            {
                                var data11 = lookupLst.Where(t => t.Type == "Deeds Office Identifier" && t.Value == drdeed["DeedsOfficeId"].ToString()).FirstOrDefault();
                                if (data11 != null)
                                {
                                    property1.deedsOfficeId = GetMaskedData(data11.Text);
                                }
                            }
                            if (drdeed["TitleDeedNo"] != DBNull.Value)
                                property1.titleDeedNo = GetMaskedData(drdeed["TitleDeedNo"].ToString());
                            if (drdeed["TitleDeedNoOLD"] != DBNull.Value)
                                property1.titleDeedNoOLD = GetMaskedData(drdeed["TitleDeedNoOLD"].ToString());
                            if (drdeed["TitleDeedFee"] != DBNull.Value)
                                property1.titleDeedFee = 000;
                            if (drdeed["DatePurchase"] != DBNull.Value)
                                property1.datePurchase = DateTime.MinValue;
                            if (drdeed["DateRegister"] != DBNull.Value)
                                property1.dateRegister =DateTime.MinValue;
                            if (drdeed["PurchaseAmount"] != DBNull.Value)
                                property1.purchaseAmount = 000;
                            if (drdeed["StreetAddress"] != DBNull.Value)
                                property1.streetAddress = GetMaskedData(drdeed["StreetAddress"].ToString());
                            if (drdeed["StreetNumber"] != DBNull.Value)
                                property1.streetNumber = GetMaskedData(drdeed["StreetNumber"].ToString());
                            if (drdeed["StreetName"] != DBNull.Value)
                                property1.streetName = GetMaskedData(drdeed["StreetName"].ToString());
                            if (drdeed["StreetType"] != DBNull.Value)
                                property1.streetType = GetMaskedData(drdeed["StreetType"].ToString());
                            if (drdeed["X"] != DBNull.Value)
                                property1.x = 0;
                            if (drdeed["Y"] != DBNull.Value)
                                property1.y = 0;
                            if (drdeed["SuburbCode"] != DBNull.Value)
                                property1.suburbCode = GetMaskedData(drdeed["SuburbCode"].ToString());
                            if (drdeed["SuburbDeeds"] != DBNull.Value)
                                property1.suburbDeeds = GetMaskedData(drdeed["SuburbDeeds"].ToString());
                            if (drdeed["Town"] != DBNull.Value)
                                property1.town = GetMaskedData(drdeed["Town"].ToString());
                            if (drdeed["Authority"] != DBNull.Value)
                                property1.authority = drdeed["Authority"].ToString();
                            if (drdeed["MunicipalityName"] != DBNull.Value)
                                property1.municipalityName = GetMaskedData(drdeed["MunicipalityName"].ToString());
                            if (drdeed["ProvinceId"] != DBNull.Value)
                            {

                                var data = lookupLst.Where(t => t.Type == "Province Identifier" && t.Value == drdeed["ProvinceId"].ToString()).FirstOrDefault();
                                if (data != null)
                                {
                                    property1.provinceId = GetMaskedData(data.Text);

                                }
                            }
                            if (drdeed["IsCurrOwnerUpdated"] != DBNull.Value)
                            {
                                var data7 = lookupLst.Where(t => t.Type == "Is Current Owner Updated" && t.Value == drdeed["IsCurrOwnerUpdated"].ToString()).FirstOrDefault();
                                if (data7 != null)
                                {
                                    property1.isCurrOwnerUpdated = GetMaskedData(data7.Text);
                                }
                            }
                            if (drdeed["Extent"] != DBNull.Value)
                                property1.extent = GetMaskedData(drdeed["Extent"].ToString());
                            if (drdeed["AttorneyFirmNumber"] != DBNull.Value)
                                property1.attorneyFirmNumber = GetMaskedData(drdeed["AttorneyFirmNumber"].ToString());
                            if (drdeed["AttorneyFileNumber"] != DBNull.Value)
                                property1.attorneyFileNumber = GetMaskedData(drdeed["AttorneyFileNumber"].ToString());
                            if (drdeed["TransferSeqNo"] != DBNull.Value)
                                property1.transferSeqNo =0000;
                            if (drdeed["DateCaptured"] != DBNull.Value)
                                property1.dateCaptured = DateTime.MinValue;
                            if (drdeed["BondNumber"] != DBNull.Value)
                                property1.bondNumber = GetMaskedData(drdeed["BondNumber"].ToString());
                            if (drdeed["BondHolder"] != DBNull.Value)
                                property1.bondHolder = GetMaskedData(drdeed["BondHolder"].ToString());
                            if (drdeed["BondAmount"] != DBNull.Value)
                                property1.bondAmount =000;
                            if (drdeed["PropertyType"] != DBNull.Value)
                            {
                                var data5 = lookupLst.Where(t => t.Type == "Property Type" && t.Value == drdeed["PropertyType"].ToString()).FirstOrDefault();
                                if (data5 != null)
                                {
                                    property1.propertyType = GetMaskedData(data5.Text);

                                }
                            }
                            if (drdeed["PropertyName"] != DBNull.Value)
                                property1.propertyName = GetMaskedData(drdeed["PropertyName"].ToString());
                            if (drdeed["SchemeId"] != DBNull.Value)
                                property1.schemeId = GetMaskedData(drdeed["SchemeId"].ToString());
                            if (drdeed["SuburbId"] != DBNull.Value)
                                property1.suburbId =0;
                            if (drdeed["Erf"] != DBNull.Value)
                                property1.erf = GetMaskedData(drdeed["Erf"].ToString());
                            if (drdeed["Portion"] != DBNull.Value)
                                property1.portion = 0;
                            if (drdeed["Unit"] != DBNull.Value)
                                property1.unit = 0;
                            if (drdeed["CreatedOndate"] != DBNull.Value)
                                property1.createdOndate = DateTime.MinValue;
                            if (drdeed["ErfSize"] != DBNull.Value)
                                property1.erfSize = GetMaskedData(drdeed["ErfSize"].ToString());
                            if (drdeed["StandNo"] != DBNull.Value)
                                property1.standNo = GetMaskedData(drdeed["StandNo"].ToString());
                            if (drdeed["PortionNo"] != DBNull.Value)
                                property1.portionNo = GetMaskedData(drdeed["PortionNo"].ToString());
                            if (drdeed["TownShipNo"] != DBNull.Value)
                                property1.townShipNo = 00;
                            if (drdeed["PrevExtent"] != DBNull.Value)
                                property1.prevExtent = GetMaskedData(drdeed["PrevExtent"].ToString());
                            if (drdeed["IsCurrentOwner"] != DBNull.Value)
                                property1.isCurrentOwner = false;
                            if (drdeed["PropertyDeedID"] != DBNull.Value)
                                property1.propertyDeedId = Convert.ToInt32(drdeed["PropertyDeedID"]);
                            if (drdeed["StreetAddress"] != DBNull.Value)
                                property1.address = GetMaskedData(drdeed["StreetAddress"].ToString());

                        }


                        if (dr["BuyerID"] != DBNull.Value)
                            property1.buyerid = dr["BuyerID"].ToString();
                        if (dr["BuyerIDNO"] != DBNull.Value)
                            property1.buyerIDNO = GetMaskedData(dr["BuyerIDNO"].ToString());
                        if (dr["BuyerName"] != DBNull.Value)
                            property1.buyerName = GetMaskedData(dr["BuyerName"].ToString());
                        if (dr["BuyerType"] != DBNull.Value)
                        {

                            var data1 = lookupLst.Where(t => t.Type == "Buyer Type" && t.Value == dr["BuyerType"].ToString()).FirstOrDefault();
                            if (data1 != null)
                            {
                                property1.buyerType =GetMaskedData(data1.Text);

                            }
                        }
                        property1.fullAddress = "";
                        if (!string.IsNullOrEmpty(property1.streetNumber))
                            property1.fullAddress += property1.streetNumber;

                        if (!string.IsNullOrEmpty(property1.streetName))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.streetName;
                        }
                        if (!string.IsNullOrEmpty(property1.streetType))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.streetType;
                        }

                        if (!string.IsNullOrEmpty(property1.town))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.town;
                        }
                        if(dr["BuyerStatus"] != DBNull.Value)
                        property1.buyerStatus = dr["BuyerStatus"].ToString();
                        if (dr["Share"] != DBNull.Value)
                        property1.share = dr["Share"].ToString();
                        property1.type = GetMaskedData("Buyer");
                        

                        var propertybuyerexists = profile.propertyOwners.Find(t => t.type == property1.type && t.datePurchase == property1.datePurchase && t.fullAddress == property1.fullAddress && t.isCurrentOwner == property1.isCurrentOwner);
                        if (propertybuyerexists == null)
                        {
                            profile.propertyOwners.Add(property1);
                        }
                    }

                //Property deeds Sellar
                DataTable dtSellar = dataSet.Tables[10];
                DataTable dtdeedSeller = dataSet.Tables[11];
                
                if (dtSellar.Rows.Count > 0)
                    foreach (DataRow dr1 in dtSellar.Rows)
                    {
                        var property1 = new PropertyDeedDetail();
                        foreach (DataRow drdeed1 in dtdeedSeller.Select("PropertyDeedId = " + dr1["PropertyDeedId"].ToString()))
                        {
                            if (drdeed1["DeedsOfficeId"] != DBNull.Value)
                            {
                                var data11 = lookupLst.Where(t => t.Type == "Deeds Office Identifier" && t.Value == drdeed1["DeedsOfficeId"].ToString()).FirstOrDefault();
                                if (data11 != null)
                                {
                                    property1.deedsOfficeId = GetMaskedData(data11.Text);
                                }
                            }
                            if (drdeed1["TitleDeedNo"] != DBNull.Value)
                                property1.titleDeedNo = GetMaskedData(drdeed1["TitleDeedNo"].ToString());
                            if (drdeed1["TitleDeedNoOLD"] != DBNull.Value)
                                property1.titleDeedNoOLD = GetMaskedData(drdeed1["TitleDeedNoOLD"].ToString());
                            if (drdeed1["TitleDeedFee"] != DBNull.Value)
                                property1.titleDeedFee = 0000;
                            if (drdeed1["DatePurchase"] != DBNull.Value)
                                property1.datePurchase = DateTime.MinValue;
                            if (drdeed1["DateRegister"] != DBNull.Value)
                                property1.dateRegister =DateTime.MinValue;
                            if (drdeed1["PurchaseAmount"] != DBNull.Value)
                                property1.purchaseAmount =0;
                            if (drdeed1["StreetAddress"] != DBNull.Value)
                                property1.streetAddress = GetMaskedData(drdeed1["StreetAddress"].ToString());
                            if (drdeed1["StreetNumber"] != DBNull.Value)
                                property1.streetNumber = GetMaskedData(drdeed1["StreetNumber"].ToString());
                            if (drdeed1["StreetName"] != DBNull.Value)
                                property1.streetName = GetMaskedData(drdeed1["StreetName"].ToString());
                            if (drdeed1["StreetType"] != DBNull.Value)
                                property1.streetType = GetMaskedData(drdeed1["StreetType"].ToString());
                            if (drdeed1["X"] != DBNull.Value)
                                property1.x = 0;
                            if (drdeed1["Y"] != DBNull.Value)
                                property1.y = 0;
                            if (drdeed1["SuburbCode"] != DBNull.Value)
                                property1.suburbCode = GetMaskedData(drdeed1["SuburbCode"].ToString());
                            if (drdeed1["SuburbDeeds"] != DBNull.Value)
                                property1.suburbDeeds = GetMaskedData(drdeed1["SuburbDeeds"].ToString());
                            if (drdeed1["Town"] != DBNull.Value)
                                property1.town = GetMaskedData(drdeed1["Town"].ToString());
                            if (drdeed1["Authority"] != DBNull.Value)
                                property1.authority = GetMaskedData(drdeed1["Authority"].ToString());
                            if (drdeed1["MunicipalityName"] != DBNull.Value)
                                property1.municipalityName = GetMaskedData(drdeed1["MunicipalityName"].ToString());
                            if (drdeed1["ProvinceId"] != DBNull.Value)
                            {

                                var data = lookupLst.Where(t => t.Type == "Province Identifier" && t.Value == drdeed1["ProvinceId"].ToString()).FirstOrDefault();
                                if (data != null)
                                {
                                    property1.provinceId = GetMaskedData(data.Text);

                                }
                            }
                            if (drdeed1["IsCurrOwnerUpdated"] != DBNull.Value)
                            {
                                var data7 = lookupLst.Where(t => t.Type == "Is Current Owner Updated" && t.Value == drdeed1["IsCurrOwnerUpdated"].ToString()).FirstOrDefault();
                                if (data7 != null)
                                {
                                    property1.isCurrOwnerUpdated = GetMaskedData(data7.Text);
                                }
                            }
                            if (drdeed1["Extent"] != DBNull.Value)
                                property1.extent = GetMaskedData(drdeed1["Extent"].ToString());
                            if (drdeed1["AttorneyFirmNumber"] != DBNull.Value)
                                property1.attorneyFirmNumber = GetMaskedData(drdeed1["AttorneyFirmNumber"].ToString());
                            if (drdeed1["AttorneyFileNumber"] != DBNull.Value)
                                property1.attorneyFileNumber = GetMaskedData(drdeed1["AttorneyFileNumber"].ToString());
                            if (drdeed1["TransferSeqNo"] != DBNull.Value)
                                property1.transferSeqNo =0;
                            if (drdeed1["DateCaptured"] != DBNull.Value)
                                property1.dateCaptured = DateTime.MinValue;
                            if (drdeed1["BondNumber"] != DBNull.Value)
                                property1.bondNumber = GetMaskedData(drdeed1["BondNumber"].ToString());
                            if (drdeed1["BondHolder"] != DBNull.Value)
                                property1.bondHolder = GetMaskedData(drdeed1["BondHolder"].ToString());
                            if (drdeed1["BondAmount"] != DBNull.Value)
                                property1.bondAmount = 00000;
                            if (drdeed1["PropertyType"] != DBNull.Value)
                            {
                                var data5 = lookupLst.Where(t => t.Type == "Property Type" && t.Value == drdeed1["PropertyType"].ToString()).FirstOrDefault();
                                if (data5 != null)
                                {
                                    property1.propertyType = GetMaskedData(data5.Text);

                                }
                            }
                            if (drdeed1["PropertyName"] != DBNull.Value)
                                property1.propertyName = GetMaskedData(drdeed1["PropertyName"].ToString());
                            if (drdeed1["SchemeId"] != DBNull.Value)
                                property1.schemeId = GetMaskedData(drdeed1["SchemeId"].ToString());
                            if (drdeed1["SuburbId"] != DBNull.Value)
                                property1.suburbId = 00;
                            if (drdeed1["Erf"] != DBNull.Value)
                                property1.erf = GetMaskedData(drdeed1["Erf"].ToString());
                            if (drdeed1["Portion"] != DBNull.Value)
                                property1.portion = 00;
                            if (drdeed1["Unit"] != DBNull.Value)
                                property1.unit = 0;
                            if (drdeed1["CreatedOndate"] != DBNull.Value)
                                property1.createdOndate = DateTime.MinValue;
                            if (drdeed1["ErfSize"] != DBNull.Value)
                                property1.erfSize = GetMaskedData(drdeed1["ErfSize"].ToString());
                            if (drdeed1["StandNo"] != DBNull.Value)
                                property1.standNo = GetMaskedData(drdeed1["StandNo"].ToString());
                            if (drdeed1["PortionNo"] != DBNull.Value)
                                property1.portionNo = GetMaskedData(drdeed1["PortionNo"].ToString());
                            if (drdeed1["TownShipNo"] != DBNull.Value)
                                property1.townShipNo = 0;
                            if (drdeed1["PrevExtent"] != DBNull.Value)
                                property1.prevExtent = GetMaskedData(drdeed1["PrevExtent"].ToString());
                            if (drdeed1["IsCurrentOwner"] != DBNull.Value)
                                property1.isCurrentOwner = false;
                            if (drdeed1["PropertyDeedID"] != DBNull.Value)
                                property1.propertyDeedId = Convert.ToInt32(drdeed1["PropertyDeedID"]);
                            if (drdeed1["StreetAddress"] != DBNull.Value)
                                property1.address = GetMaskedData(drdeed1["StreetAddress"].ToString());

                        }


                        if (dr1["SellerID"] != DBNull.Value)
                            property1.buyerid = dr1["SellerID"].ToString();
                        if (dr1["SellerIDNO"] != DBNull.Value)
                            property1.buyerIDNO = GetMaskedData(dr1["SellerIDNO"].ToString());
                        if (dr1["SellerName"] != DBNull.Value)
                            property1.buyerName = GetMaskedData(dr1["SellerName"].ToString());
                        if (dr1["SellerType"] != DBNull.Value)
                        {

                            var data1 = lookupLst.Where(t => t.Type == "Buyer Type" && t.Value == dr1["SellerType"].ToString()).FirstOrDefault();
                            if (data1 != null)
                            {
                                property1.buyerType = GetMaskedData(data1.Text);

                            }
                        }
                        property1.fullAddress = "";
                        if (!string.IsNullOrEmpty(property1.streetNumber))
                            property1.fullAddress += property1.streetNumber;

                        if (!string.IsNullOrEmpty(property1.streetName))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.streetName;
                        }
                        if (!string.IsNullOrEmpty(property1.streetType))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.streetType;
                        }

                        if (!string.IsNullOrEmpty(property1.town))
                        {
                            if (!string.IsNullOrEmpty(property1.fullAddress))
                                property1.fullAddress += ", ";
                            property1.fullAddress += property1.town;
                        }

                        property1.buyerStatus = dr1["SellerStatus"].ToString();
                        property1.type = "Seller";
                        
                        var propertybuyerexists = profile.propertyOwners.Find(t => t.type == property1.type && t.datePurchase == property1.datePurchase && t.fullAddress == property1.fullAddress && t.isCurrentOwner == property1.isCurrentOwner);
                        if (propertybuyerexists == null)
                        {
                            profile.propertyOwners.Add(property1);
                        }
                    }
            }

            return profile;

        }
        private int GetCount(string phoneNumber)
        {
            int res = _eSService.GetCommercialPhoneCount(phoneNumber);
            return res;
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

        public CommercialAuditorVm GetAuditorinfo(int AuditorId)
        {
            CommercialAuditorVm aud = new CommercialAuditorVm();
            try
            {
                SqlConnection con = (SqlConnection)_productionContext.Database.GetDbConnection();
                using (con)
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    DataSet data = new DataSet();
                    SqlCommand cmd = new SqlCommand("qspAuditorDetail", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AuditorId", AuditorId));
                    cmd.CommandTimeout = 0;
                    List<LookupData> lookuplst = _IDAScontext.LookupDatas.ToList();

                    SqlDataAdapter sDA = new SqlDataAdapter();
                    sDA.SelectCommand = cmd;

                    if (con.State != ConnectionState.Open)
                        con.Open();

                    sDA.Fill(data);

                    DataTable endr = data.Tables[0];
                    foreach (DataRow draddress in endr.Rows)
                    {
                        var auditoraddress = new AuditorAddressVM();
                        if (draddress["AddressTypeInd"] != DBNull.Value)
                        {
                            var data19 = lookuplst.Where(t => t.Type == "Address Type Indicator" && t.Value == draddress["AddressTypeInd"].ToString()).FirstOrDefault();
                            if (data19 != null)
                            {
                                auditoraddress.AddressTypeInd = data19.Text;

                            }
                        }
                        if (draddress["OriginalAddress1"] != DBNull.Value)
                            auditoraddress.OriginalAddress1 = draddress["OriginalAddress1"].ToString();
                        if (draddress["OriginalAddress2"] != DBNull.Value)
                            auditoraddress.OriginalAddress2 = draddress["OriginalAddress2"].ToString();
                        if (draddress["OriginalAddress3"] != DBNull.Value)
                            auditoraddress.OriginalAddress3 = draddress["OriginalAddress3"].ToString();
                        if (draddress["OriginalAddress4"] != DBNull.Value)
                            auditoraddress.OriginalAddress4 = draddress["OriginalAddress4"].ToString();
                        if (draddress["OriginalPostalCode"] != DBNull.Value)
                            auditoraddress.OriginalPostalCode = draddress["OriginalPostalCode"].ToString();
                        if (draddress["CreatedOnDate"] != DBNull.Value)
                            auditoraddress.CreatedOnDate = Convert.ToDateTime(draddress["CreatedOnDate"]);
                        if (draddress["LastUpdatedDate"] != DBNull.Value)
                            auditoraddress.LastUpdatedDate = Convert.ToDateTime(draddress["LastUpdatedDate"]);

                        auditoraddress.AuditorFullAddress = "";
                        if (!string.IsNullOrEmpty(auditoraddress.OriginalAddress1) && auditoraddress.OriginalAddress1 != " ")
                            auditoraddress.AuditorFullAddress += auditoraddress.OriginalAddress1;

                        if (!string.IsNullOrEmpty(auditoraddress.OriginalAddress2) && auditoraddress.OriginalAddress2 != " ")
                        {
                            if (!string.IsNullOrEmpty(auditoraddress.AuditorFullAddress))
                                auditoraddress.AuditorFullAddress += ", ";
                            auditoraddress.AuditorFullAddress += auditoraddress.OriginalAddress2;
                        }
                        if (!string.IsNullOrEmpty(auditoraddress.OriginalAddress3) && auditoraddress.OriginalAddress3 != " ")
                        {
                            if (!string.IsNullOrEmpty(auditoraddress.AuditorFullAddress))
                                auditoraddress.AuditorFullAddress += ", ";
                            auditoraddress.AuditorFullAddress += auditoraddress.OriginalAddress3;
                        }

                        if (!string.IsNullOrEmpty(auditoraddress.OriginalAddress4) && auditoraddress.OriginalAddress4 != " ")
                        {
                            if (!string.IsNullOrEmpty(auditoraddress.AuditorFullAddress))
                                auditoraddress.AuditorFullAddress += ", ";
                            auditoraddress.AuditorFullAddress += auditoraddress.OriginalAddress4;
                        }
                        if (!string.IsNullOrEmpty(auditoraddress.OriginalPostalCode) && auditoraddress.OriginalPostalCode != " ")
                        {
                            if (!string.IsNullOrEmpty(auditoraddress.AuditorFullAddress))
                                auditoraddress.AuditorFullAddress += ", ";
                            auditoraddress.AuditorFullAddress += auditoraddress.OriginalPostalCode;
                        }
                        var directoraddresspre = aud.auditoraddresess.Find(m => m.AddressTypeInd == auditoraddress.AddressTypeInd && m.OriginalAddress1 == auditoraddress.OriginalAddress1
                           && m.OriginalAddress2 == auditoraddress.OriginalAddress2 && m.OriginalAddress3 == auditoraddress.OriginalAddress3 && m.OriginalAddress4 == auditoraddress.OriginalAddress4 &&
                           m.OriginalPostalCode == auditoraddress.OriginalPostalCode);

                        if (directoraddresspre == null)
                        {
                            aud.auditoraddresess.Add(auditoraddress);
                        }

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return aud;
        }

    }


  

    public class CompanyRequest
    {
        public int Id { get; set; }
        public Guid userId { get; set; }
        public Guid customerId { get; set; }
        public string SearchType { get; set; }
        public string SearchCriteria { get; set; }
        public string InputType { get; set; }
        public bool istrailuser { get; set; }
    }
}
