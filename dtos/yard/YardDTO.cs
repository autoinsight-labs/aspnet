using AutoInsightAPI.Dtos.Common;

namespace AutoInsightAPI.Dtos
{
  public class YardDto : HateoasResourceDto, IIdentifiable
  {
    public string Id { get; private set; } = string.Empty;
    public AddressDto Address { get; set; } = new();
    public string OwnerId { get; set; } = string.Empty;
  }
}
