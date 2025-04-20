using Microsoft.Win32;
using OneClickTweak.Handlers;
using OneClickTweak.Settings.Definition;
using OneClickTweak.WindowsHandlers.Registry;

namespace OneClickTweak.WindowsHandlers.Windows;

public abstract class WindowsHandler(string? name) : BaseHandler(name)
{
    public static SettingType GetSettingType(string key)
    {
        var registryType = (RegistryValueKind)Enum.Parse(typeof(RegistryValueKind), key, false);
        return ConversionByValueKind[registryType].SettingType;
    }

    private static readonly List<RegistryConversion> Conversions =
    [
        new(RegistryValueKind.String, SettingType.String, x => x, Convert.ToString),
        new(RegistryValueKind.ExpandString, SettingType.ExpandString, x => x, Convert.ToString),
        new(RegistryValueKind.MultiString, SettingType.MultiString, x => x, Convert.ToString),
        new(RegistryValueKind.Binary, SettingType.Bytes, x => x == null ? null : Convert.FromBase64String(x), x => x == null ? null : Convert.ToBase64String((byte[])x)),
        new(RegistryValueKind.DWord, SettingType.Int32, x => x == null ? null : Convert.ToInt32(x), x => x == null ? null : Convert.ToInt32(x).ToString()),
        new(RegistryValueKind.QWord, SettingType.Int64, x => x == null ? null : Convert.ToInt64(x), x => x == null ? null : Convert.ToInt64(x).ToString())
    ];
    
    private static readonly IDictionary<RegistryValueKind, RegistryConversion> ConversionByValueKind = Conversions.ToDictionary(x => x.ValueKind, x => x);

    protected static readonly Dictionary<SettingType, RegistryConversion> ConversionBySettingType = Conversions.ToDictionary(x => x.SettingType);
}