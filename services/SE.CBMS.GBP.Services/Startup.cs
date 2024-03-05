//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.HttpsPolicy;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Microsoft.OpenApi.Models;
//using SchneiderElectric.CBMS.GBP.DAO.Interfaces.GlobalBillPay;
//using SchneiderElectric.CBMS.GBP.DAO.Sql.GlobalBillPay;
//using SchneiderElectric.CBMS.GBP.Orchestration.GlobalBillPay;
//using SchneiderElectric.CBMS.GBP.Orchestration.Interfaces;
//using SE.CBMS.GBP.DAO.Interfaces.GlobalBillPay;
//using SE.CBMS.InvoiceReg.Business;
//using Swashbuckle.AspNetCore.Swagger;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SE.CBMS.GBP.Services
//{
//    public class Startup
//    {
//        public IConfiguration Configuration { get; }
//        public Startup(IConfiguration configuration, IHostingEnvironment env)
//        {
//            Configuration = configuration;
//            var builder = new ConfigurationBuilder()
//            .SetBasePath(env.ContentRootPath)
//            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//            .AddEnvironmentVariables();

//            Configuration = builder.Build();
//        }
//        // This method gets called by the runtime. Use this method to add services to the container.
//        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
//        public void ConfigureServices(IServiceCollection services)
//        {

//            services.AddCors(o => o.AddPolicy("corPolicy", builder =>
//            {
//                builder.AllowAnyOrigin()
//                       .AllowAnyMethod()
//                       .AllowAnyHeader();
//            }));
//            services.AddSingleton(_ => Configuration);
//            services.AddMvc().AddJsonOptions(options =>
//            {
//                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
//            });
//            AddDataLayers(services);
//            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
//            services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Global Bill Pay", Version = "v1" });
//            });
//        }
//        private void AddDataLayers(IServiceCollection services)
//        {
//            services.AddTransient<IGBPDataLoadDAO, GBPDataLoadDAO>();
//            services.AddTransient<IGBPDataLoadOrchestration, GBPDataLoadOrchestration>();


//            services.AddTransient<IGBPActionDAO , GBPActionDAO>();
//            services.AddTransient<IGBPActionOrchestration, GBPActionOrchestration>();



//            services.AddTransient<IRecalServiceCallsPost>(io =>
//            {
//                return ActivatorUtilities.CreateInstance<RecalcServicePost>(io,
//                                    Configuration.GetSection("AppSettings").GetSection("RecalcService").Value);
//            });
//        }

//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
//        {
//            loggerFactory.AddLog4Net("log4net.config");
//            //if (!string.IsNullOrEmpty(Configuration.GetValue<string>("Log4NetCore:Name")))
//            //{
//            //    loggerFactory.AddLog4Net(Configuration.GetValue<string>("Log4NetCore:Name"));
//            //}
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }
//            else
//            {
//                app.UseHsts();
//            }
//            app.UseCors(c => c
//                 .AllowAnyOrigin()
//                 .AllowAnyMethod()
//                 .AllowAnyHeader()
//                 .AllowCredentials());
//            app.UseStaticFiles();
//            app.UseSwagger();
//            app.UseAuthentication();
//            app.UseHttpsRedirection();
//            app.UseSwaggerUI(c =>
//            {
//                c.SwaggerEndpoint("../swagger/v1/swagger.json", "Global Bill Pay");
//            });
//            app.UseMvc();
//        }
//    }
//}
