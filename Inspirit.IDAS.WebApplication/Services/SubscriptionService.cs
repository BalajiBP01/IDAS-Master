using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.IDAS;
using Inspirit.IDAS.Data.Production;
using Inspirit.IDAS.WebApplication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication
{
    public class SubscriptionService
    {
        ProductionDbContext _ProductionDbContext;
        IDASDbContext _IDAScontext;
        public SubscriptionService(ProductionDbContext ProductionDbContext, IDASDbContext IDASDbContext)
        {
            _ProductionDbContext = ProductionDbContext;
            _IDAScontext = IDASDbContext;
        }
        public async Task<SubscriptionDataTableRespone> SubscriptionList(SubDataTableRequest request)
        {
            SubscriptionDataTableRespone response = new SubscriptionDataTableRespone();
            try
            {

                var sublsts = _IDAScontext.Subscriptions.ToList();



                SqlConnection con = (SqlConnection)_IDAScontext.Database.GetDbConnection();
                using (con)
                {
                    if (con.State != ConnectionState.Open)
                        con.Open();
                    DataTable dataTable = new DataTable();
                    SqlCommand cmd = new SqlCommand("qspSubscriptionDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Customerid", request.CustomerId));
                    cmd.CommandTimeout = 0;
                    try
                    {
                        if (con.State != ConnectionState.Open)
                            con.Open();
                        SqlDataAdapter sDA = new SqlDataAdapter();
                        sDA.SelectCommand = cmd;
                        sDA.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dataTable.Rows)
                            {
                                SubscriptionDataTable subdata = new SubscriptionDataTable();

                                subdata.CustomerId = (Guid)dr["CustomerId"];
                                subdata.Id = (Guid)dr["Id"];
                                subdata.IsAutoBilled = (bool)dr["IsAutoBilled"];
                                string licensed = (string)dr["UsageType"];
                                subdata.IsLicenced = licensed == "Credits" ?
                                                      false : true;
                                bool Paid = (bool)dr["isPaid"];
                                subdata.IsPaid = (Paid == true ? ((Guid)dr["CustomerId"]).ToString() : "");
                                subdata.NumberofAssign = (int)dr["Avialable"];
                                subdata.NumberOfUsers = (int)dr["NumberOfUsersAssigned"];
                                subdata.ProductName = (string)dr["Name"];
                                subdata.SubscriptionDate = (DateTime)dr["StartDate"];
                                if (dr["SubDisplayNumber"] != DBNull.Value)
                                {
                                    subdata.Number = (string)dr["SubDisplayNumber"];
                                }
                                else
                                {
                                    subdata.Number = sublsts.Where(t => t.Id == subdata.Id).Select(t => t.Number.ToString()).FirstOrDefault();
                                }
                                subdata.UsageType = licensed == "Credits" ?
                                                      "Credits" : "Licensed";
                                response.data.Add(subdata);
                            }
                        }


                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<List<SubscriptionLicenceVm>> GetSubscribtionUsers(Guid guid)
        {
            List<SubscriptionLicenceVm> response = new List<SubscriptionLicenceVm>();
            List<SubscriptionLicenceVm> response2 = new List<SubscriptionLicenceVm>();
            try
            {

                var subscription = _IDAScontext.Subscriptions.Include(m => m.SubscriptionItems).Where(m => m.Id == guid).FirstOrDefault();
                var custUsers = _IDAScontext.CustomerUsers.Where(t => t.CustomerId == subscription.CustomerId && t.Status.ToUpper() == "Active".ToUpper()).ToList();
                var filtercusts = custUsers.Where(t => t.SubscriptionId == guid || t.SubscriptionId == null).ToList();
                Guid subitemId = subscription.SubscriptionItems.FirstOrDefault(t => t.SubscriptionId == subscription.Id).Id;

                var subscriptionLicenses = _IDAScontext.SubscriptionLicences.Where(t => t.SubscriptionItemId == subitemId).ToList();

                response = (from cu in filtercusts
                            join sl in subscriptionLicenses on cu.Id equals sl.CustomerUserId
                            into tmpsl
                            select new SubscriptionLicenceVm
                            {
                                SubscriptionId = guid,
                                CustomerUserId = cu.Id,
                                CustomerName = cu.FirstName + "  " + cu.LastName,

                            }
                           ).ToList();
                foreach (var res in response)
                {
                    var act = subscriptionLicenses.Where(t => t.CustomerUserId == res.CustomerUserId && t.IsActive == true).FirstOrDefault();
                    if (act != null)
                    {
                        res.IsActive = true;
                    }
                    else
                    {
                        res.IsActive = false;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return response;
        }
        public async Task<string> AssignLicencetoUsers(SubscriptionLicenceRequest subscriptionLicenceRequest)
        {
            string Message = "";
            bool isBreak = false;
            List<SubscriptionLicense> subscriptionLicences = new List<SubscriptionLicense>();
            try
            {
                var SubscriptionItems = _IDAScontext.SubscriptionItems
                                             .Include(sb => sb.Subscription)
                                             .Include(prrate => prrate.ProductPackage)
                                             .Include(sbprrate => sbprrate.ProductPackage.Product)
                                             .ToList();

                int isActiveUsersCount = subscriptionLicenceRequest
                                .subscriptionLicenceVms
                                .Where(r => r.IsActive == true)
                                .ToList().Count();

                foreach (var sID in subscriptionLicenceRequest.subscriptionLicenceVms)
                {
                    var data = (from s in SubscriptionItems
                                where s.SubscriptionId == sID.SubscriptionId
                                select new
                                {
                                    s.Id,
                                    sID.SubscriptionId,
                                    s.Duration,
                                    s.Quantity,
                                    s.ProductPackage.Product.UsageType
                                }).ToList();
                    var _cUser = _IDAScontext.CustomerUsers.Find(sID.CustomerUserId);
                    foreach (var sitemID in data)
                    {
                        if (sitemID.UsageType != "Credits")
                        {
                            if (isActiveUsersCount > sitemID.Quantity)
                            {
                                Message = "This subscription only " + sitemID.Quantity + " users allowed";
                                isBreak = true;
                                break;
                            }
                        }
                        var _sLicence = await _IDAScontext.SubscriptionLicences
                            .FirstOrDefaultAsync(m => m.SubscriptionItemId == sitemID.Id
                            && m.CustomerUserId == sID.CustomerUserId);

                        if (_sLicence != null)
                        {
                            _sLicence.IsActive = sID.IsActive;
                            _IDAScontext.SubscriptionLicences.Update(_sLicence);

                            if (!sID.IsActive)
                                _cUser.SubscriptionId = null;
                            else
                                _cUser.SubscriptionId = sID.SubscriptionId;

                            _IDAScontext.CustomerUsers.Update(_cUser);
                        }
                        else
                        {
                            if (sID.IsActive)
                            {
                                _sLicence = new SubscriptionLicense();
                                _sLicence.ID = Guid.NewGuid();
                                _sLicence.SubscriptionItemId = sitemID.Id;
                                _sLicence.CustomerUserId = sID.CustomerUserId;
                                _sLicence.AssignedDate = DateTime.Now;
                                _sLicence.IsActive = sID.IsActive;
                                subscriptionLicences.Add(_sLicence);

                                _cUser.SubscriptionId = sID.SubscriptionId;
                                _IDAScontext.CustomerUsers.Update(_cUser);
                            }
                        }
                        await _IDAScontext.SaveChangesAsync();
                    }
                    if (isBreak)
                        break;
                }
                if (subscriptionLicences.Count() > 0)
                {
                    await _IDAScontext.SubscriptionLicences.AddRangeAsync(subscriptionLicences);
                    await _IDAScontext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Message;
        }
        public async Task<List<ProductPackageRate>> GetServices()

        {
            List<ProductPackageRate> services1 = new List<ProductPackageRate>();
            try
            {
                services1 = await _IDAScontext.ProductPackageRates.Include(t => t.Product).Include(t => t.Product.Service).ToListAsync();
               
            }
            catch (Exception ex)
            {
                var masg = ex.Message;
            }

            return services1;


        }
        public async Task<List<ProductsVm>> GetSubscription()
        {
            List<Product> products = new List<Product>();
            List<ProductsVm> list = new List<ProductsVm>();
            try
            {
                var prod = await _IDAScontext.Products.Include(m => m.PackageRates).Where(m => m.Status == true).ToListAsync();
                foreach (var product in prod)
                {
                    var pro = new ProductsVm();
                    pro.Id = product.Id;
                    pro.Name = product.Name;
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
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            return list;
        }
        public void CreateSubscription(ProductsVm vm, Guid customerId)
        {
            Data.Subscription subscription = new Data.Subscription();
            subscription.Id = Guid.NewGuid();
            subscription.Date = DateTime.Today;
            subscription.CustomerId = customerId;
            if (vm.Duration == 0)
                vm.Duration = 1;

            int count = _IDAScontext.Subscriptions.Count();
            if (count == 0)
                subscription.Number = 1;
            else
                subscription.Number = _IDAScontext.Subscriptions.Max(t => t.Number) + 1;


            SubscriptionItem item = new SubscriptionItem();
            item.Id = Guid.NewGuid();
            item.SubscriptionId = subscription.Id;
            if (vm.StartDate != null)
            {
                item.StartDate = DateTime.Parse(vm.StartDate, CultureInfo.GetCultureInfo("en-gb"));
            }

            decimal rate;
            int min = vm.rates.Where(t => t.MinLimit > 0).Min(t => t.MinLimit);
            int max = vm.rates.Where(t => t.MinLimit > 0).Max(t => t.MaxLimit);
            if (vm.Quantity < min)
            {
                var rateobj = vm.rates.FirstOrDefault(t => t.MinLimit == min);
                item.ProductPackageId = rateobj.Id;
                rate = rateobj.UnitPrice;
            }
            else if (vm.Quantity >= max)
            {
                var rateobj = vm.rates.FirstOrDefault(t => t.MinLimit == max);
                item.ProductPackageId = rateobj.Id;
                rate = rateobj.UnitPrice;
            }
            else
            {
                var rateobj = vm.rates.FirstOrDefault(t => t.MinLimit <= vm.Quantity && t.MaxLimit >= vm.Quantity);
                item.ProductPackageId = rateobj.Id;
                rate = rateobj.UnitPrice;
            }
            // var rate = vm.rates.FirstOrDefault(t=>t.MinLimit>)

            int lastday = new DateTime(item.StartDate.Year, item.StartDate.Month, 1).AddMonths(1).AddDays(-1).Day;

            int days = lastday - 2;
            item.Rate = rate * 30 / days;
            item.Quantity = vm.Quantity;
            item.Duration = vm.Duration;
            item.BillingType = vm.BillingType;

            var enddate = _IDAScontext.Products.Where(m => m.Id == vm.Id).FirstOrDefault();
            if (enddate.UsageType == "Monthly")
            {
                item.EndDate = item.StartDate.AddMonths(item.Duration);
            }
            else if (enddate.UsageType == "Yearly")
            {
                item.EndDate = item.StartDate.AddYears(item.Duration);
            }
            else if (enddate.UsageType == "Credits")
            {
                string date = "09/09/9999";
                item.EndDate = DateTime.Parse(date, CultureInfo.GetCultureInfo("en-gb"));
            }


            //item.EndDate = 

            subscription.SubscriptionItems.Add(item);


            _IDAScontext.Subscriptions.Add(subscription);
            _IDAScontext.SaveChanges();

        }
    
        public async Task<List<ProductPackageRate>> GetProducts(Guid id)
        {
            List<ProductPackageRate> prods = new List<ProductPackageRate>();
            prods = _IDAScontext.ProductPackageRates.Where(m => m.ProductId == id).ToList();
            return prods;
        }
        public async Task<SubscriptionVm> GetSubscriptionDetail(Guid id)
        {
            SubscriptionVm vm = new SubscriptionVm();
            var res = _IDAScontext.SubscriptionItems.Include(m => m.Subscription).Where(m => m.SubscriptionId == id).FirstOrDefault();
            vm.SubscriptionId = res.SubscriptionId;
            vm.Quantity = res.Quantity;
            vm.StartDate = res.StartDate;
            vm.Duration = res.Duration;
            vm.BillingType = res.BillingType;

            var prod = _IDAScontext.ProductPackageRates.Include(m => m.Product).Where(m => m.Id == res.ProductPackageId).FirstOrDefault();
            vm.ProductId = prod.ProductId;
            vm.ProductName = prod.Product.Name;

            return vm;
        }
    }
}
public class SubDataTableRequest
{
    public Guid CustomerId { get; set; }
    public Guid SubscriptionId { get; set; }
    public DataTableRequest dtRequest { get; set; }
}
public class ProductsVm
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsSelect { get; set; }
    public List<ProductPackageRate> rates { get; set; } = new List<ProductPackageRate>();
    public string BillingType { get; set; }//monthly or yearlly
    public int Duration { get; set; }
    public int Quantity { get; set; }
    public string StartDate { get; set; }

}
public class PackageRateVm
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int MinLimit { get; set; }
    public int MaxlLimit { get; set; }
    public decimal Price { get; set; }
    public bool IsSelect { get; set; }
}
public class SubscriptionDataTableRespone : DataTableResponse<SubscriptionDataTable>
{

}
public class SubscriptionDataTable
{
    public Guid Id { get; set; }
    public DateTime SubscriptionDate { get; set; }
    public string ProductName { get; set; }
    public string IsPaid { get; set; }
    public string UsageType { get; set; }
    public int NumberofAssign { get; set; }
    public string Number { get; set; }
    public Guid CustomerId { get; set; }
    public int NumberOfUsers { get; set; }
    public bool IsAutoBilled { get; set; }
    public bool IsLicenced { get; set; }
}
public class SubscriptionLicenceVm
{
    public Guid? SubscriptionId { get; set; }
    public Guid CustomerUserId { get; set; }
    public string CustomerName { get; set; }
    public bool IsActive { get; set; }
}
public class SubscriptionLicenceRequest
{
    public List<SubscriptionLicenceVm> subscriptionLicenceVms = new List<SubscriptionLicenceVm>();
}
public class SubscriptionVm
{
    public Guid SubscriptionId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public DateTime StartDate { get; set; }
    public string BillingType { get; set; }// Monthly yearly
    public int Duration { get; set; }
    public int Quantity { get; set; }
}
public class SubscriptionItemVm
{
    public Guid Id { get; set; }

    public ProductPackageRate ProductPackage { get; set; }

    public Guid ProductPackageId { get; set; }


    public string Status { get; set; }


    public DateTime StartDate { get; set; }


    public string BillingType { get; set; }// Monthly yearly

    public int Duration { get; set; }

    public int Quantity { get; set; }

    public decimal Rate { get; set; }

    public List<SubscriptionItem> subscriptionItems = new List<SubscriptionItem>();
    public Guid SubscriptionId { get; set; }



}
public class SubscriptionResponse
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
}