using Microsoft.AspNetCore.Components.Web;

namespace Vibe.UI.Tests.Components.Advanced;

public class DragDropTests : TestBase
{
    [Fact]
    public void DragDrop_RendersAccessibleListAndHandles()
    {
        var cut = Render<DragDrop<string>>(parameters => parameters
            .Add(p => p.Items, ["Plan", "Build", "Ship"])
            .Add(p => p.AriaLabel, "Release steps"));

        var root = cut.Find(".vibe-dragdrop");
        root.GetAttribute("role").ShouldBe("list");
        root.GetAttribute("aria-label").ShouldBe("Release steps");
        root.GetAttribute("aria-disabled").ShouldBe("false");
        root.GetAttribute("data-orientation").ShouldBe("vertical");

        cut.FindAll("[role='listitem']").Count.ShouldBe(3);
        cut.FindAll(".dragdrop-handle").Count.ShouldBe(3);
        var firstHandle = cut.Find(".dragdrop-handle");
        firstHandle.GetAttribute("draggable").ShouldBe("true");
        firstHandle.GetAttribute("aria-keyshortcuts").ShouldBe("Alt+ArrowUp Alt+ArrowDown");
        firstHandle.QuerySelector("svg").ShouldNotBeNull();
    }

    [Fact]
    public void DragDrop_RendersItemTemplate()
    {
        var cut = Render<DragDrop<string>>(parameters => parameters
            .Add(p => p.Items, ["Build"])
            .Add(p => p.ItemTemplate, item => builder =>
            {
                builder.OpenElement(0, "strong");
                builder.AddContent(1, $"Task: {item}");
                builder.CloseElement();
            }));

        cut.Find(".dragdrop-item-content strong").TextContent.ShouldBe("Task: Build");
    }

    [Fact]
    public void DragDrop_DraggingReordersWithoutMutatingSource()
    {
        var source = new List<string> { "Plan", "Build", "Ship" };
        List<string>? changed = null;
        DragDrop<string>.ReorderEvent? reorder = null;
        var cut = Render<DragDrop<string>>(parameters => parameters
            .Add(p => p.Items, source)
            .Add(p => p.ItemsChanged, items => changed = items)
            .Add(p => p.OnReordered, value => reorder = value));

        cut.FindAll(".dragdrop-handle")[0].TriggerEvent("ondragstart", new DragEventArgs());
        cut.FindAll(".dragdrop-item")[2].TriggerEvent("ondragenter", new DragEventArgs());
        cut.FindAll(".dragdrop-item")[2].TriggerEvent("ondrop", new DragEventArgs());

        changed.ShouldBe(["Build", "Ship", "Plan"]);
        source.ShouldBe(["Plan", "Build", "Ship"]);
        reorder.ShouldNotBeNull();
        reorder.OldIndex.ShouldBe(0);
        reorder.NewIndex.ShouldBe(2);
        cut.FindAll(".dragdrop-item-content").Select(element => element.TextContent.Trim())
            .ShouldBe(["Build", "Ship", "Plan"]);
    }

    [Fact]
    public void DragDrop_AltArrowReordersAndAnnouncesMove()
    {
        List<string>? changed = null;
        var cut = Render<DragDrop<string>>(parameters => parameters
            .Add(p => p.Items, ["Plan", "Build", "Ship"])
            .Add(p => p.ItemsChanged, items => changed = items));

        cut.FindAll(".dragdrop-handle")[0].TriggerEvent(
            "onkeydown",
            new KeyboardEventArgs { Key = "ArrowDown", AltKey = true });

        changed.ShouldBe(["Build", "Plan", "Ship"]);
        cut.Find(".dragdrop-live-region").TextContent.ShouldContain("Moved Plan to position 2 of 3");
    }

