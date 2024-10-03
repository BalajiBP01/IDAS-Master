using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;
using Inspirit.IDAS.Data.IDAS;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace Inspirit.IDAS.Data
{
    public class IDASDbContext : DbContext
    {
        public DbSet<CustomerUser> CustomerUsers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerProduct> CustomerProducts { get; set; }
        public DbSet<Creditnote> Creditnotes { get; set; }
        public DbSet<ApplicationMessage> ApplicationMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CustomerLog> CustomerLogs { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<DataServicesAgreement> DataServicesAgreements { get; set; }       
        public DbSet<Product> Products { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ProductPackageRate> ProductPackageRates { get; set; }
        public DbSet<DoNotCallRegistry> DoNotCallRegistrys { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ProFormaInvoice> ProFormaInvoices { get; set; }
        public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
        public DbSet<ProformaInvoiceLineItem> ProformaInvoiceLineItems { get; set; }
        public DbSet<LookupData> LookupDatas { get; set; }
        public DbSet<ApplicationSetting> ApplicationSetting { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<CustomerDSA> CustomerDSAs { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<ProductCategory> ProductCategorys { get; set; }
        public DbSet<ProductDataType> ProductDataTypes { get; set; }
        public DbSet<BatchTrace> BatchTraces { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionItem> SubscriptionItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Workorder> Workorders { get; set; }
        public DbSet<SubscriptionLicense> SubscriptionLicences { get; set; }
        public DbSet<TrailUser> TrailUsers { get; set; }
        public DbSet<TrailUserLog> TrailUserLogs { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<InvoiceAttachment> InvoiceAttachments { get; set; }
        public DbSet<BatchProcessFileGeneration> BatchProcessFileGeneration { get; set; }
        
        public DbSet<LeadsGenaration> LeadsGenaration { get; set; }
        public DbSet<LeadFileGeneration> LeadFileGeneration { get; set; }
        public IDASDbContext(DbContextOptions<IDASDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Krishna
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(cust => cust.Customer)
                .WithMany()
                .HasForeignKey(cust => cust.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(inv => inv.Invoice)
                .WithMany()
                .HasForeignKey(inv => inv.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            });
        }
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            try
            {
                using (var serviceScope = serviceProvider.CreateScope())
                {


                    var context = serviceScope.ServiceProvider.GetService<IDASDbContext>();//serviceProvider.GetService<IDASDbContext>();
                    await context.Database.EnsureCreatedAsync();
                    if (!await context.Customers.AnyAsync())
                    {
                        GetApplicationSetting(context);
                        GetLookupdata(context);
                        GetEmailTemplates(context);

                        GetTestData(context);

                        GetProductData(context);
                    }
                }
            }
            catch (Exception ex)
            {

            }


        }
        private static void GetProductData(IDASDbContext context)
        {
            Service service = new Service();
            service.Id = Guid.Parse("245A63DB-746B-44EB-BDB7-C5F7EEE85CF7");
            service.Name = "Tracing";
            service.Code = "Tracing";
            context.Services.Add(service);

            Service service2 = new Service();
            service2.Id = Guid.Parse("245A63DB-746B-44EB-BDB7-C5F7EEE85CF9");
            service2.Name = "Lead Generation";
            service2.Code = "Leads";
            context.Services.Add(service2);

            Service service3 = new Service();
            service3.Id = Guid.Parse("245A63DB-746B-44EB-BDB7-C5F7EEE85C10");
            service3.Name = "Batch Processing";
            service3.Code = "Batch";
            context.Services.Add(service3);

            Service service4 = new Service();
            service4.Id = Guid.Parse("245A63DB-746B-44EB-BDB7-C5F7EEE85C11");
            service4.Name = "Api";
            service4.Code = "Api";
            context.Services.Add(service4);

           ;

            context.SaveChanges();

            Product product = new Product();
            product.Id = Guid.Parse("6018AD4D-E5A7-4023-82BC-21733BEC9CC1");
            product.Name = "Premium Subscription Monthly";//user licence
            product.ServiceId = service.Id;
            product.UsageType = "Monthly";
            context.Products.Add(product);

            Product product11 = new Product();
            product11.Id = Guid.Parse("6018AD4D-E5A7-4023-82BC-21733BEC9C11");
            product11.Name = "Premium Subscription Yearly";//user licence
            product11.ServiceId = service.Id;
            product11.UsageType = "Yearly";
            context.Products.Add(product11);

            Product product1 = new Product();
            product1.Id = Guid.Parse("6018AD4D-E5A7-4023-82BC-21733BEC9CC2");
            product1.Name = "Prepaid Subscription Points";//points
            product1.ServiceId = service.Id;
            product1.UsageType = "Credits";
            context.Products.Add(product1);

            Product product2 = new Product();
            product2.Id = Guid.Parse("6018AD4D-E5A7-4023-82BC-21733BEC9CC3");
            product2.Name = "Batch tracing";//points
            product2.ServiceId = service2.Id;
            product2.UsageType = "Credits";
            context.Products.Add(product2);

            Product product3 = new Product();
            product3.Id = Guid.Parse("6018AD4D-E5A7-4023-82BC-21733BEC9CC4");
            product3.Name = "Lead generation";//points
            product3.ServiceId = service3.Id;
            product3.UsageType = "Credits";
            context.Products.Add(product3);
            context.SaveChanges();
            

            ProductPackageRate packagerate = new ProductPackageRate();
            packagerate.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664FA7");
            packagerate.MinLimit = 0;
            packagerate.MaxLimit = 1;
            packagerate.UnitPrice = 2300;
            packagerate.ProductId = product.Id;
            context.ProductPackageRates.Add(packagerate);


            ProductPackageRate packagerate111 = new ProductPackageRate();
            packagerate111.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664F17");
            packagerate111.MinLimit = 0;
            packagerate111.MaxLimit = 1;
            packagerate111.UnitPrice = 25000;
            packagerate111.ProductId = product11.Id;
            context.ProductPackageRates.Add(packagerate111);


            ProductPackageRate packagerate1 = new ProductPackageRate();
            packagerate1.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664FA8");
            packagerate1.MinLimit = 2;
            packagerate1.MaxLimit = 10;
            packagerate1.UnitPrice = 2100;

            packagerate1.ProductId = product.Id;
            context.ProductPackageRates.Add(packagerate1);


            ProductPackageRate packagerate12 = new ProductPackageRate();
            packagerate12.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664F28");
            packagerate12.MinLimit = 2;
            packagerate12.MaxLimit = 10;
            packagerate12.UnitPrice = 22000;
            packagerate12.ProductId = product11.Id;
            context.ProductPackageRates.Add(packagerate12);


            ProductPackageRate packagerate2 = new ProductPackageRate();
            packagerate2.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664FA9");
            packagerate2.MinLimit = 10;
            packagerate2.MaxLimit = 0;
            packagerate2.UnitPrice = 1500;

            packagerate2.ProductId = product.Id;
            context.ProductPackageRates.Add(packagerate2);


            ProductPackageRate packagerate222 = new ProductPackageRate();
            packagerate222.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664F29");
            packagerate222.MinLimit = 10;
            packagerate222.MaxLimit = 0;

            packagerate222.UnitPrice = 15000;
            packagerate222.ProductId = product11.Id;
            context.ProductPackageRates.Add(packagerate222);



            ProductPackageRate packagerate3 = new ProductPackageRate();
            packagerate3.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664F10");
            packagerate3.MinLimit = 350;
            packagerate3.MaxLimit = 3000;
            packagerate3.UnitPrice = 2.10m;
            packagerate3.ProductId = product1.Id;
            context.ProductPackageRates.Add(packagerate3);

            ProductPackageRate packagerate4 = new ProductPackageRate();
            packagerate4.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664F11");
            packagerate4.MinLimit = 3001;
            packagerate4.MaxLimit = 6000;
            packagerate4.UnitPrice = 1.80m;
            packagerate4.ProductId = product1.Id;
            context.ProductPackageRates.Add(packagerate4);

            ProductPackageRate packagerate5 = new ProductPackageRate();
            packagerate5.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664F12");
            packagerate5.MinLimit = 60001;
            packagerate5.MaxLimit = 0;
            packagerate5.UnitPrice = 1.25m;
            packagerate5.ProductId = product1.Id;
            context.ProductPackageRates.Add(packagerate5);

            ProductPackageRate packagerate31 = new ProductPackageRate();
            packagerate31.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664F30");
            packagerate31.MinLimit = 100;
            packagerate31.MaxLimit = 1000;
            packagerate31.UnitPrice = 1.3m;
            packagerate31.ProductId = product2.Id;
            context.ProductPackageRates.Add(packagerate31);

            ProductPackageRate packagerate41 = new ProductPackageRate();
            packagerate41.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664F31");
            packagerate41.MinLimit = 1001;
            packagerate41.MaxLimit = 10000;
            packagerate41.UnitPrice = 1.15m;
            packagerate41.ProductId = product2.Id;
            context.ProductPackageRates.Add(packagerate41);

            ProductPackageRate packagerate51 = new ProductPackageRate();
            packagerate51.Id = Guid.Parse("32DA2BB3-D514-4BC0-BEF9-7B38BF664F32");
            packagerate51.MinLimit = 10001;
            packagerate51.MaxLimit = 0;
            packagerate51.UnitPrice = .09m;
            packagerate51.ProductId = product2.Id;
            context.ProductPackageRates.Add(packagerate51);





            context.SaveChanges();
        }

        private static void GetTestData(IDASDbContext context)
        {
            DataServicesAgreement dsa = new DataServicesAgreement();
            dsa.Id = Guid.NewGuid();
            dsa.Version = 1;
            dsa.EffectiveDate = DateTime.Today.AddDays(-10);
            dsa.IsPublished = true;
            context.DataServicesAgreements.Add(dsa);

            context.SaveChanges();


        }
        private static void GetApplicationSetting(IDASDbContext context)
        {
           
            ApplicationMessage msg = new ApplicationMessage();
            msg.Id = Guid.Parse("ABCDBB78-1F24-4398-B3E2-4385CDC50111");
            msg.Message = "Demo message1";
            msg.Showmessage = true;
            context.ApplicationMessages.Add(msg);

            msg = new ApplicationMessage();
            msg.Id = Guid.Parse("ABCDBB78-1F24-4398-B3E2-4385CDC50112");
            msg.Message = "Demo message2";
            msg.Showmessage = true;
            context.ApplicationMessages.Add(msg);

            ApplicationSetting setting = new ApplicationSetting();
            setting.Id = Guid.Parse("D33DBB78-1F24-4398-B3E2-4385CDC50111");
            setting.SettingName = "From Email";
            setting.SettingValue = "test@test.com";
            context.ApplicationSetting.Add(setting);

            setting = new ApplicationSetting();
            setting.Id = Guid.Parse("D33DBB78-1F24-4398-B3E2-4385CDC50112");
            setting.SettingName = "Email Server";
            setting.SettingValue = "smtp.gmail.com";
            context.ApplicationSetting.Add(setting);

            setting = new ApplicationSetting();
            setting.Id = Guid.Parse("D33DBB78-1F24-4398-B3E2-4385CDC50113");
            setting.SettingName = "Email UserID";
            setting.SettingValue = "email.testing.2017@gmail.com";
            context.ApplicationSetting.Add(setting);
            setting = new ApplicationSetting();
            setting.Id = Guid.Parse("D33DBB78-1F24-4398-B3E2-4385CDC50114");
            setting.SettingName = "Email Password";
            setting.SettingValue = "email@2017";
            context.ApplicationSetting.Add(setting);
            setting = new ApplicationSetting();

            setting.Id = Guid.Parse("D33DBB78-1F24-4398-B3E2-4385CDC50115");
            setting.SettingName = "Email port";
            setting.SettingValue = "587";
            context.ApplicationSetting.Add(setting);
            setting = new ApplicationSetting();
            setting.Id = Guid.Parse("D33DBB78-1F24-4398-B3E2-4385CDC50116");
            setting.SettingName = "VAT %";
            setting.SettingValue = ".15";
            context.ApplicationSetting.Add(setting);
            setting = new ApplicationSetting();
            setting.Id = Guid.Parse("D33DBB78-1F24-4398-B3E2-4385CDC50117");
            setting.SettingName = "Handling Fee";
            setting.SettingValue = "0";
            context.ApplicationSetting.Add(setting);
            setting = new ApplicationSetting();
            setting.Id = Guid.Parse("D33DBB78-1F24-4398-B3E2-4385CDC50118");
            setting.SettingName = "Handling Fee Min Value";
            setting.SettingValue = "0";

            context.ApplicationSetting.Add(setting);
        }

        private static void GetLookupdata(IDASDbContext context)
        {
            LookupData lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C2AAE");
            lookup.Value = "O";
            lookup.Text = "Owner";
            lookup.Type = "Occupant Type Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1234");
            lookup.Value = "T";
            lookup.Text = "Tentant";
            lookup.Type = "Occupant Type Indicator";
            context.LookupDatas.Add(lookup);

            //CONSUMER-1
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1235");
            lookup.Value = "1";
            lookup.Text = "Male";
            lookup.Type = "Gender Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1236");
            lookup.Value = "2";
            lookup.Text = "Female";
            lookup.Type = "Gender Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1237");
            lookup.Value = "3";
            lookup.Text = "Unknown";
            lookup.Type = "Gender Indicator";
            context.LookupDatas.Add(lookup);

            //CONSUMER TELEPHONE
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1238");
            lookup.Value = "1";
            lookup.Text = "Home";
            lookup.Type = "Telephone Type Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1239");
            lookup.Value = "2";
            lookup.Text = "Work";
            lookup.Type = "Telephone Type Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1240");
            lookup.Value = "3";
            lookup.Text = "Cell";
            lookup.Type = "Telephone Type Indicator";
            context.LookupDatas.Add(lookup);

            //CONSUMER EMAIL CONFIRMED
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1241");
            lookup.Value = "1";
            lookup.Text = "valid";
            lookup.Type = "Domain Valid YN";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1242");
            lookup.Value = "0";
            lookup.Text = "invalid";
            lookup.Type = "Domain Valid YN";
            context.LookupDatas.Add(lookup);

            //CONSUMER DEBT REVIEW
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1243");
            lookup.Value = "A";
            lookup.Text = "Consumer has Applied for Debt Review";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1244");
            lookup.Value = "B";
            lookup.Text = "Debt Review application has been rejected";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1245");
            lookup.Value = "T";
            lookup.Text = "Debt Review application has been Terminated";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1246");
            lookup.Value = "C";
            lookup.Text = "Consumer is under Debt Review";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1247");
            lookup.Value = "F";
            lookup.Text = "DC declares that the consumer is no longer over-indebted";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1248");
            lookup.Value = "D1";
            lookup.Text = "The consumer has reached an informal agreement with creditors, with NO court order";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1249");
            lookup.Value = "D2";
            lookup.Text = "The consumer has reached an informal agreement with creditors, WITH a court order";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1250");
            lookup.Value = "D3";
            lookup.Text = "Formal debt restructuring through the courts has commenced";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1251");
            lookup.Value = "H";
            lookup.Text = "Application terminated by Debt Counsellor";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1252");
            lookup.Value = "TRANS";
            lookup.Text = "Consumer transferred from one Debt Counsellor to another";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1253");
            lookup.Value = "D4";
            lookup.Text = "Formal debt restructuring is completed, and a court order granted";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1254");
            lookup.Value = "A1";
            lookup.Text = "Voluntary Withdrawal – Prior to “C”";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1255");
            lookup.Value = "F1";
            lookup.Text = "Consumer no longer over-indebted – Except for Mortgage";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1256");
            lookup.Value = "F2";
            lookup.Text = "Consumer no longer over-indebted – All Debts";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1257");
            lookup.Value = "G";
            lookup.Text = "Voluntary withdrawal by consumer  - Prior to Court order declaring the consumer over-indebted";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1258");
            lookup.Value = "G1";
            lookup.Text = "Voluntary withdrawal by consumer – debt review court order rescinded";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1259");
            lookup.Value = "I";
            lookup.Text = "Consumer deceased";
            lookup.Type = "Debt Review Status Code";
            context.LookupDatas.Add(lookup);

            //CONSUMER JUDGEMENT
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1260");
            lookup.Value = "0";
            lookup.Text = "Unknown Data";
            lookup.Type = "Judgement Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1261");
            lookup.Value = "ADMIN";
            lookup.Text = "Admin Order";
            lookup.Type = "Judgement Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1262");
            lookup.Value = "JUDG";
            lookup.Text = "Judgement";
            lookup.Type = "Judgement Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1263");
            lookup.Value = "REC";
            lookup.Text = "Recission";
            lookup.Type = "Judgement Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1264");
            lookup.Value = "SEQ";
            lookup.Text = "Sequestration";
            lookup.Type = "Judgement Type Code";
            context.LookupDatas.Add(lookup);

            //COMMERCIAL
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1265");
            lookup.Value = "0";
            lookup.Text = "No";
            lookup.Type = "Company Withdrawn From Public Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1266");
            lookup.Value = "1";
            lookup.Text = "Yes";
            lookup.Type = "Company Withdrawn From Public Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1267");
            lookup.Value = "0";
            lookup.Text = "No";
            lookup.Type = "Company Possible Name Conflict Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1268");
            lookup.Value = "1";
            lookup.Text = "Yes";
            lookup.Type = "Company Possible Name Conflict Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1269");
            lookup.Value = "0";
            lookup.Text = "No";
            lookup.Type = "Company Possible Duplicate Record Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1270");
            lookup.Value = "1";
            lookup.Text = "Yes";
            lookup.Type = "Company Possible Duplicate Record Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1271");
            lookup.Value = "0";
            lookup.Text = "UNKNOWN DATA";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1272");
            lookup.Value = "00";
            lookup.Text = "PRIVATE HOUSEHOLDS, EXTERRITORIAL ORGANISATIONS, REPRESENTATIVES OF FOREIGN GOVERNMENTS AND OTHER ACTIVITIES NOT ADEQUATELY DEFINED";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1273");
            lookup.Value = "01";
            lookup.Text = "AGRICULTURE HUNTING FORESTRY AND FISHING";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1274");
            lookup.Value = "02";
            lookup.Text = "MINING AND QUARRYING";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1275");
            lookup.Value = "03";
            lookup.Text = "MANUFACTURING";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1276");
            lookup.Value = "04";
            lookup.Text = "ELECTRICITY GAS AND WATER SUPPLY";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1277");
            lookup.Value = "05";
            lookup.Text = "CONSTRUCTION";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1278");
            lookup.Value = "06";
            lookup.Text = "WHOLESALE AND RETAIL TRADE; REPAIR OF MOTOR VEHICLES, MOTOR CYCLES AND PERSONAL AND HOUSEHOLD GOODS; HOTELS AND RESTAURANTS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1279");
            lookup.Value = "07";
            lookup.Text = "TRANSPORT STORAGE AND COMMUNICATION";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1280");
            lookup.Value = "08";
            lookup.Text = "FINANCIAL INTERMEDIATION, INSURANCE, REAL ESTATE AND BUSINESS SERVICES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1281");
            lookup.Value = "09";
            lookup.Text = "COMMUNITY, SOCIAL AND PERSONAL SERVICES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1282");
            lookup.Value = "11";
            lookup.Text = "AGRICULTURE AND HUNTING";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1283");
            lookup.Value = "12";
            lookup.Text = "FORESTRY AND LOGGING";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1284");
            lookup.Value = "13";
            lookup.Text = "FISHING";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1285");
            lookup.Value = "21";
            lookup.Text = "MINING OF COAL AND LIGNITE";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1286");
            lookup.Value = "22";
            lookup.Text = "EXTRACTION OF CRUDE PETROLEUM AND NATURAL GAS; SERVICES ACTIVITIES IN CIDENTAL TO OIL AND GAS EXTRACTION; EXCLUDING SURVEYING";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1287");
            lookup.Value = "23";
            lookup.Text = "MINING OF GOLD AND URANIUM ORE";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1288");
            lookup.Value = "24";
            lookup.Text = "MINING OF METAL ORES, EXCEPT GOLD AND URANIUM";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1289");
            lookup.Value = "25";
            lookup.Text = "OTHER MINING AND QUARRYING";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1290");
            lookup.Value = "29";
            lookup.Text = "SERVICES ACTIVITIES INCIDENTAL TO MINING OF MINERALS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1291");
            lookup.Value = "30";
            lookup.Text = "MANUFACTURE OF FOOD PRODUCT, BEVERAGES AND TOBACCO PRODUCTS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1292");
            lookup.Value = "31";
            lookup.Text = "MANUFACTURE OF TEXTILES, CLOTHING AND LEATHER GOODS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1293");
            lookup.Value = "32";
            lookup.Text = "MANUFACTURE OF WOOD AND OF PRODUCTS OF WOOD AND CORK, EXCEPT FURNITURE; MANUFACTURE OF ARTICLES OF STRAW AND PLATING MATERIALS; MANUFACTURE OF PAPER AND PAPER PRODUCTS; PUBLISHING, PRINTING AND REPOD";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1294");
            lookup.Value = "33";
            lookup.Text = "MANUFACTURE OF COKE, REFINED PETROLEUM PRODUCTS AND NUCLEAR FUEL; MANUFACTURE OF CHEMICALS AND CHEMICALS PRODUCTS; MANUFACTURE OF RUBBER AND PLASTIC PRODUCTS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1295");
            lookup.Value = "34";
            lookup.Text = "MANUFACTURE OF OTHER NON-METALLIC MINERAL PRODUCTS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1296");
            lookup.Value = "35";
            lookup.Text = "MANUFACTURE OF BASIC METALS, FABRICATED METAL PRODUCTS, MACHINERY AND EQUIPMENT AND OF OFFICE, ACCOUNTING AND COMPUTING MACHINERY";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1297");
            lookup.Value = "36";
            lookup.Text = "MANUFACTURE OF ELECTRICAL MACHINERY AND APPARATUS N.E.C";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1298");
            lookup.Value = "37";
            lookup.Text = "MANUFACTURE OF RADIO, TELEVISION AND COMMUNICATION EQUIPMENT AND APPARATUS AND OF MEDICAL, PRECISION";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1299");
            lookup.Value = "38";
            lookup.Text = "MANUFACTURE OF TRANSPORT EQUIPMENT";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1300");
            lookup.Value = "39";
            lookup.Text = "MANUFACTURE OF FURNITUREL MANUFACTURING N.E.C; RECYCLING";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1301");
            lookup.Value = "41";
            lookup.Text = "ELECTRICITY, GAS, STEAM AND HOT WATER SUPPLY";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1302");
            lookup.Value = "42";
            lookup.Text = "COLLECTION, PURIFICATION AND DISTRIBUTION OF WATER";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1303");
            lookup.Value = "50";
            lookup.Text = "CONSTRUCTION";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1304");
            lookup.Value = "61";
            lookup.Text = "WHOLESALE AND COMMISSION TRADE, EXCEPT OF MOTOR VEHICLES AND MOTOR CYCLES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1305");
            lookup.Value = "62";
            lookup.Text = "RETAIL TRADE, EXCEPT OF MOTOR VEHICLES AND MOTOR CYCLES; REPAIR OF PERSONAL HOUSEHOLD GOODS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1306");
            lookup.Value = "63";
            lookup.Text = "SALE, MAINTENANCE AND REPAIR OF MOTOR VEHICLES AND MOTOR CYCLES; RETAIL TRADE IN AUTOMOTIVE FUEL";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1307");
            lookup.Value = "64";
            lookup.Text = "HOTEL AND RESTAURANTS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1308");
            lookup.Value = "71";
            lookup.Text = "LAND TRANSPORT; TRANSPORT VIA PIPELINES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1309");
            lookup.Value = "72";
            lookup.Text = "WATER TRANSPORT";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1310");
            lookup.Value = "73";
            lookup.Text = "AIR TRANSPORT";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1311");
            lookup.Value = "74";
            lookup.Text = "SUPPORTING AND AUXILARY TRANSPORT ACTIVITIES; ACTIVITIES OF TRAVEL AGENCIES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1312");
            lookup.Value = "75";
            lookup.Text = "POST AND TELECOMMUNICATIONS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1313");
            lookup.Value = "81";
            lookup.Text = "FINANCIAL INTERMEDIATION, EXCEPT INSURANCE AND PENSION FUNDING";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1314");
            lookup.Value = "82";
            lookup.Text = "INSURANCE AND PENSION FUNDING, EXCEPT COMPULSORY SOCIAL SECURITY";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1315");
            lookup.Value = "83";
            lookup.Text = "ACTIVITIES AUXILIARY TO FINACIAL INTERMEDIATION";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1316");
            lookup.Value = "84";
            lookup.Text = "REAL ESTATE ACTIVITIES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1317");
            lookup.Value = "85";
            lookup.Text = "RENTING OF MACHINERY AND EQUIPMENT, WITHOUT OPERATORM AND OF PERSONAL AND HOUSEHOLD GOODS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1318");
            lookup.Value = "86";
            lookup.Text = "COMPUTER AND RELATED ACTIVITIES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1319");
            lookup.Value = "87";
            lookup.Text = "RESEARCH AND DEVELOPMENT";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1320");
            lookup.Value = "88";
            lookup.Text = "OTHER BUSINESS ACTIVITIES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1321");
            lookup.Value = "91";
            lookup.Text = "PUBLIC ADMINSTRATION AND DEFENCE ACTIVITIES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1322");
            lookup.Value = "92";
            lookup.Text = "EDUCATION";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1323");
            lookup.Value = "93";
            lookup.Text = "HEALTH AND SOCIAL WORK";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1324");
            lookup.Value = "94";
            lookup.Text = "OTHER COMMUNITY, SOCIAL AND PERSONAL SERVICE ACTIVITIES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1325");
            lookup.Value = "95";
            lookup.Text = "ACTIVITIES OF MEMBERSHIP ORGANISATIONS";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1326");
            lookup.Value = "96";
            lookup.Text = "RECREATIONAL, CULTURAL AND SPORTING ACTIVITIES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1327");
            lookup.Value = "99";
            lookup.Text = "OTHER SERVICE ACTIVITIES";
            lookup.Type = "SIC Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1328");
            lookup.Value = "0";
            lookup.Text = "No Status";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1329");
            lookup.Value = "01";
            lookup.Text = "Defensive Name";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1330");
            lookup.Value = "02";
            lookup.Text = "Suspended";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1331");
            lookup.Value = "03";
            lookup.Text = "In Business";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1332");
            lookup.Value = "04";
            lookup.Text = "Transfered to Archive";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1333");
            lookup.Value = "05";
            lookup.Text = "Provisional Liquidation";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1334");
            lookup.Value = "06";
            lookup.Text = "Voluntary Liquidation";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1335");
            lookup.Value = "07";
            lookup.Text = "Final Liquidation";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1336");
            lookup.Value = "08";
            lookup.Text = "Deregistration Process";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1337");
            lookup.Value = "09";
            lookup.Text = "Deregistration Final";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1338");
            lookup.Value = "10";
            lookup.Text = "Business Rescue";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1339");
            lookup.Value = "11";
            lookup.Text = "Transvaal Ordinance";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1340");
            lookup.Value = "12";
            lookup.Text = "Judicial Management Preliminary";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1341");
            lookup.Value = "13";
            lookup.Text = "CO-Operative";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1342");
            lookup.Value = "14";
            lookup.Text = "Dissolved";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1343");
            lookup.Value = "16";
            lookup.Text = "Name Unconditionally Reserved";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1344");
            lookup.Value = "17";
            lookup.Text = "Judicial Management Final";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1345");
            lookup.Value = "18";
            lookup.Text = "Defensive Name Extended";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1346");
            lookup.Value = "19";
            lookup.Text = "Cancelled";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1347");
            lookup.Value = "20";
            lookup.Text = "Conversion CO/CC or CC/CO";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1348");
            lookup.Value = "21";
            lookup.Text = "Duplicate Name";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1349");
            lookup.Value = "22";
            lookup.Text = "CO converted to CO-Operative";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1350");
            lookup.Value = "28";
            lookup.Text = "AR Restoration Process";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1351");
            lookup.Value = "29";
            lookup.Text = "AR Final deregistration";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1352");
            lookup.Value = "38";
            lookup.Text = "AR Deregistration Process";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1353");
            lookup.Value = "39";
            lookup.Text = "Deregistered - Transfer";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1354");
            lookup.Value = "49";
            lookup.Text = "Deregistered - Merger";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1355");
            lookup.Value = "GG";
            lookup.Text = "Send to GG A-list";
            lookup.Type = "Status Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1356");
            lookup.Value = "06";
            lookup.Text = "Public Company";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1357");
            lookup.Value = "07";
            lookup.Text = "Private Company";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1358");
            lookup.Value = "08";
            lookup.Text = "Non Profit Company";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1359");
            lookup.Value = "09";
            lookup.Text = "Limited By Guarantee";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1360");
            lookup.Value = "10";
            lookup.Text = "External Company";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1361");
            lookup.Value = "12";
            lookup.Text = "External Company under Section 21A";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1362");
            lookup.Value = "20";
            lookup.Text = "Transvaal Ordinance";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1363");
            lookup.Value = "21";
            lookup.Text = "INC";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1364");
            lookup.Value = "22";
            lookup.Text = "Unlimited";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1365");
            lookup.Value = "23";
            lookup.Text = "Close Corporation";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1366");
            lookup.Value = "24";
            lookup.Text = "Primary Co-Operative";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1367");
            lookup.Value = "25";
            lookup.Text = "Secondary Co-Operative";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1368");
            lookup.Value = "26";
            lookup.Text = "Tertiary Co-Operative";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1369");
            lookup.Value = "30";
            lookup.Text = "State owned company";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1370");
            lookup.Value = "31";
            lookup.Text = "Statutory body";
            lookup.Type = "Type Code of Company";
            context.LookupDatas.Add(lookup);

            //COMMERCIAL CAPITAL
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1371");
            lookup.Value = "0";
            lookup.Text = "No";
            lookup.Type = "Premium";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1372");
            lookup.Value = "1";
            lookup.Text = "Yes";
            lookup.Type = "Premium";
            context.LookupDatas.Add(lookup);

            //COMMERCIAL DIRECTOR
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1373");
            lookup.Value = "0";
            lookup.Text = "No";
            lookup.Type = "Is RSA Resident YN";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1374");
            lookup.Value = "1";
            lookup.Text = "Yes";
            lookup.Type = "Is RSA Resident YN";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1375");
            lookup.Value = "0";
            lookup.Text = "No";
            lookup.Type = "Is Withdrawn from Public YN";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1376");
            lookup.Value = "1";
            lookup.Text = "Yes";
            lookup.Type = "Is Withdrawn from Public YN";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1377");
            lookup.Value = "0";
            lookup.Text = "Unknown Data";
            lookup.Type = "Director Designation Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1378");
            lookup.Value = "A";
            lookup.Text = "Director";
            lookup.Type = "Director Designation Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1379");
            lookup.Value = "B";
            lookup.Text = "Officer";
            lookup.Type = "Director Designation Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1380");
            lookup.Value = "C";
            lookup.Text = "Law Representative";
            lookup.Type = "Director Designation Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1381");
            lookup.Value = "D";
            lookup.Text = "Both Director and Office";
            lookup.Type = "Director Designation Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1382");
            lookup.Value = "E";
            lookup.Text = "Secretaries";
            lookup.Type = "Director Designation Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1383");
            lookup.Value = "IN";
            lookup.Text = "Invalid Code";
            lookup.Type = "Director Designation Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1384");
            lookup.Value = "R";
            lookup.Text = "Representative";
            lookup.Type = "Director Designation Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1385");
            lookup.Value = "0";
            lookup.Text = "Unknown Data";
            lookup.Type = "Director Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1386");
            lookup.Value = "A";
            lookup.Text = "Active";
            lookup.Type = "Director Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1387");
            lookup.Value = "B";
            lookup.Text = "Deceased";
            lookup.Type = "Director Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1388");
            lookup.Value = "C";
            lookup.Text = "Resigned";
            lookup.Type = "Director Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1389");
            lookup.Value = "D";
            lookup.Text = "Disqualified";
            lookup.Type = "Director Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1390");
            lookup.Value = "E";
            lookup.Text = "Rehabilitated";
            lookup.Type = "Director Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1391");
            lookup.Value = "F";
            lookup.Text = "Remove";
            lookup.Type = "Director Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1392");
            lookup.Value = "I";
            lookup.Text = "Invalid Code";
            lookup.Type = "Director Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1393");
            lookup.Value = "N";
            lookup.Text = "Resignation notice recieved";
            lookup.Type = "Director Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1394");
            lookup.Value = "0";
            lookup.Text = "Unknown Data";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1395");
            lookup.Value = "1";
            lookup.Text = "Audit Committee Member";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1396");
            lookup.Value = "2";
            lookup.Text = "Non Executive Independent Director";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1397");
            lookup.Value = "3";
            lookup.Text = "Financial Director";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1398");
            lookup.Value = "4";
            lookup.Text = "Chief Operations Officer";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1399");
            lookup.Value = "5";
            lookup.Text = "Chief Executive Officer";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1400");
            lookup.Value = "6";
            lookup.Text = "Chairman of the Board";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1401");
            lookup.Value = "A";
            lookup.Text = "Auditor";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1402");
            lookup.Value = "B";
            lookup.Text = "Both Director and Officer";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1403");
            lookup.Value = "C";
            lookup.Text = "Secretary (Companies and CC's)";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1404");
            lookup.Value = "D";
            lookup.Text = "Director";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1405");
            lookup.Value = "E";
            lookup.Text = "External/Overseas";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1406");
            lookup.Value = "F";
            lookup.Text = "Director (Companies and CC's)";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1407");
            lookup.Value = "G";
            lookup.Text = "Local Manager";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1408");
            lookup.Value = "H";
            lookup.Text = "Testamentary Trust";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1409");
            lookup.Value = "I";
            lookup.Text = "Manager";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1410");
            lookup.Value = "J";
            lookup.Text = "Representative Trustee";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1411");
            lookup.Value = "K";
            lookup.Text = "Non Executive Director";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1412");
            lookup.Value = "L";
            lookup.Text = "Legal Representative";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1413");
            lookup.Value = "M";
            lookup.Text = "Member";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1414");
            lookup.Value = "N";
            lookup.Text = "Alternate Director";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1415");
            lookup.Value = "O";
            lookup.Text = "Officer";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1416");
            lookup.Value = "P";
            lookup.Text = "Partnership";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1417");
            lookup.Value = "Q";
            lookup.Text = "Primary/Secondary Founding Member";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1418");
            lookup.Value = "R";
            lookup.Text = "Representative";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1419");
            lookup.Value = "S";
            lookup.Text = "Company Secretary (Natural Person)";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1420");
            lookup.Value = "T";
            lookup.Text = "Trust";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1421");
            lookup.Value = "V";
            lookup.Text = "Incorporator";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1422");
            lookup.Value = "W";
            lookup.Text = "Joint Executor";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1423");
            lookup.Value = "X";
            lookup.Text = "Executor";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1424");
            lookup.Value = "Y";
            lookup.Text = "Other Trustee";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1425");
            lookup.Value = "Z";
            lookup.Text = "Founding Member";
            lookup.Type = "Director Type Code";
            context.LookupDatas.Add(lookup);

            //COMMERCIAL JUDGEMENT
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1426");
            lookup.Value = "0";
            lookup.Text = "No";
            lookup.Type = "Is Verified YN";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1427");
            lookup.Value = "1";
            lookup.Text = "Yes";
            lookup.Type = "Is Verified YN";
            context.LookupDatas.Add(lookup);

            //lookup.type = "Rescinded"

            //COMMERCIAL NAME
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1428");
            lookup.Value = "0";
            lookup.Text = "No";
            lookup.Type = "Is Withdrawn from Public YN";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1429");
            lookup.Value = "1";
            lookup.Text = "Yes";
            lookup.Type = "Is Withdrawn from Public YN";
            context.LookupDatas.Add(lookup);

            //DIRECTOR ADDRESS
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1430");
            lookup.Value = "B";
            lookup.Text = "Business";
            lookup.Type = "Address Type Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1431");
            lookup.Value = "P";
            lookup.Text = "Postal";
            lookup.Type = "Address Type Indicator";
            context.LookupDatas.Add(lookup);

            //DIRECTOR TELEPHONE
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1432");
            lookup.Value = "B";
            lookup.Text = "Business Telephone Number";
            lookup.Type = "Telephone Type Indicator";
            context.LookupDatas.Add(lookup);

            //COMMERCIAL AUDITOR
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1433");
            lookup.Value = "0";
            lookup.Text = "No";
            lookup.Type = "Fine Letter";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1434");
            lookup.Value = "1";
            lookup.Text = "Yes";
            lookup.Type = "Fine Letter";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1435");
            lookup.Value = "0";
            lookup.Text = "Unknown Data";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1436");
            lookup.Value = "ACCA";
            lookup.Text = "Chartered Association Of Certified Accountants";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1437");
            lookup.Value = "AGA";
            lookup.Text = "Associated General Accountant (SA)";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1438");
            lookup.Value = "CA";
            lookup.Text = "Chartered Accountants";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1439");
            lookup.Value = "CFA";
            lookup.Text = "Comm and Financial Accountants";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1440");
            lookup.Value = "CIMA";
            lookup.Text = "Chartered Institute of Management Accountants";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1441");
            lookup.Value = "CIS";
            lookup.Text = "Institute of Chartered Sec and Admin";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1442");
            lookup.Value = "GA";
            lookup.Text = "Chartered Accountants";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1443");
            lookup.Value = "IAC";
            lookup.Text = "Institute of Admin and Commerce";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1444");
            lookup.Value = "IAT";
            lookup.Text = "Institute of Accounting Technicians";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1445");
            lookup.Value = "SAIBA";
            lookup.Text = "The SA Institute for Business Accountants";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1446");
            lookup.Value = "SAIBR";
            lookup.Text = "The SA Institute for Business Accountants";
            lookup.Type = "Profession Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1447");
            lookup.Value = "A";
            lookup.Text = "Auditor";
            lookup.Type = "Auditor Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1448");
            lookup.Value = "B";
            lookup.Text = "ACC";
            lookup.Type = "Auditor Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1449");
            lookup.Value = "C";
            lookup.Text = "Liquidator";
            lookup.Type = "Auditor Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1450");
            lookup.Value = "D";
            lookup.Text = "Resign as per Letter";
            lookup.Type = "Auditor Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1451");
            lookup.Value = "M";
            lookup.Text = "Member of Audit Committee";
            lookup.Type = "Auditor Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1452");
            lookup.Value = "P";
            lookup.Text = "Designated Auditor (Natural Person)";
            lookup.Type = "Auditor Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1453");
            lookup.Value = "U";
            lookup.Text = "UNKNOWN DATA INTEGRITY DATA";
            lookup.Type = "Auditor Type Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1454");
            lookup.Value = "A";
            lookup.Text = "Current";
            lookup.Type = "Auditor Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1455");
            lookup.Value = "B";
            lookup.Text = "Name Change";
            lookup.Type = "Auditor Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1456");
            lookup.Value = "C";
            lookup.Text = "Resign";
            lookup.Type = "Auditor Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1457");
            lookup.Value = "D";
            lookup.Text = "Removed";
            lookup.Type = "Auditor Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1458");
            lookup.Value = "E";
            lookup.Text = "Deceased";
            lookup.Type = "Auditor Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1459");
            lookup.Value = "F";
            lookup.Text = "Previous Name";
            lookup.Type = "Auditor Status Code";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1460");
            lookup.Value = "U";
            lookup.Text = "UNKNOWN DATA INTEGRITY DATA";
            lookup.Type = "Auditor Status Code";
            context.LookupDatas.Add(lookup);

            //PROPERTY DEED
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1461");
            lookup.Value = "0";
            lookup.Text = "No";
            lookup.Type = "Is Current Owner Updated";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1462");
            lookup.Value = "1";
            lookup.Text = "Yes";
            lookup.Type = "Is Current Owner Updated";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1463");
            lookup.Value = "1";
            lookup.Text = "PRETORIA";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1464");
            lookup.Value = "2";
            lookup.Text = "JOHANNESBURG";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1465");
            lookup.Value = "3";
            lookup.Text = "BLOEMFONTEIN";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1466");
            lookup.Value = "4";
            lookup.Text = "PIETERMARITZBURG";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1467");
            lookup.Value = "5";
            lookup.Text = "KIMBERLEY";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1468");
            lookup.Value = "6";
            lookup.Text = "VRYBURG";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1469");
            lookup.Value = "7";
            lookup.Text = "KING WILLIAMS TOWN";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1470");
            lookup.Value = "8";
            lookup.Text = "CAPE TOWN";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1471");
            lookup.Value = "9";
            lookup.Text = "UMTATA";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1472");
            lookup.Value = "11";
            lookup.Text = "MPUMALANGA";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1473");
            lookup.Value = "16";
            lookup.Text = "NOT AVAILABLE";
            lookup.Type = "Deeds Office Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1474");
            lookup.Value = "1";
            lookup.Text = "EASTERN CAPE";
            lookup.Type = "Province Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1475");
            lookup.Value = "2";
            lookup.Text = "FREESTATE";
            lookup.Type = "Province Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1476");
            lookup.Value = "3";
            lookup.Text = "GAUTENG";
            lookup.Type = "Province Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1477");
            lookup.Value = "4";
            lookup.Text = "KWAZULU NATAL";
            lookup.Type = "Province Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1478");
            lookup.Value = "5";
            lookup.Text = "LIMPOPO";
            lookup.Type = "Province Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1479");
            lookup.Value = "6";
            lookup.Text = "MPUMALANGA";
            lookup.Type = "Province Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1480");
            lookup.Value = "7";
            lookup.Text = "NORTH-WEST";
            lookup.Type = "Province Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1481");
            lookup.Value = "8";
            lookup.Text = "NORTHERN CAPE";
            lookup.Type = "Province Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1482");
            lookup.Value = "9";
            lookup.Text = "WESTERN CAPE";
            lookup.Type = "Province Identifier";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1483");
            lookup.Value = "1";
            //lookup.Value (PropertyTypeId) = "A";
            lookup.Text = "Farm";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1484");
            lookup.Value = "2";
            //lookup.Value (PropertyTypeId) = "C";
            lookup.Text = "Gated Community - Complex";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1485");
            lookup.Value = "3";
            //lookup.Value (PropertyTypeId) = "F";
            lookup.Text = "Full Title - Erf - Land Parcel";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1486");
            lookup.Value = "4";
            //lookup.Value (PropertyTypeId) = "H";
            lookup.Text = "Agricultural Holding";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1487");
            lookup.Value = "5";
            //lookup.Value (PropertyTypeId) = "S";
            lookup.Text = "Sectional Title";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1488");
            lookup.Value = "6";
            //lookup.Value (PropertyTypeId) = "NULL";
            lookup.Text = "Open Space";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1489");
            lookup.Value = "7";
            //lookup.Value (PropertyTypeId) = "NULL";
            lookup.Text = "Park";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1490");
            lookup.Value = "8";
            //lookup.Value (PropertyTypeId) = "NULL";
            lookup.Text = "Street";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1491");
            lookup.Value = "9";
            //lookup.Value (PropertyTypeId) = "NULL";
            lookup.Text = "Trust";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1492");
            lookup.Value = "10";
            //lookup.Value (PropertyTypeId) = "NULL";
            lookup.Text = "Town Authority Change";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1493");
            lookup.Value = "11";
            //lookup.Value (PropertyTypeId) = "NULL";
            lookup.Text = "Authority Type";
            lookup.Type = "Property Type";
            context.LookupDatas.Add(lookup);

            //DEED BUYER
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1494");
            lookup.Value = "1";
            lookup.Text = "Private Person";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1495");
            lookup.Value = "2";
            lookup.Text = "Company";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1496");
            lookup.Value = "3";
            lookup.Text = "Government";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1497");
            lookup.Value = "4";
            lookup.Text = "Close Corporation";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1498");
            lookup.Value = "5";
            lookup.Text = "Local Authority";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1499");
            lookup.Value = "6";
            lookup.Text = "Financial Institution";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1500");
            lookup.Value = "7";
            lookup.Text = "Church";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1501");
            lookup.Value = "8";
            lookup.Text = "Trust";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1502");
            lookup.Value = "9";
            lookup.Text = "Estate";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1503");
            lookup.Value = "10";
            lookup.Text = "Partnership";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1504");
            lookup.Value = "11";
            lookup.Text = "Trustee";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1505");
            lookup.Value = "12";
            lookup.Text = "Administrator";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1506");
            lookup.Value = "13";
            lookup.Text = "Association";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1507");
            lookup.Value = "14";
            lookup.Text = "Fund";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1508");
            lookup.Value = "15";
            lookup.Text = "National Government";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1509");
            lookup.Value = "16";
            lookup.Text = "Parastatal Government";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1510");
            lookup.Value = "17";
            lookup.Text = "Third Tier Government";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1511");
            lookup.Value = "18";
            lookup.Text = "Inactive Government Property";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1512");
            lookup.Value = "19";
            lookup.Text = "Other Government Property";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1513");
            lookup.Value = "20";
            lookup.Text = "School";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1514");
            lookup.Value = "21";
            lookup.Text = "Body Corporate";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1515");
            lookup.Value = "22";
            lookup.Text = "Union";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1516");
            lookup.Value = "23";
            lookup.Text = "Foundation";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1517");
            lookup.Value = "24";
            lookup.Text = "Tribe";
            lookup.Type = "Buyer Type";
            context.LookupDatas.Add(lookup);

            //TYPE OF BUSINESS
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1518");
            lookup.Value = "1";
            lookup.Text = "Accounting and tax Consultants";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1519");
            lookup.Value = "2";
            lookup.Text = "Business Consulting Services";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1520");
            lookup.Value = "3";
            lookup.Text = "Cash Loans";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1521");
            lookup.Value = "4";
            lookup.Text = "Debt Collection & Tracing";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1522");
            lookup.Value = "5";
            lookup.Text = "Debt collector";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1523");
            lookup.Value = "6";
            lookup.Text = "Debt Counsellors";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1524");
            lookup.Value = "7";
            lookup.Text = "Debt recovery";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1525");
            lookup.Value = "8";
            lookup.Text = "Debt Review and Admin";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1526");
            lookup.Value = "9";
            lookup.Text = "Financial Sector";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1527");
            lookup.Value = "10";
            lookup.Text = "Financial Server";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1528");
            lookup.Value = "11";
            lookup.Text = "Forensic Insurance Investigations";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1529");
            lookup.Value = "12";
            lookup.Text = "Forensic Investigation";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1530");
            lookup.Value = "13";
            lookup.Text = "Housing Solutions";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1531");
            lookup.Value = "14";
            lookup.Text = "ICT / Telecommunications";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1532");
            lookup.Value = "15";
            lookup.Text = "Insurance";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1533");
            lookup.Value = "16";
            lookup.Text = "Insurance administration";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1534");
            lookup.Value = "17";
            lookup.Text = "IT Company Web Application";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1535");
            lookup.Value = "18";
            lookup.Text = "Insurance";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1536");
            lookup.Value = "19";
            lookup.Text = "Law Firm";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1537");
            lookup.Value = "20";
            lookup.Text = "Legal Practice";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1538");
            lookup.Value = "21";
            lookup.Text = "Real Estate";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1539");
            lookup.Value = "22";
            lookup.Text = "Safety, Security, Logistics, tracking";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1540");
            lookup.Value = "23";
            lookup.Text = "Security & Medical";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1541");
            lookup.Value = "24";
            lookup.Text = "Tracing Agent";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1542");
            lookup.Value = "25";
            lookup.Text = "Tracking and tracing";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1543");
            lookup.Value = "26";
            lookup.Text = "Unclaimed Funds";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1544");
            lookup.Value = "27";
            lookup.Text = "Sales";
            lookup.Type = "Type of Business";
            context.LookupDatas.Add(lookup);

            //Consumer Address
            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1545");
            lookup.Value = "1";
            lookup.Text = "Residential";
            lookup.Type = "Address Type Indicator";
            context.LookupDatas.Add(lookup);

            lookup = new LookupData();
            lookup.ID = Guid.Parse("1859B3EC-A85F-4756-B6BD-E8214A3C1546");
            lookup.Value = "2";
            lookup.Text = "Postal";
            lookup.Type = "Address Type Indicator";
            context.LookupDatas.Add(lookup);
            context.SaveChanges();
        }


        private static void GetEmailTemplates(IDASDbContext context)
        {
            EmailTemplate email = new EmailTemplate();
            email.Id = Guid.NewGuid();
            email.Type = "SignUp";
            email.Subject = "Welcome to IDAS";
            email.MailContent = "Thank you for completing the contract. Our administration team will review the application. On approval login detials will be sent to registered email.";
            context.EmailTemplates.Add(email);
            email = new EmailTemplate();
            email.Id = Guid.NewGuid();
            email.Type = "DoNotCallRegistry";
            email.Subject = "Message from IDAS - DoNotCallRegistry";
            email.MailContent = "Your details is on the Do Not Call Registry";
            context.EmailTemplates.Add(email);
            email = new EmailTemplate();
            email.Id = Guid.NewGuid();
            email.Type = "LoginDetial";
            email.Subject = "Welcome IDAS";
            email.MailContent = "Welcome IDAS. Login Name: <email>  Password: <passwordcode> ";
            context.EmailTemplates.Add(email);
            context.SaveChanges();
        }

        private static void GetDSA(IDASDbContext context)
        {
            DataServicesAgreement data = new DataServicesAgreement();
            data.Id = Guid.NewGuid();
            data.EffectiveDate = DateTime.Today.AddDays(-5);
            //data.Version = 1;
            data.IsPublished = true;
            context.DataServicesAgreements.Add(data);
            context.SaveChanges();
        }
        

    }
}
