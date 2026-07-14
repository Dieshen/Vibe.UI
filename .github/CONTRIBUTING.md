# Contributing to Vibe.UI

Thank you for your interest in contributing to Vibe.UI! 🎉

## Table of Contents
- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [How to Contribute](#how-to-contribute)
- [Coding Guidelines](#coding-guidelines)
- [Commit Messages](#commit-messages)
- [Pull Request Process](#pull-request-process)
- [Component Development](#component-development)
- [Testing](#testing)

## Code of Conduct

This project adheres to a code of conduct. By participating, you are expected to uphold this code. Please be respectful and constructive in all interactions.

## Getting Started

1. **Fork the repository** - Click the "Fork" button at the top right
2. **Clone your fork** - `git clone https://github.com/YOUR-USERNAME/Vibe.UI.git`
3. **Add upstream remote** - `git remote add upstream https://github.com/Narcoleptic-Fox/Vibe.UI.git`
4. **Create a branch** - `git checkout -b feature/your-feature-name`

## Development Setup

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Git
- Your favorite code editor (VS Code, Visual Studio, Rider, etc.)

### Build the Project

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test

# Run the documentation site locally
cd samples/Vibe.UI.Docs
dotnet run
# Navigate to http://localhost:5000
```

### Project Structure

```
Vibe.UI/
├── src/
│   ├── Vibe.UI/              # Main component library
│   └── Vibe.UI.CLI/          # CLI tool
├── samples/
│   ├── Vibe.UI.Demo/         # Demo application
│   └── Vibe.UI.Docs/         # Documentation site
├── tests/
│   ├── Vibe.UI.Tests/        # Unit tests for components
│   └── Vibe.UI.CLI.Tests/    # CLI tests
└── .github/                  # GitHub configuration
```

## How to Contribute

### Reporting Bugs
- Use the [Bug Report template](.github/ISSUE_TEMPLATE/bug_report.yml)
- Search existing issues first to avoid duplicates
- Provide a minimal reproducible example
- Include version numbers and environment details

### Suggesting Features
- Use the [Feature Request template](.github/ISSUE_TEMPLATE/feature_request.yml)
- Explain the use case and benefits
- Provide examples or mockups if possible
- Discuss in [GitHub Discussions](https://github.com/Narcoleptic-Fox/Vibe.UI/discussions) first for major changes

### Improving Documentation
- Use the [Documentation template](.github/ISSUE_TEMPLATE/documentation.yml)
- Documentation PRs are always welcome!
- Update both code comments and user-facing docs

## Coding Guidelines

### C# Style
- Follow [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and small (single responsibility)

### Blazor Components
- Use PascalCase for component names
- Prefix internal components with underscore: `_InternalComponent.razor`
- Use `[Parameter]` attribute for component parameters
- Add XML comments for all parameters

Example:
```csharp
/// <summary>
/// A button component with various styles and sizes.
/// </summary>
public partial class Button : ComponentBase
{
    /// <summary>
    /// Gets or sets the button variant.
    /// </summary>
    [Parameter]
    public ButtonVariant Variant { get; set; } = ButtonVariant.Primary;
}
```

### CSS/Styling
- Use CSS custom properties for theming
- Follow BEM-like naming: `vibe-component__element--modifier`
- Keep styles scoped to components where possible
- Use semantic color names from the theme

## Commit Messages

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks
- `perf`: Performance improvements

**Examples:**
```
feat(button): add loading state support

fix(input): resolve focus issue on mobile browsers

docs(readme): update installation instructions

test(checkbox): add unit tests for indeterminate state
```

## Pull Request Process

1. **Update your fork**
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

2. **Make your changes**
   - Write clean, readable code
   - Add tests for new functionality
   - Update documentation

3. **Test thoroughly**
   ```bash
   dotnet test
   dotnet build --configuration Release
   ```

4. **Commit your changes**
   ```bash
   git add .
   git commit -m "feat(component): add new feature"
   ```

5. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create a Pull Request**
   - Use the PR template
   - Link related issues
   - Provide clear description and screenshots
   - Request review from maintainers

7. **Address feedback**
   - Respond to review comments
   - Make requested changes
   - Push updates to your branch

8. **Merge**
   - Maintainers will merge once approved
   - Delete your branch after merge

## Component Development

### Creating a New Component

1. **Create the component file**
   ```
   src/Vibe.UI/Components/{Category}/{ComponentName}.razor
   ```

2. **Component structure**
   ```razor
   @namespace Vibe.UI.Components
   @inherits Vibe.UI.Base.VibeComponent

   <div class="@CombinedClass" @attributes="AdditionalAttributes">
       @ChildContent
   </div>

   @code {
       [Parameter]
       public RenderFragment ChildContent { get; set; }

       protected override string ComponentClass => "vibe-component-name";
   }
   ```

3. **Add styling**
   ```
   src/Vibe.UI/Components/{Category}/{ComponentName}.razor.css
   ```

4. **Create documentation page**
   ```
   samples/Vibe.UI.Docs/Pages/Components/{ComponentName}View.razor
   ```

5. **Add to CLI registry** (if applicable)
   ```
   src/Vibe.UI.CLI/Services/ComponentRegistry.cs
   ```

### Component Checklist
- [ ] Component implements common parameters (Class, Style, Id, etc.)
- [ ] Component is accessible (ARIA attributes, keyboard support)
- [ ] Component is themeable (uses CSS custom properties)
- [ ] Component has XML documentation
- [ ] Component has CSS scoped styles
- [ ] Component has unit tests
- [ ] Component has documentation page with examples
- [ ] Component follows naming conventions

## Testing

### Unit Tests
```csharp
[Fact]
public void Button_RendersCorrectly()
{
    // Arrange
    using var ctx = new TestContext();

    // Act
    var cut = ctx.RenderComponent<Button>(parameters => parameters
        .Add(p => p.Variant, ButtonVariant.Primary)
        .Add(p => p.ChildContent, "Click Me")
    );

    // Assert
    cut.MarkupMatches("<button class=\"vibe-button vibe-button--primary\">Click Me</button>");
}
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific project tests
dotnet test tests/Vibe.UI.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Questions?

- Open a [Discussion](https://github.com/Narcoleptic-Fox/Vibe.UI/discussions)
- Check existing [Issues](https://github.com/Narcoleptic-Fox/Vibe.UI/issues)
- Read the [Documentation](https://github.com/Narcoleptic-Fox/Vibe.UI#readme)

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
