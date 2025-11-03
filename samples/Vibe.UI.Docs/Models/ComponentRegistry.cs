namespace Vibe.UI.Docs.Models;

public class ComponentRegistry
{
    public List<ComponentMetadata> Components { get; set; } = new();

    public List<string> GetCategories()
    {
        return Components
            .Select(c => c.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    public List<ComponentMetadata> GetComponentsByCategory(string category)
    {
        return Components
            .Where(c => c.Category == category)
            .OrderBy(c => c.Name)
            .ToList();
    }

    public ComponentMetadata? GetComponent(string name)
    {
        return Components.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
