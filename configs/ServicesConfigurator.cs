using System.Text.Json.Serialization;
using AutoInsightAPI.Models;
using AutoInsightAPI.Profiles;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Services;
using AutoInsightAPI.handlers;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using FluentValidation;
using AutoInsightAPI.Validators;
using System.Collections.Generic;

namespace AutoInsightAPI.configs;

public static class ServicesConfigurator
{
    private const string UserIdFieldName = "userId";
    public static void Configure(IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<YardVehicleDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<YardDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<YardEmployeeDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateYardDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateYardEmployeeDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateYardVehicleDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateVehicleDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateModelDtoValidator>();
        
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
        services.AddScoped<IModelRepository, ModelRepository>();
        
        services.AddScoped<CreateYardVehicleRepositories>(provider => 
            new CreateYardVehicleRepositories(
                provider.GetRequiredService<IYardRepository>(),
                provider.GetRequiredService<IYardVehicleRepository>(),
                provider.GetRequiredService<IVehicleRepository>(),
                provider.GetRequiredService<IModelRepository>()
            ));

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
                    Description = @"🚗 **AutoInsight API** - Smart Yard & Vehicle Management System

**Key Features:**
- ✅ **Auto-generated IDs** - Never send IDs in POST requests
- ✅ **Flexible Creation** - Create or link existing resources on-the-fly  
- ✅ **HATEOAS Support** - Full hypermedia links in responses
- ✅ **Fluent Validation** - Comprehensive request validation
- ✅ **Pagination** - Efficient data retrieval with configurable page sizes
- ✅ **QR Code Integration** - Quick vehicle lookup by QR codes

**API Design:**
- **Request schemas** (CreateXxxDto): No IDs, optimized for creation
- **Response schemas** (XxxDto): Include IDs, links, and full object graphs
- **Flexible associations**: Link existing resources OR create new ones seamlessly"
                };

                document.Tags = new List<OpenApiTag>
                {
                    new OpenApiTag { Name = "yard", Description = "Yard management: CRUD operations for yards with auto-generated IDs and flexible address creation." },
                    new OpenApiTag { Name = "employee", Description = "Yard employee management: CRUD operations with role-based access and auto-generated IDs." },
                    new OpenApiTag { Name = "vehicle", Description = "Vehicle and yard-vehicle associations: supports flexible creation with existing or new vehicles/models." }
                };

