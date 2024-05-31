using System.Reflection;
using System.Text;
using CI.Api.Infrastructure;
using CI.Api.Jobs;
using CI.Api.Options;
using CI.Api.Services;
using FastEndpoints;
using FastEndpoints.Swagger;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

// AddInfrastructure()
// AddApplication()
// instead of...
builder.Services.AddMemoryCache();
builder.Services.AddOptions<WorkflowOptions>().Bind(configuration.GetSection(nameof(WorkflowOptions)));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddDbContext<ProductsDbContext>(cfg =>
{
    cfg.UseSqlite(builder.Configuration.GetConnectionString(nameof(ProductsDbContext)));
});
builder.Services.AddHangfire(cfg =>
{
    cfg.UseMemoryStorage();
}).AddHangfireServer();

builder.Services.AddSingleton<ProductParserFactory>();
builder.Services.AddSingleton<IProductParser, ExcelProductParser>();
    
var app = builder.Build();

app.UseHangfireDashboard();
app.UseFastEndpoints()
    .UseSwaggerGen();

// var time = CronLibrary.Parse(...);
var everyFiveMinutes = "*/5 * * * *";
RecurringJob.AddOrUpdate<SplitProductsIntoGroups>("productsprocess",job => job.Execute(), everyFiveMinutes);
app.Run();