using Microsoft.Win32;
using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Runtime;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.WindowsHandlers;

public class RegistryHandler : WindowsHandler, ISettingsHandler
{
    public override string Name => "Registry";
    
    private IEnumerable<SettingsInstance> GetFoundInstances(IEnumerable<UserInstance> users)
    {
        yield return new SettingsInstance
        {
            Scope = SettingScope.Machine,
            Path = "LOCAL_MACHINE"
        };

        yield return new SettingsInstance
        {
            Scope = SettingScope.DefaultUser,
            Path = "HKEY_CURRENT_USER",
        };

        foreach (var user in users)
        {
            yield return new SettingsInstance
            {
                Scope = user.IsCurrent ? SettingScope.CurrentUser : SettingScope.OtherUser,
                User = user,
                Path = "HKEY_CURRENT_USER"
            };
        }
    }
    
    private static IDictionary<SettingType, RegistryValueKind> typeMap = new Dictionary<SettingType, RegistryValueKind>
    {

    };
}