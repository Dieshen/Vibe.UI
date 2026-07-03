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

        cut.FindAll("button.vibe-tag-remove").Count.ShouldBe(1);
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
}

