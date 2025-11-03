# NuGet Packaging Plan for Vibe.UI (Standalone CLI - Single Source)

**Status:** Ready to implement
**Approach:** shadcn/ui style - copy source code, zero dependencies
**Date:** 2025-11-02

---

## Overview

Package **Vibe.UI.CLI** as a standalone dotnet tool that copies component source code into user projects with ZERO dependency on Vibe.UI package.

### Key Design Principles

1. ✅ **Single Source of Truth** - Components maintained ONLY in `src/Vibe.UI/Components/`
2. ✅ **Build-Time Inclusion** - CLI package references Vibe.UI files at pack time (no duplication in repo)
3. ✅ **Standalone Installation** - Users get full source code with `vibe init` + `vibe add`
4. ✅ **Simple Structure** - Infrastructure goes in `Vibe/` folder (not `Lib/Vibe/`)
5. ✅ **Optional Package** - Vibe.UI NuGet package is alternative for users who prefer pre-built components

---

## User Experience

### Installation Workflow

```bash
# 1. Install CLI tool (one-time)
dotnet tool install -g Vibe.UI.CLI

# 2. Initialize infrastructure in project
cd MyBlazorApp
vibe init

# 3. Add components as needed
vibe add button
vibe add input
vibe add card
```

### Resulting Project Structure

```
MyBlazorProject/
├── Vibe/                              # Created by vibe init
│   ├── Base/
│   │   └── ThemedComponentBase.cs
│   ├── Services/
│   │   ├── Dialog/
│   │   │   ├── IDialogService.cs
│   │   │   ├── DialogService.cs
│   │   │   ├── DialogParameters.cs
│   │   │   └── DialogEventArgs.cs
│   │   ├── Toast/
│   │   │   ├── IToastService.cs
│   │   │   ├── ToastService.cs
│   │   │   └── ToastEventArgs.cs
│   │   ├── LucideIcons.cs
│   │   ├── FormValidators.cs
│   │   ├── ChartDataBuilder.cs
│   │   └── DataTableExporter.cs
│   ├── Themes/
│   │   ├── Models/
│   │   │   ├── Theme.cs
│   │   │   └── ThemeOptions.cs
│   │   └── Services/
│   │       └── ThemeManager.cs
│   ├── ServiceCollectionExtensions.cs
│   └── _Imports.razor
├── Components/
│   └── Vibe/                          # Created by vibe add
│       ├── Input/
│       │   ├── Button.razor
│       │   ├── Button.razor.css
│       │   ├── Input.razor
│       │   └── ...
│       ├── Form/
│       └── ...
├── wwwroot/
│   └── js/
│       ├── themeInterop.js           # Created by vibe init
│       └── vibe-chart.js             # Optional
└── Program.cs                         # User adds: builder.Services.AddVibeUI()
```

---

## Repository Structure (Single Source)

```
Vibe.UI/
├── src/
│   ├── Vibe.UI/
│   │   ├── Components/                ← ONLY location for components
│   │   │   ├── Input/
│   │   │   │   ├── Button.razor
│   │   │   │   ├── Button.razor.css
│   │   │   │   └── ...
│   │   │   └── ...
│   │   ├── Base/
│   │   ├── Services/
│   │   ├── Themes/
│   │   ├── wwwroot/
│   │   ├── ServiceCollectionExtensions.cs
│   │   ├── _Imports.razor
│   │   └── Vibe.UI.csproj
│   └── Vibe.UI.CLI/
│       ├── Commands/
│       ├── Services/
│       ├── Vibe.UI.CLI.csproj        ← References ../Vibe.UI/ files
│       ├── README.cli.md
│       └── (NO Templates/ folder!)
├── icon.png
└── NUGET_PACKAGING_PLAN.md (this file)
```

**Key Point:** NO `Templates/` folder in repository - components referenced from `../Vibe.UI/` at pack time.

---

## Implementation Plan

### Step 1: Update Vibe.UI.CLI.csproj

**File:** `src/Vibe.UI.CLI/Vibe.UI.CLI.csproj`

Add build-time references to include Vibe.UI files in the CLI package:

