using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AutoInsightAPI.handlers;

public static class EmployeeHandler
{
    public static void Map(WebApplication app)
    {
        var employeeGroup = app.MapGroup("/yards/{id}/employees").WithTags("yard", "employee");

        employeeGroup.MapGet("/", GetYardEmployees);
        employeeGroup.MapPost("/", CreateYardEmployee);
        employeeGroup.MapGet("/{employeeId}", GetYardEmployeeById);
        employeeGroup.MapDelete("/{employeeId}", DeleteYardEmployee);
        employeeGroup.MapPatch("/{employeeId}", UpdateYardEmployee);
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
        if (pageNumber <= 0)
        {
            return TypedResults.BadRequest(
                // TypedResults.Problem(
                //     title: "Bad Request",
                //     detail: $"{nameof(pageNumber)} must be greater than 0",
                //     statusCode: StatusCodes.Status400BadRequest
                // )
            );
        }

        if (pageSize <= 0)
        {
            return TypedResults.BadRequest(
                // TypedResults.Problem(
                //     title: "Bad Request",
                //     detail: $"{nameof(pageSize)} must be greater than 0",
                //     statusCode: StatusCodes.Status400BadRequest
                // )
            );
        }

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

        var employees = await yardEmployeeRepository.ListPagedAsync(pageNumber, pageSize, yard);
        var employeesResponse = mapper.Map<PagedResponseDto<YardEmployeeDto>>(employees);

        employeesResponse.Links = linkService.GenerateCollectionLinks($"yards/{id}/employees", pageNumber, pageSize, employees.TotalPages);

        foreach (var employee in employeesResponse.Data)
        {
            employee.Links = linkService.GenerateResourceLinks($"yards/{id}/employees", employee.Id);
        }

        return TypedResults.Ok(employeesResponse);
    }

    private static async Task<Results<Created<YardEmployeeDto>, NotFound>> CreateYardEmployee(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        IMapper mapper,
        ILinkService linkService,
        string id,
        YardEmployeeDto yardEmployeeDto
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

        var newEmployee = new YardEmployee(
            name: yardEmployeeDto.Name,
            imageUrl: yardEmployeeDto.ImageUrl,
            role: yardEmployeeDto.Role,
            userId: yardEmployeeDto.UserId,
            yard: yard
        );

        var createdYardEmployee = await yardEmployeeRepository.CreateAsync(newEmployee);
        var yardEmployeeDtoResult = mapper.Map<YardEmployeeDto>(createdYardEmployee);

        yardEmployeeDtoResult.Links = linkService.GenerateResourceLinks($"yards/{id}/employees", createdYardEmployee.Id);

        return TypedResults.Created($"/yards/{createdYardEmployee.YardId}/employees/{createdYardEmployee.Id}",
            yardEmployeeDtoResult);
    }

    private static async Task<Results<Ok<YardEmployeeDto>, NotFound>> GetYardEmployeeById(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        IMapper mapper,
        ILinkService linkService,
        string id, string employeeId
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

        var yardEmployee = await yardEmployeeRepository.FindAsync(employeeId);

        if (yardEmployee is null)
        {
            return TypedResults.NotFound(
                // TypedResults.Problem(
                //     title: "Not Found",
                //     detail: $"Yard employee with id '{employeeId}' not found.",
                //     statusCode: StatusCodes.Status404NotFound
                // )
            );
        }

        var yardEmployeeResult = mapper.Map<YardEmployeeDto>(yardEmployee);

        yardEmployeeResult.Links = linkService.GenerateResourceLinks($"yards/{id}/employees", yardEmployee.Id);

        return TypedResults.Ok(yardEmployeeResult);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteYardEmployee(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        string id,
        string employeeId
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

        var yardEmployee = await yardEmployeeRepository.FindAsync(employeeId);

        if (yardEmployee is null)
        {
            return TypedResults.NotFound(
                // TypedResults.Problem(
                //     title: "Not Found",
                //     detail: $"Yard employee with id '{employeeId}' not found.",
                //     statusCode: StatusCodes.Status404NotFound
                // )
            );
        }

        await yardEmployeeRepository.DeleteAsync(yardEmployee);

        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<YardEmployeeDto>, BadRequest, NotFound>> UpdateYardEmployee(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        IMapper mapper,
        ILinkService linkService,
        YardEmployeeDto yardEmployeeDto,
        string id,
        string employeeId
    )
    {
        if (!Enum.IsDefined(typeof(EmployeeRole), yardEmployeeDto.Role))
        {
            return TypedResults.BadRequest(
                // TypedResults.Problem(
                //     title: "Invalid Role",
                //     detail: $"The role value '{yardEmployeeDTO.Role}' is not valid.",
                //     statusCode: StatusCodes.Status400BadRequest
                // )
            );
        }

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

        var yardEmployee = await yardEmployeeRepository.FindAsync(employeeId);

        if (yardEmployee is null)
        {
            return TypedResults.NotFound(
                // TypedResults.Problem(
                //     title: "Not Found",
                //     detail: $"Yard employee with id '{employeeId}' not found.",
                //     statusCode: StatusCodes.Status404NotFound
                // )
            );
        }

        mapper.Map(yardEmployeeDto, yardEmployee);
        await yardEmployeeRepository.UpdateAsync();

        var newYardEmployee = mapper.Map<YardEmployeeDto>(yardEmployee);

        newYardEmployee.Links = linkService.GenerateResourceLinks($"yards/{id}/employees", yardEmployee.Id);

        return TypedResults.Ok(newYardEmployee);
    }
}
