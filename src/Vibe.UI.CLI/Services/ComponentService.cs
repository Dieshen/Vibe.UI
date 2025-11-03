using Vibe.UI.CLI.Models;

namespace Vibe.UI.CLI.Services;

public class ComponentService
{
    private readonly Dictionary<string, ComponentInfo> _components;

    public ComponentService()
    {
        _components = InitializeComponents();
    }

    public List<ComponentInfo> GetAvailableComponents()
    {
        return _components.Values.OrderBy(c => c.Category).ThenBy(c => c.Name).ToList();
    }

    public ComponentInfo? GetComponent(string name)
    {
        var key = name.ToLowerInvariant();
        return _components.TryGetValue(key, out var component) ? component : null;
    }

    public List<string> GetInstalledComponents(string projectPath, string componentsDir)
    {
        var installedPath = Path.Combine(projectPath, componentsDir);

        if (!Directory.Exists(installedPath))
            return new List<string>();

        var razorFiles = Directory.GetFiles(installedPath, "*.razor", SearchOption.AllDirectories);
        return razorFiles.Select(Path.GetFileNameWithoutExtension).ToList();
    }

    public async Task InstallComponentAsync(
        string projectPath,
        string componentsDir,
        string componentName,
        bool overwrite = false,
        string? customName = null,
        string? customOutputDir = null)
    {
        var component = GetComponent(componentName);
        if (component == null)
            throw new InvalidOperationException($"Component '{componentName}' not found.");

        // Determine target directory - use custom output dir or default flat structure (like shadcn/ui)
        var targetDir = string.IsNullOrEmpty(customOutputDir)
            ? Path.Combine(projectPath, componentsDir)
            : Path.Combine(projectPath, customOutputDir);

        Directory.CreateDirectory(targetDir);

        // Determine component file name - use custom name or original name
        var outputComponentName = customName ?? component.Name;

        // Get source component path
        var sourcePath = GetComponentSourcePath(component.Name, component.Category);

        // Copy .razor file
        var razorFile = Path.Combine(targetDir, $"{outputComponentName}.razor");
        if (!File.Exists(razorFile) || overwrite)
        {
            var razorContent = await GetComponentContentAsync(sourcePath, component.Name);

            // Rename component if custom name provided
            if (!string.IsNullOrEmpty(customName))
            {
                razorContent = RenameComponentInContent(razorContent, component.Name, customName);
            }

            await File.WriteAllTextAsync(razorFile, razorContent);
        }

        // Copy .razor.css file if it exists
        var sourceCssPath = $"{sourcePath}.css";
        if (File.Exists(sourceCssPath))
        {
            var cssFile = Path.Combine(targetDir, $"{outputComponentName}.razor.css");
            if (!File.Exists(cssFile) || overwrite)
            {
                var cssContent = await File.ReadAllTextAsync(sourceCssPath);

                // Update CSS class names if renamed
                if (!string.IsNullOrEmpty(customName))
                {
                    cssContent = RenameCssClasses(cssContent, component.Name, customName);
                }

                await File.WriteAllTextAsync(cssFile, cssContent);
            }
        }
    }

