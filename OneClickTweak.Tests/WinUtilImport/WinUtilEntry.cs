namespace OneClickTweak.Tests.WinUtilImport;

public class WinUtilEntry
{
    static ICollection<WinUtilEntry> History = new List<WinUtilEntry>();
    
    public WinUtilEntry()
    {
        History.Add(this);
    }
    
    public required string Content { get; set; }
    
    public string? Description { get; set; }
    
    public required string Category { get; set; }
    
    public required string Panel { get; set; }
    
    public required string Order { get; set; }
    
    public string? Link { get; set; }
    
    public ICollection<WinUtilRegistry> Registry { get; set; } = new List<WinUtilRegistry>();
    
    public ICollection<WinUtilService> Service { get; set; } = new List<WinUtilService>();
    
    public ICollection<string> Appx { get; set; } = new List<string>();
    
    public ICollection<WinUtilScheduledTask> ScheduledTask { get; set; } = new List<WinUtilScheduledTask>();

    public ICollection<string> InvokeScript { get; set; } = new List<string>();
}