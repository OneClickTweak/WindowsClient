using Microsoft.Win32;
using OneClickTweak.Settings;

namespace OneClickTweak.WindowsHandlers;

public class RegistryHandler : WindowsHandler, ISettingsHandler
{
    public override string Name => "Registry";

    private static IDictionary<SettingType, RegistryValueKind> typeMap = new Dictionary<SettingType, RegistryValueKind>
    {

    };
}