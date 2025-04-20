using OneClickTweak.Settings.Runtime;

namespace OneClickTweak.WindowsHandlers.GroupPolicy;

public class GroupPolicyInstance : SettingsInstance
{
    public required GroupPolicySection Section { get; init; }
    
    public IGroupPolicyObject? GroupPolicy { get; init; }
}