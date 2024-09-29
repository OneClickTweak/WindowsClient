using OneClickTweak.Handlers;
using OneClickTweak.LinuxHandlers;
using OneClickTweak.Service;
using OneClickTweak.Service.Services;
using OneClickTweak.Settings.Services;
using OneClickTweak.Settings.Users;
using OneClickTweak.WindowsHandlers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

if (OperatingSystem.IsWindows())
{
    builder.Services.AddSingleton<IUserLocator, WindowsUserLocator>();
}
else if (OperatingSystem.IsLinux())
{
    builder.Services.AddSingleton<IUserLocator, LinuxUserLocator>();
}

SettingsHandlerRegistry.Register<FirefoxHandler>(x => builder.Configuration.GetSection(nameof(FirefoxHandler)).Bind(x));
SettingsHandlerRegistry.Register<SqliteHandler>(x => builder.Configuration.GetSection(nameof(SqliteHandler)).Bind(x));

if (OperatingSystem.IsWindows())
{
    SettingsHandlerRegistry.Register<RegistryHandler>(x => builder.Configuration.GetSection(nameof(RegistryHandler)).Bind(x));
    SettingsHandlerRegistry.Register<GroupPolicyHandler>(x => builder.Configuration.GetSection(nameof(GroupPolicyHandler)).Bind(x));
}

builder.Services.AddSingleton(new SettingsHandlerCollection());
builder.Services.AddSingleton<SettingsParser>();
builder.Services.AddSingleton<LoaderService>();

var host = builder.Build();
host.Run();