using OneClickTweak.Settings;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.LinuxHandlers;

public class LinuxUserLocator : IUserLocator
{
    public Task<ICollection<UserInstance>> GetUsers()
    {
        throw new NotImplementedException();
    }
}