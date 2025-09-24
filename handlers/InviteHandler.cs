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

public static class InviteHandler
{
    public static void Map(WebApplication app)
    {
        var yardInviteGroup = app.MapGroup("/yards/{id}/invites").WithTags("yard", "invite")
            .WithDescription("Manage employee invites for yards: create and list invites.");

        yardInviteGroup.MapPost("/", CreateInvite)
            .WithSummary("Create employee invite")
            .WithDescription(@"Creates an invitation for a new employee to join the yard.
            
Example Request Body:
```json
{
    ""name"": ""Jane Doe"",
    ""email"": ""jane@example.com"",
    ""role"": ""MEMBER""
}
```")
            .Accepts<CreateYardEmployeeDto>("application/json")
            .Produces<EmployeeInviteDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .AddEndpointFilter<ValidationFilter<CreateYardEmployeeDto>>()
            .WithOpenApi(HandlerHelpers.BuildOpenApiOperation(
                "CreateEmployeeInvite",
                new Dictionary<string, (ParameterLocation, string)>
                {
                    { "id", (ParameterLocation.Path, "Yard identifier") }
                },
                ("Example payload to create an employee invite.", new OpenApiObject
                {
                    ["name"] = new OpenApiString("Jane Doe"),
                    ["email"] = new OpenApiString("jane@example.com"),
                    ["role"] = new OpenApiString("MEMBER")
                })
            ));

        yardInviteGroup.MapGet("/", GetYardInvites)
            .WithSummary("List yard invites")
            .WithDescription(@"Retrieves a paginated list of invites for a specific yard.
            
Query Parameters:
- pageNumber (optional): Page number to retrieve. Default: 1
- pageSize (optional): Number of items per page. Default: 10")
            .Produces<Dtos.PagedResponseDto<EmployeeInviteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.OperationId = "ListYardInvites";
                return op;
            });

        // Rotas globais para convites
        var inviteGroup = app.MapGroup("/invites").WithTags("invite")
            .WithDescription("Global invite operations: accept, reject, and list user invites.");

        inviteGroup.MapPost("/{token}/accept", AcceptInvite)
            .WithSummary("Accept invite")
            .WithDescription(@"Accepts an employee invitation and creates the yard employee.
            
Example Request Body:
```json
{
    ""userId"": ""usr_123"",
    ""imageUrl"": ""https://example.com/photo.jpg""
}
```")
            .Accepts<AcceptInviteDto>("application/json")
            .Produces<YardEmployeeDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .AddEndpointFilter<ValidationFilter<AcceptInviteDto>>()
            .WithOpenApi(HandlerHelpers.BuildOpenApiOperation(
                "AcceptInvite",
                new Dictionary<string, (ParameterLocation, string)>
                {
                    { "token", (ParameterLocation.Path, "Invite token") }
                },
                ("Example payload to accept an invite.", new OpenApiObject
                {
                    ["userId"] = new OpenApiString("usr_123"),
                    ["imageUrl"] = new OpenApiString("https://example.com/photo.jpg")
                })
            ));

        inviteGroup.MapPost("/{token}/reject", RejectInvite)
            .WithSummary("Reject invite")
            .WithDescription("Rejects an employee invitation.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOpenApi(op =>
            {
                op.OperationId = "RejectInvite";
                return op;
            });

        inviteGroup.MapGet("/user/{userId}", GetUserInviteHistory)
            .WithSummary("List user invite history")
            .WithDescription(@"Retrieves a paginated list of invites that were ACCEPTED by a specific user.
            
This endpoint shows the history of invitations that the user has accepted in the past.
For pending invites, use the email-based endpoint instead.
            
Query Parameters:
- pageNumber (optional): Page number to retrieve. Default: 1
- pageSize (optional): Number of items per page. Default: 10")
            .Produces<Dtos.PagedResponseDto<EmployeeInviteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(op =>
            {
                op.OperationId = "ListUserInviteHistory";
                return op;
            });

        inviteGroup.MapGet("/email/{email}", GetEmailInvites)
            .WithSummary("List pending invites by email")
            .WithDescription(@"Retrieves a paginated list of PENDING invites for a specific email address.
            
This endpoint shows invitations that are waiting for the email owner to accept or reject.
For accepted invites history, use the user-based endpoint instead.
            
Query Parameters:
- pageNumber (optional): Page number to retrieve. Default: 1
- pageSize (optional): Number of items per page. Default: 10")
            .Produces<Dtos.PagedResponseDto<EmployeeInviteDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi(op =>
            {
                op.OperationId = "ListEmailInvites";
                return op;
            });
    }

    private static async Task<Results<Created<EmployeeInviteDto>, BadRequest, NotFound, Conflict>> CreateInvite(
        IYardRepository yardRepository,
        IEmployeeInviteRepository inviteRepository,
        IMapper mapper,
        ILinkService linkService,
        string id,
        CreateYardEmployeeDto createYardEmployeeDto
    )
    {
        var yard = await yardRepository.FindAsync(id);
        if (yard is null)
            return TypedResults.NotFound();

        var existingInvite = await inviteRepository.FindByEmailAndYardAsync(createYardEmployeeDto.Email, id);
        if (existingInvite is not null)
            return TypedResults.Conflict();

        var token = Guid.NewGuid().ToString("N");

        var newInvite = new EmployeeInvite(
            email: createYardEmployeeDto.Email,
            name: createYardEmployeeDto.Name,
            role: createYardEmployeeDto.Role,
            token: token,
            yard: yard
        );

        var createdInvite = await inviteRepository.CreateAsync(newInvite);
        var inviteDto = mapper.Map<EmployeeInviteDto>(createdInvite);

        inviteDto.Links = linkService.GenerateResourceLinks($"yards/{id}/invites", createdInvite.Id);

        return TypedResults.Created($"/yards/{id}/invites/{createdInvite.Id}", inviteDto);
    }

    private static async Task<Results<Ok<PagedResponseDto<EmployeeInviteDto>>, BadRequest, NotFound>> GetYardInvites(
        IYardRepository yardRepository,
        IEmployeeInviteRepository inviteRepository,
        IMapper mapper,
        ILinkService linkService,
        string id,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return TypedResults.BadRequest();

        var yard = await yardRepository.FindAsync(id);
        if (yard is null)
            return TypedResults.NotFound();

        var pagedInvites = await inviteRepository.ListByYardAsync(pageNumber, pageSize, id);
        var inviteDtos = mapper.Map<List<EmployeeInviteDto>>(pagedInvites.Data);

        foreach (var inviteDto in inviteDtos)
        {
            inviteDto.Links = linkService.GenerateResourceLinks($"yards/{id}/invites", inviteDto.Id);
        }

        var pagedResponse = new PagedResponseDto<EmployeeInviteDto>
        {
            Data = inviteDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = pagedInvites.TotalRecords,
            TotalPages = (int)Math.Ceiling((double)pagedInvites.TotalRecords / pageSize)
        };

        return TypedResults.Ok(pagedResponse);
    }

    private static async Task<Results<Ok<YardEmployeeDto>, BadRequest, NotFound, Conflict>> AcceptInvite(
        IEmployeeInviteRepository inviteRepository,
        IYardEmployeeRepository employeeRepository,
        IMapper mapper,
        ILinkService linkService,
        string token,
        AcceptInviteDto acceptInviteDto
    )
    {
        var invite = await inviteRepository.FindByTokenAsync(token);
        if (invite is null)
            return TypedResults.NotFound();

        if (invite.Status != InviteStatus.PENDING)
            return TypedResults.Conflict();

        // Aceitar o convite
        invite.Accept(acceptInviteDto.UserId);

        // Criar o YardEmployee
        var yardEmployee = new YardEmployee(
            name: invite.Name,
            imageUrl: acceptInviteDto.ImageUrl,
            role: invite.Role,
            userId: acceptInviteDto.UserId,
            yard: invite.Yard
        );

        var createdEmployee = await employeeRepository.CreateAsync(yardEmployee);
        await inviteRepository.UpdateAsync();

        var employeeDto = mapper.Map<YardEmployeeDto>(createdEmployee);
        employeeDto.Links = linkService.GenerateResourceLinks($"yards/{invite.YardId}/employees", createdEmployee.Id);

        return TypedResults.Ok(employeeDto);
    }

    private static async Task<Results<NoContent, NotFound, Conflict>> RejectInvite(
        IEmployeeInviteRepository inviteRepository,
        string token
    )
    {
        var invite = await inviteRepository.FindByTokenAsync(token);
        if (invite is null)
            return TypedResults.NotFound();

        if (invite.Status != InviteStatus.PENDING)
            return TypedResults.Conflict();

        invite.Reject();
        await inviteRepository.UpdateAsync();

        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<PagedResponseDto<EmployeeInviteDto>>, BadRequest>> GetUserInviteHistory(
        IEmployeeInviteRepository inviteRepository,
        IMapper mapper,
        ILinkService linkService,
        string userId,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return TypedResults.BadRequest();

        var pagedInvites = await inviteRepository.ListByUserAsync(pageNumber, pageSize, userId);
        var pagedResponse = MapInvitesToPagedResponse(pagedInvites, mapper, linkService, pageNumber, pageSize);

        return TypedResults.Ok(pagedResponse);
    }

    private static async Task<Results<Ok<PagedResponseDto<EmployeeInviteDto>>, BadRequest>> GetEmailInvites(
        IEmployeeInviteRepository inviteRepository,
        IMapper mapper,
        ILinkService linkService,
        string email,
        int pageNumber = 1,
        int pageSize = 10
    )
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return TypedResults.BadRequest();

        var pagedInvites = await inviteRepository.ListByEmailAsync(pageNumber, pageSize, email);
        var pagedResponse = MapInvitesToPagedResponse(pagedInvites, mapper, linkService, pageNumber, pageSize);

        return TypedResults.Ok(pagedResponse);
    }

    private static PagedResponseDto<EmployeeInviteDto> MapInvitesToPagedResponse(
        PagedResponse<EmployeeInvite> pagedInvites,
        IMapper mapper,
        ILinkService linkService,
        int pageNumber,
        int pageSize)
    {
        var inviteDtos = mapper.Map<List<EmployeeInviteDto>>(pagedInvites.Data);

        foreach (var inviteDto in inviteDtos)
        {
            inviteDto.Links = linkService.GenerateResourceLinks($"yards/{inviteDto.YardId}/invites", inviteDto.Id);
        }

        return new PagedResponseDto<EmployeeInviteDto>
        {
            Data = inviteDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = pagedInvites.TotalRecords,
            TotalPages = (int)Math.Ceiling((double)pagedInvites.TotalRecords / pageSize)
        };
    }
}
