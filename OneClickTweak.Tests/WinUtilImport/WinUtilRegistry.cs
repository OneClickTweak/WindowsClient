namespace OneClickTweak.Tests.WinUtilImport;

public class WinUtilRegistry
{
    public required string Path { get; set; }
    
    public required string Name { get; set; }
    
    public required string Type { get; set; }
    
    public required string Value { get; set; }
    
    public required string? OriginalValue { get; set; }
}