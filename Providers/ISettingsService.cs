namespace LambdaNu.Providers;

public interface ISettingsService
{
    event EventHandler<string> SettingsChanged;
    double SpeedOfLight { get; set; }
}
