using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class QRCode
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required string Id { get; set; }

    public string? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }
    public required string YardId { get; set; }
    public required Yard Yard { get; set; }
}
