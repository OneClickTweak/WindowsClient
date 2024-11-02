using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Serialization;
using OneClickTweak.Tests.WinUtilImport;
using Xunit.Abstractions;

namespace OneClickTweak.Tests;

public class TestLoadDefinitions(ITestOutputHelper outputHelper)
{
    private static Regex FixupRegex = new Regex("""("(InvokeScript|UndoScript)":\s*\[[\r\n\s]*)([\r\n\s]*"[^"\\]*(?:\\.[^"\\]*)*"[\r\n\s]*,?[\r\n\s]*)+""", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    [Fact]
    public void DefinitionsAreLoaded()
    {
        var definitionsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "TestFiles", "definitions.json");
        using var stream = File.OpenRead(definitionsFile);
        var test = JsonSerializer.Deserialize<SettingDefinition>(stream, SettingsSerializer.Options);
    }    
    
    [Fact]
    public void TestImportWinUtil()
    {
        var definitionsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "TestFiles", "tweaks.json");
        var json = File.ReadAllText(definitionsFile);
        var fixedJson = FixupRegex.Replace(json, match =>
        {
            var sb = new StringBuilder();
            sb.Append(match.Groups[1].Value);
            foreach (var group in match.Groups.Values.Skip(3))
            {
                sb.Append(group.Value.Trim().ReplaceLineEndings("\\n"));
            }

            return sb.ToString();
        });
        var options = new JsonSerializerOptions(SettingsSerializer.Options)
        {
            PropertyNameCaseInsensitive = true
        };

        var test = JsonSerializer.Deserialize<WinUtilEntries>(fixedJson, options);
        foreach (var item in test)
        {
            outputHelper.WriteLine($"{item.Key}: {item.Value.Description}");
        }
    }
}