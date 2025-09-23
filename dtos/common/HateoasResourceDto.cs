namespace AutoInsightAPI.Dtos.Common;

public interface IIdentifiable
{
    string Id { get; }
}

public class HateoasResourceDto
{
    public List<LinkDto> Links { get; set; } = new();
}
