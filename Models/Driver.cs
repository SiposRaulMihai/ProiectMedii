namespace ParcAuto_Web_App.Models;

public class Driver
{
    public int DriverId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime? LicenseExpiryDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
