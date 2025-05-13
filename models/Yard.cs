using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Yard
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public string Id {get; set;}

  public required string AddressId {get; set;}
  public required Address Address {get; set;}
  public required string OwnerId {get; set;}

  public List<YardEmployee> YardEmployees => new List<YardEmployee>();
  public List<YardVehicle> YardVehicles => new List<YardVehicle>();
}
