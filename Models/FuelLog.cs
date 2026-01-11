namespace ParcAuto_Web_App.Models;

public class FuelLog
{
    public int FuelLogId { get; set; }
    public int VehicleId { get; set; }
    public DateTime FuelDate { get; set; }
    public decimal Liters { get; set; }
    public decimal Price { get; set; }
    
    // Navigation properties
    public Vehicle? Vehicle { get; set; }
}
