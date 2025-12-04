using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LambdaNu.ViewModels;

internal partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private double _speedOfLight;
    private const double c = 299792458;

    public SettingsViewModel()
    {
        ApplyCommand = new AsyncRelayCommand(Apply);
        RestoreSOLCommand = new AsyncRelayCommand(Restore);

        SpeedOfLight = Preferences.Default.Get("SpeedOfLight", c);
    }

    public ICommand ApplyCommand { get; }
    public ICommand RestoreSOLCommand { get; }

    private async Task Apply()
    {
        Preferences.Default.Set("SpeedOfLight", SpeedOfLight);
        await Shell.Current.GoToAsync("..?refresh=true");
    }

    private async Task Restore()
    {
        SpeedOfLight = c;
        Preferences.Default.Set("SpeedOfLight", SpeedOfLight);
    }
}
