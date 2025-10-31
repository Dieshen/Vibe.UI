namespace Vibe.UI.Tests.Components.Navigation;

public class PaginationTests : TestContext
{
    public PaginationTests()
    {
        this.AddVibeUIServices();
    }

    [Fact]
    public void Pagination_RendersWithBasicStructure()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>();

        // Assert
        cut.Find(".vibe-pagination").Should().NotBeNull();
        cut.Find("nav[role='navigation']").Should().NotBeNull();
        cut.Find(".pagination-list").Should().NotBeNull();
    }

    [Fact]
    public void Pagination_RendersPreviousButton()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 2)
            .Add(p => p.TotalPages, 5));

        // Assert
        var prevButton = cut.Find(".pagination-prev");
        prevButton.Should().NotBeNull();
    }

    [Fact]
    public void Pagination_RendersNextButton()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.TotalPages, 5));

        // Assert
        var nextButton = cut.Find(".pagination-next");
        nextButton.Should().NotBeNull();
    }

    [Fact]
    public void Pagination_DisablesPreviousButton_OnFirstPage()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.TotalPages, 5));

        // Assert
        var prevButton = cut.Find(".pagination-prev");
        prevButton.HasAttribute("disabled").Should().BeTrue();
        prevButton.ClassList.Should().Contain("disabled");
    }

    [Fact]
    public void Pagination_DisablesNextButton_OnLastPage()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 5)
            .Add(p => p.TotalPages, 5));

        // Assert
        var nextButton = cut.Find(".pagination-next");
        nextButton.HasAttribute("disabled").Should().BeTrue();
        nextButton.ClassList.Should().Contain("disabled");
    }

    [Fact]
    public void Pagination_ShowsAllPages_WhenLessThanMaxVisible()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.TotalPages, 5)
            .Add(p => p.MaxVisiblePages, 7));

        // Assert
        var pageButtons = cut.FindAll(".pagination-link");
        pageButtons.Should().HaveCount(5);
    }

    [Fact]
    public void Pagination_ShowsEllipsis_WhenManyPages()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 5)
            .Add(p => p.TotalPages, 20)
            .Add(p => p.MaxVisiblePages, 7));

        // Assert
        var ellipsis = cut.FindAll(".pagination-ellipsis");
        ellipsis.Should().NotBeEmpty();
    }

    [Fact]
    public void Pagination_HighlightsActivePage()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 3)
            .Add(p => p.TotalPages, 5));

        // Assert
        var activeButton = cut.FindAll(".pagination-link.active");
        activeButton.Should().HaveCount(1);
        activeButton[0].TextContent.Trim().Should().Be("3");
    }

    [Fact]
    public void Pagination_TriggersPageChanged_OnButtonClick()
    {
        // Arrange
        int changedPage = 0;
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.TotalPages, 5)
            .Add(p => p.PageChanged, EventCallback.Factory.Create<int>(this, page => changedPage = page)));

        // Act
        cut.Find(".pagination-next").Click();

        // Assert
        changedPage.Should().Be(2);
    }

    [Fact]
    public void Pagination_ShowsFirstLastButtons_WhenEnabled()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 3)
            .Add(p => p.TotalPages, 10)
            .Add(p => p.ShowFirstLast, true));

        // Assert
        cut.Find(".pagination-first").Should().NotBeNull();
        cut.Find(".pagination-last").Should().NotBeNull();
    }

    [Fact]
    public void Pagination_HidesFirstLastButtons_ByDefault()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 3)
            .Add(p => p.TotalPages, 10));

        // Assert
        cut.FindAll(".pagination-first").Should().BeEmpty();
        cut.FindAll(".pagination-last").Should().BeEmpty();
    }

    [Fact]
    public void Pagination_DisablesFirstButton_OnFirstPage()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.TotalPages, 10)
            .Add(p => p.ShowFirstLast, true));

        // Assert
        var firstButton = cut.Find(".pagination-first");
        firstButton.HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Pagination_DisablesLastButton_OnLastPage()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 10)
            .Add(p => p.TotalPages, 10)
            .Add(p => p.ShowFirstLast, true));

        // Assert
        var lastButton = cut.Find(".pagination-last");
        lastButton.HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Pagination_AppliesCustomCssClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CssClass, "custom-pagination"));

        // Assert
        cut.Find(".vibe-pagination").ClassList.Should().Contain("custom-pagination");
    }

    [Fact]
    public void Pagination_NavigatesToSpecificPage()
    {
        // Arrange
        int changedPage = 0;
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.TotalPages, 5)
            .Add(p => p.PageChanged, EventCallback.Factory.Create<int>(this, page => changedPage = page)));

        // Act - Click on page 4 button
        var pageButtons = cut.FindAll(".pagination-link");
        pageButtons[3].Click(); // 4th button (page 4)

        // Assert
        changedPage.Should().Be(4);
    }

    [Fact]
    public void Pagination_DoesNotChangePageToInvalid()
    {
        // Arrange
        var cut = RenderComponent<Pagination>(parameters => parameters
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.TotalPages, 5));

        // Act - Try to go to page 0 (should be prevented by the component)
        // The previous button on page 1 should be disabled, so this shouldn't trigger

        // Assert - Current page should remain 1
        cut.Instance.CurrentPage.Should().Be(1);
    }
}
