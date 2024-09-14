namespace OneClickTweak.Service.Settings;

[Flags]
public enum SettingPlatform
{
    Windows = 0x01,
    Linux = 0x02,
    MacOS = 0x04
}