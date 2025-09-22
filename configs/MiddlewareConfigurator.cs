using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
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

                    var problemDetails = new
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Message = "An unexpected error occurred.",
                        Detailed = exception?.Message ?? "No further details available."
                    };

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                });
            });
            app.UseHsts();
        }

        app.UseStatusCodePages(async context =>
        {
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                var problemDetails = new
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request.",
                };
                
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            }
        });
    }
}