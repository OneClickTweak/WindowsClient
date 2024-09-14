using Microsoft.Win32;
using OneClickTweak.Settings;

namespace OneClickTweak.WindowsHandlers;

public class RegistryHandler : ISettingsHandler
{
    public string Name => "Registry";

    private static IDictionary<SettingType, RegistryValueKind> typeMap = new Dictionary<SettingType, RegistryValueKind>
    {

    };
}