using System.Text.Json.Serialization;
using AutoInsightAPI.models;
using AutoInsightAPI.Profiles;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Services;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using FluentValidation;
using AutoInsightAPI.Validators;

namespace AutoInsightAPI.configs;

public static class ServicesConfigurator
{
    public static void Configure(IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<YardVehicleDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<YardDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<YardEmployeeDtoValidator>();
        
        services.AddAutoMapper(typeof(YardProfile));
        services.AddAutoMapper(typeof(YardEmployeeProfile));
        services.AddAutoMapper(typeof(YardVehicleProfile));
        services.AddAutoMapper(typeof(AddressProfile));
        services.AddAutoMapper(typeof(PagedResponseProfile));
        services.AddAutoMapper(typeof(QRCodeProfile));
        services.AddAutoMapper(typeof(VehicleProfile));

        services.AddScoped<IYardRepository, YardRepository>();
        services.AddScoped<IYardEmployeeRepository, YardEmployeeRepository>();
        services.AddScoped<IYardVehicleRepository, YardVehicleRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();

        services.AddHttpContextAccessor();
        services.AddScoped<ILinkService, LinkService>();

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        var oracleConnectionString = Environment.GetEnvironmentVariable("ORACLE_CONNECTION_STRING");
        services.AddDbContext<AutoInsightDb>(opt
            => opt.UseOracle(oracleConnectionString));

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "AutoInsight API",
                    Version = "v1",
                    Description = "API for smart yard, employee and vehicle management. Includes CRUD endpoints, pagination and QR Code lookup."
                };

                document.Tags = new List<OpenApiTag>
                {
                    new OpenApiTag { Name = "yard", Description = "Yard resources: list, get, create, update and delete yards." },
                    new OpenApiTag { Name = "employee", Description = "Employees linked to a yard: list, get, create, update and delete." },
                    new OpenApiTag { Name = "vehicle", Description = "Vehicles and yard vehicles: query vehicles and manage yard associations." }
                };

                if (document.Components?.Schemas is { } schemas)
                {
                    if (schemas.TryGetValue("AddressDto", out var addressSchema))
                    {
                        addressSchema.Description = "Full address associated with a yard.";
                        addressSchema.Example = new OpenApiObject
                        {
                            ["id"] = new OpenApiString("addr_123"),
                            ["country"] = new OpenApiString("BR"),
                            ["state"] = new OpenApiString("SP"),
                            ["city"] = new OpenApiString("São Paulo"),
                            ["zipCode"] = new OpenApiString("01311-000"),
                            ["neighborhood"] = new OpenApiString("Bela Vista"),
                            ["complement"] = new OpenApiString("Av. Paulista, 1106")
                        };
                    }

                    if (schemas.TryGetValue("ModelDto", out var modelSchema))
                    {
                        modelSchema.Description = "Vehicle model description.";
                        modelSchema.Example = new OpenApiObject
                        {
                            ["id"] = new OpenApiString("mdl_001"),
                            ["name"] = new OpenApiString("Honda CG 160"),
                            ["year"] = new OpenApiInteger(2023)
                        };
                    }

                    if (schemas.TryGetValue("VehicleDto", out var vehicleSchema))
                    {
                        vehicleSchema.Description = "Registered vehicle.";
                        vehicleSchema.Example = new OpenApiObject
                        {
                            ["id"] = new OpenApiString("veh_abc123"),
                            ["plate"] = new OpenApiString("ABC1D23"),
                            ["model"] = new OpenApiObject
                            {
                                ["id"] = new OpenApiString("mdl_001"),
                                ["name"] = new OpenApiString("Honda CG 160"),
                                ["year"] = new OpenApiInteger(2023)
                            },
                            ["userId"] = new OpenApiString("usr_001")
                        };
                    }

                    if (schemas.TryGetValue("YardDto", out var yardSchema))
                    {
                        yardSchema.Description = "Yard for storing and managing vehicles.";
                        yardSchema.Example = new OpenApiObject
                        {
                            ["id"] = new OpenApiString("yrd_123"),
                            ["ownerId"] = new OpenApiString("usr_owner_001"),
                            ["address"] = new OpenApiObject
                            {
                                ["id"] = new OpenApiString("addr_123"),
                                ["country"] = new OpenApiString("BR"),
                                ["state"] = new OpenApiString("SP"),
                                ["city"] = new OpenApiString("São Paulo"),
                                ["zipCode"] = new OpenApiString("01311-000"),
                                ["neighborhood"] = new OpenApiString("Bela Vista"),
                                ["complement"] = new OpenApiString("Av. Paulista, 1106")
                            }
                        };
                    }

                    if (schemas.TryGetValue("YardEmployeeDto", out var employeeSchema))
                    {
                        employeeSchema.Description = "Employee linked to a yard.";
                        employeeSchema.Example = new OpenApiObject
                        {
                            ["id"] = new OpenApiString("emp_001"),
                            ["name"] = new OpenApiString("Jane Doe"),
                            ["imageUrl"] = new OpenApiString("https://cdn.example.com/jane.png"),
                            ["role"] = new OpenApiString("ADMIN"),
                            ["userId"] = new OpenApiString("usr_002")
                        };
                    }

                    if (schemas.TryGetValue("YardVehicleDto", out var yardVehicleSchema))
                    {
                        yardVehicleSchema.Description = "Association between a yard and a vehicle, with status and timestamps.";
                        yardVehicleSchema.Example = new OpenApiObject
                        {
                            ["id"] = new OpenApiString("yv_001"),
                            ["status"] = new OpenApiString("ON_SERVICE"),
                            ["enteredAt"] = new OpenApiString("2025-05-20T10:00:00Z"),
                            ["leftAt"] = new OpenApiNull(),
                            ["vehicle"] = new OpenApiObject
                            {
                                ["id"] = new OpenApiString("veh_abc123"),
                                ["plate"] = new OpenApiString("ABC1D23"),
                                ["model"] = new OpenApiObject
                                {
                                    ["id"] = new OpenApiString("mdl_001"),
                                    ["name"] = new OpenApiString("Honda CG 160"),
                                    ["year"] = new OpenApiInteger(2023)
                                },
                                ["userId"] = new OpenApiString("usr_001")
                            }
                        };
                    }
                }
                return Task.CompletedTask;
            });
        });
    }
}
