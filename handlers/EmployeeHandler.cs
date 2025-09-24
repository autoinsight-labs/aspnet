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

public static class EmployeeHandler
{
    public static void Map(WebApplication app)
    {
        var employeeGroup = app.MapGroup("/yards/{id}/employees").WithTags("yard", "employee")
            .WithDescription("Manage yard employees: list, create, get, update and delete.");

        employeeGroup.MapGet("/", GetYardEmployees)
            .WithSummary("List yard employees")
            .WithDescription(@"Retrieves a paginated list of employees for a specific yard.

Path Parameters:
- id (string): Yard identifier

Query Parameters:
- pageNumber (optional): Page number to retrieve. Must be greater than zero. Default: 1
- pageSize (optional): Number of items per page. Must be greater than zero. Default: 10

Example Request:
`GET /yards/yrd_123/employees?pageNumber=1&pageSize=10`

Example Response (200 OK):
```json
{
  ""pageNumber"": 1,
  ""pageSize"": 10,
  ""totalPages"": 1,
  ""totalRecords"": 2,
  ""data"": [
    {
      ""id"": ""emp_001"",
      ""name"": ""Jane Doe"",
      ""imageUrl"": ""https://cdn.example.com/jane.png"",
      ""role"": ""ADMIN"",
      ""userId"": ""usr_002""
    }
  ]
}
```

Response Codes:
- 200 OK: Returns paginated employees (`PagedResponseDto<YardEmployeeDto>`)
- 400 Bad Request: Invalid `pageNumber` or `pageSize` (<= 0)
- 404 Not Found: Yard not found
- 500 Internal Server Error: Unexpected server error")
            .WithName("ListYardEmployees")
            .Produces<Dtos.PagedResponseDto<YardEmployeeDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.OperationId = "ListYardEmployees";
                return op;
            });

        employeeGroup.MapGet("/{employeeId}", GetYardEmployeeById)
            .WithSummary("Get yard employee by id")
            .WithDescription(@"Returns the employee data by id within the yard context.

Path Parameters:
- id (string): Yard identifier
- employeeId (string): Employee identifier

Example Request:
`GET /yards/yrd_123/employees/emp_001`

Example Response (200 OK):
```json
{
  ""id"": ""emp_001"",
  ""name"": ""Jane Doe"",
  ""imageUrl"": ""https://cdn.example.com/jane.png"",
  ""role"": ""ADMIN"",
  ""userId"": ""usr_002""
}
```

Response Codes:
- 200 OK: Returns the employee data (`YardEmployeeDto`)
- 404 Not Found: Yard or employee not found
- 500 Internal Server Error: Unexpected server error")
            .Produces<YardEmployeeDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.OperationId = "GetYardEmployeeById";
                return op;
            });

        employeeGroup.MapDelete("/{employeeId}", DeleteYardEmployee)
            .WithSummary("Delete yard employee")
            .WithDescription(@"Removes the specified employee from the yard.

Path Parameters:
- id (string): Yard identifier
- employeeId (string): Employee identifier

Example Request:
`DELETE /yards/yrd_123/employees/emp_001`

Example Response (204 No Content):
(Empty response body)

Response Codes:
- 204 No Content: Employee successfully removed
- 404 Not Found: Yard or employee not found
- 500 Internal Server Error: Unexpected server error")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithOpenApi(op =>
            {
                op.OperationId = "DeleteYardEmployee";
                return op;
            });

        employeeGroup.MapPatch("/{employeeId}", UpdateYardEmployee)
            .WithSummary("Update yard employee")
            .WithDescription(@"Updates the employee data within the yard. All fields are replace operations.
Example Request Body:
```json
{
    ""name"": ""Jane Doe"",
    ""imageUrl"": ""https://cdn.example.com/jane.png"",
    ""role"": ""MEMBER"",
    ""userId"": ""usr_002""
}
```
")
            .Accepts<YardEmployeeDto>("application/json")
            .Produces<YardEmployeeDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidationFilter<YardEmployeeDto>>()
            .WithOpenApi(HandlerHelpers.BuildOpenApiOperation(
                "UpdateYardEmployee",
                new Dictionary<string, (ParameterLocation, string)>
                {
                    { "id", (ParameterLocation.Path, "Yard identifier") },
                    { "employeeId", (ParameterLocation.Path, "Employee identifier") }
                },
                ("Example payload to update a yard employee.", new OpenApiObject
                {
                    ["name"] = new OpenApiString("Jane Doe"),
                    ["imageUrl"] = new OpenApiString("https://cdn.example.com/jane.png"),
                    ["role"] = new OpenApiString("MEMBER"),
                    ["userId"] = new OpenApiString("usr_002")
                })
            ));
    }

    private static async Task<Results<Ok<PagedResponseDto<YardEmployeeDto>>, BadRequest, NotFound>> GetYardEmployees(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        IMapper mapper,
        ILinkService linkService,
        string id,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        return await HandlerHelpers.HandlePagedListWithParent<Yard, YardEmployee, YardEmployeeDto>(
            id,
            pageNumber,
            pageSize,
            yardRepository.FindAsync,
            yardEmployeeRepository.ListPagedAsync,
            new ResourceContext(mapper, linkService, "yards", "employees")
        );
    }

    private static async Task<Results<Ok<YardEmployeeDto>, NotFound>> GetYardEmployeeById(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        IMapper mapper,
        ILinkService linkService,
        string id, string employeeId
    )
    {
        return await HandlerHelpers.HandleGetChildById<Yard, YardEmployee, YardEmployeeDto>(
            id,
            employeeId,
            yardRepository.FindAsync,
            yardEmployeeRepository.FindAsync,
            new ResourceContext(mapper, linkService, "yards", "employees")
        );
    }

    private static async Task<Results<NoContent, NotFound>> DeleteYardEmployee(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        string id,
        string employeeId
    )
    {
        return await HandlerHelpers.HandleDeleteChild<Yard, YardEmployee>(
            id,
            employeeId,
            yardRepository.FindAsync,
            yardEmployeeRepository.FindAsync,
            yardEmployeeRepository.DeleteAsync
        );
    }

    private static async Task<Results<Ok<YardEmployeeDto>, NotFound>> UpdateYardEmployee(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        IMapper mapper,
        ILinkService linkService,
        YardEmployeeDto yardEmployeeDto,
        string id,
        string employeeId
    )
    {
        return await HandlerHelpers.HandleUpdateChild<Yard, YardEmployee, YardEmployeeDto>(
            id,
            employeeId,
            yardEmployeeDto,
            yardRepository.FindAsync,
            yardEmployeeRepository.FindAsync,
            yardEmployeeRepository.UpdateAsync,
            new ResourceContext(mapper, linkService, "yards", "employees")
        );
    }
}
