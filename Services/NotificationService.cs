using Plugin.LocalNotification;

namespace ParcAuto_Web_App.Services;

public class NotificationService
{
    public async Task ShowNotification(string title, string message)
    {
        var request = new NotificationRequest
        {
            NotificationId = new Random().Next(1000, 9999),
            Title = title,
            Description = message,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(1) // Afișează imediat
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    public async Task ShowImminentReservationAlert(string vehicleName, DateTime startDate)
    {
        var minutesRemaining = (int)(startDate - DateTime.Now).TotalMinutes;
        
        var title = minutesRemaining <= 1 
            ? "⚠️ Rezervarea începe ACUM!" 
            : $"⚠️ {minutesRemaining} minute până la rezervare";
            
        var message = $"Vehiculul {vehicleName} trebuie preluat.";

        await ShowNotification(title, message);
    }

    public async Task<bool> RequestPermissions()
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        return await LocalNotificationCenter.Current.AreNotificationsEnabled();
    }
}
