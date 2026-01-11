namespace ParcAuto_Web_App.Models;

public class Maintenance
{
    public int MaintenanceId { get; set; }
    public int VehicleId { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    
    // Navigation properties
    public Vehicle? Vehicle { get; set; }
}
