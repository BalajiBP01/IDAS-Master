using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
namespace Inspirit.IDAS.Admin
{
    public class InvoiceService
    {
        IDASDbContext _dbContext;
        EmailService _emailService;
        private IHostingEnvironment _hostingEnvironment;
        public InvoiceService(IDASDbContext dbContext, EmailService emailService, IHostingEnvironment HostingEnvironment)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _hostingEnvironment = HostingEnvironment;
    }
        public async Task<ActionResult<InvoiceDataResponse>> InvoiceDataList(InvoiceDataTableRequest request)
        {
            InvoiceDataResponse response = new InvoiceDataResponse();
            try
            {
                DateTime FilterToDate = new DateTime(Convert.ToDateTime(request.todate).Year, Convert.ToDateTime(request.todate).Month, Convert.ToDateTime(request.todate).Day, 23, 59, 59);
                var lst = _dbContext.Invoices.Where(t => t.Date >= Convert.ToDateTime(request.fromdate) && t.Date <= FilterToDate).AsQueryable();
                int cnt = lst.Count();
                var lstpayment = new List<Payment>();
                var flt = lst;
                List<Payment> paylistid = new List<Payment>();
                if (request.customerId != Guid.Empty && Convert.ToString(request.customerId) != "0")
                {
                    if (!String.IsNullOrEmpty(request.fromdate) && !String.IsNullOrEmpty(request.todate))
                    {
                        if (request.ispaid.ToUpper() == "BOTH")
                        {
                            flt = lst.Where(m => m.Date >= Convert.ToDateTime(request.fromdate)
                        && m.Date <= FilterToDate && m.CustomerId == request.customerId);
                        }
                        else if (request.ispaid.ToUpper() == "PAY")
                        {
                            flt = lst.Where(m => m.Date >= Convert.ToDateTime(request.fromdate)
                       && m.Date <= FilterToDate && m.CustomerId == request.customerId && m.ispaid == false);
                        }
                        else if (request.ispaid.ToUpper() == "PAID")
                        {
                            flt = lst.Where(m => m.Date >= Convert.ToDateTime(request.fromdate)
                       && m.Date <= FilterToDate && m.CustomerId == request.customerId && m.ispaid == true);
                        }
                    }
                    lstpayment = _dbContext.Payments.Where(t => t.CustomerId == request.customerId).ToList();
                }
                else
                {
                    if (!String.IsNullOrEmpty(request.fromdate) && !String.IsNullOrEmpty(request.todate))
                    {
                        if (request.ispaid.ToUpper() == "BOTH")
                        {
                            flt = lst.Where(m => m.Date >= Convert.ToDateTime(request.fromdate)
                        && m.Date <= FilterToDate);
                        }
                        else if (request.ispaid.ToUpper() == "PAY")
                        {
                            flt = lst.Where(m => m.Date >= Convert.ToDateTime(request.fromdate)
                       && m.Date <= FilterToDate && m.ispaid == false);
                        }
                        else if (request.ispaid.ToUpper() == "PAID")
                        {
                            flt = lst.Where(m => m.Date >= Convert.ToDateTime(request.fromdate)
                       && m.Date <= FilterToDate && m.ispaid == true);
                        }
                    }
                    lstpayment = _dbContext.Payments.ToList();
                }
                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();
                var data = (from s in flt
                            select new InvoiceVm
                            {
                                ID = s.ID,
                                Date = s.Date,
                                EmailDate = s.EmailSendDate,
                                InvoiceNumber = s.InvoiceNumber,
                                CustomerId = s.CustomerId,
                                TradingName = s.Customer.TradingName,
                                CustomerCode = s.Customer.Code,
                                ReferenceNumber = s.ReferenceNumber,
                                Total = s.Total,
                                isPayed = s.ispaid,
                                VatTotal = s.VatTotal,
                                SubTotal = s.SubTotal,
                                InvoiceDate = s.InvoiceDate,
                                msg = "Successfull",
                                IsProformaInvoice = s.ProFormaInvoice,
                                isCancelled = s.isCancelled,
                                InvoiceDisplayNumber = s.InvoiceDisplayNumber,
                                IsTacinvSent = s.IsTaxinvSent
                            }).ToList();
                foreach (var d in data)
                {
                    var invoiceIdCreditNote = _dbContext.Creditnotes.Where(m => m.InvoiceId == d.ID).FirstOrDefault();
                    if (invoiceIdCreditNote == null)
                    {
                        d.IsCreditNoteRaised = false;
                    }
                    else
                    {
                        d.IsCreditNoteRaised = true;
                        d.CreditNoteNumber = invoiceIdCreditNote.CreditNoteNumber.ToString();
                        d.CreditNoteTotal = invoiceIdCreditNote.CreditNoteValue;
                    }
                    var pyt = lstpayment.Where(t => t.InvoiceId == d.ID).FirstOrDefault();
                    if (pyt != null)
                        d.PaymentValue = pyt.PaymentAmountReceive;
                }
                response.data = data;
            }
            catch (Exception ex)
            {
                response.data[0].msg = ex.Message;
                return response;
            }
            return response;
        }
        public InvoiceCrudResponses Insert(Invoice data)
        {
            InvoiceCrudResponses response = new InvoiceCrudResponses();

            try
            {
                var id = _dbContext.Invoices.Where(m => m.ID == data.ID).FirstOrDefault();
                if (id != null)
                {
                    if (data.InvoiceLineItems.Count != 0)
                    {
                        foreach (var items in data.InvoiceLineItems)
                        {
                            var lineitems = _dbContext.InvoiceLineItems.Where(m => m.ID == items.ID).FirstOrDefault();
                            if (lineitems != null)
                            {
                                _dbContext.InvoiceLineItems.Update(lineitems);
                                _dbContext.SaveChanges();
                            }
                            else
                            {
                                _dbContext.InvoiceLineItems.Add(lineitems);
                                _dbContext.SaveChanges();
                            }
                        }

                    }


                    _dbContext.Invoices.Update(data);
                    _dbContext.SaveChanges();
                }
                else
                {
                    data.ID = Guid.NewGuid();
                    _dbContext.Invoices.Add(data);
                    _dbContext.SaveChanges();
                }

                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;

        }
        public List<Customer> GetCustomerList()
        {
            List<Customer> list = new List<Customer>();
            list = _dbContext.Customers.Where(m => m.Status == "Active").ToList();
            return list;
        }
        public List<ProductVm> GetProductList()
        {

            List<ProductVm> productlist = new List<ProductVm>();
            List<Product> list = new List<Product>();

            list = _dbContext.Products.Include(m => m.PackageRates).ToList();

            foreach (var product in list)
            {
                var prod = new ProductVm();
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
        public List<CustomerVModel> GetCustomers()
        {
            List<CustomerVModel> list = new List<CustomerVModel>();
            var clist = _dbContext.Customers.Where(m => m.Status == "Active").ToList();
            foreach (var cust in clist)
            {
                var customer = new CustomerVModel();
                customer.CustomerId = cust.Id;
                customer.CustomerName = cust.RegistrationName;
                customer.isSelected = false;
                customer.RegistrationNumber = cust.RegistrationNumber;
                list.Add(customer);
            }
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
        public InvoiceCrudResponses Update(Invoice data)
        {
            InvoiceCrudResponses response = new InvoiceCrudResponses();
            try
            {
                foreach (var invitem in data.InvoiceLineItems)
                {
                    _dbContext.InvoiceLineItems.Update(invitem);
                }

                _dbContext.Invoices.Update(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;

        }
        public InvoiceCrudResponses Delete(Guid id)
        {
            InvoiceCrudResponses response = new InvoiceCrudResponses();
            try
            {
                var data = _dbContext.Invoices.FirstOrDefault(t => t.ID == id);
                _dbContext.Invoices.Remove(data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public InvoiceCrudResponses Cancel(Guid id)
        {
            InvoiceCrudResponses response = new InvoiceCrudResponses();
            try
            {

                var data = _dbContext.Invoices.Include(t => t.InvoiceLineItems).FirstOrDefault(t => t.ID == id);
                data.isCancelled = true;

                string invItemId = string.Empty;
                data.InvoiceLineItems.ForEach(t => invItemId += "," + t.ID.ToString());
                var itemid = invItemId.TrimStart(',');

                var orders = _dbContext.Workorders.Where(t => itemid.Contains(Convert.ToString(t.InvoiceLineItemId)));

                foreach (var inv in data.InvoiceLineItems)
                {

                    var work = orders.Where(m => m.InvoiceLineItemId == inv.ID).ToList();
                    if (work.Count > 0)
                    {
                        work.ForEach(t => t.isCancelled = true);
                    }
                    _dbContext.Workorders.UpdateRange(work);

                }
                _dbContext.Invoices.Update(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<TaxInvoiceReport> View(Guid id)
        {
            TaxInvoiceReport data = new TaxInvoiceReport();
            try
            {
                data.invoice = _dbContext.Invoices.Include(m => m.Customer)
                    .Where(m => m.ID == id)
                    .Include(m => m.InvoiceLineItems)
                    .FirstOrDefault();
                data.invoice.Customer = _dbContext.Customers.Find(data.invoice.CustomerId);
                if (data.invoice.Customer != null)
                {
                    data.ClientName = data.invoice.Customer.TradingName;
                    data.RefrenceNumber = data.invoice.InvoiceDisplayNumber.ToString();
                    data.Address = data.invoice.Customer.PhysicalAddress;
                    data.Code = data.invoice.Customer.Code;
                    data.Date = data.invoice.Date.ToString("dd/MM/yyyy");
                    data.TelePhoneNumber = data.invoice.Customer.TelephoneNumber;
                    data.VatNumber = data.invoice.Customer.VATNumber;
                    data.Email = data.invoice.Customer.BillingEmail;
                    data.Comments = data.invoice.Remarks;
                    data.BankName = _dbContext.ApplicationSetting
                                              .FirstOrDefault(b => b.SettingName == "Bank Name")
                                              .SettingValue;
                    data.AccountNumber = _dbContext.ApplicationSetting
                                               .FirstOrDefault(b => b.SettingName == "Account Number")
                                               .SettingValue;
                    data.BranchCode = _dbContext.ApplicationSetting
                                               .FirstOrDefault(b => b.SettingName == "Branch Code")
                                               .SettingValue;
                    data.IDASCompanyAddress = _dbContext.ApplicationSetting
                                               .FirstOrDefault(b => b.SettingName == "IDAS Company Address")
                                               .SettingValue;

                }
            }
            catch (Exception ex)
            {

            }
            return data;
        }
        public SubscriptionResponse CreatePayment(Guid invoiceId, string paymentType, string referance, string comments, decimal PaymentAmountReceive, DateTime PaymentReceievedDate)
        {
            SubscriptionResponse response = new SubscriptionResponse();
            try
            {
                Invoice invoice = _dbContext.Invoices.Include(t => t.InvoiceLineItems).FirstOrDefault(t => t.ID == invoiceId);
               
                    if (invoice.Total >= PaymentAmountReceive && PaymentAmountReceive > 0)
                    {
                        invoice.ispaid = true;
                        _dbContext.Update(invoice);
                        _dbContext.SaveChanges();

                        Payment payment = new Payment();
                        payment.Id = Guid.NewGuid();
                        payment.InvoiceId = invoice.ID;
                        payment.CustomerId = invoice.CustomerId;
                        payment.Reference = invoice.ReferenceNumber.ToString();
                        payment.PaymentReceivedDate = PaymentReceievedDate;
                        int count = _dbContext.Payments.Count();
                        if (count == 0)
                            payment.Number = 1;
                        else
                            payment.Number = count + 1;
                        payment.Date = DateTime.Now;
                        payment.PaymentAmountReceive = PaymentAmountReceive;
                        payment.Reference = referance;
                        payment.Comments = comments;
                        payment.PaymentType = paymentType;
                        payment.Amount = invoice.Total;
                        _dbContext.Payments.Add(payment);
                        _dbContext.SaveChanges();
                        response.isSuccess = true;
                        response.Message = "Payment Done";

                        // CreateWorkorders(invoice);
                    }
                    else
                    {
                        response.isSuccess = false;
                        response.Message = "Amount received connot be greater than Invoice value :" + invoice.Total;
                    }

                

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                response.Message = ex.Message;
                response.isSuccess = false;
            }
            return response;
        }
        public SubscriptionResponse CreateCreditNote(Creditnote note)
        {
            SubscriptionResponse response = new SubscriptionResponse();
            Invoice invoice = _dbContext.Invoices.Include(t => t.InvoiceLineItems).FirstOrDefault(t => t.ID == note.InvoiceId);
            try
            {
                Creditnote creditnote = new Creditnote();
                creditnote.Id = Guid.NewGuid();
                creditnote.InvoiceId = invoice.ID;

                creditnote.CreditNoteValue = Convert.ToDecimal(note.CreditNoteValue);
                int count = _dbContext.Creditnotes.Count();
                if (count == 0)
                    creditnote.CreditNoteNumber = 1;
                else
                    creditnote.CreditNoteNumber = _dbContext.Creditnotes.Max(t => t.CreditNoteNumber) + 1;

                creditnote.CreditNoteDate = DateTime.Today;

                creditnote.Comments = note.Comments;

                _dbContext.Creditnotes.Add(creditnote);
                invoice.IsCreditNoteRaised = true;
                _dbContext.Update(invoice);
                _dbContext.SaveChanges();
                response.isSuccess = true;
                response.Message = "Credit Note Created";
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public InvoiceCrudResponses CheckExistingInvoiceId(Guid invoiceId)
        {
            InvoiceCrudResponses response = new InvoiceCrudResponses();

            Payment invoiceid = _dbContext.Payments.FirstOrDefault(t => t.InvoiceId == invoiceId);

            if (invoiceid != null)
            {
                response.IsSuccess = false;

            }
            else
            {
                response.IsSuccess = true;
            }
            return response;

        }
        public InvoiceCrudResponses CheckExistingCreditNote(Guid invoiceId)
        {
            InvoiceCrudResponses response = new InvoiceCrudResponses();

            Creditnote invoiceidcreditnote = _dbContext.Creditnotes.FirstOrDefault(t => t.InvoiceId == invoiceId);

            if (invoiceidcreditnote != null)
            {
                response.IsSuccess = false;

            }
            else
            {
                response.IsSuccess = true;
            }
            return response;

        }
        private void CreateWorkorders(Invoice invoice)
        {
            try
            {
                if (!invoice.ProFormaInvoice)
                {
                    List<SubscriptionItem> items = new List<SubscriptionItem>();
                    string strSubscriptionID = string.Empty;
                    invoice.InvoiceLineItems.ForEach(t => strSubscriptionID += "," + t.SubscriptionID.ToString());
                    var id = strSubscriptionID.TrimStart(',');
                    if (id != null)
                    {
                        items = _dbContext.SubscriptionItems.Include(t => t.ProductPackage).ThenInclude(p => p.Product).Where(t => strSubscriptionID.Contains(t.SubscriptionId.ToString())).ToList();
                    }

                    foreach (InvoiceLineItem item in invoice.InvoiceLineItems)
                    {
                        var sub = items.FirstOrDefault(t => t.SubscriptionId == item.SubscriptionID);
                        int duration = 1;
                        int credits = 0;

                        int woCnt = 1;
                        if (item.ProductPackageRate.Product.UsageType == "Yearly")
                        {
                            if (sub.BillingType == "Monthly")
                            {
                                woCnt = 1;
                            }
                            else if (sub.BillingType == "One Time Payment")
                            {
                                woCnt = sub.Duration; //sub.Duration * 12;
                            }
                            duration = sub.Duration * 12;
                        }
                        else if (item.ProductPackageRate.Product.UsageType == "Credits")
                        {
                            credits = item.Quantity;
                        }
                        else if (item.ProductPackageRate.Product.UsageType == "Monthly")
                        {
                            if (sub.BillingType == "Monthly")
                            {
                                woCnt = 1;
                            }
                            else if (sub.BillingType == "One Time Payment")
                            {
                                woCnt = sub.Duration; //sub.Duration * 12;
                            }
                            duration = sub.Duration;
                        }

                        Workorder order = new Workorder();
                        order.Id = Guid.NewGuid();
                        order.CustomerId = invoice.CustomerId;
                        order.Credits = credits;
                        order.Status = "Active";
                        order.InvoiceLineItemId = item.ID;
                        order.ServiceType = item.ProductPackageRate.Product.UsageType;
                        
                        if (sub.isBilled == true)
                        {
                            order.StartDate = (new DateTime(Convert.ToInt32(sub.StartDate.Year), (sub.StartDate.Month), 1));
                        }
                        else
                        {
                            order.StartDate = sub.StartDate;
                        }
                        order.isCancelled = false;
                        if (sub.BillingType == "Monthly")
                        {
                            order.EndDate = invoice.InvoiceDate.AddMonths(1).AddDays(-1);
                            order.EndDate = new DateTime(order.EndDate.Year, order.EndDate.Month, order.EndDate.Day, 23, 59, 59);
                        }
                        else if (sub.BillingType == "One Time Payment")
                        {
                            order.EndDate = sub.EndDate;
                        }
                        order.ProductPackageId = item.ProductPackageRateID.Value;
                        order.SubscriptionItemID = sub.Id;

                        sub.isBilled = true;
                        _dbContext.SubscriptionItems.Update(sub);
                        _dbContext.Workorders.Add(order);
                    }

                    _dbContext.SaveChanges();
                }

            }
            catch (Exception ex)
            {

            }

        }
        public string GenerateMonthlyInvoices(InvoiceGenerationVm list)
        {
            string message = string.Empty;
            try
            {
                // int startyear = 2018, startmonth = 11;
                //var Customerslist = _dbContext.Customers.Where(t => t.TradingName == "company3").ToList();
                DateTime lastday = new DateTime(Convert.ToInt32(list.StartYear), Convert.ToInt32(list.StartMonth), 1, 23, 59, 59)
                                       .AddMonths(1).AddDays(-1);
                DateTime firstday = new DateTime(Convert.ToInt32(list.StartYear), Convert.ToInt32(list.StartMonth), 1);
                DateTime previoumontStartDate = lastday.AddDays(-1 * lastday.Day + 1).AddMonths(-1).AddHours(-23).AddMinutes(-59).AddSeconds(-59);
                decimal subTotel = 0;
                bool isfound = false;
                ApplicationSetting setting = _dbContext.ApplicationSetting.FirstOrDefault(t => t.SettingName == "VAT %");
                string strProductPackageRateId = string.Empty;
                foreach (var cust in list.customers)
                {
                    subTotel = 0;
                    List<Subscription> lstsubscriptions = _dbContext.Subscriptions.Include(t => t.SubscriptionItems)
                                                          .Where(t => t.SubscriptionItems.Any(i => i.EndDate >= lastday
                                                           && i.Subscription.CustomerId == cust.CustomerId) && t.IsAutoBilled == true).ToList();

                    List<Invoice> lstInvociceGeneratedPreviousmonth = new List<Invoice>();
                    lstInvociceGeneratedPreviousmonth = _dbContext.Invoices.Include(m => m.InvoiceLineItems)
                                                        .Where(t => t.CustomerId == cust.CustomerId
                                                        && t.InvoiceDate >= previoumontStartDate && t.InvoiceDate <= previoumontStartDate.AddMonths(1).AddDays(-1)).ToList();

                    var lstInvoiceGenereatedCurrentMonth = _dbContext.Invoices.Include(m => m.InvoiceLineItems)
                                                                               .Where(t => t.CustomerId == cust.CustomerId
                                                                                && t.InvoiceDate >= lastday.AddDays(-1 * lastday.Day + 1)
                                                                                && t.InvoiceDate <= lastday && t.ProFormaInvoice == false).ToList();
                    Invoice objInvoice = new Invoice();
                    objInvoice.ID = Guid.NewGuid();
                    objInvoice.InvoiceNumber = _dbContext.Invoices.Count() + 1;
                    objInvoice.InvoiceDisplayNumber = new displaynumber().GenerateNumber(objInvoice.InvoiceNumber, "INV", 10);
                    objInvoice.ProFormaInvoice = false;
                    objInvoice.EmailSendDate = DateTime.MinValue;
                    objInvoice.IsEmailSend = false;
                    objInvoice.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    objInvoice.ReferenceNumber = 0;
                    objInvoice.isCancelled = false;
                    objInvoice.CustomerId = cust.CustomerId;
                    objInvoice.InvoiceDate = new DateTime(firstday.Year, firstday.Month, firstday.Day);
                    if (lstInvoiceGenereatedCurrentMonth.Count == 0)
                    {
                        foreach (Invoice Inv in lstInvociceGeneratedPreviousmonth)
                        {
                            if (!Inv.ProFormaInvoice)
                            {
                                objInvoice.Discount = Inv.Discount;
                                foreach (InvoiceLineItem Item in Inv.InvoiceLineItems)
                                {
                                    if (Item.BillingType.ToLower() == "monthly")
                                    {
                                        Subscription objSub = lstsubscriptions.Where(t => t.Id == Item.SubscriptionID).FirstOrDefault();
                                        if (objSub != null)
                                        {
                                            SubscriptionItem Subitems = _dbContext.SubscriptionItems.Where(t => t.SubscriptionId == objSub.Id).FirstOrDefault();

                                            InvoiceLineItem objItem = new InvoiceLineItem();
                                            if (Subitems.isBilled == true)
                                            {
                                                objItem.BillingType = Item.BillingType;
                                                objItem.ID = Guid.NewGuid();
                                                objItem.InvoiceID = objInvoice.ID;
                                                objItem.ProductPackageRateID = Item.ProductPackageRateID;
                                                objItem.Description = Item.Description;
                                                objItem.Quantity = Item.Quantity;
                                                if (Subitems.ProRataPrice == Item.UnitPrice)
                                                {
                                                    objItem.UnitPrice = Subitems.Rate;
                                                }
                                                else if (Subitems.Rate == Item.UnitPrice)
                                                {
                                                    objItem.UnitPrice = Subitems.Rate;
                                                }
                                                else if (Subitems.ProRataPrice != Item.UnitPrice && Subitems.Rate != Item.UnitPrice)
                                                {
                                                    objItem.UnitPrice = Item.UnitPrice;
                                                }

                                                objItem.NetAmount = objItem.Quantity * objItem.UnitPrice;
                                                objItem.VatAmount = Convert.ToDecimal(setting.SettingValue);
                                                objItem.BillingType = Item.BillingType;
                                                objItem.UsageType = Item.UsageType;
                                                objItem.SubscriptionID = Item.SubscriptionID;
                                                subTotel += objItem.NetAmount;
                                                objInvoice.InvoiceLineItems.Add(objItem);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    strProductPackageRateId = string.Empty;
                    foreach (Subscription sub in lstsubscriptions)
                    {
                        foreach (SubscriptionItem subItem in sub.SubscriptionItems)
                        {
                            if (!strProductPackageRateId.Contains(subItem.ProductPackageId.ToString()))
                                strProductPackageRateId += "," + subItem.ProductPackageId.ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(strProductPackageRateId)) strProductPackageRateId = strProductPackageRateId.TrimStart(',');
                    var lstProdPackage = _dbContext.ProductPackageRates.Include(y => y.Product).Where(t => strProductPackageRateId.Contains(t.Id.ToString())).ToList();
                    foreach (Subscription sub in lstsubscriptions)
                    {

                        isfound = false;

                        if (objInvoice.InvoiceLineItems.Find(t => t.SubscriptionID == sub.Id) == null)
                        {

                            foreach (SubscriptionItem subItem in sub.SubscriptionItems.Where(i => i.EndDate >= lastday && (i.isBilled == false || i.isBilled == null)))
                            {
                                var prodpackage = lstProdPackage.Where(t => t.Id == subItem.ProductPackageId).FirstOrDefault();
                                InvoiceLineItem item = new InvoiceLineItem();
                                item.ID = Guid.NewGuid();
                                item.InvoiceID = objInvoice.ID;
                                item.ProductPackageRateID = subItem.ProductPackageId;
                                item.Description = prodpackage.Product.Name;
                                item.BillingType = subItem.BillingType;
                                if (subItem.BillingType == "Monthly")
                                {
                                    item.Quantity = subItem.Quantity;
                                    item.UnitPrice = subItem.ProRataPrice;
                                    item.NetAmount = subItem.ProRataNetAmount;
                                }
                                else
                                {
                                    item.Quantity = subItem.Quantity * subItem.Duration;
                                    item.UnitPrice = subItem.ProRataPrice;
                                    item.NetAmount = subItem.ProRataNetAmount;
                                }


                                item.VatAmount = Convert.ToDecimal(setting.SettingValue);
                                item.BillingType = subItem.BillingType;
                                item.UsageType = prodpackage.Product.UsageType;
                                item.SubscriptionID = sub.Id;
                                //subItem.isBilled = true;
                                //_dbContext.Update(subItem);
                                subTotel += item.NetAmount;
                                objInvoice.InvoiceLineItems.Add(item);
                            }
                        }
                    }
                    objInvoice.SubTotal = subTotel;
                    decimal disprice = objInvoice.SubTotal * objInvoice.Discount / 100;

                    if (setting != null)
                    {
                        objInvoice.VatTotal = Math.Round((objInvoice.SubTotal - disprice) * Convert.ToDecimal(setting.SettingValue) / 100, 2);
                    }
                    objInvoice.Total = (objInvoice.SubTotal - disprice) + objInvoice.VatTotal;
                    if (objInvoice.InvoiceLineItems.Count > 0)
                    {
                        _dbContext.Invoices.Add(objInvoice);
                        _dbContext.SaveChanges();
                        CreateWorkorders(objInvoice);
                        message = "Invoice Generated";

                    }
                    else
                    {
                        message = "Invoice Already Generated";
                    }
                    #region Commented
                    //                    foreach (Subscription subscription in subscriptions)
                    //                    {
                    //                        var invoices = _dbContext.Invoices.Include(m => m.InvoiceLineItems).Where(m => m.SubscriptionID == subscription.Id).FirstOrDefault();
                    //                        if (invoices == null)
                    //                        {
                    //                            lstSubscriptionsForInvoice.Add(subscription);
                    //                            #region commented
                    //                            //var subs = _dbContext.SubscriptionItems.Include(t => t.Subscription).Include(t => t.ProductPackage).ThenInclude(p => p.Product).Where(t => t.SubscriptionId == subscription.Id).FirstOrDefault();


                    //                            //Invoice invoice = new Invoice();
                    //                            //invoice.ID = Guid.NewGuid();
                    //                            //invoice.Date = DateTime.Today;
                    //                            //invoice.CustomerId = subscription.CustomerId;

                    //                            //int count = _dbContext.Invoices.Count();
                    //                            //if (count == 0)
                    //                            //    invoice.InvoiceNumber = 1;
                    //                            //else
                    //                            //    invoice.InvoiceNumber = _dbContext.Invoices.Max(t => t.InvoiceNumber) + 1;
                    //                            //foreach (SubscriptionItem subItem in subs.Subscription.SubscriptionItems.Where(i => i.EndDate > lastday))
                    //                            //{
                    //                            //    InvoiceLineItem item = new InvoiceLineItem();
                    //                            //    item.ID = Guid.NewGuid();
                    //                            //    item.InvoiceID = invoice.ID;
                    //                            //    item.Description = subItem.ProductPackage.Product.Name;
                    //                            //    item.ProductPackageRateID = subItem.ProductPackageId;
                    //                            //    item.BillingType = subItem.BillingType;
                    //                            //    item.UsageType = subItem.ProductPackage.Product.UsageType;
                    //                            //    if (subItem.BillingType == "Monthly")
                    //                            //    {
                    //                            //        item.Quantity = subItem.Quantity;
                    //                            //    }
                    //                            //    else
                    //                            //    {
                    //                            //        item.Quantity = subItem.Quantity * subItem.Duration;
                    //                            //    }
                    //                            //    item.UnitPrice = subItem.Rate;
                    //                            //    item.NetAmount = item.Quantity * item.UnitPrice;
                    //                            //    invoice.InvoiceLineItems.Add(item);

                    //                            //    invoice.SubscriptionID = subItem.SubscriptionId;
                    //                            //}
                    //                            //invoice.SubTotal = invoice.InvoiceLineItems.Sum(t => t.NetAmount);

                    //                            //ApplicationSetting setting = _dbContext.ApplicationSetting.FirstOrDefault(t => t.SettingName == "VAT %");
                    //                            //if (setting != null)
                    //                            //{
                    //                            //    invoice.VatTotal = invoice.SubTotal * Convert.ToDecimal(setting.SettingValue);
                    //                            //}

                    //                            //invoice.Total = invoice.SubTotal + invoice.VatTotal;

                    //                            //_dbContext.Invoices.Add(invoice);
                    //                            //_dbContext.SaveChanges();
                    //#endregion
                    //                        }
                    //                        else
                    //                        {

                    //#region
                    //                            //var subs = _dbContext.SubscriptionItems.Include(t => t.Subscription).Include(t => t.ProductPackage).ThenInclude(p => p.Product).Where(t => t.SubscriptionId == subscription.Id && t.EndDate > lastday).FirstOrDefault();
                    //                            //var invoicesgen = _dbContext.Invoices.Include(m => m.InvoiceLineItems).Where(m => m.SubscriptionID == subs.SubscriptionId && m.Date.Month == lastday.Month && m.BillingType == "Monthly").FirstOrDefault();
                    //                            //if (invoicesgen == null )
                    //                            //{
                    //                            //    Invoice invoice = new Invoice();
                    //                            //    invoice.ID = Guid.NewGuid();
                    //                            //    invoice.Date = DateTime.Today;
                    //                            //    invoice.CustomerId = subscription.CustomerId;

                    //                            //    int count = _dbContext.Invoices.Count();
                    //                            //    if (count == 0)
                    //                            //        invoice.InvoiceNumber = 1;
                    //                            //    else
                    //                            //        invoice.InvoiceNumber = _dbContext.Invoices.Max(t => t.InvoiceNumber) + 1;
                    //                            //    foreach (SubscriptionItem subItem in subs.Subscription.SubscriptionItems.Where(i => i.EndDate > lastday))
                    //                            //    {
                    //                            //        InvoiceLineItem item = new InvoiceLineItem();
                    //                            //        item.ID = Guid.NewGuid();
                    //                            //        item.InvoiceID = invoice.ID;
                    //                            //        item.Description = subItem.ProductPackage.Product.Name;
                    //                            //        item.ProductPackageRateID = subItem.ProductPackageId;
                    //                            //        item.BillingType = subItem.BillingType;
                    //                            //        item.UsageType = subItem.ProductPackage.Product.UsageType;
                    //                            //        if (subItem.BillingType == "Monthly")
                    //                            //        {
                    //                            //            item.Quantity = subItem.Quantity;
                    //                            //        }
                    //                            //        else
                    //                            //        {
                    //                            //            item.Quantity = subItem.Quantity * subItem.Duration;
                    //                            //        }
                    //                            //        item.UnitPrice = subItem.Rate;
                    //                            //        item.NetAmount = item.Quantity * item.UnitPrice;
                    //                            //        invoice.InvoiceLineItems.Add(item);

                    //                            //        invoice.SubscriptionID = subItem.SubscriptionId;
                    //                            //    }
                    //                            //    invoice.SubTotal = invoice.InvoiceLineItems.Sum(t => t.NetAmount);

                    //                            //    ApplicationSetting setting = _dbContext.ApplicationSetting.FirstOrDefault(t => t.SettingName == "VAT %");
                    //                            //    if (setting != null)
                    //                            //    {
                    //                            //        invoice.VatTotal = invoice.SubTotal * Convert.ToDecimal(setting.SettingValue);
                    //                            //    }

                    //                            //    invoice.Total = invoice.SubTotal + invoice.VatTotal;

                    //                            //    _dbContext.Invoices.Add(invoice);
                    //                            //    _dbContext.SaveChanges();
                    //                            //}
                    //#endregion
                    //                        }
                    //                    }
                    #endregion

                }
                return message;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                message = ex.Message;
                return message;
            }

        }
        
        private async Task<ActionResult<string>> SavePdfAttachment([FromBody]EmailProperty emailProperty)
        {
            string Message = "";
            try
            {
                string folderName = "ImagesforMail";
                string webRootPath = _hostingEnvironment.WebRootPath;
                string newPath = Path.Combine(webRootPath, folderName);
                string fullPath = Path.Combine(newPath, emailProperty.fileName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                else
                {
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                byte[] imageBytes = Convert.FromBase64String(emailProperty.base64);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                using (FileStream fs = new FileStream(fullPath, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 5f, 6f, 28f, 2f);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    image.ScaleToFit(600, 950);
                    document.Open();
                    document.Add(image);
                    document.Close();
                }
                Message = "Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Message;
        }

        public List<CustomerVModel> getValidCustomers(string styear, string stmonth)
        {

            List<CustomerVModel> lst = new List<CustomerVModel>();

            DateTime currentdate = new DateTime(Convert.ToInt32(styear), Convert.ToInt32(stmonth), 1);
            DateTime endDate = new DateTime(Convert.ToInt32(styear), Convert.ToInt32(stmonth), 1).AddMonths(1).AddDays(-1);

            List<Subscription> lstsubscriptions = _dbContext.Subscriptions.Include(t => t.SubscriptionItems).Include(t => t.Customer)
                                                  .Where(t => t.SubscriptionItems.Any(i => i.EndDate >= endDate) && t.IsAutoBilled == true).ToList();

            List<Invoice> lstInvocice = new List<Invoice>();
            lstInvocice = _dbContext.Invoices.Include(m => m.InvoiceLineItems).Where(t => t.InvoiceDate == currentdate).ToList();
            bool found = false;
            DateTime tempDt;
            foreach (var sub in lstsubscriptions)
            {
                foreach (var item in sub.SubscriptionItems)
                {
                    tempDt = item.StartDate.AddDays(-1 * item.StartDate.Day);
                    if (item.isBilled == null) item.isBilled = false;
                    // 01 Nov -- 16 Nov
                    // 01 Dec
                    // 01 Oct
                    if (currentdate > tempDt)
                    {

                        if (item.BillingType == "Monthly" || (item.BillingType == "One Time Payment" && item.isBilled != true))
                        {
                            found = false;
                            foreach (var l in lstInvocice)
                            {
                                //if(l.isCancelled == false)
                                //{
                                foreach (var it in l.InvoiceLineItems)
                                {
                                    if (it.SubscriptionID == sub.Id)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                //}else if(l.isCancelled == true)
                                //{
                                //    foreach (var it in l.InvoiceLineItems)
                                //    {
                                //        if (it.SubscriptionID == sub.Id)
                                //        {
                                //            found = false;

                                //        }
                                //    }
                                //}

                            }
                            if (!found)
                            {
                                if (lst.Where(t => t.CustomerId == sub.CustomerId).FirstOrDefault() == null)
                                {
                                    var cust = new CustomerVModel();

                                    cust.CustomerId = sub.CustomerId;
                                    cust.CustomerName = sub.Customer.TradingName;
                                    cust.RegistrationNumber = sub.Customer.RegistrationNumber;
                                    cust.isSelected = false;
                                    lst.Add(cust);
                                }
                            }
                        }
                    }

                }

            }
            return lst;
        }
        public List<ProductPackageRate> PackageRates(Guid ProductId)
        {
            List<ProductPackageRate> rates = new List<ProductPackageRate>();
            try
            {
                rates = _dbContext.ProductPackageRates.Where(m => m.ProductId == ProductId).ToList();

            }
            catch (Exception ex)
            {

            }
            return rates;
        }
        public ProductPackageRate rate(Guid id)
        {
            ProductPackageRate prdrate = new ProductPackageRate();
            prdrate = _dbContext.ProductPackageRates.Where(m => m.Id == id).FirstOrDefault();
            return prdrate;
        }
        public string GetVat()
        {
            var appsetting = _dbContext.ApplicationSetting.Where(m => m.SettingName.ToLower() == "VAT %".ToLower()).Select(m => m.SettingValue).FirstOrDefault();
            return appsetting;
        }
        public InvoiceCrudResponses UpdateInvoice(Invoice data)
        {
            InvoiceCrudResponses response = new InvoiceCrudResponses();
            try
            {
                foreach (var invitem in data.InvoiceLineItems)
                {
                    _dbContext.InvoiceLineItems.Update(invitem);
                }

                _dbContext.Invoices.Update(data);
                _dbContext.SaveChanges();
                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;

        }
        public async Task<string> SendMail(string toEmail, string attachmentFilePath, bool ispaid, Guid invoiceid, bool istaxinv)
        {
            EmailTemplate email = _dbContext.EmailTemplates.FirstOrDefault(t => t.Type == "Tax Invoice");
            string Message = "";
            if (!String.IsNullOrEmpty(toEmail))
            {
                var builder = new BodyBuilder();
                builder.HtmlBody += email.MailContent;
                var subject = ispaid ? "Tax Invoice" : "Proforma Invoice";
                Message = await _emailService.SendEmailAsync(toEmail, subject, "", attachmentFilePath, "", builder,"invoice");
                if (Message == "Mail has been sent")
                {
                    var invoice = await _dbContext.Invoices.FindAsync(invoiceid);
                    if (istaxinv == true)
                        invoice.IsTaxinvSent = true;
                    else
                        invoice.IsEmailSend = true;
                    invoice.EmailSendDate = DateTime.Now;
                    _dbContext.Invoices.Update(invoice);
                    await _dbContext.SaveChangesAsync();
                }
            }
            else
            {
                Message = "No contents founds on Email Template";
            }
            return Message;
        }
        public async Task<List<InvoiceBulkEmail>> GetInvoiceBulkEmail(string styear, string stmonth)
        {
            List<InvoiceBulkEmail> lst = new List<InvoiceBulkEmail>();
            try
            {
                DateTime currentdate = new DateTime(Convert.ToInt32(styear), Convert.ToInt32(stmonth), 1);
                DateTime endDate = new DateTime(Convert.ToInt32(styear), Convert.ToInt32(stmonth), 1).AddMonths(1).AddDays(-1);
                var Invoices = await _dbContext.Invoices
                                            .Include(p => p.InvoiceLineItems)
                                            .Where(w => w.Date >= currentdate && w.Date <= endDate && w.isCancelled != true).ToListAsync();
                lst = (from i in Invoices
                       join c in _dbContext.Customers on
                       i.CustomerId equals c.Id
                       select new InvoiceBulkEmail
                       {
                           ClientName = c.TradingName,
                           RefrenceNumber = i.InvoiceDisplayNumber,
                           Address = c.PhysicalAddress,
                           Date = i.Date.ToString("dd/MM/yyyy"),
                           Code = c.Code,
                           VatNumber = c.VATNumber,
                           TelePhoneNumber = c.TelephoneNumber,
                           Email = c.BillingEmail,
                           Comments = i.Remarks,
                           isPaid = i.ispaid,
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
                           invoice = i,
                           Invoicevalue = i.Total,
                           isSelected = false,
                       }).ToList();
            }
            catch (Exception ex) { }
            return lst;
        }
    }
    public class InvoiceDataResponse : DataTableResponse<InvoiceVm>
    {

    }
    public class InvoiceVm
    {
        public Guid ID { get; set; }
        public int InvoiceNumber { get; set; }
        public bool ProFormaInvoice { get; set; }
        public DateTime Date { get; set; }
        public DateTime EmailDate { get; set; }
        public DateTime PaymentReceivedDate { get; set; }
        public int ReferenceNumber { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal VatTotal { get; set; }
        public decimal Total { get; set; }
        public string BillingType { get; set; }
        public string TradingName { get; set; }
        public string CustomerCode { get; set; }
        public List<InvoiceLineItem> InvoiceLineItems { get; set; } = new List<InvoiceLineItem>();
        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public Subscription Subscription { get; set; }
        public Guid? SubscriptionID { get; set; }
        public bool isCancelled { get; set; }
        public bool isPayed { get; set; }
        public bool IsCreditNoteRaised { get; set; }
        public string CreditNoteNumber { get; set; }
        public decimal CreditNoteTotal { get; set; }
        public string msg { get; set; }
        public decimal? PaymentValue { get; set; }
        public DateTime InvoiceDate { get; set; }
        public bool IsProformaInvoice { get; set; }
        public string InvoiceDisplayNumber { get; set; }
        public bool IsTacinvSent { get; set; }

    }
    public class InvoiceCrudResponses
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
    public class PaymentCrudResponses
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
    public class ProductPackageRateVm
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public int MinLimit { get; set; }

        public int MaxLimit { get; set; }

 

        public decimal UnitPrice { get; set; }
    }
    public class ProductVm
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<ProductPackageRateVm> PackageRates { get; set; } = new List<ProductPackageRateVm>();

        public Service Service { get; set; }

        public Guid ServiceId { get; set; }

        public string UsageType { get; set; } // monthly,yearly,credits


    }
    public class CustomerVModel
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string RegistrationNumber { get; set; }
        public bool isSelected { get; set; }

    }
    public class TaxInvoiceReport
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
        public Invoice invoice { get; set; }
    }
    public class InvoiceBulkEmail
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
        public decimal Invoicevalue { get; set; }
        public bool isSelected { get; set; }
        public bool isPaid { get; set; }
        public Invoice invoice { get; set; }
    }
    public class InvoiceGenerationVm
    {
        public string StartMonth { get; set; }
        public string StartYear { get; set; }
        public string date { get; set; }
        public List<CustomerVModel> customers { get; set; } = new List<CustomerVModel>();
    }
    public class InvoiceDataTableRequest
    {
        public string fromdate { get; set; }
        public string todate { get; set; }
        public Guid customerId { get; set; }
        public string ispaid { get; set; }
        public bool IsCreditNoteRaised { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }
    public class CreditNote
    {
        public Guid Id { get; set; }
        public Invoice Invoice { get; set; }
        public Guid InvoiceId { get; set; }
        public int CreditNoteNumber { get; set; }
        public decimal CreditNoteValue { get; set; }
        public DateTime CreditNoteDate { get; set; }

        public string Comments { get; set; }
    }
    public class CrudResponseCreditnote
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}

