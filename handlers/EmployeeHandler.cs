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
        var employeeGroup = app.MapGroup("/yards/{id}/employees").WithTags(["yard", "employee"]);

        employeeGroup.MapGet("/",
            async Task<Results<Ok<PagedResponseDTO<YardEmployeeDTO>>, BadRequest, NotFound>> (
                IYardRepository yardRepository,
                IYardEmployeeRepository yardEmployeeRepository,
                IMapper mapper,
                string id,
                int pageNumber = 1,
                int pageSize = 10
            ) =>
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

                return TypedResults.Ok(mapper.Map<PagedResponseDTO<YardEmployeeDTO>>(employees));
            }).WithTags("employee");

        employeeGroup.MapPost("/", async Task<Results<Created<YardEmployeeDTO>, NotFound>> (
            IYardRepository yardRepository,
            IYardEmployeeRepository yardEmployeeRepository,
            IMapper mapper,
            string id,
            YardEmployeeDTO yardEmployeeDto
        ) =>
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
            var yardEmployeeDtoResult = mapper.Map<YardEmployeeDTO>(createdYardEmployee);

            return TypedResults.Created($"/yard/{createdYardEmployee.YardId}/employees/{createdYardEmployee.Id}",
                yardEmployeeDtoResult);
        }).WithTags("employee");

        employeeGroup.MapGet("/{employeeId}", async Task<Results<Ok<YardEmployeeDTO>, NotFound>> (
            IYardRepository yardRepository,
            IYardEmployeeRepository yardEmployeeRepository,
            IMapper mapper,
            string id, string employeeId
        ) =>
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

            var yardEmployeeResult = mapper.Map<YardEmployeeDTO>(yardEmployee);

            return TypedResults.Ok(yardEmployeeResult);
        }).WithTags("employee");

        employeeGroup.MapDelete("/{employeeId}", async Task<Results<NoContent, NotFound>> (
            IYardRepository yardRepository,
            IYardEmployeeRepository yardEmployeeRepository,
            IMapper _,
            string id,
            string employeeId
        ) =>
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
        }).WithTags("employee");

        employeeGroup.MapPut("/{employeeId}",
            async Task<Results<Ok<YardEmployeeDTO>, BadRequest, NotFound>> (
                IYardRepository yardRepository,
                IYardEmployeeRepository yardEmployeeRepository,
                IMapper mapper,
                YardEmployeeDTO yardEmployeeDto,
                string id,
                string employeeId
            ) =>
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

                var newYardEmployee = mapper.Map<YardEmployeeDTO>(yardEmployee);

                return TypedResults.Ok(newYardEmployee);
            }).WithTags("employee");
    }
}