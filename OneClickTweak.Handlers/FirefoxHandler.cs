using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Runtime;

namespace OneClickTweak.Handlers;

public class FirefoxHandler : ISettingsHandler
{
    public string Name => "Firefox";

    public bool IsVersionMatch(Setting item)
    {
        throw new NotImplementedException();
    }
}