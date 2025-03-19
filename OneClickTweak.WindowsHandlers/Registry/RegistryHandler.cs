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

    public static SettingType GetSettingType(string key)
    {
        var registryType = (RegistryValueKind)Enum.Parse(typeof(RegistryValueKind), key, false);
        return RegistryTypeMap[registryType];
    }

    private static readonly IDictionary<RegistryValueKind, SettingType> RegistryTypeMap = new Dictionary<RegistryValueKind, SettingType>
    {
        { RegistryValueKind.String, SettingType.String },
        { RegistryValueKind.ExpandString, SettingType.ExpandString },
        { RegistryValueKind.MultiString, SettingType.MultiString },
        { RegistryValueKind.Binary, SettingType.Bytes },
        { RegistryValueKind.DWord, SettingType.Int32 },
        { RegistryValueKind.QWord, SettingType.Int64 }
    };
}