                if (document.Components?.Schemas is { } schemas)
                {
                    ConfigureAddressDtoSchema(schemas);
                    ConfigureModelDtoSchema(schemas);
                    ConfigureVehicleDtoSchema(schemas);
                    ConfigureYardDtoSchema(schemas);
                    ConfigureYardEmployeeDtoSchema(schemas);
                    ConfigureYardVehicleDtoSchema(schemas);
                    ConfigureCreateYardDtoSchema(schemas);
                    ConfigureCreateYardEmployeeDtoSchema(schemas);
                    ConfigureCreateYardVehicleDtoSchema(schemas);
                    ConfigureCreateVehicleDtoSchema(schemas);
                    ConfigureCreateModelDtoSchema(schemas);
                }
                return Task.CompletedTask;
            });
        });
    }

    private static void ConfigureAddressDtoSchema(IDictionary<string, OpenApiSchema> schemas)
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
    }

    private static void ConfigureModelDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
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
    }

    private static void ConfigureVehicleDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
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
[UserIdFieldName] = new OpenApiString("usr_001")
            };
        }
    }

    private static void ConfigureYardDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
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
    }

    private static void ConfigureYardEmployeeDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
        if (schemas.TryGetValue("YardEmployeeDto", out var employeeSchema))
        {
            employeeSchema.Description = "Employee linked to a yard.";
            employeeSchema.Example = new OpenApiObject
            {
                ["id"] = new OpenApiString("emp_001"),
                ["name"] = new OpenApiString("Jane Doe"),
                ["imageUrl"] = new OpenApiString("https://cdn.example.com/jane.png"),
                ["role"] = new OpenApiString("ADMIN"),
[UserIdFieldName] = new OpenApiString("usr_002")
            };
        }
    }

    private static void ConfigureYardVehicleDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
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
    [UserIdFieldName] = new OpenApiString("usr_001")
                }
            };
        }
    }

    private static void ConfigureCreateYardDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
        if (schemas.TryGetValue("CreateYardDto", out var createYardSchema))
        {
            createYardSchema.Description = @"Payload for creating a new yard (ID will be auto-generated).

**Note:** Do not include an `id` field - it will be automatically generated by the system.

**Required fields:**
- `ownerId`: Must reference an existing user
- `address`: Complete address object with all required fields";
            createYardSchema.Example = new OpenApiObject
            {
                ["ownerId"] = new OpenApiString("usr_owner_001"),
                ["address"] = new OpenApiObject
                {
                    ["country"] = new OpenApiString("BR"),
                    ["state"] = new OpenApiString("SP"),
                    ["city"] = new OpenApiString("São Paulo"),
                    ["zipCode"] = new OpenApiString("01311-000"),
                    ["neighborhood"] = new OpenApiString("Bela Vista"),
                    ["complement"] = new OpenApiString("Av. Paulista, 1106")
                }
            };
        }
    }

    private static void ConfigureCreateYardEmployeeDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
        if (schemas.TryGetValue("CreateYardEmployeeDto", out var createEmployeeSchema))
        {
            createEmployeeSchema.Description = @"Payload for creating a new yard employee (ID will be auto-generated).

**Note:** Do not include an `id` field - it will be automatically generated by the system.

**Required fields:**
- `name`: Employee full name
- `imageUrl`: URL to employee photo
- `role`: Must be either `ADMIN` or `MEMBER`
- `userId`: Must reference an existing user";
            createEmployeeSchema.Example = new OpenApiObject
            {
                ["name"] = new OpenApiString("Jane Doe"),
                ["imageUrl"] = new OpenApiString("https://cdn.example.com/jane.png"),
                ["role"] = new OpenApiString("ADMIN"),
[UserIdFieldName] = new OpenApiString("usr_002")
            };
        }
    }

    private static void ConfigureCreateYardVehicleDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
        if (schemas.TryGetValue("CreateYardVehicleDto", out var createYardVehicleSchema))
        {
            createYardVehicleSchema.Description = @"Payload for creating a new yard vehicle association (ID will be auto-generated). 

**Note:** This schema is for REQUEST bodies only. Response schemas include additional fields like `id` and HATEOAS `links`.

Supports three flexible options:

**Option 1 - Link existing vehicle:**
```json
{
  ""status"": ""WAITING"",
  ""enteredAt"": ""2025-05-20T09:30:00Z"",
  ""vehicleId"": ""veh_abc123""
}
```

**Option 2 - Create vehicle with existing model:**
```json
{
  ""status"": ""WAITING"",
  ""enteredAt"": ""2025-05-20T09:30:00Z"",
  ""vehicle"": {
    ""plate"": ""XYZ5E67"",
    ""modelId"": ""mdl_002"",
    ""userId"": ""usr_003""
  }
}
```

**Option 3 - Create vehicle and model:**
```json
{
  ""status"": ""WAITING"",
  ""enteredAt"": ""2025-05-20T09:30:00Z"",
  ""vehicle"": {
    ""plate"": ""XYZ5E67"",
    ""model"": {
      ""name"": ""Toyota Corolla"",
      ""year"": 2024
    },
    ""userId"": ""usr_003""
  }
}
```";
            createYardVehicleSchema.Example = new OpenApiObject
            {
                ["status"] = new OpenApiString("WAITING"),
                ["enteredAt"] = new OpenApiString("2025-05-20T09:30:00Z"),
                ["leftAt"] = new OpenApiNull(),
                ["vehicleId"] = new OpenApiString("veh_abc123")
            };
        }
    }

    private static void ConfigureCreateVehicleDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
        if (schemas.TryGetValue("CreateVehicleDto", out var createVehicleSchema))
        {
            createVehicleSchema.Description = @"Payload for creating a new vehicle (ID will be auto-generated).

**Note:** This schema is for REQUEST bodies only. Response schemas include additional fields like `id` and HATEOAS `links`.

Supports two options:

**Option 1 - Use existing model:**
```json
{
  ""plate"": ""XYZ5E67"",
  ""modelId"": ""mdl_002"",
  ""userId"": ""usr_003""
}
```

**Option 2 - Create new model:**
```json
{
  ""plate"": ""XYZ5E67"",
  ""model"": {
    ""name"": ""Toyota Corolla"",
    ""year"": 2024
  },
  ""userId"": ""usr_003""
}
```

**Note:** You must provide either `modelId` OR `model`, but not both.";
            createVehicleSchema.Example = new OpenApiObject
            {
                ["plate"] = new OpenApiString("XYZ5E67"),
                ["modelId"] = new OpenApiString("mdl_002"),
[UserIdFieldName] = new OpenApiString("usr_003")
            };
        }
    }

    private static void ConfigureCreateModelDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
        if (schemas.TryGetValue("CreateModelDto", out var createModelSchema))
        {
            createModelSchema.Description = @"Payload for creating a new vehicle model (ID will be auto-generated).

**Note:** This schema is for REQUEST bodies only. Response schemas include additional fields like `id` and HATEOAS `links`.

**Example:**
```json
{
  ""name"": ""Toyota Corolla"",
  ""year"": 2024
}
```

**Validations:**
- `name`: Required, cannot be empty
- `year`: Required, must be greater than 1900";
            createModelSchema.Example = new OpenApiObject
            {
                ["name"] = new OpenApiString("Toyota Corolla"),
                ["year"] = new OpenApiInteger(2024)
            };
        }
    }
}
