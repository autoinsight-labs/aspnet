namespace AutoInsightAPI.Dtos
{
  public class AddressDto
  {
    public string? Id { get; private set; }
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public string? Complement { get; set; }
  }
}
