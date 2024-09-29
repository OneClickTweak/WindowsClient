namespace OneClickTweak.Settings.Services;

public record SettingsRegistryItem(Type Type, Action<object>? ConfigurationAction);