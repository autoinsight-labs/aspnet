using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EmployeeInvite
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public required string Id { get; set; }

  public required string YardEmployeeId { get; set; }
  public required YardEmployee YardEmployee { get; set; }
  public required string YardId { get; set; }
  public required Yard Yard { get; set; }
}
