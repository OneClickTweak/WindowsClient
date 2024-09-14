using Microsoft.Win32;
using OneClickTweak.Service.Settings.Handlers.GroupPolicy;

namespace OneClickTweak.Service.Settings.Handlers;

public class GroupPolicyHandler : ISettingsHandler
{
    public string Name => "GPO";

    public void SetValue()
    {
        ComputerGroupPolicyObject.SetPolicySetting(@"HKLM\Software\Policies\Microsoft\Windows\HomeGroup!DisableHomeGroup", "0", RegistryValueKind.DWord);
    }
}