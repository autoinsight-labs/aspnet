using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
  public class Address
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; private set; }

    public string Country { get; private set; }
    public string State { get; private set; }
    public string City { get; private set; }
    public string ZipCode { get; private set; }
    public string Neighborhood { get; private set; }
    public string? Complement { get; private set; }

    public Address() { }

    public Address(string id, string country, string state, string city, string zipCode, string neighborhood, string? complement)
    {
      this.Id = id;
      this.Country = country;
      this.State = state;
      this.City = city;
      this.ZipCode = zipCode;
      this.Neighborhood = neighborhood;
      this.Complement = complement;
    }

    public void Update(string country, string state, string city, string zipCode, string neighborhood, string? complement)
    {
      this.Country = country;
      this.State = state;
      this.City = city;
      this.ZipCode = zipCode;
      this.Neighborhood = neighborhood;
      this.Complement = complement;
    }
  }
}
