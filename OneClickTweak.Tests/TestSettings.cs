using System.Text.Json;
using OneClickTweak.Settings.Definition;
using OneClickTweak.Settings.Serialization;
using OneClickTweak.Settings.Services;
using OneClickTweak.WindowsHandlers;

namespace OneClickTweak.Tests;

public class TestSettings
{
    [Fact]
    public void TestSerializationComplete()
    {
        var test = JsonSerializer.Deserialize<SettingDefinition>(SingleSetting, SettingsSerializer.Options);
        var json = JsonSerializer.Serialize(test, SettingsSerializer.Options);
        Assert.Equal(GetReferenceJson(SingleSetting), json);
    }

    [Fact]
    public void TestFlatten()
    {
        var test = JsonSerializer.Deserialize<SettingDefinition>(SingleSetting, SettingsSerializer.Options);
        Assert.NotNull(test);

        SettingsHandlerRegistry.Register<RegistryHandler>();
        SettingsHandlerRegistry.Register<GroupPolicyHandler>();
        var handlers = new SettingsHandlerCollection();

        var parser = new SettingsParser(handlers);
        var flat = parser.FlattenSettings(test, handlers, null);

        var json = JsonSerializer.Serialize(flat, SettingsSerializer.Options);
        Assert.Equal(GetReferenceJson(FlattenedSetting), json);
    }

    private string GetReferenceJson(string json)
    {
        var jsonObject = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(jsonObject, SettingsSerializer.Options);
    }

    private const string SingleSetting =
        """
        {
          "name": "Windows.Power.Hibernate",
          "settings": [
            {
              "platform": [ "Windows" ],
              "scope": "Machine",
              "settings": [
                {
                  "handler": "Registry",
                  "path": [ "SYSTEM", "CurrentControlSet", "Control", "Power" ],
                  "key": "HibernateEnabled",
                  "type": "Int32",
                  "values": [
                    { "name": "Global.Enabled", "value": "1", "isDefault": true },
                    { "name": "Global.Disabled", "value": "0" }
                  ]
                },
                {
                  "handler": "GPO",
                  "path": [ "SYSTEM", "Policies", "Control", "Power" ],
                  "key": "HibernateDisabled",
                  "type": "Boolean",
                  "values": [
                    { "name": "Global.Enabled", "value": "false", "isDefault": true },
                    { "name": "Global.Disabled", "value": "true" }
                  ]
                }
              ]
            }
          ]
        }
        """;

    private const string FlattenedSetting =
        """
        [
          {
            "platform": [ "Windows" ],
            "handler": "Registry",
            "scope": "Machine",
            "path": [ "SYSTEM", "CurrentControlSet", "Control", "Power" ],
            "key": "HibernateEnabled",
            "type": "Int32",
            "values": [
              { "name": "Global.Enabled", "value": "1", "isDefault": true },
              { "name": "Global.Disabled", "value": "0" }
            ]
          },
          {
            "platform": [ "Windows" ],
            "handler": "GPO",
            "scope": "Machine",
            "path": [ "SYSTEM", "Policies", "Control", "Power" ],
            "key": "HibernateDisabled",
            "type": "Boolean",
            "values": [
              { "name": "Global.Enabled", "value": "false", "isDefault": true },
              { "name": "Global.Disabled", "value": "true" }
            ]
          }
        ]
      """;
}