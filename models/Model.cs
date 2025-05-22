using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
  public class Model
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; private set; }

    public string Name { get; private set; }
    public int Year { get; private set; }

    public Model() { }

    public Model(string name, int year)
    {
      this.Name = name;
      this.Year = year;
    }
  }
}
