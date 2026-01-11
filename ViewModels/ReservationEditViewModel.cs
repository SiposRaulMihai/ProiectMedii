using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParcAuto_Web_App.Models;
using ParcAuto_Web_App.Services;
using System.Collections.ObjectModel;
using Plugin.LocalNotification;

namespace ParcAuto_Web_App.ViewModels;

[QueryProperty(nameof(ReservationId), "id")]
public partial class ReservationEditViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private int reservationId;

    [ObservableProperty]
    private ObservableCollection<Vehicle> vehicles = new();

    [ObservableProperty]
    private ObservableCollection<Driver> drivers = new();

    [ObservableProperty]
    private Vehicle? selectedVehicle;

    [ObservableProperty]
    private Driver? selectedDriver;

    [ObservableProperty]
    private DateTime startDate = DateTime.Now;

    [ObservableProperty]
    private TimeSpan startTime = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    private DateTime endDate = DateTime.Now.AddDays(1);

    [ObservableProperty]
    private TimeSpan endTime = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    private string? purpose;

    [ObservableProperty]
    private string status = "Pending";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool IsEditMode => ReservationId > 0;

    public ReservationEditViewModel(ApiService apiService)
    {
        _apiService = apiService;
        _ = LoadPickerDataAsync();
    }

    partial void OnReservationIdChanged(int value)
    {
        if (value > 0)
        {
            _ = LoadReservationAsync();
        }
    }

    private async Task LoadPickerDataAsync()
    {
        try
        {
            // Load vehicles
            var vehicleList = await _apiService.GetVehiclesAsync();
            if (vehicleList != null)
            {
                Vehicles.Clear();
                foreach (var vehicle in vehicleList)
                {
                    Vehicles.Add(vehicle);
                }
            }

            // Load drivers
            var driverList = await _apiService.GetDriversAsync();
            if (driverList != null)
            {
                Drivers.Clear();
                foreach (var driver in driverList)
                {
                    Drivers.Add(driver);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading picker data: {ex.Message}");
        }
    }

    private async Task LoadReservationAsync()
    {
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            var reservation = await _apiService.GetReservationAsync(ReservationId);
            if (reservation != null)
            {
                StartDate = reservation.StartDate.Date;
                StartTime = reservation.StartDate.TimeOfDay;
                EndDate = reservation.EndDate.Date;
                EndTime = reservation.EndDate.TimeOfDay;
                Purpose = reservation.Purpose;
                Status = reservation.Status;

                // Set selected vehicle
                SelectedVehicle = Vehicles.FirstOrDefault(v => v.VehicleId == reservation.VehicleId);
                
                // Set selected driver
                SelectedDriver = Drivers.FirstOrDefault(d => d.DriverId == reservation.DriverId);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading reservation: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveReservationAsync()
    {
        ErrorMessage = string.Empty;

        if (!ValidateInput())
        {
            return;
        }

        IsBusy = true;

        try
        {
            var reservation = new Reservation
            {
                ReservationId = ReservationId,
                VehicleId = SelectedVehicle!.VehicleId,
                DriverId = SelectedDriver!.DriverId,
                StartDate = StartDate.Date + StartTime,
                EndDate = EndDate.Date + EndTime,
                Purpose = Purpose,
                Status = Status
            };

            bool success;
            if (IsEditMode)
            {
                success = await _apiService.UpdateReservationAsync(ReservationId, reservation);
            }
            else
            {
                var result = await _apiService.CreateReservationAsync(reservation);
                success = result != null;
                
                // Schedule local notification for the new reservation
                if (success && result != null)
                {
                    await ScheduleReservationNotificationAsync(result);
                }
            }

            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = "Failed to save reservation. Please check your input and try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private bool ValidateInput()
    {
        if (SelectedVehicle == null)
        {
            ErrorMessage = "Vehicle is required. Please select a vehicle.";
            return false;
        }

        if (SelectedDriver == null)
        {
            ErrorMessage = "Driver is required. Please select a driver.";
            return false;
        }

        if (StartDate == default)
        {
            ErrorMessage = "Start date is required.";
            return false;
        }

        if (EndDate == default)
        {
            ErrorMessage = "End date is required.";
            return false;
        }

        if (EndDate.Date < StartDate.Date || (EndDate.Date == StartDate.Date && EndTime <= StartTime))
        {
            ErrorMessage = "End date and time must be after start date and time.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Purpose))
        {
            ErrorMessage = "Purpose is required and cannot be empty.";
            return false;
        }

        return true;
    }

    private async Task ScheduleReservationNotificationAsync(Reservation reservation)
    {
        try
        {
            // Calculate notification time (5 minutes before start time)
            var notificationTime = reservation.StartDate.AddMinutes(-5);
            
            // Only schedule if the notification time is in the future
            if (notificationTime > DateTime.Now)
            {
                var notification = new NotificationRequest
                {
                    NotificationId = reservation.ReservationId,
                    Title = "Rezervare în 5 minute!",
                    Description = $"Rezervarea pentru {SelectedVehicle?.Make} {SelectedVehicle?.Model} ({SelectedVehicle?.LicensePlate}) începe la {reservation.StartDate:HH:mm}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notificationTime
                    },
                    BadgeNumber = 1
                };

                await LocalNotificationCenter.Current.Show(notification);
            }
            
            // Also schedule a notification 1 hour before
            var oneHourBefore = reservation.StartDate.AddHours(-1);
            if (oneHourBefore > DateTime.Now)
            {
                var notification = new NotificationRequest
                {
                    NotificationId = reservation.ReservationId + 10000, // Different ID
                    Title = "Rezervare în 1 oră",
                    Description = $"Rezervarea pentru {SelectedVehicle?.Make} {SelectedVehicle?.Model} ({SelectedVehicle?.LicensePlate}) începe la {reservation.StartDate:HH:mm}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = oneHourBefore
                    },
                    BadgeNumber = 1
                };

                await LocalNotificationCenter.Current.Show(notification);
            }

            // Schedule notification 24 hours before
            var oneDayBefore = reservation.StartDate.AddDays(-1);
            if (oneDayBefore > DateTime.Now)
            {
                var notification = new NotificationRequest
                {
                    NotificationId = reservation.ReservationId + 20000, // Different ID
                    Title = "Rezervare mâine",
                    Description = $"Rezervarea pentru {SelectedVehicle?.Make} {SelectedVehicle?.Model} ({SelectedVehicle?.LicensePlate}) începe mâine la {reservation.StartDate:HH:mm}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = oneDayBefore
                    },
                    BadgeNumber = 1
                };

                await LocalNotificationCenter.Current.Show(notification);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error scheduling notification: {ex.Message}");
        }
    }
}
