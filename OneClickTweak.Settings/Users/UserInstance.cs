namespace OneClickTweak.Settings.Users;

public class UserInstance
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string LocalPath { get; init; }

    public bool IsCurrent { get; init; }
}