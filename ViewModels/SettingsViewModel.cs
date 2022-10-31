using System.Windows.Input;
using LambdaNu.Providers;

namespace LambdaNu.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService settingsService;
    private double speedOfLight;

    public SettingsViewModel(ISettingsService settingsService)
    {
        this.settingsService = settingsService;

        ApplyCommand = new Command((o) => ApplyExecute(o));
        RestoreSOLCommand = new Command((o) => RestoreSOL(o));

        speedOfLight = this.settingsService.SpeedOfLight;
    }

    public ICommand ApplyCommand { get; }
    public ICommand RestoreSOLCommand { get; }

    public double SpeedOfLight
    {
        get => speedOfLight;
        set
        {
            speedOfLight = value;
            OnPropertyChanged();
            settingsService.SpeedOfLight = value;
        }
    }

    private async void ApplyExecute(object o)
    {
        await Shell.Current.GoToAsync("..").ConfigureAwait(false);
    }

    private void RestoreSOL(object o)
    {
        SpeedOfLight = 299792458;
    }
}
