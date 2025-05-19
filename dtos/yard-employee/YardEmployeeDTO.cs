using AutoInsightAPI.Models;

namespace AutoInsightAPI.Dtos
{
  public class YardEmployeeDTO
  {
    public string Id {get; private set;}
    public string Name {get; set;}
    public string ImageUrl {get; set;}
    public EmployeeRole Role {get; set;}
    public string UserId {get; set;}
  }
}
