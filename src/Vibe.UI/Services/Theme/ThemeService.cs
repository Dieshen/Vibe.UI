using Vibe.UI.Configuration;

namespace Vibe.UI.Services.Theme;

/// <summary>
/// Service for generating theme CSS based on configuration.
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Generates CSS for the configured theme.
    /// </summary>
    string GenerateThemeCss();
}

/// <summary>
/// Default implementation of theme service.
/// </summary>
public class ThemeService : IThemeService
{
    private readonly VibeThemeOptions _options;

    public ThemeService(VibeThemeOptions options)
    {
        _options = options;
    }

    public string GenerateThemeCss()
    {
        var (lightColors, darkColors) = GetColorsForBaseColor(_options.BaseColor);

        // Use custom colors if provided
        if (_options.BaseColor == "Custom")
        {
            if (_options.LightColors != null)
                lightColors = MapCustomColors(_options.LightColors);
            if (_options.DarkColors != null)
                darkColors = MapCustomColors(_options.DarkColors);
        }

        return GenerateCss(lightColors, darkColors, _options.BorderRadius);
    }

    private (ColorSet light, ColorSet dark) GetColorsForBaseColor(string baseColor)
    {
        return baseColor switch
        {
            "Slate" => (
                new ColorSet
                {
                    Background = "hsl(0 0% 100%)",
                    Foreground = "hsl(222.2 84% 4.9%)",
                    Primary = "hsl(222.2 47.4% 11.2%)",
                    Secondary = "hsl(210 40% 96.1%)",
                    Muted = "hsl(214.3 31.8% 91.4%)"
                },
                new ColorSet
                {
                    Background = "hsl(222.2 84% 4.9%)",
                    Foreground = "hsl(210 40% 98%)",
                    Primary = "hsl(217.2 32.6% 17.5%)",
                    Secondary = "hsl(217.2 32.6% 17.5%)",
                    Muted = "hsl(215 20.2% 65.1%)"
                }
            ),
            "Gray" => (
                new ColorSet
                {
                    Background = "hsl(0 0% 100%)",
                    Foreground = "hsl(0 0% 3.9%)",
                    Primary = "hsl(0 0% 14.9%)",
                    Secondary = "hsl(0 0% 96.1%)",
                    Muted = "hsl(0 0% 89.8%)"
                },
                new ColorSet
                {
                    Background = "hsl(0 0% 3.9%)",
                    Foreground = "hsl(0 0% 98%)",
                    Primary = "hsl(0 0% 14.9%)",
                    Secondary = "hsl(0 0% 14.9%)",
                    Muted = "hsl(0 0% 63.9%)"
                }
            ),
            "Zinc" => (
                new ColorSet
                {
                    Background = "hsl(0 0% 100%)",
                    Foreground = "hsl(240 10% 3.9%)",
                    Primary = "hsl(240 5.9% 10%)",
                    Secondary = "hsl(240 4.8% 95.9%)",
                    Muted = "hsl(240 5.9% 90%)"
                },
                new ColorSet
                {
                    Background = "hsl(240 10% 3.9%)",
                    Foreground = "hsl(0 0% 98%)",
                    Primary = "hsl(240 3.7% 15.9%)",
                    Secondary = "hsl(240 3.7% 15.9%)",
                    Muted = "hsl(240 5% 64.9%)"
                }
            ),
            "Neutral" => (
                new ColorSet
                {
                    Background = "hsl(0 0% 100%)",
                    Foreground = "hsl(0 0% 3.9%)",
                    Primary = "hsl(0 0% 14.9%)",
                    Secondary = "hsl(0 0% 96.1%)",
                    Muted = "hsl(0 0% 89.8%)"
                },
                new ColorSet
                {
                    Background = "hsl(0 0% 3.9%)",
                    Foreground = "hsl(0 0% 98%)",
                    Primary = "hsl(0 0% 14.9%)",
                    Secondary = "hsl(0 0% 14.9%)",
                    Muted = "hsl(0 0% 63.9%)"
                }
            ),
            "Stone" => (
                new ColorSet
                {
                    Background = "hsl(0 0% 100%)",
                    Foreground = "hsl(20 14.3% 4.1%)",
                    Primary = "hsl(24 9.8% 10%)",
                    Secondary = "hsl(60 9.1% 97.8%)",
                    Muted = "hsl(24 5.7% 82.9%)"
                },
                new ColorSet
                {
                    Background = "hsl(20 14.3% 4.1%)",
                    Foreground = "hsl(60 9.1% 97.8%)",
                    Primary = "hsl(24 9.8% 10%)",
                    Secondary = "hsl(24 9.8% 10%)",
                    Muted = "hsl(24 5.4% 63.9%)"
                }
            ),
            "Blue" => (
                new ColorSet
                {
                    Background = "hsl(0 0% 100%)",
                    Foreground = "hsl(222.2 84% 4.9%)",
                    Primary = "hsl(221.2 83.2% 53.3%)",
                    Secondary = "hsl(210 40% 96.1%)",
                    Muted = "hsl(214.3 31.8% 91.4%)"
                },
                new ColorSet
                {
                    Background = "hsl(222.2 84% 4.9%)",
                    Foreground = "hsl(210 40% 98%)",
                    Primary = "hsl(217.2 91.2% 59.8%)",
                    Secondary = "hsl(217.2 32.6% 17.5%)",
                    Muted = "hsl(215 20.2% 65.1%)"
                }
            ),
            _ => throw new ArgumentException($"Unknown base color: {baseColor}")
        };
    }

