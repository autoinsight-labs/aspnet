using AutoInsightAPI.Services;
using AutoMapper;

namespace AutoInsightAPI.handlers;

public record ResourceContext(
    IMapper Mapper,
    ILinkService LinkService,
    string ParentResourceName,
    string ChildResourceName
);
