using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParcAuto_Web_App.Models;
using ParcAuto_Web_App.Services;

namespace ParcAuto_Web_App.ViewModels;

[QueryProperty(nameof(VehicleId), "id")]
public partial class VehicleDetailsViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private Vehicle? vehicle;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private int vehicleId;

    public VehicleDetailsViewModel(ApiService apiService)
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
            Vehicle = await _apiService.GetVehicleAsync(VehicleId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading vehicle: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditVehicleAsync()
    {
        if (Vehicle == null) return;

        await Shell.Current.GoToAsync($"vehicleEdit?id={Vehicle.VehicleId}");
    }

    [RelayCommand]
    private async Task DeleteVehicleAsync()
    {
        if (Vehicle == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Delete Vehicle",
            $"Are you sure you want to delete {Vehicle.DisplayName}?",
            "Delete",
            "Cancel");

        if (!confirm) return;

        IsBusy = true;

        try
        {
            var success = await _apiService.DeleteVehicleAsync(Vehicle.VehicleId);
            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to delete vehicle.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
