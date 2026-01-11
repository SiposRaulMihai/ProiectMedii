using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParcAuto_Web_App.Models;
using ParcAuto_Web_App.Services;

namespace ParcAuto_Web_App.ViewModels;

[QueryProperty(nameof(Driver), "Driver")]
public partial class DriverEditViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private Driver? driver;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string licenseNumber = string.Empty;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private string pageTitle = "Add Driver";

    public DriverEditViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    partial void OnDriverChanged(Driver? value)
    {
        if (value != null)
        {
            IsEditMode = true;
            PageTitle = "Edit Driver";
            FirstName = value.FirstName;
            LastName = value.LastName;
            LicenseNumber = value.LicenseNumber;
            PhoneNumber = value.PhoneNumber ?? string.Empty;
            Email = value.Email ?? string.Empty;
        }
        else
        {
            IsEditMode = false;
            PageTitle = "Add Driver";
            ClearForm();
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || 
            string.IsNullOrWhiteSpace(LastName) || 
            string.IsNullOrWhiteSpace(LicenseNumber))
        {
            await Shell.Current.DisplayAlert("Validation Error", "Please fill in all required fields (First Name, Last Name, License Number)", "OK");
            return;
        }

        try
        {
            Driver driverToSave;
            if (IsEditMode && Driver != null)
            {
                driverToSave = new Driver
                {
                    DriverId = Driver.DriverId,
                    FirstName = FirstName,
                    LastName = LastName,
                    LicenseNumber = LicenseNumber,
                    PhoneNumber = PhoneNumber,
                    Email = Email
                };
                var success = await _apiService.UpdateDriverAsync(Driver.DriverId, driverToSave);
                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Driver updated successfully", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to update driver", "OK");
                }
            }
            else
            {
                driverToSave = new Driver
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    LicenseNumber = LicenseNumber,
                    PhoneNumber = PhoneNumber,
                    Email = Email
                };
                var success = await _apiService.AddDriverAsync(driverToSave);
                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Driver added successfully", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to add driver", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    public async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void ClearForm()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        LicenseNumber = string.Empty;
        PhoneNumber = string.Empty;
        Email = string.Empty;
    }
}
