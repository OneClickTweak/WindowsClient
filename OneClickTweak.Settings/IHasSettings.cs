namespace OneClickTweak.Settings;

public interface IHasSettings
{
    ICollection<Setting>? Settings { get; }
}