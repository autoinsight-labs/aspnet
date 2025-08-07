using Microsoft.AspNetCore.Http.HttpResults;
using AutoMapper;
using AutoInsightAPI.Repositories;
using AutoInsightAPI.Dtos;
using AutoInsightAPI.Models;
using AutoInsightAPI.configs;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

ServicesConfigurator.Configure(builder.Services);

var app = builder.Build();

MiddlewareConfigurator.Configure(app);

app.MapGet("/health", () => Results.Ok())
    .WithSummary("Health Check")
    .WithDescription("Always returns 200 when the application is running.");

// Yard Routes
var yardGroup = app.MapGroup("/yards").WithTags("yard");

yardGroup.MapGet("/", async Task<Results<Ok<PagedResponseDTO<YardDTO>>, BadRequest>> (
    IYardRepository yardRepository,
    IMapper mapper,
    int pageNumber = 1,
    int pageSize = 10
) => {
    if (pageNumber <= 0) {
        return TypedResults.BadRequest(
            // TypedResults.Problem(
            //     title: "Bad Request",
            //     detail: $"{nameof(pageNumber)} must be greater than 0",
            //     statusCode: StatusCodes.Status400BadRequest
            // )
        );
    }

    if (pageSize <= 0) {
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

yardGroup.MapGet("/{id}", async Task<Results<Ok<YardDTO>, NotFound>> (string id, IYardRepository yardRepository, IMapper mapper) => {
    var yard = await yardRepository.FindAsync(id);

    if (yard is null) {
        return TypedResults.NotFound();
    }

    var yardResponse = mapper.Map<YardDTO>(yard);
    return TypedResults.Ok(yardResponse);
});

yardGroup.MapPost("/", async Task<Results<Created<YardDTO>, ProblemHttpResult>> (YardDTO yardDTO, IYardRepository yardRepository, IMapper mapper) => {
    try
    {
        var createdYard = await yardRepository.CreateAsync(mapper.Map<Yard>(yardDTO));

        var yardDTOResult = mapper.Map<YardDTO>(createdYard);
        return TypedResults.Created($"/yard/{createdYard.Id}", yardDTOResult);
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

yardGroup.MapDelete("/{id}", async Task<Results<NoContent, NotFound>> (string id, IYardRepository yardRepository) => {
    var existingYard = await yardRepository.FindAsync(id);

    if (existingYard is null) {
        return TypedResults.NotFound();
    }

   await yardRepository.DeleteAsync(existingYard);

   return TypedResults.NoContent();
});

yardGroup.MapPut("/{id}", async Task<Results<Ok<YardDTO>, NotFound>> (string id, YardDTO yardDTO, IYardRepository yardRepository, IMapper mapper) => {
    var existingYard = await yardRepository.FindAsync(id);

    if (existingYard is null) {
        return TypedResults.NotFound();
    }

    mapper.Map(yardDTO, existingYard);

    await yardRepository.UpdateAsync();

    var newYard = mapper.Map<YardDTO>(existingYard);
    return TypedResults.Ok(newYard);
});

// Employee routes

yardGroup.MapGet("/{id}/employees", async Task<Results<Ok<PagedResponseDTO<YardEmployeeDTO>>, BadRequest, NotFound>> (
    IYardRepository yardRepository,
    IYardEmployeeRepository yardEmployeeRepository,
    IMapper mapper,
    string id,
    int pageNumber = 1,
    int pageSize = 10
) => {
    if (pageNumber <= 0) {
        return TypedResults.BadRequest(
            // TypedResults.Problem(
            //     title: "Bad Request",
            //     detail: $"{nameof(pageNumber)} must be greater than 0",
            //     statusCode: StatusCodes.Status400BadRequest
            // )
        );
    }

    if (pageSize <= 0) {
        return TypedResults.BadRequest(
            // TypedResults.Problem(
            //     title: "Bad Request",
            //     detail: $"{nameof(pageSize)} must be greater than 0",
            //     statusCode: StatusCodes.Status400BadRequest
            // )
        );
    }

    var yard = await yardRepository.FindAsync(id);

    if (yard is null) {
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

yardGroup.MapPost("/{id}/employees", async Task<Results<Created<YardEmployeeDTO>, NotFound>> (
    IYardRepository yardRepository,
    IYardEmployeeRepository yardEmployeeRepository,
    IMapper mapper,
    string id,
    YardEmployeeDTO yardEmployeeDTO
) => {
    var yard = await yardRepository.FindAsync(id);

    if (yard is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard with id '{id}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }

    var newEmployee = new YardEmployee(
        name: yardEmployeeDTO.Name,
        imageUrl: yardEmployeeDTO.ImageUrl,
        role: yardEmployeeDTO.Role,
        userId: yardEmployeeDTO.UserId,
        yard: yard
    );

    var createdYardEmployee = await yardEmployeeRepository.CreateAsync(newEmployee);
    var yardEmployeeDTOResult = mapper.Map<YardEmployeeDTO>(createdYardEmployee);

    return TypedResults.Created($"/yard/{createdYardEmployee.YardId}/employees/{createdYardEmployee.Id}", yardEmployeeDTOResult);
}).WithTags("employee");

yardGroup.MapGet("/{id}/employees/{employeeId}", async Task<Results<Ok<YardEmployeeDTO>, NotFound>> (
    IYardRepository yardRepository,
    IYardEmployeeRepository yardEmployeeRepository,
    IMapper mapper,
    string id, string employeeId
) => {
    var yard = await yardRepository.FindAsync(id);

    if (yard is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard with id '{id}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }

    var yardEmployee = await yardEmployeeRepository.FindAsync(employeeId);

    if (yardEmployee is null) {
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

yardGroup.MapDelete("/{id}/employees/{employeeId}", async Task<Results<NoContent, NotFound>> (
    IYardRepository yardRepository,
    IYardEmployeeRepository yardEmployeeRepository,
    IMapper mapper,
    string id,
    string employeeId
) => {
    var yard = await yardRepository.FindAsync(id);

    if (yard is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard with id '{id}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }

    var yardEmployee = await yardEmployeeRepository.FindAsync(employeeId);

    if (yardEmployee is null) {
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

yardGroup.MapPut("/{id}/employees/{employeeId}", async Task<Results<Ok<YardEmployeeDTO>, BadRequest , NotFound>> (
    IYardRepository yardRepository,
    IYardEmployeeRepository yardEmployeeRepository,
    IMapper mapper,
    YardEmployeeDTO yardEmployeeDTO,
    string id,
    string employeeId
) => {
    if (!Enum.IsDefined(typeof(EmployeeRole), yardEmployeeDTO.Role))
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

    if (yard is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard with id '{id}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }

    var yardEmployee = await yardEmployeeRepository.FindAsync(employeeId);

    if (yardEmployee is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard employee with id '{employeeId}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }

    mapper.Map(yardEmployeeDTO, yardEmployee);
    await yardEmployeeRepository.UpdateAsync();

    var newYardEmployee = mapper.Map<YardEmployeeDTO>(yardEmployee);

    return TypedResults.Ok(newYardEmployee);
}).WithTags("employee");

// Vehicle Routes

var vehicleGroup = app.MapGroup("/vehicles").WithTags("vehicle");

vehicleGroup.MapGet("/", async Task<Results<Ok<VehicleDTO>, NotFound>> (string qrCodeId, IVehicleRepository vehicleRepository, IMapper mapper) => {
    var vehicle = await vehicleRepository.FindAsyncByQRCode(qrCodeId);

    if (vehicle is null) {
        return TypedResults.NotFound();
    }


    var vehicleResponse = mapper.Map<VehicleDTO>(vehicle);
    return TypedResults.Ok(vehicleResponse);
});

vehicleGroup.MapGet("/{id}", async Task<Results<Ok<VehicleDTO>, NotFound>> (string id, IVehicleRepository vehicleRepository, IMapper mapper) => {
    var vehicle = await vehicleRepository.FindAsyncById(id);

    if (vehicle is null) {
        return TypedResults.NotFound();
    }


    var vehicleResponse = mapper.Map<VehicleDTO>(vehicle);
    return TypedResults.Ok(vehicleResponse);
});

yardGroup.MapGet("/{id}/vehicles", async Task<Results<Ok<PagedResponseDTO<YardVehicleDTO>>, BadRequest, NotFound>> (
    IYardRepository yardRepository,
    IYardVehicleRepository yardVehicleRepository,
    IMapper mapper,
    string id,
    int pageNumber = 1,
    int pageSize = 10
) => {
    if (pageNumber <= 0) {
        return TypedResults.BadRequest(
            // TypedResults.Problem(
            //     title: "Bad Request",
            //     detail: $"{nameof(pageNumber)} must be greater than 0",
            //     statusCode: StatusCodes.Status400BadRequest
            // )
        );
    }

    if (pageSize <= 0) {
        return TypedResults.BadRequest(
            // TypedResults.Problem(
            //     title: "Bad Request",
            //     detail: $"{nameof(pageSize)} must be greater than 0",
            //     statusCode: StatusCodes.Status400BadRequest
            // )
        );
    }

    var yard = await yardRepository.FindAsync(id);

    if (yard is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard with id '{id}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }

    var yardVehicles = await yardVehicleRepository.ListPagedAsync(pageNumber, pageSize, yard);

    return TypedResults.Ok(mapper.Map<PagedResponseDTO<YardVehicleDTO>>(yardVehicles));
}).WithTags("vehicle");

yardGroup.MapGet("/{id}/vehicles/{yardVehicleId}", async Task<Results<Ok<YardVehicleDTO>, BadRequest, NotFound>> (
    IYardRepository yardRepository,
    IYardVehicleRepository yardVehicleRepository,
    IMapper mapper,
    string id,
    string yardVehicleId
) => {
    var yard = await yardRepository.FindAsync(id);

    if (yard is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard with id '{id}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }

    var yardVehicle = await yardVehicleRepository.FindAsync(yardVehicleId);

    if (yardVehicle is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard vehicle with id '{yardVehicleId}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }

    var yardVehicleResult = mapper.Map<YardVehicleDTO>(yardVehicle);

    return TypedResults.Ok(yardVehicleResult);
}).WithTags("vehicle");

yardGroup.MapPut("/{id}/vehicles/{yardVehicleId}", async Task<Results<Ok<YardVehicleDTO>, BadRequest, NotFound>> (
    IYardRepository yardRepository,
    IYardVehicleRepository yardVehicleRepository,
    IMapper mapper,
    string id,
    string yardVehicleId,
    YardVehicleDTO yardVehicleDTO
) => {
    if (!Enum.IsDefined(typeof(Status), yardVehicleDTO.Status))
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

    if (yard is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard with id '{id}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }

    var yardVehicle = await yardVehicleRepository.FindAsync(yardVehicleId);

    if (yardVehicle is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard vehicle with id '{yardVehicleId}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }


    mapper.Map(yardVehicleDTO, yardVehicle);
    await yardVehicleRepository.UpdateAsync();

    var newYardEmployee = mapper.Map<YardVehicleDTO>(yardVehicle);

    return TypedResults.Ok(newYardEmployee);
}).WithTags("vehicle");

yardGroup.MapPost("/{id}/vehicles", async Task<Results<Created<YardVehicleDTO>, BadRequest, NotFound>> (
    IYardRepository yardRepository,
    IYardVehicleRepository yardVehicleRepository,
    IVehicleRepository vehicleRepository,
    IMapper mapper,
    string id,
    YardVehicleDTO yardVehicleDTO
) => {
    if (!Enum.IsDefined(typeof(Status), yardVehicleDTO.Status))
    {
        return TypedResults.BadRequest(
            // TypedResults.Problem(
            //     title: "Invalid Role",
            //     detail: $"The role value '{yardVehicleDTO.Status}' is not valid.",
            //     statusCode: StatusCodes.Status400BadRequest
            // )
        );
    }

    if (yardVehicleDTO.EnteredAt is not DateTime enteredAt)
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

    if (yard is null) {
        return TypedResults.NotFound(
            // TypedResults.Problem(
            //     title: "Not Found",
            //     detail: $"Yard with id '{id}' not found.",
            //     statusCode: StatusCodes.Status404NotFound
            // )
        );
    }

    var vehicle = await vehicleRepository.FindAsyncById(yardVehicleDTO.Vehicle.Id);

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
        status: yardVehicleDTO.Status,
        enteredAt: enteredAt,
        leftAt: yardVehicleDTO.LeftAt,
        vehicle: vehicle,
        yard: yard
    );

    var createdYardVehicle = await yardVehicleRepository.CreateAsync(newYardVehicle);
    var yardVehicleDTOResult = mapper.Map<YardVehicleDTO>(createdYardVehicle);

    return TypedResults.Created($"/yard/{createdYardVehicle.YardId}/vehicles/{createdYardVehicle.Id}", yardVehicleDTOResult);
}).WithTags("vehicle");

app.Run();
