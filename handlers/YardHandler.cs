using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using AutoInsightAPI.Validators;

namespace AutoInsightAPI.handlers;

public static class YardHandler
{
    private const string YardResource = "yards";
    
    public static void Map(WebApplication app)
    {
        var yardGroup = app.MapGroup($"/{YardResource}").WithTags("yard")
            .WithDescription("Endpoints to manage yards. Supports pagination, get by id, create, update and delete.");

        yardGroup.MapGet("/", GetYards)
            .WithSummary("List yards")
            .WithDescription(@"Retrieves a paginated list of all yards registered in the system.

Query Parameters:
- pageNumber (optional): Page number to retrieve. Must be greater than zero. Default: 1
- pageSize (optional): Number of items per page. Must be greater than zero. Default: 10

Example Request:
```
GET /yards?pageNumber=1&pageSize=5
```

Example Response (200 OK):
```json
{
  ""pageNumber"": 1,
  ""pageSize"": 5,
  ""totalPages"": 3,
  ""totalRecords"": 15,
  ""data"": [
    {
      ""id"": ""yrd_123"",
      ""ownerId"": ""usr_owner_001"",
      ""address"": {
        ""country"": ""BR"",
        ""state"": ""SP"",
        ""city"": ""São Paulo"",
        ""zipCode"": ""01311-000"",
        ""neighborhood"": ""Bela Vista""
      }
    }
  ]
}
```

Example Error Response (400 Bad Request):
```json
{
  ""statusCode"": 400,
  ""message"": ""Page number must be greater than zero.""
}
```

Response Codes:
- 200 OK: Returns paginated yards data (PagedResponseDto<YardDto>)
- 400 Bad Request: Invalid pageNumber or pageSize (<= 0)
- 500 Internal Server Error: Unexpected server error")
            .WithName("ListYards")
            .Produces<AutoInsightAPI.Dtos.PagedResponseDto<YardDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.OperationId = "ListYards";
                op.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "pageNumber",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Description = "Page number (>= 1)",
                    Required = false
                });
                op.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "pageSize",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Description = "Page size (>= 1)",
                    Required = false
                });
                return op;
            });

        yardGroup.MapGet("/{id}", GetYardById)
            .WithSummary("Get yard by id")
            .WithDescription(@"Retrieves detailed information about a specific yard using its unique identifier.

Path Parameters:
- id (string): Unique identifier of the yard

Example Request:
```
GET /yards/yrd_123
```

Example Response (200 OK):
```json
{
  ""id"": ""yrd_123"",
  ""ownerId"": ""usr_owner_001"",
  ""address"": {
    ""country"": ""BR"",
    ""state"": ""SP"",
    ""city"": ""São Paulo"",
    ""zipCode"": ""01311-000"",
    ""neighborhood"": ""Bela Vista""
  }
}
```

Example Error Response (404 Not Found):
```json
{
  ""statusCode"": 404,
  ""message"": ""Yard with id not found""
}
```

Response Codes:
- 200 OK: Returns yard data (YardDto)
- 404 Not Found: Yard with the specified ID does not exist
- 500 Internal Server Error: Unexpected server error")
            .WithName("GetYardById")
            .Produces<YardDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op => { op.OperationId = "GetYardById"; return op; });

        yardGroup.MapPost("/", CreateYard)
            .WithSummary("Create yard")
            .WithDescription("Creates a new yard. The provided address must be complete, and ownerId must reference an existing user.")
            .Accepts<YardDto>("application/json")
            .Produces<YardDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<ValidationFilter<YardDto>>()
            .WithOpenApi(op =>
            {
                op.OperationId = "CreateYard";
                op.RequestBody = new Microsoft.OpenApi.Models.OpenApiRequestBody
                {
                    Description = "Example payload to create a yard.",
                    Required = true,
                    Content = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiMediaType>
                    {
                        ["application/json"] = new Microsoft.OpenApi.Models.OpenApiMediaType
                        {
                            Example = new Microsoft.OpenApi.Any.OpenApiObject
                            {
                                ["ownerId"] = new Microsoft.OpenApi.Any.OpenApiString("usr_owner_001"),
                                ["address"] = new Microsoft.OpenApi.Any.OpenApiObject
                                {
                                    ["country"] = new Microsoft.OpenApi.Any.OpenApiString("BR"),
                                    ["state"] = new Microsoft.OpenApi.Any.OpenApiString("SP"),
                                    ["city"] = new Microsoft.OpenApi.Any.OpenApiString("São Paulo"),
                                    ["zipCode"] = new Microsoft.OpenApi.Any.OpenApiString("01311-000"),
                                    ["neighborhood"] = new Microsoft.OpenApi.Any.OpenApiString("Bela Vista"),
                                    ["complement"] = new Microsoft.OpenApi.Any.OpenApiString("Av. Paulista, 1106")
                                }
                            }
                        }
                    }
                };
                return op;
            });

        yardGroup.MapDelete("/{id}", DeleteYard)
            .WithSummary("Delete yard")
            .WithDescription(@"Permanently deletes a yard from the system.

Path Parameters:
- id (string): Unique identifier of the yard to delete

Example Request:
```
DELETE /yards/yrd_123
```

Example Response (204 No Content):
```
(Empty response body)
```

Response Codes:
- 204 No Content: Yard successfully deleted
- 404 Not Found: Yard with the specified ID does not exist
- 500 Internal Server Error: Unexpected server error")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op => { op.OperationId = "DeleteYard"; return op; });

        yardGroup.MapPatch("/{id}",  UpdateYard)
            .WithSummary("Update yard")
            .WithDescription("Updates an existing yard by id. At least one field must be provided.")
            .Accepts<YardDto>("application/json")
            .Produces<YardDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidationFilter<YardDto>>()
            .WithOpenApi(op =>
            {
                op.OperationId = "UpdateYard";
                op.Parameters.Add(new Microsoft.OpenApi.Models.OpenApiParameter
                {
                    Name = "id",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Path,
                    Description = "Yard identifier",
                    Required = true
                });
                op.RequestBody = new Microsoft.OpenApi.Models.OpenApiRequestBody
                {
                    Description = "Example payload to update a yard.",
                    Required = true,
                    Content = new Dictionary<string, Microsoft.OpenApi.Models.OpenApiMediaType>
                    {
                        ["application/json"] = new Microsoft.OpenApi.Models.OpenApiMediaType
                        {
                            Example = new Microsoft.OpenApi.Any.OpenApiObject
                            {
                                ["ownerId"] = new Microsoft.OpenApi.Any.OpenApiString("usr_owner_001"),
                                ["address"] = new Microsoft.OpenApi.Any.OpenApiObject
                                {
                                    ["city"] = new Microsoft.OpenApi.Any.OpenApiString("São Paulo"),
                                    ["neighborhood"] = new Microsoft.OpenApi.Any.OpenApiString("Bela Vista")
                                }
                            }
                        }
                    }
                };
                return op;
            });
    }

    private static async Task<Results<Ok<PagedResponseDto<YardDto>>, BadRequest>> GetYards(
        IYardRepository yardRepository,
        IMapper mapper,
        ILinkService linkService,
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

        var yards = await yardRepository.ListPagedAsync(pageNumber, pageSize);
        var yardsResponse = mapper.Map<PagedResponseDto<YardDto>>(yards);

        yardsResponse.Links = linkService.GenerateCollectionLinks(YardResource, pageNumber, pageSize, yards.TotalPages);

        foreach (var yard in yardsResponse.Data)
        {
            yard.Links = linkService.GenerateResourceLinks(YardResource, yard.Id);
        }

        return TypedResults.Ok(yardsResponse);
    }

    private static async Task<Results<Ok<YardDto>, NotFound>> GetYardById(
        string id,
        IYardRepository yardRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        var yard = await yardRepository.FindAsync(id);

        if (yard is null)
        {
            return TypedResults.NotFound();
        }

        var yardResponse = mapper.Map<YardDto>(yard);

        yardResponse.Links = linkService.GenerateResourceLinks(YardResource, yard.Id);

        return TypedResults.Ok(yardResponse);
    }

    private static async Task<Results<Created<YardDto>, ProblemHttpResult>> CreateYard(
        YardDto yardDto,
        IYardRepository yardRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        var createdYard = await yardRepository.CreateAsync(mapper.Map<Yard>(yardDto));

        var yardDtoResult = mapper.Map<YardDto>(createdYard);

        yardDtoResult.Links = linkService.GenerateResourceLinks(YardResource, createdYard.Id);

        return TypedResults.Created($"/{YardResource}/{createdYard.Id}", yardDtoResult);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteYard(string id,
        IYardRepository yardRepository
    )
    {
        var existingYard = await yardRepository.FindAsync(id);

        if (existingYard is null)
        {
            return TypedResults.NotFound();
        }

        await yardRepository.DeleteAsync(existingYard);

        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<YardDto>, NotFound>> UpdateYard(
        string id,
        YardDto yardDto,
        IYardRepository yardRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        var existingYard = await yardRepository.FindAsync(id);

        if (existingYard is null)
        {
            return TypedResults.NotFound();
        }

        mapper.Map(yardDto, existingYard);

        await yardRepository.UpdateAsync();

        var newYard = mapper.Map<YardDto>(existingYard);

        newYard.Links = linkService.GenerateResourceLinks(YardResource, existingYard.Id);

        return TypedResults.Ok(newYard);
    }
}
