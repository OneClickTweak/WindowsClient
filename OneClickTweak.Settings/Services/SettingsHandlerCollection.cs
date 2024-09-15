namespace OneClickTweak.Settings.Services;

public class SettingsHandlerCollection
{
    private readonly IDictionary<string, ISettingsHandler> handlers = new Dictionary<string, ISettingsHandler>(StringComparer.OrdinalIgnoreCase);

    public SettingsHandlerCollection()
    {
        foreach (var handlerType in SettingsHandlerRegistry.GetRegisteredHandlers())
        {
            Register(handlerType);
        }
    }

    private void Register(Type handlerType)
    {
        var instance = Activator.CreateInstance(handlerType);
        if (instance is ISettingsHandler handler)
        {
            handlers.Add(handler.Name, handler);
        }

        throw new ArgumentException($"Type '{handlerType.FullName}' does not implement ISettingsHandler interface.");
    }

    public ISettingsHandler? GetHandler(string handlerName)
    {
        return handlers.TryGetValue(handlerName, out var handler) ? handler : null;
    }
}