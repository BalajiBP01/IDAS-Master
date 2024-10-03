using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class BatchTracingService
    {
        IDASDbContext _dbContext;
        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;
        public BatchTracingService(IDASDbContext dbContext,  IConfiguration config, IHostingEnvironment hostingEnvironment)
        {
            _dbContext = dbContext;
            _configuration = config;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<BatchTracingDataTableResponse> GetBatchTracingList(BatchDataTableRequest request)
        {
            BatchTracingDataTableResponse response = new BatchTracingDataTableResponse();
            try
            {
                

                DateTime FilterToDate = new DateTime(Convert.ToDateTime(request.todate).Year, Convert.ToDateTime(request.todate).Month, Convert.ToDateTime(request.todate).Day, 23, 59, 59);
                var lst = _dbContext.BatchTraces.AsQueryable();
                var flt = lst;
                int cnt = lst.Count();
                
                flt = lst.Where(t => t.UploadDate >= Convert.ToDateTime(request.fromdate)
                && t.UploadDate <= FilterToDate && t.ProFormaInvoiceId != null);
                if (request.customerId != Guid.Empty)
                {
                    flt = lst.Where(c => c.CustomerId == request.customerId);
                }
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();
                var data = (from s in flt
                            join c in _dbContext.Customers on s.CustomerId equals c.Id
                            join p in _dbContext.ProFormaInvoices on s.ProFormaInvoiceId equals p.ID
                            into tmpil
                            from p in tmpil.DefaultIfEmpty(new Data.IDAS.ProFormaInvoice())
                            select new BatchTraceVM
                            {
                                AdminCertified = s.AdminCertified,
                                CustomerName = c.TradingName,
                                BatchNumber = "Batch_" + s.BatchNumber.ToString(),
                                CustomerId = s.CustomerId,
                                CustomerUserID = s.CustomerUserID,
                                FileName = s.OutPutFileName,
                                ID = s.ID,
                                UploadDate = Convert.ToString(s.UploadDate),
                                IsDataDownloaded = s.IsDataDownloaded,
                                ProFormaInvoiceId = s.ProFormaInvoiceId.Value == Guid.Empty ? "" : s.ProFormaInvoiceId.Value.ToString(),
                                ProFormaInvoiceNumber = p.ProformaDisplyNumber
                            }).ToList();
                response.data = data;

                foreach (var file in response.data)
                {
                    var memory = new MemoryStream();

                    string foldername = "ExportExcel";
                    string webrootpath = _hostingEnvironment.WebRootPath;
                    string changepath = webrootpath.Replace("Admin", "WebApplication");
                    string newpath = Path.Combine(changepath, foldername);
                    string filepath = Path.Combine(newpath,file.FileName);
                    if (System.IO.File.Exists(filepath))
                    {
                        file.isFileExists = true;
                    }
                    else
                    {
                        file.isFileExists = false;
                    }
                    
                    memory.Position = 0;
                }

            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<BatchtraceResponse> Update(Guid Batchid, Guid ApprovedId)
        {
            BatchtraceResponse res = new BatchtraceResponse();
            try
            {
                BatchTrace trace = new BatchTrace();
                trace = _dbContext.BatchTraces.Where(t => t.ID == Batchid).FirstOrDefault();
                trace.AdminCertified = true;
                trace.ApprovedBy = ApprovedId;
                trace.ApprovedOnDate = DateTime.Now;

                BatchProcessFileGeneration filegeneration = new BatchProcessFileGeneration();
                filegeneration.Id = Guid.NewGuid();
                filegeneration.BatchId = trace.ID;
                filegeneration.CreatedDate = DateTime.Now;
                filegeneration.CustomerID = trace.CustomerId;
                filegeneration.CustomerUserId = trace.CustomerUserID;
                filegeneration.Executed = false;
                filegeneration.ExecutedDate = null;
                filegeneration.IdNumber = trace.IdNumbers;
                _dbContext.BatchProcessFileGeneration.Add(filegeneration);
                _dbContext.BatchTraces.Update(trace);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.isSuccess = false;
            }
            return res;
        }
        public async Task<string> RemoveBatchTrace(Guid ID)
        {
            string FileName = "";
            try
            {
                var data = _dbContext.BatchTraces.Find(ID);
                FileName = data.OutPutFileName;
                _dbContext.BatchTraces.Remove(data);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex) { }
            return FileName;
        }
    }
    public class BatchDataTableRequest
    {
        public string fromdate { get; set; }
        public string todate { get; set; }
        public Guid customerId { get; set; }
        public DataTableRequest dtRequest { get; set; }
        public bool isFileExists { get; set; }
    }
    public class BatchTracingDataTableResponse : DataTableResponse<BatchTraceVM>
    {

    }
    public class BatchTraceVM
    {
        public Guid ID { get; set; }
        public CustomerUser CustomerUser { get; set; }
        public Guid CustomerUserID { get; set; }
        public string FileName { get; set; }
        public string BatchNumber { get; set; }
        public string UploadDate { get; set; }
        public int TotalRecords { get; set; }
        public int FoundRecords { get; set; }
        public string CustomerName { get; set; }
        public bool IsDataDownloaded { get; set; }
        public string ProFormaInvoiceId { get; set; }
        public string ProFormaInvoiceNumber { get; set; }
        public bool AdminCertified { get; set; }
        public Guid? CustomerId { get; set; }
        public bool isFileExists { get; set; }
    }
    public class BatchtraceResponse
    {
        public string Message { get; set; }
        public bool isSuccess { get; set; }
    }
}
