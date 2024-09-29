using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Runtime;

namespace OneClickTweak.Handlers;

public class SqliteHandler : ISettingsHandler
{
    public string Name => "Sqlite";

    public bool IsVersionMatch(Setting item)
    {
        throw new NotImplementedException();
    }
}