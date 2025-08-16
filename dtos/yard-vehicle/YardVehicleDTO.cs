using AutoInsightAPI.Models;

namespace AutoInsightAPI.Dtos
{
  public class YardVehicleDto
  {
    public string Id {get; private set;}
    public Status Status {get; set;}
    public DateTime? EnteredAt {get; set;}
    public DateTime? LeftAt {get; set;}
    public VehicleDto Vehicle {get; set;}
  }
}
