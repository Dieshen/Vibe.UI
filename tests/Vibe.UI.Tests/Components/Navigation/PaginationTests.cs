namespace Vibe.UI.Tests.Components.Navigation;

public class PaginationTests : TestBase
{
    [Fact]
    public void Pagination_Renders_WithDefaultProps()
    {
        // Act
        var cut = Render<Pagination>();

        // Assert
        var pagination = cut.Find(".vibe-pagination");
        pagination.ShouldNotBeNull();
        pagination.GetAttribute("role")!.ShouldBe("navigation");
    }

    [Fact]
    public void Pagination_RendersListSemanticsAndAccessibleDefaults()
    {
        var cut = Render<Pagination>();

        cut.Find("nav.vibe-pagination").GetAttribute("aria-label").ShouldBe("pagination");
        cut.Find(".pagination-list").GetAttribute("role").ShouldBe("list");
        cut.Find(".pagination-prev").GetAttribute("type").ShouldBe("button");
        cut.Find(".pagination-prev").GetAttribute("aria-label").ShouldBe("Go to previous page");
        cut.Find(".pagination-next").GetAttribute("aria-label").ShouldBe("Go to next page");
        cut.FindAll(".pagination-icon").ShouldAllBe(icon => icon.GetAttribute("aria-hidden") == "true");
    }

