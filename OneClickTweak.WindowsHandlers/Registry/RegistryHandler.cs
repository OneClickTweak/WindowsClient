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

    public bool ApplySettings(SettingsInstance instance, SelectedSetting selected, ILogger logger)
    {
        if (instance is not RegistryInstance registryInstance)
        {
            logger.LogError($"Invalid instance for {selected}");
            return false;
        }

        var key = GetRegistryKey(registryInstance);
        if (key == null)
        {
            logger.LogError($"Unknown registry root key for {selected}");
            return false;
        }

        if (selected.Setting.Type == null || !ConversionBySettingType.TryGetValue(selected.Setting.Type.Value, out var keyType))
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
        var conversion = ConversionBySettingType[selected.Setting.Type.Value];
        var keyValue = selected.Value.Value;
        var converted = conversion.ConvertTo(keyValue);

        var subKey = key.OpenSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
        if (subKey == null)
        {
            logger.LogError($"No permissions to access {selected}");
            return false;
        }

        if (converted == null)
        {
            subKey.DeleteValue(keyName);
        }
        else
        {
            subKey.SetValue(keyName, converted, conversion.ValueKind);
        }

        return true;
    }

    private RegistryKey? GetRegistryKey(RegistryInstance instance)
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
                return null;
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
        return ConversionByValueKind[registryType].SettingType;
    }

    private static readonly List<RegistryConversion> Conversions =
    [
        new(RegistryValueKind.String, SettingType.String, x => x, Convert.ToString),
        new(RegistryValueKind.ExpandString, SettingType.ExpandString, x => x, Convert.ToString),
        new(RegistryValueKind.MultiString, SettingType.MultiString, x => x, Convert.ToString),
        new(RegistryValueKind.Binary, SettingType.Bytes, x => x == null ? null : Convert.FromBase64String(x), x => x == null ? null : Convert.ToBase64String((byte[])x)),
        new(RegistryValueKind.DWord, SettingType.Int32, x => x == null ? null : Convert.ToInt32(x), x => x == null ? null : Convert.ToInt32(x).ToString()),
        new(RegistryValueKind.QWord, SettingType.Int64, x => x == null ? null : Convert.ToInt64(x), x => x == null ? null : Convert.ToInt64(x).ToString())
    ];
    
    private static readonly IDictionary<RegistryValueKind, RegistryConversion> ConversionByValueKind = Conversions.ToDictionary(x => x.ValueKind, x => x);
    
    private static readonly Dictionary<SettingType, RegistryConversion> ConversionBySettingType = Conversions.ToDictionary(x => x.SettingType);
}