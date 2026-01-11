using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParcAuto_Web_App.Models;
using ParcAuto_Web_App.Services;

namespace ParcAuto_Web_App.ViewModels;

[QueryProperty(nameof(ReservationId), "id")]
public partial class ReservationDetailsViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private Reservation? reservation;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private int reservationId;

    public ReservationDetailsViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    partial void OnReservationIdChanged(int value)
    {
        if (value > 0)
        {
            _ = LoadReservationAsync();
        }
    }

    private async Task LoadReservationAsync()
    {
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            Reservation = await _apiService.GetReservationAsync(ReservationId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading reservation: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditReservationAsync()
    {
        if (Reservation == null) return;

        await Shell.Current.GoToAsync($"reservationEdit?id={Reservation.ReservationId}");
    }

    [RelayCommand]
    private async Task DeleteReservationAsync()
    {
        if (Reservation == null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Delete Reservation",
            "Are you sure you want to delete this reservation?",
            "Delete",
            "Cancel");

        if (!confirm) return;

        IsBusy = true;

        try
        {
            var success = await _apiService.DeleteReservationAsync(Reservation.ReservationId);
            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to delete reservation.", "OK");
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
