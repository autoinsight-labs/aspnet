public class Yard
{
  public required string Id {get; set;}
  public required string AddressId {get; set;}
  public required Address Address {get; set;}
  public required string OwnerId {get; set;}
  public required List<YardEmployee> YardEmployees {get; set;}
  public required List<YardVehicle> YardVehicles{get; set;}
}
