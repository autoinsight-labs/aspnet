using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
  public enum Status {
    SCHEDULED,
    WAITING,
    ON_SERVICE,
    FINISHED,
    CANCELLED
  }

  public class YardVehicle
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id {get; private set;}

    public Status Status {get; private set;}
    public DateTime? EnteredAt {get; private set;}
    public DateTime? LeftAt {get; private set;}
    public string VehicleId {get; private set;}
    public Vehicle Vehicle {get; private set;}
    public string YardId {get; private set;}
    public Yard Yard {get; private set;}

    public YardVehicle() { }

    public YardVehicle(Status status, DateTime? enteredAt, DateTime? leftAt, Vehicle vehicle, Yard yard)
    {
      this.Status = status;
      this.EnteredAt = enteredAt;
      this.LeftAt = leftAt;
      this.VehicleId = vehicle.Id;
      this.Vehicle = vehicle;
      this.YardId = yard.Id;
      this.Yard = yard;
    }

    public void Update(Status status, DateTime? enteredAt, DateTime? leftAt)
    {
      this.Status = status;
      this.EnteredAt = enteredAt;
      this.LeftAt = leftAt;
    }
  }
}
