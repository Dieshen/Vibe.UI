using Vibe.UI.Services.Toast;

namespace Vibe.UI.Tests.Components.Feedback;

public class ToastContainerTests : TestBase
{
    [Fact]
    public void ToastContainer_RendersBaseClassAndPosition()
    {
        var cut = Render<ToastContainer>();

        var container = cut.Find(".vibe-toast-container");
        container.ClassList.ShouldContain("bottom-right");
    }

    [Fact]
    public void ToastContainer_AppliesCustomPosition()
    {
        var cut = Render<ToastContainer>(parameters => parameters
            .Add(p => p.Position, "top-left"));

        cut.Find(".vibe-toast-container").ClassList.ShouldContain("top-left");
    }

    [Fact]
    public void ToastContainer_PreservesAdditionalAttributes()
    {
        var cut = Render<ToastContainer>(parameters => parameters
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "toast-container",
                ["aria-live"] = "polite"
            }));

        var container = cut.Find(".vibe-toast-container");
        container.GetAttribute("data-testid").ShouldBe("toast-container");
        container.GetAttribute("aria-live").ShouldBe("polite");
    }

    [Fact]
    public void ToastContainer_RendersToast_WhenServiceRaisesAdded()
    {
        var service = new FakeToastService();
        Services.AddSingleton<IToastService>(service);
        var cut = Render<ToastContainer>();

        service.RaiseAdded(new ToastEventArgs
        {
            Id = "toast-1",
            Title = "Saved",
            Description = "Changes saved",
            Variant = "success",
            Icon = "!",
            Duration = 5000
        });

        cut.WaitForAssertion(() =>
        {
            var toast = cut.Find(".vibe-toast");
            toast.ClassList.ShouldContain("toast-success");
            cut.Find(".toast-title").TextContent.ShouldBe("Saved");
            cut.Find(".toast-description").TextContent.ShouldBe("Changes saved");
            cut.Find(".toast-icon").TextContent.ShouldBe("!");
        });
    }

    [Fact]
    public void ToastContainer_EnforcesMaxToasts()
    {
        var service = new FakeToastService();
        Services.AddSingleton<IToastService>(service);
        var cut = Render<ToastContainer>(parameters => parameters
            .Add(p => p.MaxToasts, 2));

        service.RaiseAdded(CreateToast("toast-1", "One"));
        service.RaiseAdded(CreateToast("toast-2", "Two"));
        service.RaiseAdded(CreateToast("toast-3", "Three"));

        cut.WaitForAssertion(() =>
        {
            var titles = cut.FindAll(".toast-title").Select(title => title.TextContent).ToArray();
            titles.ShouldBe(["Two", "Three"]);
        });
    }

    [Fact]
    public void ToastContainer_RemovesToast_WhenServiceRaisesRemoved()
    {
        var service = new FakeToastService();
        Services.AddSingleton<IToastService>(service);
        var cut = Render<ToastContainer>();

        service.RaiseAdded(CreateToast("toast-1", "Saved"));
        cut.WaitForAssertion(() => cut.FindAll(".vibe-toast").Count.ShouldBe(1));

        service.RaiseRemoved(CreateToast("toast-1", "Saved"));

        cut.WaitForAssertion(() => cut.FindAll(".vibe-toast").ShouldBeEmpty());
    }

    [Fact]
    public void ToastContainer_CloseButtonRemovesToast()
    {
        var service = new FakeToastService();
        Services.AddSingleton<IToastService>(service);
        var cut = Render<ToastContainer>();

        service.RaiseAdded(CreateToast("toast-1", "Saved"));
        cut.WaitForAssertion(() => cut.Find(".toast-close").ShouldNotBeNull());

        cut.Find(".toast-close").Click();

        cut.WaitForAssertion(() => cut.FindAll(".vibe-toast").ShouldBeEmpty());
    }

    [Fact]
    public void ToastContainer_UnsubscribesOnDispose()
    {
        var service = new FakeToastService();
        Services.AddSingleton<IToastService>(service);
        var cut = Render<ToastContainer>();

        service.AddedSubscribers.ShouldBe(1);
        service.RemovedSubscribers.ShouldBe(1);

        cut.Instance.Dispose();

        service.AddedSubscribers.ShouldBe(0);
        service.RemovedSubscribers.ShouldBe(0);
    }

    [Fact]
    public void ToastContainer_AppliesCustomClass()
    {
        var cut = Render<ToastContainer>(parameters => parameters
            .Add(p => p.Class, "toast-stack"));

        cut.Find(".vibe-toast-container").ClassList.ShouldContain("toast-stack");
    }

    private static ToastEventArgs CreateToast(string id, string title) => new()
    {
        Id = id,
        Title = title,
        Description = $"{title} description",
        Variant = "default",
        Duration = 5000
    };

    private sealed class FakeToastService : IToastService
    {
        private EventHandler<ToastEventArgs>? _onToastAdded;
        private EventHandler<ToastEventArgs>? _onToastRemoved;

        public int AddedSubscribers { get; private set; }
        public int RemovedSubscribers { get; private set; }

        public event EventHandler<ToastEventArgs> OnToastAdded
        {
            add
            {
                _onToastAdded += value;
                AddedSubscribers++;
            }
            remove
            {
                _onToastAdded -= value;
                AddedSubscribers = Math.Max(0, AddedSubscribers - 1);
            }
        }

        public event EventHandler<ToastEventArgs> OnToastRemoved
        {
            add
            {
                _onToastRemoved += value;
                RemovedSubscribers++;
            }
            remove
            {
                _onToastRemoved -= value;
                RemovedSubscribers = Math.Max(0, RemovedSubscribers - 1);
            }
        }

        public Task ShowAsync(string title, string? message = null, int duration = 5000) => Task.CompletedTask;

        public Task ShowSuccessAsync(string title, string? message = null, int duration = 5000) => Task.CompletedTask;

        public Task ShowErrorAsync(string title, string? message = null, int duration = 5000) => Task.CompletedTask;

        public Task ShowWarningAsync(string title, string? message = null, int duration = 5000) => Task.CompletedTask;

        public Task ShowInfoAsync(string title, string? message = null, int duration = 5000) => Task.CompletedTask;

        public Task ShowCustomAsync(string title, string? message, string variant, string? icon = null, int duration = 5000) =>
            Task.CompletedTask;

        public void RaiseAdded(ToastEventArgs args)
        {
            _onToastAdded?.Invoke(this, args);
        }

        public void RaiseRemoved(ToastEventArgs args)
        {
            _onToastRemoved?.Invoke(this, args);
        }
    }
}
