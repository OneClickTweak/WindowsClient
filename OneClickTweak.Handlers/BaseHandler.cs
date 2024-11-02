using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Runtime;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.Handlers;

public abstract class BaseHandler : ISettingsHandler
{
    protected BaseHandler(string? name)
    {
        Name = name ?? GetType().Name.Replace("Handler", string.Empty);
    }

    public string Name { get; init; }

    public int ChangeTimeout { get; set; } = 600;

    public abstract IEnumerable<SettingsInstance> GetFoundInstances(IEnumerable<UserInstance> users);

    public virtual bool IsVersionMatch(SettingsInstance instance, Setting item)
    {
        if (instance.Version == null)
        {
            return true;
        }

        if (!Version.TryParse(instance.Version, out var version))
        {
            throw new InvalidOperationException("Unable to parse Version");
        }

        if (item.MinVersion != null)
        {
            if (Version.TryParse(item.MinVersion, out var minVersion))
            {
                if (minVersion.CompareTo(version) < 0)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        if (item.MaxVersion != null)
        {
            if (Version.TryParse(item.MaxVersion, out var maxVersion))
            {
                if (maxVersion.CompareTo(version) >= 0)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}