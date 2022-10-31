using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LambdaNu.Providers;
using LambdaNu.Views;

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

public class MainViewModel : ViewModelBase
{
    private readonly ISettingsService settingsService;
    private double result;
    private double input;
    private UnitType toUnit;
    private UnitType fromUnit;
    private string bwMode = "";
    private double bwDelta;
    private UnitType bwDeltaUnit;
    private double bwResult;
    private double bwResultLower;
    private double bwResultUpper;
    private UnitType bwUnit;

    public MainViewModel(ISettingsService settingsService)
    {
        this.settingsService = settingsService;
        this.settingsService.SettingsChanged += this.SettingsService_SettingsChanged;

        Units = Enum.GetValues(typeof(UnitType)).Cast<UnitType>().ToList().AsReadOnly();
        BwModes = new List<string> { "+", "-", "\u00B1" }.AsReadOnly();

        ConfigCommand = new Command((o) => ShowConfigExecute(o));

        Input = 193;
        FromUnit = UnitType.THz;
        ToUnit = UnitType.nm;
        BwMode = "+";
        BwDelta = 1;
        BwDeltaUnit = UnitType.GHz;
        BwUnit = UnitType.pm;
    }

    private void SettingsService_SettingsChanged(object? sender, string e)
    {
        if (e == nameof(ISettingsService.SpeedOfLight))
        {
            Calculate();
            CalculateBw();
        }
    }

    public ICommand ConfigCommand { get; }
    public ReadOnlyCollection<UnitType> Units { get; }
    public UnitType FromUnit
    {
        get => fromUnit;
        set
        {
            fromUnit = value;
            Calculate();
            CalculateBw();
        }
    }
    public UnitType ToUnit
    {
        get => toUnit;
        set
        {
            toUnit = value;
            Calculate();
            CalculateBw();
        }
    }
    public double Input
    {
        get => input;
        set
        {
            input = value;
            Calculate();
            CalculateBw();
        }
    }
    public double Result
    {
        get => result;
        private set
        {
            result = value;
            OnPropertyChanged();
        }
    }
    public ReadOnlyCollection<string> BwModes { get; }
    public string BwMode
    {
        get => bwMode;
        set
        {
            bwMode = value;
            CalculateBw();
        }
    }
    public double BwDelta
    {
        get => bwDelta;
        set
        {
            bwDelta = value;
            CalculateBw();
        }
    }

    public UnitType BwDeltaUnit
    {
        get => bwDeltaUnit;
        set
        {
            bwDeltaUnit = value;
            CalculateBw();
        }
    }
    public double BwResult
    {
        get => bwResult;
        set
        {
            bwResult = value;
            OnPropertyChanged();
        }
    }
    public double BwResultLower
    {
        get => bwResultLower;
        set
        {
            bwResultLower = value;
            OnPropertyChanged();
        }
    }
    public double BwResultUpper
    {
        get => bwResultUpper;
        set
        {
            bwResultUpper = value;
            OnPropertyChanged();
        }
    }
    public UnitType BwUnit
    {
        get => bwUnit;
        set
        {
            bwUnit = value;
            CalculateBw();
        }
    }

    private void Calculate()
    {
        Result = Convert(Input, FromUnit, ToUnit);
    }

    private void CalculateBw()
    {
        var input = Convert(Input, FromUnit, BwDeltaUnit);
        double lower;
        double upper;
        if ((BwMode == "+" && fromUnit.IsM() && bwDeltaUnit.IsM())
            || (BwMode == "+" && fromUnit.IsHz() && bwDeltaUnit.IsHz()))
        {
            lower = Convert(input, bwDeltaUnit, bwUnit);
            upper = Convert(input + bwDelta, bwDeltaUnit, bwUnit);
        }
        else if ((BwMode == "-" && fromUnit.IsM() && bwDeltaUnit.IsHz())
            || (BwMode == "-" && fromUnit.IsHz() && bwDeltaUnit.IsM()))
        {
            upper = Convert(input, bwDeltaUnit, bwUnit);
            lower = Convert(input + bwDelta, bwDeltaUnit, bwUnit);
        }
        else if ((BwMode == "-" && fromUnit.IsM() && bwDeltaUnit.IsM())
            || (BwMode == "-" && fromUnit.IsHz() && bwDeltaUnit.IsHz()))
        {
            lower = Convert(input - BwDelta, BwDeltaUnit, BwUnit);
            upper = Convert(input, BwDeltaUnit, BwUnit);
        }
        else if ((BwMode == "+" && fromUnit.IsM() && bwDeltaUnit.IsHz())
            || (BwMode == "+" && fromUnit.IsHz() && bwDeltaUnit.IsM()))
        {
            upper = Convert(input - BwDelta, BwDeltaUnit, BwUnit);
            lower = Convert(input, BwDeltaUnit, BwUnit);
        }
        else
        {
            lower = Convert(input - BwDelta, BwDeltaUnit, BwUnit);
            upper = Convert(input + bwDelta, bwDeltaUnit, bwUnit);
            if (lower > upper)
            {
                (lower, upper) = (upper, lower);
            }
        }

        BwResultLower = Convert(lower, bwUnit, fromUnit);
        BwResultUpper = Convert(upper, bwUnit, fromUnit);
        BwResult = Math.Abs(upper - lower);
    }

    private double Convert(double input, UnitType fromUnit, UnitType toUnit)
    {
        var speedOfLight = settingsService.SpeedOfLight;
        var inputAsThz = fromUnit switch
        {
            UnitType.Hz  => input / 1e12,
            UnitType.kHz => input / 1e9,
            UnitType.MHz => input / 1e6,
            UnitType.GHz => input / 1e3,
            UnitType.THz => input,
            UnitType.m   => speedOfLight / input / 1e12,
            UnitType.mm  => speedOfLight / input / 1e9,
            UnitType.um  => speedOfLight / input / 1e6,
            UnitType.nm  => speedOfLight / input / 1e3,
            UnitType.pm  => speedOfLight / input,
            _ => double.NaN,
        };

        return toUnit switch
        {
            UnitType.Hz  => inputAsThz * 1e12,
            UnitType.kHz => inputAsThz * 1e9,
            UnitType.MHz => inputAsThz * 1e6,
            UnitType.GHz => inputAsThz * 1e3,
            UnitType.THz => inputAsThz * 1,
            UnitType.m   => speedOfLight / inputAsThz / 1e12,
            UnitType.mm  => speedOfLight / inputAsThz / 1e9,
            UnitType.um  => speedOfLight / inputAsThz / 1e6,
            UnitType.nm  => speedOfLight / inputAsThz / 1e3,
            UnitType.pm  => speedOfLight / inputAsThz,
            _ => double.NaN,
        };
    }

    private async void ShowConfigExecute(object o)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage)).ConfigureAwait(false);
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