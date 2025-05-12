public class Booking
{
  public required string Id { get; set; }
  public required DateTime OccursAt { get; set; }
  public DateTime? CancelledAt { get; set; }
  public required string VehicleId { get; set; }
  public required Vehicle Vehicle { get; set; }
  public required string YardId { get; set; }
  public required Yard Yard { get; set; }
}
