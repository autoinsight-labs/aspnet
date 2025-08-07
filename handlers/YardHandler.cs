using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AutoInsightAPI.handlers;

public static class YardHandler
{
    public static void Map(WebApplication app)
    {
        var yardGroup = app.MapGroup("/yards").WithTags("yard");

        yardGroup.MapGet("/", GetYards);
        yardGroup.MapGet("/{id}", GetYardById);
        yardGroup.MapPost("/", CreateYard);
        yardGroup.MapDelete("/{id}", DeleteYard);
        yardGroup.MapPut("/{id}",  UpdateYard);
    }

    private static async Task<Results<Ok<PagedResponseDTO<YardDTO>>, BadRequest>> GetYards(
        IYardRepository yardRepository,
        IMapper mapper,
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

        var yards = await yardRepository.ListPagedAsync(pageNumber, pageSize);

        return TypedResults.Ok(mapper.Map<PagedResponseDTO<YardDTO>>(yards));
    }

    private static async Task<Results<Ok<YardDTO>, NotFound>> GetYardById(
        string id,
        IYardRepository yardRepository,
        IMapper mapper
    )
    {
        var yard = await yardRepository.FindAsync(id);

        if (yard is null)
        {
            return TypedResults.NotFound();
        }

        var yardResponse = mapper.Map<YardDTO>(yard);
        return TypedResults.Ok(yardResponse);
    }

    private static async Task<Results<Created<YardDTO>, ProblemHttpResult>> CreateYard(
        YardDTO yardDto,
        IYardRepository yardRepository,
        IMapper mapper
    )
    {
        try
        {
            var createdYard = await yardRepository.CreateAsync(mapper.Map<Yard>(yardDto));

            var yardDtoResult = mapper.Map<YardDTO>(createdYard);
            return TypedResults.Created($"/yard/{createdYard.Id}", yardDtoResult);
        }
        catch (Exception)
        {
            return TypedResults.Problem(
                title: "Internal Server Error",
                detail: "Something went wrong, please try again.",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
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

    private static async Task<Results<Ok<YardDTO>, NotFound>> UpdateYard(
        string id,
        YardDTO yardDto,
        IYardRepository yardRepository,
        IMapper mapper
    )
    {
        var existingYard = await yardRepository.FindAsync(id);

        if (existingYard is null)
        {
            return TypedResults.NotFound();
        }

        mapper.Map(yardDto, existingYard);

        await yardRepository.UpdateAsync();

        var newYard = mapper.Map<YardDTO>(existingYard);
        return TypedResults.Ok(newYard);
    }
}