using OneClickTweak.Settings.Definition;

namespace OneClickTweak.Service;

public class GlobalOptions
{
    public int UsersChangeTimeout { get; set; } = 600;

    public ICollection<DefinitionSource> Sources { get; set; } = new List<DefinitionSource>();
}