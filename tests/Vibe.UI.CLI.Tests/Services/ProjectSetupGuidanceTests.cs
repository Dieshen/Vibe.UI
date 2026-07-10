using FluentAssertions;
using Vibe.UI.CLI.Models;
using Vibe.UI.CLI.Services;
using Xunit;

namespace Vibe.UI.CLI.Tests.Services;

public class ProjectSetupGuidanceTests
{
    [Fact]
    public void BuildNextSteps_WebApp_IncludesServerClientAndAssetsGuidance()
    {
        // Arrange
        var serverPath = Path.Combine(Path.GetTempPath(), "TestApp");
        var clientPath = Path.Combine(Path.GetTempPath(), "TestApp.Client");
        var topology = new ProjectTopology(
            BlazorProjectKind.BlazorWebApp,
            "Blazor Web App",
            Path.GetTempPath(),
            serverPath,
            Path.Combine(serverPath, "TestApp.csproj"),
            serverPath,
            Path.Combine(serverPath, "TestApp.csproj"),
            clientPath,
            Path.Combine(clientPath, "TestApp.Client.csproj"),
            "TestApp.Client");

        // Act
        var steps = ProjectSetupGuidance.BuildNextSteps(topology, clientPath, withCss: false);

        // Assert
        steps.Should().Contain(step => step.Contains("server project", StringComparison.OrdinalIgnoreCase)
            && step.Contains("AddVibeUI", StringComparison.Ordinal));
        steps.Should().Contain(step => step.Contains("client project", StringComparison.OrdinalIgnoreCase)
            && step.Contains("AddVibeUI", StringComparison.Ordinal));
        steps.Should().Contain(step => step.Contains("AddInteractiveWebAssemblyComponents", StringComparison.Ordinal));
        steps.Should().Contain(step => step.Contains("AddInteractiveWebAssemblyRenderMode", StringComparison.Ordinal));
        steps.Should().Contain(step => step.Contains("@Assets[\"css/vibe-base.css\"]", StringComparison.Ordinal));
        steps.Should().Contain(step => step.Contains("reuse the initialized client project automatically", StringComparison.Ordinal));
    }

    [Fact]
    public void BuildNextSteps_StandaloneWasm_UsesIndexHtmlDirectLinks()
    {
        // Arrange
        var projectPath = Path.Combine(Path.GetTempPath(), "TestWasm");
        var topology = new ProjectTopology(
            BlazorProjectKind.BlazorWebAssembly,
            "Blazor WebAssembly",
            projectPath,
            projectPath,
            Path.Combine(projectPath, "TestWasm.csproj"),
            null,
            null,
            projectPath,
            Path.Combine(projectPath, "TestWasm.csproj"),
            "TestWasm");

        // Act
        var steps = ProjectSetupGuidance.BuildNextSteps(topology, projectPath, withCss: false);

        // Assert
        steps.Should().Contain(step => step.Contains("Program.cs", StringComparison.Ordinal)
            && step.Contains("AddVibeUI", StringComparison.Ordinal));
        steps.Should().Contain(step => step.Contains("wwwroot/index.html", StringComparison.Ordinal)
            && step.Contains("css/vibe-base.css", StringComparison.Ordinal));
        steps.Should().NotContain(step => step.Contains("@Assets", StringComparison.Ordinal));
    }

    [Fact]
    public void BuildNextSteps_WebAppWithCss_UsesSingleServerOwnedOutput()
    {
        // Arrange
        var serverPath = Path.Combine(Path.GetTempPath(), "TestApp");
        var clientPath = Path.Combine(Path.GetTempPath(), "TestApp.Client");
        var topology = new ProjectTopology(
            BlazorProjectKind.BlazorWebApp,
            "Blazor Web App",
            Path.GetTempPath(),
            serverPath,
            Path.Combine(serverPath, "TestApp.csproj"),
            serverPath,
            Path.Combine(serverPath, "TestApp.csproj"),
            clientPath,
            Path.Combine(clientPath, "TestApp.Client.csproj"),
            "TestApp.Client");

        // Act
        var steps = ProjectSetupGuidance.BuildNextSteps(topology, clientPath, withCss: true);

        // Assert
        steps.Should().Contain(step => step.Contains("configured on the server", StringComparison.Ordinal)
            && step.Contains("shared server/client root", StringComparison.Ordinal));
        steps.Should().Contain(step => step.Contains("do not generate a second client copy", StringComparison.Ordinal));
        steps.Should().NotContain(step => step.Contains("vibe css --watch", StringComparison.Ordinal));
        steps.Should().NotContain(step => step.Contains("CSS commands", StringComparison.Ordinal));
    }
}
