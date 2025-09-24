using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
  public enum InviteStatus
  {
    PENDING,
    ACCEPTED,
    REJECTED
  }

  public class EmployeeInvite
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; private set; }

    public string Email { get; private set; }
    public string Name { get; private set; }
    public EmployeeRole Role { get; private set; }
    public InviteStatus Status { get; private set; }
    public string Token { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public string? AcceptedByUserId { get; private set; }
    
    public string YardId { get; private set; }
    public Yard Yard { get; private set; }

    public EmployeeInvite() { }

    public EmployeeInvite(string email, string name, EmployeeRole role, string token, Yard yard)
    {
      this.Email = email;
      this.Name = name;
      this.Role = role;
      this.Status = InviteStatus.PENDING;
      this.Token = token;
      this.CreatedAt = DateTime.UtcNow;
      this.YardId = yard.Id;
      this.Yard = yard;
    }

    public void Accept(string userId)
    {
      this.Status = InviteStatus.ACCEPTED;
      this.AcceptedAt = DateTime.UtcNow;
      this.AcceptedByUserId = userId;
    }

    public void Reject()
    {
      this.Status = InviteStatus.REJECTED;
    }
  }
}