    [Fact]
    public void Pagination_Renders_CorrectNumberOfPages()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 1));

        // Assert
        var pageButtons = cut.FindAll(".pagination-link");
        pageButtons.Count.ShouldBe(5);
    }

    [Fact]
    public void Pagination_Highlights_CurrentPage()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 3));

        // Assert
        var activeButton = cut.Find(".pagination-link.active");
        activeButton.TextContent.Trim().ShouldBe("3");
        activeButton.GetAttribute("aria-current").ShouldBe("page");
        activeButton.GetAttribute("aria-label").ShouldBe("Page 3, current page");
    }

    [Fact]
    public void Pagination_LabelsInactivePagesWithoutCurrentState()
    {
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 3));

        var inactiveButton = cut.FindAll(".pagination-link").First(button => button.TextContent.Trim() == "4");
        inactiveButton.GetAttribute("aria-current").ShouldBeNull();
        inactiveButton.GetAttribute("aria-label").ShouldBe("Go to page 4");
    }

    [Fact]
    public void Pagination_Disables_PrevButton_OnFirstPage()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 1));

        // Assert
        var prevButton = cut.Find(".pagination-prev");
        prevButton.HasAttribute("disabled").ShouldBeTrue();
        prevButton.GetAttribute("aria-disabled").ShouldBe("true");
    }

    [Fact]
    public void Pagination_Disables_NextButton_OnLastPage()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 5));

        // Assert
        var nextButton = cut.Find(".pagination-next");
        nextButton.HasAttribute("disabled").ShouldBeTrue();
        nextButton.GetAttribute("aria-disabled").ShouldBe("true");
    }

    [Fact]
    public void Pagination_Shows_FirstLastButtons_WhenEnabled()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 10)
            .Add(p => p.CurrentPage, 5)
            .Add(p => p.ShowFirstLast, true));

        // Assert
        cut.FindAll(".pagination-first").ShouldNotBeEmpty();
        cut.FindAll(".pagination-last").ShouldNotBeEmpty();
    }

    [Fact]
    public void Pagination_Hides_FirstLastButtons_WhenDisabled()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 10)
            .Add(p => p.CurrentPage, 5)
            .Add(p => p.ShowFirstLast, false));

        // Assert
        cut.FindAll(".pagination-first").ShouldBeEmpty();
        cut.FindAll(".pagination-last").ShouldBeEmpty();
    }

    [Fact]
    public void Pagination_InvokesPageChanged_WhenPageClicked()
    {
        // Arrange
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.PageChanged, page => selectedPage = page));

        // Act
        var pageButtons = cut.FindAll(".pagination-link");
        pageButtons[2].Click(); // Click page 3

        // Assert
        selectedPage.ShouldBe(3);
    }

    [Fact]
    public void Pagination_Shows_Ellipsis_ForManyPages()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 20)
            .Add(p => p.CurrentPage, 10)
            .Add(p => p.MaxVisiblePages, 7));

        // Assert
        var ellipsis = cut.FindAll(".pagination-ellipsis");
        ellipsis.ShouldNotBeEmpty();
    }

    [Fact]
    public void Pagination_PreviousButton_DecreasesPage()
    {
        // Arrange
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 3)
            .Add(p => p.PageChanged, page => selectedPage = page));

        // Act
        cut.Find(".pagination-prev").Click();

        // Assert
        selectedPage.ShouldBe(2);
    }

    [Fact]
    public void Pagination_NextButton_IncreasesPage()
    {
        // Arrange
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 3)
            .Add(p => p.PageChanged, page => selectedPage = page));

        // Act
        cut.Find(".pagination-next").Click();

        // Assert
        selectedPage.ShouldBe(4);
    }

    [Fact]
    public void Pagination_FirstButton_NavigatesToFirstPage()
    {
        // Arrange
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 10)
            .Add(p => p.CurrentPage, 5)
            .Add(p => p.ShowFirstLast, true)
            .Add(p => p.PageChanged, page => selectedPage = page));

        // Act
        cut.Find(".pagination-first").Click();

        // Assert
        selectedPage.ShouldBe(1);
    }

    [Fact]
    public void Pagination_LastButton_NavigatesToLastPage()
    {
        // Arrange
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 10)
            .Add(p => p.CurrentPage, 5)
            .Add(p => p.ShowFirstLast, true)
            .Add(p => p.PageChanged, page => selectedPage = page));

        // Act
        cut.Find(".pagination-last").Click();

        // Assert
        selectedPage.ShouldBe(10);
    }

    [Fact]
    public void Pagination_FirstButton_DisabledOnFirstPage()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 10)
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.ShowFirstLast, true));

        // Assert
        var firstButton = cut.Find(".pagination-first");
        firstButton.HasAttribute("disabled").ShouldBeTrue();
        firstButton.ClassList.ShouldContain("disabled");
        firstButton.GetAttribute("aria-disabled").ShouldBe("true");
        firstButton.GetAttribute("aria-label").ShouldBe("Go to first page");
    }

    [Fact]
    public void Pagination_LastButton_DisabledOnLastPage()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 10)
            .Add(p => p.CurrentPage, 10)
            .Add(p => p.ShowFirstLast, true));

        // Assert
        var lastButton = cut.Find(".pagination-last");
        lastButton.HasAttribute("disabled").ShouldBeTrue();
        lastButton.ClassList.ShouldContain("disabled");
        lastButton.GetAttribute("aria-disabled").ShouldBe("true");
        lastButton.GetAttribute("aria-label").ShouldBe("Go to last page");
    }

    [Fact]
    public void Pagination_WithSinglePage_BothPrevNextDisabled()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 1)
            .Add(p => p.CurrentPage, 1));

        // Assert
        cut.Find(".pagination-prev").HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".pagination-next").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Pagination_WithZeroPages_HandlesGracefully()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 0)
            .Add(p => p.CurrentPage, 1));

        // Assert - should render without errors
        var pagination = cut.Find(".vibe-pagination");
        pagination.ShouldNotBeNull();
        cut.Find(".pagination-link.active").TextContent.Trim().ShouldBe("1");
        cut.Find(".pagination-prev").HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".pagination-next").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Pagination_ClickingCurrentPage_DoesNotInvokeCallback()
    {
        // Arrange
        var callbackInvoked = false;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 3)
            .Add(p => p.PageChanged, page => callbackInvoked = true));

        // Act - Find the active button and click it
        var activeButton = cut.Find(".pagination-link.active");
        activeButton.Click();

        // Assert - callback should not be invoked for current page
        callbackInvoked.ShouldBeFalse();
    }

    [Fact]
    public void Pagination_ClickingDisabledPrevButton_DoesNotInvokeCallback()
    {
        // Arrange
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.PageChanged, page => selectedPage = page));

        // Act - Try to click disabled prev button
        var prevButton = cut.Find(".pagination-prev");
        prevButton.Click();

        // Assert - callback should not be invoked
        selectedPage.ShouldBe(0);
    }

    [Fact]
    public void Pagination_ClickingDisabledNextButton_DoesNotInvokeCallback()
    {
        // Arrange
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 5)
            .Add(p => p.PageChanged, page => selectedPage = page));

        // Act - Try to click disabled next button
        var nextButton = cut.Find(".pagination-next");
        nextButton.Click();

        // Assert - callback should not be invoked
        selectedPage.ShouldBe(0);
    }

    [Fact]
    public void Pagination_VeryLargePageCount_RendersWithEllipsis()
    {
        // Act - 1000 pages
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 1000)
            .Add(p => p.CurrentPage, 500)
            .Add(p => p.MaxVisiblePages, 7));

        // Assert
        var ellipsis = cut.FindAll(".pagination-ellipsis");
        ellipsis.Count.ShouldBeGreaterThan(0);

        // Should not render all 1000 page buttons
        var pageButtons = cut.FindAll(".pagination-link");
        pageButtons.Count.ShouldBeLessThan(1000);
    }

    [Fact]
    public void Pagination_PageRangeCalculation_CurrentPageNearStart()
    {
        // Act - Current page near start
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 20)
            .Add(p => p.CurrentPage, 2)
            .Add(p => p.MaxVisiblePages, 7));

        // Assert - Should show page 1 and not have ellipsis at start
        var firstPageButton = cut.FindAll(".pagination-link").First();
        firstPageButton.TextContent.Trim().ShouldBe("1");
    }

    [Fact]
    public void Pagination_PageRangeCalculation_CurrentPageNearEnd()
    {
        // Act - Current page near end
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 20)
            .Add(p => p.CurrentPage, 19)
            .Add(p => p.MaxVisiblePages, 7));

        // Assert - Should show page 20
        var lastPageButton = cut.FindAll(".pagination-link").Last();
        lastPageButton.TextContent.Trim().ShouldBe("20");
    }

    [Fact]
    public void Pagination_PageRangeCalculation_CurrentPageInMiddle()
    {
        // Act - Current page in middle
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 20)
            .Add(p => p.CurrentPage, 10)
            .Add(p => p.MaxVisiblePages, 7));

        // Assert - Should have ellipsis on both sides
        var ellipsis = cut.FindAll(".pagination-ellipsis");
        ellipsis.Count.ShouldBe(2);
    }

    [Fact]
    public void Pagination_MaxVisiblePages_LimitsRenderedPages()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 100)
            .Add(p => p.CurrentPage, 50)
            .Add(p => p.MaxVisiblePages, 5));

        // Assert - Should not render more than MaxVisiblePages + ellipsis
        var pageButtons = cut.FindAll(".pagination-link");
        pageButtons.Count.ShouldBeLessThanOrEqualTo(5);
    }

    [Fact]
    public void Pagination_WithSmallMaxVisiblePages_ShowsCorrectPages()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 20)
            .Add(p => p.CurrentPage, 10)
            .Add(p => p.MaxVisiblePages, 3));

        // Assert - With MaxVisiblePages=3, algorithm shows first, current, and last
        // GetPageNumbers logic: always shows page 1, then middle range, then page TotalPages
        // With MaxVisiblePages=3, leftSide = max(2, 10 - 0) = 10, rightSide = min(19, 10 + -1) = 10
        // So it shows: 1, [ellipsis], 10, [ellipsis], 20 = 3 page buttons + 2 ellipsis
        var pageButtons = cut.FindAll(".pagination-link");
        pageButtons.Count.ShouldBeGreaterThanOrEqualTo(1); // At minimum shows some pages

        // Verify first and last pages are always shown
        var firstPage = pageButtons.First();
        firstPage.TextContent.Trim().ShouldBe("1");
        var lastPage = pageButtons.Last();
        lastPage.TextContent.Trim().ShouldBe("20");
    }

    [Fact]
    public void Pagination_WithMaxVisiblePagesBelowMinimum_RendersFirstCurrentLast()
    {
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 20)
            .Add(p => p.CurrentPage, 10)
            .Add(p => p.MaxVisiblePages, 0));

        cut.FindAll(".pagination-link")
            .Select(button => button.TextContent.Trim())
            .ShouldBe(["1", "10", "20"]);
        cut.FindAll(".pagination-ellipsis").Count.ShouldBe(2);
    }

    [Fact]
    public void Pagination_AriaLabel_SetCorrectly()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 1));

        // Assert
        var nav = cut.Find("nav");
        nav.GetAttribute("aria-label")!.ShouldBe("pagination");
    }

    [Fact]
    public void Pagination_CustomAriaLabelClassAndAdditionalAttributes_AreApplied()
    {
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.AriaLabel, " Search results pages ")
            .Add(p => p.Class, "compact-pages")
            .AddUnmatched("data-testid", "pager"));

        var nav = cut.Find(".vibe-pagination");
        nav.ClassList.ShouldContain("compact-pages");
        nav.GetAttribute("aria-label").ShouldBe("Search results pages");
        nav.GetAttribute("data-testid").ShouldBe("pager");
    }

    [Fact]
    public void Pagination_NavigationRole_SetCorrectly()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 1));

        // Assert
        var nav = cut.Find("nav");
        nav.GetAttribute("role")!.ShouldBe("navigation");
    }

    [Fact]
    public void Pagination_AllPagesVisible_NoEllipsis()
    {
        // Act - TotalPages equals MaxVisiblePages
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 7)
            .Add(p => p.CurrentPage, 4)
            .Add(p => p.MaxVisiblePages, 7));

        // Assert - Should not have ellipsis
        var ellipsis = cut.FindAll(".pagination-ellipsis");
        ellipsis.ShouldBeEmpty();
    }

    [Fact]
    public void Pagination_SequentialPageNavigation_WorksCorrectly()
    {
        // Arrange
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.PageChanged, page => selectedPage = page));

        // Act - Navigate sequentially from page 1 to 5
        cut.Find(".pagination-next").Click();
        selectedPage.ShouldBe(2);

        cut.Render(parameters => parameters.Add(p => p.CurrentPage, 2));
        cut.Find(".pagination-next").Click();
        selectedPage.ShouldBe(3);

        cut.Render(parameters => parameters.Add(p => p.CurrentPage, 3));
        cut.Find(".pagination-next").Click();
        selectedPage.ShouldBe(4);

        cut.Render(parameters => parameters.Add(p => p.CurrentPage, 4));
        cut.Find(".pagination-next").Click();
        selectedPage.ShouldBe(5);
    }

    [Fact]
    public void Pagination_DirectPageJump_WorksCorrectly()
    {
        // Arrange
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 10)
            .Add(p => p.CurrentPage, 1)
            .Add(p => p.PageChanged, page => selectedPage = page));

        // Act - Jump to a page that's guaranteed to be visible (last page always shown)
        var pageButtons = cut.FindAll(".pagination-link");
        var page10Button = pageButtons.FirstOrDefault(b => b.TextContent.Trim() == "10");
        page10Button.ShouldNotBeNull(); // Last page should always be visible
        page10Button.Click();

        // Assert
        selectedPage.ShouldBe(10);
    }

    [Fact]
    public void Pagination_ClampsCurrentPageAboveTotal_ForRenderingAndPreviousCallback()
    {
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, 10)
            .Add(p => p.PageChanged, page => selectedPage = page));

        cut.Find(".pagination-link.active").TextContent.Trim().ShouldBe("5");
        cut.Find(".pagination-next").HasAttribute("disabled").ShouldBeTrue();

        cut.Find(".pagination-prev").Click();

        selectedPage.ShouldBe(4);
    }

    [Fact]
    public void Pagination_ClampsCurrentPageBelowOne_ForRenderingAndNextCallback()
    {
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, -3)
            .Add(p => p.PageChanged, page => selectedPage = page));

        cut.Find(".pagination-link.active").TextContent.Trim().ShouldBe("1");
        cut.Find(".pagination-prev").HasAttribute("disabled").ShouldBeTrue();

        cut.Find(".pagination-next").Click();

        selectedPage.ShouldBe(2);
    }

    [Fact]
    public void Pagination_ClickingClampedCurrentPage_DoesNotInvokeCallback()
    {
        var selectedPage = 0;
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 5)
            .Add(p => p.CurrentPage, -3)
            .Add(p => p.PageChanged, page => selectedPage = page));

        cut.Find(".pagination-link.active").Click();

        selectedPage.ShouldBe(0);
    }

    [Fact]
    public void Pagination_WithMaxVisiblePagesLargerThanTotal_ShowsAllPages()
    {
        // Act
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 3)
            .Add(p => p.CurrentPage, 2)
            .Add(p => p.MaxVisiblePages, 10));

        // Assert - Should show all 3 pages
        var pageButtons = cut.FindAll(".pagination-link");
        pageButtons.Count.ShouldBe(3);

        // Should not have ellipsis
        var ellipsis = cut.FindAll(".pagination-ellipsis");
        ellipsis.ShouldBeEmpty();
    }

    [Fact]
    public void Pagination_EmptyResults_RendersWithoutCrashing()
    {
        // Act - Edge case: TotalPages = 1, which might represent empty results
        var cut = Render<Pagination>(parameters => parameters
            .Add(p => p.TotalPages, 1)
            .Add(p => p.CurrentPage, 1));

        // Assert
        var pagination = cut.Find(".vibe-pagination");
        pagination.ShouldNotBeNull();

        // Both prev and next should be disabled
        cut.Find(".pagination-prev").HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".pagination-next").HasAttribute("disabled").ShouldBeTrue();
    }
}
