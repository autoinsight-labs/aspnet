namespace AutoInsightAPI.Dtos
{
  public class CreateVehicleDto
  {
    public string Plate { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? ModelId { get; set; } = null;
    public CreateModelDto? Model { get; set; } = null;
  }
}
