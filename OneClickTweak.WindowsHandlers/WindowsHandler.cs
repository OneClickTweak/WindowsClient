using OneClickTweak.Settings.Definition;

namespace OneClickTweak.WindowsHandlers;

public abstract class WindowsHandler
{
    public abstract string Name { get; }

    public virtual bool IsVersionMatch(Setting item)
    {
        if (item.MinVersion != null)
        {
            if (Version.TryParse(item.MinVersion, out var minVersion))
            {
                if (minVersion.CompareTo(Environment.OSVersion.Version) < 0)
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
                if (maxVersion.CompareTo(Environment.OSVersion.Version) >= 0)
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