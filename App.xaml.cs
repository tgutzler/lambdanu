namespace LambdaNu;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());

		// Restore location; specific logic for Desktop (Windows/Mac)
		if (DeviceInfo.Current.Idiom == DeviceIdiom.Desktop)
		{
			// Set default size if nothing is saved
			const double defaultWidth = 375;
			const double defaultHeight = 570;

			// Load saved values
			window.Width = Preferences.Get("WindowWidth", defaultWidth);
			window.Height = Preferences.Get("WindowHeight", defaultHeight);
			var x = Preferences.Get("WindowX", -1d); // -1 as a flag for "not set"
			var y = Preferences.Get("WindowY", -1d);

			// Apply position (only if we have a valid saved position)
			if (x >= 0 && y >= 0)
			{
				window.X = x;
				window.Y = y;
			}
		}

		// Hook into lifecycle events to save state
		window.Destroying += (s, e) => SaveWindowData(window);
		window.Stopped += (s, e) => SaveWindowData(window);

		return window;
	}

	private void SaveWindowData(Window window)
	{
		if (DeviceInfo.Current.Idiom == DeviceIdiom.Desktop && window != null)
		{
			// Important: Don't save if minimized, otherwise it might save -32000 coords
			if (window.Width > 0 && window.Height > 0)
			{
				Preferences.Set("WindowWidth", window.Width);
				Preferences.Set("WindowHeight", window.Height);
				Preferences.Set("WindowX", window.X);
				Preferences.Set("WindowY", window.Y);
			}
		}
	}
}