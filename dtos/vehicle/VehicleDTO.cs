using AutoInsightAPI.Dtos.Common;

namespace AutoInsightAPI.Dtos
{
  public class VehicleDto : HateoasResourceDto
  {
    public string Id {get; set;}
    public string Plate {get; set;}
    public ModelDto Model {get; set;}
    public string UserId {get; set;}
  }
}
