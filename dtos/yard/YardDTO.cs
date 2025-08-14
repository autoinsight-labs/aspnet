namespace AutoInsightAPI.Dtos
{
  public class YardDto
  {
    public string Id {get; private set;}

    public AddressDto Address {get; set;}
    public string OwnerId {get; set;}
  }
}
