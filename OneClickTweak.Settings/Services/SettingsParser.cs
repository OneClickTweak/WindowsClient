using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Runtime;

namespace OneClickTweak.Settings.Services;

public class SettingsParser(SettingsHandlerCollection settingsHandlers)
{
    public IEnumerable<SettingScope> GetScopes(ICollection<Setting> settings)
    {
        return settings.SelectMany(EnumerateDeep).Where(x => x.Scope != null).Select(x => x.Scope!.Value).Distinct().OrderBy(x => x);
    }

    public ISettingsHandler? GetHandler(Setting settings)
    {
        var handlers = EnumerateDeep(settings).Where(x => x.Handler != null).Select(x => x.Handler!).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (handlers.Count == 1)
        {
            return settingsHandlers.GetHandler(handlers.Single());
        }

        return null;
    }

    public IEnumerable<Setting> FlattenSettings(Setting source, SettingsHandlerCollection handlers, Setting? current = null)
    {
        current ??= Clone(source);
        if (source.Settings?.Any() != true)
        {
            yield return current;
            yield break;
        }

        foreach (var item in source.Settings)
        {
            var copy = Clone(item);
            AppendTo(current, copy);
            if (item.Settings != null)
            {
                foreach (var subItem in FlattenSettings(item, handlers, copy))
                {
                    yield return subItem;
                }
            }
            else
            {
                yield return copy;
            }
        }
    }

    private Setting Clone(Setting setting)
    {
        return setting with
        {
            Settings = null,
            Path = setting.Path?.ToList(),
            Values = setting.Values?.Select(x => x with { }).ToList(),
            Options = setting.Options.ToDictionary(StringComparer.OrdinalIgnoreCase)
        };
    }

    private void AppendTo(Setting source, Setting destination)
    {
        destination.Platform ??= source.Platform;
        destination.Name = source.Name.Concat(destination.Name).ToArray();
        destination.Tags = (source.Tags ?? []).Concat(destination.Tags ?? []).ToHashSet();
        destination.Handler ??= source.Handler;
        destination.Scope ??= source.Scope;
        destination.Path ??= source.Path?.ToList();
        destination.Key ??= source.Key;
        destination.Type ??= source.Type;
        destination.MinVersion ??= source.MinVersion;
        destination.MaxVersion ??= source.MaxVersion;
        destination.Values ??= source.Values?.ToList();
        foreach (var option in source.Options)
        {
            if (!destination.Options.TryGetValue(option.Key, out var optionValue) || string.IsNullOrEmpty(optionValue))
            {
                destination.Options[option.Key] = option.Value;
            }
        }
    }

    private static IEnumerable<Setting> EnumerateDeep(Setting source)
    {
        yield return source;
        if (source.Settings == null)
        {
            yield break;
        }

        foreach (var item in source.Settings)
        {
            yield return item;
            foreach (var subItem in EnumerateDeep(item))
            {
                yield return subItem;
            }
        }
    }

    private static bool IsSupportedPlatform(SupportedPlatform? platform)
    {
        if (platform is null or SupportedPlatform.Any)
        {
            return true;
        }

        if (platform.Value.HasFlag(SupportedPlatform.Windows) && OperatingSystem.IsWindows())
        {
            return true;
        }

        return false;
    }


}