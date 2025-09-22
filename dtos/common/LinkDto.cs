namespace AutoInsightAPI.Dtos.Common;

public class LinkDto
{
    public string Href { get; set; } = string.Empty;
    public string Rel { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Type { get; set; }
}
