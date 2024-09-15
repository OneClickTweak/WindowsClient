namespace OneClickTweak.Settings.Services;

public class SettingsParser(SettingsHandlerCollection settingsHandlers)
{
    public IEnumerable<SettingScope> GetScopes(SettingDefinition definition)
    {
        return EnumerateSettings(definition).Where(x => x.Scope != null).Select(x => x.Scope!.Value).Distinct().OrderBy(x => x);
    }

    public IEnumerable<Setting> GetSettings(ICollection<Setting>? source, string? handler)
    {
        if (source == null)
        {
            yield break;
        }

        foreach (var item in source)
        {
            if (!IsSupportedPlatform(item.Platform))
            {
                continue;
            }

            handler ??= item.Handler;
            if (handler != null)
            {
                var instance = settingsHandlers.GetHandler(handler);
                if (instance != null && !instance.IsVersionMatch(item))
                {
                    
                }
            }

            foreach (var subItem in GetSettings(item.Settings, handler))
            {
                yield return subItem;
            }
        }
    }

    private static IEnumerable<Setting> EnumerateSettings(IHasSettings source)
    {
        if (source.Settings == null)
        {
            yield break;
        }

        foreach (var item in source.Settings)
        {
            yield return item;
            foreach (var subItem in EnumerateSettings(item))
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