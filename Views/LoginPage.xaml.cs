using ParcAuto_Web_App.ViewModels;

namespace ParcAuto_Web_App.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
