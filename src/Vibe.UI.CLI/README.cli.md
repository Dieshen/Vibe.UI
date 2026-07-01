# Vibe.UI CLI

CLI tool for adding Vibe.UI components to your Blazor projects.

## Installation

```bash
dotnet tool install -g Vibe.UI.CLI --version 1.0.0-beta
```

## Quick Start

```bash
# 1. Initialize Vibe.UI infrastructure
cd MyBlazorApp
vibe init

# This creates:
# - Vibe/ folder with base classes and services
# - wwwroot/js/vibe-theme.js

# 2. Add to Program.cs:
builder.Services.AddVibeUI();

# 3. Add components
vibe add button
vibe add input
vibe add card

# Components are now in Components/!
```

## Project Structure

After `vibe init`:
```
MyProject/
├── Vibe/                    ← Infrastructure
│   ├── Base/
│   ├── Services/
│   └── Themes/
├── Components/              ← Components (added via vibe add)
│   ├── Button.razor         ← Flat structure (default)
│   ├── Input.razor
│   └── Card.razor
├── wwwroot/
│   └── js/
│       └── vibe-theme.js
└── Program.cs
```

### Directory Structure Philosophy

**Vibe.UI uses a FLAT directory structure by default**, inspired by shadcn/ui:

✅ **Default (Flat Structure):**
```
Components/
├── Button.razor
├── Input.razor
├── Card.razor
├── Dialog.razor
└── Alert.razor
```

This design provides:
- **Simpler imports**: `@using MyApp.Components.Button` vs `@using MyApp.Components.Input.Button`
- **Easier discovery**: All components visible in one directory
- **Less cognitive load**: No need to remember which category a component belongs to
- **Cleaner autocomplete**: Better IDE experience
- **Scalable**: Works well even with 110 components

**Why not category subdirectories?**

The source code IS organized by category (Input/, Form/, DataDisplay/, etc.) for maintainer convenience. However, components are installed flat for user convenience.

**Want custom organization?** Use the `--output` flag:

```bash
# Custom category structure
vibe add button --output Components/Input
vibe add dialog --output Components/Overlay

# Result:
Components/
├── Input/
│   └── Button.razor
└── Overlay/
    └── Dialog.razor
```

## Features

✅ **Source code control** - All code in your project
✅ **Zero dependencies** - No Vibe.UI package required
✅ **Full customization** - Modify any component
✅ **Custom naming** - `vibe add button --name PrimaryButton`
✅ **Custom location** - `vibe add button --output UI/Components`

## Commands

### `vibe init`
Copies infrastructure (base classes, services, theme system) to `Vibe/` folder.

Options:
- `--minimal` - Core infrastructure only
- `--no-theme` - Skip theme system
- `--with-charts` - Include Chart.js support

### `vibe add <component>`
Adds a component to `Components/`.

Options:
- `--name <name>` - Rename component
- `--output <path>` - Custom output directory
- `--overwrite` - Overwrite existing files
- `-y` - Skip confirmation prompt

### `vibe list`
Shows all available components.

### `vibe update <component>`
Updates an existing component (shows diff, asks for confirmation).

## Available Components

**Input:** Button, Checkbox, ColorPicker, FileUpload, Input, InputOTP, MultiSelect, Radio, RadioGroup, Rating, Select, Slider, Switch, TagInput, TextArea, Toggle

**Form:** Form, FormField, Label, ValidatedInput, Combobox

**DataDisplay:** Avatar, Badge, Chart, DataTable, Progress, Table, Timeline

**Layout:** AspectRatio, Card, Separator

**Navigation:** Breadcrumb, Pagination, Tabs

**Overlay:** Dialog, Drawer, Popover, Tooltip

**Feedback:** Alert, Confetti, EmptyState, Skeleton, Spinner, Toast

**Advanced:** KanbanBoard, TreeView, VirtualScroll

**Disclosure:** Accordion, Carousel, Collapsible

**Theme:** ThemeToggle

And more! Run `vibe list` for the complete list.

## Documentation

Full documentation: https://narcoleptic-fox.github.io/Vibe.UI/

## Inspired by shadcn/ui

This CLI follows the shadcn/ui philosophy: copy source code instead of package dependencies.
You own the code and can customize it however you need.
