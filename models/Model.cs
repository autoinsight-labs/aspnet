using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Model
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public required string Id { get; set; }

  public required string Name { get; set; }
  public required int Year { get; set; }
}
