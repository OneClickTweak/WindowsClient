using OneClickTweak.Settings.Runtime;

namespace OneClickTweak.Settings.Services;

public static class SettingsHandlerRegistry
{
    public static readonly ICollection<SettingsRegistryItem> Handlers = new List<SettingsRegistryItem>();

    public static void Register<THandler>(Func<THandler> createFunc, Action<object>? configurationAction = null)
        where THandler : class, ISettingsHandler
    {
        Handlers.Add(new SettingsRegistryItem(typeof(THandler), createFunc, configurationAction));
    }
}