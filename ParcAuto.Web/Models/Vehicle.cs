using System.ComponentModel.DataAnnotations;

namespace ParcAuto.Web.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }

        [Required]
        public string Make { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Range(1990, 2100)]
        public int Year { get; set; }

        [Required]
        [StringLength(10)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        public string VIN { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        [Required]
        public string Status { get; set; } = "Available";
    }
}
