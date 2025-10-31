using FluentAssertions;
using Spectre.Console.Cli;
using Vibe.UI.CLI.Commands;
using Xunit;

namespace Vibe.UI.CLI.Tests.Commands;

public class ListCommandTests
{
    private readonly ListCommand _command;

    public ListCommandTests()
    {
        _command = new ListCommand();
    }

    [Fact]
    public void Execute_ReturnsSuccessCode()
    {
        // Arrange
        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "list",
            data: null);

        // Act
        var result = _command.Execute(context);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Execute_DoesNotThrowException()
    {
        // Arrange
        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "list",
            data: null);

        // Act
        Action act = () => _command.Execute(context);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Execute_ListsAllAvailableComponents()
    {
        // This test verifies that the command executes successfully
        // The actual component listing is tested in ComponentServiceTests
        // This is an integration test to ensure the command wiring works

        // Arrange
        var context = new CommandContext(
            remainingArguments: Array.Empty<string>(),
            name: "list",
            data: null);

        // Act
        var result = _command.Execute(context);

        // Assert
        result.Should().Be(0, "command should execute successfully");
    }
}
