using Microsoft.Extensions.Logging;
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
            Section = GroupPolicySection.Machine,
            Version = Environment.OSVersion.Version.ToString()
        };

        yield return new GroupPolicyInstance
        {
            Scope = SettingScope.User,
            Section = GroupPolicySection.User,
            Version = Environment.OSVersion.Version.ToString()
        };
        //
        // foreach (var user in users)
        // {
        //     yield return new GroupPolicyInstance
        //     {
        //         Scope = SettingScope.User,
        //         User = user,
        //         Version = Environment.OSVersion.Version.ToString()
        //     };
        // }
    }
    
    public override Task<bool> Apply(SettingsInstance instance, SelectedSetting selected, ILogger logger)
    {
        return Task.FromResult(ApplySync());

        bool ApplySync()
        {
            if (instance is not GroupPolicyInstance gpInstance)
            {
                logger.LogError($"Invalid instance for {selected}");
                return false;
            }

            if (selected.Setting.Type == null || !ConversionBySettingType.TryGetValue(selected.Setting.Type.Value, out var conversion))
            {
                logger.LogError($"Undetermined type for {selected}");
                return false;
            }

            if (selected.Setting.Path == null || selected.Setting.Path.Count == 0 || selected.Setting.Path.Any(string.IsNullOrWhiteSpace))
            {
                logger.LogError($"Undetermined path for {selected}");
                return false;
            }

            if (string.IsNullOrWhiteSpace(selected.Setting.Key))
            {
                logger.LogError($"Undetermined key for {selected}");
                return false;
            }

            var keyPath = string.Join('\\', selected.Setting.Path);
            var keyName = selected.Setting.Key;
            var keyValue = selected.Value.Value;
            var converted = conversion.ConvertTo(keyValue);

            var error = ComputerGroupPolicyObject.SetPolicySetting(gpInstance.Section, keyPath, keyName, conversion.ValueKind, converted);
            if (error != null)
            {
                logger.LogError($"Error setting GPO: {error.Message}");
                return false;
                
            }

            return true;
        }
    }

    private GroupPolicySection GetSection(SettingScope scope)
    {
        return scope switch
        {
            SettingScope.Machine => GroupPolicySection.Machine,
            SettingScope.User => GroupPolicySection.User,
            _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
        };
    }
}