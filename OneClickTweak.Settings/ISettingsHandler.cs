namespace OneClickTweak.Settings;

public interface ISettingsHandler
{
    string Name { get; }

    IEnumerable<SettingScope> GetScopes() => AllScopes;

    bool IsVersionMatch(Setting item);
    
    public static readonly ICollection<SettingScope> AllScopes = [ SettingScope.Machine, SettingScope.CurrentUser, SettingScope.DefaultUser, SettingScope.User ];
}