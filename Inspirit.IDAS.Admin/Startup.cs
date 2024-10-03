using Inspirit.IDAS.Admin.Services;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.Production;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag.AspNetCore;
using System.Reflection;

namespace Inspirit.IDAS.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IDASDbContext>(options =>
           options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IDASDbContext), typeof(IDASDbContext));
            services.AddScoped(typeof(SecurityService), typeof(SecurityService));
            services.AddScoped(typeof(CustomerService), typeof(CustomerService));
            services.AddScoped(typeof(CustomerUserService), typeof(CustomerUserService));
            services.AddScoped(typeof(DsaService), typeof(DsaService));
            services.AddScoped(typeof(LookupDataService), typeof(LookupDataService));
            services.AddScoped(typeof(InvoiceService), typeof(InvoiceService));
            services.AddScoped(typeof(EmailTemplateService), typeof(EmailTemplateService));
            services.AddScoped(typeof(PaymentService), typeof(PaymentService));
            services.AddScoped(typeof(UserService), typeof(UserService));
            services.AddScoped(typeof(ProdservService), typeof(ProdservService));
            services.AddScoped(typeof(DoNotCallRegistryService), typeof(DoNotCallRegistryService));
            services.AddScoped(typeof(ContactusService), typeof(ContactusService));
            services.AddScoped(typeof(SubscriptionService), typeof(SubscriptionService));
            services.AddScoped(typeof(ApplicationMessageService), typeof(ApplicationMessageService));
            services.AddScoped(typeof(EmailService), typeof(EmailService));
            services.AddScoped(typeof(AppSettingService), typeof(AppSettingService));
            services.AddScoped(typeof(ProductServices), typeof(ProductServices));
            services.AddScoped(typeof(ProFormaInvoiceService), typeof(ProFormaInvoiceService));
            services.AddScoped(typeof(BatchTracingService), typeof(BatchTracingService));
            services.AddScoped(typeof(DashboardService), typeof(DashboardService));
            services.AddScoped(typeof(NewsService), typeof(NewsService));
            services.AddScoped(typeof(LeadGenerationService), typeof(LeadGenerationService));
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            }); 

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder => builder
              .WithOrigins("*")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("content-disposition", "content-type"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();


            //app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, settings =>
            //{
            //    settings.GeneratorSettings.DefaultPropertyNameHandling = PropertyNameHandling.CamelCase;

            //});



            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });


            

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                    //spa.UseAngularCliServer(npmScript: "start:asp");
                }
            });
        }
    }
}
