namespace Vibe.UI.Tests.Components.Utility;

public class DropdownMenuTests : TestBase
{
    [Fact]
    public void DropdownMenu_RendersTriggerAndBaseClass()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Actions")));

        var root = cut.Find(".vibe-dropdown");
        root.ShouldNotBeNull();

        var trigger = cut.Find(".dropdown-trigger");
        trigger.TextContent.ShouldBe("Actions");
        trigger.TagName.ShouldBe("BUTTON");
        trigger.GetAttribute("type").ShouldBe("button");
        trigger.QuerySelector("button").ShouldBeNull();
        trigger.GetAttribute("aria-haspopup").ShouldBe("menu");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.GetAttribute("aria-controls").ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void DropdownMenu_DoesNotRenderContentInitially()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu item")));

        cut.FindAll(".dropdown-content").ShouldBeEmpty();
        cut.FindAll(".dropdown-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void DropdownMenu_TogglesContent_WhenTriggerIsClicked()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Actions"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<button>Delete</button>")));

        cut.Find(".dropdown-trigger").Click();

        cut.Find(".dropdown-content").TextContent.ShouldContain("Delete");
        cut.Find(".dropdown-trigger").GetAttribute("aria-expanded").ShouldBe("true");
        cut.Find(".dropdown-content").GetAttribute("role").ShouldBe("menu");
        cut.Find(".dropdown-content").GetAttribute("id").ShouldBe(cut.Find(".dropdown-trigger").GetAttribute("aria-controls"));
        cut.Find(".dropdown-backdrop").ShouldNotBeNull();

        cut.Find(".dropdown-trigger").Click();

        cut.FindAll(".dropdown-content").ShouldBeEmpty();
        cut.FindAll(".dropdown-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void DropdownMenu_BackdropClosesContent()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Actions"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu item")));

        cut.Find(".dropdown-trigger").Click();
        cut.Find(".dropdown-backdrop").Click();

        cut.FindAll(".dropdown-content").ShouldBeEmpty();
    }

    [Fact]
    public void DropdownMenu_OpensAndCloses_WithKeyboard()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Actions"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu item")));

        var trigger = cut.Find(".dropdown-trigger");
        trigger.Click();

        cut.Find(".dropdown-content").TextContent.ShouldContain("Menu item");
        cut.Find(".dropdown-trigger").GetAttribute("aria-expanded").ShouldBe("true");

        cut.Find(".dropdown-trigger").KeyDown("Escape");
        cut.FindAll(".dropdown-content").ShouldBeEmpty();
        cut.Find(".dropdown-trigger").GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public void DropdownMenu_ClosesWhenMenuContentIsSelected()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Actions"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<button role=\"menuitem\">Delete</button>")));

        cut.Find(".dropdown-trigger").Click();
        cut.Find("[role='menuitem']").Click();

        cut.FindAll(".dropdown-content").ShouldBeEmpty();
        cut.Find(".dropdown-trigger").GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public void DropdownMenu_CanKeepContentOpenAfterSelection()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Actions"))
            .Add(p => p.Content, builder => builder.AddMarkupContent(0, "<button role=\"menuitem\">Delete</button>"))
            .Add(p => p.CloseOnSelect, false));

        cut.Find(".dropdown-trigger").Click();
        cut.Find("[role='menuitem']").Click();

        cut.Find(".dropdown-content").ShouldNotBeNull();
    }

    [Fact]
    public void DropdownMenu_DisabledTriggerDoesNotOpen()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Actions"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu item"))
            .Add(p => p.Disabled, true));

        var trigger = cut.Find(".dropdown-trigger");
        trigger.HasAttribute("disabled").ShouldBeTrue();
        trigger.Click();

        cut.FindAll(".dropdown-content").ShouldBeEmpty();
    }

    [Fact]
    public void DropdownMenu_AppliesPositionAndAlignmentClasses()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.TriggerContent, builder => builder.AddContent(0, "Actions"))
            .Add(p => p.Content, builder => builder.AddContent(0, "Menu item"))
            .Add(p => p.Position, "top")
            .Add(p => p.Align, "end"));

        cut.Find(".dropdown-trigger").Click();

        var content = cut.Find(".dropdown-content");
        content.ClassList.ShouldContain("dropdown-top");
        content.ClassList.ShouldContain("dropdown-end");
    }

    [Fact]
    public void DropdownMenu_PreservesAdditionalAttributes()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "dropdown",
                ["aria-label"] = "Actions menu"
            }));

        var root = cut.Find(".vibe-dropdown");
        root.GetAttribute("data-testid").ShouldBe("dropdown");
        root.GetAttribute("aria-label").ShouldBe("Actions menu");
    }

    [Fact]
    public void DropdownMenu_AppliesCustomClass()
    {
        var cut = Render<DropdownMenu>(parameters => parameters
            .Add(p => p.Class, "compact-dropdown"));

        cut.Find(".vibe-dropdown").ClassList.ShouldContain("compact-dropdown");
    }
}
