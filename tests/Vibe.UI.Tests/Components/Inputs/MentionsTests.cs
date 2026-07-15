namespace Vibe.UI.Tests.Components.Inputs;

public class MentionsTests : TestBase
{
    [Fact]
    public void Mentions_Renders_WithDefaultProps()
    {
        var cut = Render<Mentions>();

        cut.Find(".vibe-mentions").ShouldNotBeNull();
        cut.Find(".mentions-input").ShouldNotBeNull();
    }

    [Fact]
    public void Mentions_Renders_InputWithDefaultAccessibility()
    {
        var cut = Render<Mentions>();

        var input = cut.Find(".mentions-input");
        input.GetAttribute("placeholder").ShouldBe("Type @ to mention...");
        input.GetAttribute("aria-label").ShouldBe("Mention input");
        input.GetAttribute("role").ShouldBe("combobox");
        input.GetAttribute("aria-autocomplete").ShouldBe("list");
        input.GetAttribute("aria-haspopup").ShouldBe("listbox");
        input.GetAttribute("aria-expanded").ShouldBe("false");
        input.GetAttribute("aria-controls").ShouldNotBeNullOrEmpty();
        input.GetAttribute("autocomplete").ShouldBe("off");
    }

    [Fact]
    public void Mentions_Accepts_CustomPlaceholder()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(p => p.Placeholder, "Custom placeholder")
        );

        cut.Find(".mentions-input").GetAttribute("placeholder").ShouldBe("Custom placeholder");
    }

    [Fact]
    public void Mentions_Accepts_CustomMentionPrefix()
    {
        var cut = Render<Mentions>(parameters => parameters.Add(p => p.MentionPrefix, "+"));

        cut.Instance.MentionPrefix.ShouldBe("+");
    }

    [Fact]
    public void Mentions_Allows_Hashtags_ByDefault()
    {
        var cut = Render<Mentions>();

        cut.Instance.AllowHashtags.ShouldBeTrue();
    }

    [Fact]
    public void Mentions_Has_DefaultMaxSuggestions()
    {
        var cut = Render<Mentions>();

        cut.Instance.MaxSuggestions.ShouldBe(5);
    }

    [Fact]
    public void Mentions_Accepts_CustomAriaLabel()
    {
        var cut = Render<Mentions>(parameters => parameters.Add(p => p.AriaLabel, "Mention users"));

        cut.Find(".mentions-input").GetAttribute("aria-label").ShouldBe("Mention users");
    }

    [Fact]
    public void Mentions_Applies_CustomCssClass()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(p => p.CssClass, "custom-mentions").Add(p => p.Class, "base-class")
        );

        cut.Find(".vibe-mentions").ClassList.ShouldContain("custom-mentions");
        cut.Find(".vibe-mentions").ClassList.ShouldContain("base-class");
    }

    [Fact]
    public void Mentions_ToleratesNullItemsAndSuggestions()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(p => p.Items, null!).Add(p => p.Suggestions, null!)
        );

        cut.FindAll(".mention-tag").ShouldBeEmpty();
        cut.Find(".mentions-input").Input("@");
        cut.FindAll(".mentions-suggestions").ShouldBeEmpty();
    }

    [Fact]
    public void Mentions_Renders_InitialItems()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(
                p => p.Items,
                new List<Mentions.MentionItem>
                {
                    new()
                    {
                        Id = "user-1",
                        Name = "Alice",
                        Type = Mentions.MentionType.User,
                    },
                    new()
                    {
                        Id = "tag-1",
                        Name = "dotnet",
                        Type = Mentions.MentionType.Hashtag,
                    },
                }
            )
        );

        var tags = cut.FindAll(".mention-tag");
        tags.Count.ShouldBe(2);
        tags[0].TextContent.ShouldContain("@Alice");
        tags[1].TextContent.ShouldContain("#dotnet");
    }

    [Fact]
    public void Mentions_RendersSharedIconForRemoveAction()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(
                p => p.Items,
                new List<Mentions.MentionItem>
                {
                    new() { Id = "alice", Name = "Alice" },
                }
            )
        );

        var removeButton = cut.Find(".mention-remove");
        removeButton.GetAttribute("type").ShouldBe("button");
        removeButton.GetAttribute("aria-label").ShouldBe("Remove Alice");
        removeButton.QuerySelector("svg.vibe-icon").ShouldNotBeNull();
        removeButton.TextContent.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void Mentions_UpdatesRenderedItems_WhenItemsParameterReferenceChanges()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(
                p => p.Items,
                new List<Mentions.MentionItem>
                {
                    new() { Id = "alice", Name = "Alice" },
                }
            )
        );

        cut.Find(".mention-tag").TextContent.ShouldContain("@Alice");

        cut.Render(parameters =>
            parameters.Add(
                p => p.Items,
                new List<Mentions.MentionItem>
                {
                    new() { Id = "bob", Name = "Bob" },
                }
            )
        );

        var tag = cut.Find(".mention-tag");
        tag.TextContent.ShouldContain("@Bob");
        tag.TextContent.ShouldNotContain("Alice");
    }

    [Fact]
    public void Mentions_FiltersSuggestionsAndRendersListboxSemantics()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(p => p.Suggestions, CreateSuggestions()).Add(p => p.MaxSuggestions, 2)
        );

        var input = cut.Find(".mentions-input");
        input.Input("@ali");

        var list = cut.Find(".mentions-suggestions");
        list.GetAttribute("role").ShouldBe("listbox");
        list.GetAttribute("id").ShouldBe(input.GetAttribute("aria-controls"));
        list.GetAttribute("aria-label").ShouldBe("Mention input suggestions");
        input.GetAttribute("aria-expanded").ShouldBe("true");

        var options = cut.FindAll(".mention-suggestion");
        options.Count.ShouldBe(2);
        options[0].GetAttribute("role").ShouldBe("option");
        options[0].GetAttribute("aria-selected").ShouldBe("true");
        options[0].GetAttribute("tabindex").ShouldBe("-1");
        options[0].TextContent.ShouldContain("Alice");
        options[1].TextContent.ShouldContain("Alicia");
        input.GetAttribute("aria-activedescendant").ShouldBe(options[0].GetAttribute("id"));
    }

    [Fact]
    public void Mentions_HidesSuggestions_WhenInputDoesNotStartWithPrefix()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(p => p.Suggestions, CreateSuggestions())
        );

        var input = cut.Find(".mentions-input");
        input.Input("ali");

        cut.FindAll(".mentions-suggestions").ShouldBeEmpty();
        input.GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public void Mentions_DoesNotShowHashtagSuggestions_WhenHashtagsAreDisabled()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(p => p.AllowHashtags, false).Add(p => p.Suggestions, CreateSuggestions())
        );

        cut.Find(".mentions-input").Input("#ali");

        cut.FindAll(".mentions-suggestions").ShouldBeEmpty();
    }

    [Fact]
    public void Mentions_EscapeClosesSuggestions()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(p => p.Suggestions, CreateSuggestions())
        );

        var input = cut.Find(".mentions-input");
        input.Input("@");
        cut.FindAll(".mention-suggestion").ShouldNotBeEmpty();

        input.KeyDown("Escape");

        cut.FindAll(".mentions-suggestions").ShouldBeEmpty();
        input.GetAttribute("aria-expanded").ShouldBe("false");
        input.GetAttribute("aria-activedescendant").ShouldBeNull();
    }

    [Fact]
    public void Mentions_BlurClosesSuggestions()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(p => p.Suggestions, CreateSuggestions())
        );

        var input = cut.Find(".mentions-input");
        input.Input("@");
        input.GetAttribute("aria-expanded").ShouldBe("true");

        input.Blur();

        cut.FindAll(".mentions-suggestions").ShouldBeEmpty();
        input = cut.Find(".mentions-input");
        input.GetAttribute("aria-expanded").ShouldBe("false");
        input.GetAttribute("aria-activedescendant").ShouldBeNull();
    }

    [Fact]
    public void Mentions_UsesCspSafeKeyGuardDrivenByAriaExpanded()
    {
        var cut = Render<Mentions>(parameters =>
            parameters.Add(p => p.Suggestions, CreateSuggestions())
        );

        var input = cut.Find(".mentions-input");

        // The navigation-key guard moved from an inline onkeydown attribute to
        // vibe-dom.js initializeExpandedKeyGuard, so it survives a strict CSP.
        input.HasAttribute("onkeydown").ShouldBeFalse();
        input.GetAttribute("role").ShouldBe("combobox");

        // The guard cancels defaults only while the popup is open; it reads
        // aria-expanded, which must track the closed/open state.
        input.GetAttribute("aria-expanded").ShouldBe("false");

        input.Input("@");
        cut.Find(".mentions-input").GetAttribute("aria-expanded").ShouldBe("true");
    }

    [Fact]
    public void Mentions_KeyboardNavigation_SelectsActiveSuggestion()
    {
        List<Mentions.MentionItem>? changedItems = null;
        Mentions.MentionItem? addedMention = null;
        var cut = Render<Mentions>(parameters =>
            parameters
                .Add(p => p.Suggestions, CreateSuggestions())
                .Add(p => p.ItemsChanged, items => changedItems = items)
                .Add(p => p.OnMentionAdded, item => addedMention = item)
        );

        var input = cut.Find(".mentions-input");
        input.Input("@");
        input.KeyDown("ArrowDown");

        var options = cut.FindAll(".mention-suggestion");
        options[1].ClassList.ShouldContain("active");
        options[1].GetAttribute("aria-selected").ShouldBe("true");
        input.GetAttribute("aria-activedescendant").ShouldBe(options[1].GetAttribute("id"));

        input.KeyDown("Enter");

        addedMention.ShouldNotBeNull();
        addedMention!.Id.ShouldBe("bob");
        addedMention.Name.ShouldBe("Bob");
        addedMention.Type.ShouldBe(Mentions.MentionType.User);
        changedItems.ShouldNotBeNull();
        changedItems!.Count.ShouldBe(1);
        changedItems[0].Id.ShouldBe("bob");
        cut.Find(".mentions-input").GetAttribute("value").ShouldBe(string.Empty);
        cut.FindAll(".mentions-suggestions").ShouldBeEmpty();
        cut.Find(".mention-tag").TextContent.ShouldContain("@Bob");
    }

    [Fact]
    public void Mentions_SelectsHashtagSuggestion_WithHashtagType()
    {
        Mentions.MentionItem? addedMention = null;
        var cut = Render<Mentions>(parameters =>
            parameters
                .Add(p => p.Suggestions, CreateSuggestions())
                .Add(p => p.OnMentionAdded, item => addedMention = item)
        );

        cut.Find(".mentions-input").Input("#ali");
        cut.Find(".mention-suggestion").Click();

        addedMention.ShouldNotBeNull();
        addedMention!.Type.ShouldBe(Mentions.MentionType.Hashtag);
        cut.Find(".mention-tag").TextContent.ShouldContain("#Alice");
    }

    [Fact]
    public void Mentions_SelectingSuggestion_DoesNotMutateCallerItemsList()
    {
        var callerItems = new List<Mentions.MentionItem>();
        List<Mentions.MentionItem>? changedItems = null;
        var cut = Render<Mentions>(parameters =>
            parameters
                .Add(p => p.Items, callerItems)
                .Add(p => p.Suggestions, CreateSuggestions())
                .Add(p => p.ItemsChanged, items => changedItems = items)
        );

        cut.Find(".mentions-input").Input("@ali");
        cut.Find(".mention-suggestion").Click();

        callerItems.ShouldBeEmpty();
        changedItems.ShouldNotBeNull();
        changedItems!.Count.ShouldBe(1);
        ReferenceEquals(changedItems, callerItems).ShouldBeFalse();
        cut.FindAll(".mention-tag").Count.ShouldBe(1);
    }

    [Fact]
    public void Mentions_RemovesDuplicateDisplayNameByRenderedItem()
    {
        var first = new Mentions.MentionItem { Id = "first", Name = "Alex" };
        var second = new Mentions.MentionItem { Id = "second", Name = "Alex" };
        var callerItems = new List<Mentions.MentionItem> { first, second };
        List<Mentions.MentionItem>? changedItems = null;
        Mentions.MentionItem? removedMention = null;
        var cut = Render<Mentions>(parameters =>
            parameters
                .Add(p => p.Items, callerItems)
                .Add(p => p.ItemsChanged, items => changedItems = items)
                .Add(p => p.OnMentionRemoved, item => removedMention = item)
        );

        cut.FindAll(".mention-remove")[1].Click();

        removedMention.ShouldNotBeNull();
        removedMention!.Id.ShouldBe("second");
        changedItems.ShouldNotBeNull();
        changedItems!.Count.ShouldBe(1);
        changedItems[0].Id.ShouldBe("first");
        callerItems.Count.ShouldBe(2);
        cut.FindAll(".mention-tag").Count.ShouldBe(1);
    }

    [Fact]
    public void Mentions_BackspaceRemovesLastMention_WhenInputIsEmpty()
    {
        Mentions.MentionItem? removedMention = null;
        List<Mentions.MentionItem>? changedItems = null;
        var cut = Render<Mentions>(parameters =>
            parameters
                .Add(
                    p => p.Items,
                    new List<Mentions.MentionItem>
                    {
                        new() { Id = "first", Name = "Alice" },
                        new() { Id = "second", Name = "Bob" },
                    }
                )
                .Add(p => p.ItemsChanged, items => changedItems = items)
                .Add(p => p.OnMentionRemoved, item => removedMention = item)
        );

        cut.Find(".mentions-input").KeyDown("Backspace");

        removedMention.ShouldNotBeNull();
        removedMention!.Id.ShouldBe("second");
        changedItems.ShouldNotBeNull();
        changedItems!.Single().Id.ShouldBe("first");
    }

    [Fact]
    public void Mentions_DisabledPreventsInputSuggestionsAndRemoval()
    {
        var itemsChanged = false;
        Mentions.MentionItem? removedMention = null;
        var cut = Render<Mentions>(parameters =>
            parameters
                .Add(p => p.Disabled, true)
                .Add(
                    p => p.Items,
                    new List<Mentions.MentionItem>
                    {
                        new() { Id = "alice", Name = "Alice" },
                    }
                )
                .Add(p => p.Suggestions, CreateSuggestions())
                .Add(p => p.ItemsChanged, _ => itemsChanged = true)
                .Add(p => p.OnMentionRemoved, item => removedMention = item)
        );

        var root = cut.Find(".vibe-mentions");
        var input = cut.Find(".mentions-input");
        root.ClassList.ShouldContain("mentions-disabled");
        root.GetAttribute("data-state").ShouldBe("disabled");
        root.GetAttribute("aria-disabled").ShouldBe("true");
        input.HasAttribute("disabled").ShouldBeTrue();
        input.GetAttribute("aria-disabled").ShouldBe("true");
        cut.FindAll(".mention-remove").ShouldBeEmpty();

        input.Input("@");
        input.KeyDown("Backspace");

        cut.FindAll(".mentions-suggestions").ShouldBeEmpty();
        cut.FindAll(".mention-tag").Count.ShouldBe(1);
        itemsChanged.ShouldBeFalse();
        removedMention.ShouldBeNull();
    }

    [Fact]
    public void Mentions_ReadOnlyPreventsInputSuggestionsAndRemoval()
    {
        var itemsChanged = false;
        Mentions.MentionItem? removedMention = null;
        var cut = Render<Mentions>(parameters =>
            parameters
                .Add(p => p.ReadOnly, true)
                .Add(
                    p => p.Items,
                    new List<Mentions.MentionItem>
                    {
                        new() { Id = "alice", Name = "Alice" },
                    }
                )
                .Add(p => p.Suggestions, CreateSuggestions())
                .Add(p => p.ItemsChanged, _ => itemsChanged = true)
                .Add(p => p.OnMentionRemoved, item => removedMention = item)
        );

        var root = cut.Find(".vibe-mentions");
        var input = cut.Find(".mentions-input");
        root.ClassList.ShouldContain("mentions-readonly");
        root.GetAttribute("data-state").ShouldBe("readonly");
        root.GetAttribute("aria-disabled").ShouldBeNull();
        input.HasAttribute("readonly").ShouldBeTrue();
        input.GetAttribute("aria-readonly").ShouldBe("true");
        cut.FindAll(".mention-remove").ShouldBeEmpty();

        input.Input("@");
        input.KeyDown("Backspace");

        cut.FindAll(".mentions-suggestions").ShouldBeEmpty();
        cut.FindAll(".mention-tag").Count.ShouldBe(1);
        itemsChanged.ShouldBeFalse();
        removedMention.ShouldBeNull();
    }

    private static List<Mentions.MentionSuggestion> CreateSuggestions() =>
        [
            new()
            {
                Id = "alice",
                Name = "Alice",
                Description = "Product",
            },
            new()
            {
                Id = "bob",
                Name = "Bob",
                Description = "Engineering",
            },
            new()
            {
                Id = "alicia",
                Name = "Alicia",
                Description = "Design",
            },
        ];
}
