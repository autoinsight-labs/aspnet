using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    var problemDetails = new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Title = "An unexpected error occurred.",
                        Detail = exception?.Message ?? "No further details available.",
                        Instance = context.Request?.Path.Value,
                        Type = "https://httpstatuses.io/500"
                    };

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/problem+json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                });
            });
            app.UseHsts();
        }

        app.UseStatusCodePages(async context =>
        {
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Detail = "Invalid request.",
                    Instance = context.HttpContext.Request?.Path.Value,
                    Type = "https://httpstatuses.io/400"
                };

                context.HttpContext.Response.ContentType = "application/problem+json";
                await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            }
        });
    }
}