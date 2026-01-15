using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Models;

namespace ParcAuto.Web.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ParcAutoContext(
            serviceProvider.GetRequiredService<DbContextOptions<ParcAutoContext>>());
        
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Creăm baza de date dacă nu există
        context.Database.EnsureCreated();

        // 2. Populăm Utilizatorul de Test (pentru logare pe Mobil și Site)
        var testEmail = "test@gmail.com";
        if (await userManager.FindByEmailAsync(testEmail) == null)
        {
            var user = new ApplicationUser { UserName = testEmail, Email = testEmail, EmailConfirmed = true };
            await userManager.CreateAsync(user, "Password123!");
        }

        // 3. Populăm Vehiculele (pentru a avea listă pe Mobil)
        if (!context.Vehicles.Any())
        {
            context.Vehicles.AddRange(
                new ParcAuto.Web.Models.Vehicle 
                { 
                    Make = "Dacia", 
                    Model = "Logan", 
                    Year = 2024, 
                    LicensePlate = "CJ-01-RAU", 
                    VIN = "VIN111", 
                    Status = "Available",
                    Color = "White",
                    Mileage = 0
                },
                new ParcAuto.Web.Models.Vehicle 
                { 
                    Make = "Toyota", 
                    Model = "Corolla", 
                    Year = 2023, 
                    LicensePlate = "B-99-ABC", 
                    VIN = "VIN222", 
                    Status = "Available",
                    Color = "Blue",
                    Mileage = 5000
                }
            );
            await context.SaveChangesAsync();
        }

        // 4. Populăm Mentenanțele (pentru a evita eroarea de Foreign Key)
        if (!context.Maintenances.Any())
        {
            var vehicle = context.Vehicles.First();
            context.Maintenances.Add(new Maintenance 
            { 
                VehicleId = vehicle.VehicleId, 
                MaintenanceDate = DateTime.Now, 
                Description = "Schimb de ulei periodic", 
                Cost = 450 
            });
            await context.SaveChangesAsync();
        }
    }
}
