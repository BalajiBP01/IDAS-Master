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
    public class LeadGenerationService
    {
        IDASDbContext _dbContext;
        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;
        public LeadGenerationService(IDASDbContext dbContext, IConfiguration config, IHostingEnvironment hostingEnvironment)
        {
            _dbContext = dbContext;
            _configuration = config;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<LeadDataTableResponse> GetLeadList(LeadDataTableRequest request)
        {
            LeadDataTableResponse response = new LeadDataTableResponse();
            try
            {
                DateTime FilterToDate = new DateTime(Convert.ToDateTime(request.todate).Year, Convert.ToDateTime(request.todate).Month, Convert.ToDateTime(request.todate).Day, 23, 59, 59);
                var lst =  _dbContext.LeadsGenaration.AsQueryable();
                var flt = lst;
                int cnt = lst.Count();
                flt = lst.Where(t => t.LeadsDate >= Convert.ToDateTime(request.fromdate)
                && t.LeadsDate <= FilterToDate);
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
                            select new LeadGenerationVM
                            {
                                AdminCertified = s.AdminCertified.Value,
                                CustomerName = c.TradingName,
                                LeadNumber = "Lead_" + s.LeadsNumber.ToString(),
                                CustomerId = s.CustomerId,
                                CustomerUserID = s.CustomerUserID,
                                FileName = s.OutPutFileName,
                                ID = s.ID,
                                UploadDate = Convert.ToString(s.LeadsDate),
                                ProFormaInvoiceId = s.ProFormaInvoiceId.Value == null ? "" : s.ProFormaInvoiceId.Value.ToString(),
                                ProFormaInvoiceNumber = p.ProformaDisplyNumber
                            }).ToList();
                response.data = data;

                foreach (var file in response.data)
                {
                    var memory = new MemoryStream();

                    string foldername = "ExportExcel";
                    string webrootpath = _hostingEnvironment.WebRootPath;
                    string changepath= webrootpath.Replace("Admin", "WebApplication");
                    string newpath = Path.Combine(changepath, foldername);
                    string filepath = Path.Combine(newpath, file.FileName);
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
        public async Task<LeadResponse> ApproveLead(Guid LeadId, Guid ApprovedId)
        {
            LeadResponse res = new LeadResponse();
            try
            {
                LeadsGenaration lead = new LeadsGenaration();
                lead = _dbContext.LeadsGenaration.Where(t => t.ID == LeadId).FirstOrDefault();
                lead.AdminCertified = true;
                lead.ApprovedBy = ApprovedId;
                lead.ApprovedOnDate = DateTime.Now;

                LeadFileGeneration fileGeneration = new LeadFileGeneration();
                fileGeneration.CreatedBy = ApprovedId;
                fileGeneration.CreatedDate = DateTime.Now;
                fileGeneration.CustomerID = lead.CustomerId;
                fileGeneration.CustomerUserId = lead.CustomerUserID;
                fileGeneration.Executed = false;
                fileGeneration.ExecutedDate = null;
                fileGeneration.ID = Guid.NewGuid();
                fileGeneration.LeadId = lead.ID;
                fileGeneration.LeadOutput = lead.OutputDetail;
                _dbContext.LeadFileGeneration.Add(fileGeneration);
                _dbContext.LeadsGenaration.Update(lead);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.IsSuccess = false;
            }
            return res;
        }
        public async Task<string> RemoveLead(Guid ID)
        {
            string FileName = "";
            try
            {
                var data = _dbContext.LeadsGenaration.Find(ID);
                FileName = data.OutPutFileName;
                _dbContext.LeadsGenaration.Remove(data);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex) { }
            return FileName;
        }
    }


    public class LeadDataTableRequest
    {
        public string fromdate { get; set; }
        public string todate { get; set; }
        public Guid customerId { get; set; }
        public DataTableRequest dtRequest { get; set; }
        public bool isFileExists { get; set; }
    }
    public class LeadDataTableResponse : DataTableResponse<LeadGenerationVM>
    {

    }
    public class LeadGenerationVM
    {
        public Guid ID { get; set; }
        public CustomerUser CustomerUser { get; set; }
        public Guid CustomerUserID { get; set; }
        public string FileName { get; set; }
        public string LeadNumber { get; set; }
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
    public class LeadResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
