using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AutoInsightAPI.handlers;

public static class VehicleHandler
{
    public static void Map(WebApplication app)
    {
        var vehicleGroup = app.MapGroup("/vehicles").WithTags("vehicle");
        var yardVehicleGroup = app.MapGroup("/yards/{id}/vehicles").WithTags("vehicle", "yard");

        vehicleGroup.MapGet("/", GetVehicleByQrCode);
        vehicleGroup.MapGet("/{id}", GetVehicleById);

        yardVehicleGroup.MapGet("/", GetYardVehicles);
        yardVehicleGroup.MapGet("/{yardVehicleId}", GetYardVehicleById);
        yardVehicleGroup.MapPatch("/{yardVehicleId}", UpdateYardVehicle);
        yardVehicleGroup.MapPost("/", CreateYardVehicle);
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
            return TypedResults.NotFound(
                // TypedResults.Problem(
                //     title: "Not Found",
                //     detail: $"Yard vehicle with id '{yardVehicleId}' not found.",
                //     statusCode: StatusCodes.Status404NotFound
                // )
            );
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
        if (!Enum.IsDefined(typeof(Status), yardVehicleDto.Status))
        {
            return TypedResults.BadRequest(
                // TypedResults.Problem(
                //     title: "Invalid Role",
                //     detail: $"The role value '{yardVehicleDTO.Status}' is not valid.",
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
        if (!Enum.IsDefined(typeof(Status), yardVehicleDto.Status))
        {
            return TypedResults.BadRequest(
                // TypedResults.Problem(
                //     title: "Invalid Role",
                //     detail: $"The role value '{yardVehicleDTO.Status}' is not valid.",
                //     statusCode: StatusCodes.Status400BadRequest
                // )
            );
        }

        if (yardVehicleDto.EnteredAt is not DateTime enteredAt)
        {
            return TypedResults.BadRequest(
                // TypedResults.Problem(
                //     title: "Invalid entered_at time",
                //     detail: $"The entered_at field cannot be null",
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

        var vehicle = await vehicleRepository.FindAsyncById(yardVehicleDto.Vehicle.Id);

        if (vehicle is null)
        {
            return TypedResults.NotFound(
                // TypedResults.Problem(
                //     title: "Not Found",
                //     detail: $"Vehicle with id '{yardVehicleDTO.Vehicle.Id}' not found.",
                //     statusCode: StatusCodes.Status404NotFound
                // )
            );
        }

        var newYardVehicle = new YardVehicle(
            status: yardVehicleDto.Status,
            enteredAt: enteredAt,
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
