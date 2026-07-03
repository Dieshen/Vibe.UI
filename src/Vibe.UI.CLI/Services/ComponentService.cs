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
        var installedPath = GetProjectRelativePath(projectPath, componentsDir, "componentsDir");

        if (!Directory.Exists(installedPath))
            return new List<string>();

        var razorFiles = Directory.GetFiles(installedPath, "*.razor", SearchOption.AllDirectories);
        return razorFiles
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => name != null)
            .Select(name => name!)
            .ToList();
    }

    /// <summary>
    /// Installs a component into the target project.
    /// </summary>
    /// <param name="projectPath">The root path of the target project.</param>
    /// <param name="componentsDir">The base components directory (e.g., "Components").</param>
    /// <param name="componentName">The name of the component to install.</param>
    /// <param name="overwrite">Whether to overwrite existing component files.</param>
    /// <param name="customName">Optional custom name for the component (renames the component).</param>
    /// <param name="customOutputDir">
    /// Optional custom output directory. If not specified, uses a FLAT STRUCTURE (all components
    /// directly in componentsDir) following the shadcn/ui pattern. This makes imports simpler and
    /// components easier to discover. Specify this parameter to use custom organization like category
    /// subdirectories (e.g., "Components/Input").
    /// </param>
    /// <remarks>
    /// <para><strong>Directory Structure Design:</strong></para>
    /// <para>
    /// By default, Vibe.UI uses a FLAT directory structure where all components are installed directly
    /// into the components directory without category subdirectories. This design choice:
    /// </para>
    /// <list type="bullet">
    ///   <item>Matches the shadcn/ui pattern that inspired this library</item>
    ///   <item>Simplifies import paths (@using MyApp.Components.Button vs @using MyApp.Components.Input.Button)</item>
    ///   <item>Makes components easier to discover (no need to remember categories)</item>
    ///   <item>Reduces cognitive load (no "which category?" decisions)</item>
    ///   <item>Scales well with modern IDE tooling</item>
    /// </list>
    /// <para><strong>Examples:</strong></para>
    /// <code>
    /// // Default flat structure (recommended):
    /// await InstallComponentAsync(projectPath, "Components", "button", false);
    /// // Result: Components/Button.razor
    ///
    /// // Custom subdirectory structure:
    /// await InstallComponentAsync(projectPath, "Components", "button", false, null, "Components/Input");
    /// // Result: Components/Input/Button.razor
    /// </code>
    /// <para>
    /// Note: The source code in Vibe.UI.CLI is organized by category for maintainer convenience,
    /// but components are installed flat by default for user convenience.
    /// </para>
    /// </remarks>
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

        // Determine target directory. By default, we use a flat structure (all components directly
        // in componentsDir) to match the shadcn/ui pattern. This makes imports simpler and components
        // easier to discover. Users can override this with customOutputDir to maintain their own
        // organizational structure (e.g., category subdirectories).
        var targetDir = string.IsNullOrEmpty(customOutputDir)
            ? GetProjectRelativePath(projectPath, componentsDir, "componentsDir")
            : GetProjectRelativePath(projectPath, customOutputDir, "customOutputDir");

        Directory.CreateDirectory(targetDir);

        // Determine component file name - use custom name or original name
        var outputComponentName = customName ?? component.Name;

        // Get source component path
        var sourcePath = GetComponentSourcePath(component.Name, component.Category);

        // Copy .razor file with safety checks
        var razorFile = Path.Combine(targetDir, $"{outputComponentName}.razor");
        var fileChangeDetector = new FileChangeDetector(projectPath);

        if (!File.Exists(razorFile) || overwrite)
        {
            //Check if file has been modified before overwriting
            if (File.Exists(razorFile) && overwrite)
            {
                var relativePath = Path.GetRelativePath(projectPath, razorFile);
                var storedHash = await fileChangeDetector.GetStoredHashAsync(relativePath);

                if (storedHash != null)
                {
                    var currentHash = FileChangeDetector.ComputeFileHash(razorFile);
                    if (currentHash != storedHash)
                    {
                        // File has been modified - create backup
                        var backupPath = await fileChangeDetector.CreateBackupAsync(razorFile);
                        Console.WriteLine($"⚠️  File was modified. Backup created: {Path.GetRelativePath(projectPath, backupPath)}");
                    }
                }
            }

            var razorContent = await GetComponentContentAsync(sourcePath, component.Name);

            // Rename component if custom name provided
            if (!string.IsNullOrEmpty(customName))
            {
                razorContent = RenameComponentInContent(razorContent, component.Name, customName);
            }

            // Update namespace if custom output directory provided
            if (!string.IsNullOrEmpty(customOutputDir))
            {
                razorContent = UpdateNamespace(razorContent, customOutputDir, projectPath);
            }

            await File.WriteAllTextAsync(razorFile, razorContent);

            // Save new checksum
            var newRelativePath = Path.GetRelativePath(projectPath, razorFile);
            var newHash = FileChangeDetector.ComputeFileHash(razorFile);
            await fileChangeDetector.SaveFileHashAsync(newRelativePath, newHash);
        }

        // Copy .razor.css file if it exists with safety checks
        var sourceCssPath = $"{sourcePath}.css";
        if (File.Exists(sourceCssPath))
        {
            var cssFile = Path.Combine(targetDir, $"{outputComponentName}.razor.css");
            if (!File.Exists(cssFile) || overwrite)
            {
                // Check if CSS file has been modified before overwriting
                if (File.Exists(cssFile) && overwrite)
                {
                    var relativePath = Path.GetRelativePath(projectPath, cssFile);
                    var storedHash = await fileChangeDetector.GetStoredHashAsync(relativePath);

                    if (storedHash != null)
                    {
                        var currentHash = FileChangeDetector.ComputeFileHash(cssFile);
                        if (currentHash != storedHash)
                        {
                            // File has been modified - create backup
                            var backupPath = await fileChangeDetector.CreateBackupAsync(cssFile);
                            Console.WriteLine($"⚠️  File was modified. Backup created: {Path.GetRelativePath(projectPath, backupPath)}");
                        }
                    }
                }

                var cssContent = await File.ReadAllTextAsync(sourceCssPath);

                // Update CSS class names if renamed
                if (!string.IsNullOrEmpty(customName))
                {
                    cssContent = RenameCssClasses(cssContent, component.Name, customName);
                }

                await File.WriteAllTextAsync(cssFile, cssContent);

                // Save new checksum
                var newRelativePath = Path.GetRelativePath(projectPath, cssFile);
                var newHash = FileChangeDetector.ComputeFileHash(cssFile);
                await fileChangeDetector.SaveFileHashAsync(newRelativePath, newHash);
            }
        }
    }

    private Dictionary<string, ComponentInfo> InitializeComponents()
    {
        var components = new Dictionary<string, ComponentInfo>
        {
            // Inputs Components (23 components)
            ["button"] = new ComponentInfo { Name = "Button", Category = "Inputs", Description = "Displays a button or a component that looks like a button" },
            ["checkbox"] = new ComponentInfo { Name = "Checkbox", Category = "Inputs", Description = "A control that allows the user to toggle between checked and not checked" },
            ["input"] = new ComponentInfo { Name = "Input", Category = "Inputs", Description = "Displays a form input field or a component that looks like an input field" },
            ["radio"] = new ComponentInfo { Name = "Radio", Category = "Inputs", Description = "A radio button control" },
            ["radiogroup"] = new ComponentInfo { Name = "RadioGroup", Category = "Inputs", Description = "A set of checkable buttons where no more than one can be checked at a time", Dependencies = new List<string> { "RadioGroupItem" } },
            ["radiogroupitem"] = new ComponentInfo { Name = "RadioGroupItem", Category = "Inputs", Description = "An item within a radio group" },
            ["select"] = new ComponentInfo { Name = "Select", Category = "Inputs", Description = "Displays a list of options for the user to pick from" },
            ["slider"] = new ComponentInfo { Name = "Slider", Category = "Inputs", Description = "An input where the user selects a value from within a given range" },
            ["switch"] = new ComponentInfo { Name = "Switch", Category = "Inputs", Description = "A control that allows the user to toggle between checked and not checked" },
            ["textarea"] = new ComponentInfo { Name = "TextArea", Category = "Inputs", Description = "Displays a form textarea or a component that looks like a textarea" },
            ["toggle"] = new ComponentInfo { Name = "Toggle", Category = "Inputs", Description = "A two-state button that can be either on or off" },
            ["togglegroup"] = new ComponentInfo { Name = "ToggleGroup", Category = "Inputs", Description = "A set of two-state buttons that can be toggled on or off", Dependencies = new List<string> { "ToggleGroupItem" } },
            ["togglegroupitem"] = new ComponentInfo { Name = "ToggleGroupItem", Category = "Inputs", Description = "An item within a toggle group", HasCss = false },
            ["colorpicker"] = new ComponentInfo { Name = "ColorPicker", Category = "Inputs", Description = "A color picker component for selecting colors", HasCss = false },
            ["multiselect"] = new ComponentInfo { Name = "MultiSelect", Category = "Inputs", Description = "Select multiple items from a dropdown", HasCss = false },
            ["inputotp"] = new ComponentInfo { Name = "InputOTP", Category = "Inputs", Description = "One-time password input with auto-focus and keyboard navigation", HasCss = false },
            ["fileupload"] = new ComponentInfo { Name = "FileUpload", Category = "Inputs", Description = "Drag-and-drop file upload with multiple file support" },
            ["rating"] = new ComponentInfo { Name = "Rating", Category = "Inputs", Description = "Star rating component with half-star support" },
            ["taginput"] = new ComponentInfo { Name = "TagInput", Category = "Inputs", Description = "Multi-tag input field with suggestions" },
            ["richtexteditor"] = new ComponentInfo { Name = "RichTextEditor", Category = "Inputs", Description = "WYSIWYG rich text editor with formatting toolbar", HasCss = false },
            ["mentions"] = new ComponentInfo { Name = "Mentions", Category = "Inputs", Description = "@mention and #hashtag input with autocomplete", HasCss = false },
            ["transferlist"] = new ComponentInfo { Name = "TransferList", Category = "Inputs", Description = "Dual-list selector for moving items between lists", HasCss = false },
            ["imagecropper"] = new ComponentInfo { Name = "ImageCropper", Category = "Inputs", Description = "Image cropping tool with zoom and rotation", HasCss = false },

            // Form Components (7 components)
            ["form"] = new ComponentInfo { Name = "Form", Category = "Form", Description = "Building forms with validation", HasCss = false },
            ["formfield"] = new ComponentInfo { Name = "FormField", Category = "Form", Description = "A form field wrapper", HasCss = false },
            ["formlabel"] = new ComponentInfo { Name = "FormLabel", Category = "Form", Description = "Renders an accessible label associated with controls", HasCss = false },
            ["formmessage"] = new ComponentInfo { Name = "FormMessage", Category = "Form", Description = "Displays form validation messages", HasCss = false },
            ["label"] = new ComponentInfo { Name = "Label", Category = "Form", Description = "Renders an accessible label associated with controls", HasCss = false },
            ["combobox"] = new ComponentInfo { Name = "Combobox", Category = "Form", Description = "Autocomplete input and command palette with a list of suggestions" },
            ["validatedinput"] = new ComponentInfo { Name = "ValidatedInput", Category = "Form", Description = "Input field with integrated validation display", HasCss = false },

            // Data Display (7 components)
            ["avatar"] = new ComponentInfo { Name = "Avatar", Category = "DataDisplay", Description = "An image element with a fallback for representing the user" },
            ["badge"] = new ComponentInfo { Name = "Badge", Category = "DataDisplay", Description = "Displays a badge or a component that looks like a badge" },
            ["datatable"] = new ComponentInfo { Name = "DataTable", Category = "DataDisplay", Description = "Powerful table with sorting, filtering, and pagination" },
            ["progress"] = new ComponentInfo { Name = "Progress", Category = "DataDisplay", Description = "Displays an indicator showing the completion progress of a task" },
            ["table"] = new ComponentInfo { Name = "Table", Category = "DataDisplay", Description = "A responsive table component", HasCss = false },
            ["chart"] = new ComponentInfo { Name = "Chart", Category = "DataDisplay", Description = "Data visualization charts", HasCss = false, HasJavaScript = true },
            ["timeline"] = new ComponentInfo { Name = "Timeline", Category = "DataDisplay", Description = "Event timeline with status indicators and timestamps" },
            ["tag"] = new ComponentInfo { Name = "Tag", Category = "DataDisplay", Description = "Compact label/pill component with variants and optional remove action" },

            // Layout (12 components)
            ["aspectratio"] = new ComponentInfo { Name = "AspectRatio", Category = "Layout", Description = "Displays content within a desired aspect ratio" },
            ["card"] = new ComponentInfo { Name = "Card", Category = "Layout", Description = "Displays a card with header, content, and footer" },
            ["container"] = new ComponentInfo { Name = "Container", Category = "Layout", Description = "Responsive container with max-width constraints" },
            ["divider"] = new ComponentInfo { Name = "Divider", Category = "Layout", Description = "A horizontal or vertical divider line" },
            ["grid"] = new ComponentInfo { Name = "Grid", Category = "Layout", Description = "CSS grid layout container", Dependencies = new List<string> { "GridItem" } },
            ["griditem"] = new ComponentInfo { Name = "GridItem", Category = "Layout", Description = "An item within a grid layout", HasCss = false },
            ["separator"] = new ComponentInfo { Name = "Separator", Category = "Layout", Description = "Visually or semantically separates content" },
            ["resizable"] = new ComponentInfo { Name = "Resizable", Category = "Layout", Description = "Accessible resizable panel groups and layouts", HasCss = false },
            ["sheet"] = new ComponentInfo { Name = "Sheet", Category = "Layout", Description = "Extends the Dialog component to display content that complements the main content" },
            ["masonrygrid"] = new ComponentInfo { Name = "MasonryGrid", Category = "Layout", Description = "Pinterest-style masonry grid layout for variable-height items", HasCss = false },
            ["splitter"] = new ComponentInfo { Name = "Splitter", Category = "Layout", Description = "Resizable split pane divider with drag support", HasCss = false },
            ["stack"] = new ComponentInfo { Name = "Stack", Category = "Layout", Description = "Vertical or horizontal stack layout with gap spacing" },

            // Navigation (14 components)
            ["breadcrumb"] = new ComponentInfo { Name = "Breadcrumb", Category = "Navigation", Description = "Displays the path to the current resource using a hierarchy of links", Dependencies = new List<string> { "BreadcrumbItem" } },
            ["breadcrumbitem"] = new ComponentInfo { Name = "BreadcrumbItem", Category = "Navigation", Description = "An item within a breadcrumb" },
            ["link"] = new ComponentInfo { Name = "Link", Category = "Navigation", Description = "A styled anchor link component", HasCss = false },
            ["menu"] = new ComponentInfo { Name = "Menu", Category = "Navigation", Description = "A dropdown menu component", Dependencies = new List<string> { "MenuItem", "MenuSeparator" } },
            ["menuitem"] = new ComponentInfo { Name = "MenuItem", Category = "Navigation", Description = "An item within a menu" },
            ["menuseparator"] = new ComponentInfo { Name = "MenuSeparator", Category = "Navigation", Description = "A separator within a menu" },
            ["menubar"] = new ComponentInfo { Name = "Menubar", Category = "Navigation", Description = "A visually persistent menu common in desktop applications" },
            ["sidebar"] = new ComponentInfo { Name = "Sidebar", Category = "Navigation", Description = "Collapsible sidebar with resize support", HasCss = false },
            ["navigationmenu"] = new ComponentInfo { Name = "NavigationMenu", Category = "Navigation", Description = "A collection of links for navigating websites", Dependencies = new List<string> { "NavigationMenuItem" }, HasCss = false },
            ["navigationmenuitem"] = new ComponentInfo { Name = "NavigationMenuItem", Category = "Navigation", Description = "An item within a navigation menu", HasCss = false },
            ["pagination"] = new ComponentInfo { Name = "Pagination", Category = "Navigation", Description = "Pagination with page navigation, next and previous links" },
            ["stepper"] = new ComponentInfo { Name = "Stepper", Category = "Navigation", Description = "Step-by-step wizard navigation component" },
            ["tabs"] = new ComponentInfo { Name = "Tabs", Category = "Navigation", Description = "A set of layered sections of content", Dependencies = new List<string> { "TabItem" } },
            ["tabitem"] = new ComponentInfo { Name = "TabItem", Category = "Navigation", Description = "An item within tabs", HasCss = false },

            // Overlay (17 components)
            ["alertdialog"] = new ComponentInfo { Name = "AlertDialog", Category = "Overlay", Description = "A modal dialog that interrupts the user with important content" },
            ["contextmenu"] = new ComponentInfo { Name = "ContextMenu", Category = "Overlay", Description = "Displays a menu to the user triggered by right-click", Dependencies = new List<string> { "ContextMenuItem" } },
            ["contextmenuitem"] = new ComponentInfo { Name = "ContextMenuItem", Category = "Overlay", Description = "An item within a context menu" },
            ["dialog"] = new ComponentInfo { Name = "Dialog", Category = "Overlay", Description = "A window overlaid on either the primary window or another dialog" },
            ["dialogcontainer"] = new ComponentInfo { Name = "DialogContainer", Category = "Overlay", Description = "Container for managing multiple dialog instances", HasCss = false },
            ["dialogtrigger"] = new ComponentInfo { Name = "DialogTrigger", Category = "Overlay", Description = "Trigger element that opens a dialog", HasCss = false },
            ["dialogcontent"] = new ComponentInfo { Name = "DialogContent", Category = "Overlay", Description = "Content container for dialog body", HasCss = false },
            ["dialogheader"] = new ComponentInfo { Name = "DialogHeader", Category = "Overlay", Description = "Header section of a dialog", HasCss = false },
            ["dialogtitle"] = new ComponentInfo { Name = "DialogTitle", Category = "Overlay", Description = "Title element for a dialog", HasCss = false },
            ["dialogdescription"] = new ComponentInfo { Name = "DialogDescription", Category = "Overlay", Description = "Description text for a dialog", HasCss = false },
            ["dialogfooter"] = new ComponentInfo { Name = "DialogFooter", Category = "Overlay", Description = "Footer section of a dialog", HasCss = false },
            ["dialogclose"] = new ComponentInfo { Name = "DialogClose", Category = "Overlay", Description = "Close button for a dialog", HasCss = false },
            ["dialogroot"] = new ComponentInfo { Name = "DialogRoot", Category = "Overlay", Description = "Root container for dialog component", HasCss = false },
            ["drawer"] = new ComponentInfo { Name = "Drawer", Category = "Overlay", Description = "A panel that slides out from the edge of the screen" },
            ["hovercard"] = new ComponentInfo { Name = "HoverCard", Category = "Overlay", Description = "For sighted users to preview content available behind a link" },
            ["popover"] = new ComponentInfo { Name = "Popover", Category = "Overlay", Description = "Displays rich content in a portal, triggered by a button" },
            ["tooltip"] = new ComponentInfo { Name = "Tooltip", Category = "Overlay", Description = "A popup that displays information related to an element" },

            // Feedback (9 components)
            ["alert"] = new ComponentInfo { Name = "Alert", Category = "Feedback", Description = "Displays a callout for user attention" },
            ["skeleton"] = new ComponentInfo { Name = "Skeleton", Category = "Feedback", Description = "Use to show a placeholder while content is loading" },
            ["toast"] = new ComponentInfo { Name = "Toast", Category = "Feedback", Description = "A succinct message that is displayed temporarily", Dependencies = new List<string> { "ToastContainer" } },
            ["toastcontainer"] = new ComponentInfo { Name = "ToastContainer", Category = "Feedback", Description = "Container for toast notifications" },
            ["sonner"] = new ComponentInfo { Name = "Sonner", Category = "Feedback", Description = "Enhanced toast notifications with stacking and promise support" },
            ["emptystate"] = new ComponentInfo { Name = "EmptyState", Category = "Feedback", Description = "Placeholder for empty content areas", HasCss = false },
            ["spinner"] = new ComponentInfo { Name = "Spinner", Category = "Feedback", Description = "Loading indicator with customizable sizes", HasCss = false },
            ["notificationcenter"] = new ComponentInfo { Name = "NotificationCenter", Category = "Feedback", Description = "Centralized notification hub with badge and dropdown panel", HasCss = false },
            ["confetti"] = new ComponentInfo { Name = "Confetti", Category = "Feedback", Description = "Celebratory confetti animation with customizable particles", HasCss = false },

            // Date & Time (3 components)
            ["calendar"] = new ComponentInfo { Name = "Calendar", Category = "DateTime", Description = "A date field component that allows users to enter and edit date" },
            ["datepicker"] = new ComponentInfo { Name = "DatePicker", Category = "DateTime", Description = "A date picker component with calendar dropdown", HasCss = false },
            ["daterangepicker"] = new ComponentInfo { Name = "DateRangePicker", Category = "DateTime", Description = "A date range picker component", HasCss = false },

            // Utility (6 components)
            ["command"] = new ComponentInfo { Name = "Command", Category = "Utility", Description = "Fast, composable, unstyled command menu" },
            ["scrollarea"] = new ComponentInfo { Name = "ScrollArea", Category = "Utility", Description = "Augments native scroll functionality for custom, cross-browser styling" },
            ["dropdownmenu"] = new ComponentInfo { Name = "DropdownMenu", Category = "Utility", Description = "Displays a menu to the user triggered by a button" },
            ["kbd"] = new ComponentInfo { Name = "Kbd", Category = "Utility", Description = "Keyboard shortcut display component" },
            ["qrcode"] = new ComponentInfo { Name = "QRCode", Category = "Utility", Description = "Experimental QR-style visual preview for URLs and text", HasCss = false },
            ["icon"] = new ComponentInfo { Name = "Icon", Category = "Utility", Description = "Lucide icon component with size and color customization" },

            // Advanced Components (4 components)
            ["treeview"] = new ComponentInfo { Name = "TreeView", Category = "Advanced", Description = "Hierarchical data display with expand/collapse", Dependencies = new List<string> { "TreeViewNode" }, HasCss = false },
            ["treeviewnode"] = new ComponentInfo { Name = "TreeViewNode", Category = "Advanced", Description = "Individual node component within TreeView hierarchy", HasCss = false },
            ["kanbanboard"] = new ComponentInfo { Name = "KanbanBoard", Category = "Advanced", Description = "Kanban board with draggable cards and columns", HasCss = false },
            ["virtualscroll"] = new ComponentInfo { Name = "VirtualScroll", Category = "Advanced", Description = "Efficient rendering for large lists with virtual scrolling", HasCss = false },

            // Disclosure (5 components)
            ["accordion"] = new ComponentInfo { Name = "Accordion", Category = "Disclosure", Description = "A vertically stacked set of interactive headings", Dependencies = new List<string> { "AccordionItem" } },
            ["accordionitem"] = new ComponentInfo { Name = "AccordionItem", Category = "Disclosure", Description = "An item within an accordion" },
            ["carousel"] = new ComponentInfo { Name = "Carousel", Category = "Disclosure", Description = "A carousel with motion and swipe support", Dependencies = new List<string> { "CarouselItem" } },
            ["carouselitem"] = new ComponentInfo { Name = "CarouselItem", Category = "Disclosure", Description = "An item within a carousel" },
            ["collapsible"] = new ComponentInfo { Name = "Collapsible", Category = "Disclosure", Description = "An interactive component which expands/collapses a panel", HasCss = false },

            // Theme (2 components)
            ["themeprovider"] = new ComponentInfo { Name = "ThemeProvider", Category = "Theme", Description = "Theme context provider for dark/light mode", HasCss = false },
            ["themetoggle"] = new ComponentInfo { Name = "ThemeToggle", Category = "Theme", Description = "Dark/light mode toggle button", HasCss = false },
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

            // 3. Dotnet global tool: Templates folder in package root (../../.. from tools/net10.0/any)
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
        // Avoid broad replacements (can unintentionally rewrite labels/text).
        // Only rename explicit class declarations if present.
        // Note: Most Razor components do not declare a class here; the component name is the file name.
        content = System.Text.RegularExpressions.Regex.Replace(
            content,
            $@"(\bpartial\s+class\s+){System.Text.RegularExpressions.Regex.Escape(originalName)}\b",
            $"$1{newName}",
            System.Text.RegularExpressions.RegexOptions.None);

        content = System.Text.RegularExpressions.Regex.Replace(
            content,
            $@"(\bclass\s+){System.Text.RegularExpressions.Regex.Escape(originalName)}\b",
            $"$1{newName}",
            System.Text.RegularExpressions.RegexOptions.None);

        return content;
    }

    /// <summary>
    /// Updates the @namespace directive to match the custom output directory structure.
    /// </summary>
    private string UpdateNamespace(string content, string customOutputDir, string projectPath)
    {
        // If no custom output directory, don't modify namespace
        if (string.IsNullOrEmpty(customOutputDir))
            return content;

        // Extract the relative path and convert to namespace format
        // Example: "Components/Forms" -> "Components.Forms"
        var namespaceSegment = customOutputDir.Replace('/', '.').Replace('\\', '.');

        // Find existing @namespace directive
        var namespacePattern = @"@namespace\s+([^\s]+)";
        var match = System.Text.RegularExpressions.Regex.Match(content, namespacePattern);

        if (match.Success)
        {
            var originalNamespace = match.Groups[1].Value;

            // Replace only the last segment of the namespace with custom path
            // Example: "Vibe.UI.Components" + "Forms" -> "Vibe.UI.Components.Forms"
            var baseNamespace = originalNamespace.Contains('.')
                ? string.Join(".", originalNamespace.Split('.').Take(originalNamespace.Split('.').Length - 1))
                : originalNamespace;

            var newNamespace = $"{baseNamespace}.{namespaceSegment}";

            content = System.Text.RegularExpressions.Regex.Replace(
                content,
                namespacePattern,
                $"@namespace {newNamespace}");
        }

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

    private static string GetProjectRelativePath(string projectPath, string relativePath, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            throw new ArgumentException("Project path cannot be empty.", nameof(projectPath));
        }

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentException($"{parameterName} cannot be empty.", parameterName);
        }

        var normalizedRelativePath = relativePath
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);

        if (Path.IsPathRooted(normalizedRelativePath))
        {
            throw new InvalidOperationException($"{parameterName} must be a relative path.");
        }

        var projectFullPath = Path.GetFullPath(projectPath);
        var candidateFullPath = Path.GetFullPath(Path.Combine(projectFullPath, normalizedRelativePath));

        var projectPrefix = projectFullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        if (!candidateFullPath.StartsWith(projectPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"{parameterName} must be within the project directory.");
        }

        return candidateFullPath;
    }
}
