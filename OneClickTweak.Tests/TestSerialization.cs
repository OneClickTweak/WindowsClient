namespace OneClickTweak.Tests;

public class TestSerialization
{
    [Fact]
    public void TestDeserialize()
    {
    }

    private const string Settings =
"""
[
  {
    "platforms": [ "windows" ],
    "versions": [
      "handler": "registry",
      "path": [ "HKLM", "SYSTEM", "CurrentControlSet", "Control", "Power" ],
      "key": "HibernateEnabled",
      "valueType": "int32",
      "values": [
        { "name": "enabled", "value": "1" },
        { "name": "disabled", "value": "0" }
      ]
    ]
  }
]                             
""";
}