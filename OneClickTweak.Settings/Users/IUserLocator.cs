namespace OneClickTweak.Settings.Users;

public interface IUserLocator
{
    Task<ICollection<UserInstance>> GetUsersAsync(CancellationToken cancellationToken);
}