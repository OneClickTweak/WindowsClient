using OneClickTweak.Settings.Users;

namespace OneClickTweak.Service.Services;

public class LoaderService(IUserLocator userLocator)
{
    public async Task Load()
    {
        var users = await userLocator.GetUsers();
    }
}