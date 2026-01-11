using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParcAuto_Web_App.Services;

namespace ParcAuto_Web_App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool rememberMe = true;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public LoginViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        // Clear previous error
        ErrorMessage = string.Empty;

        // Validation
        if (!ValidateInput())
        {
            return;
        }

        IsBusy = true;

        try
        {
            var token = await _apiService.LoginAsync(Email, Password);

            if (!string.IsNullOrEmpty(token))
            {
                // Token is already stored in SecureStorage by ApiService
                // Save login state and user email
                Preferences.Default.Set("is_logged_in", true);
                Preferences.Default.Set("user_email", Email);
                
                // Navigate to main app
                await Shell.Current.GoToAsync("///MainPage");
            }
            else
            {
                ErrorMessage = "Invalid email or password.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        // Clear previous error
        ErrorMessage = string.Empty;

        // Validation
        if (!ValidateInput())
        {
            return;
        }

        IsBusy = true;

        try
        {
            var success = await _apiService.RegisterAsync(Email, Password);

            if (success)
            {
                // Auto-login after registration
                await LoginAsync();
            }
            else
            {
                ErrorMessage = "Registration failed. Email may already be in use.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Registration failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool ValidateInput()
    {
        // Email validation
        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Email is required.";
            return false;
        }

        if (!Email.Contains('@'))
        {
            ErrorMessage = "Email must be a valid email address.";
            return false;
        }

        // Password validation - matches backend rules
        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Password is required.";
            return false;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters.";
            return false;
        }

        // Check for at least one digit (required by backend)
        if (!Password.Any(char.IsDigit))
        {
            ErrorMessage = "Password must contain at least one digit.";
            return false;
        }

        return true;
    }
}
