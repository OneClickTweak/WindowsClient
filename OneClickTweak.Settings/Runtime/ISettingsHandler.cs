using OneClickTweak.Settings.Definition;

namespace OneClickTweak.Settings.Runtime;

public interface ISettingsHandler
{
    string Name { get; }
    
    int ChangeTimeout { get; set; }

    bool IsVersionMatch(Setting item);

    IEnumerable<SettingScope> GetScopes() => AllScopes;

    IEnumerable<string> GetHashBase(Setting setting)
    {
        if (setting.Settings?.Any() == true)
        {
            throw new ArgumentException($"Hash can be only calculated on a final, flattened setting");
        }

        if (setting.Path?.Count == 0 && string.IsNullOrWhiteSpace(setting.Key))
        {
            throw new ArgumentException($"Cannot calculate hash because path and key are empty");
        }

        if (setting.Path?.Any() == true)
        {
            foreach (var part in setting.Path)
            {
                yield return part;
            }
        }

        if (setting.Key != null)
        {
            yield return setting.Key;
        }
    }

    public static readonly ICollection<SettingScope> AllScopes = [ SettingScope.Machine, SettingScope.CurrentUser, SettingScope.DefaultUser, SettingScope.OtherUser ];
}