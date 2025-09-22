using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using AutoInsightAPI.Validators;

namespace AutoInsightAPI.handlers;

public static class VehicleHandler
{
    public static void Map(WebApplication app)
    {
        var vehicleGroup = app.MapGroup("/vehicles").WithTags("vehicle")
            .WithDescription("Query vehicles by id and QR Code.");
        var yardVehicleGroup = app.MapGroup("/yards/{id}/vehicles").WithTags("vehicle", "yard")
            .WithDescription("Manage vehicles linked to a specific yard.");

        vehicleGroup.MapGet("/", GetVehicleByQrCode)
            .WithSummary("Get vehicle by QR Code")
            .WithDescription("Returns a vehicle associated with the provided QR Code. Provide qrCodeId as query parameter.")
            .WithName("GetVehicleByQrCode")
            .Produces<VehicleDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.OperationId = "GetVehicleByQrCode";
                op.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "qrCodeId",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Description = "QR Code identifier",
                    Required = true
                });
                op.Responses["200"].Content["application/json"].Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["id"] = new Microsoft.OpenApi.Any.OpenApiString("veh_abc123"),
                    ["plate"] = new Microsoft.OpenApi.Any.OpenApiString("ABC1D23"),
                    ["model"] = new Microsoft.OpenApi.Any.OpenApiObject
                    {
                        ["id"] = new Microsoft.OpenApi.Any.OpenApiString("mdl_001"),
                        ["name"] = new Microsoft.OpenApi.Any.OpenApiString("Honda CG 160"),
                        ["year"] = new Microsoft.OpenApi.Any.OpenApiInteger(2023)
                    },
                    ["userId"] = new Microsoft.OpenApi.Any.OpenApiString("usr_001")
                };
                return op;
            });
        vehicleGroup.MapGet("/{id}", GetVehicleById)
            .WithSummary("Get vehicle by id")
            .WithDescription("Returns a vehicle by its id.")
            .Produces<VehicleDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi(op => { op.OperationId = "GetVehicleById"; return op; });

        yardVehicleGroup.MapGet("/", GetYardVehicles)
            .WithSummary("List yard vehicles")
            .WithDescription(@"Retrieves a paginated list of vehicles linked to a specific yard.

Path Parameters:
- id (string): Yard identifier

Query Parameters:
- pageNumber (optional): Page number to retrieve. Must be greater than zero. Default: 1
- pageSize (optional): Number of items per page. Must be greater than zero. Default: 10

Example Request:
```
GET /yards/yrd_123/vehicles?pageNumber=1&pageSize=10
```

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
- 200 OK: Returns paginated yard vehicles (PagedResponseDto<YardVehicleDto>)
- 400 Bad Request: Invalid pageNumber or pageSize (<= 0)
- 404 Not Found: Yard not found
- 500 Internal Server Error: Unexpected server error")
            .Produces<AutoInsightAPI.Dtos.PagedResponseDto<YardVehicleDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op => { op.OperationId = "ListYardVehicles"; return op; });
        yardVehicleGroup.MapGet("/{yardVehicleId}", GetYardVehicleById)
            .WithSummary("Get yard vehicle by id")
            .WithDescription(@"Returns a yard vehicle by its id.

Path Parameters:
- id (string): Yard identifier
- yardVehicleId (string): Yard vehicle identifier

Example Request:
```
GET /yards/yrd_123/vehicles/yv_001
```

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
- 200 OK: Returns yard vehicle (YardVehicleDto)
- 404 Not Found: Yard or yard vehicle not found
- 500 Internal Server Error: Unexpected server error")
            .Produces<YardVehicleDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op => { op.OperationId = "GetYardVehicleById"; return op; });
        yardVehicleGroup.MapPatch("/{yardVehicleId}", UpdateYardVehicle)
            .WithSummary("Update yard vehicle")
            .WithDescription("Updates a vehicle associated with the yard. Status must be one of SCHEDULED, WAITING, ON_SERVICE, FINISHED or CANCELLED.")
            .Accepts<YardVehicleDto>("application/json")
            .Produces<YardVehicleDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidationFilter<YardVehicleDto>>()
            .WithOpenApi(op =>
            {
                op.OperationId = "UpdateYardVehicle";
                op.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "id",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Path,
                    Description = "Yard identifier",
                    Required = true
                });
                op.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "yardVehicleId",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Path,
                    Description = "Yard vehicle identifier",
                    Required = true
                });
                op.RequestBody = new Microsoft.OpenApi.Models.OpenApiRequestBody
                {
                    Description = "Example payload to update a yard vehicle.",
                    Required = true,
                    Content = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiMediaType>
                    {
                        ["application/json"] = new Microsoft.OpenApi.Models.OpenApiMediaType
                        {
                            Example = new Microsoft.OpenApi.Any.OpenApiObject
                            {
                                ["status"] = new Microsoft.OpenApi.Any.OpenApiString("ON_SERVICE"),
                                ["enteredAt"] = new Microsoft.OpenApi.Any.OpenApiString("2025-05-20T10:00:00Z"),
                                ["leftAt"] = new Microsoft.OpenApi.Any.OpenApiNull(),
                                ["vehicle"] = new Microsoft.OpenApi.Any.OpenApiObject
                                {
                                    ["id"] = new Microsoft.OpenApi.Any.OpenApiString("veh_abc123"),
                                    ["plate"] = new Microsoft.OpenApi.Any.OpenApiString("ABC1D23"),
                                    ["model"] = new Microsoft.OpenApi.Any.OpenApiObject
                                    {
                                        ["id"] = new Microsoft.OpenApi.Any.OpenApiString("mdl_001"),
                                        ["name"] = new Microsoft.OpenApi.Any.OpenApiString("Honda CG 160"),
                                        ["year"] = new Microsoft.OpenApi.Any.OpenApiInteger(2023)
                                    },
                                    ["userId"] = new Microsoft.OpenApi.Any.OpenApiString("usr_001")
                                }
                            }
                        }
                    }
                };
                return op;
            });
        yardVehicleGroup.MapPost("/", CreateYardVehicle)
            .WithSummary("Create yard vehicle")
            .WithDescription("Creates a link between a vehicle and a yard. Requires a valid vehicle id and a non-null enteredAt.")
            .Accepts<YardVehicleDto>("application/json")
            .Produces<YardVehicleDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidationFilter<YardVehicleDto>>()
            .WithOpenApi(op =>
            {
                op.OperationId = "CreateYardVehicle";
                op.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "id",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Path,
                    Description = "Yard identifier",
                    Required = true
                });
                op.RequestBody = new Microsoft.OpenApi.Models.OpenApiRequestBody
                {
                    Description = "Example payload to create a yard vehicle.",
                    Required = true,
                    Content = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiMediaType>
                    {
                        ["application/json"] = new Microsoft.OpenApi.Models.OpenApiMediaType
                        {
                            Example = new Microsoft.OpenApi.Any.OpenApiObject
                            {
                                ["status"] = new Microsoft.OpenApi.Any.OpenApiString("WAITING"),
                                ["enteredAt"] = new Microsoft.OpenApi.Any.OpenApiString("2025-05-20T09:30:00Z"),
                                ["vehicle"] = new Microsoft.OpenApi.Any.OpenApiObject
                                {
                                    ["id"] = new Microsoft.OpenApi.Any.OpenApiString("veh_abc123"),
                                    ["plate"] = new Microsoft.OpenApi.Any.OpenApiString("ABC1D23"),
                                    ["model"] = new Microsoft.OpenApi.Any.OpenApiObject
                                    {
                                        ["id"] = new Microsoft.OpenApi.Any.OpenApiString("mdl_001"),
                                        ["name"] = new Microsoft.OpenApi.Any.OpenApiString("Honda CG 160"),
                                        ["year"] = new Microsoft.OpenApi.Any.OpenApiInteger(2023)
                                    },
                                    ["userId"] = new Microsoft.OpenApi.Any.OpenApiString("usr_001")
                                }
                            }
                        }
                    }
                };
                return op;
            });
    }

    private static async Task<Results<Ok<VehicleDto>, NotFound>> GetVehicleByQrCode(
        string qrCodeId,
        IVehicleRepository vehicleRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        var vehicle = await vehicleRepository.FindAsyncByQRCode(qrCodeId);

        if (vehicle is null)
        {
            return TypedResults.NotFound();
        }


        var vehicleResponse = mapper.Map<VehicleDto>(vehicle);

        vehicleResponse.Links = linkService.GenerateResourceLinks("vehicles", vehicle.Id);

        return TypedResults.Ok(vehicleResponse);
    }

    private static async Task<Results<Ok<VehicleDto>, NotFound>> GetVehicleById(
        string id,
        IVehicleRepository vehicleRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        var vehicle = await vehicleRepository.FindAsyncById(id);

        if (vehicle is null)
        {
            return TypedResults.NotFound();
        }


        var vehicleResponse = mapper.Map<VehicleDto>(vehicle);

        vehicleResponse.Links = linkService.GenerateResourceLinks("vehicles", vehicle.Id);

        return TypedResults.Ok(vehicleResponse);
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
        if (pageNumber <= 0)
        {
            return TypedResults.BadRequest();
        }

        if (pageSize <= 0)
        {
            return TypedResults.BadRequest();
        }

        var yard = await yardRepository.FindAsync(id);

        if (yard is null)
        {
            return TypedResults.NotFound();
        }

        var yardVehicles = await yardVehicleRepository.ListPagedAsync(pageNumber, pageSize, yard);
        var yardVehiclesResponse = mapper.Map<PagedResponseDto<YardVehicleDto>>(yardVehicles);

        yardVehiclesResponse.Links = linkService.GenerateCollectionLinks($"yards/{id}/vehicles", pageNumber, pageSize, yardVehicles.TotalPages);

        foreach (var yardVehicle in yardVehiclesResponse.Data)
        {
            yardVehicle.Links = linkService.GenerateResourceLinks($"yards/{id}/vehicles", yardVehicle.Id);
            yardVehicle.Vehicle.Links = linkService.GenerateResourceLinks("vehicles", yardVehicle.Vehicle.Id);
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
        var yard = await yardRepository.FindAsync(id);

        if (yard is null)
        {
            return TypedResults.NotFound(
                // TypedResults.Problem(
                //     title: "Not Found",
                //     detail: $"Yard with id '{id}' not found.",
                //     statusCode: StatusCodes.Status404NotFound
                // )
            );
        }

        var yardVehicle = await yardVehicleRepository.FindAsync(yardVehicleId);

        if (yardVehicle is null)
        {
            return TypedResults.NotFound();
        }

        var yardVehicleResult = mapper.Map<YardVehicleDto>(yardVehicle);

        yardVehicleResult.Links = linkService.GenerateResourceLinks($"yards/{id}/vehicles", yardVehicle.Id);

        return TypedResults.Ok(yardVehicleResult);
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
        var yard = await yardRepository.FindAsync(id);

        if (yard is null)
        {
            return TypedResults.NotFound();
        }

        var yardVehicle = await yardVehicleRepository.FindAsync(yardVehicleId);

        if (yardVehicle is null)
        {
            return TypedResults.NotFound(
                // TypedResults.Problem(
                //     title: "Not Found",
                //     detail: $"Yard vehicle with id '{yardVehicleId}' not found.",
                //     statusCode: StatusCodes.Status404NotFound
                // )
            );
        }


        mapper.Map(yardVehicleDto, yardVehicle);
        await yardVehicleRepository.UpdateAsync();

        var newYardVehicle = mapper.Map<YardVehicleDto>(yardVehicle);

        newYardVehicle.Links = linkService.GenerateResourceLinks($"yards/{id}/vehicles", yardVehicle.Id);

        return TypedResults.Ok(newYardVehicle);
    }

    private static async Task<Results<Created<YardVehicleDto>, BadRequest, NotFound>> CreateYardVehicle(
        IYardRepository yardRepository,
        IYardVehicleRepository yardVehicleRepository,
        IVehicleRepository vehicleRepository,
        IMapper mapper,
        ILinkService linkService,
        string id,
        YardVehicleDto yardVehicleDto
    )
    {
        var yard = await yardRepository.FindAsync(id);

        if (yard is null)
        {
            return TypedResults.NotFound();
        }

        var vehicle = await vehicleRepository.FindAsyncById(yardVehicleDto.Vehicle.Id);

        if (vehicle is null)
        {
            return TypedResults.NotFound();
        }

        var newYardVehicle = new YardVehicle(
            status: yardVehicleDto.Status,
            enteredAt: (DateTime)yardVehicleDto.EnteredAt,
            leftAt: yardVehicleDto.LeftAt,
            vehicle: vehicle,
            yard: yard
        );

        var createdYardVehicle = await yardVehicleRepository.CreateAsync(newYardVehicle);
        var yardVehicleDtoResult = mapper.Map<YardVehicleDto>(createdYardVehicle);

        yardVehicleDtoResult.Links = linkService.GenerateResourceLinks($"yards/{id}/vehicles", createdYardVehicle.Id);

        return TypedResults.Created($"/yards/{createdYardVehicle.YardId}/vehicles/{createdYardVehicle.Id}",
            yardVehicleDtoResult);
    }
}
