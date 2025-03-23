using OneClickTweak.Settings.Definition;

namespace OneClickTweak.Settings.Runtime;

public class SelectedSetting
{
    public required Setting Setting { get; init; }
    
    public required SettingValue Value { get; init; }
    
    public string? CustomValue { get; init; }

    public override string ToString()
    {
        return $"{string.Join('.', Setting.Name)} {CustomValue ?? Value.Name}";
    }
}