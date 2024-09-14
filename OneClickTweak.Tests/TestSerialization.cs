using System.Text.Json;
using OneClickTweak.Settings;

namespace OneClickTweak.Tests;

public class TestSerialization
{
    [Fact]
    public void TestDeserialize()
    {
      var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web);
      var test = JsonSerializer.Deserialize<SettingDefinition>(SingleSetting, opts);
      var json = JsonSerializer.Serialize(test, opts);
      Assert.Equal(json, SingleSetting);
    }

    private const string SingleSetting =
"""
{
  "platforms": [ "Windows" ],
  "name": "Windows.Power.Hibernate",
  "settings": [
    {
      "settings": [
        {
          "handler": "Registry",
          "path": [ "HKLM", "SYSTEM", "CurrentControlSet", "Control", "Power" ],
          "key": "HibernateEnabled",
          "type": "Int32",
          "values": [
            { "name": "Enabled", "value": "1", "isDefault": "true" },
            { "name": "Disabled", "value": "0" }
          ]
        },
        {
          "handler": "GPO",
          "path": [ "GPO", "SYSTEM" ],
          "key: "HibernateDisabled",
          "type": "Boolean",
          "values": [
            { "name": "Enabled", "value": "false", "isDefault": "true" },
            { "name": "Disabled", "value": "true" }
          ]
        }
      ]
    },
  ]
}
""";
}