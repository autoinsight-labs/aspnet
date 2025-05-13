using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using AutoMapper;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

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

yardGroup.MapGet("/{id}", async Task<Results<Ok<GetYardDTO>, NotFound>> (string id, AutoInsightDB db, IMapper mapper) => {
    var yard = await db.Yards.Include(y => y.Address).FirstOrDefaultAsync(y => y.Id == id);

    if (yard is null) {
        return TypedResults.NotFound();
    }

    var yardResponse = mapper.Map<GetYardDTO>(yard);
    return TypedResults.Ok(yardResponse);
});

yardGroup.MapPost("/", async Task<Results<Created<GetYardDTO>, ProblemHttpResult>> (CreateYardDTO newYard, AutoInsightDB db, IMapper mapper) => {
    using var transaction = await db.Database.BeginTransactionAsync();

    try
    {
        Address address = newYard.Address;
        db.Addresses.Add(address);

        await db.SaveChangesAsync();

        Yard yard = new Yard
        {
            OwnerId = newYard.OwnerId,
            AddressId = address.Id,
            Address = address
        };

        db.Yards.Add(yard);
        await db.SaveChangesAsync();

        await transaction.CommitAsync();

        var yardResponse = mapper.Map<GetYardDTO>(yard);
        return TypedResults.Created($"/yard/{yard.Id}", yardResponse);
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();

        return TypedResults.Problem(
            title: "Internal Server Error",
            detail: "Something went wrong, please try again.",
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
});

app.Run();
