namespace Vibe.UI.Docs.Models;

public class ComponentMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new();
    public List<string> Dependencies { get; set; } = new();
    public List<PropMetadata> Props { get; set; } = new();
    public List<EventMetadata> Events { get; set; } = new();
}

public class PropMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Default { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Required { get; set; }
}

public class EventMetadata
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
