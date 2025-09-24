using AutoInsightAPI.Models;

namespace AutoInsightAPI.Dtos
{
  public class CreateYardEmployeeDto
  {
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
    public string UserId { get; set; } = string.Empty;
  }
}
