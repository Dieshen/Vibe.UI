namespace Vibe.UI.Tests.Components.Advanced;

public class KanbanBoardTests : TestBase
{
    [Fact]
    public void KanbanBoard_Renders_WithAccessibleDefaultProps()
    {
        // Act
        var cut = Render<KanbanBoard>();

        // Assert
        var kanban = cut.Find(".vibe-kanban");
        kanban.GetAttribute("role").ShouldBe("region");
        kanban.GetAttribute("aria-label").ShouldBe("Kanban board");
        kanban.GetAttribute("aria-disabled").ShouldBe("false");
        kanban.GetAttribute("data-readonly").ShouldBe("false");

        var empty = cut.Find(".kanban-empty");
        empty.GetAttribute("role").ShouldBe("status");
        empty.TextContent.ShouldBe("No columns");
        cut.FindAll(".kanban-columns").ShouldBeEmpty();
    }

    [Fact]
    public void KanbanBoard_Displays_EmptyContent_WhenNoColumns()
    {
        // Arrange
        var emptyMarkup = "<div>No columns</div>";

        // Act
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.EmptyContent, emptyMarkup));

        // Assert
        var empty = cut.Find(".kanban-empty");
        empty.ShouldNotBeNull();
        empty.InnerHtml.ShouldContain("No columns");
    }

    [Fact]
    public void KanbanBoard_Renders_Columns_WithListSemantics()
    {
        // Arrange
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new()
            {
                Id = "todo",
                Title = "To Do",
                Cards = new()
                {
                    new() { Id = "card1", Title = "Task 1" },
                    new() { Id = "card2", Title = "Task 2" }
                }
            },
            new() { Id = "done", Title = "Done", Cards = new() }
        };

        // Act
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var columnList = cut.Find(".kanban-columns");
        columnList.GetAttribute("role").ShouldBe("list");
        columnList.GetAttribute("aria-label").ShouldBe("Kanban columns");

        var columnElements = cut.FindAll(".kanban-column");
        columnElements.Count.ShouldBe(2);
        columnElements[0].GetAttribute("role").ShouldBe("listitem");
        columnElements[0].GetAttribute("aria-label").ShouldBe("To Do, 2 cards");
        columnElements[1].GetAttribute("aria-label").ShouldBe("Done, 0 cards");

        cut.Find(".column-count").GetAttribute("aria-label").ShouldBe("2 cards");
        cut.Find(".kanban-column-empty").TextContent.ShouldBe("No cards");
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
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var cards = cut.FindAll(".kanban-card");
        cards.Count.ShouldBe(2);
        cards[0].GetAttribute("role").ShouldBe("group");
        cards[0].GetAttribute("aria-label").ShouldBe("Task 1");
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
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var tags = cut.FindAll(".card-tag");
        tags.Count.ShouldBe(2);
        cut.Markup.ShouldContain("bug");
        cut.Markup.ShouldContain("urgent");
    }

    [Fact]
    public void KanbanBoard_Ignores_NullColumnsCardsAndTags()
    {
        // Arrange
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            null!,
            new() { Id = "empty", Title = null!, Cards = null! },
            new()
            {
                Id = "cards",
                Title = "Cards",
                Cards = new()
                {
                    null!,
                    new()
                    {
                        Id = "card1",
                        Title = null,
                        Tags = new() { null!, " ", " bug " }
                    }
                }
            }
        };

        // Act
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        cut.FindAll(".kanban-column").Count.ShouldBe(2);
        cut.FindAll(".kanban-card").Count.ShouldBe(1);
        cut.Find(".kanban-column").GetAttribute("aria-label").ShouldBe("Untitled column, 0 cards");
        cut.Find(".kanban-card").GetAttribute("aria-label").ShouldBe("Untitled card");
        cut.Find(".card-tag").TextContent.ShouldBe("bug");
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
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns));

        // Assert
        var count = cut.Find(".column-count");
        count.TextContent.ShouldBe("3");
        count.GetAttribute("aria-label").ShouldBe("3 cards");
    }

    [Fact]
    public void KanbanBoard_Shows_DisabledAddCardButton_WhenNoCallbackIsRegistered()
    {
        // Arrange
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new() { Id = "1", Title = "To Do", Cards = new() }
        };

        // Act
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.AllowAddCard, true));

        // Assert
        var addButton = cut.Find(".column-add-btn");
        addButton.GetAttribute("aria-label").ShouldBe("Add card to To Do");
        addButton.HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void KanbanBoard_InvokesAddCardRequested_WhenEnabledButtonIsClicked()
    {
        // Arrange
        string? requestedColumnId = null;
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new() { Id = "todo", Title = "To Do", Cards = new() }
        };

        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.OnAddCardRequested, id => requestedColumnId = id));

        // Act
        cut.Find(".column-add-btn").Click();

        // Assert
        requestedColumnId.ShouldBe("todo");
        cut.Find(".column-add-btn").HasAttribute("disabled").ShouldBeFalse();
    }

    [Fact]
    public void KanbanBoard_DoesNotInvokeAddCardRequested_WhenColumnIdIsBlank()
    {
        // Arrange
        var requested = false;
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new() { Id = " ", Title = "To Do", Cards = new() }
        };

        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.OnAddCardRequested, _ => requested = true));

        // Act
        cut.Find(".column-add-btn").Click();

        // Assert
        requested.ShouldBeFalse();
        cut.Find(".column-add-btn").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void KanbanBoard_InvokesCardClicked_WhenCardIsClicked()
    {
        // Arrange
        KanbanBoard.KanbanCard? clickedCard = null;
        var card = new KanbanBoard.KanbanCard { Id = "card1", Title = "Task 1" };
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new() { Id = "1", Title = "To Do", Cards = new() { card } }
        };

        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.OnCardClicked, EventCallback.Factory.Create<KanbanBoard.KanbanCard>(
                this, c => clickedCard = c)));

        // Act
        var cardElement = cut.Find(".kanban-card");
        cardElement.Click();

        // Assert
        clickedCard.ShouldBeSameAs(card);
        cardElement.GetAttribute("role").ShouldBe("button");
        cardElement.GetAttribute("tabindex").ShouldBe("0");
        cardElement.GetAttribute("aria-disabled").ShouldBe("false");
    }

    [Fact]
    public void KanbanBoard_InvokesCardClicked_WhenCardIsKeyboardActivated()
    {
        // Arrange
        KanbanBoard.KanbanCard? clickedCard = null;
        var card = new KanbanBoard.KanbanCard { Id = "card1", Title = "Task 1" };
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new() { Id = "1", Title = "To Do", Cards = new() { card } }
        };

        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.OnCardClicked, EventCallback.Factory.Create<KanbanBoard.KanbanCard>(
                this, c => clickedCard = c)));

        // Act
        cut.Find(".kanban-card").KeyDown("Enter");

        // Assert
        clickedCard.ShouldBeSameAs(card);
    }

    [Fact]
    public void KanbanBoard_DisabledState_SuppressesInteractionsAndDragging()
    {
        // Arrange
        var clicked = false;
        var addRequested = false;
        var columns = CreateMoveColumns();

        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.Disabled, true)
            .Add(p => p.OnCardClicked, _ => clicked = true)
            .Add(p => p.OnAddCardRequested, _ => addRequested = true));

        // Act
        cut.Find(".kanban-card").Click();
        cut.Find(".column-add-btn").Click();

        // Assert
        var kanban = cut.Find(".vibe-kanban");
        kanban.GetAttribute("aria-disabled").ShouldBe("true");
        kanban.ClassList.ShouldContain("vibe-kanban-disabled");

        var card = cut.Find(".kanban-card");
        card.GetAttribute("aria-disabled").ShouldBe("true");
        card.GetAttribute("draggable").ShouldBe("false");
        card.HasAttribute("tabindex").ShouldBeFalse();
        cut.Find(".column-add-btn").HasAttribute("disabled").ShouldBeTrue();
        clicked.ShouldBeFalse();
        addRequested.ShouldBeFalse();
    }

    [Fact]
    public void KanbanBoard_ReadOnlyState_SuppressesInteractionsWithoutDisablingRoot()
    {
        // Arrange
        var clicked = false;
        var columns = CreateMoveColumns();

        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.ReadOnly, true)
            .Add(p => p.OnCardClicked, _ => clicked = true));

        // Act
        cut.Find(".kanban-card").Click();

        // Assert
        var kanban = cut.Find(".vibe-kanban");
        kanban.GetAttribute("aria-disabled").ShouldBe("false");
        kanban.GetAttribute("data-readonly").ShouldBe("true");
        kanban.ClassList.ShouldContain("vibe-kanban-readonly");
        cut.Find(".kanban-card").GetAttribute("aria-disabled").ShouldBe("true");
        clicked.ShouldBeFalse();
    }

    [Fact]
    public void KanbanBoard_DoesNotInvokeCardClicked_WhenCardIsDisabled()
    {
        // Arrange
        var clicked = false;
        var columns = new List<KanbanBoard.KanbanColumn>
        {
            new()
            {
                Id = "todo",
                Title = "To Do",
                Cards = new()
                {
                    new() { Id = "card1", Title = "Task 1", IsDisabled = true }
                }
            }
        };

        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.OnCardClicked, _ => clicked = true));

        // Act
        cut.Find(".kanban-card").Click();

        // Assert
        cut.Find(".kanban-card").ClassList.ShouldContain("kanban-card-disabled");
        cut.Find(".kanban-card").GetAttribute("aria-disabled").ShouldBe("true");
        clicked.ShouldBeFalse();
    }

    [Fact]
    public async Task KanbanBoard_MoveCard_MovesCardAndInvokesCallback()
    {
        // Arrange
        KanbanBoard.CardMovedEventArgs? moved = null;
        var columns = CreateMoveColumns();
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.OnCardMoved, EventCallback.Factory.Create<KanbanBoard.CardMovedEventArgs>(
                this, args => moved = args)));

        // Act
        await cut.InvokeAsync(() => cut.Instance.MoveCard("card1", "todo", "done"));

        // Assert
        columns[0].Cards!.ShouldBeEmpty();
        columns[1].Cards!.Single().Id.ShouldBe("card1");
        moved.ShouldNotBeNull();
        moved.Card.Id.ShouldBe("card1");
        moved.FromColumnId.ShouldBe("todo");
        moved.ToColumnId.ShouldBe("done");
    }

    [Fact]
    public async Task KanbanBoard_DragLifecycle_MovesCardToDropTarget()
    {
        // Arrange
        KanbanBoard.CardMovedEventArgs? moved = null;
        var columns = CreateMoveColumns();
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.OnCardMoved, EventCallback.Factory.Create<KanbanBoard.CardMovedEventArgs>(
                this, args => moved = args)));

        // Act
        await cut.Find(".kanban-card").TriggerEventAsync("ondragstart", new Microsoft.AspNetCore.Components.Web.DragEventArgs());
        var destinationCards = cut.FindAll(".kanban-cards")[1];
        await destinationCards.TriggerEventAsync("ondragover", new Microsoft.AspNetCore.Components.Web.DragEventArgs());

        // Assert
        cut.FindAll(".kanban-column")[1].ClassList.ShouldContain("kanban-column-drop-target");

        // Act
        destinationCards = cut.FindAll(".kanban-cards")[1];
        await destinationCards.TriggerEventAsync("ondrop", new Microsoft.AspNetCore.Components.Web.DragEventArgs());

        // Assert
        columns[0].Cards!.ShouldBeEmpty();
        columns[1].Cards!.Single().Id.ShouldBe("card1");
        moved.ShouldNotBeNull();
        moved.ToColumnId.ShouldBe("done");
        cut.FindAll(".kanban-column")[1].ClassList.ShouldNotContain("kanban-column-drop-target");
    }

    [Fact]
    public async Task KanbanBoard_ControlArrow_MovesFocusedCardToAdjacentColumn()
    {
        // Arrange
        var columns = CreateMoveColumns();
        var cut = Render<KanbanBoard>(parameters => parameters.Add(p => p.Columns, columns));
        var card = cut.Find(".kanban-card");

        // Assert
        card.GetAttribute("tabindex").ShouldBe("0");
        (card.GetAttribute("aria-keyshortcuts") ?? string.Empty).ShouldContain("Control+ArrowRight");

        // Act
        await card.TriggerEventAsync(
            "onkeydown",
            new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowRight", CtrlKey = true });

        // Assert
        columns[0].Cards!.ShouldBeEmpty();
        columns[1].Cards!.Single().Id.ShouldBe("card1");
    }

    [Fact]
    public async Task KanbanBoard_MoveCard_DoesNothingForInvalidInputs()
    {
        // Arrange
        var moveCount = 0;
        var columns = CreateMoveColumns();
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, columns)
            .Add(p => p.OnCardMoved, _ => moveCount++));

        // Act
        await cut.InvokeAsync(() => cut.Instance.MoveCard(" ", "todo", "done"));
        await cut.InvokeAsync(() => cut.Instance.MoveCard("missing", "todo", "done"));
        await cut.InvokeAsync(() => cut.Instance.MoveCard("card1", "todo", "todo"));
        await cut.InvokeAsync(() => cut.Instance.MoveCard("card1", "missing", "done"));

        // Assert
        columns[0].Cards!.Single().Id.ShouldBe("card1");
        columns[1].Cards!.ShouldBeEmpty();
        moveCount.ShouldBe(0);
    }

    [Fact]
    public async Task KanbanBoard_MoveCard_DoesNothingWhenBoardOrParticipantsAreNonInteractive()
    {
        // Arrange
        var moveCount = 0;
        var disabledColumnSet = CreateMoveColumns();
        disabledColumnSet[1].IsDisabled = true;

        var disabledColumnCut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, disabledColumnSet)
            .Add(p => p.OnCardMoved, _ => moveCount++));

        // Act
        await disabledColumnCut.InvokeAsync(() => disabledColumnCut.Instance.MoveCard("card1", "todo", "done"));

        var disabledBoardSet = CreateMoveColumns();
        var disabledBoardCut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, disabledBoardSet)
            .Add(p => p.Disabled, true)
            .Add(p => p.OnCardMoved, _ => moveCount++));
        await disabledBoardCut.InvokeAsync(() => disabledBoardCut.Instance.MoveCard("card1", "todo", "done"));

        var readOnlyBoardSet = CreateMoveColumns();
        var readOnlyBoardCut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.Columns, readOnlyBoardSet)
            .Add(p => p.ReadOnly, true)
            .Add(p => p.OnCardMoved, _ => moveCount++));
        await readOnlyBoardCut.InvokeAsync(() => readOnlyBoardCut.Instance.MoveCard("card1", "todo", "done"));

        // Assert
        disabledColumnSet[0].Cards!.Single().Id.ShouldBe("card1");
        disabledColumnSet[1].Cards!.ShouldBeEmpty();
        disabledBoardSet[0].Cards!.Single().Id.ShouldBe("card1");
        disabledBoardSet[1].Cards!.ShouldBeEmpty();
        readOnlyBoardSet[0].Cards!.Single().Id.ShouldBe("card1");
        readOnlyBoardSet[1].Cards!.ShouldBeEmpty();
        moveCount.ShouldBe(0);
    }

    [Fact]
    public void KanbanBoard_Applies_CustomCssClass_ClassAndAdditionalAttributes()
    {
        // Act
        var cut = Render<KanbanBoard>(parameters => parameters
            .Add(p => p.CssClass, "custom-kanban")
            .Add(p => p.Class, "base-class")
            .Add(p => p.AriaLabel, "Project board")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                { "data-testid", "kanban" }
            }));

        // Assert
        var kanban = cut.Find(".vibe-kanban");
        kanban.ClassList.ShouldContain("custom-kanban");
        kanban.ClassList.ShouldContain("base-class");
        kanban.GetAttribute("aria-label").ShouldBe("Project board");
        kanban.GetAttribute("data-testid").ShouldBe("kanban");
    }

    private static List<KanbanBoard.KanbanColumn> CreateMoveColumns()
    {
        var card = new KanbanBoard.KanbanCard { Id = "card1", Title = "Task 1" };

        return new()
        {
            new() { Id = "todo", Title = "To Do", Cards = new() { card } },
            new() { Id = "done", Title = "Done", Cards = new() }
        };
    }
}
