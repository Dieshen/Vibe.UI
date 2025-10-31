using FluentAssertions;
using System.Linq;
using System.Windows.Forms;
using Vibe.UI.VS2022.Commands;
using Xunit;

namespace Vibe.UI.VS2022.Tests;

public class ComponentSelectionDialogTests
{
    [Fact]
    public void Constructor_InitializesWithComponents()
    {
        // Arrange
        var components = new[] { "button", "checkbox", "input", "select" };

        // Act
        var dialog = new ComponentSelectionDialog(components);

        // Assert
        dialog.Should().NotBeNull();
    }

    [Fact]
    public void Dialog_HasCorrectTitle()
    {
        // Arrange
        var components = new[] { "button" };

        // Act
        var dialog = new ComponentSelectionDialog(components);

        // Assert
        dialog.Text.Should().Be("Select Component");
    }

    [Fact]
    public void Dialog_HasCorrectSize()
    {
        // Arrange
        var components = new[] { "button" };

        // Act
        var dialog = new ComponentSelectionDialog(components);

        // Assert
        dialog.Size.Width.Should().Be(400);
        dialog.Size.Height.Should().Be(300);
    }

    [Fact]
    public void Dialog_IsCenterParent()
    {
        // Arrange
        var components = new[] { "button" };

        // Act
        var dialog = new ComponentSelectionDialog(components);

        // Assert
        dialog.StartPosition.Should().Be(FormStartPosition.CenterParent);
    }

    [Fact]
    public void Dialog_ContainsListBox()
    {
        // Arrange
        var components = new[] { "button", "checkbox" };

        // Act
        var dialog = new ComponentSelectionDialog(components);

        // Assert
        var listBoxes = dialog.Controls.OfType<ListBox>();
        listBoxes.Should().HaveCount(1, "dialog should have exactly one ListBox");
    }

    [Fact]
    public void Dialog_ContainsButton()
    {
        // Arrange
        var components = new[] { "button" };

        // Act
        var dialog = new ComponentSelectionDialog(components);

        // Assert
        var buttons = dialog.Controls.OfType<Button>();
        buttons.Should().HaveCountGreaterThan(0, "dialog should have at least one button");
    }

    [Fact]
    public void Dialog_ButtonHasCorrectText()
    {
        // Arrange
        var components = new[] { "button" };

        // Act
        var dialog = new ComponentSelectionDialog(components);
        var addButton = dialog.Controls.OfType<Button>().FirstOrDefault();

        // Assert
        addButton.Should().NotBeNull();
        addButton!.Text.Should().Be("Add");
    }

    [Fact]
    public void Dialog_ButtonHasCorrectDialogResult()
    {
        // Arrange
        var components = new[] { "button" };

        // Act
        var dialog = new ComponentSelectionDialog(components);
        var addButton = dialog.Controls.OfType<Button>().FirstOrDefault();

        // Assert
        addButton.Should().NotBeNull();
        addButton!.DialogResult.Should().Be(DialogResult.OK);
    }

    [Fact]
    public void Dialog_ListBoxFillsForm()
    {
        // Arrange
        var components = new[] { "button" };

        // Act
        var dialog = new ComponentSelectionDialog(components);
        var listBox = dialog.Controls.OfType<ListBox>().FirstOrDefault();

        // Assert
        listBox.Should().NotBeNull();
        listBox!.Dock.Should().Be(DockStyle.Fill);
    }

    [Fact]
    public void Dialog_ButtonIsAtBottom()
    {
        // Arrange
        var components = new[] { "button" };

        // Act
        var dialog = new ComponentSelectionDialog(components);
        var addButton = dialog.Controls.OfType<Button>().FirstOrDefault();

        // Assert
        addButton.Should().NotBeNull();
        addButton!.Dock.Should().Be(DockStyle.Bottom);
    }

    [Theory]
    [InlineData(new[] { "button" }, 1)]
    [InlineData(new[] { "button", "checkbox" }, 2)]
    [InlineData(new[] { "button", "checkbox", "input", "select" }, 4)]
    public void Dialog_ListBoxContainsAllComponents(string[] components, int expectedCount)
    {
        // Act
        var dialog = new ComponentSelectionDialog(components);
        var listBox = dialog.Controls.OfType<ListBox>().FirstOrDefault();

        // Assert
        listBox.Should().NotBeNull();
        listBox!.Items.Count.Should().Be(expectedCount);
    }

    [Fact]
    public void Dialog_AcceptButtonIsSet()
    {
        // Arrange
        var components = new[] { "button" };

        // Act
        var dialog = new ComponentSelectionDialog(components);
        var addButton = dialog.Controls.OfType<Button>().FirstOrDefault();

        // Assert
        dialog.AcceptButton.Should().Be(addButton);
    }

    [Fact]
    public void SelectedComponent_IsNull_WhenNoSelection()
    {
        // Arrange
        var components = new[] { "button", "checkbox" };

        // Act
        var dialog = new ComponentSelectionDialog(components);

        // Assert
        dialog.SelectedComponent.Should().BeNull();
    }

    [Fact]
    public void Dialog_CanBeInstantiatedMultipleTimes()
    {
        // Arrange
        var components = new[] { "button", "checkbox" };

        // Act
        var dialog1 = new ComponentSelectionDialog(components);
        var dialog2 = new ComponentSelectionDialog(components);

        // Assert
        dialog1.Should().NotBeNull();
        dialog2.Should().NotBeNull();
        dialog1.Should().NotBeSameAs(dialog2);
    }
}
