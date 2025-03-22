using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Runtime;
using OneClickTweak.Settings.Users;
using OneClickTweak.WindowsHandlers.Windows;

namespace OneClickTweak.WindowsHandlers.Registry;

public class RegistryHandler() : WindowsHandler("Registry")
{
    private const string RegistryOptionKey = "Registry";
    private const string LocationOptionKey = "Location";
    private const string HiveOptionKey = "Hive";

    public override IEnumerable<SettingsInstance> GetFoundInstances(IEnumerable<UserInstance> users)
    {
        yield return new SettingsInstance
        {
            Handler = Name,
            Scope = SettingScope.Machine,
            Version = Environment.OSVersion.Version.ToString(),
            Options = new Dictionary<string, object>
            {
                { RegistryOptionKey, "HKEY_LOCAL_MACHINE" }
            }
        };

        yield return new SettingsInstance
        {
            Handler = Name,
            Scope = SettingScope.User,
            Version = Environment.OSVersion.Version.ToString(),
            Options = new Dictionary<string, object>
            {
                { RegistryOptionKey, "HKEY_CURRENT_USER" }, // HKU:\\.Default
                { LocationOptionKey, Environment.ExpandEnvironmentVariables(@"%SYSTEMROOT%\Profiles\Default User\NTUser.dat") }
            }
        };

        foreach (var user in users)
        {
            var options = new Dictionary<string, object>
            {
                { RegistryOptionKey, "HKEY_CURRENT_USER" }
            };

            if (!user.IsCurrent)
            {
                options.Add(LocationOptionKey, Path.Join(user.LocalPath, "NTUser.dat"));
            }
            
            yield return new SettingsInstance
            {
                Handler = Name,
                Scope = SettingScope.User,
                User = user,
                Version = Environment.OSVersion.Version.ToString(),
                Options = options
            };
        }
    }

    public void AcquireInstance(ICollection<SettingsInstance> instances, ILogger logger)
    {
        var privilegesAcquired = false;
        foreach (var instance in instances.Where(x => x.Handler == Name))
        {
            if (instance.Scope == SettingScope.User && instance.Options.TryGetValue(LocationOptionKey, out var value) && value is string location)
            {
                if (!privilegesAcquired)
                {
                    // Acquires the privileges necessary for loading the hive
                    Hive.AcquirePrivileges();
                    privilegesAcquired = true;
                }

                var (hive, error) = Hive.LoadFromFile(location);
                if (error != null)
                {
                    logger.LogError(error);
                }
                else if (hive != null)
                {
                    // Loads the hive;
                    instance.Options[HiveOptionKey] = hive;
                }
            }
        }
    }

    public void ApplySettings(SettingsInstance instance, Setting setting)
    {
        if (setting.Handler != Name)
        {
            throw new InvalidOperationException("Handler mismatch");
        }

        var key = GetRegistryKey(instance);
    }

    private RegistryKey GetRegistryKey(SettingsInstance instance)
    {
        switch (instance.Scope)
        {
            case SettingScope.Machine:
                return Microsoft.Win32.Registry.LocalMachine;
            case SettingScope.User when instance.Options.TryGetValue(HiveOptionKey, out var value) && value is Hive hive && hive.RootKey != null:
                return hive.RootKey;
            case SettingScope.User:
                return Microsoft.Win32.Registry.CurrentUser;
            default:
                throw new NotImplementedException("Unknown settings scope");
        }
    }

    public void ReleaseInstance(ICollection<SettingsInstance> instances)
    {
        var privilegesAcquired = false;
        foreach (var instance in instances.Where(x => x.Handler == Name))
        {
            if (instance.Scope == SettingScope.User && instance.Options.TryGetValue(HiveOptionKey, out var value) && value is Hive hive)
            {
                // Unloads the hive
                hive.SaveAndUnload();
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