using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParcAuto.Web.Models
{
    public class Maintenance
    {
        public int MaintenanceId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }

        [Required]
        [Display(Name = "Maintenance Date")]
        public DateTime MaintenanceDate { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Cost { get; set; }
    }
}
