namespace AutoInsightAPI.Dtos
{
  public class CreateYardDto
  {
    public AddressDto Address { get; set; } = new();
    public string OwnerId { get; set; } = string.Empty;
  }
}
