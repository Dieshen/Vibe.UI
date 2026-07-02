namespace Vibe.UI.Tests.Components.Inputs;

public class InputOTPTests : TestBase
{
    [Fact]
    public void InputOTP_RendersDefaultNumericSlots()
    {
        var cut = RenderComponent<InputOTP>();

        var group = cut.Find(".input-otp");
        group.GetAttribute("role").ShouldBe("group");
        group.GetAttribute("aria-label").ShouldBe("One-time password input");

        var inputs = cut.FindAll("input");
        inputs.Count.ShouldBe(6);
        inputs[0].GetAttribute("type").ShouldBe("tel");
        inputs[0].GetAttribute("inputmode").ShouldBe("numeric");
        inputs[0].GetAttribute("autocomplete").ShouldBe("one-time-code");
        inputs[0].GetAttribute("maxlength").ShouldBe("1");
    }

    [Fact]
    public void InputOTP_RendersExistingValueAcrossSlots()
    {
        var cut = RenderComponent<InputOTP>(parameters => parameters
            .Add(p => p.Value, "123")
            .Add(p => p.Length, 4));

        var inputs = cut.FindAll("input");
        inputs[0].GetAttribute("value").ShouldBe("1");
        inputs[1].GetAttribute("value").ShouldBe("2");
        inputs[2].GetAttribute("value").ShouldBe("3");
        (inputs[3].GetAttribute("value") ?? string.Empty).ShouldBe(string.Empty);
    }

    [Fact]
    public void InputOTP_RendersSeparatorsAtConfiguredIndices()
    {
        var cut = RenderComponent<InputOTP>(parameters => parameters
            .Add(p => p.Length, 4)
            .Add(p => p.Separator, "/")
            .Add(p => p.SeparatorIndices, new List<int> { 0, 2 }));

        var separators = cut.FindAll(".input-otp-separator");
        separators.Count.ShouldBe(2);
        separators[0].TextContent.ShouldBe("/");
        separators[1].TextContent.ShouldBe("/");
    }

    [Fact]
    public void InputOTP_UpdatesSlotCount_WhenLengthChanges()
    {
        var cut = RenderComponent<InputOTP>(parameters => parameters
            .Add(p => p.Length, 2));

        cut.FindAll("input").Count.ShouldBe(2);

        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Length, 4));

        cut.FindAll("input").Count.ShouldBe(4);
    }

    [Fact]
    public void InputOTP_AppliesAlphaPatternAttributes()
    {
        var cut = RenderComponent<InputOTP>(parameters => parameters
            .Add(p => p.Pattern, InputOTP.OTPPattern.Alpha));

        var input = cut.Find("input");
        input.GetAttribute("type").ShouldBe("text");
        input.GetAttribute("inputmode").ShouldBe("text");
    }

    [Fact]
    public void InputOTP_AppliesDisabledState()
    {
        var cut = RenderComponent<InputOTP>(parameters => parameters
            .Add(p => p.Disabled, true));

        cut.Find(".input-otp").ClassList.ShouldContain("input-otp-disabled");
        cut.FindAll("input").ShouldAllBe(input => input.HasAttribute("disabled"));
    }

    [Fact]
    public void InputOTP_InvokesValueChanged_ForValidInput()
    {
        string? changedValue = null;
        var cut = RenderComponent<InputOTP>(parameters => parameters
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.Find("input").Input("7");

        changedValue.ShouldBe("7");
    }

    [Fact]
    public void InputOTP_RejectsInvalidNumericInput()
    {
        string? changedValue = null;
        var cut = RenderComponent<InputOTP>(parameters => parameters
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.Find("input").Input("A");

        changedValue.ShouldBeNull();
    }

    [Fact]
    public void InputOTP_InvokesOnComplete_WhenLengthIsFilled()
    {
        string? completedValue = null;
        var cut = RenderComponent<InputOTP>(parameters => parameters
            .Add(p => p.Length, 2)
            .Add(p => p.Value, "1")
            .Add(p => p.OnComplete, value => completedValue = value));

        cut.FindAll("input")[1].Input("2");

        completedValue.ShouldBe("12");
    }

    [Fact]
    public void InputOTP_BackspaceClearsCurrentSlot()
    {
        string? changedValue = null;
        var cut = RenderComponent<InputOTP>(parameters => parameters
            .Add(p => p.Length, 2)
            .Add(p => p.Value, "12")
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.FindAll("input")[1].KeyDown("Backspace");

        changedValue.ShouldBe("1");
    }
}
