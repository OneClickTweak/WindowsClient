namespace OneClickTweak.Settings;

public interface ISettingsHandler
{
    string Name { get; }

    bool IsVersionMatch(Setting item);

    IEnumerable<SettingScope> GetScopes() => AllScopes;

    public static readonly ICollection<SettingScope> AllScopes = [ SettingScope.Machine, SettingScope.CurrentUser, SettingScope.DefaultUser, SettingScope.User ];
}