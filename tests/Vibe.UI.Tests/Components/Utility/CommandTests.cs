namespace Vibe.UI.Tests.Components.Utility;

public class CommandTests : TestBase
{
    [Fact]
    public void Command_RendersInputAndListbox()
    {
        var cut = RenderComponent<Command>();

        cut.Find(".vibe-command").ShouldNotBeNull();

        var input = cut.Find("input.command-input");
        input.GetAttribute("placeholder").ShouldBe("Type a command or search...");
        input.GetAttribute("role").ShouldBe("combobox");
        input.GetAttribute("aria-autocomplete").ShouldBe("list");
        input.GetAttribute("autocomplete").ShouldBe("off");
        input.GetAttribute("spellcheck").ShouldBe("false");

        var list = cut.Find(".command-list");
        list.GetAttribute("role").ShouldBe("listbox");
        list.GetAttribute("id").ShouldNotBeNullOrEmpty();
        input.GetAttribute("aria-controls").ShouldBe(list.GetAttribute("id"));
    }

    [Fact]
    public void Command_RendersIconAndShortcut()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Icon, "<svg data-testid=\"search\"></svg>")
            .Add(p => p.Items, CreateItems()));

        cut.Find(".command-input-icon").InnerHtml.ShouldContain("data-testid=\"search\"");
        cut.Find(".command-item-shortcut").TextContent.ShouldBe("Ctrl+K");
    }

    [Fact]
    public void Command_OmitsIcon_WhenIconIsEmpty()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Icon, string.Empty));

        cut.FindAll(".command-input-icon").ShouldBeEmpty();
    }

    [Fact]
    public void Command_RendersItemsWithOptionAttributes()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItems()));

        var items = cut.FindAll(".command-item");
        items.Count.ShouldBe(4);
        items[0].GetAttribute("role").ShouldBe("option");
        items[0].GetAttribute("aria-selected").ShouldBe("false");
        items[0].GetAttribute("aria-disabled").ShouldBe("false");
        items[1].GetAttribute("aria-disabled").ShouldBe("true");
        items[1].ClassList.ShouldContain("disabled");
    }

    [Fact]
    public void Command_SelectsFirstEnabledItem_OnFocus()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItemsWithDisabledFirst()));

        cut.Find("input").Focus();

        var items = cut.FindAll(".command-item");
        items[0].ClassList.ShouldNotContain("selected");
        items[1].ClassList.ShouldContain("selected");
        cut.Find("input").GetAttribute("aria-activedescendant").ShouldBe(items[1].GetAttribute("id"));
    }

    [Fact]
    public void Command_FiltersItemsByLabel()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItems()));

        cut.Find("input").Input("open");

        var item = cut.Find(".command-item");
        item.TextContent.ShouldContain("Open File");
        cut.FindAll(".command-item").Count.ShouldBe(1);
    }

    [Fact]
    public void Command_FiltersItemsByValue()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItems()));

        cut.Find("input").Input("settings");

        var item = cut.Find(".command-item");
        item.TextContent.ShouldContain("Preferences");
        cut.FindAll(".command-item").Count.ShouldBe(1);
    }

    [Fact]
    public void Command_RendersEmptyState_WhenNoItemsMatch()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItems()));

        cut.Find("input").Input("missing");

        cut.Find(".command-empty").TextContent.ShouldBe("No results found.");
        cut.FindAll(".command-item").ShouldBeEmpty();
    }

    [Fact]
    public void Command_ArrowDownSkipsDisabledItems()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItems()));

        cut.Find("input").Focus();
        cut.Find("input").KeyDown("ArrowDown");

        var items = cut.FindAll(".command-item");
        items[2].ClassList.ShouldContain("selected");
        cut.Find("input").GetAttribute("aria-activedescendant").ShouldBe(items[2].GetAttribute("id"));
    }

    [Fact]
    public void Command_HomeAndEndMoveToEnabledBounds()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItemsWithDisabledBounds()));

        cut.Find("input").Focus();
        cut.Find("input").KeyDown("End");

        var items = cut.FindAll(".command-item");
        items[2].ClassList.ShouldContain("selected");

        cut.Find("input").KeyDown("Home");

        items = cut.FindAll(".command-item");
        items[1].ClassList.ShouldContain("selected");
    }

    [Fact]
    public void Command_EnterSelectsHighlightedItemAndClearsInput()
    {
        Command.CommandItem? selected = null;
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItems())
            .Add(p => p.ItemSelected, item => selected = item));

        cut.Find("input").Focus();
        cut.Find("input").Input("open");
        cut.Find("input").KeyDown("Enter");

        selected.ShouldNotBeNull();
        selected.Value.ShouldBe("open");
        cut.Find("input").GetAttribute("value").ShouldBe(string.Empty);
    }

    [Fact]
    public void Command_ClickSelectsEnabledItem()
    {
        Command.CommandItem? selected = null;
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItems())
            .Add(p => p.ItemSelected, item => selected = item));

        cut.FindAll(".command-item")[2].Click();

        selected.ShouldNotBeNull();
        selected.Value.ShouldBe("save");
    }

    [Fact]
    public void Command_ClickDoesNotSelectDisabledItem()
    {
        Command.CommandItem? selected = null;
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItems())
            .Add(p => p.ItemSelected, item => selected = item));

        cut.FindAll(".command-item")[1].Click();

        selected.ShouldBeNull();
    }

    [Fact]
    public void Command_EscapeClearsInput()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.Items, CreateItems()));

        cut.Find("input").Input("open");
        cut.Find("input").KeyDown("Escape");

        cut.Find("input").GetAttribute("value").ShouldBe(string.Empty);
    }

    [Fact]
    public void Command_PreservesAdditionalAttributes()
    {
        var cut = RenderComponent<Command>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "command",
                ["aria-label"] = "Command palette"
            }));

        var root = cut.Find(".vibe-command");
        root.GetAttribute("data-testid").ShouldBe("command");
        root.GetAttribute("aria-label").ShouldBe("Command palette");
    }

    private static List<Command.CommandItem> CreateItems() =>
    [
        new() { Label = "Open File", Value = "open", Shortcut = "Ctrl+K" },
        new() { Label = "Delete File", Value = "delete", Disabled = true },
        new() { Label = "Save File", Value = "save" },
        new() { Label = "Preferences", Value = "settings" }
    ];

    private static List<Command.CommandItem> CreateItemsWithDisabledFirst() =>
    [
        new() { Label = "Delete File", Value = "delete", Disabled = true },
        new() { Label = "Open File", Value = "open" },
        new() { Label = "Save File", Value = "save" }
    ];

    private static List<Command.CommandItem> CreateItemsWithDisabledBounds() =>
    [
        new() { Label = "Delete File", Value = "delete", Disabled = true },
        new() { Label = "Open File", Value = "open" },
        new() { Label = "Save File", Value = "save" },
        new() { Label = "Archive File", Value = "archive", Disabled = true }
    ];
}
