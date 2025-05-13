public class GetYardVehicleDTO
{
  public required string Id {get; set;}
  public required Status Status {get; set;}
  public required DateTime EnteredAt {get; set;}
  public DateTime? LeftAt {get; set;}
  public required Vehicle Vehicle {get; set;}
}
