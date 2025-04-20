using OneClickTweak.AndroidHandlers;
using OneClickTweak.Handlers.Firefox;
using OneClickTweak.LinuxHandlers;
using OneClickTweak.Service;
using OneClickTweak.Service.Services;
using OneClickTweak.Settings.Services;
using OneClickTweak.Settings.Users;
using OneClickTweak.WindowsHandlers.GroupPolicy;
using OneClickTweak.WindowsHandlers.Registry;
using OneClickTweak.WindowsHandlers.Windows;

// TODO: Include web hosting API only
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    WebRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "dev/projects/OneClickTweakUI/build")
});
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<GlobalOptions>(nameof(GlobalOptions), builder.Configuration);

if (OperatingSystem.IsWindows())
{
    builder.Services.AddSingleton<IUserLocator, WindowsUserLocator>(x =>
    {
        var locator = new WindowsUserLocator();
        builder.Configuration.GetSection(nameof(WindowsUserLocator)).Bind(locator);
        return locator;
    });
}
else if (OperatingSystem.IsLinux())
{
    builder.Services.AddSingleton<IUserLocator, LinuxUserLocator>(x =>
    {
        var locator = new LinuxUserLocator();
        builder.Configuration.GetSection(nameof(LinuxUserLocator)).Bind(locator);
        return locator;
    });
}

SettingsHandlerRegistry.Register(() => new FirefoxHandler(), x => builder.Configuration.GetSection(nameof(FirefoxHandler)).Bind(x));
SettingsHandlerRegistry.Register(() => new AdbHandler(), x => builder.Configuration.GetSection(nameof(AdbHandler)).Bind(x));

if (OperatingSystem.IsWindows())
{
    SettingsHandlerRegistry.Register(() => new RegistryHandler(), x => builder.Configuration.GetSection(nameof(RegistryHandler)).Bind(x));
    SettingsHandlerRegistry.Register(() => new GroupPolicyHandler(), x => builder.Configuration.GetSection(nameof(GroupPolicyHandler)).Bind(x));
}

builder.Services.AddSingleton(new SettingsHandlerCollection());
builder.Services.AddSingleton<SettingsParser>();
builder.Services.AddSingleton<LoaderService>();

builder.Services.AddControllers();

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();