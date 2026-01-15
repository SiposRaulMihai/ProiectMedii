using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParcAuto.Web.Models
{
    public class FuelLog
    {
        public int FuelLogId { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }

        [Required]
        [Display(Name = "Fuel Date")]
        public DateTime FuelDate { get; set; }

        [Required]
        public decimal Liters { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
