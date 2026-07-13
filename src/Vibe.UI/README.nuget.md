# Vibe.UI

Modern Blazor component library with 111 components. Inspired by shadcn/ui.

## Installation

```bash
dotnet add package Vibe.UI --version 1.0.0-beta
```

## Setup

### 1. Add Base CSS

Add to your `App.razor`, `_Host.cshtml`, or `index.html`:

```html
<head>
    <!-- Design tokens, themes, reset -->
    <link href="_content/Vibe.UI/css/vibe-base.css" rel="stylesheet" />

    <!-- Scoped component styles (required) -->
    <link href="_content/Vibe.UI/Vibe.UI.bundle.scp.css" rel="stylesheet" />
</head>
```

That's it! Component styles are scoped via `.razor.css` files and bundled into `Vibe.UI.bundle.scp.css`.

**Optional:** Add utility classes:

```html
<link href="_content/Vibe.UI/css/vibe-utilities.css" rel="stylesheet" />
```

### 2. Register services

```csharp
// Program.cs
builder.Services.AddVibeUI();
```

### 3. Use components

```razor
@using Vibe.UI.Components

<Button Variant="ButtonVariant.Primary">Click me</Button>
<Input Label="Name" @bind-Value="name" />
<Card>
  <h3>Card Title</h3>
  <p>Card content</p>
</Card>
```

## Alternative: CLI Approach (Recommended)

For full source code control, use the CLI instead:

```bash
dotnet tool install -g Vibe.UI.CLI --version 1.0.0-beta
vibe init
vibe add button
```

### CLI Benefits:
- ✅ Own the source code
- ✅ Customize any component
- ✅ No package dependency

### Package Benefits:
- ✅ Quick setup
- ✅ Automatic updates
- ✅ Smaller project size

Choose the approach that fits your needs!

## Chart.js Setup (Optional)

If using the `Chart` component, add Chart.js:

```html
<head>
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4"></script>
    <script src="_content/Vibe.UI/js/vibe-chart.js"></script>
</head>
```

## Features

- 111 production-ready components
- Built-in theming system (light/dark mode)
- Chart.js integration
- Form validation helpers
- ARIA and keyboard support across core interactive components
- Responsive design
- Minimal dependencies

## Documentation

Full documentation: https://narcoleptic-fox.github.io/Vibe.UI/
