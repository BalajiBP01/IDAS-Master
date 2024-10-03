using Inspirit.IDAS.Admin;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin.Services
{
    public class SubscriptionService
    {
        IDASDbContext _IDAScontext;

        public SubscriptionService(IDASDbContext IDASDbContext)
        {
            _IDAScontext = IDASDbContext;
        }
        public async Task<SubscriptionDataTableRespone> SubscriptionList(SubscriptionDataTableRequest request)
        {
            SubscriptionDataTableRespone response = new SubscriptionDataTableRespone();
            try
            {
                DateTime FilterToDate = new DateTime(Convert.ToDateTime(request.todate).Year, Convert.ToDateTime(request.todate).Month, Convert.ToDateTime(request.todate).Day, 23, 59, 59);
                var lst = _IDAScontext.Subscriptions.AsQueryable();
                var flt = lst;
                if (request.customerId != Guid.Empty)
                {
                    lst = lst.Where(c => c.CustomerId == request.customerId);
                }
                flt = lst.Where(t => t.Date >= Convert.ToDateTime(request.fromdate)
                && t.Date <= FilterToDate);

                var data = await (from s in flt
                                  join si in _IDAScontext.SubscriptionItems
                                  on s.Id equals si.SubscriptionId

                                  join pr in _IDAScontext.ProductPackageRates
                                  on si.ProductPackageId equals pr.Id
                                  join p in _IDAScontext.Products on pr.ProductId
                                  equals p.Id
                                  select new SubscriptionVm
                                  {
                                      SubscriptionId = s.Id,
                                      SubscriptionDate = s.Date,
                                      Number = s.Number,
                                      ProductName = p.Name,
                                      CustomerName = s.Customer.TradingName,
                                      StartDate = si.StartDate,
                                      EndDate = si.EndDate,
                                      Quantity = si.Quantity,
                                      UsageType = p.UsageType,
                                      SubDisplayNumber = s.SubDisplayNumber,
                                      NumberofAssign = p.UsageType == "Credits" ?
                                                       0 : si.Quantity
                                  }).ToListAsync();
                response.data = data;
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<List<ProductsVm>> GetSubscription()
        {
            List<Product> products = new List<Product>();
            List<ProductsVm> list = new List<ProductsVm>();

            var prod = await _IDAScontext.Products.Include(m => m.PackageRates).Where(m => m.Status == true).ToListAsync();
            foreach (var product in prod)
            {
                var pro = new ProductsVm();
                pro.Id = product.Id;
                pro.Name = product.UsageType;
                var rates = product.PackageRates.ToList();
                if (rates != null)
                {
                    foreach (var rate in rates)
                    {
                        var pck = new ProductPackageRate();
                        pck.Id = rate.Id;
                        pck.MaxLimit = rate.MaxLimit;
                        pck.MinLimit = rate.MinLimit;
                        pck.UnitPrice = rate.UnitPrice; //to be calculated
                        pck.ProductId = rate.Product.Id;
                        pro.rates.Add(pck);
                    }

                }
                list.Add(pro);
            }
            return list;
        }
        public async Task<List<Customer>> GetCustomerList()
        {
            List<Customer> list = new List<Customer>();
            try
            {
                list = await _IDAScontext.Customers
                    .Where(m => m.Status == "Active").ToListAsync();
            }
            catch (Exception ex) { }
            return list;
        }
        public async Task<Customer> GetCustomerById(Guid customerid)
        {
            Customer customer = new Customer();
            try
            {
                customer = await _IDAScontext.Customers
                    .FirstOrDefaultAsync(m => m.Status == "Active" && m.Id == customerid);
            }
            catch (Exception ex) { }
            return customer;
        }
        public async Task<SubscriptionVm> GetSubscriptionDetail(Guid id)
        {
            SubscriptionVm vm = new SubscriptionVm();
            try
            {

                var res = _IDAScontext.SubscriptionItems.Include(m => m.Subscription).Where(m => m.SubscriptionId == id).FirstOrDefault();
                vm.SubscriptionId = res.SubscriptionId;
                vm.Quantity = res.Quantity;
                vm.StartDate = res.StartDate;
                vm.Duration = res.Duration;
                vm.BillingType = res.BillingType;
                vm.CustomerId = res.Subscription.CustomerId;
                vm.SubDisplayNumber = res.Subscription.SubDisplayNumber;
                var prod = _IDAScontext.ProductPackageRates.Include(m => m.Product).Where(m => m.Id == res.ProductPackageId).FirstOrDefault();

                vm.ProductId = prod.ProductId;
                vm.ProductName = prod.Product.Name;


            }
            catch (Exception ex)
            {

            }
            return vm;
        }
        public SubscriptionResponse CreateSubscription(ProductsVm vm)
        {
            SubscriptionResponse response = new SubscriptionResponse();
            try
            {
                Data.Subscription subscription = new Data.Subscription();
                subscription.Id = Guid.NewGuid();
                subscription.Date = DateTime.Now;
                if (vm.CustomerId != null)
                { subscription.CustomerId = vm.CustomerId; }

                if (vm.Duration == 0)
                    vm.Duration = 1;
                subscription.Number = _IDAScontext.Subscriptions.Count() + 1;

               

                SubscriptionItem item = new SubscriptionItem();
                item.Id = Guid.NewGuid();
                item.SubscriptionId = subscription.Id;
                if (vm.StartDate != null)
                {
                    item.StartDate = DateTime.Parse(vm.StartDate, CultureInfo.GetCultureInfo("en-gb"));
                }

                decimal rate = 0;

                var Prate = vm.rates.Where(p => vm.Quantity >= p.MinLimit && vm.Quantity <= p.MaxLimit && p.IsDeleted == 0).FirstOrDefault();

                if (Prate == null)
                {
                    response.isSuccess = false;
                    response.Message = "Enter the valid Quantity";
                }
                else
                {
                    item.ProductPackageId = Prate.Id;
                    rate = Prate.UnitPrice;
                  
                    item.Rate = rate;
                    item.Quantity = vm.Quantity;
                    item.Duration = vm.Duration;
                    item.BillingType = vm.BillingType;

                    var enddate = _IDAScontext.Products.Where(m => m.Id == vm.Id).FirstOrDefault();
                    if (enddate.UsageType == "Monthly")
                    {

                        if (item.BillingType == "Monthly")
                        {
                            int lastday = new DateTime(item.StartDate.Year, item.StartDate.Month, 1).AddMonths(1).AddDays(-1).Day;

                             
                                item.ProRataPrice = (rate * (lastday - item.StartDate.Day + 1)) / lastday;
                                item.ProRataNetAmount = (rate * (lastday - item.StartDate.Day + 1)) / lastday * item.Quantity;
                        }
                        else if (item.BillingType == "One Time Payment")
                        {
                            int lastdayofmonth = new DateTime(item.StartDate.Year, item.StartDate.Month, 1).AddMonths(1).AddDays(-1).Day;
                            decimal remcal = 0;
                            if (item.Duration > 1)
                            {
                                 remcal = ((item.Duration - 1) * item.Quantity * rate);
                            }
                            var unitPrice = ((rate * (lastdayofmonth - item.StartDate.Day) + 1) / lastdayofmonth);
                            var currentmonthrate = ((rate * (lastdayofmonth - item.StartDate.Day) + 1) / lastdayofmonth) * item.Quantity;
                            item.ProRataPrice = unitPrice;
                            item.ProRataNetAmount = remcal + currentmonthrate;
                        }
                        DateTime EndDatecal = item.StartDate.AddMonths(item.Duration - 1);
                        item.EndDate = new DateTime(EndDatecal.Year, EndDatecal.Month, 1,23,59,59).AddMonths(1).AddDays(-1);
                    }
                    else if (enddate.UsageType == "Yearly")
                    {
                        if (item.BillingType == "Monthly")
                        {
                            int lastday = new DateTime(item.StartDate.Year, item.StartDate.Month, 1).AddMonths(1).AddDays(-1).Day;

                                item.ProRataPrice = (rate * (lastday - item.StartDate.Day) + 1) / lastday;
                                item.ProRataNetAmount = (rate * (lastday - item.StartDate.Day) + 1) / lastday;
                        }
                        else if (item.BillingType == "One Time Payment")
                        {
                            int duration = item.Duration;
                            decimal remcal = 0;
                            if(duration > 1)
                            {
                                remcal = ((item.Duration - 1) * item.Quantity * rate);
                            }
                            var unitPrice = ((rate * (365 - (item.StartDate.Day + 1))) / 365);
                            var currentrate =( (rate * (365 -( item.StartDate.Day +1))) / 365) * item.Quantity;

                            item.ProRataPrice = unitPrice;
                            item.ProRataNetAmount = currentrate + remcal;
                        }

                        DateTime EndDatecal = item.StartDate.AddYears(item.Duration);

                        item.EndDate = new DateTime(EndDatecal.Year, EndDatecal.Month, 1,23,59,59).AddMonths(-1).AddDays(1);
                    }
                    else if (enddate.UsageType == "Credits")
                    {
                        DateTime EndDatecal = Convert.ToDateTime(item.StartDate.AddYears(10));
                        DateTime _enddate = new DateTime(EndDatecal.Year, EndDatecal.Month, 1);
                        item.EndDate = new DateTime(_enddate.Year, _enddate.Month, 1,23,59,59).AddDays(-1);
                        item.ProRataPrice = rate;
                        item.ProRataNetAmount = rate * item.Quantity;
                    }
                    if (enddate.IsPostpaid == true)
                        subscription.IsAutoBilled = false;
                    else
                        subscription.IsAutoBilled = true;

                    item.isBilled = false;
                    subscription.SubscriptionItems.Add(item);

                    subscription.SubDisplayNumber = new displaynumber().GenerateNumber(subscription.Number, enddate.Code.ToUpper(), 10);


                    _IDAScontext.Subscriptions.Add(subscription);
                    _IDAScontext.SaveChanges();
                    response.isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.isSuccess = false;

            }
            return response;
        }
      
        public async Task<List<Product>> GetProducts(Guid id)
        {
            List<Product> prods = new List<Product>();
            prods = await (from p in _IDAScontext.Products
                           join cp in _IDAScontext.CustomerProducts
                           on p.Id equals cp.ProductId
                           where cp.CustomerId == id && p.Status == true
                           && cp.Active == true
                           select p
                          ).ToListAsync();
            return prods;
        }
        public async Task<List<ProductPackageRate>> GetProductsRates(Guid id)
        {
            List<ProductPackageRate> prods = new List<ProductPackageRate>();
            prods = _IDAScontext.ProductPackageRates.Where(m => m.ProductId == id
            && m.IsDeleted == 0).ToList();
            return prods;
        }
        public SubscriptionResponse CreateInvoice(Guid subscriptionId)
        {
            SubscriptionResponse response = new SubscriptionResponse();

            var subscriptions = _IDAScontext.SubscriptionItems.Include(t => t.Subscription).Include(t => t.ProductPackage).ThenInclude(p => p.Product).Where(t => t.SubscriptionId == subscriptionId);



            Invoice invoice = new Invoice();
            invoice.ID = Guid.NewGuid();
            invoice.Date = DateTime.Today;


            int count = _IDAScontext.Invoices.Count();
            if (count == 0)
                invoice.InvoiceNumber = 1;
            else
                invoice.InvoiceNumber = _IDAScontext.Invoices.Max(t => t.InvoiceNumber) + 1;
            foreach (SubscriptionItem subItem in subscriptions)
            {


                if (subItem.EndDate >= DateTime.Now && subItem.StartDate <= DateTime.Now)
                {
                    Invoice invoicegen_mon = new Invoice();
                    Invoice invoicegen = new Invoice();
                    if (subItem.BillingType == "Monthly")
                    {
                        invoicegen_mon = _IDAScontext.Invoices.Where(m => m.SubscriptionID == subItem.SubscriptionId && (m.Date.Month == DateTime.Now.Month)).FirstOrDefault();
                    }
                    else
                    {
                        invoicegen = _IDAScontext.Invoices.Where(m => m.SubscriptionID == subItem.SubscriptionId).FirstOrDefault();
                    }



                    if (invoicegen_mon == null || invoicegen == null)
                    {
                        InvoiceLineItem item = new InvoiceLineItem();
                        item.ID = Guid.NewGuid();
                        item.InvoiceID = invoice.ID;
                        item.Description = subItem.ProductPackage.Product.Name;
                        item.ProductPackageRateID = subItem.ProductPackageId;

                        if (invoicegen_mon == null)
                        {
                            item.Quantity = subItem.Quantity;
                        }
                        else if (invoicegen == null)
                        {
                            item.Quantity = subItem.Quantity * subItem.Duration;
                        }
                      
                        item.UnitPrice = subItem.Rate;
                        item.NetAmount = item.Quantity * item.UnitPrice;
                        invoice.InvoiceLineItems.Add(item);
                        invoice.SubscriptionID = subItem.SubscriptionId;
                        invoice.CustomerId = subItem.Subscription.CustomerId;

                        invoice.SubTotal = invoice.InvoiceLineItems.Sum(t => t.NetAmount);

                        ApplicationSetting setting = _IDAScontext.ApplicationSetting.FirstOrDefault(t => t.SettingName == "VAT %");
                        if (setting != null)
                        {
                            invoice.VatTotal = invoice.SubTotal * Convert.ToDecimal(setting.SettingValue);
                        }

                        invoice.Total = invoice.SubTotal + invoice.VatTotal;

                    }
                    else
                    {
                        // errormsg "Invoice already generated"
                        response.Message = "Invoice already generated";
                        response.isSuccess = false;
                        return response;
                    }
                }
                else
                {
                    // errormsg  "Subscription ended"
                    response.Message = "Subscription ended";
                    response.isSuccess = false;
                    return response;
                }
            }

            _IDAScontext.Invoices.Add(invoice);
            _IDAScontext.SaveChanges();
            response.isSuccess = true;
            response.Message = "Invoice Generated";

            return response;


        }
    }
}
public class SubscriptionDataTableRespone : DataTableResponse<SubscriptionVm>
{

}
public class SubscriptionDataTableRequest
{
    public string fromdate { get; set; }
    public string todate { get; set; }
    public Guid customerId { get; set; }
    public DataTableRequest dtRequest { get; set; }
}
public class SubscriptionResponse
{
    public string Message { get; set; }
    public bool isSuccess { get; set; }
}
public class SubscriptionVm
{
    public Guid SubscriptionId { get; set; }
    public DateTime SubscriptionDate { get; set; }
    public string CustomerName { get; set; }
    public int Number { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Quantity { get; set; }
    public string UsageType { get; set; }
    public int NumberofAssign { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string BillingType { get; set; }// Monthly yearly
    public int Duration { get; set; }
    public Guid CustomerId { get; set; }
    public string SubDisplayNumber { get; set; }
}
public class ProductsVm
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid CustomerId { get; set; }
    public bool IsSelect { get; set; }
    public List<ProductPackageRate> rates { get; set; } = new List<ProductPackageRate>();
    public List<Customer> customers { get; set; } = new List<Customer>();
    public string BillingType { get; set; }//monthly or yearlly
    public int Duration { get; set; }
    public int Quantity { get; set; }
    public string StartDate { get; set; }
    public string StartMonth { get; set; }
    public string StartYear { get; set; }

}
public class InvoiceGenerationVm
{
    public string StartMonth { get; set; }
    public string StartYear { get; set; }
    public string date { get; set; }
    public List<CustomerVModel> customers { get; set; } = new List<CustomerVModel>();
}