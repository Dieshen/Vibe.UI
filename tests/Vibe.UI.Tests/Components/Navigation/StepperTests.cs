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
        nav.GetAttribute("aria-orientation").ShouldBe("horizontal");
        cut.Find("ol.vibe-stepper-list").GetAttribute("role").ShouldBe("list");
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
        items[1].GetAttribute("aria-current").ShouldBe("step");
        items[1].GetAttribute("aria-posinset").ShouldBe("2");
        items[1].GetAttribute("aria-setsize").ShouldBe("3");
        items[2].ClassList.ShouldNotContain("vibe-stepper-item-completed");
        items[2].ClassList.ShouldNotContain("vibe-stepper-item-active");

        var indicators = cut.FindAll(".vibe-stepper-indicator");
        indicators[0].GetAttribute("aria-label").ShouldBe("Plan, step 1 of 3, completed");
        indicators[1].GetAttribute("aria-label").ShouldBe("Build, step 2 of 3, current");
        indicators[2].GetAttribute("aria-label").ShouldBe("Ship, step 3 of 3, pending");
        indicators[0].QuerySelector("span")!.GetAttribute("aria-hidden").ShouldBe("true");
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
        items[0].GetAttribute("aria-invalid").ShouldBe("true");
        cut.FindAll(".vibe-stepper-indicator")[0].GetAttribute("aria-label").ShouldBe("Failed, step 1 of 2, error");
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
    public void Stepper_ClickableStepInvokesCallbacksWithKeyboard()
    {
        int? clicked = null;
        int? changed = null;
        var cut = Render<Stepper>(parameters => parameters
            .Add(p => p.Steps, CreateSteps())
            .Add(p => p.Clickable, true)
            .Add(p => p.OnStepClick, EventCallback.Factory.Create<int>(this, value => clicked = value))
            .Add(p => p.CurrentStepChanged, EventCallback.Factory.Create<int>(this, value => changed = value)));

        cut.FindAll(".vibe-stepper-indicator")[2].KeyDown("Enter");

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
        cut.FindAll(".vibe-stepper-indicator")[1].GetAttribute("aria-disabled").ShouldBe("true");
    }

    [Fact]
    public void Stepper_DisabledStepDoesNotInvokeCallbacks_WhenClickable()
    {
        int? clicked = null;
        var cut = Render<Stepper>(parameters => parameters
            .Add(p => p.Steps, new List<Stepper.StepItem>
            {
                new() { Label = "Plan" },
                new() { Label = "Blocked", Disabled = true },
                new() { Label = "Ship" }
            })
            .Add(p => p.Clickable, true)
            .Add(p => p.OnStepClick, EventCallback.Factory.Create<int>(this, value => clicked = value)));

        cut.FindAll(".vibe-stepper-indicator")[1].Click();
        cut.FindAll(".vibe-stepper-indicator")[1].KeyDown("Enter");

        clicked.ShouldBeNull();
        cut.FindAll(".vibe-stepper-item")[1].ClassList.ShouldContain("vibe-stepper-item-disabled");
        cut.FindAll(".vibe-stepper-item")[1].GetAttribute("aria-disabled").ShouldBe("true");
        cut.FindAll(".vibe-stepper-indicator")[1].HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Stepper_AppliesVerticalOrientationAndAttributes()
    {
        var cut = Render<Stepper>(parameters => parameters
            .Add(p => p.Steps, CreateSteps())
            .Add(p => p.Orientation, Stepper.StepperOrientation.Vertical)
            .Add(p => p.AriaLabel, "Setup progress")
            .Add(p => p.Class, "setup-steps")
            .AddUnmatched("data-stepper", "setup"));

        var nav = cut.Find(".vibe-stepper");
        nav.ClassList.ShouldContain("vibe-stepper-vertical");
        nav.ClassList.ShouldContain("setup-steps");
        nav.GetAttribute("aria-label").ShouldBe("Setup progress");
        nav.GetAttribute("aria-orientation").ShouldBe("vertical");
        nav.GetAttribute("data-stepper").ShouldBe("setup");
        cut.FindAll(".vibe-stepper-connector").Count.ShouldBe(2);
        cut.FindAll(".vibe-stepper-connector").ShouldAllBe(connector => connector.GetAttribute("aria-hidden") == "true");
    }

    [Fact]
    public void Stepper_ConnectsIndicatorToDescription_WhenDescriptionExists()
    {
        var cut = Render<Stepper>(parameters => parameters
            .Add(p => p.Steps, CreateSteps()));

        var buildIndicator = cut.FindAll(".vibe-stepper-indicator")[1];
        var descriptionId = buildIndicator.GetAttribute("aria-describedby");

        descriptionId.ShouldNotBeNullOrWhiteSpace();
        cut.Find($"#{descriptionId}").TextContent.ShouldBe("Build phase");
        cut.FindAll(".vibe-stepper-indicator")[0].GetAttribute("aria-describedby").ShouldBeNull();
    }

    private static List<Stepper.StepItem> CreateSteps() =>
    [
        new() { Label = "Plan" },
        new() { Label = "Build", Description = "Build phase" },
        new() { Label = "Ship" }
    ];
}
