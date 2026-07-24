# Vibe.UI
[![build](https://img.shields.io/github/actions/workflow/status/Narcoleptic-Fox/Vibe.UI/ci.yml
)](https://github.com/Narcoleptic-Fox/Vibe.UI/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Vibe.UI.svg)](https://www.nuget.org/packages/Vibe.UI/)
[![codecov](https://codecov.io/gh/Narcoleptic-Fox/Vibe.UI/branch/main/graph/badge.svg)](https://codecov.io/gh/Narcoleptic-Fox/Vibe.UI)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

> **Beta Release (1.0.0-beta)** - This library is ready for broader dogfooding across supported Blazor hosting models. APIs may still change before stable 1.0. See [Known Limitations](#known-limitations) below.

A comprehensive Blazor component library inspired by shadcn/ui, built with Razor components and C#. It includes **111 Razor component files**, theming, Chart.js integration, form validation, icons, testing infrastructure, and CLI tooling.

Project documentation is indexed in [`docs/README.md`](docs/README.md).

> **Built for developers who want full control.** Copy components into your project and customize them, or use our NuGet package for quick integration.

## Features

### Components & Features
- **111 UI Components** - Comprehensive component library with accessibility-minded Input, Form, Data Display, Navigation, Overlay, Feedback, and Advanced components
- **Chart.js Integration** - Full-featured data visualization with 7 chart types (Line, Bar, Pie, Doughnut, Radar, PolarArea, Area)
- **Icon Library** - 70+ Lucide icons built-in with SVG support and customizable styling
- **Form Validation** - Built-in validators (email, phone, password strength, credit card, etc.) with real-time feedback
- **Comprehensive Theme Management** - Create, customize, and switch between themes at runtime
- **Built-in Light and Dark Themes** - Ready to use out of the box
- **Support for External CSS Frameworks** - Integrate with Material, Bootstrap, and Tailwind CSS
- **Customizable CSS Variables** - Fine-tune theming with an extensive set of CSS variables
- **Auto-Detection of System Theme** - Automatically adapt to user's OS theme preference
- **Pure Razor Components** - Built with Razor/C# with minimal JavaScript
- **Complete Theming API** - Programmatic control over themes via C# API
- **Theme Persistence** - Save user theme preferences across sessions

### Developer Tools
- **Vibe CLI** - Command-line tool to add components like shadcn (`vibe add button`); see the [CLI guide](docs/CLI.md)
- **Comprehensive Testing** - Unit and integration tests with bUnit and xUnit
- **Demo Application** - Routed component catalog with interactive examples

## Vibe.UI.CSS

Vibe.UI.CSS is the utility-first companion (Tailwind-style) for Vibe.UI.

- [Vibe.UI.CSS architecture and reference](docs/VIBE-UI-CSS.md)
- [Vibe.UI.CSS beta core profile](docs/Vibe.UI.CSS.CoreProfile.md)
- [Tailwind parity roadmap](docs/Vibe.UI.CSS.TailwindParity.md)

## shadcn/ui Parity

- [Vibe.UI parity roadmap](docs/Vibe.UI.ShadcnParity.md)
- [Vibe.UI component beta profile](docs/Vibe.UI.ComponentBetaProfile.md)

## Installation

Vibe.UI `1.0.0-beta` targets .NET 10. Install the .NET 10 SDK before using the
packages or CLI.

### Option 1: CLI (Recommended - Full Control)

Install components as source code you can customize:

```bash
# Install CLI tool
dotnet tool install -g Vibe.UI.CLI --version 1.0.0-beta

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

Infrastructure goes in `Vibe/`, and components go in `Components/vibe/` by
default.

### Option 2: NuGet Package

Use pre-built components from package:

```bash
dotnet add package Vibe.UI --version 1.0.0-beta
```

Load the required base and scoped component styles in `Components/App.razor`,
`index.html`, `_Host.cshtml`, or the equivalent root document for your hosting
model:

```html
<link href="_content/Vibe.UI/css/vibe-base.css" rel="stylesheet" />
<link href="_content/Vibe.UI/Vibe.UI.bundle.scp.css" rel="stylesheet" />
```

Register Vibe.UI in each server or client process that renders components:

```csharp
builder.Services.AddVibeUI();
```

See [CSS setup](docs/CSS-SETUP.md) and
[hosting compatibility](docs/Compatibility.md) for standalone WebAssembly and
hosted Blazor Web App layouts.

**Benefits:**
- ✅ Quick setup
- ✅ Automatic updates
- ✅ Smaller project files

## Quick Start

### Step 1: Add Components

After completing the package CSS and service setup above, use components from
`Vibe.UI.Components`:

```razor
@page "/"
@using Vibe.UI.Components

<Button Variant="ButtonVariant.Primary">Click Me</Button>
<Card>
    <h3>Hello Vibe.UI</h3>
    <p>Components work immediately - no setup needed!</p>
</Card>
```

Components render with the shared design tokens and scoped component styles.

### Step 2: Add Theme Toggle (Optional)

Want light/dark mode? Just add the ThemeToggle component anywhere:

```razor
<!-- Simple icon toggle -->
<ThemeToggle />

<!-- Toggle with label -->
<ThemeToggle ShowLabel="true" />
```

The ThemeToggle component:
- ✅ Automatically detects system preference
- ✅ Persists user choice to localStorage
- ✅ Works without any service registration
- ✅ Uses the packaged `vibe-theme.js` module for document-level theme state

### Step 3: Service Registration

Register Vibe.UI in each process that renders components. This is required for
service-backed components such as Toast and Dialog and keeps hosting behavior
consistent:

```csharp
using Vibe.UI;

builder.Services.AddVibeUI();
```

## Using Vibe.UI Components

### Button

```razor
<Button Variant="ButtonVariant.Primary" Size="ButtonSize.Medium">
    Click Me
</Button>
```

### Alert

```razor
<Alert Type="AlertType.Info" Title="Information" IsDismissible="true">
    This is an informational message.
</Alert>
```

### Card

```razor
<Card>
    <CardHeader>
        <CardTitle>Card Title</CardTitle>
        <CardDescription>Card Description</CardDescription>
    </CardHeader>
    <CardContent>
        Card content goes here.
    </CardContent>
    <CardFooter>
        <Button>Submit</Button>
    </CardFooter>
</Card>
```

### Icons

Use 70+ built-in Lucide icons:

```razor
<!-- Basic icon -->
<Icon Name="heart" Size="24" />

<!-- Colored icon -->
<Icon Name="check-circle" Color="#10b981" Size="32" />

<!-- Animated icon -->
<Icon Name="loader" CssClass="icon-spin" />

<!-- Available icons: menu, x, check, plus, edit, trash, search, user, home, heart, star, calendar, bell, mail, and 60+ more -->
```

### Charts

Full Chart.js integration for data visualization:

```razor
@using Vibe.UI.Components
@using Vibe.UI.Services
@using static Vibe.UI.Components.Chart

<!-- Line Chart -->
<Chart Data="@chartData"
       Type="ChartType.Line"
       Title="Sales Data"
       Height="400" />

@code {
    private ChartData chartData = new ChartDataBuilder()
        .WithLabels("Jan", "Feb", "Mar", "Apr", "May", "Jun")
        .AddDataset("Revenue", new[] { 65.0, 59.0, 80.0, 81.0, 56.0, 55.0 })
        .AddDataset("Costs", new[] { 28.0, 48.0, 40.0, 19.0, 86.0, 27.0 })
        .Build();
}

<!-- Chart types: Line, Bar, Pie, Doughnut, Radar, PolarArea, Area -->
<!-- Pie/Doughnut slice colors: set ChartDataset.BackgroundColors / BorderColors -->
<!-- See docs/CHARTS.md for complete documentation -->
```

### Form Validation

Built-in validation with real-time feedback:

```razor
@using Vibe.UI.Services

<ValidatedInput @bind-Value="email"
                Label="Email Address"
                InputType="email"
                Required="true"
                Validator="@FormValidators.Email()"
                HelperText="We'll never share your email"
                ShowValidationIcon="true" />

<ValidatedInput @bind-Value="password"
                Label="Password"
                InputType="password"
                Required="true"
                Validator="@FormValidators.StrongPassword(minLength: 8)"
                ShowValidationIcon="true" />

@code {
    private string email = "";
    private string password = "";

    // Available validators: Required, Email, Phone, Url, MinLength, MaxLength,
    // Range, Pattern, StrongPassword, CreditCard, FutureDate, PastDate, and more
}
```

### Theme Toggle

Simple theme toggle component for switching between light and dark modes:

```razor
<!-- Simple icon toggle -->
<ThemeToggle />

<!-- Toggle with label text -->
<ThemeToggle ShowLabel="true" />

<!-- Custom icons -->
<ThemeToggle>
    <DarkIcon>🌙</DarkIcon>
    <LightIcon>☀️</LightIcon>
</ThemeToggle>

<!-- Customize tooltips -->
<ThemeToggle
    DarkModeTooltip="Enable dark mode"
    LightModeTooltip="Enable light mode" />
```

The ThemeToggle component automatically:
- Detects system color scheme preference on first load
- Persists user selection to localStorage
- Applies the `.dark` class to `<html>` element
- Works without any service registration

## Creating Custom Themed Components

Extend the `VibeComponent` class to create your own themed components:

```razor
@inherits Vibe.UI.Base.VibeComponent

<div class="@CombinedClass">
    <!-- Your component's HTML -->
    <h1>My Custom Component</h1>
    @ChildContent
</div>

@code {
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    // Override to provide your component's CSS class
    protected override string ComponentClass => "my-custom-component";
}
```

## Theme Customization

Vibe.UI theming follows shadcn/ui-style CSS variables. The theme service and small JavaScript module coordinate runtime switching and persistence.

### How Theming Works

Theming is controlled via CSS variables and the `.dark` class on the document root:

- **Light mode**: CSS variables defined in `:root`
- **Dark mode**: CSS variables defined in `.dark` selector
- **Switching**: Simply toggle the `.dark` class on `<html>`

### Customizing Colors

Edit your CSS to customize the theme colors:

```css
:root {
  /* Light mode colors */
  --vibe-background: #ffffff;
  --vibe-foreground: #111111;
  --vibe-primary: #0066cc;
  --vibe-primary-foreground: #ffffff;
  /* ... more variables */
}

.dark {
  /* Dark mode colors */
  --vibe-background: #0f172a;
  --vibe-foreground: #f8fafc;
  --vibe-primary: #3b82f6;
  --vibe-primary-foreground: #ffffff;
  /* ... more variables */
}
```

### Using the CLI for Theme Setup

The `vibe init` command will help you set up theming with base color selection (similar to shadcn/ui):

```bash
vibe init

# Follow the prompts:
# ✔ Would you like to use TypeScript? no
# ✔ Which base color would you like to use? Slate
# ✔ Where is your global CSS file? wwwroot/css/app.css
```

This will generate the appropriate CSS variables for your chosen base color.

### Manual Theme Toggle

You can also toggle themes manually via JavaScript:

```javascript
// Toggle dark mode
document.documentElement.classList.toggle('dark');

// Or set explicitly
document.documentElement.classList.add('dark');    // Dark mode
document.documentElement.classList.remove('dark'); // Light mode
```

## CSS Variables

Vibe.UI uses CSS variables for theming. Here are the main variables:

| Variable                      | Purpose                            |
| ----------------------------- | ---------------------------------- |
| `--vibe-background`           | Background color                   |
| `--vibe-foreground`           | Text color                         |
| `--vibe-primary`              | Primary color                      |
| `--vibe-primary-foreground`   | Text color on primary background   |
| `--vibe-secondary`            | Secondary color                    |
| `--vibe-secondary-foreground` | Text color on secondary background |
| `--vibe-accent`               | Accent color                       |
| `--vibe-accent-foreground`    | Text color on accent background    |
| `--vibe-muted`                | Muted background color             |
| `--vibe-muted-foreground`     | Text color on muted background     |
| `--vibe-card`                 | Card background color              |
| `--vibe-card-foreground`      | Text color on card background      |
| `--vibe-border`               | Border color                       |
| `--vibe-input`                | Input field background color       |
| `--vibe-ring`                 | Focus ring color                   |
| `--vibe-radius`               | Border radius                      |
| `--vibe-font`                 | Font family                        |

## Available Components

Vibe.UI beta includes **111 Razor component files**, including composed roots,
items, and helper primitives:

### Layout Components
- **AspectRatio** - Container maintaining a specific aspect ratio
- **Card** - Versatile content container with header, body, and footer sections
- **Separator** - Visual divider for content separation
- **Resizable** - Panels with user-adjustable dimensions
- **Sheet** - Slide-in panel for secondary content

### Data Display Components
- **Avatar** - User or entity image representations
- **Badge** - Small status indicators or counts
- **Table** - Standard data table for structured information
- **DataTable** - Enhanced table with sorting, filtering, pagination, and CSV/JSON export
- **Progress** - Visual indicators of completion percentage or activity
- **Chart** - Full Chart.js integration with 7 chart types and real-time updates
- **Timeline** - Event timeline with status indicators and timestamps

### Navigation Components
- **Breadcrumb** - Path-based navigation indicators
- **Menubar** - Horizontal navigation system
- **NavigationMenu** - Hierarchical navigation structure
- **Pagination** - Page navigation controls
- **Tabs** - Content organization into selectable tabs

### Input Components
- **Button** - Clickable controls with multiple variants
- **Checkbox** - Binary selection controls
- **Input** - Text input fields
- **Radio** - Single-selection option buttons
- **RadioGroup** - Group of radio buttons with single selection
- **Select** - Dropdown selection menu
- **Slider** - Range selection control
- **Switch** - Toggle controls
- **TextArea** - Multi-line text input
- **Toggle** - Alternative toggle control
- **ToggleGroup** - Group of toggle buttons with single or multiple selection
- **ColorPicker** - Visual color selection tool with HSL/RGB/HEX support
- **MultiSelect** - Selection control for multiple options
- **ValidatedInput** - Input with built-in validation and real-time feedback
- **InputOTP** - One-time password input with auto-focus
- **FileUpload** - Drag-and-drop file upload with multiple file support
- **Rating** - Star rating component with half-star support
- **TagInput** - Multi-tag input field with suggestions
- **RichTextEditor** - WYSIWYG editor with formatting toolbar
- **Mentions** - @mention and #hashtag input with autocomplete
- **TransferList** - Dual-list selector for moving items between lists
- **ImageCropper** - Image cropping tool with zoom and rotation

### Disclosure Components
- **Accordion** - Expandable content sections
- **Collapsible** - Simple show/hide content panels
- **Carousel** - Content slideshow with navigation

### Overlay Components
- **AlertDialog** - Modal dialog requiring user attention
- **Dialog** - Standard modal window
- **Drawer** - Side-sliding panel
- **ContextMenu** - Right-click activated menus
- **HoverCard** - Content preview on hover
- **Popover** - Small content overlay on click
- **Tooltip** - Small information popup on hover

### Feedback Components
- **Alert** - Contextual feedback messages
- **Skeleton** - Loading state placeholders
- **Toast** - Temporary notification messages
- **Sonner** - Enhanced toast notifications with stacking and promise support
- **EmptyState** - Placeholder for empty content areas
- **Spinner** - Loading indicator with customizable sizes
- **NotificationCenter** - Centralized notification hub with badge and dropdown panel
- **Confetti** - Celebratory confetti animation with customizable particles

### Date & Time Components
- **Calendar** - Date selection calendar
- **DatePicker** - Date selection with popup calendar
- **DateRangePicker** - Selection control for date ranges

### Form Components
- **Form** - Organized form container
- **FormField** - Structured form field wrapper
- **FormLabel** - Accessible form input labels
- **FormMessage** - Validation and help text
- **Label** - Standalone accessible label component
- **Combobox** - Combined input and dropdown
- **ValidatedInput** - Input with validation, error messages, and icons
- **FormValidators** - Built-in validators (Email, Phone, URL, Password, CreditCard, etc.)

### Theme Components
- **ThemeToggle** - Simple light/dark mode toggle with system preference detection

### Layout Components
- **AspectRatio** - Container maintaining a specific aspect ratio
- **Card** - Versatile content container with header, body, and footer sections
- **Separator** - Visual divider for content separation
- **Resizable** - Panels with user-adjustable dimensions
- **Sheet** - Slide-in panel for secondary content
- **MasonryGrid** - Pinterest-style masonry grid layout for variable-height items
- **Splitter** - Resizable split pane divider with drag support

### Navigation Components
- **Breadcrumb** - Path-based navigation indicators
- **Menubar** - Horizontal navigation system
- **NavigationMenu** - Hierarchical navigation structure
- **Pagination** - Page navigation controls
- **Tabs** - Content organization into selectable tabs
- **Sidebar** - Collapsible sidebar with resize support

### Utility Components
- **Command** - Keyboard command palette interface
- **ScrollArea** - Custom scrollable container
- **DropdownMenu** - Context-specific dropdown menu
- **Icon** - 70+ Lucide icons with SVG support and customizable styling
- **Kbd** - Keyboard shortcut display component
- **QRCode** - Experimental QR-style visual preview for URLs and text

### Advanced Components
- **TreeView** - Hierarchical data display with expand/collapse
- **KanbanBoard** - Kanban board with draggable cards and columns
- **VirtualScroll** - Efficient rendering for large lists with virtual scrolling

## CLI Reference

The Vibe CLI provides several commands for working with components:

### Commands

- `vibe init` - Initialize Vibe.UI in your project
- `vibe add <component>` - Add a component to your project
- `vibe list` - List all available components
- `vibe update [component]` - Update components to latest version

### Options

- `-y, --yes` - Skip confirmation prompts
- `-p, --path <path>` - Specify project directory
- `-o, --overwrite` - Overwrite existing files

## Testing

Vibe.UI includes comprehensive testing infrastructure using bUnit and xUnit.

Run tests:

```bash
dotnet test tests/Vibe.UI.Tests/Vibe.UI.Tests.csproj
```

Example test structure:

```csharp
public class ButtonTests : TestContext
{
    public ButtonTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Button_RendersWithDefaultProps()
    {
        var cut = RenderComponent<Button>(parameters => parameters
            .Add(p => p.ChildContent, "Click me"));

        cut.Find("button").Should().NotBeNull();
        cut.Find("button").TextContent.Should().Be("Click me");
    }
}
```

## Contributing

We welcome contributions! Please see our [Contributing Guide](.github/CONTRIBUTING.md) for details on:

- Setting up your development environment
- Coding standards and best practices
- Submitting pull requests
- Creating new components
- Writing tests

Please read our [Code of Conduct](.github/CODE_OF_CONDUCT.md) before contributing.

## Support

- **Issues**: [GitHub Issues](https://github.com/Narcoleptic-Fox/Vibe.UI/issues)
- **Discussions**: [GitHub Discussions](https://github.com/Narcoleptic-Fox/Vibe.UI/discussions)
- **Documentation**: [Full Documentation](https://narcoleptic-fox.github.io/Vibe.UI/)

## Project Structure

```
Vibe.UI/
├── src/
│   ├── Vibe.UI/              # Core component library
│   └── Vibe.UI.CLI/          # Command-line tool
├── tests/
│   └── Vibe.UI.Tests/        # Unit and integration tests
├── samples/
│   └── Vibe.UI.Docs/         # Documentation site
└── README.md
```

## Acknowledgments

Vibe.UI is inspired by [shadcn/ui](https://ui.shadcn.com/) and built for the Blazor community.

## Security

Found a security vulnerability? Please review our [Security Policy](.github/SECURITY.md) for responsible disclosure guidelines.

## Known Limitations

As a beta release, Vibe.UI has some known limitations we're actively working on:

### Components
- **Accessibility evidence**: automated serious/critical Axe, keyboard, focus,
  and ARIA gates are in place, but formal WCAG conformance still requires
  manual screen-reader, 200% zoom/reflow, and forced-colors evaluation
- **Visual coverage**: reviewed Chromium/Windows baselines cover the six
  highest-risk surfaces; all 111 source components have direct test-file
  coverage, which is not a claim of equal visual or behavioral depth

### Vibe.UI.CSS
- **Compatibility scope**: beta guarantees the documented
  [core profile](docs/Vibe.UI.CSS.CoreProfile.md), not the complete Tailwind
  compiler or plugin ecosystem
- **Scanning**: complete class names must be present in scanned source; runtime
  concatenation and indirect attribute dictionaries are v1 roadmap work

### CLI
- **Template packaging**: CLI templates are bundled in the NuGet package; local development requires packaging first

For detailed roadmaps, see:
- [Documentation index](docs/README.md)
- [Component parity with shadcn/ui](docs/Vibe.UI.ShadcnParity.md)
- [Utility parity with Tailwind CSS](docs/Vibe.UI.CSS.TailwindParity.md)
- [Beta release checklist](docs/Beta-Readiness-Checklist.md)
- [Historical alpha release](docs/Alpha-0.1.0-Checklist.md)

## Sponsorship

Love Vibe.UI? Consider supporting its development:

- ☕ [Buy Me a Coffee](https://buymeacoffee.com/dieshen)
- 💖 [GitHub Sponsors](https://github.com/sponsors/Dieshen)

## License

MIT License - see [LICENSE](LICENSE) for details

