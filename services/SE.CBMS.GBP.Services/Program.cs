using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SchneiderElectric.CBMS.GBP.DAO.Interfaces.GlobalBillPay;
using SchneiderElectric.CBMS.GBP.DAO.Sql.GlobalBillPay;
using SchneiderElectric.CBMS.GBP.Orchestration.GlobalBillPay;
using SchneiderElectric.CBMS.GBP.Orchestration.Interfaces;
using SE.CBMS.GBP.DAO.Interfaces.GlobalBillPay;
using SE.CBMS.InvoiceReg.Business;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddCors(o => o.AddPolicy("corPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
services.AddSingleton(_ => configuration);
//services.AddMvc().AddJsonOptions(options =>
//{
//    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
//});
services.AddControllers(options => options.EnableEndpointRouting = false)
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.UseCamelCasing(true);
    });

 
 services.AddTransient<IGBPDataLoadDAO, GBPDataLoadDAO>();
services.AddTransient<IGBPDataLoadOrchestration, GBPDataLoadOrchestration>();
services.AddTransient<IGBPActionDAO, GBPActionDAO>();
services.AddTransient<IGBPActionOrchestration, GBPActionOrchestration>();

services.AddTransient<IRecalServiceCallsPost>(io =>
{
    return ActivatorUtilities.CreateInstance<RecalcServicePost>(io,
                        configuration.GetSection("AppSettings").GetSection("RecalcService").Value);
}); 
services.AddMvc();//.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Global Bill Pay", Version = "v1" });
});

var app = builder.Build();
var loggerFactory = app.Services.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
loggerFactory.AddLog4Net("log4net.config");
//if (!string.IsNullOrEmpty(Configuration.GetValue<string>("Log4NetCore:Name")))
//{
//    loggerFactory.AddLog4Net(Configuration.GetValue<string>("Log4NetCore:Name"));
//}
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}
app.UseCors("corPolicy");
app.UseStaticFiles();
app.UseSwagger();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("../swagger/v1/swagger.json", "Global Bill Pay");
});
app.UseMvc();
app.Run();


namespace SE.CBMS.GBP.Services
{
    public class Program
    {
    }
}
