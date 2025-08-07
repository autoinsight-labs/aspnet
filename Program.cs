using Microsoft.AspNetCore.Http.HttpResults;
using AutoMapper;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.configs;
using AutoInsightAPI.handlers;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

ServicesConfigurator.Configure(builder.Services);

var app = builder.Build();

MiddlewareConfigurator.Configure(app);

app.MapGet("/health", () => Results.Ok())
    .WithSummary("Health Check")
    .WithDescription("Always returns 200 when the application is running.");

// Yard Routes
YardHandler.Map(app);
// Employee routes
EmployeeHandler.Map(app);
// Vehicle Routes
VehicleHandler.Map(app);
app.Run();
