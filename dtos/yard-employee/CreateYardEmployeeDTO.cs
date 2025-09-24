using AutoInsightAPI.Models;

namespace AutoInsightAPI.Dtos
{
  public class CreateYardEmployeeDto
  {
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
  }
}
