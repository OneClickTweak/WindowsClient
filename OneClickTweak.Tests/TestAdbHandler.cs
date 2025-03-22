using OneClickTweak.AndroidHandlers;
using OneClickTweak.Settings.Users;
using Xunit.Abstractions;

namespace OneClickTweak.Tests;


public class TestAdbHandler(ITestOutputHelper outputHelper)
{
    [Fact]
    public void DevicesEnumerated()
    {
        var handler = new AdbHandler();
        var users = Enumerable.Empty<UserInstance>();
        foreach (var instance in handler.GetFoundInstances(users))
        {
            outputHelper.WriteLine($"Found instance: {instance.Options.GetValueOrDefault(AdbHandler.DeviceNameKey)}");
        }
    }
}