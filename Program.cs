using AutoInsightAPI.configs;
using AutoInsightAPI.handlers;
using AutoInsightAPI.Models;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

ServicesConfigurator.Configure(builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AutoInsightDb>();
        context.Database.Migrate();
        app.Logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating the database");
        throw;
    }
}

MiddlewareConfigurator.Configure(app);

app.MapGet("/health", () => Results.Ok())
    .WithSummary("Health Check")
    .WithDescription("Returns 200 OK when the application is running.")
    .Produces(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status500InternalServerError)
    .WithOpenApi(op =>
    {
        op.OperationId = "HealthCheck";
        op.Description = @"Application health check endpoint

Verifies the application's health status and returns basic operational information.

Response Fields:
- status (string): Current application status (""Healthy"")
- timestamp (DateTime): Exact UTC date/time when the check was performed

Example Response:
```json
{
  ""status"": ""Healthy"",
  ""timestamp"": ""2025-01-15T10:30:00.000Z""
}
```

Possible Failures:
- 500 Internal Server Error: Critical application failure (Unreachable)

Example Error Response:
```json
{
  ""statusCode"": 500,
  ""message"": ""Internal server error occurred""
}
```

Recommended Usage:
This endpoint should be called by load balancers, monitoring tools (Kubernetes liveness/readiness probes),
and alerting systems to verify that the application is functioning properly.";
        if (!op.Responses.TryGetValue("200", out var ok))
        {
            ok = new Microsoft.OpenApi.Models.OpenApiResponse { Description = "OK" };
            op.Responses["200"] = ok;
        }
        if (!ok.Content.TryGetValue("application/json", out var okJson))
        {
            okJson = new Microsoft.OpenApi.Models.OpenApiMediaType();
            ok.Content["application/json"] = okJson;
        }
        okJson.Example = new Microsoft.OpenApi.Any.OpenApiObject
        {
            ["status"] = new Microsoft.OpenApi.Any.OpenApiString("Healthy"),
            ["timestamp"] = new Microsoft.OpenApi.Any.OpenApiString("2025-01-15T10:30:00.000Z")
        };
        if (!op.Responses.TryGetValue("500", out var err))
        {
            err = new Microsoft.OpenApi.Models.OpenApiResponse { Description = "Internal Server Error" };
            op.Responses["500"] = err;
        }
        if (!err.Content.TryGetValue("application/problem+json", out var errJson))
        {
            errJson = new Microsoft.OpenApi.Models.OpenApiMediaType();
            err.Content["application/problem+json"] = errJson;
        }
        errJson.Example = new Microsoft.OpenApi.Any.OpenApiObject
        {
            ["type"] = new Microsoft.OpenApi.Any.OpenApiString("https://httpstatuses.io/500"),
            ["title"] = new Microsoft.OpenApi.Any.OpenApiString("An unexpected error occurred."),
            ["status"] = new Microsoft.OpenApi.Any.OpenApiInteger(500),
            ["detail"] = new Microsoft.OpenApi.Any.OpenApiString("Internal server error occurred"),
            ["instance"] = new Microsoft.OpenApi.Any.OpenApiString("/health")
        };
        return op;
    });

// Yard Routes
YardHandler.Map(app);
// Employee routes
EmployeeHandler.Map(app);
// Invite routes
InviteHandler.Map(app);
// Vehicle Routes
VehicleHandler.Map(app);

await app.RunAsync();
