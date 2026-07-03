namespace Vibe.UI.Tests.Components.Navigation;

public class StepperTests : TestBase
{
    [Fact]
    public void Stepper_RendersProgressNavigationAndSteps()
    {
        var cut = Render<Stepper>(parameters => parameters
            .Add(p => p.Steps, CreateSteps()));

        var nav = cut.Find("nav.vibe-stepper");
        nav.GetAttribute("aria-label").ShouldBe("Progress");
        cut.FindAll(".vibe-stepper-item").Count.ShouldBe(3);
        cut.Markup.ShouldContain("Plan");
        cut.Markup.ShouldContain("Build phase");
    }

    [Fact]
    public void Stepper_MarksCompletedActiveAndPendingSteps()
    {
        var cut = Render<Stepper>(parameters => parameters
            .Add(p => p.Steps, CreateSteps())
            .Add(p => p.CurrentStep, 1));

        var items = cut.FindAll(".vibe-stepper-item");
        items[0].ClassList.ShouldContain("vibe-stepper-item-completed");
        items[1].ClassList.ShouldContain("vibe-stepper-item-active");
        items[2].ClassList.ShouldNotContain("vibe-stepper-item-completed");
        items[2].ClassList.ShouldNotContain("vibe-stepper-item-active");
    }

    [Fact]
    public void Stepper_UsesExplicitStatusAndIcon()
    {
        var cut = Render<Stepper>(parameters => parameters
            .Add(p => p.Steps, new List<Stepper.StepItem>
            {
                new() { Label = "Failed", Status = Stepper.StepStatus.Error },
                new() { Label = "Deploy", Icon = "D" }
            }));

        var items = cut.FindAll(".vibe-stepper-item");
        items[0].ClassList.ShouldContain("vibe-stepper-item-error");
        cut.FindAll(".vibe-stepper-indicator")[0].TextContent.ShouldContain("!");
        cut.FindAll(".vibe-stepper-indicator")[1].TextContent.ShouldContain("D");
    }

    [Fact]
    public void Stepper_ClickableStepInvokesCallbacks()
    {
        int? clicked = null;
        int? changed = null;
        var cut = Render<Stepper>(parameters => parameters
            .Add(p => p.Steps, CreateSteps())
            .Add(p => p.Clickable, true)
            .Add(p => p.OnStepClick, EventCallback.Factory.Create<int>(this, value => clicked = value))
            .Add(p => p.CurrentStepChanged, EventCallback.Factory.Create<int>(this, value => changed = value)));

        cut.FindAll(".vibe-stepper-indicator")[2].Click();

        clicked.ShouldBe(2);
        changed.ShouldBe(2);
    }

    [Fact]
    public void Stepper_NonClickableStepDoesNotInvokeCallbacks()
    {
        int? clicked = null;
        var cut = Render<Stepper>(parameters => parameters
            .Add(p => p.Steps, CreateSteps())
            .Add(p => p.Clickable, false)
            .Add(p => p.OnStepClick, EventCallback.Factory.Create<int>(this, value => clicked = value)));

        cut.FindAll(".vibe-stepper-indicator")[1].Click();

        clicked.ShouldBeNull();
        cut.FindAll(".vibe-stepper-indicator")[1].HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Stepper_AppliesVerticalOrientationAndAttributes()
    {
        var cut = Render<Stepper>(parameters => parameters
            .Add(p => p.Steps, CreateSteps())
            .Add(p => p.Orientation, Stepper.StepperOrientation.Vertical)
            .Add(p => p.Class, "setup-steps")
            .AddUnmatched("data-stepper", "setup"));

        var nav = cut.Find(".vibe-stepper");
        nav.ClassList.ShouldContain("vibe-stepper-vertical");
        nav.ClassList.ShouldContain("setup-steps");
        nav.GetAttribute("data-stepper").ShouldBe("setup");
        cut.FindAll(".vibe-stepper-connector").Count.ShouldBe(2);
    }

    private static List<Stepper.StepItem> CreateSteps() =>
    [
        new() { Label = "Plan" },
        new() { Label = "Build", Description = "Build phase" },
        new() { Label = "Ship" }
    ];
}
