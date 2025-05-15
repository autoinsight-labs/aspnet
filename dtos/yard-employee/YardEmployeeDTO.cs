using AutoInsightAPI.Models;

namespace AutoInsightAPI.Dtos
{
  public class YardEmployeeDTO
  {
    public string Id {get; private set;}

    public EmployeeRole Role {get; set;}
    public string UserId {get; set;}
  }
}
