using System.Reflection;
using System.Text.Json;
using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Serialization;
using OneClickTweak.Settings.Services;
using OneClickTweak.WindowsHandlers.GroupPolicy;
using OneClickTweak.WindowsHandlers.Registry;

namespace OneClickTweak.Tests;

public class TestSettings
{
    [Fact]
    public void TestSerializationComplete()
    {
        var definitionsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "TestFiles", "definitions.json");
        var contents = File.ReadAllText(definitionsFile);
        var test = JsonSerializer.Deserialize<Setting[]>(contents, SettingsSerializer.Options);
        var json = JsonSerializer.Serialize(test, SettingsSerializer.Options);
        Assert.Equal(GetReferenceJson(contents), json);
    }

    [Fact]
    public void TestFlatten()
    {
        var definitionsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "TestFiles", "definitions.json");
        var contents = File.ReadAllText(definitionsFile);
        var test = JsonSerializer.Deserialize<Setting[]>(contents, SettingsSerializer.Options);
        Assert.NotNull(test);

        SettingsHandlerRegistry.Register(() => new RegistryHandler());
        SettingsHandlerRegistry.Register(() => new GroupPolicyHandler());
        var handlers = new SettingsHandlerCollection();

        var parser = new SettingsParser(handlers);
        var flat = parser.FlattenSettings(test[0], handlers, null);

        var json = JsonSerializer.Serialize(flat, SettingsSerializer.Options);
        var flatDefinitions = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "TestFiles", "definitions-flat.json");
        var flatContents = File.ReadAllText(flatDefinitions);
        Assert.Equal(GetReferenceJson(flatContents), json);
    }

    private string GetReferenceJson(string json)
    {
        var jsonObject = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(jsonObject, SettingsSerializer.Options);
    }
}