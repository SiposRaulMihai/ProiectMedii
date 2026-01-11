using ParcAuto_Web_App.ViewModels;

namespace ParcAuto_Web_App.Views;

public partial class VehicleEditPage : ContentPage
{
    public VehicleEditPage(VehicleEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
