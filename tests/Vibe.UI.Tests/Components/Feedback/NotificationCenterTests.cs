namespace Vibe.UI.Tests.Components.Feedback;

public class NotificationCenterTests : TestBase
{
    [Fact]
    public void NotificationCenter_RendersTriggerWithAccessibleState()
    {
        var cut = Render<NotificationCenter>();

        var root = cut.Find(".vibe-notification-center");
        root.ShouldNotBeNull();

        var trigger = cut.Find(".notification-trigger");
        trigger.GetAttribute("type").ShouldBe("button");
        trigger.GetAttribute("aria-label").ShouldBe("Notifications");
        trigger.GetAttribute("aria-haspopup").ShouldBe("dialog");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.GetAttribute("aria-controls").ShouldNotBeNullOrWhiteSpace();
        trigger.QuerySelector("svg").ShouldNotBeNull();

        var liveRegion = cut.Find(".notification-live-region");
        liveRegion.GetAttribute("role").ShouldBe("status");
        liveRegion.GetAttribute("aria-live").ShouldBe("polite");
        liveRegion.TextContent.ShouldBe("No unread notifications");
    }

    [Fact]
    public void NotificationCenter_UsesCustomAriaLabelAndFallsBackWhenBlank()
    {
        var cut = Render<NotificationCenter>(parameters => parameters
            .Add(p => p.AriaLabel, "Inbox alerts"));

        cut.Find(".notification-trigger").GetAttribute("aria-label").ShouldBe("Inbox alerts");

        cut.Render(parameters => parameters
            .Add(p => p.AriaLabel, " "));

        cut.Find(".notification-trigger").GetAttribute("aria-label").ShouldBe("Notifications");
    }

    [Fact]
    public void NotificationCenter_OpensPanelWithDialogSemantics_WhenTriggerClicked()
    {
        var cut = Render<NotificationCenter>();

        cut.Find(".notification-trigger").Click();

        var trigger = cut.Find(".notification-trigger");
        var panel = cut.Find(".notification-panel");
        var title = cut.Find(".panel-title");

        trigger.GetAttribute("aria-expanded").ShouldBe("true");
        panel.GetAttribute("role").ShouldBe("dialog");
        panel.GetAttribute("aria-modal").ShouldBe("false");
        panel.GetAttribute("tabindex").ShouldBe("-1");
        panel.GetAttribute("id").ShouldBe(trigger.GetAttribute("aria-controls"));
        panel.GetAttribute("aria-labelledby").ShouldBe(title.GetAttribute("id"));
        cut.Find(".vibe-notification-center").ClassList.ShouldContain("open");
    }

