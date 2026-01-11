using ParcAuto_Web_App.Services;
using ParcAuto_Web_App.Views;

namespace ParcAuto_Web_App;

public partial class AppShell : Shell
{
	private readonly ApiService _apiService;

	public AppShell(ApiService apiService)
	{
		InitializeComponent();
		_apiService = apiService;
		
		// Register routes for navigation
		Routing.RegisterRoute("login", typeof(LoginPage));
		Routing.RegisterRoute("vehicleDetails", typeof(VehicleDetailsPage));
		Routing.RegisterRoute("vehicleEdit", typeof(VehicleEditPage));
		Routing.RegisterRoute("driverEdit", typeof(DriverEditPage));
		Routing.RegisterRoute("reservationDetails", typeof(ReservationDetailsPage));
		Routing.RegisterRoute("reservationEdit", typeof(ReservationEditPage));
		
		// Update user email in header
		UpdateUserEmail();
	}

	private void UpdateUserEmail()
	{
		var userEmail = Preferences.Default.Get("user_email", "user@example.com");
		UserEmailLabel.Text = userEmail;
	}

	private async void OnLogoutClicked(object sender, EventArgs e)
	{
		// Clear the token from secure storage
		await _apiService.ClearTokenAsync();
		
		// Clear login state and user preferences
		Preferences.Default.Clear();

		// Navigate to login page
		var loginPage = Handler?.MauiContext?.Services.GetService<LoginPage>();
		if (loginPage != null)
		{
			SetRootPage(loginPage);
		}
	}

	private static void SetRootPage(Page page)
	{
		var app = Application.Current;
		if (app != null && app.Windows.Count > 0)
		{
			app.Windows[0].Page = page;
		}
	}
}

