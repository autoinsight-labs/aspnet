using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Vehicle
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public required string Id {get; set;}

  public required string Plate {get; set;}
  public required string ModelId {get; set;}
  public required Model Model {get; set;}
  public required string UserId {get; set;}
}
