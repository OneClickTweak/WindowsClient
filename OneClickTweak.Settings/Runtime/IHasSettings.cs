using OneClickTweak.Settings.Definition;

namespace OneClickTweak.Settings.Runtime;

public interface IHasSettings
{
    ICollection<Setting>? Settings { get; }
}