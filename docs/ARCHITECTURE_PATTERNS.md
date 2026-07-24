# Vibe.UI Architecture Patterns Documentation

This document provides comprehensive documentation for the architectural patterns introduced in Vibe.UI, including CSS architecture, utility classes, the ClassBuilder utility, shared enums, and component composition patterns.

## Table of Contents

1. [CSS Architecture](#css-architecture)
2. [ClassBuilder Utility](#classbuilder-utility)
3. [Shared Enums](#shared-enums)
4. [Dialog Composition Pattern](#dialog-composition-pattern)
5. [Theming](#theming)
6. [Best Practices](#best-practices)

---

## CSS Architecture

Vibe.UI follows a modern, maintainable CSS architecture that combines scoped component styles with a design token system and utility classes.

### Overview

The CSS architecture consists of three main layers:

1. **Design Tokens** (`vibe-base.css`) - CSS variables defining the design system
2. **Utility Classes** (`vibe-utilities.css`) - Reusable utility classes for common patterns
3. **Scoped Component Styles** (`.razor.css` files) - Component-specific styles

### Design Tokens: vibe-base.css

The `vibe-base.css` file is the foundation of the design system. It provides:

- **CSS Variables (Design Tokens)** - All theme colors, spacing, typography, and other design decisions
- **Theme Definitions** - Light and dark theme color palettes
- **CSS Reset** - Minimal reset for consistent cross-browser rendering
- **Animation Keyframes** - Reusable animations for components

**Location:** `src/Vibe.UI/wwwroot/css/vibe-base.css`

#### Key Design Tokens

```css
:root {
    /* Colors */
    --vibe-background: hsl(0 0% 100%);
    --vibe-foreground: hsl(240 10% 3.9%);
    --vibe-primary: hsl(240 5.9% 10%);
    --vibe-primary-foreground: hsl(0 0% 98%);

    /* Spacing Scale (4px base unit) */
    --vibe-spacing-1: 0.25rem;  /* 4px */
    --vibe-spacing-2: 0.5rem;   /* 8px */
    --vibe-spacing-4: 1rem;     /* 16px */
    --vibe-spacing-8: 2rem;     /* 32px */

    /* Border Radius */
    --vibe-radius: 0.5rem;      /* 8px */
    --vibe-radius-sm: 0.3rem;   /* 5px */
    --vibe-radius-lg: 0.75rem;  /* 12px */

    /* Typography */
    --vibe-font: 'Inter', -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
    --vibe-font-mono: 'ui-monospace', 'SFMono-Regular', 'Menlo', 'Monaco', 'Consolas', monospace;

    /* Shadows */
    --vibe-shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.05);
    --vibe-shadow-md: 0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1);
    --vibe-shadow-lg: 0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1);

    /* Transitions */
    --vibe-transition: all 0.2s ease;
    --vibe-transition-fast: all 0.15s ease;
    --vibe-transition-slow: all 0.3s ease;
}
```

#### Dark Theme

Dark theme colors are defined using the `.dark` selector:

```css
.dark {
    --vibe-background: hsl(240 10% 3.9%);
    --vibe-foreground: hsl(0 0% 98%);
    --vibe-primary: hsl(0 0% 98%);
    --vibe-primary-foreground: hsl(240 5.9% 10%);
    /* ... more dark theme colors */
}
```

To enable dark mode, simply add the `dark` class to the `<html>` element:

```javascript
document.documentElement.classList.add('dark');
```

### Utility Classes: vibe-utilities.css

The `vibe-utilities.css` file provides small, single-purpose utility classes for common styling patterns. These follow a functional CSS approach similar to Tailwind CSS but scoped to Vibe.UI.

**Location:** `src/Vibe.UI/wwwroot/css/vibe-utilities.css`

#### Available Utility Categories

**Display:**
```css
.vibe-flex          /* display: flex */
.vibe-grid          /* display: grid */
.vibe-hidden        /* display: none */
.vibe-block         /* display: block */
```

**Flexbox:**
```css
.vibe-flex-row      /* flex-direction: row */
.vibe-flex-col      /* flex-direction: column */
.vibe-items-center  /* align-items: center */
.vibe-justify-between /* justify-content: space-between */
.vibe-gap-4         /* gap: 1rem */
```

**Typography:**
```css
.vibe-text-sm       /* font-size: 0.875rem */
.vibe-text-base     /* font-size: 1rem */
.vibe-text-lg       /* font-size: 1.125rem */
.vibe-font-semibold /* font-weight: 600 */
.vibe-truncate      /* text overflow ellipsis */
```

**Colors:**
```css
.vibe-text-primary     /* color: var(--vibe-primary) */
.vibe-text-muted       /* color: var(--vibe-muted-foreground) */
.vibe-bg-background    /* background: var(--vibe-background) */
```

**Spacing:**
```css
.vibe-gap-1 to .vibe-gap-8  /* Uses spacing scale */
```

**Animations:**
```css
.vibe-animate-spin      /* Rotating animation */
.vibe-animate-pulse     /* Pulsing opacity */
.vibe-animate-fade-in   /* Fade in effect */
```

### Scoped Component Styles

Each component has its own `.razor.css` file containing component-specific styles. These styles are automatically scoped to the component by Blazor's CSS isolation feature.

**Example:** `Button.razor.css`

```css
/* Base button styles */
.vibe-button {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: var(--vibe-spacing-2);
    border-radius: var(--vibe-radius-sm);
    font-family: var(--vibe-font);
    transition: var(--vibe-transition);
}

/* Size variants */
.vibe-button-small {
    height: 2rem;
    padding: 0 0.75rem;
    font-size: 0.875rem;
}

.vibe-button-medium {
    height: 2.5rem;
    padding: 0 1rem;
    font-size: 0.875rem;
}

/* Style variants */
.vibe-button-primary {
    background-color: var(--vibe-primary);
    color: var(--vibe-primary-foreground);
}
```

#### Benefits of Scoped CSS

1. **No Class Name Collisions** - Styles are automatically scoped to the component
2. **Co-location** - Styles live next to the component they style
3. **Design Token Integration** - Uses CSS variables from `vibe-base.css`
4. **Tree Shaking** - Unused component styles are not included in the build

### CSS Naming Conventions

All Vibe.UI CSS classes follow consistent naming conventions:

- **Prefix:** All classes start with `vibe-` to avoid conflicts
- **Component Base:** `vibe-{component}` (e.g., `vibe-button`, `vibe-dialog`)
- **Variants:** `vibe-{component}-{variant}` (e.g., `vibe-button-primary`, `vibe-button-large`)
- **States:** `vibe-{component}-{state}` (e.g., `vibe-button-loading`, `vibe-button-disabled`)
- **Sub-elements:** `vibe-{component}-{element}` (e.g., `vibe-button-icon`, `vibe-dialog-header`)

### Migration Guide: Global CSS to Scoped CSS

If you're migrating from global CSS to scoped CSS:

**Before (Global CSS):**
```css
/* app.css */
.button {
    padding: 1rem;
}

.button-primary {
    background: blue;
}
```

**After (Scoped CSS):**
```css
/* Button.razor.css */
.vibe-button {
    padding: var(--vibe-spacing-4);
}

.vibe-button-primary {
    background-color: var(--vibe-primary);
}
```

**Migration Steps:**

1. Create a `.razor.css` file next to your component
2. Move component-specific styles from global CSS to the scoped file
3. Replace hard-coded values with CSS variables
4. Update class names to follow `vibe-` prefix convention
5. Remove the original global CSS rules

---

## ClassBuilder Utility

The `ClassBuilder` is a fluent API for building CSS class strings with support for conditional classes, enum-based variants, and lazy evaluation.

### Purpose and Benefits

**Why use ClassBuilder?**

- **Type Safety** - Compile-time checking for enum values
- **Readability** - Fluent API makes intent clear
- **Maintainability** - Centralized class logic
- **Immutability** - Thread-safe and predictable
- **Composability** - Easy to build complex class combinations

### API Documentation

#### Basic Usage

```csharp
using Vibe.UI.Base;

protected override string ComponentClass => new ClassBuilder()
    .Add("vibe-button")
    .Build();
```

#### Methods

**Add(string? className)**

Adds a CSS class to the builder.

```csharp
new ClassBuilder()
    .Add("vibe-button")
    .Add("custom-class")
    .Build();
// Result: "vibe-button custom-class"
```

**AddIf(string? className, bool condition)**

Conditionally adds a CSS class based on a boolean condition.

```csharp
new ClassBuilder()
    .Add("vibe-button")
    .AddIf("vibe-button-disabled", IsDisabled)
    .AddIf("vibe-button-loading", IsLoading)
    .Build();
```

**AddWhen(string? className, Func<bool> condition)**

Adds a CSS class based on a lazy-evaluated condition. Useful for complex conditions.

```csharp
new ClassBuilder()
    .Add("vibe-button")
    .AddWhen("vibe-button-icon-only", () => Icon != null && ChildContent == null)
    .Build();
```

**AddVariant<TEnum>(string? prefix, TEnum value)**

Adds a CSS class based on an enum variant. The class is constructed as `{prefix}-{enumValue}` where the enum value is converted to lowercase.

```csharp
new ClassBuilder()
    .Add("vibe-button")
    .AddVariant("vibe-button", ButtonVariant.Primary)   // Adds "vibe-button-primary"
    .AddVariant("vibe-button", ComponentSize.Large)     // Adds "vibe-button-large"
    .Build();
```

**AddClass(string? userClass)**

Adds a user-provided CSS class. Alias for `Add()` that makes intent clear when accepting user classes.

```csharp
[Parameter]
public string? Class { get; set; }

protected override string ComponentClass => new ClassBuilder()
    .Add("vibe-button")
    .AddClass(Class)  // Adds user-provided classes
    .Build();
```

**Build()**

Builds and returns the final CSS class string. Multiple classes are separated by spaces.

```csharp
string classes = new ClassBuilder()
    .Add("vibe-button")
    .AddIf("vibe-button-disabled", true)
    .Build();
// Result: "vibe-button vibe-button-disabled"
```

### Complete Examples

#### Simple Button Component

```csharp
@namespace Vibe.UI.Components
@inherits Vibe.UI.Base.VibeComponent
@using Vibe.UI.Enums

<button class="@CombinedClass" disabled="@Disabled" @onclick="OnClick">
    @ChildContent
</button>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public ButtonVariant Variant { get; set; } = ButtonVariant.Primary;
    [Parameter] public ComponentSize Size { get; set; } = ComponentSize.Medium;
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    protected override string ComponentClass => new ClassBuilder()
        .Add("vibe-button")
        .AddVariant("vibe-button", Variant)
        .AddVariant("vibe-button", Size)
        .AddIf("vibe-button-disabled", Disabled)
        .Build();
}
```

#### Complex Component with Multiple Conditions

```csharp
protected override string ComponentClass => new ClassBuilder()
    .Add("vibe-input")
    .AddVariant("vibe-input", Variant)
    .AddVariant("vibe-input", Size)
    .AddIf("vibe-input-disabled", Disabled)
    .AddIf("vibe-input-error", HasError)
    .AddIf("vibe-input-full-width", FullWidth)
    .AddWhen("vibe-input-with-icon", () => Icon != null || Suffix != null)
    .AddClass(Class)  // User-provided classes
    .Build();
```

#### Usage Patterns

**Pattern 1: Base + Variants**
```csharp
new ClassBuilder()
    .Add("vibe-button")
    .AddVariant("vibe-button", ButtonVariant.Primary)
    .AddVariant("vibe-button", ComponentSize.Large)
    .Build();
```

**Pattern 2: Base + Conditional States**
```csharp
new ClassBuilder()
    .Add("vibe-dialog")
    .AddIf("vibe-dialog-open", IsOpen)
    .AddIf("vibe-dialog-fullscreen", IsFullscreen)
    .Build();
```

**Pattern 3: Base + Variants + States + User Classes**
```csharp
new ClassBuilder()
    .Add("vibe-card")
    .AddVariant("vibe-card", CardVariant.Elevated)
    .AddIf("vibe-card-interactive", IsClickable)
    .AddClass(UserProvidedClass)
    .Build();
```

---

## Shared Enums

Vibe.UI provides a set of shared enums in the `Vibe.UI.Enums` namespace for consistent component configuration across the library.

### Available Enums

#### ComponentSize

Defines size options for components.

```csharp
namespace Vibe.UI.Enums;

public enum ComponentSize
{
    Small,   // Compact appearance
    Medium,  // Default size (balanced)
    Large    // Prominent appearance
}
```

**Usage:**
```csharp
<Button Size="ComponentSize.Large">Large Button</Button>
<Input Size="ComponentSize.Small" />
```

**Components using ComponentSize:**
- Button
- Input
- Select
- Badge
- Avatar
- And more...

#### ButtonVariant

Defines visual style variants for button components.

```csharp
namespace Vibe.UI.Enums;

public enum ButtonVariant
{
    Primary,     // Main call-to-action
    Secondary,   // Secondary actions
    Destructive, // Dangerous actions (delete, remove)
    Outline,     // Transparent with border
    Ghost,       // Minimal styling
    Link         // Text link appearance
}
```

**Usage:**
```csharp
<Button Variant="ButtonVariant.Primary">Save</Button>
<Button Variant="ButtonVariant.Destructive">Delete</Button>
<Button Variant="ButtonVariant.Ghost">Cancel</Button>
```

#### AlertVariant

Defines visual style variants for alert and notification components.

```csharp
namespace Vibe.UI.Enums;

public enum AlertVariant
{
    Default,     // General information
    Destructive, // Errors and critical issues
    Success,     // Successful operations
    Info,        // Informational messages
    Warning      // Warning messages
}
```

**Usage:**
```csharp
<Alert Variant="AlertVariant.Success">Changes saved successfully!</Alert>
<Alert Variant="AlertVariant.Destructive">An error occurred</Alert>
<Alert Variant="AlertVariant.Warning">This action cannot be undone</Alert>
```

#### InputVariant

Defines visual style variants for input components.

```csharp
namespace Vibe.UI.Enums;

public enum InputVariant
{
    Text,     // Minimal with bottom border
    Filled,   // Filled background
    Outlined  // Visible border
}
```

**Usage:**
```csharp
<Input Variant="InputVariant.Outlined" />
<Input Variant="InputVariant.Filled" />
```

#### CardVariant

Defines visual style variants for card components.

```csharp
namespace Vibe.UI.Enums;

public enum CardVariant
{
    Default,     // Standard card
    Elevated,    // With shadow/elevation
    Outlined,    // With border, no shadow
    Interactive  // Clickable with hover effects
}
```

**Usage:**
```csharp
<Card Variant="CardVariant.Elevated">
    <CardContent>Elevated card with shadow</CardContent>
</Card>
```

#### Orientation

Defines orientation options for layout and directional components.

```csharp
namespace Vibe.UI.Enums;

public enum Orientation
{
    Horizontal,  // Left-to-right flow
    Vertical     // Top-to-bottom flow
}
```

**Usage:**
```csharp
<Tabs Orientation="Orientation.Vertical">
    <TabList>
        <Tab>Tab 1</Tab>
        <Tab>Tab 2</Tab>
    </TabList>
</Tabs>
```

#### Position

Defines positioning options for overlay components.

```csharp
namespace Vibe.UI.Enums;

public enum Position
{
    Top,          // Above, centered
    Bottom,       // Below, centered
    Left,         // Left side, centered
    Right,        // Right side, centered
    TopLeft,      // Above, left-aligned
    TopRight,     // Above, right-aligned
    BottomLeft,   // Below, left-aligned
    BottomRight   // Below, right-aligned
}
```

**Usage:**
```csharp
<Tooltip Position="Position.Top">
    <TooltipTrigger>Hover me</TooltipTrigger>
    <TooltipContent>Tooltip appears above</TooltipContent>
</Tooltip>
```

### Migration from Inline Enums

If you have components using inline enums, migrate them to use shared enums:

**Before:**
```csharp
// ButtonComponent.razor
public enum ButtonVariant { Primary, Secondary }
```

**After:**
```csharp
// ButtonComponent.razor
@using Vibe.UI.Enums

[Parameter]
public ButtonVariant Variant { get; set; } = ButtonVariant.Primary;
```

---

## Dialog Composition Pattern

Vibe.UI implements a shadcn/ui-style composition pattern for Dialog components. This pattern allows for flexible, declarative dialog construction using sub-components.

### Overview

The Dialog composition pattern consists of:

- **DialogRoot** - Container component managing state and context
- **DialogTrigger** - Element that opens the dialog
- **DialogContent** - The dialog content container
- **DialogHeader** - Optional header section
- **DialogTitle** - Dialog title
- **DialogDescription** - Dialog description
- **DialogFooter** - Optional footer section
- **DialogClose** - Element that closes the dialog

### DialogContext

The `DialogContext` class provides cascading context for dialog sub-components to communicate state and actions.

```csharp
namespace Vibe.UI.Components;

public class DialogContext
{
    public DialogContext(Func<Task> openAction, Func<Task> closeAction)
    {
        _openAction = openAction;
        _closeAction = closeAction;
    }

    public Task Open() => _openAction();
    public Task Close() => _closeAction();
}
```

The context is created by `DialogRoot` and cascaded to child components using Blazor's `CascadingValue`.

### Usage Examples

#### Basic Dialog (Slot-Based Pattern)

```razor
<Dialog @bind-IsOpen="@isOpen">
    <Header>
        <h2>Dialog Title</h2>
    </Header>
    <ChildContent>
        <p>This is the dialog content.</p>
    </ChildContent>
    <Footer>
        <Button OnClick="@(() => isOpen = false)">Close</Button>
    </Footer>
</Dialog>

@code {
    private bool isOpen = false;
}
```

#### Composition Pattern (shadcn-style)

```razor
<DialogRoot @bind-IsOpen="@isOpen">
    <DialogTrigger>
        <Button>Open Dialog</Button>
    </DialogTrigger>

    <DialogContent>
        <DialogHeader>
            <DialogTitle>Edit Profile</DialogTitle>
            <DialogDescription>
                Make changes to your profile here. Click save when you're done.
            </DialogDescription>
        </DialogHeader>

        <div class="dialog-body">
            <Input Label="Name" @bind-Value="@name" />
            <Input Label="Email" @bind-Value="@email" />
        </div>

        <DialogFooter>
            <DialogClose>
                <Button Variant="ButtonVariant.Secondary">Cancel</Button>
            </DialogClose>
            <Button OnClick="@SaveProfile">Save Changes</Button>
        </DialogFooter>
    </DialogContent>
</DialogRoot>

@code {
    private bool isOpen = false;
    private string name = "";
    private string email = "";

    private void SaveProfile()
    {
        // Save logic here
        isOpen = false;
    }
}
```

#### Confirmation Dialog

```razor
<DialogRoot @bind-IsOpen="@showConfirm">
    <DialogTrigger>
        <Button Variant="ButtonVariant.Destructive">Delete Account</Button>
    </DialogTrigger>

    <DialogContent>
        <DialogHeader>
            <DialogTitle>Are you sure?</DialogTitle>
            <DialogDescription>
                This action cannot be undone. This will permanently delete your account
                and remove your data from our servers.
            </DialogDescription>
        </DialogHeader>

        <DialogFooter>
            <DialogClose>
                <Button Variant="ButtonVariant.Ghost">Cancel</Button>
            </DialogClose>
            <DialogClose>
                <Button Variant="ButtonVariant.Destructive" OnClick="@DeleteAccount">
                    Delete Account
                </Button>
            </DialogClose>
        </DialogFooter>
    </DialogContent>
</DialogRoot>
```

#### Form Dialog

```razor
<DialogRoot @bind-IsOpen="@showForm" CloseOnOutsideClick="false">
    <DialogTrigger>
        <Button>Add User</Button>
    </DialogTrigger>

    <DialogContent>
        <DialogHeader>
            <DialogTitle>Add New User</DialogTitle>
            <DialogDescription>
                Enter the user's information below.
            </DialogDescription>
        </DialogHeader>

        <div class="vibe-flex vibe-flex-col vibe-gap-4">
            <Input Label="Full Name" @bind-Value="@newUser.Name" Required />
            <Input Label="Email" Type="email" @bind-Value="@newUser.Email" Required />
            <Select Label="Role" @bind-Value="@newUser.Role">
                <option value="user">User</option>
                <option value="admin">Admin</option>
            </Select>
        </div>

        <DialogFooter>
            <DialogClose>
                <Button Variant="ButtonVariant.Secondary">Cancel</Button>
            </DialogClose>
            <Button OnClick="@AddUser" Disabled="@(!IsFormValid)">
                Add User
            </Button>
        </DialogFooter>
    </DialogContent>
</DialogRoot>
```

### Component Properties

**DialogRoot:**
- `IsOpen` (bool) - Controls dialog visibility
- `CloseOnOutsideClick` (bool) - Close when clicking overlay (default: true)
- `CloseOnEscape` (bool) - Close when pressing Escape (default: true)

**DialogTrigger:**
- `ChildContent` (RenderFragment) - Content that triggers the dialog
- `OnClick` (EventCallback) - Optional click callback

**DialogClose:**
- `ChildContent` (RenderFragment) - Content that closes the dialog
- `OnClick` (EventCallback) - Optional click callback

### Benefits of Composition Pattern

1. **Flexibility** - Compose dialogs with only the parts you need
2. **Semantic** - Clear intent with named sub-components
3. **Reusability** - Sub-components can be reused in different contexts
4. **Type Safety** - Full IntelliSense support for all parts
5. **Accessibility** - Proper ARIA attributes automatically applied

---

## Theming

Vibe.UI uses a pure CSS theming system following shadcn/ui patterns. See [THEMING.md](./THEMING.md) for complete theming documentation.

### Quick Reference

**Enable Dark Mode:**
```javascript
document.documentElement.classList.add('dark');
```

**Toggle Dark Mode:**
```javascript
document.documentElement.classList.toggle('dark');
```

**Using ThemeToggle Component:**
```razor
<ThemeToggle />
```

**Customize Colors:**
```css
:root {
    --vibe-primary: hsl(220 70% 50%);
    --vibe-primary-foreground: hsl(0 0% 100%);
}

.dark {
    --vibe-primary: hsl(220 70% 60%);
    --vibe-primary-foreground: hsl(0 0% 100%);
}
```

---

## Best Practices

### Component Development Guidelines

1. **Inherit from VibeComponent**
   ```csharp
   @inherits Vibe.UI.Base.VibeComponent
   ```

2. **Use ClassBuilder for CSS Classes**
   ```csharp
   protected override string ComponentClass => new ClassBuilder()
       .Add("vibe-component")
       .AddVariant("vibe-component", Variant)
       .AddClass(Class)
       .Build();
   ```

3. **Use Shared Enums**
   ```csharp
   @using Vibe.UI.Enums

   [Parameter]
   public ComponentSize Size { get; set; } = ComponentSize.Medium;
   ```

4. **Create Scoped CSS File**
   ```
   MyComponent.razor
   MyComponent.razor.css
   ```

5. **Use CSS Variables**
   ```css
   .vibe-mycomponent {
       background-color: var(--vibe-background);
       color: var(--vibe-foreground);
       padding: var(--vibe-spacing-4);
       border-radius: var(--vibe-radius);
   }
   ```

### CSS Naming Conventions

1. **Prefix all classes with `vibe-`**
   ```css
   .vibe-button { }
   .vibe-dialog { }
   ```

2. **Use kebab-case for class names**
   ```css
   .vibe-button-primary { }
   .vibe-dialog-content { }
   ```

3. **Follow BEM-like pattern for sub-elements**
   ```css
   .vibe-button { }
   .vibe-button-icon { }
   .vibe-button-content { }
   ```

4. **Use descriptive state classes**
   ```css
   .vibe-button-loading { }
   .vibe-button-disabled { }
   .vibe-dialog-open { }
   ```

### Accessibility Considerations

The canonical cross-component requirements and validation guidance live in
[Accessibility](ACCESSIBILITY.md). The examples below summarize the component
authoring pattern.

1. **Use Semantic HTML**
   ```razor
   <button type="button">...</button>
   <dialog role="dialog" aria-modal="true">...</dialog>
   ```

2. **Provide ARIA Attributes**
   ```razor
   <div role="dialog" aria-labelledby="dialog-title" aria-describedby="dialog-desc">
       <h2 id="dialog-title">Title</h2>
       <p id="dialog-desc">Description</p>
   </div>
   ```

3. **Support Keyboard Navigation**
   ```razor
   @onkeydown="HandleKeyDown"

   @code {
       private void HandleKeyDown(KeyboardEventArgs e)
       {
           if (e.Key == "Escape") CloseDialog();
       }
   }
   ```

4. **Manage Focus**
   ```csharp
   protected override async Task OnAfterRenderAsync(bool firstRender)
   {
       if (IsOpen)
       {
           await FocusFirstElement();
       }
   }
   ```

5. **Provide Screen Reader Text**
   ```razor
   <span class="vibe-sr-only">Close dialog</span>
   ```

### Performance Best Practices

1. **Use CSS Variables** - Enables efficient theme switching
2. **Minimize JavaScript** - Prefer CSS-only solutions
3. **Lazy Load Large Components** - Use Blazor's lazy loading
4. **Optimize Animations** - Use `transform` and `opacity` for better performance
5. **Avoid Layout Thrashing** - Batch DOM reads and writes

### Testing Considerations

1. **Test with Different Themes**
   ```csharp
   [Fact]
   public void Button_RendersCorrectly_InDarkMode()
   {
       JSInterop.SetupVoid("eval", _ => true);
       var cut = RenderComponent<Button>();
       // Test dark mode styles
   }
   ```

2. **Test Accessibility**
   ```csharp
   [Fact]
   public void Dialog_HasCorrectAriaAttributes()
   {
       var cut = RenderComponent<Dialog>(parameters => parameters
           .Add(p => p.IsOpen, true));

       var dialog = cut.Find("[role='dialog']");
       dialog.GetAttribute("aria-modal").Should().Be("true");
   }
   ```

3. **Test Keyboard Interactions**
   ```csharp
   [Fact]
   public void Dialog_ClosesOnEscape()
   {
       var cut = RenderComponent<Dialog>(parameters => parameters
           .Add(p => p.IsOpen, true));

       cut.Find(".vibe-dialog").KeyDown(new KeyboardEventArgs { Key = "Escape" });

       cut.Instance.IsOpen.Should().BeFalse();
   }
   ```

---

## Summary

The Vibe.UI architecture provides:

- **Maintainable CSS** with scoped styles and design tokens
- **Type-Safe ClassBuilder** for dynamic class generation
- **Consistent Enums** for component configuration
- **Flexible Composition** patterns for complex components
- **Pure CSS Theming** with no JavaScript dependencies
- **Accessibility First** approach to component design

By following these patterns and best practices, you can create consistent, maintainable, and accessible components that integrate seamlessly with the Vibe.UI design system.
