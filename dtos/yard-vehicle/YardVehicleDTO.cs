using AutoInsightAPI.Models;

namespace AutoInsightAPI.Dtos
{
  public class YardVehicleDTO
  {
    public string Id {get; private set;}
    public Status Status {get; set;}
    public DateTime EnteredAt {get; set;}
    public DateTime? LeftAt {get; set;}
    public VehicleDTO Vehicle {get; set;}
  }
}
