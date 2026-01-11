using ParcAuto_Web_App.ViewModels;

namespace ParcAuto_Web_App.Views;

public partial class VehicleListPage : ContentPage
{
    private readonly VehicleListViewModel _viewModel;

    public VehicleListPage(VehicleListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
