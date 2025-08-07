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

        yardGroup.MapGet("/", async Task<Results<Ok<PagedResponseDTO<YardDTO>>, BadRequest>> (
            IYardRepository yardRepository,
            IMapper mapper,
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

            var yards = await yardRepository.ListPagedAsync(pageNumber, pageSize);

            return TypedResults.Ok(mapper.Map<PagedResponseDTO<YardDTO>>(yards));
        });

        yardGroup.MapGet("/{id}",
            async Task<Results<Ok<YardDTO>, NotFound>> (string id, IYardRepository yardRepository, IMapper mapper) =>
            {
                var yard = await yardRepository.FindAsync(id);

                if (yard is null)
                {
                    return TypedResults.NotFound();
                }

                var yardResponse = mapper.Map<YardDTO>(yard);
                return TypedResults.Ok(yardResponse);
            });

        yardGroup.MapPost("/",
            async Task<Results<Created<YardDTO>, ProblemHttpResult>> (YardDTO yardDto, IYardRepository yardRepository,
                IMapper mapper) =>
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
            });

        yardGroup.MapDelete("/{id}",
            async Task<Results<NoContent, NotFound>> (string id, IYardRepository yardRepository) =>
            {
                var existingYard = await yardRepository.FindAsync(id);

                if (existingYard is null)
                {
                    return TypedResults.NotFound();
                }

                await yardRepository.DeleteAsync(existingYard);

                return TypedResults.NoContent();
            });

        yardGroup.MapPut("/{id}",
            async Task<Results<Ok<YardDTO>, NotFound>> (string id, YardDTO yardDto, IYardRepository yardRepository,
                IMapper mapper) =>
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
            });
    }
}