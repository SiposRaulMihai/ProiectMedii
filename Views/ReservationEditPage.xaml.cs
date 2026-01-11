using ParcAuto_Web_App.ViewModels;

namespace ParcAuto_Web_App.Views;

public partial class ReservationEditPage : ContentPage
{
    public ReservationEditPage(ReservationEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
