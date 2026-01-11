using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParcAuto_Web_App.Models;
using ParcAuto_Web_App.Services;
using System.Collections.ObjectModel;

namespace ParcAuto_Web_App.ViewModels;

public partial class DriverListViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Driver> drivers = new();

    [ObservableProperty]
    private bool isRefreshing;

    public DriverListViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadDriversAsync()
    {
        try
        {
            IsRefreshing = true;
            var driverList = await _apiService.GetDriversAsync();
            
            Drivers.Clear();
            foreach (var driver in driverList)
            {
                Drivers.Add(driver);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load drivers: {ex.Message}", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public async Task AddDriverAsync()
    {
        await Shell.Current.GoToAsync("driverEdit");
    }

    [RelayCommand]
    public async Task EditDriverAsync(Driver driver)
    {
        if (driver == null) return;
        
        var parameters = new Dictionary<string, object>
        {
            { "Driver", driver }
        };
        
        await Shell.Current.GoToAsync("driverEdit", parameters);
    }

    [RelayCommand]
    public async Task DeleteDriverAsync(Driver driver)
    {
        if (driver == null) return;

        bool answer = await Shell.Current.DisplayAlert(
            "Confirm Delete",
            $"Are you sure you want to delete {driver.FirstName} {driver.LastName}?",
            "Yes", "No");

        if (!answer) return;

        try
        {
            var success = await _apiService.DeleteDriverAsync(driver.DriverId);
            if (success)
            {
                Drivers.Remove(driver);
                await Shell.Current.DisplayAlert("Success", "Driver deleted successfully", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to delete driver", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to delete driver: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        await LoadDriversAsync();
    }
}
