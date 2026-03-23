using CleanArchTemplate.Api.Extensions;
using CleanArchTemplate.Application.Extensions;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Services.AddApi(config);

builder.Services.AddEmailService(config);

builder.Services.AddApplication(config);

builder.Services.AddDatabase(config);

var app = builder.Build();

app.UseSharedMiddlewares();

app.Run();


