using AutoInsightAPI.Models;
using AutoInsightAPI.Dtos.Common;

namespace AutoInsightAPI.Dtos
{
  public class YardEmployeeDto : HateoasResourceDto, IIdentifiable
  {
    public string Id { get; private set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
    public string UserId { get; set; } = string.Empty;
  }
}
