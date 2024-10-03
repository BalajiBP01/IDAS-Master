using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using System.Text;
using System.Data;
using Inspirit.IDAS.Data.Production;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inspirit.IDAS.ESData
{
    public class ESService
    {

        public static Uri EsNode;
        public static ConnectionSettings EsConfig;
        public static ElasticClient client;


        //local
        //public static string _consumerdata = "testconsumer";
        //public static string _commercialdata = "testcommercial";
        //server

        public static string _commercialdata = nameof(CommercialData).ToLower();// testcommercial
        public static string _consumeraddressesinformation = nameof(ConsumerAddressInformation).ToLower();
        public static string _consumerindex = nameof(ConsumerIndex).ToLower();
        DAL dAL = new DAL();
        List<Commercialdata> regNo = new List<Commercialdata>();

        public ESService(string iPAddress)
        {
            try
            {
                EsNode = new Uri(iPAddress);

                EsConfig = new ConnectionSettings(EsNode).DisableDirectStreaming();
                client = new ElasticClient(EsConfig);

                var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };

                var indexConfig = new IndexState
                {
                    Settings = settings
                };

                if (!client.IndexExists(_consumeraddressesinformation).Exists)
                {
                    var res = client.CreateIndex("consumeraddressinformation", c => c
                     .InitializeUsing(indexConfig)
                     .Mappings(m => m.Map<ConsumerAddressInformation>(mp => mp.AutoMap()))
                    );
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ESSearchConsumerResponse> SearchConsumerAsync(ESSearchConsumerRequest search)
        {
            ESSearchConsumerResponse result = new ESSearchConsumerResponse();
            QueryContainer query = null;

            try
            {
                SortDescriptor<ConsumerIndex> sortDescriptor = new SortDescriptor<ConsumerIndex>();

                if (search.GlobalSearch != null)
                {
                    string[] splitglobal = search.GlobalSearch.Split(',');
                    foreach (var item in splitglobal)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(q => q.Query(item.Trim()).DefaultOperator(Operator.And));
                    }
                }
                else
                {
                    // krishna start pending search passport
                    //if (search.IDNumber != null) commented previous code
                    //{
                    //    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.IDNumber).Field(p => p.PassportNo)).Query(search.IDNumber).DefaultOperator(Operator.And));
                    //}

                    if (search.IDNumber != null)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.IDNumber)).Query("*" + search.IDNumber + "*").DefaultOperator(Operator.And));
                    }
                    else if (search.Passport != null)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.PassportNo)).Query("*" + search.Passport + "*").DefaultOperator(Operator.And));
                    }

                    // krishna end

                    if (search.Surname != null)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Surname)).Query(search.Surname.Trim()).DefaultOperator(Operator.And));
                    }
                    if (search.Firstname != null)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Firstname)).Query(search.Firstname.Trim()).DefaultOperator(Operator.And));
                    }
                    if (search.Address != null)
                    {
                        //removes double spacing 
                        if (search.Address.Contains(","))
                        {
                            search.Address = search.Address.Replace(",", " ");
                        }
                        if (search.Address.Contains("  "))
                        {
                            search.Address = search.Address.Replace("  ", " ");
                        }
                        string[] address = search.Address.Split(' ');
                        string add = string.Empty;

                        for (int i = 0; i < address.Count(); i++)
                        {
                            if (i == address.Count() - 1)
                                add += "(" + address[i] + ")";
                            else
                                add += "(" + address[i] + ")" + " AND ";
                        }
                        //query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Address)).Query(add).DefaultOperator(Operator.And));
                        query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Address)).Query(add).DefaultOperator(Operator.And));
                    }
                    if (search.DateOfBirth.Year > 1)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().Term(t => t.DateOfBirth, search.DateOfBirth);
                    }

                    if (search.DateOfBirthFromDate.Year > 1 && search.DateOfBirthToDate.Year > 1)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().DateRange(t => t
                                                                             .Field(u => u.DateOfBirth)
                                                                             .GreaterThanOrEquals(search.DateOfBirthFromDate)
                                                                             .LessThanOrEquals(search.DateOfBirthToDate));
                    }

                    if (search.PhoneNumber != null)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(q => q.Fields(f => f.Field(p => p.PhoneNumber)).Query(search.PhoneNumber.Trim()).DefaultOperator(Operator.And));
                    }
                    if (search.Email != null)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(q => q.Fields(f => f.Field(p => p.Email)).Query(search.Email.Trim()).DefaultOperator(Operator.And));
                    }
                    if (search.Gender != null)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().Term(t => t.Gender.ToLower(), search.Gender.ToLower());
                    }
                }
                var searchResponse = await client.SearchAsync<ConsumerIndex>(s => s
                  .Index(_consumerindex)
                  .Take(300)
                  .Query(se => query)
                 );
                List<ConsumerIndex> consumers = searchResponse.Documents.ToList();

                result.data = consumers;
                result.TotalCount = consumers.Count;
                result.TotalTime = searchResponse.Took;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //where no id number set text
            foreach (Inspirit.IDAS.ESData.ConsumerIndex rec in result.data)
            {
                if (rec.IDNumber == "")
                {
                    rec.IDNumber = "Invalid ID Number";
                }
            }

            //
            return result;
        }

        public async Task<ESSearchCommercialResponse> SearchCommercialAsync(ESSearchCommercialRequest search)
        {
            ESSearchCommercialResponse result = new ESSearchCommercialResponse();
            QueryContainer query = null;

            try
            {
                SortDescriptor<CommercialData> sortDescriptor = new SortDescriptor<CommercialData>();

                if (search.GlobalSearch != null)
                {
                    string[] splitglobal = search.GlobalSearch.Split(',');
                    foreach (var item in splitglobal)
                    {
                        query &= new QueryContainerDescriptor<CommercialData>().QueryString(q => q.Query(item.Replace("/", "").Trim()).DefaultOperator(Operator.And));
                    }
                }
                else
                {
                    if (search.CompanyRegNumber != null)
                    {
                        string regNumber = search.CompanyRegNumber.Replace("/", "");

                        if (regNumber.Length == 12)
                        {
                            //get the commercial data from the database
                            regNo = dAL.getRegistrationNo(regNumber);
                            result.data2 = regNo;
                        }
                        else
                        {
                            query &= new QueryContainerDescriptor<CommercialData>().QueryString(t => t.Query(search.CompanyRegNumber.Replace("/", "")).DefaultField(f => f.CompanyRegNo).DefaultOperator(Operator.And));
                        }
                    }
                    if (search.CompanyName != null)
                    {
                        query &= new QueryContainerDescriptor<CommercialData>().QueryString(t => t.Query(search.CompanyName).Fields(f => f.Field(j => j.CompanyName)).DefaultOperator(Operator.And));
                    }
                    if (search.CommercialStatusCode != null)
                    {
                        query &= new QueryContainerDescriptor<CommercialData>().QueryString(t => t.Query(search.CommercialStatusCode).Fields(f => f.Field(j => j.CommercialStatusCode)).DefaultOperator(Operator.And));
                    }
                    if (search.BusinessStartDate.Year > 1)
                    {
                        query &= new QueryContainerDescriptor<CommercialData>().Term(t => t.BusinessStartDate, search.BusinessStartDate);
                    }


                    if (query != null)
                    {
                        //get the commercial data from the elastic search
                        var searchResponse = await client.SearchAsync<CommercialData>(s => s
                             .Index(_commercialdata)
                             .Take(300)
                             .Query(se => query)
                            );
                        List<CommercialData> commercials = searchResponse.Documents.ToList();
                        result.data = commercials;
                        result.TotalCount = regNo.Count;
                        result.TotalTime = searchResponse.Took;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public async Task<ESSearchAddressResponse> SearchAddressAsync(ESSearchAddressRequest search)
        {
            ESSearchAddressResponse result = new ESSearchAddressResponse();
            QueryContainer query = null;
            try
            {
                if (search.Address != null)
                {
                    string[] splitglobal = search.Address.Split(' ');
                    foreach (var item in splitglobal)
                    {
                        query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Address)).Query(item).DefaultOperator(Operator.And));
                    }
                }

                var searchResponse = await client.SearchAsync<ConsumerAddressInformation>(s => s

                 .Size(300)
                 .Index(_consumeraddressesinformation)
                 .Query(m => query)
             );
                result.data = searchResponse.Documents.ToList();
                result.TotalCount = searchResponse.Total < 300 ? searchResponse.Total : 300;
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        public async Task UpsertConsumeraddressInformation(List<ConsumerAddressInformation> search)
        {
            try
            {
                var descriptor = new BulkDescriptor();
                foreach (var eachDoc in search)
                {
                    var doc = eachDoc;
                    descriptor.Index<ConsumerAddressInformation>(i => i
                        .Index(_consumeraddressesinformation)
                        .Document(doc));
                }
                var response = await client.BulkAsync(descriptor);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task UpsertCommerical(List<CommercialData> search)
        {
            var descriptor = new BulkDescriptor();
            foreach (var eachDoc in search)
            {
                var doc = eachDoc;
                descriptor.Index<CommercialData>(i => i
                    .Index(_commercialdata)
                    .Document(doc));
            }
            var response = await client.BulkAsync(descriptor);
        }

        public async Task<StringResponse> UpdsertinformationLowlevel(string strConsumerInformation, string strElasticIp)
        {
            try
            {
                var node = new System.Uri(strElasticIp);
                var settings = new ConnectionSettings(node);
                settings.RequestTimeout(TimeSpan.FromMinutes(5));
                var lowlevelClient = new ElasticLowLevelClient(settings);

                return await lowlevelClient.BulkAsync<StringResponse>(PostData.String(strConsumerInformation));
            }
            catch (Exception ex) { return new StringResponse("\"errors\":true \n" + ex.Message.ToString() + "/n" + ex.InnerException.Message); }
        }


        public async Task<List<LeadsList>> GetLeadsCount(ESLeadRequest request)
        {
            List<LeadsList> response = new List<LeadsList>();
            QueryContainer query = null;
            try
            {
                if (request.IsEmail == true)
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.EmailIndicator)).Query("1").DefaultOperator(Operator.And));
                }
                if (request.IsDirector == true)
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.DirectorIndicator)).Query("1").DefaultOperator(Operator.And));
                }
                if (request.IsAdversed == true)
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.AdverseIndicator)).Query("0").DefaultOperator(Operator.And));
                }
                if (request.IsCellNumber == true)
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.CellIndicator)).Query("1").DefaultOperator(Operator.And));
                }
                if (request.IsDeceased == true)
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.isDeceased)).Query("false").DefaultOperator(Operator.And));
                }
                if (request.IsEmployed == true)
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.EmploymentIndicator)).Query("1").DefaultOperator(Operator.And));
                }
                if (request.IsHomeOwner == true)
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.DeedsIndicator)).Query("1").DefaultOperator(Operator.And));
                }
                if (request.DoBRange1 != null && request.DoBRange2 != null)
                {

                    query &= new QueryContainerDescriptor<ConsumerIndex>().DateRange(t => t
                                                                         .Field(u => u.DateOfBirth)
                                                                         .GreaterThanOrEquals(request.DoBRange2)
                                                                        .LessThanOrEquals(request.DoBRange1));
                }
                string alloy = string.Empty;
                request.alloylst.ForEach(t => alloy = string.IsNullOrEmpty(alloy) ? "(" + t.AlloyName + ")" : alloy + " OR (" + t.AlloyName + ")");

                if (!string.IsNullOrEmpty(alloy))
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Alloy)).Query(alloy).DefaultOperator(Operator.And));
                }
                string incomecategory = String.Join(" OR ", request.inclst.Select(t => t.IncomeCategoryName).ToList());
                if (!string.IsNullOrEmpty(incomecategory))
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.IncomerCategoty)).Query(incomecategory).DefaultOperator(Operator.And));
                }
                string lsm = String.Join(" OR ", request.lsmlst.Select(t => t.LsmName).ToList());
                if (!string.IsNullOrEmpty(lsm))
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.LSM)).Query(lsm).DefaultOperator(Operator.And));
                }
                string risk = string.Empty;
                request.risklst.ForEach(t => risk = string.IsNullOrEmpty(risk) ? "(" + t.RiskName + ")" : risk + " OR (" + t.RiskName + ")");

                if (!string.IsNullOrEmpty(risk))
                {
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.RiskCategory)).Query(risk).DefaultOperator(Operator.And));
                }

                if (request.provincelst.Count() > 0)
                {
                    string Provinces = string.Empty;
                    request.provincelst.ForEach(t => Provinces = string.IsNullOrEmpty(Provinces) ? "(" + t.ProvinceName.ToLower() + ")" : Provinces + " OR (" + t.ProvinceName.ToLower() + ")");
                    query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Province)).Query(Provinces).DefaultOperator(Operator.And));
                }
                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Iscontactexists)).Query("1").DefaultOperator(Operator.And));
                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Isleadeligible)).Query("1").DefaultOperator(Operator.And));


                var searchResponse = await client.SearchAsync<ConsumerIndex>(s => s
                      .Index(_consumerindex)
                      .Query(se => query)
                                       .Aggregations(childAggs => childAggs
                                           .Terms("province_count", avg => avg.Field(p => p.Province).Size(20)
                                           .Aggregations(childAggsn => childAggsn
                                           .Terms("gen_province_count", avgs => avgs.Field(p => p.Gender)
                                               .Aggregations(childAgg => childAgg
                                               .Terms("marital_status", mar => mar.Field(p => p.MaritalStatus))
                                               ))
                                           )))
                     );
                var province_counts = searchResponse.Aggregations.Terms("province_count");

                foreach (var province in province_counts.Buckets)
                {
                    int ProvinceCount = Convert.ToInt32(province.DocCount);
                    string ProvinceName = province.Key;
                    foreach (var t in province.Values)
                    {
                        foreach (var item in ((Nest.BucketAggregate)t).Items)
                        {
                            string GenderCategory = Convert.ToString(((Nest.KeyedBucket<object>)item).Key);
                            int GenderCount = Convert.ToInt32(((Nest.KeyedBucket<object>)item).DocCount);
                            foreach (var item1 in ((Nest.IsAReadOnlyDictionaryBase<string, Nest.IAggregate>)item).Values)
                            {
                                foreach (var t1 in ((Nest.BucketAggregate)item1).Items)
                                {
                                    string MaritalStatus = Convert.ToString(((Nest.KeyedBucket<object>)t1).Key);
                                    int StatusCount = Convert.ToInt32(((Nest.KeyedBucket<object>)t1).DocCount);
                                    LeadsList lead = new LeadsList();
                                    lead.ProvinceName = ProvinceName;
                                    lead.ProvinceCount = ProvinceCount;
                                    lead.Gender = GenderCategory;
                                    lead.GenderCount = GenderCount;
                                    lead.Marital = MaritalStatus;
                                    lead.MaritalCount = StatusCount;
                                    response.Add(lead);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public string GetLeadIdNumber(EsLeadInNoRequest request)
        {

            QueryContainer query = null;
            List<ConsumerIndex> data = new List<ConsumerIndex>();
            string IdNumbers = string.Empty;
            try
            {

                var provincelst = request.tablelst.Select(t => t.ProvinceName).Distinct().ToList();
                foreach (var province in provincelst)
                {
                    var genlst = request.tablelst.Where(t => t.ProvinceName == province).Select(t => t.Gender).Distinct().ToList();
                    foreach (var gender in genlst)
                    {
                        string gen = string.Empty;
                        if (gender.ToUpper() == "Male".ToUpper())
                        {
                            gen = "1";
                        }
                        if (gender.ToUpper() == "Female".ToUpper())
                        {
                            gen = "0";
                        }
                        var maritallst = request.tablelst.Where(t => t.ProvinceName == province).Select(t => t.Marital).Distinct().ToList();
                        foreach (var status in maritallst)
                        {
                            query = new QueryContainer();

                            //province
                            query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Province)).Query(province.ToLower()).DefaultOperator(Operator.And));
                            //gender
                            query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Gender)).Query(gen).DefaultOperator(Operator.And));
                            //marital status
                            query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.MaritalStatus)).Query(status).DefaultOperator(Operator.And));
                            if (request.IsEmail == true)
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.EmailIndicator)).Query("1").DefaultOperator(Operator.And));
                            }
                            if (request.IsDirector == true)
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.DirectorIndicator)).Query("1").DefaultOperator(Operator.And));
                            }
                            if (request.IsAdversed == true)
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.AdverseIndicator)).Query("0").DefaultOperator(Operator.And));
                            }
                            if (request.IsHomeOwner == true)
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.DeedsIndicator)).Query("1").DefaultOperator(Operator.And));
                            }
                            if (request.IsCellNumber == true)
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.CellIndicator)).Query("1").DefaultOperator(Operator.And));
                            }
                            if (request.IsDeceased == true)
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.isDeceased)).Query("false").DefaultOperator(Operator.And));
                            }
                            if (request.IsEmployed == true)
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.EmploymentIndicator)).Query("1").DefaultOperator(Operator.And));
                            }
                            if (request.DoBRange1 != null && request.DoBRange2 != null)
                            {

                                query &= new QueryContainerDescriptor<ConsumerIndex>().DateRange(t => t
                                                                                     .Field(u => u.DateOfBirth)
                                                                                     .GreaterThanOrEquals(request.DoBRange2)
                                                                                    .LessThanOrEquals(request.DoBRange1));
                            }
                            string alloy = string.Empty;
                            var alloylst = request.alloylst.Select(t => t.AlloyName).ToList();
                            for (int i = 0; i < alloylst.Count(); i++)
                            {
                                if (i == alloylst.Count() - 1)
                                    alloy += "(" + alloylst[i] + ")";
                                else
                                    alloy += "(" + alloylst[i] + ")" + " OR ";
                            }
                            if (!string.IsNullOrEmpty(alloy))
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Alloy)).Query(alloy).DefaultOperator(Operator.And));
                            }
                            string incomecategory = String.Join(" OR ", request.inclst.Select(t => t.IncomeCategoryName).ToList());
                            if (!string.IsNullOrEmpty(incomecategory))
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.IncomerCategoty)).Query(incomecategory).DefaultOperator(Operator.And));
                            }
                            string lsm = String.Join(" OR ", request.lsmlst.Select(t => t.LsmName).ToList());
                            if (!string.IsNullOrEmpty(lsm))
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.LSM)).Query(lsm).DefaultOperator(Operator.And));
                            }
                            string risk = string.Empty;
                            var risklst = request.risklst.Select(t => t.RiskName).ToList();
                            for (int i = 0; i < risklst.Count(); i++)
                            {
                                if (i == risklst.Count() - 1)
                                    risk += "(" + risklst[i] + ")";
                                else
                                    risk += "(" + risklst[i] + ")" + " OR ";
                            }

                            if (!string.IsNullOrEmpty(risk))
                            {
                                query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.RiskCategory)).Query(risk).DefaultOperator(Operator.And));
                            }

                            query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Iscontactexists)).Query("1").DefaultOperator(Operator.And));
                            query &= new QueryContainerDescriptor<ConsumerIndex>().QueryString(t => t.Fields(f => f.Field(p => p.Isleadeligible)).Query("1").DefaultOperator(Operator.And));


                            int reqcount = request.tablelst.Where(t => t.ProvinceName == province && t.Marital == status && t.Gender == gender).FirstOrDefault().TotalSum;
                            int skip = 0, take = 1000;

                            while (reqcount > 0)
                            {
                                int count = reqcount > 1000 ? 1000 : reqcount;
                                var searchResponse1 = client.Search<ConsumerIndex>(s => s
                                  .Index(_consumerindex)
                                  .Query(se => query)
                                  .Skip(skip)
                                  .Take(count)
                                  );
                                if (searchResponse1.Documents.ToList().Count() > 0)
                                    data.AddRange(searchResponse1.Documents.ToList());
                                skip += take;
                                reqcount -= count;
                            }

                        }
                    }

                }
                //response.data = data;

                if (data.Count > 0)
                {
                    IdNumbers = string.Join(",", data.Select(t => t.IDNumber));
                }

            }
            catch (Exception ex)
            {

            }
            return IdNumbers;
        }

        public BatchResponse GetBatchTrace(List<string> idnos)
        {
            BatchResponse response = new BatchResponse();

            DateTime getdate = DateTime.Today;


            DateTime date1 = new DateTime(Convert.ToInt32(getdate.Year), Convert.ToInt32(getdate.Month), (Convert.ToInt32(getdate.Day)), 12, 0, 0).AddYears(-18).AddDays(-1);
            DateTime date2 = new DateTime(Convert.ToInt32(getdate.Year), Convert.ToInt32(getdate.Month), (Convert.ToInt32(getdate.Day)), 12, 0, 0).AddYears(-35).AddDays(-1);
            DateTime date3 = new DateTime(Convert.ToInt32(getdate.Year), Convert.ToInt32(getdate.Month), (Convert.ToInt32(getdate.Day)), 12, 0, 0).AddYears(-41).AddDays(-1);
            DateTime date4 = new DateTime(Convert.ToInt32(getdate.Year), Convert.ToInt32(getdate.Month), (Convert.ToInt32(getdate.Day)), 12, 0, 0).AddYears(-61).AddDays(-1);
            List<string> lstTemp = new List<string>();
            int skip = 0, take = 10000;
            while (skip < idnos.Count)
            {

                BatchResponse response2 = new BatchResponse();

                lstTemp = idnos.Skip(skip).Take(take).ToList();

                string disidnos = string.Empty;

                lstTemp.ForEach(t => { if (IsValidNumber(t)) disidnos += string.IsNullOrEmpty(disidnos) ? t : " " + t; });

                var result = client.Search<ConsumerIndex>(s => s
                .Index(_consumerindex)
                .Size(0)
                .Query(q => q.QueryString(f => f.Fields(h => h.Field(g => g.IDNumber)).Query(disidnos)))
                     .Aggregations(childAggs => childAggs
                     .Terms("gender_count", avg => avg.Field(p => p.Gender))
                     .Terms("deceased_count", avg => avg.Field(p => p.isDeceased))
                     .Terms("marital_ind", avg => avg.Field(p => p.MaritalStatus))
                     .Terms("risk_category", avg => avg.Field(p => p.RiskCategory))
                     .Terms("alloy_cout", avg => avg.Field(p => p.Alloy))
                     .Terms("deeds_count", avg => avg.Field(p => p.DeedsIndicator))
                     .Terms("adverse_count", avg => avg.Field(p => p.AdverseIndicator))
                     .Terms("director_count", avg => avg.Field(p => p.DirectorIndicator))
                     .Terms("email_count", avg => avg.Field(p => p.EmailIndicator))
                     ));


                //gender with dob
                var searchResponse = client.Search<ConsumerIndex>(s => s
                                  .Index(_consumerindex)
                                  .Size(0)
                                   .Query(q => q.QueryString(f => f.Fields(h => h.Field(g => g.IDNumber)).Query(disidnos)))
                                   .Aggregations(childAggs => childAggs
                                       .Terms("gender_dob_count", avg => avg.Field(p => p.Gender)
                                           .Aggregations(childAgg => childAgg
                                               .DateRange("dob_range", ra => ra
                                               .Field(p => p.DateOfBirth)
                                               .Ranges(
                                               r => r.From(new DateTime(Convert.ToInt32(date2.Year), Convert.ToInt32(date2.Month), (Convert.ToInt32(date2.Day)), 12, 0, 0)).To(DateMath.Anchored(date1)).Key("18-34"),
                                               r => r.From(new DateTime(Convert.ToInt32(date3.Year), Convert.ToInt32(date3.Month), (Convert.ToInt32(date3.Day)), 12, 0, 0)).To(DateMath.Anchored(date2)).Key("35-40"),
                                               r => r.From(new DateTime(Convert.ToInt32(date4.Year), Convert.ToInt32(date4.Month), (Convert.ToInt32(date4.Day)), 12, 0, 0)).To(DateMath.Anchored(date3)).Key("41-60"),
                                               r => r.From(new DateTime(Convert.ToInt32(date4.Year), Convert.ToInt32(date4.Month), (Convert.ToInt32(date4.Day)), 12, 0, 0).AddYears(-70)).To(DateMath.Anchored(date4)).Key("61+")
                                               )
                                             ))
                                       )
                                       )
                                   );

                //province gender and dob
                var searchprovinceres = client.Search<ConsumerIndex>(s => s
                                  .Index(_consumerindex)
                                  .Size(0)
                                   .Query(q => q.QueryString(f => f.Fields(h => h.Field(g => g.IDNumber)).Query(disidnos)))
                                   .Aggregations(childAggs => childAggs
                                       .Terms("province_count", avg => avg.Field(p => p.Province).Size(20)
                                       .Aggregations(childAggsn => childAggsn
                                       .Terms("gen_province_count", avgs => avgs.Field(p => p.Gender)
                                           .Aggregations(childAgg => childAgg
                                               .DateRange("dob_range_prov", ra => ra
                                               .Field(p => p.DateOfBirth)
                                               .Ranges(
                                               r => r.From(new DateTime(Convert.ToInt32(date2.Year), Convert.ToInt32(date2.Month), (Convert.ToInt32(date2.Day)), 12, 0, 0).AddDays(-1)).To(DateMath.Anchored(date1)).Key("18-34"),
                                               r => r.From(new DateTime(Convert.ToInt32(date3.Year), Convert.ToInt32(date3.Month), (Convert.ToInt32(date3.Day)), 12, 0, 0).AddDays(-1)).To(DateMath.Anchored(date2)).Key("35-40"),
                                               r => r.From(new DateTime(Convert.ToInt32(date4.Year), Convert.ToInt32(date4.Month), (Convert.ToInt32(date4.Day)), 12, 0, 0).AddDays(-1)).To(DateMath.Anchored(date3)).Key("41-60"),
                                               r => r.From(new DateTime(Convert.ToInt32(date4.Year), Convert.ToInt32(date4.Month), (Convert.ToInt32(date4.Day)), 12, 0, 0).AddYears(-70)).To(DateMath.Anchored(date4)).Key("61+")
                                               )
                                             ))
                                       )
                                       )
                                       ))
                                   );

                //gender and income category
                var searchincomecategory = client.Search<ConsumerIndex>(s => s
                            .Index(_consumerindex)
                            .Size(0)
                             .Query(q => q.QueryString(f => f.Fields(h => h.Field(g => g.IDNumber)).Query(disidnos)))
                             .Aggregations(childAggs => childAggs
                                 .Terms("inc_count", avg => avg.Field(p => p.IncomerCategoty).Size(20)
                                     .Aggregations(childAgg => childAgg
                                         .Terms("gen_category", ra => ra
                                         .Field(p => p.Gender)
                                       ))
                                 )
                                 )
                             );




                var gen_count = result.Aggregations.Terms("gender_count");
                var dec_count = result.Aggregations.Terms("deceased_count");
                var married_count = result.Aggregations.Terms("marital_ind");
                var risk_count = result.Aggregations.Terms("risk_category");
                var alloy_count = result.Aggregations.Terms("alloy_cout");
                var deeds_count = result.Aggregations.Terms("deeds_count");
                var adv_count = result.Aggregations.Terms("adverse_count");
                var dir_count = result.Aggregations.Terms("director_count");
                var email_count = result.Aggregations.Terms("email_count");
                var gen_dob_res = searchResponse.Aggregations.Terms("gender_dob_count");
                var gen_income = searchincomecategory.Aggregations.Terms("inc_count");
                var province_counts = searchprovinceres.Aggregations.Terms("province_count");
                var income_count = result.Aggregations.Terms("income_count");
                //gender
                foreach (var tempgen in gen_count.Buckets)
                {
                    GenderDetail detail = new GenderDetail();
                    detail.Gender = tempgen.Key;
                    detail.Count = Convert.ToInt32(tempgen.DocCount);
                    response2.genlst.Add(detail);
                }
                //desceded count

                foreach (var tempgen in dec_count.Buckets)
                {
                    if (tempgen.Key == "1")
                    {
                        response2.descesed_count = Convert.ToInt64(tempgen.DocCount);
                    }
                }
                //person status
                foreach (var tempgen in married_count.Buckets)
                {

                    MaritalStatusDetail details = new MaritalStatusDetail();
                    details.Status = tempgen.Key;
                    details.Count = Convert.ToInt32(tempgen.DocCount);
                    response2.maritalstatuslst.Add(details);
                }
                //risk category

                foreach (var temprisk in risk_count.Buckets)
                {
                    RiskDetail riskDetail = new RiskDetail();
                    riskDetail.Catagory = temprisk.Key;
                    riskDetail.Count = Convert.ToInt32(temprisk.DocCount);
                    response2.riskDetails.Add(riskDetail);
                }

                //alloy

                foreach (var temprisk in alloy_count.Buckets)
                {

                    AlloyDetail detail = new AlloyDetail();
                    detail.Alloycategory = temprisk.Key;
                    detail.Count = (int)temprisk.DocCount;
                    response2.alloyDetails.Add(detail);
                }
                //deeds count
                foreach (var temprisk in deeds_count.Buckets)
                {
                    if (temprisk.Key == "1")
                    {
                        response2.deeds_count = Convert.ToInt64(temprisk.DocCount);
                    }
                }
                //adverse count
                foreach (var temprisk in adv_count.Buckets)
                {
                    if (temprisk.Key == "1")
                    {
                        response2.adverse_count = Convert.ToInt64(temprisk.DocCount);
                    }
                }

                foreach (var temprisk in dir_count.Buckets)
                {
                    if (temprisk.Key == "1")
                    {
                        response2.director_count = Convert.ToInt64(temprisk.DocCount);
                    }
                }

                foreach (var temprisk in email_count.Buckets)
                {
                    if (temprisk.Key == "1")
                    {
                        response2.email_count = Convert.ToInt64(temprisk.DocCount);
                    }
                }


                //gender with dob
                foreach (var temp in gen_dob_res.Buckets)
                {
                    foreach (var t in temp.Values)
                    {

                        foreach (var item in ((Nest.BucketAggregate)t).Items)
                        {
                            GenderWithDobRange range = new GenderWithDobRange();
                            range.Gender = temp.Key;
                            range.category = ((Nest.RangeBucket)item).Key;
                            range.Count = (int)((Nest.RangeBucket)item).DocCount;
                            response2.dobranges.Add(range);
                        }

                    }
                }

                //gen with income

                foreach (var temp in gen_income.Buckets)
                {
                    foreach (var t in temp.Values)
                    {

                        foreach (var item in ((Nest.BucketAggregate)t).Items)
                        {
                            IncomeCategory category = new IncomeCategory();
                            category.Category = temp.Key;
                            category.Gender = (string)((Nest.KeyedBucket<object>)item).Key;
                            category.Count = (int)((Nest.KeyedBucket<object>)item).DocCount;
                            response2.IncomeCategories.Add(category);
                        }
                    }
                }



                foreach (var temp in province_counts.Buckets)
                {
                    foreach (var t in temp.Values)
                    {
                        foreach (var item in ((Nest.BucketAggregate)t).Items)
                        {
                            foreach (var agegroupcount in ((Nest.IsAReadOnlyDictionaryBase<string, Nest.IAggregate>)item).Values)
                            {
                                foreach (var agegroupvalue in ((Nest.BucketAggregate)agegroupcount).Items)
                                {
                                    ProvinceDetail detail = new ProvinceDetail();
                                    detail.ProvinceName = temp.Key;
                                    detail.Gender = (string)((Nest.KeyedBucket<object>)item).Key;
                                    if (((Nest.RangeBucket)agegroupvalue).Key == "61+")
                                    {
                                        detail.range_61_above = ((Nest.RangeBucket)agegroupvalue).DocCount;
                                    }
                                    else if (((Nest.RangeBucket)agegroupvalue).Key == "41-60")
                                    {
                                        detail.range_41_60 = ((Nest.RangeBucket)agegroupvalue).DocCount;
                                    }
                                    else if (((Nest.RangeBucket)agegroupvalue).Key == "35-40")
                                    {
                                        detail.range_35_40 = ((Nest.RangeBucket)agegroupvalue).DocCount;
                                    }
                                    else if (((Nest.RangeBucket)agegroupvalue).Key == "18-34")
                                    {
                                        detail.range_18_34 = ((Nest.RangeBucket)agegroupvalue).DocCount;
                                    }
                                    response2.provincelst.Add(detail);
                                }
                            }
                        }
                    }
                }


                response.adverse_count += response2.adverse_count;
                response.deeds_count += response2.deeds_count;
                response.descesed_count += response2.descesed_count;
                response.director_count += response2.director_count;
                response.email_count += response2.email_count;

                foreach (var alloy2 in response2.alloyDetails)
                {
                    if (response.alloyDetails.Find(t => t.Alloycategory.ToUpper() == alloy2.Alloycategory.ToUpper()) != null)
                        response.alloyDetails.Find(t => t.Alloycategory.ToUpper() == alloy2.Alloycategory.ToUpper()).Count += alloy2.Count;
                    else
                        response.alloyDetails.Add(alloy2);
                }

                foreach (var dobrange2 in response2.dobranges)
                {

                    if (response.dobranges.Find(t => t.category.ToUpper() == dobrange2.category.ToUpper() && t.Gender.ToUpper() == dobrange2.Gender.ToUpper()) != null)
                        response.dobranges.Find(t => t.category.ToUpper() == dobrange2.category.ToUpper() && t.Gender.ToUpper() == dobrange2.Gender.ToUpper()).Count += dobrange2.Count;
                    else
                        response.dobranges.Add(dobrange2);

                }

                foreach (var gender2 in response2.genlst)
                {

                    if (response.genlst.Find(t => t.Gender.ToUpper() == gender2.Gender.ToUpper()) != null)
                        response.genlst.Find(t => t.Gender.ToUpper() == gender2.Gender.ToUpper()).Count += gender2.Count;
                    else
                        response.genlst.Add(gender2);

                }

                foreach (var income2 in response2.IncomeCategories)
                {
                    if (response.IncomeCategories.Find(t => t.Category.ToUpper() == income2.Category.ToUpper() && t.Gender == income2.Gender) != null)
                        response.IncomeCategories.Find(t => t.Category.ToUpper() == income2.Category.ToUpper() && t.Gender == income2.Gender).Count += income2.Count;
                    else
                        response.IncomeCategories.Add(income2);
                }

                foreach (var marital2 in response2.maritalstatuslst)
                {
                    if (response.maritalstatuslst.Find(t => t.Status.ToUpper() == marital2.Status.ToUpper()) != null)
                        response.maritalstatuslst.Find(t => t.Status.ToUpper() == marital2.Status.ToUpper()).Count += marital2.Count;
                    else
                        response.maritalstatuslst.Add(marital2);
                }
                foreach (var risk2 in response2.riskDetails)
                {
                    if (response.riskDetails.Find(t => t.Catagory.ToUpper() == risk2.Catagory.ToUpper()) != null)
                        response.riskDetails.Find(t => t.Catagory.ToUpper() == risk2.Catagory.ToUpper()).Count += risk2.Count;
                    else
                        response.riskDetails.Add(risk2);
                }
                foreach (var province2 in response2.provincelst)
                {

                    if (province2.range_18_34 != 0)
                    {
                        if (response.provincelst.Find(t => t.Gender.ToUpper() == province2.Gender.ToUpper() && t.ProvinceName == province2.ProvinceName) != null)
                            response.provincelst.Find(t => t.Gender.ToUpper() == province2.Gender.ToUpper() && t.ProvinceName == province2.ProvinceName).range_18_34 += province2.range_18_34;
                        else
                            response.provincelst.Add(province2);
                    }
                    else if (province2.range_35_40 != 0)
                    {
                        if (response.provincelst.Find(t => t.Gender.ToUpper() == province2.Gender.ToUpper() && t.ProvinceName == province2.ProvinceName) != null)
                            response.provincelst.Find(t => t.Gender.ToUpper() == province2.Gender.ToUpper() && t.ProvinceName == province2.ProvinceName).range_35_40 += province2.range_35_40;
                        else
                            response.provincelst.Add(province2);
                    }
                    else if (province2.range_41_60 != 0)
                    {
                        if (response.provincelst.Find(t => t.Gender.ToUpper() == province2.Gender.ToUpper() && t.ProvinceName == province2.ProvinceName) != null)
                            response.provincelst.Find(t => t.Gender.ToUpper() == province2.Gender.ToUpper() && t.ProvinceName == province2.ProvinceName).range_41_60 += province2.range_41_60;
                        else
                            response.provincelst.Add(province2);
                    }
                    else if (province2.range_61_above != 0)
                    {
                        if (response.provincelst.Find(t => t.Gender.ToUpper() == province2.Gender.ToUpper() && t.ProvinceName == province2.ProvinceName) != null)
                            response.provincelst.Find(t => t.Gender.ToUpper() == province2.Gender.ToUpper() && t.ProvinceName == province2.ProvinceName).range_61_above += province2.range_61_above;
                        else
                            response.provincelst.Add(province2);
                    }

                }

                skip += take;
            }
            return response;
        }


        public int GetCommercialPhoneCount(string phonenumber)
        {
            var searchResponse = client.Search<CommercialData>(s => s
                   .Index(_commercialdata)
                   .Size(10000)
                   .Query(q => q.Terms(f => f.Field(h => h.CommercialTelephone).Terms(phonenumber)))
                 );

            return searchResponse.Documents.Count();
        }


        public static bool IsValidNumber(string str)
        {
            try
            {
                Convert.ToInt64(str);
                return true;
            }
            catch
            {
                return false;
            }

        }

    }

    public class ESRequest
    {
        public int start { get; set; }
        public int length { get; set; }
        public List<Sort> sort { get; set; }
    }

    public class Sort
    {
        public string fieldName { get; set; }
        public bool descending { get; set; }
    }


    public class ESSearchConsumerRequest : ESRequest
    {
        public string IDNumber { get; set; }
        public string GlobalSearch { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfBirthFromDate { get; set; }

        public DateTime DateOfBirthToDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CompanyRegNumber { get; set; }
        public string CompanyName { get; set; }
        public string Gender { get; set; }
        public string EmploymentSector { get; set; }
        public string EmploymentLevel { get; set; }

        public string MaritalStatus { get; set; }

        public string ContactScore { get; set; }

        public string LSM { get; set; }

        public bool Employed { get; set; }
        public bool Directors { get; set; }

        public bool HomeOwners { get; set; }
        public bool ExAdverseIndicator { get; set; }
        public bool CellNoOnly { get; set; }
        public bool EmailAddressOnly { get; set; }
        public string Type { get; set; }
        public string Search { get; set; }
        public string LogEs { get; set; }

        public string Passport { get; set; }
    }

    public class ESSearchCommercialRequest : ESRequest
    {

        public string CompanyRegNumber { get; set; }
        public string CompanyName { get; set; }
        public string GlobalSearch { get; set; }
        public string CommercialStatusCode { get; set; }
        public DateTime BusinessStartDate { get; set; }
        public string Search { get; set; }
        public string CommercialAddress { get; set; }
        public string CommercialTelephone { get; set; }
        public string EsLog { get; set; }
    }

    public class ESSearchAddressRequest : ESRequest
    {
        public string Address { get; set; }
        public string Search { get; set; }
    }

    public class ESSearchAddressResponse
    {
        public List<ConsumerAddressInformation> data { get; set; }
        public long TotalCount { get; set; }
        public long TotalTime { get; set; }

    }

    public class ESSearchConsumerResponse
    {
        public List<ConsumerIndex> data { get; set; }

        public long TotalCount { get; set; }
        public long TotalTime { get; set; }
        public bool isESSearchLog { get; set; }
    }


    public class ESSearchCommercialResponse
    {
        public List<CommercialData> data { get; set; }
        public List<Commercialdata> data2 { get; set; }

        public long TotalCount { get; set; }
        public string ErrorMessage { get; set; }
        public long TotalTime { get; set; }
        public bool isESSearchLog { get; set; }
    }

    public class ESSearchBatchTracing
    {
        public long TotalCount { get; set; }

    }
    public class BatchResponse
    {
        public double descesed_count { get; set; }
        public double email_count { get; set; }
        public double deeds_count { get; set; }
        public double director_count { get; set; }
        public double adverse_count { get; set; }
        public List<ProvinceDetail> provincelst { get; set; } = new List<ProvinceDetail>();
        public List<RiskDetail> riskDetails { get; set; } = new List<RiskDetail>();
        public List<MaritalStatusDetail> maritalstatuslst { get; set; } = new List<MaritalStatusDetail>();
        public List<GenderDetail> genlst { get; set; } = new List<GenderDetail>();
        public List<IncomeCategory> IncomeCategories { get; set; } = new List<IncomeCategory>();
        public List<AlloyDetail> alloyDetails { get; set; } = new List<AlloyDetail>();
        public List<GenderWithDobRange> dobranges { get; set; } = new List<GenderWithDobRange>();
    }

    public class ProvinceDetail
    {
        public string ProvinceName { get; set; }
        public string Gender { get; set; }
        public double range_61_above { get; set; }
        public double range_41_60 { get; set; }
        public double range_35_40 { get; set; }
        public double range_18_34 { get; set; }
    }


    public class RiskDetail
    {
        public string Catagory { get; set; }
        public int Count { get; set; }
    }
    public class MaritalStatusDetail
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
    public class GenderDetail
    {
        public string Gender { get; set; }
        public int Count { get; set; }
    }
    public class IncomeCategory
    {
        public int Count { get; set; }
        public string Gender { get; set; }
        public string Category { get; set; }
    }
    public class AlloyDetail
    {
        public int Count { get; set; }
        public string Alloycategory { get; set; }
    }
    public class GenderWithDobRange
    {
        public string Gender { get; set; }
        public string category { get; set; }
        public int Count { get; set; }
    }

    public class Lsm
    {
        public string Category { get; set; }
        public int Count { get; set; }
    }
    public class LeasdaMaritalStatus
    {
        public string MaritalStatus { get; set; }
        public int StatusCount { get; set; }
        public List<RiskDetail> riskList { get; set; } = new List<RiskDetail>();
        public List<AlloyDetail> alloylist { get; set; } = new List<AlloyDetail>();
        public List<Lsm> lsmlist { get; set; } = new List<Lsm>();
        public List<IncomeCategory> incomelist { get; set; } = new List<IncomeCategory>();

    }
    public class LeadsGender
    {
        public string GenderCategory { get; set; }
        public int GenderCount { get; set; }
        public List<LeasdaMaritalStatus> maritalStatuslst { get; set; } = new List<LeasdaMaritalStatus>();
    }
    public class LeadsProvince
    {
        public string ProvinceName { get; set; }
        public int ProvinceCount { get; set; }
        public List<LeadsGender> genderList { get; set; } = new List<LeadsGender>();
    }
    public class LeadsList
    {
        public string ProvinceName { get; set; }
        public int ProvinceCount { get; set; }
        public string Gender { get; set; }
        public int GenderCount { get; set; }
        public string Marital { get; set; }
        public int MaritalCount { get; set; }
        public string Alloy { get; set; }
        public int AlloyCount { get; set; }
        public string LSM { get; set; }
        public int LSMCount { get; set; }
        public string Risk { get; set; }
        public int RiskCount { get; set; }
        public string IncomeCatagory { get; set; }
        public int IncomeCatagoryCount { get; set; }
    }
    public class ESLeadRequest
    {
        public DateTime DoBRange1 { get; set; }
        public DateTime DoBRange2 { get; set; }
        public bool IsEmployed { get; set; }
        public bool IsDirector { get; set; }
        public bool IsHomeOwner { get; set; }
        public bool IsAdversed { get; set; }
        public bool IsCellNumber { get; set; }
        public bool IsEmail { get; set; }
        public string EmploymentLevel { get; set; }
        public bool IsDeceased { get; set; }
        public List<ProvinceLeads> provincelst = new List<ProvinceLeads>();
        public List<AlloyLeads> alloylst { get; set; } = new List<AlloyLeads>();
        public List<LSMLeads> lsmlst { get; set; } = new List<LSMLeads>();
        public List<RiskCategoryLeads> risklst { get; set; } = new List<RiskCategoryLeads>();
        public List<IncomeCategoryLeads> inclst { get; set; } = new List<IncomeCategoryLeads>();
    }

    public class ProvinceLeads
    {
        public string ProvinceName { get; set; }
    }

    //below all VM for Leads Idnumber fetching
    public class EsLeadInNoRequest
    {
        public int LeadsRequested { get; set; }
        public DateTime DoBRange1 { get; set; }
        public DateTime DoBRange2 { get; set; }
        public bool IsEmployed { get; set; }
        public bool IsDirector { get; set; }
        public bool IsHomeOwner { get; set; }
        public bool IsAdversed { get; set; }
        public bool IsCellNumber { get; set; }
        public bool IsEmail { get; set; }
        public string EmploymentLevel { get; set; }
        public bool IsDeceased { get; set; }

        public List<AlloyLeads> alloylst { get; set; } = new List<AlloyLeads>();
        public List<LSMLeads> lsmlst { get; set; } = new List<LSMLeads>();
        public List<RiskCategoryLeads> risklst { get; set; } = new List<RiskCategoryLeads>();
        public List<IncomeCategoryLeads> inclst { get; set; } = new List<IncomeCategoryLeads>();
        public List<LeadsResponseIdno> tablelst { get; set; } = new List<LeadsResponseIdno>();
    }

    public class LSMLeads
    {
        public string LsmName { get; set; }
        public bool Ischecked { get; set; }
    }
    public class RiskCategoryLeads
    {
        public string RiskName { get; set; }
        public bool Ischecked { get; set; }
    }
    public class AlloyLeads
    {
        public string AlloyName { get; set; }
        public bool Ischecked { get; set; }
    }
    public class IncomeCategoryLeads
    {
        public string IncomeCategoryName { get; set; }
        public bool Ischecked { get; set; }
    }
    public class LeadsResponseIdno
    {
        public string ProvinceName { get; set; }
        public string Gender { get; set; }
        public string Marital { get; set; }
        public int RequiredCount { get; set; }
        public int AvailableCount { get; set; }
        public int TotalSum { get; set; }
    }
}
