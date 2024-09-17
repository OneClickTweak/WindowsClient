namespace OneClickTweak.Settings.Services;

public class SettingsParser(SettingsHandlerCollection settingsHandlers)
{
    public IEnumerable<SettingScope> GetScopes(IHasSettings settings)
    {
        return EnumerateSettings(settings).Where(x => x.Scope != null).Select(x => x.Scope!.Value).Distinct().OrderBy(x => x);
    }

    public ISettingsHandler? GetHandler(IHasSettings settings)
    {
        var handlers = EnumerateSettings(settings).Where(x => x.Handler != null).Select(x => x.Handler!).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (handlers.Count == 1)
        {
            return settingsHandlers.GetHandler(handlers.Single());
        }

        return null;
    }

    public IEnumerable<Setting> FlattenSettings(IHasSettings source, SettingsHandlerCollection handlers, Setting? parent)
    {
        if (source.Settings?.Any() != true)
        {
            yield break;
        }

        foreach (var item in source.Settings!)
        {
            var current = item.CreateCopy();
            if (parent != null)
            {
                current.Merge(parent);
            }

            if (item.Settings != null)
            {
                foreach (var subItem in FlattenSettings(item, handlers, current))
                {
                    yield return subItem;
                }
            }
            else
            {
                yield return current;
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