    private Dictionary<string, ComponentInfo> InitializeComponents()
    {
        var components = new Dictionary<string, ComponentInfo>
        {
            // Input Components
            ["button"] = new ComponentInfo { Name = "Button", Category = "Input", Description = "Displays a button or a component that looks like a button" },
            ["checkbox"] = new ComponentInfo { Name = "Checkbox", Category = "Input", Description = "A control that allows the user to toggle between checked and not checked" },
            ["input"] = new ComponentInfo { Name = "Input", Category = "Input", Description = "Displays a form input field or a component that looks like an input field" },
            ["radio"] = new ComponentInfo { Name = "Radio", Category = "Input", Description = "A radio button control" },
            ["radiogroup"] = new ComponentInfo { Name = "RadioGroup", Category = "Input", Description = "A set of checkable buttons where no more than one can be checked at a time", Dependencies = new List<string> { "RadioGroupItem" } },
            ["radiogroupitem"] = new ComponentInfo { Name = "RadioGroupItem", Category = "Input", Description = "An item within a radio group" },
            ["select"] = new ComponentInfo { Name = "Select", Category = "Input", Description = "Displays a list of options for the user to pick from" },
            ["slider"] = new ComponentInfo { Name = "Slider", Category = "Input", Description = "An input where the user selects a value from within a given range" },
            ["switch"] = new ComponentInfo { Name = "Switch", Category = "Input", Description = "A control that allows the user to toggle between checked and not checked" },
            ["textarea"] = new ComponentInfo { Name = "TextArea", Category = "Input", Description = "Displays a form textarea or a component that looks like a textarea" },
            ["toggle"] = new ComponentInfo { Name = "Toggle", Category = "Input", Description = "A two-state button that can be either on or off" },
            ["togglegroup"] = new ComponentInfo { Name = "ToggleGroup", Category = "Input", Description = "A set of two-state buttons that can be toggled on or off", Dependencies = new List<string> { "ToggleGroupItem" } },
            ["togglegroupitem"] = new ComponentInfo { Name = "ToggleGroupItem", Category = "Input", Description = "An item within a toggle group" },
            ["colorpicker"] = new ComponentInfo { Name = "ColorPicker", Category = "Input", Description = "A color picker component for selecting colors" },
            ["multiselect"] = new ComponentInfo { Name = "MultiSelect", Category = "Input", Description = "Select multiple items from a dropdown" },
            ["inputotp"] = new ComponentInfo { Name = "InputOTP", Category = "Input", Description = "One-time password input with auto-focus and keyboard navigation" },
            ["fileupload"] = new ComponentInfo { Name = "FileUpload", Category = "Input", Description = "Drag-and-drop file upload with multiple file support" },
            ["rating"] = new ComponentInfo { Name = "Rating", Category = "Input", Description = "Star rating component with half-star support" },
            ["taginput"] = new ComponentInfo { Name = "TagInput", Category = "Input", Description = "Multi-tag input field with suggestions" },
            ["richtexteditor"] = new ComponentInfo { Name = "RichTextEditor", Category = "Input", Description = "WYSIWYG rich text editor with formatting toolbar" },
            ["mentions"] = new ComponentInfo { Name = "Mentions", Category = "Input", Description = "@mention and #hashtag input with autocomplete" },
            ["transferlist"] = new ComponentInfo { Name = "TransferList", Category = "Input", Description = "Dual-list selector for moving items between lists" },
            ["imagecropper"] = new ComponentInfo { Name = "ImageCropper", Category = "Input", Description = "Image cropping tool with zoom and rotation" },

            // Form Components
            ["form"] = new ComponentInfo { Name = "Form", Category = "Form", Description = "Building forms with validation" },
            ["formfield"] = new ComponentInfo { Name = "FormField", Category = "Form", Description = "A form field wrapper" },
            ["formlabel"] = new ComponentInfo { Name = "FormLabel", Category = "Form", Description = "Renders an accessible label associated with controls" },
            ["formmessage"] = new ComponentInfo { Name = "FormMessage", Category = "Form", Description = "Displays form validation messages" },
            ["label"] = new ComponentInfo { Name = "Label", Category = "Form", Description = "Renders an accessible label associated with controls" },
            ["combobox"] = new ComponentInfo { Name = "Combobox", Category = "Form", Description = "Autocomplete input and command palette with a list of suggestions" },
            ["validatedinput"] = new ComponentInfo { Name = "ValidatedInput", Category = "Form", Description = "Input field with integrated validation display" },

            // Data Display
            ["avatar"] = new ComponentInfo { Name = "Avatar", Category = "DataDisplay", Description = "An image element with a fallback for representing the user" },
            ["badge"] = new ComponentInfo { Name = "Badge", Category = "DataDisplay", Description = "Displays a badge or a component that looks like a badge" },
            ["datatable"] = new ComponentInfo { Name = "DataTable", Category = "DataDisplay", Description = "Powerful table with sorting, filtering, and pagination" },
            ["progress"] = new ComponentInfo { Name = "Progress", Category = "DataDisplay", Description = "Displays an indicator showing the completion progress of a task" },
            ["table"] = new ComponentInfo { Name = "Table", Category = "DataDisplay", Description = "A responsive table component" },
            ["chart"] = new ComponentInfo { Name = "Chart", Category = "DataDisplay", Description = "Data visualization charts" },
            ["timeline"] = new ComponentInfo { Name = "Timeline", Category = "DataDisplay", Description = "Event timeline with status indicators and timestamps" },

            // Layout
            ["aspectratio"] = new ComponentInfo { Name = "AspectRatio", Category = "Layout", Description = "Displays content within a desired aspect ratio" },
            ["card"] = new ComponentInfo { Name = "Card", Category = "Layout", Description = "Displays a card with header, content, and footer" },
            ["separator"] = new ComponentInfo { Name = "Separator", Category = "Layout", Description = "Visually or semantically separates content" },
            ["resizable"] = new ComponentInfo { Name = "Resizable", Category = "Layout", Description = "Accessible resizable panel groups and layouts" },
            ["sheet"] = new ComponentInfo { Name = "Sheet", Category = "Layout", Description = "Extends the Dialog component to display content that complements the main content" },
            ["masonrygrid"] = new ComponentInfo { Name = "MasonryGrid", Category = "Layout", Description = "Pinterest-style masonry grid layout for variable-height items" },
            ["splitter"] = new ComponentInfo { Name = "Splitter", Category = "Layout", Description = "Resizable split pane divider with drag support" },

            // Navigation
            ["breadcrumb"] = new ComponentInfo { Name = "Breadcrumb", Category = "Navigation", Description = "Displays the path to the current resource using a hierarchy of links", Dependencies = new List<string> { "BreadcrumbItem" } },
            ["breadcrumbitem"] = new ComponentInfo { Name = "BreadcrumbItem", Category = "Navigation", Description = "An item within a breadcrumb" },
            ["menubar"] = new ComponentInfo { Name = "Menubar", Category = "Navigation", Description = "A visually persistent menu common in desktop applications" },
            ["sidebar"] = new ComponentInfo { Name = "Sidebar", Category = "Navigation", Description = "Collapsible sidebar with resize support" },
            ["navigationmenu"] = new ComponentInfo { Name = "NavigationMenu", Category = "Navigation", Description = "A collection of links for navigating websites", Dependencies = new List<string> { "NavigationMenuItem" } },
            ["navigationmenuitem"] = new ComponentInfo { Name = "NavigationMenuItem", Category = "Navigation", Description = "An item within a navigation menu" },
            ["pagination"] = new ComponentInfo { Name = "Pagination", Category = "Navigation", Description = "Pagination with page navigation, next and previous links" },
            ["tabs"] = new ComponentInfo { Name = "Tabs", Category = "Navigation", Description = "A set of layered sections of content", Dependencies = new List<string> { "TabItem" } },
            ["tabitem"] = new ComponentInfo { Name = "TabItem", Category = "Navigation", Description = "An item within tabs" },

            // Overlay
            ["alertdialog"] = new ComponentInfo { Name = "AlertDialog", Category = "Overlay", Description = "A modal dialog that interrupts the user with important content" },
            ["contextmenu"] = new ComponentInfo { Name = "ContextMenu", Category = "Overlay", Description = "Displays a menu to the user triggered by right-click", Dependencies = new List<string> { "ContextMenuItem" } },
            ["contextmenuitem"] = new ComponentInfo { Name = "ContextMenuItem", Category = "Overlay", Description = "An item within a context menu" },
            ["dialog"] = new ComponentInfo { Name = "Dialog", Category = "Overlay", Description = "A window overlaid on either the primary window or another dialog" },
            ["drawer"] = new ComponentInfo { Name = "Drawer", Category = "Overlay", Description = "A panel that slides out from the edge of the screen" },
            ["hovercard"] = new ComponentInfo { Name = "HoverCard", Category = "Overlay", Description = "For sighted users to preview content available behind a link" },
            ["popover"] = new ComponentInfo { Name = "Popover", Category = "Overlay", Description = "Displays rich content in a portal, triggered by a button" },
            ["tooltip"] = new ComponentInfo { Name = "Tooltip", Category = "Overlay", Description = "A popup that displays information related to an element" },
            ["dialogcontainer"] = new ComponentInfo { Name = "DialogContainer", Category = "Overlay", Description = "Container for managing multiple dialog instances" },

            // Feedback
            ["alert"] = new ComponentInfo { Name = "Alert", Category = "Feedback", Description = "Displays a callout for user attention" },
            ["skeleton"] = new ComponentInfo { Name = "Skeleton", Category = "Feedback", Description = "Use to show a placeholder while content is loading" },
            ["toast"] = new ComponentInfo { Name = "Toast", Category = "Feedback", Description = "A succinct message that is displayed temporarily", Dependencies = new List<string> { "ToastContainer" } },
            ["toastcontainer"] = new ComponentInfo { Name = "ToastContainer", Category = "Feedback", Description = "Container for toast notifications" },
            ["sonner"] = new ComponentInfo { Name = "Sonner", Category = "Feedback", Description = "Enhanced toast notifications with stacking and promise support" },
            ["emptystate"] = new ComponentInfo { Name = "EmptyState", Category = "Feedback", Description = "Placeholder for empty content areas" },
            ["spinner"] = new ComponentInfo { Name = "Spinner", Category = "Feedback", Description = "Loading indicator with customizable sizes" },
            ["notificationcenter"] = new ComponentInfo { Name = "NotificationCenter", Category = "Feedback", Description = "Centralized notification hub with badge and dropdown panel" },
            ["confetti"] = new ComponentInfo { Name = "Confetti", Category = "Feedback", Description = "Celebratory confetti animation with customizable particles" },

            // Date & Time
            ["calendar"] = new ComponentInfo { Name = "Calendar", Category = "DateTime", Description = "A date field component that allows users to enter and edit date" },
            ["datepicker"] = new ComponentInfo { Name = "DatePicker", Category = "DateTime", Description = "A date picker component with calendar dropdown" },
            ["daterangepicker"] = new ComponentInfo { Name = "DateRangePicker", Category = "DateTime", Description = "A date range picker component" },

            // Utility
            ["command"] = new ComponentInfo { Name = "Command", Category = "Utility", Description = "Fast, composable, unstyled command menu" },
            ["scrollarea"] = new ComponentInfo { Name = "ScrollArea", Category = "Utility", Description = "Augments native scroll functionality for custom, cross-browser styling" },
            ["dropdownmenu"] = new ComponentInfo { Name = "DropdownMenu", Category = "Utility", Description = "Displays a menu to the user triggered by a button" },
            ["kbd"] = new ComponentInfo { Name = "Kbd", Category = "Utility", Description = "Keyboard shortcut display component" },
            ["qrcode"] = new ComponentInfo { Name = "QRCode", Category = "Utility", Description = "QR code generator for URLs and text" },
            ["icon"] = new ComponentInfo { Name = "Icon", Category = "Utility", Description = "Lucide icon component with size and color customization" },

            // Advanced Components
            ["treeview"] = new ComponentInfo { Name = "TreeView", Category = "Advanced", Description = "Hierarchical data display with expand/collapse", Dependencies = new List<string> { "TreeViewNode" } },
            ["treeviewnode"] = new ComponentInfo { Name = "TreeViewNode", Category = "Advanced", Description = "Individual node component within TreeView hierarchy" },
            ["kanbanboard"] = new ComponentInfo { Name = "KanbanBoard", Category = "Advanced", Description = "Kanban board with draggable cards and columns" },
            ["virtualscroll"] = new ComponentInfo { Name = "VirtualScroll", Category = "Advanced", Description = "Efficient rendering for large lists with virtual scrolling" },

            // Disclosure
            ["accordion"] = new ComponentInfo { Name = "Accordion", Category = "Disclosure", Description = "A vertically stacked set of interactive headings", Dependencies = new List<string> { "AccordionItem" } },
            ["accordionitem"] = new ComponentInfo { Name = "AccordionItem", Category = "Disclosure", Description = "An item within an accordion" },
            ["carousel"] = new ComponentInfo { Name = "Carousel", Category = "Disclosure", Description = "A carousel with motion and swipe support", Dependencies = new List<string> { "CarouselItem" } },
            ["carouselitem"] = new ComponentInfo { Name = "CarouselItem", Category = "Disclosure", Description = "An item within a carousel" },
            ["collapsible"] = new ComponentInfo { Name = "Collapsible", Category = "Disclosure", Description = "An interactive component which expands/collapses a panel" },

            // Theme
            ["themepanel"] = new ComponentInfo { Name = "ThemePanel", Category = "Theme", Description = "Theme customization panel with color and radius controls" },
            ["themeroot"] = new ComponentInfo { Name = "ThemeRoot", Category = "Theme", Description = "Root theme provider component for managing application theme" },
            ["themeselector"] = new ComponentInfo { Name = "ThemeSelector", Category = "Theme", Description = "Theme selector dropdown for switching between themes" },
            ["themetoggle"] = new ComponentInfo { Name = "ThemeToggle", Category = "Theme", Description = "Dark/light mode toggle button" },
        };

        return components;
    }

