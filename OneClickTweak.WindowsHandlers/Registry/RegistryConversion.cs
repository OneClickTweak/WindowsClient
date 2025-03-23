using Microsoft.Win32;
using OneClickTweak.Settings.Definition;

namespace OneClickTweak.WindowsHandlers.Registry;

public record RegistryConversion(RegistryValueKind ValueKind, SettingType SettingType, Func<string?, object?> ConvertTo, Func<object?, string?> ConvertFrom);
