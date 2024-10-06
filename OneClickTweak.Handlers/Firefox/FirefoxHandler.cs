using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Runtime;

namespace OneClickTweak.Handlers.Firefox;

public class FirefoxHandler : BaseHandler, ISettingsHandler
{
    public string Name => "Firefox";

    public bool IsVersionMatch(Setting item)
    {
        throw new NotImplementedException();
    }
}