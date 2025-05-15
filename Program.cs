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
builder.Services.AddAutoMapper(typeof(PagedResponseProfile));

builder.Services.AddScoped<IYardRepository, YardRepository>();
builder.Services.AddScoped<IYardEmployeeRepository, YardEmployeeRepository>();

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

yardGroup.MapGet("/", async Task<Results<Ok<PagedResponseDTO<YardDTO>>, BadRequest<ProblemHttpResult>>> (
    IYardRepository yardRepository,
    IMapper mapper,
    int pageNumber = 1,
    int pageSize = 10
) => {
    if (pageNumber <= 0) {
        return TypedResults.BadRequest(
            TypedResults.Problem(
                title: "Bad Request",
                detail: $"{nameof(pageNumber)} must be greater than 0",
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }

    if (pageSize <= 0) {
        return TypedResults.BadRequest(
            TypedResults.Problem(
                title: "Bad Request",
                detail: $"{nameof(pageSize)} must be greater than 0",
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }

    var yards = await yardRepository.ListPagedAsync(pageNumber, pageSize);

    return TypedResults.Ok(mapper.Map<PagedResponseDTO<YardDTO>>(yards));
});

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

yardGroup.MapDelete("/{id}", async Task<Results<NoContent, NotFound>> (string id, IYardRepository yardRepository) => {
    var existingYard = await yardRepository.FindAsync(id);

    if (existingYard is null) {
        return TypedResults.NotFound();
    }

   await yardRepository.DeleteAsync(existingYard);

   return TypedResults.NoContent();
});

yardGroup.MapPatch("/{id}", async Task<Results<Ok<YardDTO>, NotFound>> (string id, YardDTO yardDTO, IYardRepository yardRepository, IMapper mapper) => {
    var existingYard = await yardRepository.FindAsync(id);

    if (existingYard is null) {
        return TypedResults.NotFound();
    }

    mapper.Map(yardDTO, existingYard);

    await yardRepository.UpdateAsync();

    var newYard = mapper.Map<YardDTO>(existingYard);
    return TypedResults.Ok(newYard);
});

yardGroup.MapGet("/{id}/employees", async Task<Results<Ok<PagedResponseDTO<YardEmployeeDTO>>, BadRequest<ProblemHttpResult>, NotFound<ProblemHttpResult>>> (
    IYardRepository yardRepository,
    IYardEmployeeRepository yardEmployeeRepository,
    IMapper mapper,
    string id,
    int pageNumber = 1,
    int pageSize = 10
) => {
    if (pageNumber <= 0) {
        return TypedResults.BadRequest(
            TypedResults.Problem(
                title: "Bad Request",
                detail: $"{nameof(pageNumber)} must be greater than 0",
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }

    if (pageSize <= 0) {
        return TypedResults.BadRequest(
            TypedResults.Problem(
                title: "Bad Request",
                detail: $"{nameof(pageSize)} must be greater than 0",
                statusCode: StatusCodes.Status400BadRequest
            )
        );
    }

    var yard = await yardRepository.FindAsync(id);

    if (yard is null) {
        return TypedResults.NotFound(
            TypedResults.Problem(
                title: "Not Found",
                detail: $"Yard with id '{id}' not found.",
                statusCode: StatusCodes.Status404NotFound
            )
        );
    }

    var employees = await yardEmployeeRepository.ListPagedAsync(pageNumber, pageSize, yard);

    return TypedResults.Ok(mapper.Map<PagedResponseDTO<YardEmployeeDTO>>(employees));
});

app.Run();
