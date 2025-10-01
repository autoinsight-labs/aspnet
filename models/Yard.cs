using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
  public class Yard
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id {get; private set;}

    public string AddressId {get; private set;}
    public Address Address {get; private set;}
    public string OwnerId {get; private set;}

    public List<YardEmployee> YardEmployees {get; private set; } = new List<YardEmployee>();
    public List<YardVehicle> YardVehicles {get; private set; } = new List<YardVehicle>();

    public Yard() { }

    public Yard(Address address, string ownerId)
    {
      this.AddressId = address.Id;
      this.Address = address;
      this.OwnerId = ownerId;
    }

    public void Update(Address address, string ownerId)
    {
      this.AddressId = address.Id;
      this.Address = address;
      this.OwnerId = ownerId;
    }
  }
}
