namespace Vibe.UI.Configuration;

/// <summary>
/// Configuration options for Vibe.UI theming.
/// </summary>
public class VibeThemeOptions
{
    /// <summary>
    /// The base color scheme to use. Default is "Slate".
    /// Options: Slate, Gray, Zinc, Neutral, Stone, Blue, Custom
    /// </summary>
    public string BaseColor { get; set; } = "Slate";

    /// <summary>
    /// Custom light mode colors (used when BaseColor = "Custom")
    /// </summary>
    public CustomThemeColors? LightColors { get; set; }

    /// <summary>
    /// Custom dark mode colors (used when BaseColor = "Custom")
    /// </summary>
    public CustomThemeColors? DarkColors { get; set; }

    /// <summary>
    /// Border radius size. Default is 0.5rem (8px).
    /// </summary>
    public string BorderRadius { get; set; } = "0.5rem";
}

/// <summary>
/// Custom theme colors for light or dark mode.
/// </summary>
public class CustomThemeColors
{
    public string? Background { get; set; }
    public string? Foreground { get; set; }
    public string? Card { get; set; }
    public string? CardForeground { get; set; }
    public string? Popover { get; set; }
    public string? PopoverForeground { get; set; }
    public string? Primary { get; set; }
    public string? PrimaryForeground { get; set; }
    public string? Secondary { get; set; }
    public string? SecondaryForeground { get; set; }
    public string? Muted { get; set; }
    public string? MutedForeground { get; set; }
    public string? Accent { get; set; }
    public string? AccentForeground { get; set; }
    public string? Destructive { get; set; }
    public string? DestructiveForeground { get; set; }
    public string? Border { get; set; }
    public string? Input { get; set; }
    public string? Ring { get; set; }
}
