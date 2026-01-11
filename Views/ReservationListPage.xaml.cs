using ParcAuto_Web_App.ViewModels;

namespace ParcAuto_Web_App.Views;

public partial class ReservationListPage : ContentPage
{
    private readonly ReservationListViewModel _viewModel;

    public ReservationListPage(ReservationListViewModel viewModel)
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