    private ColorSet MapCustomColors(CustomThemeColors custom)
    {
        return new ColorSet
        {
            Background = custom.Background ?? "hsl(0 0% 100%)",
            Foreground = custom.Foreground ?? "hsl(222.2 84% 4.9%)",
            Primary = custom.Primary ?? "hsl(222.2 47.4% 11.2%)",
            Secondary = custom.Secondary ?? "hsl(210 40% 96.1%)",
            Muted = custom.Muted ?? "hsl(214.3 31.8% 91.4%)",
            Card = custom.Card,
            CardForeground = custom.CardForeground,
            Popover = custom.Popover,
            PopoverForeground = custom.PopoverForeground,
            PrimaryForeground = custom.PrimaryForeground,
            SecondaryForeground = custom.SecondaryForeground,
            MutedForeground = custom.MutedForeground,
            Accent = custom.Accent,
            AccentForeground = custom.AccentForeground,
            Destructive = custom.Destructive,
            DestructiveForeground = custom.DestructiveForeground,
            Border = custom.Border,
            Input = custom.Input,
            Ring = custom.Ring
        };
    }

    private string GenerateCss(ColorSet light, ColorSet dark, string borderRadius)
    {
        return $@":root {{
    --vibe-background: {light.Background};
    --vibe-foreground: {light.Foreground};
    --vibe-card: {light.Card ?? light.Background};
    --vibe-card-foreground: {light.CardForeground ?? light.Foreground};
    --vibe-popover: {light.Popover ?? light.Background};
    --vibe-popover-foreground: {light.PopoverForeground ?? light.Foreground};
    --vibe-primary: {light.Primary};
    --vibe-primary-foreground: {light.PrimaryForeground ?? "hsl(0 0% 100%)"};
    --vibe-secondary: {light.Secondary};
    --vibe-secondary-foreground: {light.SecondaryForeground ?? light.Foreground};
    --vibe-muted: {light.Muted};
    --vibe-muted-foreground: {light.MutedForeground ?? "hsl(215.4 16.3% 46.9%)"};
    --vibe-accent: {light.Accent ?? light.Secondary};
    --vibe-accent-foreground: {light.AccentForeground ?? light.Foreground};
    --vibe-destructive: {light.Destructive ?? "hsl(0 84.2% 60.2%)"};
    --vibe-destructive-foreground: {light.DestructiveForeground ?? "hsl(0 0% 100%)"};
    --vibe-border: {light.Border ?? "hsl(214.3 31.8% 91.4%)"};
    --vibe-input: {light.Input ?? "hsl(214.3 31.8% 91.4%)"};
    --vibe-ring: {light.Ring ?? light.Primary};
    --vibe-radius: {borderRadius};
}}

.dark {{
    --vibe-background: {dark.Background};
    --vibe-foreground: {dark.Foreground};
    --vibe-card: {dark.Card ?? dark.Primary};
    --vibe-card-foreground: {dark.CardForeground ?? dark.Foreground};
    --vibe-popover: {dark.Popover ?? dark.Primary};
    --vibe-popover-foreground: {dark.PopoverForeground ?? dark.Foreground};
    --vibe-primary: {dark.Primary};
    --vibe-primary-foreground: {dark.PrimaryForeground ?? dark.Foreground};
    --vibe-secondary: {dark.Secondary};
    --vibe-secondary-foreground: {dark.SecondaryForeground ?? dark.Foreground};
    --vibe-muted: {dark.Muted};
    --vibe-muted-foreground: {dark.MutedForeground ?? dark.Muted};
    --vibe-accent: {dark.Accent ?? dark.Secondary};
    --vibe-accent-foreground: {dark.AccentForeground ?? dark.Foreground};
    --vibe-destructive: {dark.Destructive ?? "hsl(0 62.8% 30.6%)"};
    --vibe-destructive-foreground: {dark.DestructiveForeground ?? dark.Foreground};
    --vibe-border: {dark.Border ?? dark.Secondary};
    --vibe-input: {dark.Input ?? dark.Secondary};
    --vibe-ring: {dark.Ring ?? dark.Primary};
}}";
    }

    private class ColorSet
    {
        public string Background { get; set; } = string.Empty;
        public string Foreground { get; set; } = string.Empty;
        public string Primary { get; set; } = string.Empty;
        public string Secondary { get; set; } = string.Empty;
        public string Muted { get; set; } = string.Empty;
        public string? Card { get; set; }
        public string? CardForeground { get; set; }
        public string? Popover { get; set; }
        public string? PopoverForeground { get; set; }
        public string? PrimaryForeground { get; set; }
        public string? SecondaryForeground { get; set; }
        public string? MutedForeground { get; set; }
        public string? Accent { get; set; }
        public string? AccentForeground { get; set; }
        public string? Destructive { get; set; }
        public string? DestructiveForeground { get; set; }
        public string? Border { get; set; }
        public string? Input { get; set; }
        public string? Ring { get; set; }
    }
}