```xml
<PropertyGroup>
  <PackAsTool>true</PackAsTool>
  <ToolCommandName>vibe</ToolCommandName>
  <PackageId>Vibe.UI.CLI</PackageId>
  <Version>1.0.0</Version>
  <Authors>Brock</Authors>
  <Description>CLI tool for adding Vibe.UI components to Blazor projects. Copies component source code into your project (shadcn/ui style) - no package dependencies required.</Description>
  <PackageTags>blazor;ui;components;cli;dotnet-tool;shadcn;vibe</PackageTags>
  <PackageLicenseExpression>MIT</PackageLicenseExpression>
  <RepositoryUrl>https://github.com/Dieshen/Vibe.UI</RepositoryUrl>
  <RepositoryType>git</RepositoryType>
  <PackageProjectUrl>https://github.com/Dieshen/Vibe.UI</PackageProjectUrl>
  <PackageReadmeFile>README.cli.md</PackageReadmeFile>
  <PackageIcon>icon.png</PackageIcon>
</PropertyGroup>

<ItemGroup>
  <!-- Components from Vibe.UI (packed into Templates/Components/) -->
  <None Include="../Vibe.UI/Components/**/*.razor"
        Pack="true"
        PackagePath="Templates/Components/%(RecursiveDir)" />
  <None Include="../Vibe.UI/Components/**/*.razor.css"
        Pack="true"
        PackagePath="Templates/Components/%(RecursiveDir)" />

  <!-- Infrastructure files (packed into Templates/Infrastructure/) -->
  <None Include="../Vibe.UI/Base/**/*.cs"
        Pack="true"
        PackagePath="Templates/Infrastructure/Base/%(RecursiveDir)" />
  <None Include="../Vibe.UI/Services/**/*.cs"
        Pack="true"
        PackagePath="Templates/Infrastructure/Services/%(RecursiveDir)" />
  <None Include="../Vibe.UI/Themes/**/*.cs"
        Pack="true"
        PackagePath="Templates/Infrastructure/Themes/%(RecursiveDir)" />
  <None Include="../Vibe.UI/ServiceCollectionExtensions.cs"
        Pack="true"
        PackagePath="Templates/Infrastructure/" />
  <None Include="../Vibe.UI/_Imports.razor"
        Pack="true"
        PackagePath="Templates/Infrastructure/" />

  <!-- JavaScript files -->
  <None Include="../Vibe.UI/wwwroot/themeInterop.js"
        Pack="true"
        PackagePath="Templates/wwwroot/js/" />
  <None Include="../Vibe.UI/wwwroot/js/vibe-chart.js"
        Pack="true"
        PackagePath="Templates/wwwroot/js/" />

  <!-- Package assets -->
  <None Include="../../icon.png" Pack="true" PackagePath="" />
  <None Include="README.cli.md" Pack="true" PackagePath="" />
</ItemGroup>
```

