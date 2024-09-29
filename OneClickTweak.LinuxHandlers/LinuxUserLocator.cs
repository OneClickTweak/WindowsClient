using OneClickTweak.Settings.Filesystem;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.LinuxHandlers;

public class LinuxUserLocator : IUserLocator
{
    public Task<ICollection<UserInstance>> GetUsers()
    {
        var result = new List<UserInstance>();
        var users = Directory.GetDirectories("/home");
        var passwd = File.ReadAllLines("/etc/passwd");
        foreach (var userDir in users)
        {
            var userLines = passwd.Where(x => x.Contains($":{userDir}:")).ToArray();
            if (userLines.Length == 1)
            {
                var splitLine = userLines[0].Split(':');
                if (splitLine.Length > 2)
                {
                    result.Add(new UserInstance
                    {
                        Id = splitLine[2],
                        Name = splitLine[0],
                        LocalPath = userDir,
                        IsCurrent = Environment.UserName == splitLine[0],
                        CanWrite = FilesystemHelpers.IsDirectoryWritable(userDir)
                    });
                }
            }
        }

        return Task.FromResult<ICollection<UserInstance>>(result);
    }
}