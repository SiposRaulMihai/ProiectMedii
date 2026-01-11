using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParcAuto_Web_App.Models;
using ParcAuto_Web_App.Services;

namespace ParcAuto_Web_App.ViewModels;

[QueryProperty(nameof(VehicleId), "id")]
public partial class VehicleEditViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private int vehicleId;

    [ObservableProperty]
    private string make = string.Empty;

    [ObservableProperty]
    private string model = string.Empty;

    [ObservableProperty]
    private int year = DateTime.Now.Year;

    [ObservableProperty]
    private string licensePlate = string.Empty;

    [ObservableProperty]
    private string vin = string.Empty;

    [ObservableProperty]
    private string? color;

    [ObservableProperty]
    private int? mileage;

    [ObservableProperty]
    private string status = "Available";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool IsEditMode => VehicleId > 0;

    public VehicleEditViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    partial void OnVehicleIdChanged(int value)
    {
        if (value > 0)
        {
            _ = LoadVehicleAsync();
        }
    }

    private async Task LoadVehicleAsync()
    {
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            var vehicle = await _apiService.GetVehicleAsync(VehicleId);
            if (vehicle != null)
            {
                Make = vehicle.Make;
                Model = vehicle.Model;
                Year = vehicle.Year;
                LicensePlate = vehicle.LicensePlate;
                Vin = vehicle.VIN;
                Color = vehicle.Color;
                Mileage = vehicle.Mileage;
                Status = vehicle.Status;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading vehicle: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveVehicleAsync()
    {
        ErrorMessage = string.Empty;

        if (!ValidateInput())
        {
            return;
        }

        IsBusy = true;

        try
        {
            var vehicle = new Vehicle
            {
                VehicleId = VehicleId,
                Make = Make,
                Model = Model,
                Year = Year,
                LicensePlate = LicensePlate,
                VIN = Vin,
                Color = Color,
                Mileage = Mileage,
                Status = Status
            };

            bool success;
            if (IsEditMode)
            {
                success = await _apiService.UpdateVehicleAsync(VehicleId, vehicle);
            }
            else
            {
                var result = await _apiService.CreateVehicleAsync(vehicle);
                success = result != null;
            }

            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = "Failed to save vehicle.";
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
        if (string.IsNullOrWhiteSpace(Make))
        {
            ErrorMessage = "Make is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Model))
        {
            ErrorMessage = "Model is required.";
            return false;
        }

        if (Year < 1900 || Year > DateTime.Now.Year + 1)
        {
            ErrorMessage = "Invalid year.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(LicensePlate))
        {
            ErrorMessage = "License plate is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Vin))
        {
            ErrorMessage = "VIN is required.";
            return false;
        }

        return true;
    }
}
