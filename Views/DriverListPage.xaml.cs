using ParcAuto_Web_App.ViewModels;

namespace ParcAuto_Web_App.Views;

public partial class DriverListPage : ContentPage
{
    private readonly DriverListViewModel _viewModel;

    public DriverListPage(DriverListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDriversAsync();
    }
}
