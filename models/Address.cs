public class Address
{
  public required string Id { get; set; }
  public required string Country { get; set; }
  public required string State { get; set; }
  public required string City { get; set; }
  public required string ZipCode { get; set; }
  public required string Neighborhood { get; set; }
  public string? Complement { get; set; }
}
