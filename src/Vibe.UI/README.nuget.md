# Vibe.UI

Modern Blazor component library with 93+ components.

## Installation

```bash
dotnet add package Vibe.UI
```

## Quick Start

```csharp
// Program.cs
builder.Services.AddVibeUI();
```

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
dotnet tool install -g Vibe.UI.CLI
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

## Features

- 93+ production-ready components
- Built-in theming system (light/dark mode)
- Chart.js integration
- Form validation helpers
- ARIA-compliant accessibility
- Responsive design
- Minimal dependencies

## Documentation

Full documentation: https://github.com/Dieshen/Vibe.UI
