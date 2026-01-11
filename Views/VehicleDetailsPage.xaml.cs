using ParcAuto_Web_App.ViewModels;

namespace ParcAuto_Web_App.Views;

public partial class VehicleDetailsPage : ContentPage
{
    public VehicleDetailsPage(VehicleDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
