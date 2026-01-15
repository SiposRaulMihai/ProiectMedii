using Microsoft.EntityFrameworkCore;
using ParcAuto.Web.Data;

namespace ParcAuto.Web.Services;

public class ReservationReminderService : BackgroundService
{
    private readonly IServiceProvider _services;
    
    public ReservationReminderService(IServiceProvider services) => _services = services;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ParcAutoContext>();
                var acum = DateTime.Now;
                var peste1Minut = acum.AddMinutes(1);
                var peste5Minute = acum.AddMinutes(5);

                // Căutăm rezervări care încep în intervalul 1-5 minute
                var rezervariApropiate = await context.Reservations
                    .Include(r => r.Vehicle)
                    .Where(r => r.StartDate > peste1Minut && r.StartDate <= peste5Minute)
                    .ToListAsync();

                foreach (var res in rezervariApropiate)
                {
                    Console.WriteLine($"⚠️ ALERTĂ: Rezervarea pentru {res.Vehicle?.LicensePlate} începe la ora {res.StartDate:HH:mm}");
                }

                // Verificăm și rezervările de mâine (notificare preventivă)
                var maine = acum.AddDays(1).Date;
                var rezervariMaine = await context.Reservations
                    .Include(r => r.Vehicle)
                    .Where(r => r.StartDate.Date == maine)
                    .ToListAsync();

                foreach (var res in rezervariMaine)
                {
                    Console.WriteLine($"📅 NOTIFICARE: Rezervare mâine pentru {res.Vehicle?.LicensePlate} la {res.StartDate:HH:mm}");
                }
            }
            
            // Verificăm în fiecare minut pentru alerte iminente
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
