using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Inspirit.IDAS.ESData;
using Inspirit.IDAS.WebApplication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication
{
    public class LeadGenerationServices
    {
        ESService _eSService;
        private IConfiguration _configuration;
        IDASDbContext _dbContext;

        public LeadGenerationServices(IConfiguration iconfig, IDASDbContext context)
        {
            _configuration = iconfig;
            _eSService = new ESService(_configuration.GetSection("ElasticDBIP").GetSection("url").Value);
            _dbContext = context;
        }
        public async Task<LeadsDatatableResponse> LeadList(DatatableLeadRequest request)
        {
            LeadsDatatableResponse response = new LeadsDatatableResponse();
            try
            {
                var lst = _dbContext.LeadsGenaration.Where(cu => cu.CustomerId == request.CustomerId).AsQueryable();
                var leadfilelist = _dbContext.LeadFileGeneration.Where(t => t.CustomerID == request.CustomerId).ToList();
                int cnt = lst.Count();
                var flt = lst;
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();

                var data = await (from b in flt
                                  select new LeadListVM
                                  {
                                      Id = b.ID,
                                      ProFormaInvoiceId = b.ProFormaInvoiceId.Value == null ? "" : b.ProFormaInvoiceId.Value.ToString(),
                                      FileName = b.OutPutFileName,
                                      LeadNumber = "Lead_" + b.LeadsNumber.ToString(),
                                      LeadDate =Convert.ToString(b.LeadsDate),
                                      RequiredRecords = b.RequestedRecors,
                                      IsAdminCertified = b.AdminCertified.Value == null ? false : true,
                                      isExecuted = b.AdminCertified.Value,
                                      ProfileReport = b.ProfileReport
                                  }).ToListAsync();
                response.data = data;
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<LeadGenerationResponse> GetLeads(LeadsRequest req)
        {
            LeadGenerationResponse response = new LeadGenerationResponse();
            List<LeadsResponse> responses = new List<LeadsResponse>();

            try
            {
                List<LookupData> genderlookup = _dbContext.LookupDatas.Where(t => t.Type == "Gender Indicator").ToList();
                ESLeadRequest request = new ESLeadRequest();
                request.EmploymentLevel = req.employmentLevel;
                request.IsAdversed = req.isAdversed;
                request.IsCellNumber = req.isCellNumber;
                request.IsDeceased = req.isDeceased;
                request.IsDirector = req.isDirector;
                request.IsEmail = req.isEmail;
                request.IsEmployed = req.isEmployed;
                request.IsHomeOwner = req.isHomeOwner;

                DateTime today = DateTime.Today;
                request.DoBRange1 = new DateTime(today.Year, today.Month, today.Day).AddYears(-req.dateRange1);
                request.DoBRange2 = new DateTime(today.Year, today.Month, today.Day).AddYears(-req.dateRange2);

                foreach (var alloy in req.alloylst)
                {
                    AlloyLeads alloylead = new AlloyLeads();
                    alloylead.AlloyName = alloy.alloyName;
                    request.alloylst.Add(alloylead);
                }
                foreach (var inc in req.inclst)
                {
                    IncomeCategoryLeads inclead = new IncomeCategoryLeads();
                    inclead.IncomeCategoryName = inc.incomeCategoryName;
                    request.inclst.Add(inclead);
                }
                foreach (var lsm in req.lsmlst)
                {
                    LSMLeads lsmlead = new LSMLeads();
                    lsmlead.LsmName = lsm.lsmName;
                    request.lsmlst.Add(lsmlead);
                }
                foreach (var risk in req.risklst)
                {
                    RiskCategoryLeads risklead = new RiskCategoryLeads();
                    risklead.RiskName = risk.riskName;
                    request.risklst.Add(risklead);
                }
                req.displayDate1 = request.DoBRange1;
                req.displayDate2 = request.DoBRange2;

                foreach (var province in req.provincelst)
                {
                    ProvinceLeads pr = new ProvinceLeads();
                    pr.ProvinceName = province.provinceName;
                    request.provincelst.Add(pr);
                }


                var leads = await _eSService.GetLeadsCount(request);

                //input list
                var ProvList = req.provincelst.Select(t => t.provinceName.ToLower()).ToList();
                var genlst = req.genderlst.Select(t => t.genderName).ToList();
                var maritallst = req.statuslst.Select(t => t.statusName).ToList();

                var glist = genlst;
                genlst = new List<string>();
                foreach (var gen in glist)
                {
                    string g = genderlookup.Find(t => t.Text == gen).Value;
                    genlst.Add(g);
                }


                List<LeadsList> ReqList = new List<LeadsList>();
                if (genlst.Count() > 0)
                    ReqList = leads.Where(t => genlst.Contains(t.Gender)).ToList();
                if (maritallst.Count() > 0)
                    ReqList = ReqList.Where(t => maritallst.Contains(t.Marital)).ToList();
                var Op = ReqList.GroupBy(g => new { g.ProvinceName, g.Gender, g.Marital }).Select(t =>
                   new LeadsResponse
                   {
                       provinceName = t.Key.ProvinceName,
                       gender = genderlookup.Where(y => y.Value == t.Key.Gender).FirstOrDefault().Text,
                       marital = t.Key.Marital,
                       availableCount = t.Sum(g => g.MaritalCount)
                   }).ToList();
                response.tableresponse = Op.ToList();

                int genreq = 0;
                int provincereq = 0;
                int maritalreq = 0;
                int reqCount = 0;
                if (response.tableresponse.Count() > 0)
                {
                    foreach (var province in req.provincelst)
                    {
                        provincereq = (req.requiredLeads * province.percentage) / 100;
                        reqCount = provincereq;
                        if (req.genderlst.Count() > 0)
                        {
                            foreach (var gen in req.genderlst)
                            {
                                genreq = (provincereq * gen.percentage) / 100;
                                reqCount = genreq;
                                string genname = gen.genderName;
                                if (req.statuslst.Count() > 0)
                                {
                                    foreach (var marital in req.statuslst)
                                    {
                                        if (genreq > 0)
                                            maritalreq = (genreq * marital.percentage) / 100;
                                        else
                                            maritalreq = (provincereq * marital.percentage) / 100;

                                        reqCount = maritalreq;
                                        var isfind = response.tableresponse.Find(t => t.provinceName.ToUpper() == province.provinceName.ToUpper() && t.gender.ToUpper() == genname.ToUpper() && t.marital.ToUpper() == marital.statusName.ToUpper());
                                        if (isfind != null)
                                        {
                                            response.tableresponse.Find(t => t.provinceName.ToUpper() == province.provinceName.ToUpper() && t.gender.ToUpper() == genname.ToUpper() && t.marital.ToUpper() == marital.statusName.ToUpper()).requiredCount = reqCount;
                                        }
                                    }
                                }

                            }
                        }
                    }

                    foreach (var item in response.tableresponse)
                    {
                        if (item.availableCount > item.requiredCount)
                            item.totalSum = item.requiredCount;
                        else
                            item.totalSum = item.availableCount;
                    }

                    var cust = _dbContext.Customers.Where(t => t.Id == req.customerId).FirstOrDefault();
                    if (req.type.ToUpper() == "ADD")
                    {
                        LeadsGenaration lead = new LeadsGenaration();
                        lead.ID = Guid.NewGuid();
                        lead.InputDetail = JsonConvert.SerializeObject(req);
                        lead.AdminCertified = false;
                        lead.CustomerId = req.customerId;
                        lead.CustomerUserID = req.customerUserId;
                        lead.LeadsDate = DateTime.Now;
                        lead.RequestedRecors = req.requiredLeads;
                        var Leadslist = _dbContext.LeadsGenaration.ToList();
                        int LeadNumber = 0;
                        if (Leadslist.Count > 0)
                        {
                            LeadNumber = Leadslist.Where(i => i.CustomerId == req.customerId && i.LeadsNumber == Leadslist.Max(m => m.LeadsNumber))
                                                         .FirstOrDefault().LeadsNumber + 1;
                        }
                        lead.LeadsNumber = (LeadNumber > 0 ? LeadNumber : 1);
                        lead.OutputDetail = JsonConvert.SerializeObject(response);
                        lead.OutPutFileName = cust.Code + "_" + "Lead_" + (LeadNumber > 0 ? LeadNumber : 1) + ".xlsx";
                        lead.ProfileReport = false;
                        response.leadId = lead.ID;
                        response.leadInput = JsonConvert.SerializeObject(req);
                        response.isProfileRaised = lead.ProfileReport;
                        _dbContext.LeadsGenaration.Add(lead);
                    }
                    else if(req.type.ToUpper() == "UPDATE") {
                        var leadrec = _dbContext.LeadsGenaration.Where(t => t.ID == req.leadId).FirstOrDefault();

                        leadrec.InputDetail = JsonConvert.SerializeObject(req);
                        leadrec.RequestedRecors = req.requiredLeads;
                        leadrec.OutputDetail = JsonConvert.SerializeObject(response);
                        response.leadId = leadrec.ID;
                        response.leadInput = JsonConvert.SerializeObject(req);
                        response.isProfileRaised = leadrec.ProfileReport;
                        _dbContext.Update(leadrec);
                    }
                    _dbContext.SaveChanges();
                    
                }
                else
                {
                    response = null;
                }
            }

            catch (Exception ex)
            {
            }
            return response;
        }

        public async Task<string> RemoveLead(Guid Id)
        {
            string FileName = string.Empty;
            try
            {
                var data = _dbContext.LeadsGenaration.Find(Id);
                FileName = data.OutPutFileName;
                _dbContext.LeadsGenaration.Remove(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return FileName;
        }

        public async Task<InvoiceGenResponse> GenerateInvoice(Guid userId, Guid LeadId)
        {
            InvoiceGenResponse response = new InvoiceGenResponse();
            try
            {
                Subscription subscription = new Subscription();
                ProFormaInvoice ProInvoice = new ProFormaInvoice();
                ProformaInvoiceLineItem pr = new ProformaInvoiceLineItem();

                var user = _dbContext.CustomerUsers.Where(t => t.Id == userId).FirstOrDefault();
                var leadrecord = _dbContext.LeadsGenaration.Where(t => t.ID == LeadId).FirstOrDefault();

                var reqrecs = JsonConvert.DeserializeObject<LeadGenerationResponse>(leadrecord.OutputDetail);
                int reqrec = reqrecs.tableresponse.Sum(t => t.totalSum);

                var product = _dbContext.Products.Where(t => t.LeadProduct == true && t.Status == true).FirstOrDefault();
                ProductPackageRate rate;
                ProInvoice.Remarks = "Lead no: Lead_" + leadrecord.LeadsNumber;
                if (product != null)
                {
                    rate = _dbContext.ProductPackageRates.Where(t => t.ProductId == product.Id && reqrec >= t.MinLimit && reqrec <= t.MaxLimit && t.IsDeleted == 0).FirstOrDefault();
                    if (rate != null)
                        pr.UnitPrice = rate.UnitPrice;
                    else
                    {

                        pr.UnitPrice = 1;
                        ProInvoice.Remarks += "\n No Rate Defined";
                    }

                }
                ProInvoice.CustomerId = user.CustomerId;
                ProInvoice.Date = DateTime.Now;
                ProInvoice.EmailSendDate = DateTime.MinValue;
                ProInvoice.ID = Guid.NewGuid();
                ProInvoice.InvoiceId = null;
                ProInvoice.IsEmailSend = false;
                ProInvoice.IsProformal = true;
                ProInvoice.IsSubmitted = false;
                int proformainvoiceNumber = 0;
                var proformainvoices = await _dbContext.ProFormaInvoices.ToListAsync();
                if (proformainvoices.Count > 0)
                {
                    proformainvoiceNumber = proformainvoices.
                    Where(i => i.ProFormaInvoiceNumber == proformainvoices.Max(m => m.ProFormaInvoiceNumber))
                    .FirstOrDefault().ProFormaInvoiceNumber + 1;
                }
                ProInvoice.ProFormaInvoiceNumber = proformainvoiceNumber > 0 ? proformainvoiceNumber : 1;
                ProInvoice.ProformaDisplyNumber = new displaynumber().GenerateNumber(ProInvoice.ProFormaInvoiceNumber, "PRO", 10);

                ProInvoice.Status = "In Progress";

                ApplicationSetting setting = _dbContext.ApplicationSetting.Where(t => t.SettingName == "VAT %").FirstOrDefault();

                pr.ID = Guid.NewGuid();
                pr.ProformaInvoiceId = ProInvoice.ID;
                pr.ProductId = product.Id;

                pr.Quantity = reqrec;




                pr.Amount = Math.Round((pr.Quantity * pr.UnitPrice), 2);

                ProInvoice.ProformaInvoiceLineItems.Add(pr);

                ProInvoice.SubTotal = pr.Amount;
                decimal vattotal = Math.Round(pr.Amount * Convert.ToDecimal(setting.SettingValue) / 100, 2);
                ProInvoice.VatTotal = vattotal;
                ProInvoice.Total = ProInvoice.SubTotal + vattotal;

                _dbContext.ProFormaInvoices.Add(ProInvoice);

                LeadsGenaration lead = _dbContext.LeadsGenaration.Where(t => t.ID == LeadId).FirstOrDefault();
                lead.ProFormaInvoiceId = ProInvoice.ID;

                _dbContext.LeadsGenaration.Update(lead);
                _dbContext.SaveChanges();



                response.Message = "Invoice Generated";
                response.isSuccess = true;
            }

            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.isSuccess = false;
            }
            return response;
        }

        public async Task<string> CheckLeadProcessConfig(Guid CustomeruserId)
        {
            string Message = "";
            try
            {
                var batchesCount = _dbContext.LeadsGenaration.Where(ba => ba.CustomerUserID == CustomeruserId && ba.ProFormaInvoiceId == null).ToList().Count();
                var applicationSetting = await _dbContext.ApplicationSetting.FirstOrDefaultAsync(a => a.SettingName == "BatchProcessAllowedWithoutAction");
                if (applicationSetting != null)
                    if (Convert.ToInt32(applicationSetting.SettingValue) <= batchesCount)
                        Message = "Action need to be taken on previous Lead(s) [Raise Invoice/Cancel] to process this lead.";
            }
            catch (Exception ex) { Message = ex.Message; }
            return Message;
        }

        public async Task<LeadGenerationResponse> GetLeadsFullData(Guid LeadId)
        {
            LeadGenerationResponse response = new LeadGenerationResponse();
            try
            {
                var leaddetail = await _dbContext.LeadsGenaration.Where(t => t.ID == LeadId).FirstOrDefaultAsync();
                response.leadId = leaddetail.ID;
                response.profileAgeGroup = leaddetail.AgeGroupGenders;
                response.profileAlloyBreakdown = leaddetail.AlloyBreakDowns;
                response.profileGender = leaddetail.ProfileGender;
                response.profileIncomeBrackets = leaddetail.IncomeBrackets;
                response.profileLocationDistributorAge = leaddetail.LocationServices;
                response.profileMarital = leaddetail.MaritalStaus;
                response.profileRiskCategory = leaddetail.RiskCategories;
                response.profileTotalRecords = leaddetail.TotalRecordsAvailable;
                response.isinvoiceraised = leaddetail.ProFormaInvoiceId == null ? false : true;
                response.isProfileRaised = leaddetail.ProfileReport;

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<LeadGenerationResponse> getLeadTable(Guid LeadId)
        {
            LeadGenerationResponse response = new LeadGenerationResponse();
            try
            {
                var leadres = await _dbContext.LeadsGenaration.Where(t => t.ID == LeadId).FirstOrDefaultAsync();
                response.leadtableresponse = leadres.OutputDetail;
                response.leadId = leadres.ID;
                response.leadInput = leadres.InputDetail;
                response.isinvoiceraised = leadres.ProFormaInvoiceId == null ? false : true;
                response.isProfileRaised = leadres.ProfileReport;
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<LeadsMessage> Update(LeadGenerationResponse req)
        {
            LeadsMessage message = new LeadsMessage();
            LeadsGenaration leads = new LeadsGenaration();
            try
            {
                var lead = _dbContext.LeadsGenaration.Where(t => t.ID == req.leadId).FirstOrDefault();
                lead.OutputDetail = JsonConvert.SerializeObject(req);
                _dbContext.Update(lead);
                _dbContext.SaveChanges();
                message.isSuccess = true;
                message.Message = "Successfull";
            }
            catch (Exception ex)
            {
                message.isSuccess = false;
                message.Message = ex.Message;
            }
            return message;
        }
        public async Task<LeadGenerationResponse> GetIdno(Guid LeadId)
        {
            LeadGenerationResponse res = new LeadGenerationResponse();
            LeadsRequest request = new LeadsRequest();
            try
            {
                var leads = _dbContext.LeadsGenaration.Where(t => t.ID == LeadId).FirstOrDefault();
                JObject json = JObject.Parse(leads.OutputDetail);
                res = JsonConvert.DeserializeObject<LeadGenerationResponse>(leads.OutputDetail);
                request = JsonConvert.DeserializeObject<LeadsRequest>(leads.InputDetail);
                EsLeadInNoRequest req = new EsLeadInNoRequest();
                foreach (var alloy in request.alloylst)
                {
                    AlloyLeads alloylead = new AlloyLeads();
                    alloylead.AlloyName = alloy.alloyName;
                    req.alloylst.Add(alloylead);
                }
                req.DoBRange1 = request.displayDate1;
                req.DoBRange2 = request.displayDate2;
                foreach (var inc in request.inclst)
                {
                    IncomeCategoryLeads inclead = new IncomeCategoryLeads();
                    inclead.IncomeCategoryName = inc.incomeCategoryName;
                    req.inclst.Add(inclead);
                }
                req.IsAdversed = request.isAdversed;
                req.IsCellNumber = request.isCellNumber;
                req.IsDeceased = request.isDeceased;
                req.IsDirector = request.isDirector;
                req.IsEmail = request.isEmail;
                req.IsEmployed = request.isEmployed;
                req.IsHomeOwner = request.isHomeOwner;
                req.LeadsRequested = request.requiredLeads;
                foreach (var lsm in request.lsmlst)
                {
                    LSMLeads lsmlead = new LSMLeads();
                    lsmlead.LsmName = lsm.lsmName;
                    req.lsmlst.Add(lsmlead);
                }
                foreach (var risk in request.risklst)
                {
                    RiskCategoryLeads risklead = new RiskCategoryLeads();
                    risklead.RiskName = risk.riskName;
                    req.risklst.Add(risklead);
                }
                foreach (var table in res.tableresponse)
                {
                    LeadsResponseIdno tablelead = new LeadsResponseIdno();
                    tablelead.AvailableCount = table.availableCount;
                    tablelead.Gender = table.gender;
                    tablelead.Marital = table.marital;
                    tablelead.ProvinceName = table.provinceName;
                    tablelead.RequiredCount = table.requiredCount;
                    tablelead.TotalSum = table.totalSum;
                    req.tablelst.Add(tablelead);
                }

                var responsefromEs = _eSService.GetLeadIdNumber(req);
            }
            catch (Exception ex)
            {

            }
            return res;
        }
    }
    public class LeadGenerationResponse
    {
        public Guid leadId { get; set; }
        public string profileGender { get; set; }
        public string profileMarital { get; set; }
        public string profileRiskCategory { get; set; }
        public string profileAlloyBreakdown { get; set; }
        public string profileLocationDistributorAge { get; set; }
        public string profileAgeGroup { get; set; }
        public string profileIncomeBrackets { get; set; }
        public string profileTotalRecords { get; set; }
        public bool isinvoiceraised { get; set; }
        public bool isProfileRaised { get; set; }
        public List<LeadsResponse> tableresponse { get; set; } = new List<LeadsResponse>();
        public List<MorrisGender> morrisGenders { get; set; } = new List<MorrisGender>();
        public List<MorrisMaritalStaus> morrisMaritalStaus { get; set; } = new List<MorrisMaritalStaus>();
        public List<MorrisRiskCategories> morrisRiskCategories { get; set; } = new List<MorrisRiskCategories>();
        public List<MorrisAlloyBreakDown> morrisAlloyBreakDowns { get; set; } = new List<MorrisAlloyBreakDown>();
        public List<LocationDistributorAgeGroup> locationDistributorAgeGroups { get; set; } = new List<LocationDistributorAgeGroup>();
        public AgeGrouGenders ageGrouGenders { get; set; } = new AgeGrouGenders();
        public IncomeBrackets IncomeBrackets { get; set; } = new IncomeBrackets();
        public List<TotalRecordsAvailable> totalRecordsAvailables { get; set; } = new List<TotalRecordsAvailable>();
        public string leadtableresponse { get; set; }
        public string leadInput { get; set; }
    }
    public class LeadsResponse
    {
        public string provinceName { get; set; }
        public string gender { get; set; }
        public string marital { get; set; }
        public int requiredCount { get; set; }
        public int availableCount { get; set; }
        public int totalSum { get; set; }
    }
    public class AgeGroupGendersForLead
    {
        public int[] Male { get; set; }
        public int[] FeMale { get; set; }
    }
    public class Province
    {
        public string provinceName { get; set; }
        public int percentage { get; set; }
    }
    public class Gender
    {
        public string genderName { get; set; }
        public int percentage { get; set; }
    }
    public class MaritalStatus
    {

        public string statusName { get; set; }
        public int percentage { get; set; }
    }
    public class LSM
    {
        public string lsmName { get; set; }
        public bool ischecked { get; set; }
    }
    public class RiskCategory
    {
        public string riskName { get; set; }
        public bool ischecked { get; set; }
    }
    public class Alloy
    {
        public string alloyName { get; set; }
        public bool ischecked { get; set; }
    }
    public class IncomeCategory
    {
        public string incomeCategoryName { get; set; }
        public bool ischecked { get; set; }
    }
    public class LeadsRequest
    {
        public Guid customerId { get; set; }
        public Guid customerUserId { get; set; }
        public int requiredLeads { get; set; }
        public int dateRange1 { get; set; }
        public int dateRange2 { get; set; }
        public bool isEmployed { get; set; }
        public bool isDirector { get; set; }
        public bool isHomeOwner { get; set; }
        public bool isAdversed { get; set; }
        public bool isCellNumber { get; set; }
        public bool isEmail { get; set; }
        public string employmentLevel { get; set; }
        public bool isDeceased { get; set; }
        public DateTime displayDate1 { get; set; }
        public DateTime displayDate2 { get; set; }
        public string type { get; set; }
        public Guid leadId { get; set; }
        public List<Gender> genderlst { get; set; } = new List<Gender>();
        public List<Province> provincelst { get; set; } = new List<Province>();
        public List<MaritalStatus> statuslst { get; set; } = new List<MaritalStatus>();
        public List<Alloy> alloylst { get; set; } = new List<Alloy>();
        public List<LSM> lsmlst { get; set; } = new List<LSM>();
        public List<RiskCategory> risklst { get; set; } = new List<RiskCategory>();
        public List<IncomeCategory> inclst { get; set; } = new List<IncomeCategory>();
    }
    public class LeadsDatatableResponse : DataTableResponse<LeadListVM>
    {

    }
    public class LeadListVM
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string LeadDate { get; set; }
        public int RequiredRecords { get; set; }
        public bool isExecuted { get; set; }
        public string LeadNumber { get; set; }
        public Guid isInvoiceRaised { get; set; }
        public string ProFormaInvoiceId { get; set; }
        public bool IsAdminCertified { get; set; }
        public bool ProfileReport { get; set; }
    }
    public class ChartGenderVm
    {
        public string Gender { get; set; }
        public int GenderCount { get; set; }
        public int AvailCout { get; set; }
        public int ReqCount { get; set; }
    }
    public class ChartMartialVm
    {
        public string MaritalStatus { get; set; }
        public int MaritalCount { get; set; }
        public int AvailCout { get; set; }
        public int ReqCount { get; set; }
    }
    public class DatatableLeadRequest
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerUserId { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }
    public class LeadInput
    {
        public List<Gender> genderlst { get; set; } = new List<Gender>();
        public List<Province> provincelst { get; set; } = new List<Province>();
        public List<MaritalStatus> statuslst { get; set; } = new List<MaritalStatus>();
        public List<Alloy> alloylst { get; set; } = new List<Alloy>();
        public List<LSM> lsmlst { get; set; } = new List<LSM>();
        public List<RiskCategory> risklst { get; set; } = new List<RiskCategory>();
        public List<IncomeCategory> inclst { get; set; } = new List<IncomeCategory>();
    }
    public class LeadsMessage
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
    }
}
