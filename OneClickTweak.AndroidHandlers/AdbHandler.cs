using System.Net;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.Models;
using OneClickTweak.Handlers;
using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Runtime;
using OneClickTweak.Settings.Users;

namespace OneClickTweak.AndroidHandlers;

public class AdbHandler() : BaseHandler("ADB")
{
    public string AdbPath { get; set; } = "adb";

    public bool AdbRestartIfNewer { get; set; } = false;

    private EndPoint? serverEndpoint;

    public override IEnumerable<SettingsInstance> GetFoundInstances(IEnumerable<UserInstance> users)
    {
        if (!AdbServer.Instance.GetStatus().IsRunning)
        {
            var server = new AdbServer();
            var result = server.StartServer(AdbPath, AdbRestartIfNewer);
            if (result != StartServerResult.Started)
            {
                throw new Exception($"Failed to start adb server: {result}");
            }

            serverEndpoint = server.EndPoint;
        }
        else
        {
            serverEndpoint = ((AdbServer)AdbServer.Instance).EndPoint;
        }
        
        var adbClient = new AdbClient();
        adbClient.Connect(serverEndpoint.ToString() ?? "127.0.0.1:62001");

        foreach (var device in adbClient.GetDevices())
        {
            if (!string.IsNullOrEmpty(device.Serial))
            {
                yield return new AdbInstance
                {
                    Scope = SettingScope.Machine,
                    Path = device.Name,
                    Device = device
                };   
            }
        }
    }
}