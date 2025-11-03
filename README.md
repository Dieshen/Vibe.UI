# Vibe.UI
[![build](https://img.shields.io/github/actions/workflow/status/Dieshen/Vibe.UI/ci.yml
)](https://github.com/Dieshen/Vibe.UI/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Vibe.UI.svg)](https://www.nuget.org/packages/Vibe.UI/)
[![codecov](https://codecov.io/gh/Dieshen/Vibe.UI/branch/main/graph/badge.svg)](https://codecov.io/gh/Dieshen/Vibe.UI)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

A comprehensive, production-ready Blazor component library inspired by Shadcn UI, built with Razor components and C#. Complete with **90+ components**, comprehensive theming, Chart.js integration, form validation, icon library, testing infrastructure, CLI tooling, and IDE extensions.

> **Built for developers who want full control.** Copy components into your project and customize them, or use our NuGet package for quick integration.

## Features

### Components & Features
- **90+ Accessible UI Components** - Comprehensive component library with Input, Form, Data Display, Navigation, Overlay, Feedback, and Advanced components
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
- **Vibe CLI** - Command-line tool to add components like shadcn (`vibe add button`)
- **VS Code Extension** - Snippets, commands, and IntelliSense support
- **Visual Studio 2022 Extension** - Project templates, item templates, and integrated tooling
- **Comprehensive Testing** - Unit and integration tests with bUnit and xUnit
- **Demo Application** - Interactive showcase of all components

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

Infrastructure goes in `Vibe/`, components in `Components/vibe/`.

### Option 2: NuGet Package

Use pre-built components from package:

```bash
dotnet add package Vibe.UI
```

**Benefits:**
- ✅ Quick setup
- ✅ Automatic updates
- ✅ Smaller project files

### Register Services

In your `Program.cs` file (or `Startup.cs` for Blazor Server), register the Vibe.UI services:

```csharp
using Vibe.UI;

// ...

// Basic setup with default options
builder.Services.AddVibe.UI();

// OR with customized options
builder.Services.AddVibe.UI(options => {
    options.DefaultThemeId = "dark"; 
    options.PersistTheme = true;
    
    // Add external theme providers if needed
    options.AddMaterialTheme()
           .AddBootstrapTheme()
           .AddTailwindTheme();
});
```

### Add Theme Root Component

In your `App.razor` file, wrap your application with the `ThemeRoot` component:

```razor
@using Vibe.UI.Components

<ThemeRoot>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
            <FocusOnNavigate RouteData="@routeData" Selector="h1" />
        </Found>
        <NotFound>
            <PageTitle>Not found</PageTitle>
            <LayoutView Layout="@typeof(MainLayout)">
                <p role="alert">Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</ThemeRoot>
```

### Add JavaScript Import

In your `index.html` (for Blazor WebAssembly) or `_Host.cshtml` (for Blazor Server) file, make sure to add the Vibe.UI JavaScript:

```html
<script src="_content/Vibe.UI/themeInterop.js"></script>
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
@using Vibe.UI.Services

<!-- Line Chart -->
<Chart ChartData="@chartData"
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

### Theme Components

Choose from several theme-related components to enhance your user experience:

```razor
<!-- Simple theme toggle between light and dark -->
<ThemeToggle />

<!-- Complete dropdown theme selector -->
<ThemeSelector Label="Choose theme:" />

<!-- Full theme customization panel -->
<ThemePanel
    Title="Theme Settings"
    CollapsiblePanel="true"
    InitiallyExpanded="false"
    AllowCustomization="true"
    AllowCreateTheme="true" />
```

## Creating Custom Themed Components

Extend the `ThemedComponentBase` class to create your own themed components:

```razor
@inherits Vibe.UI.Base.ThemedComponentBase

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

### Creating Custom Themes

You can create custom themes programmatically:

```csharp
@inject ThemeManager ThemeManager

@code {
    protected override async Task OnInitializedAsync()
    {
        // Create a custom theme based on the light theme
        var customTheme = ThemeManager.CreateCustomTheme(
            "My Custom Theme",
            "light",
            new Dictionary<string, string>
            {
                ["--vibe-primary"] = "#ff5722",
                ["--vibe-accent"] = "#2196f3"
            });
            
        // Apply the custom theme
        await ThemeManager.SetThemeAsync(customTheme.Id);
    }
}
```

### Accessing the Current Theme

You can access the current theme in any component:

```razor
@inject ThemeManager ThemeManager

<p>Current theme: @ThemeManager.CurrentTheme?.Name</p>
```

### Using System Theme Detection

Enable automatic system theme detection in your `ThemeRoot` component:

```razor
<ThemeRoot AutoDetectSystemTheme="true">
    <!-- App content -->
</ThemeRoot>
```

### Adding External CSS Framework Themes

You can integrate external CSS frameworks:

```csharp
builder.Services.AddVibe.UI(options => {
    // Configure Material Design themes
    options.AddMaterialTheme(provider => {
        provider.Name = "Light";
        provider.CdnUrl = "https://cdn.jsdelivr.net/npm/@material/material-components-web@14.0.0/dist/material-components-web.min.css";
    });
    
    // Configure Bootstrap themes
    options.AddBootstrapTheme(provider => {
        provider.Name = "Default";
        provider.CdnUrl = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css";
    });
    
    // Configure Tailwind themes
    options.AddTailwindTheme(provider => {
        provider.Name = "Default";
        provider.CdnUrl = "https://cdn.jsdelivr.net/npm/tailwindcss@2.2.19/dist/tailwind.min.css";
    });
});
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

Vibe.UI includes a comprehensive set of **90+ production-ready components**:

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
- **ThemeRoot** - Root theme provider component
- **ThemeSelector** - UI for selecting themes
- **ThemeToggle** - Light/dark mode toggle
- **ThemePanel** - Complete theme customization panel

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
- **QRCode** - QR code generator for URLs and text

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

- **Issues**: [GitHub Issues](https://github.com/Dieshen/Vibe.UI/issues)
- **Discussions**: [GitHub Discussions](https://github.com/Dieshen/Vibe.UI/discussions)
- **Documentation**: [Full Documentation](https://github.com/Dieshen/Vibe.UI)

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

## Sponsorship

Love Vibe.UI? Consider supporting its development:

- ☕ [Buy Me a Coffee](https://buymeacoffee.com/dieshen)
- 💖 [GitHub Sponsors](https://github.com/sponsors/Dieshen)

## License

MIT License - see [LICENSE](LICENSE) for details