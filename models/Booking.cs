using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
  public class Booking
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; private set; }

    public DateTime OccursAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string VehicleId { get; private set; }
    public string YardId { get; private set; }

    public Booking() { }

    public Booking(string id, DateTime occursAt, DateTime? cancelledAt, string vehicleId, string yardId)
    {
      this.Id = id;
      this.OccursAt = occursAt;
      this.CancelledAt = cancelledAt;
      this.VehicleId = vehicleId;
      this.YardId = yardId;
    }
  }
}
