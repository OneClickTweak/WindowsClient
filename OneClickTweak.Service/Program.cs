using OneClickTweak.Handlers;
using OneClickTweak.Service;
using OneClickTweak.Settings.Services;
using OneClickTweak.WindowsHandlers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

SettingsHandlerRegistry.Register<FirefoxHandler>();
SettingsHandlerRegistry.Register<SqliteHandler>();

if (OperatingSystem.IsWindows())
{
    SettingsHandlerRegistry.Register<RegistryHandler>();
    SettingsHandlerRegistry.Register<GroupPolicyHandler>();
}

builder.Services.AddSingleton(new SettingsHandlerCollection(SettingsHandlerRegistry.GetRegisteredHandlers()));
builder.Services.AddSingleton<SettingsParser>();

var host = builder.Build();
host.Run();