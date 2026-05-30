using CleanArchTemplate.Api.Extensions;
using CleanArchTemplate.Application.Extensions;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var loggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
var logger = loggerFactory.CreateLogger("OAuth");

builder.Services.AddApi(config, logger);

builder.Services.AddEmailService(config);

builder.Services.AddApplication(config);

builder.Services.AddDatabase(config);

var app = builder.Build();

app.UseSharedMiddlewares();

app.Run();


