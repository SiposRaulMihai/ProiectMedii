using Microsoft.Extensions.Logging;
using ParcAuto_Web_App.Services;
using ParcAuto_Web_App.ViewModels;
using ParcAuto_Web_App.Views;
using Plugin.LocalNotification;

namespace ParcAuto_Web_App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseLocalNotification()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register Services
		builder.Services.AddSingleton<HttpClient>();
		builder.Services.AddSingleton<ApiService>();
		builder.Services.AddSingleton<NotificationService>();
		builder.Services.AddSingleton<AppShell>();

		// Register ViewModels (transient)
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<VehicleListViewModel>();
		builder.Services.AddTransient<VehicleDetailsViewModel>();
		builder.Services.AddTransient<VehicleEditViewModel>();
		builder.Services.AddTransient<ReservationListViewModel>();
		builder.Services.AddTransient<ReservationDetailsViewModel>();
		builder.Services.AddTransient<ReservationEditViewModel>();
		builder.Services.AddTransient<DriverListViewModel>();
		builder.Services.AddTransient<DriverEditViewModel>();

		// Register Views (transient)
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<VehicleListPage>();
		builder.Services.AddTransient<VehicleDetailsPage>();
		builder.Services.AddTransient<VehicleEditPage>();
		builder.Services.AddTransient<ReservationListPage>();
		builder.Services.AddTransient<ReservationDetailsPage>();
		builder.Services.AddTransient<ReservationEditPage>();
		builder.Services.AddTransient<DriverListPage>();
		builder.Services.AddTransient<DriverEditPage>();

		return builder.Build();
	}
}
