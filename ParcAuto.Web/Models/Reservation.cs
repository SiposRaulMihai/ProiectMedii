using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParcAuto.Web.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }

        [Required]
        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public Vehicle? Vehicle { get; set; }

        [Required]
        [Display(Name = "Driver")]
        public int DriverId { get; set; }

        [ForeignKey("DriverId")]
        public Driver? Driver { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        public string Purpose { get; set; } = string.Empty;
    }
}