    [Fact]
    public void NotificationCenter_EscapeClosesOpenPanel()
    {
        var cut = Render<NotificationCenter>();
        cut.Find(".notification-trigger").Click();

        cut.Find(".notification-panel").KeyDown("Escape");

        cut.FindAll(".notification-panel").ShouldBeEmpty();
        cut.Find(".notification-trigger").GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public void NotificationCenter_EscapeFromTriggerClosesOpenPanel()
    {
        var cut = Render<NotificationCenter>();
        var trigger = cut.Find(".notification-trigger");
        trigger.Click();

        trigger.KeyDown("Escape");

        cut.FindAll(".notification-panel").ShouldBeEmpty();
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public void NotificationCenter_BackdropClosesOpenPanel()
    {
        var cut = Render<NotificationCenter>();
        cut.Find(".notification-trigger").Click();

        cut.Find(".notification-backdrop").Click();

        cut.FindAll(".notification-panel").ShouldBeEmpty();
        cut.FindAll(".notification-backdrop").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_RendersUnreadBadgeAndCapsLargeCounts()
    {
        var cut = Render<NotificationCenter>(parameters => parameters
            .Add(p => p.Notifications, Enumerable.Range(0, 120)
                .Select(index => new NotificationCenter.NotificationItem { Id = $"n{index}", Title = $"Notification {index}" })
                .ToList()));

        cut.Find(".notification-badge").TextContent.ShouldBe("99+");
        cut.Find(".notification-badge").GetAttribute("aria-hidden").ShouldBe("true");
        cut.Find(".notification-live-region").TextContent.ShouldBe("120 unread notifications");
    }

    [Fact]
    public void NotificationCenter_HidesUnreadBadge_WhenAllNotificationsAreRead()
    {
        var cut = Render<NotificationCenter>(parameters => parameters
            .Add(p => p.Notifications, new List<NotificationCenter.NotificationItem>
            {
                new() { Title = "Read", IsRead = true }
            }));

        cut.FindAll(".notification-badge").ShouldBeEmpty();
        cut.Find(".notification-live-region").TextContent.ShouldBe("No unread notifications");
    }

    [Fact]
    public void NotificationCenter_RendersNotificationsSortedByNewestWithSemanticListItems()
    {
        var old = new System.DateTime(2024, 1, 1, 9, 0, 0);
        var newest = new System.DateTime(2024, 1, 2, 9, 0, 0);
        var notifications = new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "old", Title = "Old", Message = "Earlier", Type = NotificationCenter.NotificationType.Info, Timestamp = old, IsRead = true },
            new() { Id = "new", Title = "New", Message = "Later", Type = NotificationCenter.NotificationType.Error, Timestamp = newest, IsRead = false }
        };

        var cut = RenderOpen(notifications);

        var list = cut.Find(".notification-list");
        list.GetAttribute("role").ShouldBe("list");
        list.GetAttribute("aria-label").ShouldBe("Notification list");

        var items = cut.FindAll(".notification-item");
        items.Count.ShouldBe(2);
        items[0].GetAttribute("role").ShouldBe("listitem");
        items[0].ClassList.ShouldContain("unread");
        items[0].ClassList.ShouldContain("type-error");
        items[0].TextContent.ShouldContain("New");
        items[1].ClassList.ShouldContain("read");
        items[1].TextContent.ShouldContain("Old");

        var contentButton = items[0].QuerySelector(".notification-content-button");
        contentButton.ShouldNotBeNull();
        contentButton!.GetAttribute("aria-label").ShouldBe("Error notification, New, unread");

        var time = items[0].QuerySelector(".notification-time");
        time.ShouldNotBeNull();
        time!.GetAttribute("datetime").ShouldBe(newest.ToString("O", System.Globalization.CultureInfo.InvariantCulture));
    }

    [Fact]
    public void NotificationCenter_RendersUntitledFallback_WhenTitleAndMessageAreMissing()
    {
        var cut = RenderOpen(new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "empty", Title = "", Message = null, Timestamp = null }
        });

        cut.Find(".notification-message-empty").TextContent.ShouldBe("Untitled notification");
        cut.Find(".notification-content-button").GetAttribute("aria-label").ShouldBe("Info notification, Untitled notification, unread");
        cut.FindAll(".notification-time").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_ShowsEmptyState_WhenNoNotifications()
    {
        var cut = RenderOpen();

        var empty = cut.Find(".notification-empty");
        empty.GetAttribute("role").ShouldBe("status");
        empty.GetAttribute("aria-live").ShouldBe("polite");
        empty.TextContent.ShouldContain("No notifications");
        cut.FindAll(".notification-item").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_RendersCustomEmptyContent()
    {
        var cut = Render<NotificationCenter>(parameters => parameters
            .Add(p => p.EmptyContent, builder => builder.AddMarkupContent(0, "<strong class='custom-empty'>Nothing pending</strong>")));

        cut.Find(".notification-trigger").Click();

        cut.Find(".custom-empty").TextContent.ShouldBe("Nothing pending");
    }

    [Fact]
    public void NotificationCenter_RendersAndAppliesFilters()
    {
        var cut = RenderOpen(new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "read-info", Title = "Read info", Type = NotificationCenter.NotificationType.Info, IsRead = true },
            new() { Id = "unread-error", Title = "Unread error", Type = NotificationCenter.NotificationType.Error, IsRead = false }
        });

        var filterGroup = cut.Find(".panel-filters");
        filterGroup.GetAttribute("role").ShouldBe("group");
        filterGroup.GetAttribute("aria-label").ShouldBe("Notification filters");

        var allFilter = cut.FindAll(".filter-btn").Single(button => button.TextContent.Trim() == "All");
        allFilter.ClassList.ShouldContain("active");
        allFilter.GetAttribute("aria-pressed").ShouldBe("true");

        var unreadFilter = cut.FindAll(".filter-btn").Single(button => button.TextContent.Trim() == "Unread");
        unreadFilter.Click();

