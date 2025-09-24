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
        services.AddValidatorsFromAssemblyContaining<AcceptInviteDtoValidator>();
        
        services.AddAutoMapper(typeof(YardProfile));
        services.AddAutoMapper(typeof(YardEmployeeProfile));
        services.AddAutoMapper(typeof(YardVehicleProfile));
        services.AddAutoMapper(typeof(AddressProfile));
        services.AddAutoMapper(typeof(PagedResponseProfile));
        services.AddAutoMapper(typeof(QRCodeProfile));
        services.AddAutoMapper(typeof(VehicleProfile));
        services.AddAutoMapper(typeof(EmployeeInviteProfile));

        services.AddScoped<IYardRepository, YardRepository>();
        services.AddScoped<IYardEmployeeRepository, YardEmployeeRepository>();
        services.AddScoped<IYardVehicleRepository, YardVehicleRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IModelRepository, ModelRepository>();
        services.AddScoped<IEmployeeInviteRepository, EmployeeInviteRepository>();
        
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
- ✅ **Employee Invitation System** - Secure invite-based onboarding with status tracking
- ✅ **Flexible Creation** - Create or link existing resources on-the-fly  
- ✅ **HATEOAS Support** - Full hypermedia links in responses
- ✅ **Fluent Validation** - Comprehensive request validation
- ✅ **Pagination** - Efficient data retrieval with configurable page sizes
- ✅ **QR Code Integration** - Quick vehicle lookup by QR codes

**Employee Invitation Workflow:**
1. **Create Invite** → POST `/yards/{id}/employees` creates PENDING invitation
2. **Send Token** → Use generated token to invite users (email integration separate)
3. **Accept/Reject** → Users respond via `/invites/{token}/accept` or `/invites/{token}/reject`
4. **Employee Creation** → Accepting invite automatically creates YardEmployee

**API Design:**
- **Request schemas** (CreateXxxDto): No IDs, optimized for creation
- **Response schemas** (XxxDto): Include IDs, links, and full object graphs
- **Invite schemas**: Full invitation lifecycle with status tracking (PENDING/ACCEPTED/REJECTED)
- **Flexible associations**: Link existing resources OR create new ones seamlessly"
                };

                document.Tags = new List<OpenApiTag>
                {
                    new OpenApiTag { Name = "yard", Description = "Yard management: CRUD operations for yards with auto-generated IDs and flexible address creation." },
                    new OpenApiTag { Name = "employee", Description = "Yard employee management: Read and Delete operations for existing employees (creation via invite system)." },
                    new OpenApiTag { Name = "invite", Description = "Employee invitation system: create, accept, reject, and list invites with status tracking (PENDING/ACCEPTED/REJECTED)." },
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
                    ConfigureEmployeeInviteDtoSchema(schemas);
                    ConfigureAcceptInviteDtoSchema(schemas);
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
            createEmployeeSchema.Description = @"Payload for creating an employee invitation (NOT a direct employee).

⚠️  **IMPORTANT CHANGE:** This endpoint now creates INVITATIONS instead of direct employees.

**Process Flow:**
1. Creates a pending invitation with status 'PENDING'
2. Invite must be accepted via `/invites/{token}/accept` 
3. Employee is only created when invitation is accepted

**Required fields:**
- `name`: Employee full name for the invitation
- `email`: Valid email address for the invitation 
- `role`: Must be either `ADMIN` or `MEMBER`

**Note:** `userId` and `imageUrl` are now provided when accepting the invite.";
            createEmployeeSchema.Example = new OpenApiObject
            {
                ["name"] = new OpenApiString("Jane Doe"),
                ["email"] = new OpenApiString("jane.doe@example.com"),
                ["role"] = new OpenApiString("ADMIN")
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

    private static void ConfigureEmployeeInviteDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
        if (schemas.TryGetValue("EmployeeInviteDto", out var inviteSchema))
        {
            inviteSchema.Description = @"Employee invitation with status tracking and HATEOAS links.

**Status Values:**
- `PENDING`: Invitation sent, awaiting response
- `ACCEPTED`: Invitation accepted, employee created
- `REJECTED`: Invitation declined

**Process Flow:**
1. Created via POST `/yards/{id}/employees` 
2. Accept via POST `/invites/{token}/accept`
3. Reject via POST `/invites/{token}/reject`

**HATEOAS Links:**
- `self`: Link to this specific invite
- `yard`: Link to the associated yard
- `employees`: Link to yard employees (when accepted)";

            inviteSchema.Example = new OpenApiObject
            {
                ["id"] = new OpenApiString("inv_abc123"),
                ["email"] = new OpenApiString("jane.doe@example.com"),
                ["name"] = new OpenApiString("Jane Doe"),
                ["role"] = new OpenApiString("ADMIN"),
                ["status"] = new OpenApiString("PENDING"),
                ["token"] = new OpenApiString("a1b2c3d4e5f6789012345678901234567890abcd"),
                ["createdAt"] = new OpenApiString("2024-12-19T10:30:00Z"),
                ["acceptedAt"] = new OpenApiNull(),
                ["acceptedByUserId"] = new OpenApiNull(),
                ["yardId"] = new OpenApiString("yard_456"),
                ["links"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["href"] = new OpenApiString("/yards/yard_456/invites/inv_abc123"),
                        ["rel"] = new OpenApiString("self"),
                        ["method"] = new OpenApiString("GET")
                    }
                }
            };
        }
    }

    private static void ConfigureAcceptInviteDtoSchema(IDictionary<string, OpenApiSchema> schemas)
    {
        if (schemas.TryGetValue("AcceptInviteDto", out var acceptSchema))
        {
            acceptSchema.Description = @"Payload for accepting an employee invitation.

**Required fields:**
- `userId`: The user ID who is accepting the invitation

**Optional fields:**
- `imageUrl`: Profile image URL for the employee

**Process:**
1. Validates the invitation token exists and is PENDING
2. Creates a YardEmployee with the provided details
3. Updates invitation status to ACCEPTED
4. Returns the created YardEmployeeDto with HATEOAS links

**Note:** Once accepted, the invitation cannot be modified and the employee is immediately active.";

            acceptSchema.Example = new OpenApiObject
            {
                [UserIdFieldName] = new OpenApiString("usr_789"),
                ["imageUrl"] = new OpenApiString("https://cdn.example.com/profiles/jane.jpg")
            };
        }
    }
}
