using CleanArchTemplate.Application;
using CleanArchTemplate.Application.Middleware;
using CleanArchTemplate.Application.Extensions;
using CleanArchTemplate.Infrastructure.Extensions;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;


builder.Services.AddApi(config);

builder.Services.AddInfrastructure(config);

builder.Services.AddApplication(config);

builder.Services.AddDatabase(config);

var app = builder.Build();

app.UseSharedMiddlewares();

app.Run();


