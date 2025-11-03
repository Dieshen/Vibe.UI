namespace Vibe.UI.Tests.Components.Advanced;

public class KanbanBoardTests : TestBase
{
    [Fact]
    public void KanbanBoard_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<KanbanBoard>();

        // Assert
        var kanban = cut.Find(".vibe-kanban");
        kanban.ShouldNotBeNull();
    }

    [Fact]
    public void KanbanBoard_Displays_EmptyContent_WhenNoColumns()
    {
        // Arrange
        var emptyMarkup = "<div>No columns</div>";

        // Act
        var cut = RenderComponent<KanbanBoard>(parameters => parameters
            .Add(p => p.EmptyContent, emptyMarkup));

        // Assert
        var empty = cut.Find(".kanban-empty");
        empty.ShouldNotBeNull();
        empty.InnerHtml.ShouldContain("No columns");
    }

    [Fact]
    public void KanbanBoard_Renders_Columns()
    {
        // Arrange
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new() { Id = "1", Title = "To Do", Cards = new() },
            new() { Id = "2", Title = "In Progress", Cards = new() }
        };

        // Act
        var cut = RenderComponent<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var columnElements = cut.FindAll(".kanban-column");
        columnElements.Count.ShouldBe(2);
        cut.Markup.ShouldContain("To Do");
        cut.Markup.ShouldContain("In Progress");
    }

    [Fact]
    public void KanbanBoard_Displays_Cards()
    {
        // Arrange
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new()
            {
                Id = "1",
                Title = "To Do",
                Cards = new()
                {
                    new() { Id = "card1", Title = "Task 1", Description = "Do something" },
                    new() { Id = "card2", Title = "Task 2", Description = "Do something else" }
                }
            }
        };

        // Act
        var cut = RenderComponent<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var cards = cut.FindAll(".kanban-card");
        cards.Count.ShouldBe(2);
        cut.Markup.ShouldContain("Task 1");
        cut.Markup.ShouldContain("Do something");
    }

    [Fact]
    public void KanbanBoard_Displays_CardTags()
    {
        // Arrange
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new()
            {
                Id = "1",
                Title = "To Do",
                Cards = new()
                {
                    new() { Id = "card1", Title = "Task 1", Tags = new() { "bug", "urgent" } }
                }
            }
        };

        // Act
        var cut = RenderComponent<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var tags = cut.FindAll(".card-tag");
        tags.Count.ShouldBe(2);
        cut.Markup.ShouldContain("bug");
        cut.Markup.ShouldContain("urgent");
    }

    [Fact]
    public void KanbanBoard_Shows_ColumnCount()
    {
        // Arrange
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new()
            {
                Id = "1",
                Title = "To Do",
                Cards = new()
                {
                    new() { Id = "card1" },
                    new() { Id = "card2" },
                    new() { Id = "card3" }
                }
            }
        };

        // Act
        var cut = RenderComponent<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var count = cut.Find(".column-count");
        count.TextContent.ShouldBe("3");
    }

    [Fact]
    public void KanbanBoard_Shows_AddCardButton_WhenAllowAddCardIsTrue()
    {
        // Arrange
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new() { Id = "1", Title = "To Do", Cards = new() }
        };

        // Act
        var cut = RenderComponent<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.AllowAddCard, true));

        // Assert
        var addButton = cut.Find(".column-add-btn");
        addButton.ShouldNotBeNull();
    }

    [Fact]
    public void KanbanBoard_InvokesCardClicked_WhenCardIsClicked()
    {
        // Arrange
        KanbanBoard.KanbanCard clickedCard = null;
        var card = new KanbanBoard.KanbanCard { Id = "card1", Title = "Task 1" };
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new() { Id = "1", Title = "To Do", Cards = new() { card } }
        };

        var cut = RenderComponent<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.OnCardClicked, EventCallback.Factory.Create<KanbanBoard.KanbanCard>(
                this, c => clickedCard = c)));

        // Act
        var cardElement = cut.Find(".kanban-card");
        cardElement.Click();

        // Assert
        clickedCard.ShouldNotBeNull();
        clickedCard.Id.ShouldBe("card1");
    }

    [Fact]
    public void KanbanBoard_Applies_CustomCssClass()
    {
        // Act
        var cut = RenderComponent<KanbanBoard>(parameters => parameters
            .Add(p => p.CssClass, "custom-kanban"));

        // Assert
        var kanban = cut.Find(".vibe-kanban");
        kanban.ClassList.ShouldContain("custom-kanban");
    }
}