        cut.FindAll(".notification-item").Single().TextContent.ShouldContain("Unread error");
        cut.FindAll(".filter-btn")
            .Single(button => button.TextContent.Trim() == "Unread")
            .GetAttribute("aria-pressed")
            .ShouldBe("true");
    }

    [Fact]
    public void NotificationCenter_FilterMatchingIsCaseInsensitiveAndShowsEmptyFilteredState()
    {
        var cut = Render<NotificationCenter>(parameters => parameters
            .Add(p => p.Filters, new List<string> { " all ", " error ", "ERROR", "" })
            .Add(p => p.Notifications, new List<NotificationCenter.NotificationItem>
            {
                new() { Id = "info", Title = "Info", Type = NotificationCenter.NotificationType.Info }
            }));

        cut.Find(".notification-trigger").Click();

        var filters = cut.FindAll(".filter-btn");
        filters.Count.ShouldBe(2);
        filters[0].TextContent.Trim().ShouldBe("all");
        filters[1].TextContent.Trim().ShouldBe("error");

        filters[1].Click();

        cut.Find(".notification-empty").TextContent.ShouldContain("No matching notifications");
        cut.FindAll(".notification-item").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_HidesFilters_WhenDisabled()
    {
        var cut = Render<NotificationCenter>(parameters => parameters
            .Add(p => p.ShowFilters, false)
            .Add(p => p.Notifications, new List<NotificationCenter.NotificationItem>
            {
                new() { Title = "Test" }
            }));

        cut.Find(".notification-trigger").Click();

        cut.FindAll(".panel-filters").ShouldBeEmpty();
        cut.FindAll(".notification-item").Count.ShouldBe(1);
    }

    [Fact]
    public void NotificationCenter_ClickingUnreadNotificationMarksReadAndInvokesCallbacksWithClonedList()
    {
        var sourceNotification = new NotificationCenter.NotificationItem
        {
            Id = "n1",
            Title = "Deploy complete",
            IsRead = false
        };
        var source = new List<NotificationCenter.NotificationItem> { sourceNotification };
        List<NotificationCenter.NotificationItem>? changed = null;
        NotificationCenter.NotificationItem? clicked = null;

        var cut = RenderOpen(source, parameters => parameters
            .Add(p => p.NotificationsChanged, notifications => changed = notifications)
            .Add(p => p.OnNotificationClicked, notification => clicked = notification));

        cut.Find(".notification-content-button").Click();

        sourceNotification.IsRead.ShouldBeFalse();
        changed.ShouldNotBeNull();
        changed.ShouldNotBeSameAs(source);
        changed.Count.ShouldBe(1);
        changed[0].ShouldNotBeSameAs(sourceNotification);
        changed[0].IsRead.ShouldBeTrue();
        clicked.ShouldNotBeNull();
        clicked.ShouldNotBeSameAs(sourceNotification);
        clicked.IsRead.ShouldBeTrue();
        cut.Find(".notification-item").ClassList.ShouldContain("read");
        cut.FindAll(".notification-badge").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_ClickingReadNotificationInvokesClickOnly()
    {
        var changes = 0;
        var clicks = 0;
        var cut = RenderOpen(new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "n1", Title = "Already read", IsRead = true }
        }, parameters => parameters
            .Add(p => p.NotificationsChanged, _ => changes++)
            .Add(p => p.OnNotificationClicked, _ => clicks++));

        cut.Find(".notification-content-button").Click();

        changes.ShouldBe(0);
        clicks.ShouldBe(1);
    }

    [Fact]
    public void NotificationCenter_MarkAllReadUpdatesOnlyWhenUnreadNotificationsExist()
    {
        var source = new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "one", Title = "One", IsRead = false },
            new() { Id = "two", Title = "Two", IsRead = true }
        };
        List<NotificationCenter.NotificationItem>? changed = null;

        var cut = RenderOpen(source, parameters => parameters
            .Add(p => p.NotificationsChanged, notifications => changed = notifications));

        cut.Find(".panel-action").Click();

        source[0].IsRead.ShouldBeFalse();
        changed.ShouldNotBeNull();
        changed.All(notification => notification.IsRead).ShouldBeTrue();
        cut.FindAll(".panel-action").ShouldBeEmpty();
        cut.FindAll(".notification-badge").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_RemoveNotificationUpdatesListAndStopsItemClickCallback()
    {
        var source = new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "one", Title = "One", Timestamp = new System.DateTime(2024, 1, 2) },
            new() { Id = "two", Title = "Two", Timestamp = new System.DateTime(2024, 1, 1) }
        };
        List<NotificationCenter.NotificationItem>? changed = null;
        var clicks = 0;

        var cut = RenderOpen(source, parameters => parameters
            .Add(p => p.NotificationsChanged, notifications => changed = notifications)
            .Add(p => p.OnNotificationClicked, _ => clicks++));

        var removeButton = cut.FindAll(".notification-remove-btn")[0];
        removeButton.GetAttribute("aria-label").ShouldBe("Remove One");
        removeButton.Click();

        clicks.ShouldBe(0);
        source.Count.ShouldBe(2);
        changed.ShouldNotBeNull();
        changed.Count.ShouldBe(1);
        changed[0].Title.ShouldBe("Two");
        cut.FindAll(".notification-item").Single().TextContent.ShouldContain("Two");
    }

    [Fact]
    public void NotificationCenter_PreservesLocalReadAndRemoveStateAcrossParentRerender()
    {
        var source = new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "one", Title = "One", IsRead = false, Timestamp = new System.DateTime(2024, 1, 2) },
            new() { Id = "two", Title = "Two", IsRead = false, Timestamp = new System.DateTime(2024, 1, 1) }
        };

        var cut = RenderOpen(source);

        cut.FindAll(".notification-content-button")[0].Click();
        cut.FindAll(".notification-remove-btn")[1].Click();

        cut.FindAll(".notification-item").Single().TextContent.ShouldContain("One");
        cut.Find(".notification-item").ClassList.ShouldContain("read");

        cut.Render(parameters => parameters
            .Add(p => p.Notifications, source));

        source.Count.ShouldBe(2);
        source[0].IsRead.ShouldBeFalse();
        cut.FindAll(".notification-item").Single().TextContent.ShouldContain("One");
        cut.Find(".notification-item").ClassList.ShouldContain("read");
        cut.FindAll(".notification-badge").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_ClearAllInvokesChangeAndClosesPanel()
    {
        List<NotificationCenter.NotificationItem>? changed = null;
        var cut = RenderOpen(new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "one", Title = "One" }
        }, parameters => parameters
            .Add(p => p.NotificationsChanged, notifications => changed = notifications));

        cut.Find(".clear-all-btn").Click();

        changed.ShouldNotBeNull();
        changed.ShouldBeEmpty();
        cut.FindAll(".notification-panel").ShouldBeEmpty();
        cut.Find(".notification-trigger").GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public void NotificationCenter_HidesClearAllButton_WhenNotAllowed()
    {
        var cut = RenderOpen(new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "one", Title = "One" }
        }, parameters => parameters
            .Add(p => p.AllowClearAll, false));

        cut.FindAll(".clear-all-btn").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_ViewAllRendersForOverflowAndInvokesCallback()
    {
        var invoked = false;
        var cut = RenderOpen(new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "one", Title = "One" },
            new() { Id = "two", Title = "Two" }
        }, parameters => parameters
            .Add(p => p.MaxDisplayCount, 1)
            .Add(p => p.OnViewAll, () => invoked = true));

        cut.FindAll(".notification-item").Count.ShouldBe(1);

        var viewAll = cut.Find(".view-all-btn");
        viewAll.TextContent.Trim().ShouldBe("View all (2)");
        viewAll.Click();

        invoked.ShouldBeTrue();
    }

    [Fact]
    public void NotificationCenter_ClampsInvalidMaxDisplayCountToOne()
    {
        var cut = RenderOpen(new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "one", Title = "One" },
            new() { Id = "two", Title = "Two" }
        }, parameters => parameters
            .Add(p => p.MaxDisplayCount, 0));

        cut.FindAll(".notification-item").Count.ShouldBe(1);
        cut.Find(".view-all-btn").TextContent.Trim().ShouldBe("View all (2)");
    }

    [Fact]
    public void NotificationCenter_DisabledSuppressesOpeningAndAddsDisabledState()
    {
        var cut = Render<NotificationCenter>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.Notifications, new List<NotificationCenter.NotificationItem>
            {
                new() { Title = "Blocked" }
            }));

        var root = cut.Find(".vibe-notification-center");
        root.ClassList.ShouldContain("vibe-notification-center-disabled");

        var trigger = cut.Find(".notification-trigger");
        trigger.HasAttribute("disabled").ShouldBeTrue();
        trigger.GetAttribute("aria-disabled").ShouldBe("true");
        trigger.Click();

        cut.FindAll(".notification-panel").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_DisablingAfterOpenClosesPanel()
    {
        var cut = RenderOpen();

        cut.Render(parameters => parameters
            .Add(p => p.Disabled, true));

        cut.FindAll(".notification-panel").ShouldBeEmpty();
        cut.Find(".notification-trigger").GetAttribute("aria-expanded").ShouldBe("false");
    }

    [Fact]
    public void NotificationCenter_ReadOnlyAllowsOpeningAndClickCallbackButSuppressesMutations()
    {
        var changes = 0;
        var clicks = 0;
        var source = new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "one", Title = "One", IsRead = false }
        };

        var cut = RenderOpen(source, parameters => parameters
            .Add(p => p.ReadOnly, true)
            .Add(p => p.NotificationsChanged, _ => changes++)
            .Add(p => p.OnNotificationClicked, _ => clicks++));

        cut.Find(".vibe-notification-center").ClassList.ShouldContain("vibe-notification-center-readonly");
        cut.Find(".panel-action").HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".clear-all-btn").HasAttribute("disabled").ShouldBeTrue();
        cut.Find(".notification-remove-btn").HasAttribute("disabled").ShouldBeTrue();

        cut.Find(".notification-content-button").Click();
        cut.Find(".panel-action").Click();
        cut.Find(".clear-all-btn").Click();
        cut.Find(".notification-remove-btn").Click();

        clicks.ShouldBe(1);
        changes.ShouldBe(0);
        source[0].IsRead.ShouldBeFalse();
        cut.Find(".notification-item").ClassList.ShouldContain("unread");
    }

    [Fact]
    public void NotificationCenter_HandlesNullNotificationAndFilterInputs()
    {
        var cut = Render<NotificationCenter>(parameters => parameters
            .Add(p => p.Notifications, null)
            .Add(p => p.Filters, null));

        cut.Find(".notification-trigger").Click();

        cut.Find(".notification-empty").TextContent.ShouldContain("No notifications");
        cut.FindAll(".filter-btn").Single().TextContent.Trim().ShouldBe("All");
    }

    [Fact]
    public void NotificationCenter_NormalizesMissingNotificationIdsWithoutMutatingSource()
    {
        List<NotificationCenter.NotificationItem>? changed = null;
        var source = new List<NotificationCenter.NotificationItem>
        {
            new() { Id = "", Title = "No id" }
        };

        var cut = RenderOpen(source, parameters => parameters
            .Add(p => p.NotificationsChanged, notifications => changed = notifications));

        cut.Find(".notification-content-button").Click();

        source[0].Id.ShouldBe("");
        changed.ShouldNotBeNull();
        changed[0].Id.ShouldBe("notification-0");
        changed[0].IsRead.ShouldBeTrue();
    }

    [Fact]
    public void NotificationCenter_PreservesAdditionalAttributesAndClassParameters()
    {
        var cut = Render<NotificationCenter>(parameters => parameters
            .Add(p => p.CssClass, "legacy-css-class")
            .Add(p => p.Class, "base-class")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "notification-center"
            }));

        var root = cut.Find(".vibe-notification-center");
        root.ClassList.ShouldContain("legacy-css-class");
        root.ClassList.ShouldContain("base-class");
        root.GetAttribute("data-testid").ShouldBe("notification-center");
    }

    private IRenderedComponent<NotificationCenter> RenderOpen(
        List<NotificationCenter.NotificationItem>? notifications = null,
        Action<ComponentParameterCollectionBuilder<NotificationCenter>>? configure = null)
    {
        var cut = Render<NotificationCenter>(parameters =>
        {
            if (notifications != null)
            {
                parameters.Add(p => p.Notifications, notifications);
            }

            configure?.Invoke(parameters);
        });

        cut.Find(".notification-trigger").Click();
        return cut;
    }
}
