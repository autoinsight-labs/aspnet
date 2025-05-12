using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var oracleConnectionString = Environment.GetEnvironmentVariable("ORACLE_CONNECTION_STRING");
builder.Services.AddDbContext<AutoInsightDB>(opt
    => opt.UseOracle(oracleConnectionString));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/health", () => Results.Ok())
    .WithSummary("Health Check")
    .WithDescription("Always returns 200 when the application is running.");

app.Run();
