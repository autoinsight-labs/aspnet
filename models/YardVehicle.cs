public enum Status {
  SCHEDULED,
  WAITING,
  ON_SERVICE,
  FINISHED,
  CANCELLED
}

public class YardVehicle
{
  public required string Id {get; set;}
  public required Status Status {get; set;}
  public required DateTime EnteredAt {get; set;}
  public DateTime? LeftAt {get; set;}
  public required string VehicleId {get; set;}
  public required Vehicle Vehicle {get; set;}
  public required string YardId {get; set;}
  public required Yard Yard {get; set;}
}
