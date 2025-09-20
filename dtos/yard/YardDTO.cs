using AutoInsightAPI.Dtos.Common;

namespace AutoInsightAPI.Dtos
{
  public class YardDto : HateoasResourceDto
  {
    public string Id {get; private set;}
    public AddressDto Address {get; set;}
    public string OwnerId {get; set;}
  }
}
