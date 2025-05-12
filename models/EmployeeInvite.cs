public class EmployeeInvite
{
  public required string Id { get; set; }
  public required string YardEmployeeId { get; set; }
  public required YardEmployee YardEmployee { get; set; }
  public required string YardId { get; set; }
  public required Yard Yard { get; set; }
}
