using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using OneClickTweak.Settings.Filesystem;
using OneClickTweak.Settings.Services;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.Service.Services;

public class LoaderService(
    IUserLocator userLocator,
    SettingsHandlerCollection handlers,
    IOptionsMonitor<GlobalOptions> options)
{
    private ICollection<UserInstance>? users;
    
    private readonly ConcurrentDictionary<Type, DateTime?> nextUpdate = new();

    private HashSet<string> canWriteUsers = new();

    public async Task<DateTime> DetectTickAsync(CancellationToken cancellationToken)
    {
        var usersChanged = IsExpired(userLocator.GetType()) && await DetectUserChangesAsync(false, cancellationToken);
        DetectHandlersAsync(usersChanged, cancellationToken);
        return nextUpdate.Values.Where(x => x != null).Select(x => x!.Value).Min();
    }

    private async Task<bool> DetectUserChangesAsync(bool force, CancellationToken cancellationToken)
    {
        var newUsers = await userLocator.GetUsersAsync(cancellationToken);
        if (users == null || !newUsers.OrderBy(x => x.Id).SequenceEqual(users.OrderBy(x => x.Id)) || force)
        {
            users = newUsers;
            canWriteUsers = newUsers.Where(x => FilesystemHelpers.IsDirectoryWritable(x.LocalPath)).Select(x => x.Id).ToHashSet();
            MarkChanged(userLocator.GetType(), options.CurrentValue.UsersChangeTimeout);
            return true;
        }

        return false;
    }

    private void DetectHandlersAsync(bool force, CancellationToken cancellationToken)
    {
        handlers.GetHandlers()
            .AsParallel()
            .WithCancellation(cancellationToken)
            .ForAll(handler =>
            {
                if (force || IsExpired(handler.GetType()))
                {
                    // Apply and detect changes
                    MarkChanged(handler.GetType(), handler.ChangeTimeout);
                }
            });
    }

    private bool IsExpired(Type type)
    {
        return !nextUpdate.TryGetValue(type, out var nextUpdateTime) || nextUpdateTime == null || DateTime.UtcNow > nextUpdateTime;
    }
    
    private void MarkChanged(Type type, int timeout)
    {
        nextUpdate[type] = DateTime.UtcNow.AddSeconds(timeout);
    }
}