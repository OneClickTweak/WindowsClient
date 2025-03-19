using OneClickTweak.Settings.Definition;
using OneClickTweak.WindowsHandlers.Registry;

namespace OneClickTweak.Tests.WinUtilImport;

public class WinUtilConverter
{
    public bool CanConvert(string key, WinUtilEntry entry)
    {
        return entry.Registry.Any() && GetName(key).Length > 0;
    }

    public Setting Convert(string key, WinUtilEntry entry)
    {
        return new Setting
        {
            Name = GetName(key),
            Tags = entry.Description == null ? [entry.Category] : [entry.Category, entry.Description],
            Handler = "Registry",
            Settings = entry.Registry.Select(r => new Setting
            {
                Name = [r.Name],
                Path = r.Path.Split(['\\', ':'], StringSplitOptions.RemoveEmptyEntries),
                Type = RegistryHandler.GetSettingType(r.Type),
                Key = r.Name,
                Values = GetValues(r).ToList()
            }).ToList()
        };
    }

    private IEnumerable<SettingValue> GetValues(WinUtilRegistry registry)
    {
        if (registry.OriginalValue != null)
        {
            yield return new SettingValue
            {
                Value = registry.OriginalValue == "<RemoveEntry>" ? null : registry.OriginalValue,
                IsDefault = registry.DefaultState != "true"
            };
        }

        yield return new SettingValue
        {
            Value = registry.Value,
            IsDefault = registry.DefaultState == "true"
        };
    }

    private string[] GetName(string key)
    {
        if (key.StartsWith("WPFTweaks"))
        {
            return ["Windows", "Tweak", key.Substring("WPFTweaks".Length)];
        }

        if (key.StartsWith("WPFToggle"))
        {
            return ["Windows", "Toggle", key.Substring("WPFToggle".Length)];
        }

        return [];
    }
}