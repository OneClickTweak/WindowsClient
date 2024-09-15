using OneClickTweak.Settings;

namespace OneClickTweak.Handlers;

public class FirefoxHandler : ISettingsHandler
{
    public string Name => "Firefox";

    public bool IsVersionMatch(Setting item)
    {
        throw new NotImplementedException();
    }
}