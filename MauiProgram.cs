using System.Runtime.CompilerServices;
using LambdaNu.Providers;

namespace LambdaNu;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.RegisterServices()
			.RegisterViews()
			.RegisterViewModels();

        return builder.Build();
	}

	private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<ISettingsService, SettingsService>();

		return builder;
	}

	private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
	{
        builder.Services.AddSingleton<Views.MainPage>();
        builder.Services.AddSingleton<Views.SettingsPage>();

		return builder;
    }

    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
	{
        builder.Services.AddSingleton<ViewModels.MainViewModel>();
        builder.Services.AddSingleton<ViewModels.SettingsViewModel>();

		return builder;
	}
}
