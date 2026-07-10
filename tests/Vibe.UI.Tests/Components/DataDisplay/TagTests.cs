using Bunit;
using Microsoft.AspNetCore.Components;
using Shouldly;
using Vibe.UI.Components;
using Vibe.UI.Enums;
using Xunit;

namespace Vibe.UI.Tests.Components.DataDisplay;

public class TagTests : BunitContext
{
    [Fact]
    public void Tag_RendersLabel()
    {
        var cut = Render<Tag>(p => p.Add(x => x.Label, "Hello"));
        cut.Markup.ShouldContain("Hello");
    }

    [Fact]
    public void Tag_AppliesVariantAndSizeClasses()
    {
        var cut = Render<Tag>(p => p
            .Add(x => x.Label, "Hello")
            .Add(x => x.Variant, TagVariant.Primary)
            .Add(x => x.Size, TagSize.Large));

        cut.Find("span").GetAttribute("class")!.ShouldContain("vibe-tag-primary");
        cut.Find("span").GetAttribute("class")!.ShouldContain("vibe-tag-large");
    }

    [Fact]
    public void Tag_WhenRemovable_RendersRemoveButton()
    {
        var cut = Render<Tag>(p => p
            .Add(x => x.Label, "Hello")
            .Add(x => x.Removable, true));

        var removeButton = cut.Find("button.vibe-tag-remove");
        removeButton.GetAttribute("type").ShouldBe("button");
        removeButton.GetAttribute("aria-label").ShouldBe("Remove Hello");
    }

    [Fact]
    public void Tag_UsesCustomRemoveAriaLabel_WhenProvided()
    {
        var cut = Render<Tag>(p => p
            .Add(x => x.Label, "Hello")
            .Add(x => x.Removable, true)
            .Add(x => x.RemoveAriaLabel, "Dismiss greeting tag"));

        cut.Find("button.vibe-tag-remove").GetAttribute("aria-label").ShouldBe("Dismiss greeting tag");
    }

    [Fact]
    public void Tag_WhenRemoveClicked_InvokesCallback()
    {
        var removed = false;
        var cut = Render<Tag>(p => p
            .Add(x => x.Label, "Hello")
            .Add(x => x.Removable, true)
            .Add(x => x.OnRemove, EventCallback.Factory.Create(this, () => removed = true)));

        cut.Find("button.vibe-tag-remove").Click();
        removed.ShouldBeTrue();
    }

    [Fact]
    public void Tag_AppliesCustomClassOnlyOnce()
    {
        var cut = Render<Tag>(p => p
            .Add(x => x.Label, "Hello")
            .Add(x => x.Class, "custom-tag"));

        var classes = cut.Find("span").GetAttribute("class")!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        classes.Count(c => c == "custom-tag").ShouldBe(1);
    }
}

