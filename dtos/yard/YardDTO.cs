namespace AutoInsightAPI.Dtos
{
  public class YardDTO
  {
    public string Id {get; private set;}

    public AddressDTO Address {get; set;}
    public string OwnerId {get; set;}
  }
}
