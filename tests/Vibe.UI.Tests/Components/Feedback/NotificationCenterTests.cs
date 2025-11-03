namespace Vibe.UI.Tests.Components.Feedback;

public class NotificationCenterTests : TestBase
{
    [Fact]
    public void NotificationCenter_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<NotificationCenter>();

        // Assert
        var notificationCenter = cut.Find(".vibe-notification-center");
        notificationCenter.ShouldNotBeNull();
    }

    [Fact]
    public void NotificationCenter_Displays_TriggerButton()
    {
        // Act
        var cut = RenderComponent<NotificationCenter>();

        // Assert
        var trigger = cut.Find(".notification-trigger");
        trigger.ShouldNotBeNull();
        trigger.GetAttribute("aria-label").ShouldBe("Notifications");
    }

    [Fact]
    public void NotificationCenter_Shows_UnreadBadge_WhenHasUnread()
    {
        // Arrange
        var notifications = new List<NotificationCenter.NotificationItem>
        {
            new() { Title = "Test", IsRead = false }
        };

        // Act
        var cut = RenderComponent<NotificationCenter>(parameters => parameters
            .Add(p => p.Notifications, notifications));

        // Assert
        var badge = cut.Find(".notification-badge");
        badge.ShouldNotBeNull();
        badge.TextContent.ShouldBe("1");
    }

    [Fact]
    public void NotificationCenter_Hides_UnreadBadge_WhenAllRead()
    {
        // Arrange
        var notifications = new List<NotificationCenter.NotificationItem>
        {
            new() { Title = "Test", IsRead = true }
        };

        // Act
        var cut = RenderComponent<NotificationCenter>(parameters => parameters
            .Add(p => p.Notifications, notifications));

        // Assert
        cut.FindAll(".notification-badge").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_OpensPanel_WhenTriggerClicked()
    {
        // Act
        var cut = RenderComponent<NotificationCenter>();
        var trigger = cut.Find(".notification-trigger");
        trigger.Click();

        // Assert
        var panel = cut.Find(".notification-panel");
        panel.ShouldNotBeNull();
    }

    [Fact]
    public void NotificationCenter_Displays_Notifications()
    {
        // Arrange
        var notifications = new List<NotificationCenter.NotificationItem>
        {
            new() { Title = "Notification 1", Message = "Test 1" },
            new() { Title = "Notification 2", Message = "Test 2" }
        };

        // Act
        var cut = RenderComponent<NotificationCenter>(parameters => parameters
            .Add(p => p.Notifications, notifications));
        var trigger = cut.Find(".notification-trigger");
        trigger.Click();

        // Assert
        var items = cut.FindAll(".notification-item");
        items.Count.ShouldBe(2);
    }

    [Fact]
    public void NotificationCenter_Shows_EmptyState_WhenNoNotifications()
    {
        // Act
        var cut = RenderComponent<NotificationCenter>();
        var trigger = cut.Find(".notification-trigger");
        trigger.Click();

        // Assert
        var empty = cut.Find(".notification-empty");
        empty.ShouldNotBeNull();
    }

    [Fact]
    public void NotificationCenter_Displays_Filters_WhenEnabled()
    {
        // Act
        var cut = RenderComponent<NotificationCenter>(parameters => parameters
            .Add(p => p.ShowFilters, true));
        var trigger = cut.Find(".notification-trigger");
        trigger.Click();

        // Assert
        var filters = cut.Find(".panel-filters");
        filters.ShouldNotBeNull();
    }

    [Fact]
    public void NotificationCenter_Hides_Filters_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<NotificationCenter>(parameters => parameters
            .Add(p => p.ShowFilters, false));
        var trigger = cut.Find(".notification-trigger");
        trigger.Click();

        // Assert
        cut.FindAll(".panel-filters").ShouldBeEmpty();
    }

    [Fact]
    public void NotificationCenter_Shows_ClearAllButton_WhenAllowed()
    {
        // Arrange
        var notifications = new List<NotificationCenter.NotificationItem>
        {
            new() { Title = "Test" }
        };

        // Act
        var cut = RenderComponent<NotificationCenter>(parameters => parameters
            .Add(p => p.Notifications, notifications)
            .Add(p => p.AllowClearAll, true));
        var trigger = cut.Find(".notification-trigger");
        trigger.Click();

        // Assert
        var clearButton = cut.Find(".clear-all-btn");
        clearButton.ShouldNotBeNull();
    }
}
