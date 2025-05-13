namespace AutoInsightAPI.Dtos
{
  public class AddressDTO
  {
    public string? Id { get; private set; }

    public string Country { get; set; }
    public string State { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string Neighborhood { get; set; }
    public string? Complement { get; set; }
  }
}
