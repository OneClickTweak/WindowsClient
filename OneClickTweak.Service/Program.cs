using OneClickTweak.Handlers;
using OneClickTweak.LinuxHandlers;
using OneClickTweak.Service;
using OneClickTweak.Service.Services;
using OneClickTweak.Settings.Services;
using OneClickTweak.WindowsHandlers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

if (OperatingSystem.IsWindows())
{
    builder.Services.AddSingleton<WindowsUserLocator>();
}
else if (OperatingSystem.IsLinux())
{
    builder.Services.AddSingleton<LinuxUserLocator>();
}

SettingsHandlerRegistry.Register<FirefoxHandler>();
SettingsHandlerRegistry.Register<SqliteHandler>();

if (OperatingSystem.IsWindows())
{
    SettingsHandlerRegistry.Register<RegistryHandler>();
    SettingsHandlerRegistry.Register<GroupPolicyHandler>();
}

builder.Services.AddSingleton(new SettingsHandlerCollection(SettingsHandlerRegistry.GetRegisteredHandlers()));
builder.Services.AddSingleton<SettingsParser>();
builder.Services.AddSingleton<LoaderService>();

var host = builder.Build();
host.Run();