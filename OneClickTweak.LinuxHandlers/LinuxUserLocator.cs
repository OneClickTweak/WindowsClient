using OneClickTweak.Settings.Users;

namespace OneClickTweak.LinuxHandlers;

public class LinuxUserLocator : IUserLocator
{
    public string HomeDirectory { get; set; } = "/home";

    public string UsersFile { get; set; } = "/etc/passwd";

    public async Task<ICollection<UserInstance>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var result = new List<UserInstance>();
        var users = Directory.EnumerateDirectories(HomeDirectory);
        var passwd = await File.ReadAllLinesAsync(UsersFile, cancellationToken);
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
                        IsCurrent = Environment.UserName == splitLine[0]
                    });
                }
            }
        }

        return result;
    }
}