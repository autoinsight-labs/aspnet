using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Services;
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
        yardGroup.MapPatch("/{id}",  UpdateYard);
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
        var yardsResponse = mapper.Map<PagedResponseDto<YardDto>>(yards);

        yardsResponse.Links = linkService.GenerateCollectionLinks("yards", pageNumber, pageSize, yards.TotalPages);

        foreach (var yard in yardsResponse.Data)
        {
            yard.Links = linkService.GenerateResourceLinks("yards", yard.Id);
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

        yardResponse.Links = linkService.GenerateResourceLinks("yards", yard.Id);

        return TypedResults.Ok(yardResponse);
    }

    private static async Task<Results<Created<YardDto>, ProblemHttpResult>> CreateYard(
        YardDto yardDto,
        IYardRepository yardRepository,
        IMapper mapper,
        ILinkService linkService
    )
    {
        try
        {
            var createdYard = await yardRepository.CreateAsync(mapper.Map<Yard>(yardDto));

            var yardDtoResult = mapper.Map<YardDto>(createdYard);

            yardDtoResult.Links = linkService.GenerateResourceLinks("yards", createdYard.Id);

            return TypedResults.Created($"/yards/{createdYard.Id}", yardDtoResult);
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

        newYard.Links = linkService.GenerateResourceLinks("yards", existingYard.Id);

        return TypedResults.Ok(newYard);
    }
}
