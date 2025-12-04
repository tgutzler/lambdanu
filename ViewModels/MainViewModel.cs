using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
namespace LambdaNu.ViewModels;

public enum UnitType
{
    Hz,
    kHz,
    MHz,
    GHz,
    THz,
    m,
    mm,
    um,
    nm,
    pm
}

internal partial class MainViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private double _input;
    [ObservableProperty]
    private UnitType _fromUnit;
    [ObservableProperty]
    private UnitType _toUnit;
    [ObservableProperty]
    private double _result;
    [ObservableProperty]
    private string _bwMode = "";
    [ObservableProperty]
    private double _bwDelta;
    [ObservableProperty]
    private UnitType _bwDeltaUnit;
    [ObservableProperty]
    private double _bwResult;
    [ObservableProperty]
    private double _bwResultLower;
    [ObservableProperty]
    private double _bwResultUpper;
    [ObservableProperty]
    private UnitType _bwUnit;

    public MainViewModel()
    {
        BwModes = new List<string> { "+", "-", "\u00B1" }.AsReadOnly();
        Units = new ObservableCollection<UnitType>(Enum.GetValues<UnitType>());

        ConfigCommand = new AsyncRelayCommand(ConfigAsync);

        Input = 193.4;
        FromUnit = UnitType.THz;
        ToUnit = UnitType.nm;
        BwMode = "+";
        BwDelta = 1;
        BwDeltaUnit = UnitType.GHz;
        BwUnit = UnitType.pm;
    }

    public ICommand ConfigCommand { get; }
    public ObservableCollection<UnitType> Units { get; }
    public ReadOnlyCollection<string> BwModes { get; }

    partial void OnInputChanged(double value) => Calculate();
    partial void OnFromUnitChanged(UnitType value) => Calculate();
    partial void OnToUnitChanged(UnitType value) => Calculate();
    partial void OnBwDeltaChanged(double value) => Calculate();
    partial void OnBwModeChanged(string value) => Calculate();
    partial void OnBwDeltaUnitChanged(UnitType value) => Calculate();
    partial void OnBwUnitChanged(UnitType value) => Calculate();

    private void Calculate()
    {
        Result = Convert(Input, FromUnit, ToUnit);
        var input = Convert(Input, FromUnit, BwDeltaUnit);
        double lower;
        double upper;
        if ((BwMode == "+" && FromUnit.IsM() && BwDeltaUnit.IsM())
            || (BwMode == "+" && FromUnit.IsHz() && BwDeltaUnit.IsHz()))
        {
            lower = Convert(input, BwDeltaUnit, BwUnit);
            upper = Convert(input + BwDelta, BwDeltaUnit, BwUnit);
        }
        else if ((BwMode == "-" && FromUnit.IsM() && BwDeltaUnit.IsHz())
            || (BwMode == "-" && FromUnit.IsHz() && BwDeltaUnit.IsM()))
        {
            upper = Convert(input, BwDeltaUnit, BwUnit);
            lower = Convert(input + BwDelta, BwDeltaUnit, BwUnit);
        }
        else if ((BwMode == "-" && FromUnit.IsM() && BwDeltaUnit.IsM())
            || (BwMode == "-" && FromUnit.IsHz() && BwDeltaUnit.IsHz()))
        {
            lower = Convert(input - BwDelta, BwDeltaUnit, BwUnit);
            upper = Convert(input, BwDeltaUnit, BwUnit);
        }
        else if ((BwMode == "+" && FromUnit.IsM() && BwDeltaUnit.IsHz())
            || (BwMode == "+" && FromUnit.IsHz() && BwDeltaUnit.IsM()))
        {
            upper = Convert(input - BwDelta, BwDeltaUnit, BwUnit);
            lower = Convert(input, BwDeltaUnit, BwUnit);
        }
        else
        {
            lower = Convert(input - BwDelta, BwDeltaUnit, BwUnit);
            upper = Convert(input + BwDelta, BwDeltaUnit, BwUnit);
            if (lower > upper)
            {
                (lower, upper) = (upper, lower);
            }
        }

        BwResultLower = Convert(lower, BwUnit, FromUnit);
        BwResultUpper = Convert(upper, BwUnit, FromUnit);
        BwResult = Math.Abs(upper - lower);
    }

    private double Convert(double input, UnitType fromUnit, UnitType toUnit)
    {
        var speedOfLight = Preferences.Default.Get("SpeedOfLight", 299792458);
        var inputAsThz = fromUnit switch
        {
            UnitType.Hz => input / 1e12,
            UnitType.kHz => input / 1e9,
            UnitType.MHz => input / 1e6,
            UnitType.GHz => input / 1e3,
            UnitType.THz => input,
            UnitType.m => speedOfLight / input / 1e12,
            UnitType.mm => speedOfLight / input / 1e9,
            UnitType.um => speedOfLight / input / 1e6,
            UnitType.nm => speedOfLight / input / 1e3,
            UnitType.pm => speedOfLight / input,
            _ => double.NaN,
        };

        return toUnit switch
        {
            UnitType.Hz => inputAsThz * 1e12,
            UnitType.kHz => inputAsThz * 1e9,
            UnitType.MHz => inputAsThz * 1e6,
            UnitType.GHz => inputAsThz * 1e3,
            UnitType.THz => inputAsThz * 1,
            UnitType.m => speedOfLight / inputAsThz / 1e12,
            UnitType.mm => speedOfLight / inputAsThz / 1e9,
            UnitType.um => speedOfLight / inputAsThz / 1e6,
            UnitType.nm => speedOfLight / inputAsThz / 1e3,
            UnitType.pm => speedOfLight / inputAsThz,
            _ => double.NaN,
        };
    }

    private async Task ConfigAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.SettingsPage));
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("refresh"))
            Calculate();

        query.Clear();
    }
}

internal static class Extensions
{
    public static bool IsM(this UnitType unit)
        => unit switch
        {
            UnitType.mm => true,
            UnitType.um => true,
            UnitType.nm => true,
            UnitType.pm => true,
            _ => false
        };

    public static bool IsHz(this UnitType unit)
        => unit switch
        {
            UnitType.Hz => true,
            UnitType.kHz => true,
            UnitType.MHz => true,
            UnitType.GHz => true,
            UnitType.THz => true,
            _ => false
        };
}