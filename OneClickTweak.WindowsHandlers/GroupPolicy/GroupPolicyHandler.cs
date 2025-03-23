using Microsoft.Win32;
using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Runtime;
using OneClickTweak.Settings.Users;
using OneClickTweak.WindowsHandlers.Windows;

namespace OneClickTweak.WindowsHandlers.GroupPolicy;

public class GroupPolicyHandler() : WindowsHandler("GPO")
{
    public override IEnumerable<SettingsInstance> GetFoundInstances(IEnumerable<UserInstance> users)
    {
        yield return new GroupPolicyInstance
        {
            Scope = SettingScope.Machine,
            Version = Environment.OSVersion.Version.ToString()
        };

        yield return new GroupPolicyInstance
        {
            Scope = SettingScope.User,
            Version = Environment.OSVersion.Version.ToString()
        };

        foreach (var user in users)
        {
            yield return new GroupPolicyInstance
            {
                Scope = SettingScope.User,
                User = user,
                Version = Environment.OSVersion.Version.ToString()
            };
        }
    }

    public void SetValue()
    {
        ComputerGroupPolicyObject.SetPolicySetting(@"HKLM\Software\Policies\Microsoft\Windows\HomeGroup!DisableHomeGroup", "0", RegistryValueKind.DWord);
    }
}