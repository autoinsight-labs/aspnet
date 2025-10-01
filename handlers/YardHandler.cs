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
`GET /yards?pageNumber=1&pageSize=5`

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
- 200 OK: Returns paginated yards data (`PagedResponseDto<YardDto>`)
- 400 Bad Request: Invalid `pageNumber` or `pageSize` (<= 0)
- 500 Internal Server Error: Unexpected server error")
            .WithName("ListYards")
            .Produces<Dtos.PagedResponseDto<YardDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.OperationId = "ListYards";
                op.Parameters.Add(new()
                {
                    Name = "pageNumber",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Query,
                    Description = "Page number (>= 1)",
                    Required = false
                });
                op.Parameters.Add(new()
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
`GET /yards/yrd_123`

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
- 200 OK: Returns yard data (`YardDto`)
- 404 Not Found: Yard with the specified ID does not exist
- 500 Internal Server Error: Unexpected server error")
            .WithName("GetYardById")
            .Produces<YardDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.OperationId = "GetYardById";
                return op;
            });

        yardGroup.MapPost("/", CreateYard)
            .WithSummary("Create yard")
            .WithDescription(@"Creates a new yard. The provided address must be complete, and ownerId must reference an existing user.
Example Request Body:
```json
{
    ""ownerId"": ""usr_owner_001"",
    ""address"": {
        ""country"": ""BR"",
        ""state"": ""SP"",
        ""city"": ""São Paulo"",
        ""zipCode"": ""01311-000"",
        ""neighborhood"": ""Bela Vista"",
        ""complement"": ""Av. Paulista, 1106""
    }
}
```

Example Response (201 Created):
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
")
            .Accepts<CreateYardDto>("application/json")
            .Produces<YardDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .AddEndpointFilter<ValidationFilter<CreateYardDto>>()
            .WithOpenApi(HandlerHelpers.BuildOpenApiOperation(
                "CreateYard",
                requestBody: ("Example payload to create a yard.", new OpenApiObject
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
                })
            ));

        yardGroup.MapDelete("/{id}", DeleteYard)
            .WithSummary("Delete yard")
            .WithDescription(@"Permanently deletes a yard from the system.

Path Parameters:
- id (string): Unique identifier of the yard to delete

Example Request:
`DELETE /yards/yrd_123`

Example Response (204 No Content):
(Empty response body)

Response Codes:
- 204 No Content: Yard successfully deleted
- 404 Not Found: Yard with the specified ID does not exist
- 500 Internal Server Error: Unexpected server error")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.OperationId = "DeleteYard";
                return op;
            });

        yardGroup.MapPatch("/{id}", UpdateYard)
            .WithSummary("Update yard")
            .WithDescription(@"Updates an existing yard by id. At least one field must be provided.
Example Request Body:
```json
{
    ""ownerId"": ""usr_owner_001"",
    ""address"": {
        ""city"": ""São Paulo"",
        ""neighborhood"": ""Bela Vista""
    }
}
```
")
            .Accepts<YardDto>("application/json")
            .Produces<YardDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidationFilter<YardDto>>()
            .WithOpenApi(HandlerHelpers.BuildOpenApiOperation(
                "UpdateYard",
                new Dictionary<string, (ParameterLocation, string)>
                {
                    { "id", (ParameterLocation.Path, "Yard identifier") }
                },
                ("Example payload to update a yard.", new OpenApiObject
                {
                    ["ownerId"] = new OpenApiString("usr_owner_001"),
                    ["address"] = new OpenApiObject
                    {
                        ["city"] = new OpenApiString("São Paulo"),
                        ["neighborhood"] = new OpenApiString("Bela Vista")
                    }
                })
            ));
    }

    private static async Task<Results<Ok<PagedResponseDto<YardDto>>, BadRequest>> GetYards(
        IYardRepository yardRepository,
        IMapper mapper,
        ILinkService linkService,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        return await HandlerHelpers.HandlePagedList<Yard, YardDto>(
            pageNumber,
            pageSize,
            yardRepository.ListPagedAsync,
            mapper,
            linkService,
            YardResource
        );
    }

    private static async Task<Results<Ok<YardDto>, NotFound>> GetYardById(
        string id,
        IYardRepository yardRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        return await HandlerHelpers.HandleGetById<Yard, YardDto>(
            id,
            yardRepository.FindAsync,
            mapper,
            linkService,
            YardResource
        );
    }

    private static async Task<Results<Created<YardDto>, BadRequest>> CreateYard(
        CreateYardDto createYardDto,
        IYardRepository yardRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        var yard = mapper.Map<Yard>(createYardDto);
        var createdYard = await yardRepository.CreateAsync(yard);
        var response = mapper.Map<YardDto>(createdYard);

        response.Links = linkService.GenerateResourceLinks(YardResource, response.Id);

        return TypedResults.Created($"/{YardResource}/{response.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteYard(string id,
        IYardRepository yardRepository
    )
    {
        return await HandlerHelpers.HandleDelete<Yard>(
            id,
            yardRepository.FindAsync,
            yardRepository.DeleteAsync
        );
    }

    private static async Task<Results<Ok<YardDto>, NotFound>> UpdateYard(
        string id,
        YardDto yardDto,
        IYardRepository yardRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        var yard = await yardRepository.FindAsync(id);
        if (yard is null)
            return TypedResults.NotFound();

        yard.Address.Update(
            yardDto.Address.Country,
            yardDto.Address.State,
            yardDto.Address.City,
            yardDto.Address.ZipCode,
            yardDto.Address.Neighborhood,
            yardDto.Address.Complement
        );

        yard.Update(yard.Address, yardDto.OwnerId);

        await yardRepository.UpdateAsync();

        var response = mapper.Map<YardDto>(yard);
        response.Links = linkService.GenerateResourceLinks(YardResource, id);

        return TypedResults.Ok(response);
    }
}
