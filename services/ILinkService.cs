using AutoInsightAPI.Dtos.Common;

namespace AutoInsightAPI.Services;

public interface ILinkService
{
    string GenerateLink(string routeName, object? routeValues = null);
    string GenerateLink(string action, string controller, object? routeValues = null);
    List<LinkDto> GenerateResourceLinks(string resourceType, string resourceId, bool includeRelated = true);
    List<LinkDto> GenerateCollectionLinks(string resourceType, int pageNumber, int pageSize, int totalPages);
}
