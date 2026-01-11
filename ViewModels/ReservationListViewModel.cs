using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParcAuto_Web_App.Models;
using ParcAuto_Web_App.Services;
using System.Collections.ObjectModel;
using Plugin.LocalNotification;

namespace ParcAuto_Web_App.ViewModels;

public partial class ReservationListViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly NotificationService _notificationService;

    [ObservableProperty]
    private ObservableCollection<Reservation> reservations = new();

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isBusy;

    public ReservationListViewModel(ApiService apiService, NotificationService notificationService)
    {
        _apiService = apiService;
        _notificationService = notificationService;
    }

    public async Task InitializeAsync()
    {
        await _notificationService.RequestPermissions();
        await LoadReservationsAsync();
        StartImminentReservationCheck();
    }

    [RelayCommand]
    private async Task LoadReservationsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            var reservationList = await _apiService.GetReservationsAsync();
            if (reservationList != null)
            {
                Reservations.Clear();
                foreach (var reservation in reservationList)
                {
                    Reservations.Add(reservation);
                }
                
                // Programează notificări pentru toate rezervările
                await ScheduleNotificationsForReservationsAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading reservations: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadReservationsAsync();
    }

    [RelayCommand]
    private async Task AddReservationAsync()
    {
        await Shell.Current.GoToAsync("reservationEdit");
    }

    [RelayCommand]
    private async Task SelectReservationAsync(Reservation reservation)
    {
        if (reservation == null) return;

        await Shell.Current.GoToAsync($"reservationDetails?id={reservation.ReservationId}");
    }

    private void StartImminentReservationCheck()
    {
        // Verifică la fiecare minut pentru rezervări iminente
        Dispatcher.GetForCurrentThread()?.StartTimer(TimeSpan.FromMinutes(1), () =>
        {
            CheckImminentReservations();
            return true; // Continuă verificarea
        });
    }

    private async void CheckImminentReservations()
    {
        try
        {
            var now = DateTime.Now;
            var imminentReservations = Reservations
                .Where(r => r.StartDate > now && r.StartDate <= now.AddMinutes(5))
                .ToList();

            foreach (var reservation in imminentReservations)
            {
                // Găsește vehiculul asociat
                var vehicle = await _apiService.GetVehicleAsync(reservation.VehicleId);
                if (vehicle != null)
                {
                    var vehicleName = $"{vehicle.Make} {vehicle.Model} ({vehicle.LicensePlate})";
                    await _notificationService.ShowImminentReservationAlert(vehicleName, reservation.StartDate);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking imminent reservations: {ex.Message}");
        }
    }

    private async Task ScheduleNotificationsForReservationsAsync()
    {
        try
        {
            foreach (var reservation in Reservations)
            {
                var vehicle = reservation.Vehicle;
                if (vehicle == null)
                {
                    vehicle = await _apiService.GetVehicleAsync(reservation.VehicleId);
                }

                if (vehicle != null)
                {
                    // Programează notificare cu 5 minute înainte
                    var fiveMinutesBefore = reservation.StartDate.AddMinutes(-5);
                    if (fiveMinutesBefore > DateTime.Now)
                    {
                        var notification = new NotificationRequest
                        {
                            NotificationId = reservation.ReservationId * 10 + 1,
                            Title = "Rezervare în 5 minute!",
                            Description = $"Rezervarea pentru {vehicle.Make} {vehicle.Model} ({vehicle.LicensePlate}) începe la {reservation.StartDate:HH:mm}",
                            Schedule = new NotificationRequestSchedule
                            {
                                NotifyTime = fiveMinutesBefore
                            },
                            BadgeNumber = 1
                        };

                        await LocalNotificationCenter.Current.Show(notification);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error scheduling notifications: {ex.Message}");
        }
    }
}
