using Microsoft.Extensions.Logging;
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
        yield return new RegistryInstance
        {
            Scope = SettingScope.Machine,
            Version = Environment.OSVersion.Version.ToString(),
            RootKey = "HKEY_LOCAL_MACHINE"
        };

        yield return new RegistryInstance
        {
            Scope = SettingScope.User,
            Version = Environment.OSVersion.Version.ToString(),
            RootKey = "HKEY_CURRENT_USER",
            Location = Environment.ExpandEnvironmentVariables(@"%SYSTEMROOT%\Profiles\Default User\NTUser.dat")
        };

        foreach (var user in users)
        {
            yield return new RegistryInstance
            {
                Scope = SettingScope.User,
                User = user,
                Version = Environment.OSVersion.Version.ToString(),
                RootKey = "HKEY_CURRENT_USER",
                Location = user.IsCurrent ? null : Path.Join(user.LocalPath, "NTUser.dat")
            };
        }
    }

    public void AcquireInstance(ICollection<SettingsInstance> instances, ILogger logger)
    {
        var privilegesAcquired = false;
        foreach (var instance in instances.OfType<RegistryInstance>())
        {
            if (instance.Scope == SettingScope.User && instance.Location != null)
            {
                if (!privilegesAcquired)
                {
                    // Acquires the privileges necessary for loading the hive
                    Hive.AcquirePrivileges();
                    privilegesAcquired = true;
                }

                // Loads the hive;
                var (hive, error) = Hive.LoadFromFile(instance.Location);
                if (error != null)
                {
                    logger.LogError(error);
                }
                else if (hive != null)
                {
                    instance.Hive = hive;
                }
            }
        }
    }

    public void ApplySettings(SettingsInstance instance, Setting setting)
    {
        if (instance is RegistryInstance registryInstance)
        {
            var key = GetRegistryKey(registryInstance);
        }
    }

    private RegistryKey GetRegistryKey(RegistryInstance instance)
    {
        switch (instance.Scope)
        {
            case SettingScope.Machine:
                return Microsoft.Win32.Registry.LocalMachine;
            case SettingScope.User when instance.Hive?.RootKey != null:
                return instance.Hive.RootKey;
            case SettingScope.User:
                return Microsoft.Win32.Registry.CurrentUser;
            default:
                throw new NotImplementedException("Unknown settings scope");
        }
    }

    public void ReleaseInstance(ICollection<SettingsInstance> instances)
    {
        var privilegesAcquired = false;
        foreach (var instance in instances.OfType<RegistryInstance>())
        {
            if (instance.Scope == SettingScope.User && instance.Hive != null)
            {
                // Unloads the hive
                instance.Hive.SaveAndUnload();
                instance.Hive = null;
                privilegesAcquired = true;
            }
        }

        if (privilegesAcquired)
        {
            // De-elevate back to normal privileges
            Hive.ReturnPrivileges();
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