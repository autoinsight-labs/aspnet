using AutoInsightAPI.Models;

namespace AutoInsightAPI.Dtos
{
  public class CreateYardVehicleDto
  {
    public Status Status { get; set; }
    public DateTime? EnteredAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public string? VehicleId { get; set; } = null;
    public CreateVehicleDto? Vehicle { get; set; } = null;
  }
}
