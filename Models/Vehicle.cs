namespace ParcAuto_Web_App.Models;

public class Vehicle
{
    public int VehicleId { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string VIN { get; set; } = string.Empty;
    public string? Color { get; set; }
    public int? Mileage { get; set; }
    public string Status { get; set; } = "Available";

    public string DisplayName => $"{Make} {Model} ({Year})";
}
