using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
    public class QRCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; private set; }

        public string? VehicleId { get; private set; }
        public Vehicle? Vehicle { get; private set; }

        public QRCode() { }

        public QRCode(Vehicle? vehicle)
        {
            this.VehicleId = vehicle?.Id;
            this.Vehicle = vehicle;
        }
    }
}
