using System.Runtime.CompilerServices;

namespace LambdaNu.Providers;

public sealed class SettingsService : ISettingsService
{
    public event EventHandler<string>? SettingsChanged;

    public double SpeedOfLight
    {
        get => Preferences.Get(nameof(SpeedOfLight), 299792458.0);
        set
        {
            Preferences.Set(nameof(SpeedOfLight), value);
            OnSettingsChanged();
        }
    }

    private void OnSettingsChanged([CallerMemberName] string? caller = null)
        => SettingsChanged?.Invoke(this, caller ?? string.Empty);
}
