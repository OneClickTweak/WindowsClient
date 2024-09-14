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
  "versions": [
    {
      "handler": "Registry",
      "path": [ "HKLM", "SYSTEM", "CurrentControlSet", "Control", "Power" ],
      "key": "HibernateEnabled",
      "valueType": "Int32",
      "values": [
        { "name": "enabled", "value": "1" },
        { "name": "disabled", "value": "0" }
      ]
    }
  ]
}
""";
}