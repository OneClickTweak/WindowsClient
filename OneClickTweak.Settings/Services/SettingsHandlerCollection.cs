using OneClickTweak.Settings.Runtime;

namespace OneClickTweak.Settings.Services;

public class SettingsHandlerCollection
{
    private readonly IDictionary<string, ISettingsHandler> handlers = new Dictionary<string, ISettingsHandler>(StringComparer.OrdinalIgnoreCase);

    public SettingsHandlerCollection()
    {
        foreach (var handler in SettingsHandlerRegistry.Handlers)
        {
            Register(handler);
        }
    }

    public void Register(SettingsRegistryItem handlerRegistration)
    {
        var instance = Activator.CreateInstance(handlerRegistration.Type);
        if (instance is ISettingsHandler handler)
        {
            if (handlers.TryGetValue(handler.Name, out var existingHandler))
            {
                return;
            }

            handlerRegistration.ConfigurationAction?.Invoke(handler);
            handlers.Add(handler.Name, handler);
            return;
        }

        throw new ArgumentException($"Type '{handlerRegistration.Type.FullName}' does not implement ISettingsHandler interface.");
    }

    public ISettingsHandler? GetHandler(string handlerName)
    {
        return handlers.TryGetValue(handlerName, out var handler) ? handler : null;
    }

    public IEnumerable<ISettingsHandler> GetHandlers()
    {
        return handlers.Values;
    }
}