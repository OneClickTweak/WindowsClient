using OneClickTweak.Settings.Definition;

namespace OneClickTweak.Settings.Runtime;

public class SelectedSetting
{
    public required Setting Setting { get; init; }
    
    public required SettingValue Value { get; init; }

    public override string ToString()
    {
        var name = string.Join('.', Setting.Name);
        var value = Value.Name?.Count > 0 ? string.Join('.', Value.Name) : Value.Value;
        return $"{name}: {value}";
    }
}