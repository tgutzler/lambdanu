namespace LambdaNu;

public partial class AppShell : Shell
{
	public AppShell()
	{
		Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));

		InitializeComponent();
	}
}
