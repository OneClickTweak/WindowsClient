namespace OneClickTweak.Settings.Definition;

[Flags]
public enum SupportedPlatform
{
    Any = 0,
    Windows = 0x01,
    Linux = 0x02,
    Mac = 0x04
}
