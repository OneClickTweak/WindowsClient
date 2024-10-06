using System.Management;
using System.Security.Principal;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.WindowsHandlers.Windows;

public class WindowsUserLocator : IUserLocator
{
    public Task<ICollection<UserInstance>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var currentSid = WindowsIdentity.GetCurrent().Owner?.ToString();
        var users = new List<UserInstance>();
        var userPaths = GetUserPaths();
        var query = new SelectQuery("Win32_UserAccount", $"SIDType = 1 AND AccountType = 512", [ "SID", "Name" ]);
        var searcher = new ManagementObjectSearcher(query);
        foreach (var item in searcher.Get())
        {
            var sid = Convert.ToString(item["SID"]);
            if (sid != null && userPaths.TryGetValue(sid, out var path))
            {
                var name = Convert.ToString(item["Name"]);
                users.Add(new UserInstance
                {
                    Id = sid,
                    Name = name ?? sid,
                    LocalPath = path,
                    IsCurrent = currentSid == sid
                });
            }
        }

        return Task.FromResult<ICollection<UserInstance>>(users);
    }

    private Dictionary<string, string> GetUserPaths()
    {
        var userPaths = new Dictionary<string, string>();
        var query = new SelectQuery("Win32_UserProfile", null, [ "SID", "LocalPath" ]);
        var searcher = new ManagementObjectSearcher(query);
        foreach (var item in searcher.Get())
        {
            var sid = Convert.ToString(item["SID"]);
            var path = Convert.ToString(item["LocalPath"]);
            if (sid != null && path != null)
            {
                userPaths[sid] = path;
            }
        }

        return userPaths;
    }

    public Task<IEnumerable<UserInstance>> GetUserInstances()
    {
        throw new NotImplementedException();
    }
}