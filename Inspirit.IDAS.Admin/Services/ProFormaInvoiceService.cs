using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using System.IO;
using iText.Kernel.Pdf;
using iText.Html2pdf;

namespace Inspirit.IDAS.Admin
{
    public class ProFormaInvoiceService
    {
        IDASDbContext _dbContext;
        EmailService _emailService;
       

        public ProFormaInvoiceService(IDASDbContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }
        public async Task<ProFormaInvoiceDataResponse> ProFormaInvoiceDataList(ProFormaInvoiceDataTableRequest request)
        {
            ProFormaInvoiceDataResponse response = new ProFormaInvoiceDataResponse();
            try
            {
                DateTime FilterToDate = new DateTime(Convert.ToDateTime(request.todate).Year, Convert.ToDateTime(request.todate).Month, Convert.ToDateTime(request.todate).Day, 23, 59, 59);

                var lst = _dbContext.ProFormaInvoices.AsQueryable();
                var flt = lst;
                if (request.customerId != Guid.Empty)
                {
                    lst = lst.Where(c => c.CustomerId == request.customerId);
                }
                if (!String.IsNullOrEmpty(request.status))
                {
                    lst = lst.Where(c => c.Status == request.status);
                }
                flt = lst.Where(t => t.Date >= Convert.ToDateTime(request.fromdate)
                && t.Date <= Convert.ToDateTime(FilterToDate));
                var data = await (from s in flt
                                  join c in _dbContext.Customers on
                                  s.CustomerId equals c.Id
                                  select new ProFormaInvoiceVm
                                  {
                                      ID = s.ID,
                                      CustomerName = c.TradingName,
                                      CustomerCode = c.Code,
                                      Date = s.Date,
                                      EmailDate = s.EmailSendDate,
                                      ProFormaInvoiceNumber = s.ProFormaInvoiceNumber,
                                      ReferenceNumber = s.ReferenceNumber,
                                      SubTotal = s.SubTotal,
                                      VatTotal = s.VatTotal,
                                      Total = s.Total,
                                      IsSubmitted = s.IsSubmitted,
                                      Status = s.Status,
                                      proDisplayNumber = s.ProformaDisplyNumber
                                  }).ToListAsync();
                response.data = data;
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<ProFormaInvoiceCrudResponses> Insert(ProFormaInvoice data)
        {
            ProFormaInvoiceCrudResponses response = new ProFormaInvoiceCrudResponses();
            try
            {
                int proformainvoiceNumber = 0;
                var proformainvoices = await _dbContext.ProFormaInvoices.ToListAsync();
                if (proformainvoices.Count > 0)
                {
                    proformainvoiceNumber = proformainvoices.
                    Where(i => i.ProFormaInvoiceNumber == proformainvoices.Max(m => m.ProFormaInvoiceNumber))
                    .FirstOrDefault().ProFormaInvoiceNumber + 1;
                }
                data.ProFormaInvoiceNumber = proformainvoiceNumber > 0 ? proformainvoiceNumber : 1;
                data.ProformaDisplyNumber = new displaynumber().GenerateNumber(data.ProFormaInvoiceNumber, "PRO", 10);
                data.Status = "In Progress";
                data.EmailSendDate = DateTime.MinValue;
                foreach (var pr in data.ProformaInvoiceLineItems)
                {
                    var cnt = data.ProformaInvoiceLineItems.Where(t => t.ProductId == pr.ProductId).ToList();
                    if (cnt.Count > 1)
                    {
                        response.IsSuccess = false;
                        response.Message = "Products can not be entered more than once.";
                        return response;
                    }
                    pr.ID = Guid.NewGuid();
                    pr.ProformaInvoiceId = data.ID;
                }
                _dbContext.ProFormaInvoices.Add(data);
                await _dbContext.ProformaInvoiceLineItems
                    .AddRangeAsync(data.ProformaInvoiceLineItems);
                await _dbContext.SaveChangesAsync();
                response.IsSuccess = true;
                response.Id = data.ID;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public bool FinalInvoiceSave(ProFormaInvoice data)
        {
            bool IsSubmitted = false;
            try
            {
                int invoiceNumber = 0;
                var invoices = _dbContext.Invoices.ToList();
                if (invoices.Count > 0)
                {
                    invoiceNumber = invoices.
                    Where(i => i.InvoiceNumber == invoices.Max(m => m.InvoiceNumber))
                    .FirstOrDefault().InvoiceNumber + 1;
                }
                var products = _dbContext.Products.ToList();
                Invoice invoice = new Invoice
                {
                    ID = Guid.NewGuid(),
                    InvoiceNumber = invoiceNumber > 0 ? invoiceNumber : 1,
                    ProFormaInvoice = true,
                    Date = DateTime.Now,
                    InvoiceDate = DateTime.Now,
                    ReferenceNumber = 0,
                    SubTotal = data.SubTotal,
                    VatTotal = data.VatTotal,
                    Total = data.Total,
                    CustomerId = data.CustomerId,
                    ispaid = false,
                    IsCreditNoteRaised = false,
                    isSubmited = false
                };
                foreach (var pr in data.ProformaInvoiceLineItems)
                {
                    InvoiceLineItem invoiceLineItem = new InvoiceLineItem
                    {
                        ID = Guid.NewGuid(),
                        InvoiceID = invoice.ID,
                        Description = products
                                      .FirstOrDefault(n => n.Id == pr.ProductId).Name,
                        Quantity = pr.Quantity,
                        UnitPrice = pr.UnitPrice,
                        NetAmount = pr.Amount,
                        VatAmount = 0,
                        BillingType = "One Time Payment",
                        UsageType = "One Time Payment"
                    };
                    invoice.InvoiceLineItems.Add(invoiceLineItem);
                }
                invoice.InvoiceDisplayNumber = new displaynumber().GenerateNumber(invoice.InvoiceNumber, "INV", 10);
                _dbContext.Invoices.Add(invoice);
                data.IsSubmitted = true;
                data.InvoiceId = invoice.ID;
                IsSubmitted = data.IsSubmitted;
                data.Status = "Invoice Generated";
                _dbContext.ProFormaInvoices.Update(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return IsSubmitted;
        }
        public async Task<List<Customer>> GetCustomers()
        {
            List<Customer> list = new List<Customer>();
            list = await _dbContext.Customers.Where(m => m.Status == "Active").ToListAsync();
            return list;
        }
        public async Task<Customer> GetCustomerById(Guid customerid)
        {
            Customer customer = new Customer();
            try
            {
                customer = await _dbContext.Customers
                    .FirstOrDefaultAsync(m => m.Status == "Active" && m.Id == customerid);
            }
            catch (Exception ex) { }
            return customer;
        }
        public async Task<decimal> ApplicationSetting()
        {
            decimal vatPercent = 0;
            try
            {
                var value = await _dbContext.ApplicationSetting
                .FirstOrDefaultAsync(v => v.SettingName == "VAT %");
                if (value != null)
                    vatPercent = Convert.ToDecimal(value.SettingValue);
            }
            catch (Exception ex) { }
            return vatPercent;
        }
        public async Task<List<ProductVmodel>> GetProducts()
        {
            List<ProductVmodel> productlist = new List<ProductVmodel>();

            var list = await _dbContext.Products.Include(m => m.PackageRates)
                .Where(a => a.Status == true).ToListAsync();

            foreach (var product in list)
            {
                var prod = new ProductVmodel();
                prod.Id = product.Id;
                prod.Name = product.Name;
                prod.UsageType = product.UsageType;

                foreach (var rates in product.PackageRates)
                {
                    var rate = new ProductPackageRateVm();
                    rate.Id = rates.Id;
                    rate.MaxLimit = rates.MaxLimit;
                    rate.MinLimit = rates.MinLimit;
                    rate.UnitPrice = rates.UnitPrice;
                    prod.PackageRates.Add(rate);
                }
                productlist.Add(prod);
            }
            return productlist;
        }
        public async Task<ProFormaInvoiceCrudResponses> Update(ProFormaInvoice data)
        {
            ProFormaInvoiceCrudResponses response = new ProFormaInvoiceCrudResponses();
            try
            {
                var _addProduct = data.ProformaInvoiceLineItems.
                    Where(pid => pid.ProformaInvoiceId != data.ID).ToList();

                var _updateRange = data.ProformaInvoiceLineItems.
                   Where(pid => pid.ProformaInvoiceId == data.ID).ToList();

                List<ProformaInvoiceLineItem> _addProformaItems = new List<ProformaInvoiceLineItem>();
                foreach (var pr in _addProduct)
                {
                    var cnt = data.ProformaInvoiceLineItems
                        .Where(t => t.ProductId == pr.ProductId).ToList();
                    if (cnt.Count > 1)
                    {
                        response.IsSuccess = false;
                        response.Message = "Products can not be entered more than once.";
                        return response;
                    }
                    pr.ID = Guid.NewGuid();
                    pr.ProformaInvoiceId = data.ID;
                }
                data.EmailSendDate = DateTime.MinValue;
                _dbContext.ProFormaInvoices.Update(data);
                _dbContext.ProformaInvoiceLineItems.UpdateRange(_updateRange);

                if (_addProduct.Count > 0)
                    await _dbContext.ProformaInvoiceLineItems.AddRangeAsync(_addProduct);

                await _dbContext.SaveChangesAsync();
                response.IsSuccess = true;
                response.Id = data.ID;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
        public async Task<ProFormaInvoiceCrudResponses> Cancel(ProFormaInvoice data)
        {
            ProFormaInvoiceCrudResponses response = new ProFormaInvoiceCrudResponses();
            try
            {
                data.Status = "Cancelled";
                _dbContext.ProFormaInvoices.Update(data);
                await _dbContext.SaveChangesAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
        public async Task<ProFormaInvoiceCrudResponses> Delete(Guid id)
        {
            ProFormaInvoiceCrudResponses response = new ProFormaInvoiceCrudResponses();
            try
            {
                var data = await _dbContext.ProFormaInvoices.FirstOrDefaultAsync(t => t.ID == id);
                _dbContext.ProFormaInvoices.Remove(data);
                if (data.ProformaInvoiceLineItems.ToList().Count() > 0)
                    _dbContext.ProformaInvoiceLineItems
                        .RemoveRange(data.ProformaInvoiceLineItems);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ProFormaInvoice> RatesAdd()
        {
            ProFormaInvoice prinvce = new ProFormaInvoice();
            prinvce.Date = DateTime.Now;
            prinvce.ProformaInvoiceLineItems = new List<ProformaInvoiceLineItem>();
            return prinvce;
        }
        public async Task<List<ProformaInvoiceLineItem>> ProformaProductsRemove(Guid id)
        {
            List<ProformaInvoiceLineItem> proformaInvoiceLineItems =
                new List<ProformaInvoiceLineItem>();
            try
            {
                var pr = await _dbContext.ProformaInvoiceLineItems
                    .FirstOrDefaultAsync(t => t.ID == id);
                if (pr != null)
                {
                    _dbContext.ProformaInvoiceLineItems.Remove(pr);
                    await _dbContext.SaveChangesAsync();
                    proformaInvoiceLineItems = await _dbContext.ProformaInvoiceLineItems
                        .Where(i => i.ProformaInvoiceId == pr.ProformaInvoiceId).ToListAsync();
                }
            }
            catch (Exception ex)
            {

            }
            return proformaInvoiceLineItems;
        }
        public async Task<ProFormaReport> View(Guid id)
        {
            ProFormaReport data = new ProFormaReport();
            try
            {
                List<ApplicationSetting> settings = _dbContext.ApplicationSetting.ToList();

                data.proFormaInvoice = await _dbContext.ProFormaInvoices.Where(m => m.ID == id)
                    .Include(m => m.ProformaInvoiceLineItems)
                    .FirstOrDefaultAsync();
                foreach (var pr in data.proFormaInvoice.ProformaInvoiceLineItems)
                {
                    pr.ProFormaInvoice = null;
                    pr.Product = null;
                }
                data.proFormaInvoice.Customer = _dbContext.Customers.Find(data.proFormaInvoice.CustomerId);
                if (data.proFormaInvoice.Customer != null)
                {
                    data.ClientName = data.proFormaInvoice.Customer.TradingName;
                    data.RefrenceNumber = data.proFormaInvoice.ProformaDisplyNumber.ToString();
                    data.Address = data.proFormaInvoice.Customer.PhysicalAddress;
                    data.Code = data.proFormaInvoice.Customer.Code;
                    data.Date = data.proFormaInvoice.Date.ToString("dd/MM/yyyy");
                    data.TelePhoneNumber = data.proFormaInvoice.Customer.TelephoneNumber;
                    data.VatNumber = data.proFormaInvoice.Customer.VATNumber;
                    data.Email = data.proFormaInvoice.Customer.BillingEmail;
                    data.Comments = data.proFormaInvoice.Remarks;
                    data.BankName = settings.FirstOrDefault(b => b.SettingName == "Bank Name").SettingValue;
                    data.AccountNumber =settings.FirstOrDefault(b => b.SettingName == "Account Number").SettingValue;
                    data.BranchCode =settings.FirstOrDefault(b => b.SettingName == "Branch Code").SettingValue;
                    data.IDASCompanyAddress = settings.FirstOrDefault(b => b.SettingName == "IDAS Company Address").SettingValue;
                }
                data.proFormaInvoice.Customer = null;
            }
            catch (Exception ex)
            {

            }
            return data;
        }
        public async Task<List<ProFormaInvoiceBulkEmail>> GetProFormaInvoiceBulkEmail(string styear, string stmonth)
        {
            List<ProFormaInvoiceBulkEmail> lst = new List<ProFormaInvoiceBulkEmail>();
            try
            {
                DateTime currentdate = new DateTime(Convert.ToInt32(styear), Convert.ToInt32(stmonth), 1);
                DateTime endDate = new DateTime(Convert.ToInt32(styear), Convert.ToInt32(stmonth), 1).AddMonths(1).AddDays(-1);
                var ProFormaInvoices = await _dbContext.ProFormaInvoices
                                            .Include(p => p.ProformaInvoiceLineItems)
                                            .Where(w => w.Date >= currentdate && w.Date <= endDate && w.Status.ToUpper() != "Cancelled".ToUpper()).ToListAsync();
                foreach (var p in ProFormaInvoices)
                {
                    foreach (var pl in p.ProformaInvoiceLineItems)
                    {
                        pl.ProFormaInvoice = null;
                        pl.Product = null;
                    }
                }
                lst = (from pr in ProFormaInvoices
                       join c in _dbContext.Customers on
                       pr.CustomerId equals c.Id
                       select new ProFormaInvoiceBulkEmail
                       {
                           ClientName = c.TradingName,
                           RefrenceNumber = pr.ProformaDisplyNumber,
                           ProNumber = pr.ProFormaInvoiceNumber,
                           Address = c.PhysicalAddress,
                           Date = pr.Date.ToString("dd/MM/yyyy"),
                           Code = c.Code,
                           VatNumber = c.VATNumber,
                           TelePhoneNumber = c.TelephoneNumber,
                           Email = c.BillingEmail,
                           Comments = pr.Remarks,
                           BankName = _dbContext.ApplicationSetting
                                        .FirstOrDefault(b => b.SettingName == "Bank Name")
                                        .SettingValue,
                           AccountNumber = _dbContext.ApplicationSetting
                                         .FirstOrDefault(b => b.SettingName == "Account Number")
                                         .SettingValue,
                           BranchCode = _dbContext.ApplicationSetting
                                         .FirstOrDefault(b => b.SettingName == "Branch Code")
                                         .SettingValue,
                           IDASCompanyAddress = _dbContext.ApplicationSetting
                                         .FirstOrDefault(b => b.SettingName == "IDAS Company Address")
                                         .SettingValue,
                           proFormaInvoice = pr,
                           ProformaInvoicevalue = pr.Total,
                           isSelected = false,
                       }).ToList();
            }
            catch (Exception ex) { }
            return lst;
        }
        public void CreatePayment(Guid invoiceId, string paymentType)
        {
            Invoice invoice = _dbContext.Invoices.Include(t => t.InvoiceLineItems).FirstOrDefault(t => t.ID == invoiceId);

            Payment payment = new Payment();
            payment.Id = Guid.NewGuid();
            payment.InvoiceId = invoice.ID;
            payment.CustomerId = invoice.CustomerId;
            int count = _dbContext.Payments.Count();
            if (count == 0)
                payment.Number = 1;
            else
                payment.Number = _dbContext.Payments.Max(t => t.Number) + 1;
            payment.Date = DateTime.Today;
            payment.PaymentType = paymentType;
            payment.Amount = invoice.Total;
            _dbContext.Payments.Add(payment);
            _dbContext.SaveChanges();

            CreateWorkorders(invoice);
        }
        private void CreateWorkorders(Invoice invoice)
        {
            try
            {
                List<SubscriptionItem> items = new List<SubscriptionItem>();

                if (invoice.SubscriptionID.HasValue)
                {
                    items = _dbContext.SubscriptionItems.Include(t => t.ProductPackage).ThenInclude(p => p.Product).Where(t => t.SubscriptionId == invoice.SubscriptionID).ToList();
                }

                foreach (InvoiceLineItem item in invoice.InvoiceLineItems)
                {
                    var sub = items.FirstOrDefault(t => t.ProductPackageId == item.ProductPackageRateID);
                    int duration = 1;
                    int credits = 0;

                    if (item.ProductPackageRate.Product.UsageType == "Yearly")
                    {
                        duration = sub.Duration * 12;
                    }
                    else if (item.ProductPackageRate.Product.UsageType == "Credits")
                    {
                        credits = item.Quantity;
                    }

                    int woCnt = 1;
                    if (sub.BillingType == "Monthly")
                    {
                        woCnt = sub.Duration;
                    }
                    else if (sub.BillingType == "One Time Payment")
                    {
                        woCnt = 1; //sub.Duration * 12;
                    }
                    for (int i = 1; i <= woCnt; i++)
                    {
                        Workorder order = new Workorder();
                        order.Id = Guid.NewGuid();
                        order.CustomerId = invoice.CustomerId;
                        order.Credits = credits;
                        order.Status = "Active";
                        order.ServiceType = item.ProductPackageRate.Product.UsageType;
                        if (i == 1)
                        {
                            order.StartDate = sub.StartDate;
                        }
                        else
                        {
                            order.StartDate = new DateTime(sub.StartDate.AddMonths(duration - 1).Year, sub.StartDate.AddMonths(duration - 1).Month, 1);
                        }
                        order.EndDate = order.StartDate.AddMonths(1).AddDays(-1);
                        order.ProductPackageId = item.ProductPackageRateID.Value;
                        order.SubscriptionItemID = sub.Id;
                        _dbContext.Workorders.Add(order);

                    }

                }
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }

        }
        public async Task<List<ProductPackageRate>> PackageRates(Guid ProductId)
        {
            List<ProductPackageRate> rates = new List<ProductPackageRate>();
            try
            {
                rates = await _dbContext.ProductPackageRates
                    .Where(m => m.ProductId == ProductId).ToListAsync();

            }
            catch (Exception ex)
            {

            }
            return rates;
        }
        public async Task<string> SendMail(string toEmail, string attachmentFilePath, Guid invoiceid)
        {
            string html = string.Empty;
            string Message = "";
            //Guid UserId =Guid.Empty;
            try
            {
                EmailTemplate email = _dbContext.EmailTemplates.FirstOrDefault(t => t.Type == "Proforma Invoice");
                List<ApplicationSetting> settings = _dbContext.ApplicationSetting.ToList();
                ProFormaInvoice inv = _dbContext.ProFormaInvoices.Include(t => t.ProformaInvoiceLineItems).Where(t => t.ID == invoiceid).FirstOrDefault();
                Customer userdetail = _dbContext.Customers.Where(t => t.Id == inv.CustomerId).FirstOrDefault();
              
                if (email != null)
                {
                    var builder = new BodyBuilder();
                    builder.HtmlBody += email.MailContent;

                    

                    Message = await _emailService.SendEmailAsync(toEmail, "Proforma Invoice", "", attachmentFilePath, "", builder, "Proformainvoice");
                    if (Message == "Mail Sent")
                    {
                        var proforma = await _dbContext.ProFormaInvoices.FindAsync(invoiceid);
                        proforma.IsEmailSend = true;
                        proforma.EmailSendDate = DateTime.Now;
                        _dbContext.ProFormaInvoices.Update(proforma);
                        await _dbContext.SaveChangesAsync();
                    }

                }
                else
                {
                    Message = "No contents found on Email Template";
                }
            }catch(Exception ex)
            {

            }
            return Message;
        }
    }
    public class ProFormaReport
    {
        public string ClientName { get; set; }
        public string RefrenceNumber { get; set; }
        public string Address { get; set; }
        public string Date { get; set; }
        public string Code { get; set; }
        public string VatNumber { get; set; }
        public string TelePhoneNumber { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
        public string IDASCompanyAddress { get; set; }
        public ProFormaInvoice proFormaInvoice { get; set; }
        public string ProductDetail { get; set; }
        public string ProductName { get; set; }
    }
    public class ProFormaInvoiceDataResponse : DataTableResponse<ProFormaInvoiceVm>
    {

    }
    public class ProFormaInvoiceVm
    {
        public Guid ID { get; set; }
        public int ProFormaInvoiceNumber { get; set; }
        public bool ProFormaInvoice { get; set; }
        public DateTime Date { get; set; }
        public DateTime EmailDate { get; set; }
        public int ReferenceNumber { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal VatTotal { get; set; }
        public decimal Total { get; set; }
        public string BillingType { get; set; }
        public bool IsSubmitted { get; set; }
        public List<InvoiceLineItem> InvoiceLineItems { get; set; } = new List<InvoiceLineItem>();
        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public Subscription Subscription { get; set; }
        public Guid? SubscriptionID { get; set; }
        public string Status { get; set; }
        public string proDisplayNumber { get; set; }
    }
    public class ProFormaInvoiceCrudResponses
    {
        public string Message { get; set; }
        public Guid Id { get; set; }

        public bool IsSuccess { get; set; }
    }
    public class ProductPackageRateVmodel
    {
        public Guid RateId { get; set; }
        public string Name { get; set; }
    }
    public class ProductVmodel
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<ProductPackageRateVm> PackageRates { get; set; } = new List<ProductPackageRateVm>();

        public Service Service { get; set; }

        public Guid ServiceId { get; set; }

        public string UsageType { get; set; } // monthly,yearly,credits


    }
    public class ProFormaInvoiceBulkEmail
    {
        public string ClientName { get; set; }
        public string RefrenceNumber { get; set; }
        public int ProNumber { get; set; }
        public string Address { get; set; }
        public string Date { get; set; }
        public string Code { get; set; }
        public string VatNumber { get; set; }
        public string TelePhoneNumber { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
        public string IDASCompanyAddress { get; set; }
        public decimal ProformaInvoicevalue { get; set; }
        public bool isSelected { get; set; }
        public ProFormaInvoice proFormaInvoice { get; set; }
    }
    public class EmailProperty
    {
        public Guid id { get; set; }
        public string base64 { get; set; }
        public string toMail { get; set; }
        public string fileName { get; set; }
        public bool istaxinv { get; set; }
        public bool ispaid { get; set; }
        public string InvoiceNumber { get; set; }
    }
    public class ProFormaInvoiceDataTableRequest
    {
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string status { get; set; }
        public Guid customerId { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }
}



