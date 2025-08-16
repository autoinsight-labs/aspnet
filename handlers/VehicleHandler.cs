using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.Repositories;
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
        yardVehicleGroup.MapPut("/{yardVehicleId}", UpdateYardVehicle);
        yardVehicleGroup.MapPost("/", CreateYardVehicle);
    }

    private static async Task<Results<Ok<VehicleDto>, NotFound>> GetVehicleByQrCode(
        string qrCodeId,
        IVehicleRepository vehicleRepository,
        IMapper mapper
    )
    {
        var vehicle = await vehicleRepository.FindAsyncByQRCode(qrCodeId);

        if (vehicle is null)
        {
            return TypedResults.NotFound();
        }


        var vehicleResponse = mapper.Map<VehicleDto>(vehicle);
        return TypedResults.Ok(vehicleResponse);
    }

    private static async Task<Results<Ok<VehicleDto>, NotFound>> GetVehicleById(
        string id,
        IVehicleRepository vehicleRepository,
        IMapper mapper
    )
    {
        var vehicle = await vehicleRepository.FindAsyncById(id);

        if (vehicle is null)
        {
            return TypedResults.NotFound();
        }


        var vehicleResponse = mapper.Map<VehicleDto>(vehicle);
        return TypedResults.Ok(vehicleResponse);
    }

    private static async Task<Results<Ok<PagedResponseDto<YardVehicleDto>>, BadRequest, NotFound>> GetYardVehicles(
        IYardRepository yardRepository,
        IYardVehicleRepository yardVehicleRepository,
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

        var yardVehicles = await yardVehicleRepository.ListPagedAsync(pageNumber, pageSize, yard);

        return TypedResults.Ok(mapper.Map<PagedResponseDto<YardVehicleDto>>(yardVehicles));
    }

    private static async Task<Results<Ok<YardVehicleDto>, BadRequest, NotFound>> GetYardVehicleById(
        IYardRepository yardRepository,
        IYardVehicleRepository yardVehicleRepository,
        IMapper mapper,
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

        return TypedResults.Ok(yardVehicleResult);
    }

    private static async Task<Results<Ok<YardVehicleDto>, BadRequest, NotFound>> UpdateYardVehicle(
        IYardRepository yardRepository,
        IYardVehicleRepository yardVehicleRepository,
        IMapper mapper,
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

        var newYardEmployee = mapper.Map<YardVehicleDto>(yardVehicle);

        return TypedResults.Ok(newYardEmployee);
    }

    private static async Task<Results<Created<YardVehicleDto>, BadRequest, NotFound>> CreateYardVehicle(
        IYardRepository yardRepository,
        IYardVehicleRepository yardVehicleRepository,
        IVehicleRepository vehicleRepository,
        IMapper mapper,
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

        return TypedResults.Created($"/yard/{createdYardVehicle.YardId}/vehicles/{createdYardVehicle.Id}",
            yardVehicleDtoResult);
    }
}