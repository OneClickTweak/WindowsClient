namespace OneClickTweak.Settings;

public interface ISettingsHandler
{
    string Name { get; }

    bool IsVersionMatch(Setting item);
    
//    IEnumerable<SettingsInstance> GetInstances();
}