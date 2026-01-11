using ParcAuto_Web_App.ViewModels;

namespace ParcAuto_Web_App.Views;

public partial class DriverEditPage : ContentPage
{
    public DriverEditPage(DriverEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
