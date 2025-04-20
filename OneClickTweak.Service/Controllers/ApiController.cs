using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Serialization;

namespace OneClickTweak.Service.Controllers;

[ApiController]
[Route("api")]
public class SettingsController : ControllerBase
{
    [HttpGet("settings")]
    public IActionResult Get()
    {
        var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "dev/projects/OneClickTweak/OneClickTweak.Tests/TestFiles/settings.json");
        using var contents = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        var test = JsonSerializer.Deserialize<Setting[]>(contents, SettingsSerializer.Options);
        return Ok(test);
    }
}