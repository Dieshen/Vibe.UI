# Vibe.UI Theming Guide

## Overview

Vibe.UI supports two approaches for theming: **CLI-based** (via `vibe init`) and **Library-based** (via NuGet package). Both use pure CSS with CSS variables following the shadcn/ui pattern.

---

## Option 1: CLI Approach (Recommended for Copy-Paste Workflow)

### Setup

```bash
# Initialize Vibe.UI with theme selection
vibe init

# Choose your base color when prompted:
# - Slate (default)
# - Gray
# - Zinc
# - Neutral
# - Stone
# - Blue
```

This generates CSS variables directly in your `app.css`:

```css
@layer vibe {
  :root {
    --vibe-background: hsl(0 0% 100%);
    --vibe-foreground: hsl(222.2 84% 4.9%);
    --vibe-primary: hsl(222.2 47.4% 11.2%);
    /* ...other variables... */
  }

  .dark {
    --vibe-background: hsl(222.2 84% 4.9%);
    --vibe-foreground: hsl(210 40% 98%);
    /* ...dark mode overrides... */
  }
}
```

### Add Components

```bash
vibe add button
vibe add card
```

---

## Option 2: Library Approach (NuGet Package)

### Installation

```bash
dotnet add package Vibe.UI
```

### Basic Setup (No Theme Configuration)

```csharp
// Program.cs
builder.Services.AddVibeUI();
```

```html
<!-- index.html or _Layout.cshtml -->
<link href="_content/Vibe.UI/css/vibe-components.css" rel="stylesheet" />
```

This uses the default theme built into Vibe.UI.

---

### Advanced Setup (With Theme Configuration)

Configure the theme in DI registration:

```csharp
// Program.cs
builder.Services.AddVibeUI(options =>
{
    options.BaseColor = "Slate"; // Or: Gray, Zinc, Neutral, Stone, Blue, Custom
    options.BorderRadius = "0.75rem"; // Optional: customize border radius
});
```

Then add the `<ThemeProvider />` component to your layout:

```razor
<!-- App.razor or MainLayout.razor -->
@using Vibe.UI.Components

<ThemeProvider />

<!-- Your app content here -->
```

#### Custom Colors

For complete control, use custom colors:

```csharp
builder.Services.AddVibeUI(options =>
{
    options.BaseColor = "Custom";
    options.LightColors = new CustomThemeColors
    {
        Background = "hsl(0 0% 100%)",
        Foreground = "hsl(240 10% 3.9%)",
        Primary = "hsl(346 77% 50%)", // Custom red primary
        Secondary = "hsl(240 4.8% 95.9%)",
        Accent = "hsl(240 4.8% 95.9%)",
        Destructive = "hsl(0 84.2% 60.2%)",
        Border = "hsl(240 5.9% 90%)"
    };
    options.DarkColors = new CustomThemeColors
    {
        Background = "hsl(240 10% 3.9%)",
        Foreground = "hsl(0 0% 98%)",
        Primary = "hsl(346 77% 65%)", // Lighter red for dark mode
        Secondary = "hsl(240 3.7% 15.9%)",
        // ...other dark colors...
    };
});
```

---

## Dark Mode

Dark mode works the same way in both approaches using pure CSS:

### Add ThemeToggle Component

```razor
<ThemeToggle />
```

This toggles the `.dark` class on the `<html>` element. All CSS variables automatically switch to their dark variants.

### Manual Dark Mode Control

```csharp
@inject IJSRuntime JS

private async Task ToggleDarkMode()
{
    await JS.InvokeVoidAsync("eval",
        "document.documentElement.classList.toggle('dark')");
}
```

---

## Comparison: CLI vs Library

| Feature | **CLI (`vibe init`)** | **Library (NuGet)** |
|---------|----------------------|---------------------|
| **Setup** | `vibe init` generates CSS | Reference package CSS or use `ThemeProvider` |
| **Theme Selection** | Interactive prompt | Configure in `Program.cs` |
| **Custom Colors** | Edit generated CSS | Configure via `CustomThemeColors` |
| **Components** | Copy files to your project | Use from package |
| **Updates** | Manual (re-run commands) | Automatic (NuGet update) |
| **Flexibility** | Full control over code | Configuration-based |

---

## Available Base Colors

All base colors follow shadcn/ui color palettes:

- **Slate** - Cool gray with subtle blue undertones (default)
- **Gray** - Pure neutral gray
- **Zinc** - Modern gray with slight warmth
- **Neutral** - Balanced neutral tones
- **Stone** - Warm gray with brown undertones
- **Blue** - Vibrant blue primary color

---

## CSS Variables Reference

### Core Colors
```css
--vibe-background       /* Page background */
--vibe-foreground       /* Main text color */
--vibe-primary          /* Primary brand color */
--vibe-secondary        /* Secondary surfaces */
--vibe-muted            /* Muted backgrounds */
--vibe-accent           /* Accent elements */
--vibe-destructive      /* Error/danger color */
```

### Semantic Colors
```css
--vibe-card             /* Card backgrounds */
--vibe-border           /* Border colors */
--vibe-input            /* Input backgrounds */
--vibe-ring             /* Focus ring color */
```

### Component-Specific Foregrounds
```css
--vibe-primary-foreground
--vibe-secondary-foreground
--vibe-muted-foreground
--vibe-accent-foreground
--vibe-destructive-foreground
--vibe-card-foreground
```

---

## Examples

### Example 1: Slate Theme (CLI)

```bash
vibe init
# Select: Slate
```

### Example 2: Custom Brand Colors (Library)

```csharp
builder.Services.AddVibeUI(options =>
{
    options.BaseColor = "Custom";
    options.LightColors = new CustomThemeColors
    {
        Primary = "hsl(262 83% 58%)",    // Purple
        Accent = "hsl(174 72% 56%)",     // Teal
        Destructive = "hsl(351 95% 71%)" // Pink
    };
});
```

### Example 3: Zinc with Larger Border Radius

```csharp
builder.Services.AddVibeUI(options =>
{
    options.BaseColor = "Zinc";
    options.BorderRadius = "1rem"; // 16px instead of default 8px
});
```

---

## Best Practices

1. **Choose One Approach**: Use either CLI or Library approach, not both
2. **Consistent Border Radius**: Keep border radius consistent across your app
3. **Test Dark Mode**: Always test your theme in both light and dark modes
4. **Accessibility**: Ensure sufficient contrast ratios (use tools like WebAIM)
5. **Custom Colors**: When using custom colors, maintain consistent hue relationships

---

## Troubleshooting

### Theme not applying?

**CLI Approach:**
- Ensure `app.css` is linked in your HTML
- Check that `@layer vibe` block exists in your CSS

**Library Approach:**
- Verify `<ThemeProvider />` is added to your layout
- Ensure `AddVibeUI()` with theme configuration is called in `Program.cs`

### Dark mode not working?

- Check that `<ThemeToggle />` component is present
- Verify `.dark` class is being toggled on `<html>` element (inspect in DevTools)
- Ensure dark mode CSS variables are defined

### Colors look wrong?

- Check HSL values are formatted correctly: `hsl(hue saturation% lightness%)`
- Verify no conflicting CSS is overriding Vibe.UI variables
- Clear browser cache and hard refresh
