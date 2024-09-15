using Microsoft.Win32;
using OneClickTweak.Settings;

namespace OneClickTweak.WindowsHandlers;

public class RegistryHandler : WindowsHandler
{
    public override string Name => "Registry";

    private static IDictionary<SettingType, RegistryValueKind> typeMap = new Dictionary<SettingType, RegistryValueKind>
    {

    };
}