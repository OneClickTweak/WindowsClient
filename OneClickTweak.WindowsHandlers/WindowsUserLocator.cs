using System.Collections;
using System.Management;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.WindowsHandlers;

public class WindowsUserLocator : IUserLocator
{
    public Task<ICollection<UserInstance>> GetUsers()
    {
        var currentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var query = new SelectQuery("Win32_UserAccount", $"domain='{Environment.MachineName}'");
        var searcher = new ManagementObjectSearcher(query);
        var users = new List<UserInstance>();
        foreach (var envVar in searcher.Get())
        {
            var name = Convert.ToString(envVar["Name"]);
            var localPath = Convert.ToString(envVar["LocalPath"]);
            users.Add(new UserInstance
            {
                Name = name ?? string.Empty,
                LocalPath = localPath ?? string.Empty,
                IsCurrent = currentPath == localPath
            });
        }

        return Task.FromResult<ICollection<UserInstance>>(users);
    }

    public Task<IEnumerable<UserInstance>> GetUserInstances()
    {
        throw new NotImplementedException();
    }
}