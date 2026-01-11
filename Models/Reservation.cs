namespace ParcAuto_Web_App.Models;

public class Reservation
{
    public int ReservationId { get; set; }
    public int VehicleId { get; set; }
    public int DriverId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Purpose { get; set; }
    public string Status { get; set; } = "Pending";

    // Navigation properties (for display)
    public Vehicle? Vehicle { get; set; }
    public Driver? Driver { get; set; }

    public string DisplayInfo => $"{Vehicle?.DisplayName ?? "Vehicle"} - {StartDate:MMM dd} to {EndDate:MMM dd}";
}
