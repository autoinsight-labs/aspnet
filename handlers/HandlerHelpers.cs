using AutoInsightAPI.Dtos;
using AutoInsightAPI.Dtos.Common;
using AutoInsightAPI.Models;
using AutoInsightAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace AutoInsightAPI.handlers;

public static class HandlerHelpers
{
    public static BadRequest? ValidatePaginationParameters(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return TypedResults.BadRequest();
        }
        return null;
    }

    public static async Task<Results<Ok<PagedResponseDto<TDto>>, BadRequest>> HandlePagedList<TEntity, TDto>(
        int pageNumber,
        int pageSize,
        Func<int, int, Task<PagedResponse<TEntity>>> repositoryCall,
        IMapper mapper,
        ILinkService linkService,
        string resourceName,
        string? parentResource = null)
        where TDto : HateoasResourceDto, IIdentifiable
    {
        var validationResult = ValidatePaginationParameters(pageNumber, pageSize);
        if (validationResult != null)
            return validationResult;

        var entities = await repositoryCall(pageNumber, pageSize);
        var response = mapper.Map<PagedResponseDto<TDto>>(entities);

        var resourcePath = parentResource != null ? $"{parentResource}/{resourceName}" : resourceName;
        response.Links = linkService.GenerateCollectionLinks(resourcePath, pageNumber, pageSize, entities.TotalPages);

        foreach (var item in response.Data)
        {
            item.Links = linkService.GenerateResourceLinks(resourcePath, item.Id);
        }

        return TypedResults.Ok(response);
    }

    public static async Task<Results<Ok<PagedResponseDto<TDto>>, BadRequest, NotFound>> HandlePagedListWithParent<TParent, TEntity, TDto>(
        string parentId,
        int pageNumber,
        int pageSize,
        Func<string, Task<TParent?>> parentRepositoryCall,
        Func<int, int, TParent, Task<PagedResponse<TEntity>>> childRepositoryCall,
        ResourceContext context)
        where TDto : HateoasResourceDto, IIdentifiable
    {
        var validationResult = ValidatePaginationParameters(pageNumber, pageSize);
        if (validationResult != null)
            return validationResult;

        var parent = await parentRepositoryCall(parentId);
        if (parent is null)
            return TypedResults.NotFound();

        var entities = await childRepositoryCall(pageNumber, pageSize, parent);
        var response = context.Mapper.Map<PagedResponseDto<TDto>>(entities);

        var resourcePath = $"{context.ParentResourceName}/{parentId}/{context.ChildResourceName}";
        response.Links = context.LinkService.GenerateCollectionLinks(resourcePath, pageNumber, pageSize, entities.TotalPages);

        foreach (var item in response.Data)
        {
            item.Links = context.LinkService.GenerateResourceLinks(resourcePath, item.Id);
        }

        return TypedResults.Ok(response);
    }

    public static async Task<Results<Ok<TDto>, NotFound>> HandleGetById<TEntity, TDto>(
        string id,
        Func<string, Task<TEntity?>> repositoryCall,
        IMapper mapper,
        ILinkService linkService,
        string resourceName)
        where TDto : HateoasResourceDto, IIdentifiable
    {
        var entity = await repositoryCall(id);
        if (entity is null)
            return TypedResults.NotFound();

        var response = mapper.Map<TDto>(entity);
        response.Links = linkService.GenerateResourceLinks(resourceName, id);

        return TypedResults.Ok(response);
    }

    public static async Task<Results<Ok<TDto>, NotFound>> HandleGetChildById<TParent, TEntity, TDto>(
        string parentId,
        string childId,
        Func<string, Task<TParent?>> parentRepositoryCall,
        Func<string, Task<TEntity?>> childRepositoryCall,
        ResourceContext context)
        where TDto : HateoasResourceDto, IIdentifiable
    {
        var parent = await parentRepositoryCall(parentId);
        if (parent is null)
            return TypedResults.NotFound();

        var entity = await childRepositoryCall(childId);
        if (entity is null)
            return TypedResults.NotFound();

        var response = context.Mapper.Map<TDto>(entity);
        response.Links = context.LinkService.GenerateResourceLinks($"{context.ParentResourceName}/{parentId}/{context.ChildResourceName}", childId);

        return TypedResults.Ok(response);
    }

    public static async Task<Results<Created<TDto>, BadRequest>> HandleCreate<TEntity, TDto>(
        TDto dto,
        Func<TEntity, Task<TEntity>> repositoryCall,
        IMapper mapper,
        ILinkService linkService,
        string resourceName)
        where TDto : HateoasResourceDto, IIdentifiable
    {
        var entity = mapper.Map<TEntity>(dto);
        var createdEntity = await repositoryCall(entity);
        var response = mapper.Map<TDto>(createdEntity);

        response.Links = linkService.GenerateResourceLinks(resourceName, response.Id);

        return TypedResults.Created($"/{resourceName}/{response.Id}", response);
    }

    public static async Task<Results<Ok<TDto>, NotFound>> HandleUpdate<TEntity, TDto>(
        string id,
        TDto dto,
        Func<string, Task<TEntity?>> findRepositoryCall,
        Func<Task> updateRepositoryCall,
        IMapper mapper,
        ILinkService linkService,
        string resourceName)
        where TDto : HateoasResourceDto, IIdentifiable
    {
        var entity = await findRepositoryCall(id);
        if (entity is null)
            return TypedResults.NotFound();

        mapper.Map(dto, entity);
        await updateRepositoryCall();

        var response = mapper.Map<TDto>(entity);
        response.Links = linkService.GenerateResourceLinks(resourceName, id);

        return TypedResults.Ok(response);
    }

    public static async Task<Results<Ok<TDto>, NotFound>> HandleUpdateChild<TParent, TEntity, TDto>(
        string parentId,
        string childId,
        TDto dto,
        Func<string, Task<TParent?>> parentRepositoryCall,
        Func<string, Task<TEntity?>> childRepositoryCall,
        Func<Task> updateRepositoryCall,
        ResourceContext context)
        where TDto : HateoasResourceDto, IIdentifiable
    {
        var parent = await parentRepositoryCall(parentId);
        if (parent is null)
            return TypedResults.NotFound();

        var entity = await childRepositoryCall(childId);
        if (entity is null)
            return TypedResults.NotFound();

        context.Mapper.Map(dto, entity);
        await updateRepositoryCall();

        var response = context.Mapper.Map<TDto>(entity);
        response.Links = context.LinkService.GenerateResourceLinks($"{context.ParentResourceName}/{parentId}/{context.ChildResourceName}", childId);

        return TypedResults.Ok(response);
    }

    public static async Task<Results<NoContent, NotFound>> HandleDelete<TEntity>(
        string id,
        Func<string, Task<TEntity?>> findRepositoryCall,
        Func<TEntity, Task> deleteRepositoryCall)
    {
        var entity = await findRepositoryCall(id);
        if (entity is null)
            return TypedResults.NotFound();

        await deleteRepositoryCall(entity);
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> HandleDeleteChild<TParent, TEntity>(
        string parentId,
        string childId,
        Func<string, Task<TParent?>> parentRepositoryCall,
        Func<string, Task<TEntity?>> childRepositoryCall,
        Func<TEntity, Task> deleteRepositoryCall)
    {
        var parent = await parentRepositoryCall(parentId);
        if (parent is null)
            return TypedResults.NotFound();

        var entity = await childRepositoryCall(childId);
        if (entity is null)
            return TypedResults.NotFound();

        await deleteRepositoryCall(entity);
        return TypedResults.NoContent();
    }

    public static Func<OpenApiOperation, OpenApiOperation> BuildOpenApiOperation(
        string operationId,
        Dictionary<string, (ParameterLocation, string)>? parameters = null,
        (string, OpenApiObject)? requestBody = null)
    {
        return op =>
        {
            op.OperationId = operationId;

            if (parameters is not null)
            {
                foreach (var (name, (location, description)) in parameters)
                {
                    if (op.Parameters.All(p => p.Name != name))
                    {
                        op.Parameters.Add(new OpenApiParameter
                        {
                            Name = name,
                            In = location,
                            Description = description,
                            Required = true
                        });
                    }
                }
            }

            if (requestBody is not null)
            {
                op.RequestBody = new OpenApiRequestBody
                {
                    Description = requestBody.Value.Item1,
                    Required = true,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new() { Example = requestBody.Value.Item2 }
                    }
                };
            }

            return op;
        };
    }
}
