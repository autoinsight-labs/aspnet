using AutoInsightAPI.Dtos.Common;

namespace AutoInsightAPI.Dtos
{
  public class VehicleDto : HateoasResourceDto, IIdentifiable
  {
    public string Id { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public ModelDto Model { get; set; } = new();
    public string UserId { get; set; } = string.Empty;
  }
}
