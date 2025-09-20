namespace AutoInsightAPI.Dtos.Common;

public abstract class HateoasResourceDto
{
    public List<LinkDto> Links { get; set; } = new();
}
