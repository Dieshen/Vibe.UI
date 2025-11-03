using Vibe.UI.Docs.Models;

namespace Vibe.UI.Docs.Services;

public class ComponentSearchService
{
    private readonly ComponentRegistry _registry;

    public ComponentSearchService(ComponentRegistry registry)
    {
        _registry = registry;
    }

    public List<ComponentMetadata> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return _registry.Components;
        }

        query = query.ToLower();

        return _registry.Components
            .Where(c =>
                c.Name.ToLower().Contains(query) ||
                c.Description.ToLower().Contains(query) ||
                c.Category.ToLower().Contains(query) ||
                c.Keywords.Any(k => k.ToLower().Contains(query)))
            .OrderByDescending(c => CalculateRelevance(c, query))
            .ToList();
    }

    private int CalculateRelevance(ComponentMetadata component, string query)
    {
        int score = 0;

        // Exact name match gets highest score
        if (component.Name.Equals(query, StringComparison.OrdinalIgnoreCase))
            score += 100;

        // Name starts with query
        if (component.Name.ToLower().StartsWith(query))
            score += 50;

        // Name contains query
        if (component.Name.ToLower().Contains(query))
            score += 25;

        // Description contains query
        if (component.Description.ToLower().Contains(query))
            score += 10;

        // Category matches
        if (component.Category.ToLower().Contains(query))
            score += 15;

        // Keywords match
        score += component.Keywords.Count(k => k.ToLower().Contains(query)) * 5;

        return score;
    }
}
