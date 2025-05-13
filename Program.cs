using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using AutoMapper;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Dtos;
using AutoInsightAPI.Profiles;
using AutoInsightAPI.Models;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(YardProfile));
builder.Services.AddAutoMapper(typeof(YardEmployeeProfile));
builder.Services.AddAutoMapper(typeof(YardVehicleProfile));
builder.Services.AddAutoMapper(typeof(AddressProfile));

builder.Services.AddScoped<IYardRepository, YardRepository>();

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

// Yard Routes
var yardGroup = app.MapGroup("/yard").WithTags("yard");

yardGroup.MapGet("/{id}", async Task<Results<Ok<YardDTO>, NotFound>> (string id, IYardRepository yardRepository, IMapper mapper) => {
    var yard = await yardRepository.FindAsync(id);

    if (yard is null) {
        return TypedResults.NotFound();
    }

    var yardResponse = mapper.Map<YardDTO>(yard);
    return TypedResults.Ok(yardResponse);
});

yardGroup.MapPost("/", async Task<Results<Created<YardDTO>, ProblemHttpResult>> (YardDTO yardDTO, IYardRepository yardRepository, IMapper mapper) => {
    try
    {
        var createdYard = await yardRepository.CreateAsync(mapper.Map<Yard>(yardDTO));

        var yardDTOResult = mapper.Map<YardDTO>(createdYard);
        return TypedResults.Created($"/yard/{createdYard.Id}", yardDTOResult);
    }
    catch (Exception)
    {
        return TypedResults.Problem(
            title: "Internal Server Error",
            detail: "Something went wrong, please try again.",
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
});

app.Run();
