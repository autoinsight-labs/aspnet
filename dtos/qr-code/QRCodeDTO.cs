namespace AutoInsightAPI.Dtos
{
  public class QrCodeDto
  {
    public string Id { get; private set; } = string.Empty;
    public VehicleDto? Vehicle { get; set; }
  }
}
