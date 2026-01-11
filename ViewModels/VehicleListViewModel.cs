using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParcAuto_Web_App.Models;
using ParcAuto_Web_App.Services;
using System.Collections.ObjectModel;

namespace ParcAuto_Web_App.ViewModels;

public partial class VehicleListViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Vehicle> vehicles = new();

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isBusy;

    public VehicleListViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task InitializeAsync()
    {
        await LoadVehiclesAsync();
    }

    [RelayCommand]
    private async Task LoadVehiclesAsync()
    {
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            var vehicleList = await _apiService.GetVehiclesAsync();
            if (vehicleList != null)
            {
                Vehicles.Clear();
                foreach (var vehicle in vehicleList)
                {
                    Vehicles.Add(vehicle);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading vehicles: {ex.Message}");
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
        await LoadVehiclesAsync();
    }

    [RelayCommand]
    private async Task AddVehicleAsync()
    {
        await Shell.Current.GoToAsync("vehicleEdit");
    }

    [RelayCommand]
    private async Task SelectVehicleAsync(Vehicle vehicle)
    {
        if (vehicle == null) return;

        await Shell.Current.GoToAsync($"vehicleDetails?id={vehicle.VehicleId}");
    }
}
