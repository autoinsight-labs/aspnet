namespace AutoInsightAPI.Dtos
{
  public class QrCodeDto
  {
    public string Id {get; private set;}
    public VehicleDto? Vehicle { get; set; }
  }
}
