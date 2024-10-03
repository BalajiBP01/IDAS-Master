using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.Production;
using Inspirit.IDAS.ESData;
using Inspirit.IDAS.WebApplication.Model;
using Inspirit.IDAS.WebApplication.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag.AspNetCore;
using System;
using System.Reflection;

namespace Inspirit.IDAS.WebApplication
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
            services.AddDbContext<ProductionDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ProductionConnection")));

            services.AddDbContext<IDASDbContext>(options =>
           options.UseSqlServer(Configuration.GetConnectionString("IDASConnection")));
            services.AddScoped(typeof(ProductionDbContext), typeof(ProductionDbContext));
            services.AddScoped(typeof(IDASDbContext), typeof(IDASDbContext));
           
            services.AddScoped(typeof(SecurityService), typeof(SecurityService)); 
            services.AddScoped(typeof(ProfileService), typeof(ProfileService));
            services.AddScoped(typeof(ComapnyService), typeof(ComapnyService));
            services.AddScoped(typeof(TracingService), typeof(TracingService));
            services.AddScoped(typeof(ESService), typeof(ESService));
            services.AddScoped(typeof(EmailService), typeof(EmailService));
            services.AddScoped(typeof(UserService), typeof(UserService));
            services.AddScoped(typeof(CustomerLogService), typeof(CustomerLogService));
            services.AddScoped(typeof(SubscriptionService), typeof(SubscriptionService));
            services.AddScoped(typeof(BatchTraceService), typeof(BatchTraceService));
            services.AddScoped(typeof(InvoiceService), typeof(InvoiceService));
            services.AddScoped(typeof(FullAuditReportServices), typeof(FullAuditReportServices));
            services.AddScoped(typeof(SummaryFullAuditService), typeof(SummaryFullAuditService));
            //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(ExcelDataPort<>), typeof(ExcelDataPort<>));
            services.AddScoped(typeof(InvoiceService), typeof(InvoiceService));
            services.AddScoped(typeof(ContactusService), typeof(ContactusService));
            services.AddScoped(typeof(SummaryFullAuditService), typeof(SummaryFullAuditService));
            services.AddScoped(typeof(TrailUserService), typeof(TrailUserService));
            services.AddScoped(typeof(NewsService), typeof(NewsService));
            services.AddScoped(typeof(DashboardService), typeof(DashboardService));
            services.AddScoped(typeof(LeadGenerationServices), typeof(LeadGenerationServices));
            services.AddScoped(typeof(GoogleRecaptchaService), typeof(GoogleRecaptchaService));

            //services.AddOptions<ReCAPTCHASettings>().BindConfiguration("GoogleRecaptcha");
            // services.AddTransient<GoogleRecaptchaService>();
            services.AddDistributedMemoryCache();
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
            }).AddDefaultTokenProviders();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddSessionStateTempDataProvider()
              .AddRazorPagesOptions(options =>
              {
                  options.Conventions.AddPageRoute("/Home", "");
              });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });


            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
            });

           // services.AddCors();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
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

            app.UseSession();
            app.UseAuthentication();
            app.UseMvc();
            app.UseCookiePolicy();
            
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                    // krishna code spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });

            IDASDbContext.Initialize(serviceProvider);
        }
    }
}
