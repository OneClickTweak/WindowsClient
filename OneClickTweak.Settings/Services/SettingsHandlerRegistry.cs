namespace OneClickTweak.Settings.Services;

public static class SettingsHandlerRegistry
{
    private static readonly ICollection<Type> Handlers = new List<Type>();

    public static void Register<THandler>()
        where THandler : class, ISettingsHandler
    {
        Handlers.Add(typeof(THandler));
    }

    public static IEnumerable<Type> GetRegisteredHandlers() => Handlers.AsEnumerable();
}