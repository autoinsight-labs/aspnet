using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.Repositories;
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
        employeeGroup.MapPut("/{employeeId}", UpdateYardEmployee);
    }

    private static async Task<Results<Ok<PagedResponseDto<YardEmployeeDto>>, BadRequest, NotFound>> GetYardEmployees(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        IMapper mapper,
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

        return TypedResults.Ok(mapper.Map<PagedResponseDto<YardEmployeeDto>>(employees));
    }

    private static async Task<Results<Created<YardEmployeeDto>, NotFound>> CreateYardEmployee(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        IMapper mapper,
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

        return TypedResults.Created($"/yard/{createdYardEmployee.YardId}/employees/{createdYardEmployee.Id}",
            yardEmployeeDtoResult);
    }

    private static async Task<Results<Ok<YardEmployeeDto>, NotFound>> GetYardEmployeeById(
        IYardRepository yardRepository,
        IYardEmployeeRepository yardEmployeeRepository,
        IMapper mapper,
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

        return TypedResults.Ok(newYardEmployee);
    }
}