    /// <summary>
    /// Gets the source path for a component file.
    /// Attempts multiple locations to find the component.
    /// </summary>
    private string GetComponentSourcePath(string componentName, string category)
    {
        // Get the directory where the CLI assembly is located
        var assemblyLocation = Path.GetDirectoryName(typeof(ComponentService).Assembly.Location);
        if (assemblyLocation == null)
            throw new InvalidOperationException("Could not determine CLI assembly location");

        // Try multiple possible paths for component sources
        var possiblePaths = new[]
        {
            // 1. Development mode: relative to CLI project
            Path.GetFullPath(Path.Combine(assemblyLocation, "..", "..", "..", "..", "..", "src", "Vibe.UI", "Components", category, $"{componentName}.razor")),

            // 2. Packaged with CLI in Templates folder (adjacent to tools folder)
            Path.Combine(assemblyLocation, "Templates", "Components", category, $"{componentName}.razor"),

            // 3. Dotnet global tool: Templates folder in package root (../../.. from tools/net9.0/any)
            Path.GetFullPath(Path.Combine(assemblyLocation, "..", "..", "..", "Templates", "Components", category, $"{componentName}.razor")),

            // 4. Current directory structure (if running from repo root)
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "src", "Vibe.UI", "Components", category, $"{componentName}.razor"))
        };

        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        throw new FileNotFoundException(
            $"Component source file not found for {componentName}. " +
            $"Searched locations: {string.Join(", ", possiblePaths)}");
    }

    /// <summary>
    /// Reads the component content from the source file.
    /// </summary>
    private async Task<string> GetComponentContentAsync(string sourcePath, string componentName)
    {
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException($"Component source file not found: {sourcePath}");
        }

        return await File.ReadAllTextAsync(sourcePath);
    }

    /// <summary>
    /// Renames component class and references in the razor content.
    /// </summary>
    private string RenameComponentInContent(string content, string originalName, string newName)
    {
        // Replace class name references
        // This handles cases like: public class Button, partial class Button, etc.
        content = System.Text.RegularExpressions.Regex.Replace(
            content,
            $@"\b{originalName}\b",
            newName,
            System.Text.RegularExpressions.RegexOptions.None);

        return content;
    }

    /// <summary>
    /// Renames CSS class names to match the renamed component.
    /// </summary>
    private string RenameCssClasses(string cssContent, string originalName, string newName)
    {
        // Replace CSS class names that include the component name
        // For example: .vibe-button -> .vibe-custombutton (kebab-case)
        var originalKebab = ToKebabCase(originalName);
        var newKebab = ToKebabCase(newName);

        cssContent = cssContent.Replace($"vibe-{originalKebab}", $"vibe-{newKebab}");

        return cssContent;
    }

    /// <summary>
    /// Converts PascalCase to kebab-case.
    /// </summary>
    private string ToKebabCase(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return System.Text.RegularExpressions.Regex.Replace(
            value,
            "([a-z])([A-Z])",
            "$1-$2"
        ).ToLowerInvariant();
    }
}
