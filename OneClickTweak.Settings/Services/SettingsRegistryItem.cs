using OneClickTweak.Settings.Runtime;

namespace OneClickTweak.Settings.Services;

public record SettingsRegistryItem(Type Type, Func<ISettingsHandler> CreateAction, Action<object>? ConfigureAction);