using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using ParcAuto_Web_App.Models;

namespace ParcAuto_Web_App.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string TokenKey = "auth_token";
    private const string BaseUrl = "http://10.0.2.2:5002";

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    #region Token Management

    public async Task SetTokenAsync(string token)
    {
        await SecureStorage.SetAsync(TokenKey, token);
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync(TokenKey);
    }

    public async Task ClearTokenAsync()
    {
        SecureStorage.Remove(TokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    private async Task AttachTokenAsync()
    {
        var token = await GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task HandleUnauthorizedAsync()
    {
        await ClearTokenAsync();
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var app = Application.Current;
            if (app != null && app.Windows.Count > 0)
            {
                var currentPage = app.Windows[0].Page;
                if (currentPage != null)
                {
                    await currentPage.DisplayAlert(
                        "Session Expired",
                        "Your session has expired. Please login again.",
                        "OK");
                }
                
                // Navigate to login page
                var loginPage = new Views.LoginPage(
                    app.Handler?.MauiContext?.Services.GetService<ViewModels.LoginViewModel>()!);
                app.Windows[0].Page = loginPage;
            }
        });
    }

    #endregion

    #region Auth Endpoints

    // POST /api/auth/login
    public async Task<string?> LoginAsync(string email, string password)
    {
        try
        {
            var loginData = new { email, password };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginData);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result?.Token != null)
                {
                    await SetTokenAsync(result.Token);
                    return result.Token;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    // POST /api/auth/register
    public async Task<bool> RegisterAsync(string email, string password)
    {
        try
        {
            var registerData = new { email, password };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerData);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Register error: {ex.Message}");
            return false;
        }
    }

    // GET /api/drivers
    public async Task<List<Driver>> GetDriversAsync()
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync("/api/drivers");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return new List<Driver>();
            }
            
            if (!response.IsSuccessStatusCode)
            {
                return new List<Driver>();
            }
            
            var result = await response.Content.ReadFromJsonAsync<List<Driver>>();
            return result ?? new List<Driver>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get drivers error: {ex.Message}");
            return new List<Driver>();
        }
    }

    #endregion

    #region Driver Endpoints

    // GET /api/drivers/{id}
    public async Task<Driver?> GetDriverAsync(int id)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync($"/api/drivers/{id}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Driver>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get driver error: {ex.Message}");
            return null;
        }
    }

    // POST /api/drivers [Authorize]
    public async Task<Driver?> CreateDriverAsync(Driver driver)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PostAsJsonAsync("/api/drivers", driver);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Driver>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create driver error: {ex.Message}");
            return null;
        }
    }

    // PUT /api/drivers/{id} [Authorize]
    public async Task<bool> UpdateDriverAsync(int id, Driver driver)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PutAsJsonAsync($"/api/drivers/{id}", driver);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update driver error: {ex.Message}");
            return false;
        }
    }

    // DELETE /api/drivers/{id} [Authorize]
    public async Task<bool> DeleteDriverAsync(int id)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.DeleteAsync($"/api/drivers/{id}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete driver error: {ex.Message}");
            return false;
        }
    }

    // Wrapper methods for ViewModels
    public async Task<bool> AddDriverAsync(Driver driver)
    {
        var result = await CreateDriverAsync(driver);
        return result != null;
    }

    #endregion

    #region Vehicle Endpoints

    // GET /api/vehicles
    public async Task<List<Vehicle>> GetVehiclesAsync()
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync("/api/vehicles");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return new List<Vehicle>();
            }
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Get vehicles failed with status: {response.StatusCode}");
                return new List<Vehicle>();
            }
            
            var result = await response.Content.ReadFromJsonAsync<List<Vehicle>>();
            return result ?? new List<Vehicle>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get vehicles error: {ex.Message}");
            return new List<Vehicle>();
        }
    }

    // GET /api/vehicles/{id}
    public async Task<Vehicle?> GetVehicleAsync(int id)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync($"/api/vehicles/{id}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Vehicle>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get vehicle error: {ex.Message}");
            return null;
        }
    }

    // POST /api/vehicles [Authorize]
    public async Task<Vehicle?> CreateVehicleAsync(Vehicle vehicle)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PostAsJsonAsync("/api/vehicles", vehicle);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Vehicle>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create vehicle error: {ex.Message}");
            return null;
        }
    }

    // PUT /api/vehicles/{id} [Authorize]
    public async Task<bool> UpdateVehicleAsync(int id, Vehicle vehicle)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PutAsJsonAsync($"/api/vehicles/{id}", vehicle);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update vehicle error: {ex.Message}");
            return false;
        }
    }

    // DELETE /api/vehicles/{id} [Authorize]
    public async Task<bool> DeleteVehicleAsync(int id)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.DeleteAsync($"/api/vehicles/{id}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete vehicle error: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Reservation Endpoints

    // GET /api/reservations [Authorize]
    public async Task<List<Reservation>> GetReservationsAsync()
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync("/api/reservations");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return new List<Reservation>();
            }
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Get reservations failed with status: {response.StatusCode}");
                return new List<Reservation>();
            }
            
            var result = await response.Content.ReadFromJsonAsync<List<Reservation>>();
            return result ?? new List<Reservation>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get reservations error: {ex.Message}");
            return new List<Reservation>();
        }
    }

    // GET /api/reservations/{id} [Authorize]
    public async Task<Reservation?> GetReservationAsync(int id)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync($"/api/reservations/{id}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Reservation>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get reservation error: {ex.Message}");
            return null;
        }
    }

    // POST /api/reservations [Authorize]
    public async Task<Reservation?> CreateReservationAsync(Reservation reservation)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PostAsJsonAsync("/api/reservations", reservation);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Reservation>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create reservation error: {ex.Message}");
            return null;
        }
    }

    // PUT /api/reservations/{id} [Authorize]
    public async Task<bool> UpdateReservationAsync(int id, Reservation reservation)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PutAsJsonAsync($"/api/reservations/{id}", reservation);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update reservation error: {ex.Message}");
            return false;
        }
    }

    // DELETE /api/reservations/{id} [Authorize]
    public async Task<bool> DeleteReservationAsync(int id)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.DeleteAsync($"/api/reservations/{id}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete reservation error: {ex.Message}");
            return false;
        }
    }

    // GET /api/reservations/available-vehicles [Authorize]
    public async Task<List<Vehicle>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync(
                $"/api/reservations/available-vehicles?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return new List<Vehicle>();
            }
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Get available vehicles failed with status: {response.StatusCode}");
                return new List<Vehicle>();
            }
            
            var result = await response.Content.ReadFromJsonAsync<List<Vehicle>>();
            return result ?? new List<Vehicle>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get available vehicles error: {ex.Message}");
            return new List<Vehicle>();
        }
    }

    #endregion

    #region Maintenance Endpoints

    // GET /api/maintenances
    public async Task<List<Maintenance>> GetMaintenancesAsync()
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync("/api/maintenances");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return new List<Maintenance>();
            }
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Get maintenances failed with status: {response.StatusCode}");
                return new List<Maintenance>();
            }
            
            var result = await response.Content.ReadFromJsonAsync<List<Maintenance>>();
            return result ?? new List<Maintenance>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get maintenances error: {ex.Message}");
            return new List<Maintenance>();
        }
    }

    // GET /api/maintenances/{id}
    public async Task<Maintenance?> GetMaintenanceAsync(int id)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync($"/api/maintenances/{id}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Maintenance>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get maintenance error: {ex.Message}");
            return null;
        }
    }

    // POST /api/maintenances [Authorize]
    public async Task<Maintenance?> CreateMaintenanceAsync(Maintenance maintenance)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PostAsJsonAsync("/api/maintenances", maintenance);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Maintenance>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create maintenance error: {ex.Message}");
            return null;
        }
    }

    // PUT /api/maintenances/{id} [Authorize]
    public async Task<bool> UpdateMaintenanceAsync(int id, Maintenance maintenance)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PutAsJsonAsync($"/api/maintenances/{id}", maintenance);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update maintenance error: {ex.Message}");
            return false;
        }
    }

    // DELETE /api/maintenances/{id} [Authorize]
    public async Task<bool> DeleteMaintenanceAsync(int id)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.DeleteAsync($"/api/maintenances/{id}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete maintenance error: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region FuelLog Endpoints

    // GET /api/fuellogs
    public async Task<List<FuelLog>> GetFuelLogsAsync()
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync("/api/fuellogs");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return new List<FuelLog>();
            }
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Get fuel logs failed with status: {response.StatusCode}");
                return new List<FuelLog>();
            }
            
            var result = await response.Content.ReadFromJsonAsync<List<FuelLog>>();
            return result ?? new List<FuelLog>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get fuel logs error: {ex.Message}");
            return new List<FuelLog>();
        }
    }

    // GET /api/fuellogs/{id}
    public async Task<FuelLog?> GetFuelLogAsync(int id)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync($"/api/fuellogs/{id}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FuelLog>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get fuel log error: {ex.Message}");
            return null;
        }
    }

    // POST /api/fuellogs [Authorize]
    public async Task<FuelLog?> CreateFuelLogAsync(FuelLog fuelLog)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PostAsJsonAsync("/api/fuellogs", fuelLog);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FuelLog>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Create fuel log error: {ex.Message}");
            return null;
        }
    }

    // PUT /api/fuellogs/{id} [Authorize]
    public async Task<bool> UpdateFuelLogAsync(int id, FuelLog fuelLog)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PutAsJsonAsync($"/api/fuellogs/{id}", fuelLog);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update fuel log error: {ex.Message}");
            return false;
        }
    }

    // DELETE /api/fuellogs/{id} [Authorize]
    public async Task<bool> DeleteFuelLogAsync(int id)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.DeleteAsync($"/api/fuellogs/{id}");
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete fuel log error: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Generic HTTP Methods

    // GET request
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.GetAsync(endpoint);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return default;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GET request: {ex.Message}");
            return default;
        }
    }

    // POST request
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return default;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in POST request: {ex.Message}");
            return default;
        }
    }

    // PUT request
    public async Task<bool> PutAsync<T>(string endpoint, T data)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in PUT request: {ex.Message}");
            return false;
        }
    }

    // DELETE request
    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _httpClient.DeleteAsync(endpoint);
            
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await HandleUnauthorizedAsync();
                return false;
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DELETE request: {ex.Message}");
            return false;
        }
    }

    #endregion
}

// Response model for login
public class LoginResponse
{
    public string? Token { get; set; }
}
