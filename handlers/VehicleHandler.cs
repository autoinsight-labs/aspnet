using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using AutoInsightAPI.Validators;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace AutoInsightAPI.handlers;

public record CreateYardVehicleRepositories(
    IYardRepository YardRepository,
    IYardVehicleRepository YardVehicleRepository,
    IVehicleRepository VehicleRepository,
    IModelRepository ModelRepository
);

public static class VehicleHandler
{
    private const string VehiclesResource = "vehicles";
    private const string VehicleResource = "vehicle";

    public static void Map(WebApplication app)
    {
        var vehicleGroup = app.MapGroup("/vehicles").WithTags(VehicleResource)
            .WithDescription("Query vehicles by id and QR Code.");
        var yardVehicleGroup = app.MapGroup("/yards/{id}/vehicles").WithTags(VehicleResource, "yard")
            .WithDescription("Manage vehicles linked to a specific yard.");

        vehicleGroup.MapGet("/", GetVehicleByQrCode)
            .WithSummary("Get vehicle by QR Code")
            .WithDescription(@"Returns a vehicle associated with the provided QR Code. Provide qrCodeId as query parameter.
Example Request:
`GET /vehicles?qrCodeId=qr_123`

Example Response (200 OK):
```json
{
    ""id"": ""veh_abc123"",
    ""plate"": ""ABC1D23"",
    ""model"": {
        ""id"": ""mdl_001"",
        ""name"": ""Honda CG 160"",
        ""year"": 2023
    },
    ""userId"": ""usr_001""
}
```")
            .WithName("GetVehicleByQrCode")
            .Produces<VehicleDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.OperationId = "GetVehicleByQrCode";
                op.Parameters.Add(new()
                {
                    Name = "qrCodeId",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Description = "QR Code identifier",
                    Required = true
                });
                op.Responses["200"].Content["application/json"].Example = GetVehicleExample();
                return op;
            });
        vehicleGroup.MapGet("/{id}", GetVehicleById)
            .WithSummary("Get vehicle by id")
            .WithDescription(@"Returns a vehicle by its id.
Example Request:
`GET /vehicles/veh_abc123`
")
            .Produces<VehicleDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.OperationId = "GetVehicleById";
                return op;
            });

        yardVehicleGroup.MapGet("/", GetYardVehicles)
            .WithSummary("List yard vehicles")
            .WithDescription(@"Retrieves a paginated list of vehicles linked to a specific yard.

Path Parameters:
- id (string): Yard identifier

Query Parameters:
- pageNumber (optional): Page number to retrieve. Must be greater than zero. Default: 1
- pageSize (optional): Number of items per page. Must be greater than zero. Default: 10

Example Request:
`GET /yards/yrd_123/vehicles?pageNumber=1&pageSize=10`

Example Response (200 OK):
```json
{
  ""pageNumber"": 1,
  ""pageSize"": 10,
  ""totalPages"": 1,
  ""totalRecords"": 1,
  ""data"": [
    {
      ""id"": ""yv_001"",
      ""status"": ""ON_SERVICE"",
      ""enteredAt"": ""2025-05-20T10:00:00Z"",
      ""leftAt"": null,
      ""vehicle"": {
        ""id"": ""veh_abc123"",
        ""plate"": ""ABC1D23"",
        ""model"": { ""id"": ""mdl_001"", ""name"": ""Honda CG 160"", ""year"": 2023 },
        ""userId"": ""usr_001""
      }
    }
  ]
}
```

Response Codes:
- 200 OK: Returns paginated yard vehicles (`PagedResponseDto<YardVehicleDto>`)
- 400 Bad Request: Invalid `pageNumber` or `pageSize` (<= 0)
- 404 Not Found: Yard not found
- 500 Internal Server Error: Unexpected server error")
            .Produces<Dtos.PagedResponseDto<YardVehicleDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.OperationId = "ListYardVehicles";
                return op;
            });
        yardVehicleGroup.MapGet("/{yardVehicleId}", GetYardVehicleById)
            .WithSummary("Get yard vehicle by id")
            .WithDescription(@"Returns a yard vehicle by its id.

Path Parameters:
- id (string): Yard identifier
- yardVehicleId (string): Yard vehicle identifier

Example Request:
`GET /yards/yrd_123/vehicles/yv_001`

Example Response (200 OK):
```json
{
  ""id"": ""yv_001"",
  ""status"": ""ON_SERVICE"",
  ""enteredAt"": ""2025-05-20T10:00:00Z"",
  ""leftAt"": null,
  ""vehicle"": {
    ""id"": ""veh_abc123"",
    ""plate"": ""ABC1D23"",
    ""model"": { ""id"": ""mdl_001"", ""name"": ""Honda CG 160"", ""year"": 2023 },
    ""userId"": ""usr_001""
  }
}
```

Response Codes:
- 200 OK: Returns yard vehicle (`YardVehicleDto`)
- 404 Not Found: Yard or yard vehicle not found
- 500 Internal Server Error: Unexpected server error")
            .Produces<YardVehicleDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.OperationId = "GetYardVehicleById";
                return op;
            });
        yardVehicleGroup.MapPatch("/{yardVehicleId}", UpdateYardVehicle)
            .WithSummary("Update yard vehicle")
            .WithDescription(@"Updates a vehicle associated with the yard. Status must be one of SCHEDULED, WAITING, ON_SERVICE, FINISHED or CANCELLED.
Example Request Body:
```json
{
    ""status"": ""ON_SERVICE"",
    ""enteredAt"": ""2025-05-20T10:00:00Z"",
    ""leftAt"": null,
    ""vehicle"": {
        ""id"": ""veh_abc123"",
        ""plate"": ""ABC1D23"",
        ""model"": {
            ""id"": ""mdl_001"",
            ""name"": ""Honda CG 160"",
            ""year"": 2023
        },
        ""userId"": ""usr_001""
    }
}
```
")
            .Accepts<YardVehicleDto>("application/json")
            .Produces<YardVehicleDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidationFilter<YardVehicleDto>>()
            .WithOpenApi(HandlerHelpers.BuildOpenApiOperation(
                "UpdateYardVehicle",
                new Dictionary<string, (ParameterLocation, string)>
                {
                    { "id", (ParameterLocation.Path, "Yard identifier") },
                    { "yardVehicleId", (ParameterLocation.Path, "Yard vehicle identifier") }
                },
                ("Example payload to update a yard vehicle.", new OpenApiObject
                {
                    ["status"] = new OpenApiString("ON_SERVICE"),
                    ["enteredAt"] = new OpenApiString("2025-05-20T10:00:00Z"),
                    ["leftAt"] = new OpenApiNull(),
                    [VehicleResource] = GetVehicleExample()
                })
            ));
        yardVehicleGroup.MapPost("/", CreateYardVehicle)
            .WithSummary("Create yard vehicle")
            .WithDescription(@"Creates a link between a vehicle and a yard. You can either provide an existing vehicleId OR a complete vehicle object to create a new vehicle.

Option 1 - Link existing vehicle:
```json
{
    ""status"": ""WAITING"",
    ""enteredAt"": ""2025-05-20T09:30:00Z"",
    ""vehicleId"": ""veh_abc123""
}
```

Option 2 - Create new vehicle with existing model:
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

Option 3 - Create new vehicle and new model:
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
```
")
            .Accepts<CreateYardVehicleDto>("application/json")
            .Produces<YardVehicleDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidationFilter<CreateYardVehicleDto>>()
            .WithOpenApi(HandlerHelpers.BuildOpenApiOperation(
                "CreateYardVehicle",
                new Dictionary<string, (ParameterLocation, string)>
                {
                    { "id", (ParameterLocation.Path, "Yard identifier") }
                },
                ("Option 1: Link existing vehicle", new OpenApiObject
                {
                    ["status"] = new OpenApiString("WAITING"),
                    ["enteredAt"] = new OpenApiString("2025-05-20T09:30:00Z"),
                    ["vehicleId"] = new OpenApiString("veh_abc123")
                })
            ));
    }

    private static OpenApiObject GetVehicleExample()
    {
        return new OpenApiObject
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

    private static async Task<Results<Ok<VehicleDto>, NotFound>> GetVehicleByQrCode(
        string qrCodeId,
        IVehicleRepository vehicleRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        return await HandlerHelpers.HandleGetById<Vehicle, VehicleDto>(
            qrCodeId,
            vehicleRepository.FindAsyncByQRCode,
            mapper,
            linkService,
            VehicleResource
        );
    }

    private static async Task<Results<Ok<VehicleDto>, NotFound>> GetVehicleById(
        string id,
        IVehicleRepository vehicleRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        return await HandlerHelpers.HandleGetById<Vehicle, VehicleDto>(
            id,
            vehicleRepository.FindAsyncById,
            mapper,
            linkService,
            VehicleResource
        );
    }

    private static async Task<Results<Ok<PagedResponseDto<YardVehicleDto>>, BadRequest, NotFound>> GetYardVehicles(
        IYardRepository yardRepository,
        IYardVehicleRepository yardVehicleRepository,
        IMapper mapper,
        ILinkService linkService,
        string id,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        var validationResult = HandlerHelpers.ValidatePaginationParameters(pageNumber, pageSize);
        if (validationResult != null)
            return validationResult;

        var yard = await yardRepository.FindAsync(id);
        if (yard is null)
            return TypedResults.NotFound();

        var yardVehicles = await yardVehicleRepository.ListPagedAsync(pageNumber, pageSize, yard);
        var yardVehiclesResponse = mapper.Map<PagedResponseDto<YardVehicleDto>>(yardVehicles);

        yardVehiclesResponse.Links = linkService.GenerateCollectionLinks($"yards/{id}/vehicles", pageNumber, pageSize, yardVehicles.TotalPages);

        foreach (var yardVehicle in yardVehiclesResponse.Data)
        {
            yardVehicle.Links = linkService.GenerateResourceLinks($"yards/{id}/vehicles", yardVehicle.Id);
            yardVehicle.Vehicle.Links = linkService.GenerateResourceLinks(VehicleResource, yardVehicle.Vehicle.Id);
        }

        return TypedResults.Ok(yardVehiclesResponse);
    }

    private static async Task<Results<Ok<YardVehicleDto>, BadRequest, NotFound>> GetYardVehicleById(
        IYardRepository yardRepository,
        IYardVehicleRepository yardVehicleRepository,
        IMapper mapper,
        ILinkService linkService,
        string id,
        string yardVehicleId
    )
    {
        var result = await HandlerHelpers.HandleGetChildById<Yard, YardVehicle, YardVehicleDto>(
            id,
            yardVehicleId,
            yardRepository.FindAsync,
            yardVehicleRepository.FindAsync,
            new ResourceContext(mapper, linkService, "yards", VehicleResource)
        );
        
        return result.Result switch
        {
            Ok<YardVehicleDto> ok => TypedResults.Ok(ok.Value),
            _ => TypedResults.NotFound()
        };
    }

    private static async Task<Results<Ok<YardVehicleDto>, BadRequest, NotFound>> UpdateYardVehicle(
        IYardRepository yardRepository,
        IYardVehicleRepository yardVehicleRepository,
        IMapper mapper,
        ILinkService linkService,
        string id,
        string yardVehicleId,
        YardVehicleDto yardVehicleDto
    )
    {
        var result = await HandlerHelpers.HandleUpdateChild<Yard, YardVehicle, YardVehicleDto>(
            id,
            yardVehicleId,
            yardVehicleDto,
            yardRepository.FindAsync,
            yardVehicleRepository.FindAsync,
            yardVehicleRepository.UpdateAsync,
            new ResourceContext(mapper, linkService, "yards", VehicleResource)
        );
        
        return result.Result switch
        {
            Ok<YardVehicleDto> ok => TypedResults.Ok(ok.Value),
            _ => TypedResults.NotFound()
        };
    }

    private static async Task<Results<Created<YardVehicleDto>, BadRequest, NotFound>> CreateYardVehicle(
        CreateYardVehicleRepositories repositories,
        IMapper mapper,
        ILinkService linkService,
        string id,
        CreateYardVehicleDto createYardVehicleDto
    )
    {
        var yard = await repositories.YardRepository.FindAsync(id);
        if (yard is null)
            return TypedResults.NotFound();

        Vehicle? vehicle = null;

        if (!string.IsNullOrEmpty(createYardVehicleDto.VehicleId))
        {
            vehicle = await repositories.VehicleRepository.FindAsyncById(createYardVehicleDto.VehicleId);
            if (vehicle is null)
                return TypedResults.NotFound();
        }
        else if (createYardVehicleDto.Vehicle != null)
        {
            Model? model = null;
            
            if (!string.IsNullOrEmpty(createYardVehicleDto.Vehicle.ModelId))
            {
                model = await repositories.ModelRepository.FindAsyncById(createYardVehicleDto.Vehicle.ModelId);
                if (model is null)
                    return TypedResults.NotFound();
            }
            else if (createYardVehicleDto.Vehicle.Model != null)
            {
                var newModel = mapper.Map<Model>(createYardVehicleDto.Vehicle.Model);
                model = await repositories.ModelRepository.CreateAsync(newModel);
            }
            
            if (model is null)
                return TypedResults.BadRequest();
            
            var newVehicle = new Vehicle(
                plate: createYardVehicleDto.Vehicle.Plate,
                model: model,
                userId: createYardVehicleDto.Vehicle.UserId
            );
            vehicle = await repositories.VehicleRepository.CreateAsync(newVehicle);
        }
        else
        {
            return TypedResults.BadRequest();
        }

        var newYardVehicle = new YardVehicle(
            status: createYardVehicleDto.Status,
            enteredAt: createYardVehicleDto.EnteredAt ?? DateTime.UtcNow,
            leftAt: createYardVehicleDto.LeftAt,
            vehicle: vehicle,
            yard: yard
        );

        var createdYardVehicle = await repositories.YardVehicleRepository.CreateAsync(newYardVehicle);
        var yardVehicleDtoResult = mapper.Map<YardVehicleDto>(createdYardVehicle);

        yardVehicleDtoResult.Links = linkService.GenerateResourceLinks($"yards/{id}/vehicles", createdYardVehicle.Id);
        yardVehicleDtoResult.Vehicle.Links = linkService.GenerateResourceLinks(VehicleResource, yardVehicleDtoResult.Vehicle.Id);

        return TypedResults.Created($"/yards/{createdYardVehicle.YardId}/vehicles/{createdYardVehicle.Id}",
            yardVehicleDtoResult);
    }
}