    [Fact]
    public void DragDrop_AcceptedParentOrderDoesNotResetLocalEntries()
    {
        List<string>? changed = null;
        var cut = Render<DragDrop<string>>(parameters => parameters
            .Add(p => p.Items, ["Plan", "Build", "Ship"])
            .Add(p => p.ItemsChanged, items => changed = items));

        cut.Find(".dragdrop-handle").TriggerEvent(
            "onkeydown",
            new KeyboardEventArgs { Key = "ArrowDown", AltKey = true });
        changed.ShouldNotBeNull();

        cut.Render(parameters => parameters
            .Add(p => p.Items, changed)
            .Add(p => p.ItemsChanged, items => changed = items));

        cut.FindAll(".dragdrop-item-content").Select(element => element.TextContent.Trim())
            .ShouldBe(["Build", "Plan", "Ship"]);
        cut.Find(".dragdrop-live-region").TextContent.ShouldContain("Moved Plan to position 2 of 3");
    }

    [Fact]
    public void DragDrop_ArrowWithoutAltDoesNotReorder()
    {
        var callbackCount = 0;
        var cut = Render<DragDrop<string>>(parameters => parameters
            .Add(p => p.Items, ["Plan", "Build"])
            .Add(p => p.ItemsChanged, _ => callbackCount++));

        cut.Find(".dragdrop-handle").TriggerEvent(
            "onkeydown",
            new KeyboardEventArgs { Key = "ArrowDown" });

        callbackCount.ShouldBe(0);
        cut.FindAll(".dragdrop-item-content").Select(element => element.TextContent.Trim())
            .ShouldBe(["Plan", "Build"]);
    }

    [Fact]
    public void DragDrop_DisabledItemCannotMove()
    {
        var callbackCount = 0;
        var cut = Render<DragDrop<string>>(parameters => parameters
            .Add(p => p.Items, ["Pinned", "Flexible"])
            .Add(p => p.IsItemDisabled, item => item == "Pinned")
            .Add(p => p.ItemsChanged, _ => callbackCount++));

        var item = cut.FindAll(".dragdrop-item")[0];
        var handle = cut.FindAll(".dragdrop-handle")[0];
        item.ClassList.ShouldContain("dragdrop-item-disabled");
        item.GetAttribute("data-disabled").ShouldBe("true");
        handle.HasAttribute("disabled").ShouldBeTrue();
        handle.GetAttribute("draggable").ShouldBe("false");

        handle.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowDown", AltKey = true });
        callbackCount.ShouldBe(0);
    }

    [Fact]
    public void DragDrop_DisabledStateDisablesEveryHandle()
    {
        var cut = Render<DragDrop<string>>(parameters => parameters
            .Add(p => p.Items, ["Plan", "Build"])
            .Add(p => p.Disabled, true));

        cut.Find(".vibe-dragdrop").ClassList.ShouldContain("dragdrop-disabled");
        cut.Find(".vibe-dragdrop").GetAttribute("aria-disabled").ShouldBe("true");
        cut.FindAll(".dragdrop-handle").ShouldAllBe(handle => handle.HasAttribute("disabled"));
    }

    [Fact]
    public void DragDrop_NormalizesOrientationAndRendersEmptyState()
    {
        var cut = Render<DragDrop<string>>(parameters => parameters
            .Add(p => p.Items, [])
            .Add(p => p.Orientation, "HORIZONTAL")
            .Add(p => p.EmptyText, "Nothing queued"));

        var root = cut.Find(".vibe-dragdrop");
        root.ClassList.ShouldContain("dragdrop-horizontal");
        root.GetAttribute("data-orientation").ShouldBe("horizontal");
        cut.Find(".dragdrop-empty").TextContent.ShouldBe("Nothing queued");
    }

    [Fact]
    public void DragDrop_PreservesAdditionalAttributesAndClass()
    {
        var cut = Render<DragDrop<string>>(parameters => parameters
            .Add(p => p.Items, ["Plan"])
            .Add(p => p.Class, "release-order")
            .AddUnmatched("data-testid", "dragdrop"));

        var root = cut.Find(".vibe-dragdrop");
        root.ClassList.ShouldContain("release-order");
        root.GetAttribute("data-testid").ShouldBe("dragdrop");
    }
}
