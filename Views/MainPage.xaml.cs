using LambdaNu.ViewModels;

namespace LambdaNu.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel vm)
	{
		BindingContext = vm;

		InitializeComponent();
	}
}

