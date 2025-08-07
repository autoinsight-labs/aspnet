using Scalar.AspNetCore;

namespace AutoInsightAPI.configs;

public static class MiddlewareConfigurator
{
    public static void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    }
}