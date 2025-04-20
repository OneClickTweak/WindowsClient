using Microsoft.Extensions.Logging;
using OneClickTweak.Settings.Runtime;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.Handlers.Firefox;

public class FirefoxHandler() : BaseHandler("Firefox")
{
    public override IEnumerable<SettingsInstance> GetFoundInstances(IEnumerable<UserInstance> users)
    {
        throw new NotImplementedException();
    }
    
    public override Task<bool> Apply(SettingsInstance instance, SelectedSetting selected, ILogger logger)
    {
        throw new NotImplementedException();
    }
}