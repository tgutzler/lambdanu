using LambdaNu.ViewModels;

namespace LambdaNu.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel vm)
	{
		BindingContext = vm;

		InitializeComponent();
	}
}