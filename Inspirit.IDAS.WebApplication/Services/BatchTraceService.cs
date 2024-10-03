using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Inspirit.IDAS.Data.Production;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Inspirit.IDAS.ESData;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Inspirit.IDAS.WebApplication
{
    public class BatchTraceService
    {
        IDASDbContext _IDAScontext;
        ProductionDbContext _productionContext;
        CustomerLogService _CustomerLogService;
        ESService _eSService;
        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;
        ExcelDataPort<ExcelModelResponse> _excelDataImport = new ExcelDataPort<ExcelModelResponse>();
        ExcelDataPort<BatchTracingConsumer> _excelDataExport = new ExcelDataPort<BatchTracingConsumer>();

        public BatchTraceService(IHostingEnvironment hostingEnvironment, IDASDbContext IDAScontext, CustomerLogService CustomerLogService, ProductionDbContext productionContext, IConfiguration iconfig)
        {
            _IDAScontext = IDAScontext;
            _productionContext = productionContext;
            _CustomerLogService = CustomerLogService;
            _configuration = iconfig;
            _hostingEnvironment = hostingEnvironment;
            _eSService = new ESService(_configuration.GetSection("ElasticDBIP").GetSection("url").Value);
        }
        public async Task<BatchTraceServiceResponse> BatchTraceServiceList(BatchTracingServiceRequest request)
        {
            BatchTraceServiceResponse batchTraceServiceResponse = new BatchTraceServiceResponse();
            try
            {
                var lst = _IDAScontext.BatchTraces.Where(cu => cu.CustomerId == request.CustomerId).AsQueryable();
                int cnt = lst.Count();
                var flt = lst;
                batchTraceServiceResponse.recordsTotal = cnt;
                batchTraceServiceResponse.recordsFiltered = flt.Count();

                var data = await (from b in flt
                                  select new BatchTraceServices
                                  {
                                      ID = b.ID,
                                      ProFormaInvoiceId = b.ProFormaInvoiceId.Value == Guid.Empty ? "" : b.ProFormaInvoiceId.Value.ToString(),
                                      FileName = b.FileName,
                                      BatchNumber = "Batch_" + b.BatchNumber.ToString(),
                                      UploadDate = Convert.ToString(b.UploadDate),
                                      TotalRecords = b.TotalRecords,
                                      FoundRecords = b.FoundRecords,
                                      IsDataDownloaded = b.IsDataDownloaded,
                                      AdminCertified = b.AdminCertified
                                  }).ToListAsync();
                batchTraceServiceResponse.data = data;
            }
            catch (Exception ex) { }
            return batchTraceServiceResponse;
        }

        public async Task<BatchTraceServices> ExcelValidation(List<ExcelModelResponse> excelModelResponses)
        {
            BatchTraceServices batchTraceServices = new BatchTraceServices();
            try
            {

                batchTraceServices.TotalRecords = excelModelResponses.Count();
                var lst = excelModelResponses.Select(t => t.IDNumber).ToList();
                batchTraceServices.idnumberlst = lst;

                batchTraceServices.ExcelValidation = true;
            }
            catch (Exception ex)
            {
                batchTraceServices.ExcelValidation = false;
                WriteLog("Method Name: ExcelValidation  " + ex.Message, "Batch_ExcelValidation");
                batchTraceServices.ExcelValidation = false;
                return batchTraceServices;
            }
            return batchTraceServices;
        }
        public async Task<BatchTraceServices> FetchingData(List<string> idnumbers, string filename, Guid customerid)
        {
            BatchTraceServices batchTraceServices = new BatchTraceServices();
            try
            {
                int found = 0;
                batchTraceServices.TotalRecords = idnumbers.Count();
                var batchtrcingIDS = idnumbers.Distinct().ToList();

                if (batchtrcingIDS.Count() > 0)
                {
                    var customercode = _IDAScontext.Customers.Find(customerid);
                    int batchNumber = 0;
                    var batchTracesList = await _IDAScontext.BatchTraces.ToListAsync();
                    if (batchTracesList.Count > 0)
                    {
                        batchNumber = batchTracesList.Where(i => i.BatchNumber == batchTracesList.Max(m => m.BatchNumber))
                                                     .FirstOrDefault().BatchNumber + 1;
                    }
                    string fileName = customercode.Code + "_" + (batchNumber > 0 ? batchNumber : 1) + "_" + filename.Replace(".txt", ".xlsx");
                    //_excelDataExport.ExportDataFromDataTable(dataTable, filepath);
                    await ChartPreparationDataTable(batchtrcingIDS, batchTraceServices);
                    batchTraceServices.OutPutFileName = fileName;
                    batchTraceServices.FetchingData = true;
                }
            }
            catch (Exception ex)
            {
                batchTraceServices.FetchingData = false;
                WriteLog("Method Name: FetchingData  " + ex.Message, "Batch_FetchingData");
                return batchTraceServices;
            }
            return batchTraceServices;
        }
        public async Task<BatchTraceServices> PreparingChart(BatchTrace batchTrace)
        {
            BatchTraceServices batchTraceServices = new BatchTraceServices();
            try
            {
                batchTrace.ID = Guid.NewGuid();
                batchTrace.ProFormaInvoiceId = Guid.Empty;
                int batchNumber = 0;
                var batchTracesList = await _IDAScontext.BatchTraces.Where(c => c.CustomerId == batchTrace.CustomerId).ToListAsync();
                if (batchTracesList.Count > 0)
                {
                    batchNumber = batchTracesList.Where(i => i.CustomerId == batchTrace.CustomerId && i.BatchNumber == batchTracesList.Max(m => m.BatchNumber))
                                                 .FirstOrDefault().BatchNumber + 1;
                }
                batchTrace.BatchNumber = (batchNumber > 0 ? batchNumber : 1);
                batchTrace.UploadDate = DateTime.Now;

                if (!String.IsNullOrEmpty(batchTrace.FileName))
                {
                    _IDAScontext.BatchTraces.Add(batchTrace);
                    await _IDAScontext.SaveChangesAsync();
                }

                var data = _IDAScontext.BatchTraces.Find(batchTrace.ID);
                batchTraceServices.BatchTrace = data;

                batchTraceServices.PreparingChart = true;
            }
            catch (Exception ex)
            {
                batchTraceServices.PreparingChart = false;
                WriteLog("Method Name: PreparingChart  " + ex.Message, "Batch_PreparingChart");
                return batchTraceServices;
            }
            return batchTraceServices;
        }
        private async Task<BatchTraceServices> ChartPreparationDataTable(List<string> idnumbers, BatchTraceServices batchTraceServices)
        {
            try
            {
                BatchResponse response = new BatchResponse();
                string disidnos = string.Join(' ', idnumbers.ToArray());

                var _lookupDatas = await _IDAScontext.LookupDatas.Where(t => t.IsActive == true && t.Type == "Income Categories")
                                                    .OrderByDescending(o => o.Value).ToListAsync();

                var _lookupGender = await _IDAScontext.LookupDatas.Where(t => t.IsActive == true && t.Type == "Gender Indicator")
                                                    .OrderByDescending(o => o.Value).ToListAsync();


                response = _eSService.GetBatchTrace(idnumbers);

                batchTraceServices.ageGrouGenders = new AgeGrouGenders();
                batchTraceServices.ageGrouGenders.Male = new int[4];
                batchTraceServices.ageGrouGenders.FeMale = new int[4];

                string strTempGender = string.Empty;

                foreach (var dobrange in response.dobranges)
                {
                    strTempGender = string.Empty;
                    strTempGender = _lookupGender.Where(t => t.Value.ToUpper() == dobrange.Gender.ToUpper()).FirstOrDefault().Text.ToUpper();
                    if (strTempGender == "Male".ToUpper())
                    {
                        if (dobrange.category == "61+")
                        {
                            batchTraceServices.ageGrouGenders.Male[0] = dobrange.Count;
                        }
                        else if (dobrange.category == "41-60")
                        {
                            batchTraceServices.ageGrouGenders.Male[1] = dobrange.Count;
                        }
                        else if (dobrange.category == "35-40")
                        {
                            batchTraceServices.ageGrouGenders.Male[2] = dobrange.Count;
                        }
                        else if (dobrange.category == "18-34")
                        {
                            batchTraceServices.ageGrouGenders.Male[3] = dobrange.Count;
                        }
                    }
                    else if (strTempGender == "Female".ToUpper())
                    {
                        if (dobrange.category == "61+")
                        {
                            batchTraceServices.ageGrouGenders.FeMale[0] = dobrange.Count;
                        }
                        else if (dobrange.category == "41-60")
                        {
                            batchTraceServices.ageGrouGenders.FeMale[1] = dobrange.Count;
                        }
                        else if (dobrange.category == "35-40")
                        {
                            batchTraceServices.ageGrouGenders.FeMale[2] = dobrange.Count;
                        }
                        else if (dobrange.category == "18-34")
                        {
                            batchTraceServices.ageGrouGenders.FeMale[3] = dobrange.Count;
                        }
                    }
                }

                batchTraceServices.IncomeBrackets = new IncomeBrackets();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns = new string[_lookupDatas.Count() + 1];
                batchTraceServices.IncomeBrackets.IncomeBracketMale = new int[_lookupDatas.Count() + 1];
                batchTraceServices.IncomeBrackets.IncomeBracketFeMale = new int[_lookupDatas.Count() + 1];

                TotalRecordsAvailable totalRecordsAvailable = new TotalRecordsAvailable();
                totalRecordsAvailable.recordsvalue = "";
                totalRecordsAvailable.maritalindicator = response.maritalstatuslst.Where(t => t.Status.ToUpper() == "married".ToUpper()).Select(t => t.Count).FirstOrDefault();
                totalRecordsAvailable.deceasedindicator = Convert.ToInt32(response.descesed_count);
                totalRecordsAvailable.emailaddress = Convert.ToInt32(response.email_count);
                totalRecordsAvailable.deedsindicator = Convert.ToInt32(response.deeds_count);
                totalRecordsAvailable.directorshipindicator = Convert.ToInt32(response.director_count);
                totalRecordsAvailable.adverseindicator = Convert.ToInt32(response.adverse_count);
                batchTraceServices.totalRecordsAvailables.Add(totalRecordsAvailable);
                int i = 0;
                int j = 0;



                batchTraceServices.IncomeBrackets.IncomeBracketColumns[0] = "Unknown";
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[1] = _lookupDatas.Where(t => t.Value == "K").Select(t => t.Text).FirstOrDefault();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[2] = _lookupDatas.Where(t => t.Value == "J").Select(t => t.Text).FirstOrDefault();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[3] = _lookupDatas.Where(t => t.Value == "I").Select(t => t.Text).FirstOrDefault();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[4] = _lookupDatas.Where(t => t.Value == "H").Select(t => t.Text).FirstOrDefault();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[5] = _lookupDatas.Where(t => t.Value == "G").Select(t => t.Text).FirstOrDefault();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[6] = _lookupDatas.Where(t => t.Value == "F").Select(t => t.Text).FirstOrDefault();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[7] = _lookupDatas.Where(t => t.Value == "E").Select(t => t.Text).FirstOrDefault();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[8] = _lookupDatas.Where(t => t.Value == "D").Select(t => t.Text).FirstOrDefault();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[9] = _lookupDatas.Where(t => t.Value == "C").Select(t => t.Text).FirstOrDefault();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[10] = _lookupDatas.Where(t => t.Value == "B").Select(t => t.Text).FirstOrDefault();
                batchTraceServices.IncomeBrackets.IncomeBracketColumns[11] = _lookupDatas.Where(t => t.Value == "A").Select(t => t.Text).FirstOrDefault();

                foreach (var category in response.IncomeCategories)
                {
                    strTempGender = string.Empty;
                    strTempGender = _lookupGender.Where(t => t.Value == category.Gender).FirstOrDefault().Text.ToUpper();
                    if (category.Category.ToUpper() == "Unknown".ToUpper())
                    {

                        if (strTempGender == "Male".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketMale[0] = category.Count;
                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[0] = category.Count;
                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "K".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[1] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[1] = category.Count;

                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "J".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[2] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[2] = category.Count;

                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "I".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[3] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[3] = category.Count;
                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "H".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[4] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[4] = category.Count;

                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "G".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[5] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[5] = category.Count;

                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "F".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[6] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[6] = category.Count;

                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "E".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[7] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[7] = category.Count;

                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "D".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[8] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[8] = category.Count;

                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "C".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[9] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[9] = category.Count;

                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "B".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[10] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[10] = category.Count;

                            j++;
                        }
                    }
                    else if (category.Category.ToUpper() == "A".ToUpper())
                    {
                        if (strTempGender == "Male".ToUpper())
                        {
                            batchTraceServices.IncomeBrackets.IncomeBracketMale[11] = category.Count;

                            i++;
                        }
                        else if (strTempGender == "Female".ToUpper())
                        {

                            batchTraceServices.IncomeBrackets.IncomeBracketFeMale[11] = category.Count;

                            j++;
                        }
                    }
                }

                int fountrec = 0;

                foreach (var gender in response.genlst)
                {
                    strTempGender = string.Empty;
                    strTempGender = _lookupGender.Where(t => t.Value.ToUpper() == gender.Gender.ToUpper()).FirstOrDefault().Text.ToUpper();

                    var morrisGender = new MorrisGender();
                    if (strTempGender == "Male".ToUpper())
                    {
                        morrisGender.label = "Male";
                        morrisGender.value = gender.Count;
                        morrisGender.color = "#0710B1";
                        fountrec = fountrec + gender.Count;
                    }
                    else if (strTempGender == "Female".ToUpper())
                    {
                        morrisGender.label = "Female";
                        morrisGender.value = gender.Count;
                        morrisGender.color = "#ff80ff";
                        fountrec = fountrec + gender.Count;
                    }
                    batchTraceServices.morrisGenders.Add(morrisGender);
                }

                foreach (var status in response.maritalstatuslst)
                {

                    var morrismaritalStaus = new MorrisMaritalStaus();
                    if (status.Status.ToUpper() == "Single".ToUpper())
                    {
                        morrismaritalStaus.label = "Single";
                        morrismaritalStaus.value = status.Count;
                        morrismaritalStaus.color = "#94DE00";

                    }
                    else if (status.Status.ToUpper() == "Married".ToUpper())
                    {

                        morrismaritalStaus.label = "Married";
                        morrismaritalStaus.value = status.Count;
                        morrismaritalStaus.color = "#FF1E00";

                    }
                    else if (status.Status.ToUpper() == "Divorced".ToUpper())
                    {
                        morrismaritalStaus = new MorrisMaritalStaus();
                        morrismaritalStaus.label = "Divorced";
                        morrismaritalStaus.value = status.Count;
                        morrismaritalStaus.color = "#00767C";

                    }
                    else if (status.Status.ToUpper() == "Unknown".ToUpper())
                    {
                        morrismaritalStaus = new MorrisMaritalStaus();
                        morrismaritalStaus.label = "Unknown";
                        morrismaritalStaus.value = status.Count;
                        // morrismaritalStaus.color = "#00767C";

                    }
                    batchTraceServices.morrisMaritalStaus.Add(morrismaritalStaus);
                }
                foreach (var r in response.riskDetails)
                {
                    if (r.Count > 0)
                    {
                        var morrisRiskCategories = new MorrisRiskCategories();
                        if (r.Catagory.ToUpper() == "Low Risk".ToUpper())
                        {
                            morrisRiskCategories.label = "Low Risk";
                            morrisRiskCategories.value = r.Count;
                            morrisRiskCategories.color = "#41DB00";
                            batchTraceServices.morrisRiskCategories.Add(morrisRiskCategories);
                        }

                        if (r.Catagory.ToUpper() == "Extremely Low Risk".ToUpper())
                        {
                            morrisRiskCategories.label = "Extremely Low Risk";
                            morrisRiskCategories.value = r.Count;
                            morrisRiskCategories.color = "#05390B";
                            batchTraceServices.morrisRiskCategories.Add(morrisRiskCategories);
                        }

                        if (r.Catagory.ToUpper() == "High Risk".ToUpper())
                        {
                            morrisRiskCategories.label = "High Risk";
                            morrisRiskCategories.value = r.Count;
                            morrisRiskCategories.color = "#CF0018";
                            batchTraceServices.morrisRiskCategories.Add(morrisRiskCategories);
                        }

                        if (r.Catagory.ToUpper() == "Should Not be Contacted".ToUpper())
                        {
                            morrisRiskCategories.label = "Should Not be Contacted";
                            morrisRiskCategories.value = r.Count;
                            morrisRiskCategories.color = "#5E0390";
                            batchTraceServices.morrisRiskCategories.Add(morrisRiskCategories);
                        }

                        if (r.Catagory.ToUpper() == "Medium Risk".ToUpper())
                        {
                            morrisRiskCategories.label = "Medium Risk";
                            morrisRiskCategories.value = r.Count;
                            morrisRiskCategories.color = "#FF1800";
                            batchTraceServices.morrisRiskCategories.Add(morrisRiskCategories);
                        }
                        if (r.Catagory.ToUpper() == "Unknown".ToUpper())
                        {
                            morrisRiskCategories.label = "Unknown";
                            morrisRiskCategories.value = r.Count;
                            batchTraceServices.morrisRiskCategories.Add(morrisRiskCategories);
                        }
                    }
                }



                foreach (var alloy in response.alloyDetails)
                {
                    var alloyBreakDown = new MorrisAlloyBreakDown();
                    if (alloy.Alloycategory.ToUpper() == "Highly contactable".ToUpper())
                    {
                        alloyBreakDown.label = "Highly contactable";
                        alloyBreakDown.color = "#008000";
                        alloyBreakDown.value = alloy.Count;
                        batchTraceServices.morrisAlloyBreakDowns.Add(alloyBreakDown);
                    }
                    else if (alloy.Alloycategory.ToUpper() == "Very Good".ToUpper())
                    {
                        alloyBreakDown.label = "Very Good";
                        alloyBreakDown.color = "#86592d";
                        alloyBreakDown.value = alloy.Count;
                        batchTraceServices.morrisAlloyBreakDowns.Add(alloyBreakDown);
                    }
                    else if (alloy.Alloycategory.ToUpper() == "Fair".ToUpper())
                    {
                        alloyBreakDown.label = "Fair";
                        alloyBreakDown.color = "#e6b800";
                        alloyBreakDown.value = alloy.Count;
                        batchTraceServices.morrisAlloyBreakDowns.Add(alloyBreakDown);
                    }
                    else if (alloy.Alloycategory.ToUpper() == "Average".ToUpper())
                    {
                        alloyBreakDown.label = "Average";
                        alloyBreakDown.color = "#ff751a";
                        alloyBreakDown.value = alloy.Count;
                        batchTraceServices.morrisAlloyBreakDowns.Add(alloyBreakDown);
                    }
                    else if (alloy.Alloycategory.ToUpper() == "Poor".ToUpper())
                    {
                        alloyBreakDown.label = "Poor";
                        alloyBreakDown.color = "#6b6b47";
                        alloyBreakDown.value = alloy.Count;
                        batchTraceServices.morrisAlloyBreakDowns.Add(alloyBreakDown);
                    }
                    else if (alloy.Alloycategory.ToUpper() == "No Contact".ToUpper())
                    {
                        alloyBreakDown.label = "No Contact";
                        alloyBreakDown.color = "#ff1a1a";
                        alloyBreakDown.value = alloy.Count;
                        batchTraceServices.morrisAlloyBreakDowns.Add(alloyBreakDown);
                    }
                    else if (alloy.Alloycategory.ToUpper() == "Right Party Contact".ToUpper())
                    {
                        alloyBreakDown.label = "Right Party Contact";
                        alloyBreakDown.color = "#e600ac";
                        alloyBreakDown.value = alloy.Count;
                        batchTraceServices.morrisAlloyBreakDowns.Add(alloyBreakDown);
                    }
                }



                foreach (var r in response.provincelst)
                {
                    strTempGender = string.Empty;
                    strTempGender = _lookupGender.Where(t => t.Value.ToUpper() == r.Gender.ToUpper()).FirstOrDefault().Text.ToUpper();

                    double Male18_34 = 0;
                    double Male35_40 = 0;
                    double Male41_60 = 0;
                    double Male61_above = 0;
                    double Female18_34 = 0;
                    double Female35_40 = 0;
                    double Female41_60 = 0;
                    double Female61_above = 0;

                    if (r.range_18_34 > 0 && strTempGender == "Male".ToUpper())
                    {
                        Male18_34 = r.range_18_34;
                    }
                    if (r.range_35_40 > 0 && strTempGender == "Male".ToUpper())
                    {
                        Male35_40 = r.range_35_40;
                    }
                    if (r.range_41_60 > 0 && strTempGender == "Male".ToUpper())
                    {
                        Male41_60 = r.range_41_60;
                    }
                    if (r.range_61_above > 0 && strTempGender == "Male".ToUpper())
                    {
                        Male61_above = r.range_61_above;
                    }
                    if (r.range_18_34 > 0 && strTempGender == "Female".ToUpper())
                    {
                        Female18_34 = r.range_18_34;
                    }
                    if (r.range_35_40 > 0 && strTempGender == "Female".ToUpper())
                    {
                        Female35_40 = r.range_35_40;
                    }
                    if (r.range_41_60 > 0 && strTempGender == "Female".ToUpper())
                    {
                        Female41_60 = r.range_41_60;
                    }
                    if (r.range_61_above > 0 && strTempGender == "Female".ToUpper())
                    {
                        Female61_above = r.range_61_above;
                    }

                    var locationDistributorAgeGroup = new LocationDistributorAgeGroup()
                    {
                        state = r.ProvinceName,
                        male18_34 = (int)Male18_34,
                        male35_40 = (int)Male35_40,
                        male41_60 = (int)Male41_60,
                        maleabove61 = (int)Male61_above,
                        female18_34 = (int)Female18_34,
                        female35_40 = (int)Female35_40,
                        female41_60 = (int)Female41_60,
                        femaleabove61 = (int)Female61_above
                    };
                    batchTraceServices.locationDistributorAgeGroups.Add(locationDistributorAgeGroup);
                    batchTraceServices.idnos = disidnos;
                }

                batchTraceServices.FoundRecords = fountrec;

            }
            catch (Exception ex)
            {
                WriteLog("Method Name: ChartPreparationDataTable  " + ex.Message, "Batch_ChartPreparationDataTable");
                batchTraceServices.FetchingData = false;
                return batchTraceServices;
            }
            return batchTraceServices;
        }


        public async Task<BatchTrace> View(Guid ID)
        {
            BatchTrace batchTrace = new BatchTrace();
            try
            {
                batchTrace = await _IDAScontext.BatchTraces.FindAsync(ID);
            }
            catch (Exception ex) { }
            return batchTrace;
        }
        public async Task<Guid> AddBatchTraces(BatchTrace batchTrace)
        {
            Guid guid = Guid.Empty;
            try
            {
                BatchTrace data = new BatchTrace();
                data = await _IDAScontext.BatchTraces
                    .Where(b => b.CustomerUserID == batchTrace.CustomerUserID
                    && b.FileName == batchTrace.FileName
                    && b.IsDataDownloaded == false)
                    .FirstOrDefaultAsync();
                if (data != null)
                {
                    data.IsDataDownloaded = batchTrace.IsDataDownloaded;
                    _IDAScontext.BatchTraces.Update(data);
                }
                else
                {
                    data = new BatchTrace();
                    data.ID = Guid.NewGuid();
                    data.ProFormaInvoiceId = Guid.Empty;
                    data.ApprovedOnDate = DateTime.MinValue;
                    data.CustomerUserID = batchTrace.CustomerUserID;
                    data.FileName = batchTrace.FileName;
                    data.UploadDate = DateTime.Now;
                    data.TotalRecords = batchTrace.TotalRecords;
                    data.FoundRecords = batchTrace.FoundRecords;
                    data.IsDataDownloaded = batchTrace.IsDataDownloaded;
                    data.IdNumbers = batchTrace.IdNumbers;
                    await _IDAScontext.BatchTraces.AddAsync(data);
                }
                await _IDAScontext.SaveChangesAsync();
                guid = data.ID;
            }
            catch (System.Exception ex)
            {

            }
            return guid;
        }
        public async Task<string> CheckBatchProcessConfiguration(Guid customerUserid)
        {
            string Message = "";
            try
            {
                var batchesCount = _IDAScontext.BatchTraces.Where(ba => ba.CustomerUserID == customerUserid && ba.ProFormaInvoiceId == Guid.Empty).ToList().Count();
                var applicationSetting = await _IDAScontext.ApplicationSetting.FirstOrDefaultAsync(a => a.SettingName == "BatchProcessAllowedWithoutAction");
                if (applicationSetting != null)
                    if (Convert.ToInt32(applicationSetting.SettingValue) <= batchesCount)
                        Message = "Action need to be taken on previous batche(s) [Raise Invoice/Cancel] to process this batch.";
            }
            catch (Exception ex) { Message = ex.Message; }
            return Message;
        }
        public async Task<string> RemoveBatchTrace(Guid ID)
        {
            string FileName = "";
            try
            {
                var data = _IDAScontext.BatchTraces.Find(ID);
                FileName = data.OutPutFileName;
                _IDAScontext.BatchTraces.Remove(data);
                await _IDAScontext.SaveChangesAsync();
            }
            catch (Exception ex) { }
            return FileName;
        }
        public async Task<int> GetCredits(Guid userId, Guid customerId)
        {
            return _CustomerLogService.GetUserCredits(userId, customerId, "BatchProcess");
        }
        public async Task<BatchTrace> GetConsumer_Update(Guid Id)
        {
            BatchTrace batchTrace = new BatchTrace();
            try
            {
                batchTrace = await _IDAScontext.BatchTraces.FindAsync(Id);
                batchTrace.IsDataDownloaded = true;
                _IDAScontext.BatchTraces.Update(batchTrace);
                await _IDAScontext.SaveChangesAsync();
            }
            catch (Exception ex) { }
            return batchTrace;
        }
        public async Task<InvoiceGenResponse> GenerateInvoice(Guid userId, Guid batchId)
        {
            InvoiceGenResponse message = new InvoiceGenResponse();
            try
            {
                Subscription subscription = new Subscription();
                ProFormaInvoice ProInvoice = new ProFormaInvoice();
                ProformaInvoiceLineItem pr = new ProformaInvoiceLineItem();

                var user = _IDAScontext.CustomerUsers.Where(t => t.Id == userId).FirstOrDefault();
                var batchrecords = _IDAScontext.BatchTraces.Where(t => t.ID == batchId).FirstOrDefault();

                var product = _IDAScontext.Products.Where(t => t.BatchProduct == true && t.Status == true).FirstOrDefault();
                ProductPackageRate rate;
                ProInvoice.Remarks = "Batch no: Batch_" + batchrecords.BatchNumber;
                if (product != null)
                {
                    rate = _IDAScontext.ProductPackageRates.Where(t => t.ProductId == product.Id && batchrecords.FoundRecords >= t.MinLimit && batchrecords.FoundRecords <= t.MaxLimit && t.IsDeleted == 0).FirstOrDefault();
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
                var proformainvoices = await _IDAScontext.ProFormaInvoices.ToListAsync();
                if (proformainvoices.Count > 0)
                {
                    proformainvoiceNumber = proformainvoices.
                    Where(i => i.ProFormaInvoiceNumber == proformainvoices.Max(m => m.ProFormaInvoiceNumber))
                    .FirstOrDefault().ProFormaInvoiceNumber + 1;
                }
                ProInvoice.ProFormaInvoiceNumber = proformainvoiceNumber > 0 ? proformainvoiceNumber : 1;
                ProInvoice.ProformaDisplyNumber = new displaynumber().GenerateNumber(ProInvoice.ProFormaInvoiceNumber, "PRO", 10);

                ProInvoice.Status = "In Progress";

                ApplicationSetting setting = _IDAScontext.ApplicationSetting.Where(t => t.SettingName == "VAT %").FirstOrDefault();

                pr.ID = Guid.NewGuid();
                pr.ProformaInvoiceId = ProInvoice.ID;
                pr.ProductId = product.Id;

                pr.Quantity = batchrecords.FoundRecords;




                pr.Amount = Math.Round((pr.Quantity * pr.UnitPrice), 2);

                ProInvoice.ProformaInvoiceLineItems.Add(pr);

                ProInvoice.SubTotal = pr.Amount;
                decimal vattotal = Math.Round(pr.Amount * Convert.ToDecimal(setting.SettingValue) / 100, 2);
                ProInvoice.VatTotal = vattotal;
                ProInvoice.Total = ProInvoice.SubTotal + vattotal;

                _IDAScontext.ProFormaInvoices.Add(ProInvoice);

                BatchTrace trace = _IDAScontext.BatchTraces.Where(t => t.ID == batchId).FirstOrDefault();
                trace.ProFormaInvoiceId = ProInvoice.ID;

                //BatchProcessFileGeneration batchfilegen = new BatchProcessFileGeneration();

                //batchfilegen.BatchId = trace.ID;
                //batchfilegen.CreatedDate = DateTime.Now;
                //batchfilegen.CustomerUserId = userId;
                //batchfilegen.Executed = 0;
                //batchfilegen.ExecutedDate = null;
                //batchfilegen.Id = Guid.NewGuid();
                //batchfilegen.IdNumber = trace.IdNumbers;
                //batchfilegen.CustomerID = trace.CustomerId;

                // trace.IdNumbers = null;

                _IDAScontext.BatchTraces.Update(trace);
                //_IDAScontext.BatchProcessFileGeneration.Add(batchfilegen);



                _IDAScontext.SaveChanges();



                message.Message = "Invoice Generated";
                message.isSuccess = true;
            }

            catch (Exception ex)
            {
                message.Message = ex.Message;
                message.isSuccess = false;
            }
            return message;
        }
        private int AgeCalculator(DateTime? Dob)
        {
            DateTime Now = DateTime.Now;
            int Years = new DateTime(DateTime.Now.Subtract(Convert.ToDateTime(Dob.Value)).Ticks).Year - 1;
            return Years;
        }
        private string GetContactScore(List<ConsumerTelephoneVM> details)
        {
            string score = "DIRTY";
            int cmonths = 0;
            int wmonths = 0;
            try
            {
                var list = details.OrderByDescending(t => t.CreatedonDate).ToList();
                if (list.Count >= 2)
                {
                    var cellnumber = list.Find(m => m.TelephoneTypeInd == 3);
                    var worknumber = list.Find(m => m.TelephoneTypeInd == 2 || m.TelephoneTypeInd == 1);
                    if (cellnumber != null && DateTime.Today > cellnumber.CreatedonDate)
                    {
                        cmonths = (((DateTime.Today.Year - cellnumber.CreatedonDate.Year) * 12) + (DateTime.Today.Month - cellnumber.CreatedonDate.Month));
                    }
                    if (worknumber != null && DateTime.Today > worknumber.CreatedonDate)
                    {
                        wmonths = (((DateTime.Today.Year - worknumber.CreatedonDate.Year) * 12) + (DateTime.Today.Month - worknumber.CreatedonDate.Month));
                    }
                    if (cmonths != 0 && wmonths != 0)
                    {
                        if (cmonths < 6 && wmonths < 3)
                        {
                            score = "PLATINUM";
                        }
                        else if (cmonths < 12 && wmonths < 6)
                        {
                            score = "GOLD";
                        }
                        else if (cmonths < 24 && wmonths < 12)
                        {
                            score = "SILVER";
                        }
                        else if (cmonths < 48 || wmonths < 48)
                        {
                            score = "BRONZE";
                        }
                        else
                            score = "COPPER";
                    }
                    else if (cmonths != 0 || wmonths != 0)
                    {
                        if (cmonths < 48 || wmonths < 48)
                        {
                            score = "BRONZE";
                        }
                        else
                            score = "COPPER";
                    }
                }
                else if (list.Count == 1)
                {
                    var contact = list.FirstOrDefault();
                    if (contact.CreatedonDate < DateTime.Today)
                    {
                        int months = DateTime.Today.Month - contact.CreatedonDate.Month;
                        if (months < 48)
                        {
                            score = "BRONZE";
                        }
                        else
                            score = "COPPER";
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return score;
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

        public List<string> getValidIdnumbers(List<string> idnumbers)
        {
            List<string> ids = new List<string>();

            foreach (var id in ids)
            {
                bool Namevalid = Regex.IsMatch(id, @"^[0-9]+$");
                if (Namevalid == true)
                    ids.Add(id);
            }
            return ids;
        }

    }
    public class ConsumersAgeGroup
    {
        public int Gender { get; set; }
        public int Age { get; set; }
    }
    public class ConsumerTelephoneVM
    {
        public long ConsumerID { get; set; }
        public byte? TelephoneTypeInd { get; set; }
        public DateTime CreatedonDate { get; set; }
        public long ConsumerTelephoneID { get; set; }
    }
    public class BatchTracingServiceRequest
    {
        public Guid CustomerId { get; set; }
        public Guid CustomerUserId { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }
    public class BatchTraceServiceResponse : DataTableResponse<BatchTraceServices>
    {

    }
    public class BatchTracingConsumer
    {
        public string IDNUMBER { get; set; }
        public string DOB { get; set; }
        public string TITLE { get; set; }
        public string INITIALS { get; set; }
        public string NAME { get; set; }
        public string SUR_NAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string OTHER_NAMES { get; set; }
        public string SPOUSE_NAME { get; set; }
        public string PROFILE_GENDER { get; set; }
        public string PROFILE_AGE_GROUP { get; set; }
        public string PROFILE_MARITAL_STATUS { get; set; }
        public string LSM { get; set; }
        public string CONTACT_SCORE { get; set; }
        public string INCOME { get; set; }
        public string PROFILE_HOMEOWNERSHIP { get; set; }
        public string PROFILE_DIRECTORSHIP { get; set; }
        public string PROFILE_CONTACT_ABILITY { get; set; }
        public string RISK_SCORE { get; set; }
        public string JUDGE_INDICATOR { get; set; }
        public string DECEASED_IND { get; set; }
        public string X_DATEOFDEATH { get; set; }
        public string OCCUPATION { get; set; }
        public string EMPLOYMENT_DATE { get; set; }

        public string HOME_ADDRESS_LINE_1 { get; set; }
        public string HOME_ADDRESS_LINE_2 { get; set; }
        public string HOME_ADDRESS_TOWNSHIP { get; set; }
        public string HOME_ADDRESS_REGION { get; set; }
        public string HOME_ADDRESS_PROVINCE { get; set; }
        public string HOME_ADDRESS_POSTAL_CODE { get; set; }
        public string HOME_ADDRESS_DATE { get; set; }

        public string POSTAL_ADDRESS_LINE_1 { get; set; }
        public string POSTAL_ADDRESS_LINE_2 { get; set; }
        public string POSTAL_ADDRESS_TOWNSHIP { get; set; }
        public string POSTAL_ADDRESS_REGION { get; set; }
        public string POSTAL_ADDRESS_PROVINCE { get; set; }
        public string POSTAL_ADDRESS_POSTAL_CODE { get; set; }
        public string POSTAL_ADDRESS_DATE { get; set; }

        public string HOME_1_PHONE_NUMBER { get; set; }
        public string HOME_1_DATE { get; set; }
        public string WORK_1_PHONE_NUMBER { get; set; }
        public string WORK_1_DATE { get; set; }
        public string CELL_1_PHONE_NUMBER { get; set; }
        public string CELL_1_DATE { get; set; }
        public string HOME_2_PHONE_NUMBER { get; set; }
        public string HOME_2_DATE { get; set; }
        public string WORK_2_PHONE_NUMBER { get; set; }
        public string WORK_2_DATE { get; set; }
        public string CELL_2_PHONE_NUMBER { get; set; }
        public string CELL_2_DATE { get; set; }
        public string HOME_3_PHONE_NUMBER { get; set; }
        public string HOME_3_DATE { get; set; }
        public string WORK_3_PHONE_NUMBER { get; set; }
        public string WORK_3_DATE { get; set; }
        public string CELL_3_PHONE_NUMBER { get; set; }
        public string CELL_3_DATE { get; set; }
        public string HOME_4_PHONE_NUMBER { get; set; }
        public string HOME_4_DATE { get; set; }
        public string WORK_4_PHONE_NUMBER { get; set; }
        public string WORK_4_DATE { get; set; }
        public string CELL_4_PHONE_NUMBER { get; set; }
        public string CELL_4_DATE { get; set; }
        public string HOME_5_PHONE_NUMBER { get; set; }
        public string HOME_5_DATE { get; set; }
        public string WORK_5_PHONE_NUMBER { get; set; }
        public string WORK_5_DATE { get; set; }
        public string CELL_5_PHONE_NUMBER { get; set; }
        public string CELL_5_DATE { get; set; }

        public string D_O_NUMBER_TYPE_1 { get; set; }
        public string D_O_PHONE_NUMBER_1 { get; set; }
        public string D_O_DATE_1 { get; set; }
        public string D_O_NUMBER_TYPE_2 { get; set; }
        public string D_O_PHONE_NUMBER_2 { get; set; }
        public string D_O_DATE_2 { get; set; }
        public string D_O_NUMBER_TYPE_3 { get; set; }
        public string D_O_PHONE_NUMBER_3 { get; set; }
        public string D_O_DATE_3 { get; set; }
        public string D_O_NUMBER_TYPE_4 { get; set; }
        public string D_O_PHONE_NUMBER_4 { get; set; }
        public string D_O_DATE_4 { get; set; }
        public string D_O_NUMBER_TYPE_5 { get; set; }
        public string D_O_PHONE_NUMBER_5 { get; set; }
        public string D_O_DATE_5 { get; set; }
        public string D_O_NUMBER_TYPE_6 { get; set; }
        public string D_O_PHONE_NUMBER_6 { get; set; }
        public string D_O_DATE_6 { get; set; }
        public string D_O_NUMBER_TYPE_7 { get; set; }
        public string D_O_PHONE_NUMBER_7 { get; set; }
        public string D_O_DATE_7 { get; set; }
        public string D_O_NUMBER_TYPE_8 { get; set; }
        public string D_O_PHONE_NUMBER_8 { get; set; }
        public string D_O_DATE_8 { get; set; }
        public string D_O_NUMBER_TYPE_9 { get; set; }
        public string D_O_PHONE_NUMBER_9 { get; set; }
        public string D_O_DATE_9 { get; set; }
        public string D_O_NUMBER_TYPE_10 { get; set; }
        public string D_O_PHONE_NUMBER_10 { get; set; }
        public string D_O_DATE_10 { get; set; }
        public string D_O_NUMBER_TYPE_11 { get; set; }
        public string D_O_PHONE_NUMBER_11 { get; set; }
        public string D_O_DATE_11 { get; set; }
        public string D_O_NUMBER_TYPE_12 { get; set; }
        public string D_O_PHONE_NUMBER_12 { get; set; }
        public string D_O_DATE_12 { get; set; }
        public string D_O_NUMBER_TYPE_13 { get; set; }
        public string D_O_PHONE_NUMBER_13 { get; set; }
        public string D_O_DATE_13 { get; set; }
        public string D_O_NUMBER_TYPE_14 { get; set; }
        public string D_O_PHONE_NUMBER_14 { get; set; }
        public string D_O_DATE_14 { get; set; }
        public string D_O_NUMBER_TYPE_15 { get; set; }
        public string D_O_PHONE_NUMBER_15 { get; set; }
        public string D_O_DATE_15 { get; set; }

        public string X_EMAIL { get; set; }
        public string X_EMPLOYMENT_1 { get; set; }
        public string X_EMPLOYMENT_1_DATE { get; set; }
        public string X_EMPLOYMENT_2 { get; set; }
        public string X_EMPLOYMENT_2_DATE { get; set; }
        public string X_EMPLOYMENT_3 { get; set; }
        public string X_EMPLOYMENT_3_DATE { get; set; }
        public string X_EMPLOYMENT_4 { get; set; }
        public string X_EMPLOYMENT_4_DATE { get; set; }
        public string X_EMPLOYMENT_5 { get; set; }
        public string X_EMPLOYMENT_5_DATE { get; set; }
    }
    public class BatchTraceServices
    {
        public Guid BatchId { get; set; }
        public Guid ID { get; set; }
        public string ProFormaInvoiceId { get; set; }
        public Customer Customer { get; set; }
        public BatchTrace BatchTrace { get; set; }
        public Guid CustomerID { get; set; }
        public CustomerUser CustomerUser { get; set; }
        public Guid CustomerUserID { get; set; }
        public string FileName { get; set; }
        public string OutPutFileName { get; set; }
        public string BatchNumber { get; set; }
        public string UploadDate { get; set; }
        public bool ExcelValidation { get; set; }
        public bool FetchingData { get; set; }
        public bool ProcessingData { get; set; }
        public bool PreparingChart { get; set; }

        public List<MorrisGender> morrisGenders { get; set; } = new List<MorrisGender>();
        public List<MorrisMaritalStaus> morrisMaritalStaus { get; set; } = new List<MorrisMaritalStaus>();
        public List<MorrisRiskCategories> morrisRiskCategories { get; set; } = new List<MorrisRiskCategories>();
        public List<MorrisAlloyBreakDown> morrisAlloyBreakDowns { get; set; } = new List<MorrisAlloyBreakDown>();
        public List<LocationDistributorAgeGroup> locationDistributorAgeGroups { get; set; } = new List<LocationDistributorAgeGroup>();
        public AgeGrouGenders ageGrouGenders { get; set; } = new AgeGrouGenders();
        public IncomeBrackets IncomeBrackets { get; set; } = new IncomeBrackets();
        public List<TotalRecordsAvailable> totalRecordsAvailables { get; set; } = new List<TotalRecordsAvailable>();

        public int TotalRecords { get; set; }
        public int FoundRecords { get; set; }
        public bool IsDataDownloaded { get; set; }
        public bool AdminCertified { get; set; }
        public string Message { get; set; }
        public List<BatchTrace> BatchTraceList { get; set; } = new List<BatchTrace>();
        public List<Consumer> consumers { get; set; } = new List<Consumer>();
        public List<BatchTracingConsumer> batchTracingConsumers { get; set; } = new List<BatchTracingConsumer>();
        public List<string> Columns { get; set; } = new List<string>();
        public List<string> Rows { get; set; } = new List<string>();
        public List<ExcelModelResponse> excelModelResponseList { get; set; } = new List<ExcelModelResponse>();
        public string idnos { get; set; }
        public List<string> idnumberlst { get; set; } = new List<string>();
    }
    public class InvoiceGenResponse
    {
        public string Message { get; set; }
        public bool isSuccess { get; set; }
    }

    public class AgeGrouGenders
    {
        public int[] Male { get; set; }
        public int[] FeMale { get; set; }
    }
    public class IncomeBrackets
    {
        public string[] IncomeBracketColumns { get; set; }
        public int[] IncomeBracketMale { get; set; }
        public int[] IncomeBracketFeMale { get; set; }
    }
    public class TotalRecordsAvailable
    {
        public string recordsvalue { get; set; }
        public int maritalindicator { get; set; }
        public int emailaddress { get; set; }
        public int directorshipindicator { get; set; }
        public int deceasedindicator { get; set; }
        public int deedsindicator { get; set; }
        public int adverseindicator { get; set; }
    }

    public class MorrisGender
    {
        public string label { get; set; }
        public int value { get; set; }
        public string color { get; set; }
    }
    public class MorrisMaritalStaus
    {
        public string label { get; set; }
        public int value { get; set; }
        public string color { get; set; }
    }
    public class MorrisRiskCategories
    {
        public string label { get; set; }
        public int value { get; set; }
        public string color { get; set; }
    }
    public class MorrisAlloyBreakDown
    {
        public string label { get; set; }
        public int value { get; set; }
        public string color { get; set; }
    }
    public class LocationDistributorAgeGroup
    {
        public string state { get; set; }
        public int male18_34 { get; set; }
        public int male35_40 { get; set; }
        public int male41_60 { get; set; }
        public int maleabove61 { get; set; }
        public int female18_34 { get; set; }
        public int female35_40 { get; set; }
        public int female41_60 { get; set; }
        public int femaleabove61 { get; set; }
    }
}
