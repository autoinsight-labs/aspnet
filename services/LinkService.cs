using System.Globalization;
using AutoInsightAPI.Dtos.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AutoInsightAPI.Services;

public class LinkService : ILinkService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;

    public LinkService(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
    {
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
    }

    public string GenerateLink(string routeName, object? routeValues = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new InvalidOperationException("HttpContext is not available");

        var link = _linkGenerator.GetUriByName(httpContext, routeName, routeValues);
        return link ?? string.Empty;
    }

    public string GenerateLink(string action, string controller, object? routeValues = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            throw new InvalidOperationException("HttpContext is not available");

        var link = _linkGenerator.GetUriByAction(httpContext, action, controller, routeValues);
        return link ?? string.Empty;
    }

    public List<LinkDto> GenerateResourceLinks(string resourceType, string resourceId, bool includeRelated = true)
    {
        var links = new List<LinkDto>();
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext == null) return links;

        var baseUrl = GetBaseUrl(httpContext);
        var resourcePath = resourceType.Split('/').Last();
        var resourceName = ToTitleCase(resourcePath.TrimEnd('s'));

        // Self link
        links.Add(new LinkDto
        {
            Href = $"{baseUrl}/{resourceType.ToLower()}/{resourceId}",
            Rel = "self",
            Method = "GET",
            Title = $"Get {resourceName} Details",
            Type = "application/json"
        });

        // Update link
        links.Add(new LinkDto
        {
            Href = $"{baseUrl}/{resourceType.ToLower()}/{resourceId}",
            Rel = "update",
            Method = "PATCH",
            Title = $"Update {resourceName}",
            Type = "application/json"
        });

        // Delete link
        links.Add(new LinkDto
        {
            Href = $"{baseUrl}/{resourceType.ToLower()}/{resourceId}",
            Rel = "delete",
            Method = "DELETE",
            Title = $"Delete {resourceName}",
            Type = "application/json"
        });

        // Related resources based on resource type
        if (includeRelated)
        {
            switch (resourceType.ToLower())
            {
                case "yard":
                    links.Add(new LinkDto
                    {
                        Href = $"{baseUrl}/yards/{resourceId}/employees",
                        Rel = "employees",
                        Method = "GET",
                        Title = "List Yard Employees",
                        Type = "application/json"
                    });
                    links.Add(new LinkDto
                    {
                        Href = $"{baseUrl}/yards/{resourceId}/vehicles",
                        Rel = "vehicles",
                        Method = "GET",
                        Title = "List Yard Vehicles",
                        Type = "application/json"
                    });
                    break;
                case "vehicle":
                    links.Add(new LinkDto
                    {
                        Href = $"{baseUrl}/vehicles?qrCodeId={resourceId}",
                        Rel = "qr-code",
                        Method = "GET",
                        Title = "Get Vehicle by QR Code",
                        Type = "application/json"
                    });
                    break;
            }
        }

        return links;
    }

    public List<LinkDto> GenerateCollectionLinks(string resourceType, int pageNumber, int pageSize, int totalPages)
    {
        var links = new List<LinkDto>();
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext == null) return links;

        var baseUrl = GetBaseUrl(httpContext);
        var resourcePath = resourceType.ToLower();
        var resourceName = ToTitleCase(resourceType.Split('/').Last());

        // Self link
        links.Add(new LinkDto
        {
            Href = $"{baseUrl}/{resourcePath}?pageNumber={pageNumber}&pageSize={pageSize}",
            Rel = "self",
            Method = "GET",
            Title = $"List {resourceName}",
            Type = "application/json"
        });

        // Create link
        links.Add(new LinkDto
        {
            Href = $"{baseUrl}/{resourcePath}",
            Rel = "create",
            Method = "POST",
            Title = $"Create New {resourceName}",
            Type = "application/json"
        });

        // Pagination links
        if (pageNumber > 1)
        {
            links.Add(new LinkDto
            {
                Href = $"{baseUrl}/{resourcePath}?pageNumber={pageNumber - 1}&pageSize={pageSize}",
                Rel = "prev",
                Method = "GET",
                Title = "Previous Page",
                Type = "application/json"
            });
        }

        if (pageNumber < totalPages)
        {
            links.Add(new LinkDto
            {
                Href = $"{baseUrl}/{resourcePath}?pageNumber={pageNumber + 1}&pageSize={pageSize}",
                Rel = "next",
                Method = "GET",
                Title = "Next Page",
                Type = "application/json"
            });
        }

        // First page
        if (pageNumber > 1)
        {
            links.Add(new LinkDto
            {
                Href = $"{baseUrl}/{resourcePath}?pageNumber=1&pageSize={pageSize}",
                Rel = "first",
                Method = "GET",
                Title = "First Page",
                Type = "application/json"
            });
        }

        // Last page
        if (pageNumber < totalPages)
        {
            links.Add(new LinkDto
            {
                Href = $"{baseUrl}/{resourcePath}?pageNumber={totalPages}&pageSize={pageSize}",
                Rel = "last",
                Method = "GET",
                Title = "Last Page",
                Type = "application/json"
            });
        }

        return links;
    }

    private static string GetBaseUrl(HttpContext httpContext)
    {
        var request = httpContext.Request;
        return $"{request.Scheme}://{request.Host}";
    }
    
    private static string ToTitleCase(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.Replace("-", " "));
    }
}
