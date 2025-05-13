using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
  public class EmployeeInvite
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; private set; }

    public string YardEmployeeId { get; private set; }
    public YardEmployee YardEmployee { get; private set; }
    public string YardId { get; private set; }
    public Yard Yard { get; private set; }

    public EmployeeInvite() { }

    public EmployeeInvite(string id, YardEmployee yardEmployee, Yard yard)
    {
      this.Id = id;
      this.YardEmployeeId = yardEmployee.Id;
      this.YardEmployee = yardEmployee;
      this.YardId = yard.Id;
      this.Yard = yard;
    }
  }
}
