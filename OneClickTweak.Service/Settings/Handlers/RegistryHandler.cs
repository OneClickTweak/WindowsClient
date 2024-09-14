using Microsoft.Win32;

namespace OneClickTweak.Service.Settings.Handlers;

public class RegistryHandler : ISettingsHandler
{
    public string Name => "Registry";

    private static IDictionary<SettingType, RegistryValueKind> typeMap = new Dictionary<SettingType, RegistryValueKind>
    {

    };
}