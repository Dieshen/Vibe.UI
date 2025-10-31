# Visual Studio 2022 Extension Testing

## Overview

The VS2022 extension requires the Visual Studio SDK test infrastructure for comprehensive testing. This document outlines the testing strategy and requirements.

## Test Requirements

### Prerequisites
- Visual Studio 2022 (17.0+)
- Visual Studio SDK
- Microsoft.VisualStudio.Sdk.TestFramework NuGet package

## Testing Components

### AddComponentCommand Tests

**Test Coverage:**
1. Command initialization
2. Component selection dialog
3. CLI process execution
4. Error handling
5. Project directory detection

**Example Test Structure:**
```csharp
[TestClass]
public class AddComponentCommandTests
{
    [TestMethod]
    public async Task Execute_WithValidComponent_AddsComponentSuccessfully()
    {
        // Arrange
        var package = await CreateTestPackageAsync();
        var command = new AddComponentCommand(package, commandService);

        // Act
        command.Execute(null, EventArgs.Empty);

        // Assert
        // Verify component was added
    }

    [TestMethod]
    public void GetProjectDirectory_ReturnsCorrectPath()
    {
        // Test implementation
    }
}
```

## Integration Testing

### Manual Testing Checklist

- [ ] Extension loads in VS2022
- [ ] Command appears in Tools menu
- [ ] Component selection dialog displays all components
- [ ] Selecting a component executes `vibe add {component} -y`
- [ ] Success message displays for successful installation
- [ ] Error message displays for failures
- [ ] Works with Blazor WebAssembly projects
- [ ] Works with Blazor Server projects

### Test Scenarios

1. **Happy Path**
   - Open a Blazor project
   - Select Tools > Add Vibe Component
   - Choose a component from the dialog
   - Verify component is added to project

2. **Error Handling**
   - No project open → Should show error
   - Vibe CLI not installed → Should show error
   - Invalid component → Should show error

3. **UI Tests**
   - Dialog displays all components correctly
   - Dialog can be cancelled
   - Dialog returns selected component

## Automated Testing Setup

To set up automated tests:

1. Create a test project:
```bash
dotnet new classlib -n Vibe.UI.VS2022.Tests
```

2. Add required packages:
```xml
<PackageReference Include="Microsoft.VisualStudio.Sdk.TestFramework" Version="17.0.*" />
<PackageReference Include="Microsoft.VisualStudio.Shell.15.0" Version="17.0.*" />
<PackageReference Include="xunit" Version="2.8.*" />
```

3. Configure test settings in .runsettings

4. Run tests with:
```bash
vstest.console.exe Vibe.UI.VS2022.Tests.dll
```

## Coverage Goals

Target coverage: 80%+

### Current Status
- AddComponentCommand: Manual testing only
- ComponentSelectionDialog: Manual testing only

## Future Improvements

1. Add unit tests using VS SDK test framework
2. Add UI automation tests
3. Add integration tests with mock VS environment
4. Set up CI/CD pipeline for extension testing
5. Add performance tests for command execution

## Resources

- [Visual Studio SDK Testing Documentation](https://docs.microsoft.com/en-us/visualstudio/extensibility/testing-extensions)
- [VSIX Testing Guide](https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-test-a-visual-studio-extension)
