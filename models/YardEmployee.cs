using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum EmployeeRole {
  ADMIN,
  MEMBER
}

public class YardEmployee
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public required string Id {get; set;}

  public required EmployeeRole Role {get; set;}
  public required string UserId {get;set;}
  public required string YardId {get; set;}
  public required Yard Yard {get; set;}
}