**Remove line 27-28** (existing Templates reference that doesn't exist).

---

### Step 2: Create README.cli.md

**File:** `src/Vibe.UI.CLI/README.cli.md`

```markdown
# Vibe.UI CLI

CLI tool for adding Vibe.UI components to your Blazor projects.

## Installation

```bash
dotnet tool install -g Vibe.UI.CLI
```

## Quick Start

```bash
# 1. Initialize Vibe.UI infrastructure
cd MyBlazorApp
vibe init

# This creates:
# - Vibe/ folder with base classes and services
# - wwwroot/js/themeInterop.js

# 2. Add to Program.cs:
builder.Services.AddVibeUI();

# 3. Add components
vibe add button
vibe add input
vibe add card

# Components are now in Components/Vibe/!
```

## Project Structure

After `vibe init`:
```
MyProject/
├── Vibe/                    ← Infrastructure
│   ├── Base/
│   ├── Services/
│   └── Themes/
├── Components/
│   └── Vibe/                ← Components (added via vibe add)
├── wwwroot/
│   └── js/
│       └── themeInterop.js
└── Program.cs
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
Adds a component to `Components/Vibe/`.

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

Full documentation: https://github.com/Dieshen/Vibe.UI

## Inspired by shadcn/ui

This CLI follows the shadcn/ui philosophy: copy source code instead of package dependencies.
You own the code and can customize it however you need.
```

---

### Step 3: Create README.nuget.md

**File:** `src/Vibe.UI/README.nuget.md`

```markdown
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
```

---

### Step 4: Update InitCommand.cs

**File:** `src/Vibe.UI.CLI/Commands/InitCommand.cs`

Modify the `ExecuteAsync` method to:

1. Create `Vibe/` directory structure (instead of `Lib/Vibe/`)
2. Copy all infrastructure files from `Templates/Infrastructure/`
3. Copy `themeInterop.js` to `wwwroot/js/`
4. Update or create `_Imports.razor`
5. Show success message with next steps

**Key changes:**
- Change all paths from `Lib/Vibe/` to `Vibe/`
- Copy from `Templates/Infrastructure/` to `Vibe/`
- Add instructions for `builder.Services.AddVibeUI()` in output

---

### Step 5: Update AddCommand.cs

**File:** `src/Vibe.UI.CLI/Commands/AddCommand.cs`

Update infrastructure check to look in `Vibe/` instead of `Lib/Vibe/`:

```csharp
// Check if vibe init was run
var infrastructurePath = Path.Combine(projectPath, "Vibe/Base/ThemedComponentBase.cs");
if (!File.Exists(infrastructurePath))
{
    AnsiConsole.MarkupLine("[yellow]Warning:[/] Vibe.UI infrastructure not found.");
    AnsiConsole.MarkupLine("Run [blue]vibe init[/] first.");
    return 1;
}
```

Components still install to `Components/Vibe/{Category}/`.

---

### Step 6: Update Vibe.UI.csproj

**File:** `src/Vibe.UI/Vibe.UI.csproj`

Update metadata to clarify optional nature:

```xml
<PropertyGroup>
  <PackageId>Vibe.UI</PackageId>
  <Version>1.0.0</Version>
  <Title>Vibe.UI - Modern Blazor Component Library</Title>
  <Authors>Brock</Authors>
  <Description>Pre-built Blazor component library with 93+ components (theming, forms, charts, and more).

PREFER CLI APPROACH? Use Vibe.UI.CLI for source code control:
  dotnet tool install -g Vibe.UI.CLI
  vibe init

This package is for users who prefer package references over managing source code.</Description>
  <PackageTags>blazor;components;ui;themes;charts;forms;shadcn</PackageTags>
  <PackageLicenseExpression>MIT</PackageLicenseExpression>
  <RepositoryUrl>https://github.com/Dieshen/Vibe.UI</RepositoryUrl>
  <RepositoryType>git</RepositoryType>
  <PackageProjectUrl>https://github.com/Dieshen/Vibe.UI</PackageProjectUrl>
  <PackageReadmeFile>README.nuget.md</PackageReadmeFile>
  <PackageIcon>icon.png</PackageIcon>
</PropertyGroup>

<ItemGroup>
  <None Include="../../icon.png" Pack="true" PackagePath="" />
  <None Include="README.nuget.md" Pack="true" PackagePath="" />
</ItemGroup>
```

---

### Step 7: Update Main README.md

**File:** `README.md`

Add "Two Ways to Use Vibe.UI" section near the top:

```markdown
## Installation

### Option 1: CLI (Recommended - Full Control)

Install components as source code you can customize:

```bash
# Install CLI tool
dotnet tool install -g Vibe.UI.CLI

# Initialize in your project
cd MyBlazorApp
vibe init

# Add components
vibe add button
vibe add input
vibe add card
```

**Benefits:**
- ✅ Full source code ownership
- ✅ Customize any component
- ✅ Zero package dependencies
- ✅ shadcn/ui style workflow

Infrastructure goes in `Vibe/`, components in `Components/Vibe/`.

### Option 2: NuGet Package

Use pre-built components from package:

```bash
dotnet add package Vibe.UI
```

**Benefits:**
- ✅ Quick setup
- ✅ Automatic updates
- ✅ Smaller project files
```

---

### Step 8: Create Package Icon (Optional)

**File:** `icon.png` (128x128px, repository root)

Design suggestions:
- Simple "V" monogram
- Waveform theme
- Modern, minimal
- Works on light/dark backgrounds

**For now:** Can skip or use placeholder. Not critical for initial testing.

---

## Testing Plan

### Test 1: CLI Package Build

```bash
cd src/Vibe.UI.CLI
dotnet pack -o ../../test-packages

# Verify:
# 1. .nupkg created
# 2. Extract and check Templates/ folder exists
# 3. Verify all 93 components included
```

### Test 2: CLI Installation

```bash
dotnet tool uninstall -g Vibe.UI.CLI  # Remove if already installed
dotnet tool install Vibe.UI.CLI -g --add-source ./test-packages --version 1.0.0
vibe --version  # Should show 1.0.0
```

### Test 3: Standalone Workflow

```bash
cd /tmp
dotnet new blazorwasm -n TestStandalone
cd TestStandalone

vibe init
# Verify: Vibe/ folder created with all infrastructure
# Verify: wwwroot/js/themeInterop.js created

vibe add button
# Verify: Components/Vibe/Input/Button.razor created
# Verify: Components/Vibe/Input/Button.razor.css created

vibe add input
vibe add dialog

# Add to Program.cs:
# builder.Services.AddVibeUI();

dotnet build
# Should succeed with ZERO Vibe.UI package reference!
```

### Test 4: Component Customization

```bash
# Edit Components/Vibe/Input/Button.razor
# Add custom logic, change styles
dotnet build
# Should work - proves source ownership
```

### Test 5: Package Workflow (comparison)

```bash
cd /tmp
dotnet new blazorwasm -n TestPackage
cd TestPackage

dotnet add package Vibe.UI --source ../../test-packages
# Add builder.Services.AddVibeUI() to Program.cs

dotnet build
# Should work - traditional package approach
```

---

## File Changes Checklist

### New Files:
- [ ] `src/Vibe.UI.CLI/README.cli.md`
- [ ] `src/Vibe.UI/README.nuget.md`
- [ ] `icon.png` (optional for now)

### Modified Files:
- [ ] `src/Vibe.UI.CLI/Vibe.UI.CLI.csproj`
- [ ] `src/Vibe.UI.CLI/Commands/InitCommand.cs`
- [ ] `src/Vibe.UI.CLI/Commands/AddCommand.cs`
- [ ] `src/Vibe.UI/Vibe.UI.csproj`
- [ ] `README.md`

---

## Success Criteria

- [ ] `dotnet pack src/Vibe.UI.CLI` creates valid .nupkg
- [ ] CLI package size reasonable (<5MB)
- [ ] CLI package includes Templates/Components/ with all 93 components
- [ ] CLI package includes Templates/Infrastructure/ with all base files
- [ ] `dotnet tool install Vibe.UI.CLI` works from test package
- [ ] `vibe init` creates `Vibe/` folder (not `Lib/Vibe/`)
- [ ] `vibe init` copies all infrastructure files
- [ ] `vibe add button` creates `Components/Vibe/Input/Button.razor`
- [ ] Fresh Blazor project + vibe workflow builds with ZERO Vibe.UI dependency
- [ ] All 162 existing CLI tests still pass
- [ ] Components maintained in ONLY one location (src/Vibe.UI/)

---

## Timeline Estimate

- **Step 1** (Update CLI csproj): 20 minutes
- **Step 2-3** (Create READMEs): 30 minutes
- **Step 4** (InitCommand update): 45 minutes
- **Step 5** (AddCommand update): 15 minutes
- **Step 6-7** (Update package metadata): 20 minutes
- **Step 8** (Testing): 45 minutes

**Total: ~3 hours**

---

## Notes

- Repository only contains components in `src/Vibe.UI/` (single source of truth)
- CLI package auto-includes them at pack time via MSBuild
- No duplication, no sync issues
- Users get full source code with `vibe init` + `vibe add`
- Vibe.UI NuGet package becomes optional alternative
