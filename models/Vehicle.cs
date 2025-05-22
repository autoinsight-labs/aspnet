using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
  public class Vehicle
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id {get; private set;}

    public string Plate {get; private set;}
    public string ModelId {get; private set;}
    public Model Model {get; private set;}
    public string UserId {get; private set;}

    public Vehicle() { }

    public Vehicle(string plate, Model model, string userId)
    {
      this.Plate = plate;
      this.ModelId = model.Id;
      this.Model = model;
      this.UserId = userId;
    }
  }
}
