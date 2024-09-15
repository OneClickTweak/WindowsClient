using System.Text.Json;
using OneClickTweak.Settings;
using OneClickTweak.Settings.Serialization;

namespace OneClickTweak.Tests;

public class TestSerialization
{
    [Fact]
    public void TestSerializationComplete()
    {
      var test = JsonSerializer.Deserialize<SettingDefinition>(SingleSetting, SettingsSerializer.Options);
      var json = JsonSerializer.Serialize(test, SettingsSerializer.Options);
      Assert.Equal(GetReferenceJson(SingleSetting), json);
    }

    private string GetReferenceJson(string json)
    {
      var jsonObject= JsonDocument.Parse(json);
      return JsonSerializer.Serialize(jsonObject, SettingsSerializer.Options);
    }

    private const string SingleSetting =
"""
{
  "name": "Windows.Power.Hibernate",
  "settings": [
    {
      "platform": [ "Windows" ],
      "settings": [
        {
          "handler": "Registry",
          "scope": "Machine",
          "path": [ "SYSTEM", "CurrentControlSet", "Control", "Power" ],
          "key": "HibernateEnabled",
          "type": "Int32",
          "values": [
            { "name": "Enabled", "value": "1", "isDefault": true },
            { "name": "Disabled", "value": "0" }
          ]
        },
        {
          "handler": "GPO",
          "scope": "Machine",
          "path": [ "SYSTEM", "Policies", "Control", "Power" ],
          "key": "HibernateDisabled",
          "type": "Boolean",
          "values": [
            { "name": "Enabled", "value": "false", "isDefault": true },
            { "name": "Disabled", "value": "true" }
          ]
        }
      ]
    }
  ]
}
""";
}