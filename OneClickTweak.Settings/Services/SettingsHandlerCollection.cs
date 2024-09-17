namespace OneClickTweak.Settings.Services;

public class SettingsHandlerCollection
{
    private readonly IDictionary<string, ISettingsHandler> handlers = new Dictionary<string, ISettingsHandler>(StringComparer.OrdinalIgnoreCase);

    public SettingsHandlerCollection()
    {
    }

    public SettingsHandlerCollection(IEnumerable<Type> handlerTypes)
    {
        foreach (var handlerType in handlerTypes)
        {
            Register(handlerType);
        }
    }

    public ISettingsHandler Register(Type handlerType)
    {
        var instance = Activator.CreateInstance(handlerType);
        if (instance is ISettingsHandler handler)
        {
            if (handlers.TryGetValue(handler.Name, out var existingHandler))
            {
                return existingHandler;
            }

            handlers.Add(handler.Name, handler);
            return handler;
        }

        throw new ArgumentException($"Type '{handlerType.FullName}' does not implement ISettingsHandler interface.");
    }

    public ISettingsHandler? GetHandler(string handlerName)
    {
        return handlers.TryGetValue(handlerName, out var handler) ? handler : null;
    }
}