using HMS.PatientRegistration.Api.Extensions;
using HMS.PatientRegistration.Application;
using HMS.PatientRegistration.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();
app.UseApiPipeline();

var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
var dataMode = builder.Configuration.GetSection("DataMode:Mode").Value ?? "Mock";
logger.LogInformation("HMS Patient Registration API started. DataMode: {DataMode}", dataMode);

app.Run();
