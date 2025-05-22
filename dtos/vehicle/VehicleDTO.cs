namespace AutoInsightAPI.Dtos
{
  public class VehicleDTO
  {
    public string Id {get; private set;}
    public string Plate {get; set;}
    public ModelDTO Model {get; set;}
    public string UserId {get; set;}
  }
}
