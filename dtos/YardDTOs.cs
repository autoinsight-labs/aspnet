public class CreateYardDTO
{
  public required Address Address {get; set;}
  public required string OwnerId {get; set;}
}

public class GetYardDTO
{
  public required string Id {get; set;}

  public required Address Address {get; set;}
  public required string OwnerId {get; set;}

  public required List<GetYardEmployeeDTO> YardEmployees {get; set;}
  public required List<GetYardVehicleDTO> YardVehicles {get; set;}
}
