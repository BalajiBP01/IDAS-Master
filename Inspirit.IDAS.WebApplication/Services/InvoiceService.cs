using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.EntityFrameworkCore;

namespace Inspirit.IDAS.WebApplication
{
    public class InvoiceService
    {
        IDASDbContext _dbContext;

        public InvoiceService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ActionResult<InvoiceDataResponse>> InvoiceDataList(InvDataTableRequest request)
        {
            InvoiceDataResponse response = new InvoiceDataResponse();
            try
            {
                var Inv = _dbContext.Invoices.Where(m => m.CustomerId == request.CustomerId).ToList();
                var lst = _dbContext.Invoices.Where(m => m.CustomerId == request.CustomerId).AsQueryable();
                int count = Inv.Count();               
                var flt = lst;
                if (request.dtRequest.search != null && !string.IsNullOrEmpty(request.dtRequest.search.value))
                {
                    foreach (var invs in Inv)
                    {
                        flt = _dbContext.Invoices.Where(t => t.CustomerId == request.CustomerId);
                    }
                   
                }

                response.recordsTotal = count;
                response.recordsFiltered = flt.Count();

                var data = (from s in flt.Skip(request.dtRequest.start).Take(10)
                            select new InvoiceVm
                            {
                                ID = s.ID,
                                InvoiceNumber = s.InvoiceNumber,
                                Date = s.Date,
                                ProFormaInvoice = s.ProFormaInvoice,
                                ReferenceNumber = s.ReferenceNumber,
                                Total = s.Total,
                                BillingType = s.BillingType,
                                SubTotal = s.SubTotal,
                                VatTotal = s.VatTotal,
                                CustomerId = s.CustomerId,
                                DisplayNumber = s.InvoiceDisplayNumber,
                                InvoiceDate = s.InvoiceDate,
                                isCancelled = s.isCancelled,
                                IsProformaInvoice = s.ProFormaInvoice
                            }).ToList();

                foreach(var d in data)
                {
                    var invoiceid = _dbContext.Invoices.Where(m => m.ID == d.ID).FirstOrDefault();
                    if(invoiceid == null)
                    {
                        d.isPayed = "Pay";
                    }
                    else
                    {
                        d.isPayed = "Payed";
                    }
                }
                response.data = data;

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<ActionResult<InvoiceVm>> InvoiceDataListdata(Guid Id)
        {
            InvoiceVm response = new InvoiceVm();
            try
            {
                var invoice = _dbContext.Invoices.Include(m=>m.InvoiceLineItems).Where(m => m.ID == Id).FirstOrDefault();

               

                response.ID = invoice.ID;
                response.InvoiceNumber = invoice.InvoiceNumber;
                response.ProFormaInvoice = invoice.ProFormaInvoice;
                response.ReferenceNumber = invoice.ReferenceNumber;
                response.Date = invoice.Date;
                response.Discount = invoice.Discount;
                response.SubTotal = invoice.SubTotal;
                response.VatTotal = invoice.VatTotal;
                response.Total = invoice.Total;
                foreach(var lineitems in invoice.InvoiceLineItems )
                {
                    InvoiceLineItem item = new InvoiceLineItem();
                    item.ID = lineitems.ID;
                    item.NetAmount = lineitems.NetAmount;
                    item.Quantity = lineitems.Quantity;
                    item.Description = lineitems.Description;
                    item.UnitPrice = lineitems.UnitPrice;
                    item.VatAmount = lineitems.VatAmount;
                    response.InvoiceLineItems.Add(item);

                }
                
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public List<Customer> GetCustomerList()
        {
            List<Customer> list = new List<Customer>();
            list = _dbContext.Customers.ToList();
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
        public ProductPackageRate rate(Guid id)
        {
            ProductPackageRate prdrate = new ProductPackageRate();
            prdrate = _dbContext.ProductPackageRates.Where(m => m.Id == id).FirstOrDefault();
            return prdrate;
        }

        public Invoice View(Guid id)
        {
            Invoice data = new Invoice();
            try
            {
                data = _dbContext.Invoices.Where(m => m.ID == id).Include(m => m.InvoiceLineItems).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }
            return data;
        }

        public PaymentResponse CreatePayment(Guid invoiceId, string paymentType)
        {
            Invoice invoice = _dbContext.Invoices.Include(t=>t.InvoiceLineItems).FirstOrDefault(t=>t.ID==invoiceId);

            var ispayed = _dbContext.Payments.Where(m => m.InvoiceId == invoiceId).FirstOrDefault();
            PaymentResponse response = new PaymentResponse();
            if (ispayed == null)
            {
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

               
                response.Message = "Payment done";
                response.isSuccess = true;
                CreateWorkorders(invoice);
                return response;
            }
            else
            {
                //error msg
                response.Message = "Payment already done";
                response.isSuccess = false;
                return response;
                
            }
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



        private void CreateWorkorders(Invoice invoice)
        {
            try
            {
                List<SubscriptionItem> items = new List<SubscriptionItem>();

                if (invoice.SubscriptionID.HasValue)
                {
                    items = _dbContext.SubscriptionItems.Include(t=>t.ProductPackage).ThenInclude(p=>p.Product).Where(t => t.SubscriptionId == invoice.SubscriptionID).ToList();
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
            catch(Exception ex)
            {

            }

        }
    }

    public class InvoiceDataResponse : DataTableResponse<InvoiceVm>
    {

    }

    public class InvoiceCrudResponses
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }
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
    public class ProductPackageRateVm
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public int MinLimit { get; set; }

        public int MaxLimit { get; set; }


        public decimal UnitPrice { get; set; }
    }
    
    public class InvoiceVm
    {
        public string Name { get; set; }
        public Guid ID { get; set; }

        public int InvoiceNumber { get; set; }

        public bool ProFormaInvoice { get; set; }

        public DateTime Date { get; set; }

        public int ReferenceNumber { get; set; }

        public decimal SubTotal { get; set; }

        public decimal Discount { get; set; }

        public decimal VatTotal { get; set; }

        public decimal Total { get; set; }

        public string BillingType { get; set; }
        public DateTime EmailDate { get; set; }
        public DateTime PaymentReceivedDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public bool IsProformaInvoice { get; set; }
        public bool isCancelled { get; set; }
        public List<InvoiceLineItem> InvoiceLineItems { get; set; } = new List<InvoiceLineItem>();
        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
    
        public Subscription Subscription { get; set; }

        public Guid? SubscriptionID { get; set; }
        public string isPayed { get; set; }
        public string DisplayNumber { get; set; }


    }


   

    public class InvDataTableRequest
    {
        public Guid CustomerId { get; set; }
        public Guid InvoiceId { get; set; }
        public DataTableRequest dtRequest { get; set; }
    }

    public class PaymentResponse
    {
        public string Message { get; set; }
        public bool isSuccess { get; set; }
    }

}
