using Microsoft.Win32;
using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Runtime;
using OneClickTweak.Settings.Users;
using OneClickTweak.WindowsHandlers.Windows;

namespace OneClickTweak.WindowsHandlers.Registry;

public class RegistryHandler() : WindowsHandler("Registry")
{
    public override IEnumerable<SettingsInstance> GetFoundInstances(IEnumerable<UserInstance> users)
    {
        yield return new SettingsInstance
        {
            Scope = SettingScope.Machine,
            Path = "LOCAL_MACHINE",
            Version = Environment.OSVersion.Version.ToString()
        };

        yield return new SettingsInstance
        {
            Scope = SettingScope.DefaultUser,
            Path = "HKEY_CURRENT_USER",
            Version = Environment.OSVersion.Version.ToString()
        };

        foreach (var user in users)
        {
            yield return new SettingsInstance
            {
                Scope = user.IsCurrent ? SettingScope.CurrentUser : SettingScope.OtherUser,
                User = user,
                Path = "HKEY_CURRENT_USER",
                Version = Environment.OSVersion.Version.ToString()
            };
        }
    }

    private static IDictionary<SettingType, RegistryValueKind> RegistryTypeMap = new Dictionary<SettingType, RegistryValueKind>
    {
        { SettingType.String, RegistryValueKind.String },
        { SettingType.ExpandString, RegistryValueKind.ExpandString },
        { SettingType.MultiString, RegistryValueKind.MultiString },
        { SettingType.Bytes, RegistryValueKind.Binary },
        { SettingType.Int32, RegistryValueKind.DWord },
        { SettingType.Int64, RegistryValueKind.QWord }
    };
}