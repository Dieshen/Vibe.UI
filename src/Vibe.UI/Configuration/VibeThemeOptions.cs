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
    /// <summary>Gets or sets the page background color.</summary>
    public string? Background { get; set; }
    /// <summary>Gets or sets the default foreground color.</summary>
    public string? Foreground { get; set; }
    /// <summary>Gets or sets the card background color.</summary>
    public string? Card { get; set; }
    /// <summary>Gets or sets the card foreground color.</summary>
    public string? CardForeground { get; set; }
    /// <summary>Gets or sets the popover background color.</summary>
    public string? Popover { get; set; }
    /// <summary>Gets or sets the popover foreground color.</summary>
    public string? PopoverForeground { get; set; }
    /// <summary>Gets or sets the primary accent color.</summary>
    public string? Primary { get; set; }
    /// <summary>Gets or sets the foreground color used on primary surfaces.</summary>
    public string? PrimaryForeground { get; set; }
    /// <summary>Gets or sets the secondary surface color.</summary>
    public string? Secondary { get; set; }
    /// <summary>Gets or sets the foreground color used on secondary surfaces.</summary>
    public string? SecondaryForeground { get; set; }
    /// <summary>Gets or sets the muted surface color.</summary>
    public string? Muted { get; set; }
    /// <summary>Gets or sets the foreground color used on muted surfaces.</summary>
    public string? MutedForeground { get; set; }
    /// <summary>Gets or sets the accent surface color.</summary>
    public string? Accent { get; set; }
    /// <summary>Gets or sets the foreground color used on accent surfaces.</summary>
    public string? AccentForeground { get; set; }
    /// <summary>Gets or sets the destructive action color.</summary>
    public string? Destructive { get; set; }
    /// <summary>Gets or sets the foreground color used on destructive surfaces.</summary>
    public string? DestructiveForeground { get; set; }
    /// <summary>Gets or sets the border color.</summary>
    public string? Border { get; set; }
    /// <summary>Gets or sets the input border color.</summary>
    public string? Input { get; set; }
    /// <summary>Gets or sets the focus ring color.</summary>
    public string? Ring { get; set; }
}
