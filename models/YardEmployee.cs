using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
  public enum EmployeeRole {
    ADMIN,
    MEMBER
  }

  public class YardEmployee
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id {get; private set;}

    public string Name {get; private set;}
    public string? ImageUrl {get; private set;}
    public EmployeeRole Role {get; private set;}
    public string UserId {get; private set;}
    public string YardId {get; private set;}
    public Yard Yard {get; private set;}

    public YardEmployee() { }

    public YardEmployee(string name, string? imageUrl, EmployeeRole role, string userId, Yard yard)
    {
      this.Name = name;
      this.ImageUrl = imageUrl;
      this.Role = role;
      this.UserId = userId;
      this.YardId = yard.Id;
      this.Yard = yard;
    }

    public YardEmployee(string id, string name, string? imageUrl, EmployeeRole role, string userId, Yard yard)
    {
      this.Id = id;
      this.Name = name;
      this.ImageUrl = imageUrl;
      this.Role = role;
      this.UserId = userId;
      this.YardId = yard.Id;
      this.Yard = yard;
    }

    public void Update(string name, string? imageUrl, EmployeeRole role, string userId)
    {
      this.Name = name;
      this.ImageUrl = imageUrl;
      this.Role = role;
      this.UserId = userId;
    }
  }
}
