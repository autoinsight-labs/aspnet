using AutoInsightAPI.Models;
using AutoInsightAPI.Dtos.Common;

namespace AutoInsightAPI.Dtos
{
  public class EmployeeInviteDto : HateoasResourceDto, IIdentifiable
  {
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
    public InviteStatus Status { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public string? AcceptedByUserId { get; set; }
    public string YardId { get; set; } = string.Empty;
  }
}
