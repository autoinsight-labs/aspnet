using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoInsightAPI.Models
{
    public class QRCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }
        public string YardId { get; set; }
        public Yard Yard { get; set; }

        public QRCode() { }

        public QRCode(string id, Vehicle? vehicle, Yard yard )
        {
            this.Id = id;
            this.VehicleId = vehicle?.Id;
            this.Vehicle = vehicle;
            this.YardId = yard.Id;
            this.Yard = yard;
        }
    }
}
