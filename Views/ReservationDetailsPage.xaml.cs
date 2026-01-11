using ParcAuto_Web_App.ViewModels;

namespace ParcAuto_Web_App.Views;

public partial class ReservationDetailsPage : ContentPage
{
    public ReservationDetailsPage(ReservationDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
