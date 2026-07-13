# Vibe.UI Architecture Documentation

## Overview

This document describes key architectural decisions in the Vibe.UI project, including rationale, trade-offs, and implementation details.

---

## Architectural Decision: Flat Directory Structure for Component Installation

### Status
**Accepted** - Implemented as the default installation pattern

### Context

When designing the CLI tool for Vibe.UI, we needed to decide how components should be organized when installed into user projects. Two primary approaches were considered:

1. **Category-based subdirectories**: Mirror the source code structure
   ```
   Components/
   ├── Input/
   │   ├── Button.razor
   │   └── Checkbox.razor
   ├── Form/
   │   ├── Label.razor
   │   └── ValidatedInput.razor
   └── Overlay/
       ├── Dialog.razor
       └── Popover.razor
   ```

2. **Flat structure**: All components in a single directory
   ```
   Components/
   ├── Button.razor
   ├── Checkbox.razor
   ├── Label.razor
   ├── ValidatedInput.razor
   ├── Dialog.razor
   └── Popover.razor
   ```

### Decision

We chose the **flat directory structure** as the default installation pattern, with an optional `customOutputDir` parameter to allow users to override this behavior.

### Rationale

#### 1. Alignment with shadcn/ui Philosophy

Vibe.UI is explicitly inspired by [shadcn/ui](https://ui.shadcn.com/), which pioneered the "copy components as source code" approach in the React ecosystem. shadcn/ui uses a flat structure (`components/ui/`), and maintaining consistency with this proven pattern provides:

- Familiar developer experience for users coming from shadcn/ui
- Validation of the pattern's scalability (shadcn/ui has 50+ components)
- Community alignment and best practice adoption

#### 2. Simpler Import Paths

Flat structure results in cleaner, more predictable import paths:

**Flat structure:**
```razor
@using MyApp.Components.Button
@using MyApp.Components.Dialog
```

**Category structure:**
```razor
@using MyApp.Components.Input.Button
@using MyApp.Components.Overlay.Dialog
```

The flat structure reduces namespace depth and makes imports more consistent across the codebase.

#### 3. Reduced Cognitive Load

With a flat structure, developers don't need to make or remember categorization decisions:

- "Is Select an Input component or a Form component?"
- "Where did I put the Combobox?"
- "Should Dialog be in Overlay or Feedback?"

These questions are eliminated when all components are in one directory.

#### 4. Better Discoverability

Modern IDEs provide excellent search and autocomplete features. A flat directory structure:

- Shows all components in a single directory listing
- Works well with fuzzy search (Ctrl+P in VS Code)
- Provides cleaner autocomplete suggestions
- Reduces navigation friction (no drilling into subdirectories)

#### 5. Scalability with Modern Tooling

The flat structure scales well even with 111 components because:

- IDE search/filter handles large directories efficiently
- Alphabetical sorting makes components easy to find
- Component names naturally namespace themselves (e.g., `Button`, `DialogButton`, `IconButton`)
- Modern file explorers handle large directories without performance issues

#### 6. Flexibility Through Customization

The `customOutputDir` parameter provides an escape hatch for teams with different preferences:

```bash
# Default flat structure (recommended)
vibe add button
# Result: Components/Button.razor

# Custom category structure (if needed)
vibe add button --output Components/Input
# Result: Components/Input/Button.razor
```

This design is **opinionated but not prescriptive**: we provide a sensible default while allowing customization.

### Trade-offs and Mitigations

#### Potential Concern: Directory Clutter

**Concern**: 111 components in one directory might feel overwhelming.

**Mitigation**:
- Modern IDEs handle this well with search/filter
- Component naming conventions provide visual grouping
- The categorization exists in documentation and the component registry
- Teams can use `--output` if they prefer subdirectories

#### Potential Concern: Loss of Visual Grouping

**Concern**: Can't see "all Input components" at a glance.

**Mitigation**:
- Documentation provides categorical organization
- The CLI `vibe list` command shows components by category
- Component names can include prefixes for visual grouping (e.g., `FormLabel`, `FormField`)
- Source code retains category organization for maintainer convenience

### Implementation Details

#### Source Code Organization

The **source code** in `src/Vibe.UI/Components/` IS organized by category:

```
src/Vibe.UI/Components/
├── Input/
│   ├── Button.razor
│   └── Checkbox.razor
├── Form/
│   └── Label.razor
└── Overlay/
    └── Dialog.razor
```

This provides:
- Easier maintenance and contribution
- Logical grouping for developers working on the library
- Clearer component relationships

#### Component Installation

The **ComponentService.InstallComponentAsync** method implements the flat structure:

```csharp
var targetDir = string.IsNullOrEmpty(customOutputDir)
    ? Path.Combine(projectPath, componentsDir)      // FLAT: Components/
    : Path.Combine(projectPath, customOutputDir);   // CUSTOM: user choice
```

This separation allows:
- Maintainers to organize by category
- Users to receive components in a flat structure
- Best of both worlds: maintenance convenience + user convenience

### Testing Implications

Tests must reflect the flat structure:

```csharp
// Correct test expectation (flat structure)
var componentPath = Path.Combine(_testProjectPath, "Components", "Button.razor");
File.Exists(componentPath).Should().BeTrue();

// Incorrect test expectation (category structure)
// var componentPath = Path.Combine(_testProjectPath, "Components", "Input", "Button.razor");
```

Tests that expected category subdirectories have been updated to align with the actual implementation.

### Future Considerations

#### If the library grows beyond 200 components:

1. **Consider namespace prefixes**: `Input.Button`, `Form.Button`, etc.
2. **Sub-module organization**: Separate packages for different component groups
3. **Enhanced CLI filtering**: `vibe list --category Input`
4. **Still maintain flat default**: The pattern scales well with tooling improvements

#### Monitoring user feedback:

- Track GitHub issues related to component organization
- Survey users on their actual usage patterns
- Consider making category structure opt-in if demand is significant
- Continue to align with shadcn/ui evolution

### References

- [shadcn/ui Documentation](https://ui.shadcn.com/)
- [Vibe.UI CLI README](../src/Vibe.UI.CLI/README.cli.md)
- [ComponentService Implementation](../src/Vibe.UI.CLI/Services/ComponentService.cs)

### Decision History

- **2024-11**: Initial implementation with flat structure default
- **2025-11**: Comprehensive documentation added, tests updated to reflect correct structure

---

## Future Architecture Decisions

Additional architectural decisions will be documented here as the project evolves.

### Template

```markdown
## Architectural Decision: [Title]

### Status
[Proposed | Accepted | Deprecated | Superseded]

### Context
[What is the issue we're trying to solve?]

### Decision
[What is the change we're making?]

### Rationale
[Why are we making this change?]

### Trade-offs
[What are the disadvantages or risks?]

### Implementation
[How is this implemented?]

### References
[Links to relevant resources]
```

---

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
