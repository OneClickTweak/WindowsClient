namespace OneClickTweak.Settings.Users;

public interface IUserLocator
{
    Task<ICollection<UserInstance>> GetUsers();
}