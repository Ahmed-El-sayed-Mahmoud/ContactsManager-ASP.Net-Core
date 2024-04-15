using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
//builder.Host.ConfigureLogging(logging =>
//{
//    logging.ClearProviders();
//    logging.AddConsole();
//    logging.AddDebug();
//});
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) 
    .ReadFrom.Services(services); 
});
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
});
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryServices, CountryServices>();
builder.Services.AddScoped<IPersonServices, PersonServices>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();
app.UseSerilogRequestLogging(); // for IDiagnosticContext to add log at the last log of a request
app.UseHttpLogging();
if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
if(!builder.Environment.IsEnvironment("Test"))
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", "Rotativa");
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.Run();
public partial class Program { }
