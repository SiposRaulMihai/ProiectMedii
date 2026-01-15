using System.ComponentModel.DataAnnotations;

namespace ParcAuto.Web.Models
{
    public class Driver
    {
        public int DriverId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
}
