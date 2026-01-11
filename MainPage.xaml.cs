namespace ParcAuto_Web_App;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		UpdateWelcomeMessage();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UpdateWelcomeMessage();
	}

	private void UpdateWelcomeMessage()
	{
		var userEmail = Preferences.Default.Get("user_email", "");
		
		if (!string.IsNullOrEmpty(userEmail))
		{
			WelcomeLabel.Text = $"Bun venit, {userEmail.Split('@')[0]}!";
			UserEmailLabel.Text = $"Conectat ca: {userEmail}";
		}
		else
		{
			WelcomeLabel.Text = "Bun venit!";
			UserEmailLabel.Text = "Vă rugăm să vă autentificați";
		}
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}
