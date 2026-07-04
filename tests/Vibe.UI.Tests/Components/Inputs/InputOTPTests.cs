namespace Vibe.UI.Tests.Components.Inputs;

public class InputOTPTests : TestBase
{
    [Fact]
    public void InputOTP_RendersDefaultNumericSlots()
    {
        var cut = Render<InputOTP>();

        var group = cut.Find(".input-otp");
        group.GetAttribute("role").ShouldBe("group");
        group.GetAttribute("aria-label").ShouldBe("One-time password input");
        group.HasAttribute("aria-disabled").ShouldBeFalse();

        var inputs = cut.FindAll("input");
        inputs.Count.ShouldBe(6);
        inputs[0].ClassList.ShouldContain("input-otp-slot");
        inputs[0].GetAttribute("type").ShouldBe("tel");
        inputs[0].GetAttribute("inputmode").ShouldBe("numeric");
        inputs[0].GetAttribute("autocomplete").ShouldBe("one-time-code");
        inputs[0].GetAttribute("maxlength").ShouldBe("1");
        inputs[0].GetAttribute("aria-label").ShouldBe("Digit 1 of 6");
        inputs[5].GetAttribute("aria-label").ShouldBe("Digit 6 of 6");
    }

    [Fact]
    public void InputOTP_RendersExistingValueAcrossSlots()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Value, "123")
            .Add(p => p.Length, 4));

        var inputs = cut.FindAll("input");
        inputs[0].GetAttribute("value").ShouldBe("1");
        inputs[1].GetAttribute("value").ShouldBe("2");
        inputs[2].GetAttribute("value").ShouldBe("3");
        (inputs[3].GetAttribute("value") ?? string.Empty).ShouldBe(string.Empty);
        inputs[0].ClassList.ShouldContain("input-otp-slot-filled");
        inputs[3].ClassList.ShouldNotContain("input-otp-slot-filled");
    }

    [Fact]
    public void InputOTP_TruncatesValueToConfiguredLength()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Value, "123456")
            .Add(p => p.Length, 4));

        var inputs = cut.FindAll("input");
        inputs.Count.ShouldBe(4);
        inputs.Select(input => input.GetAttribute("value") ?? string.Empty)
            .ShouldBe(["1", "2", "3", "4"]);
    }

    [Fact]
    public void InputOTP_TreatsInvalidAndWhitespaceValueCharactersAsEmptySlots()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Value, "1A 4")
            .Add(p => p.Length, 4));

        var values = cut.FindAll("input")
            .Select(input => input.GetAttribute("value") ?? string.Empty)
            .ToArray();

        values.ShouldBe(["1", string.Empty, string.Empty, "4"]);
    }

    [Fact]
    public void InputOTP_RendersNoSlots_ForNegativeLength()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, -3)
            .Add(p => p.Value, "123"));

        cut.FindAll("input").ShouldBeEmpty();
        cut.FindAll(".input-otp-separator").ShouldBeEmpty();
    }

    [Fact]
    public void InputOTP_RendersDefaultSeparatorInMiddle_WhenLengthAllows()
    {
        var cut = Render<InputOTP>();

        var separators = cut.FindAll(".input-otp-separator");
        separators.Count.ShouldBe(1);
        separators[0].TextContent.ShouldBe("-");
        separators[0].GetAttribute("aria-hidden").ShouldBe("true");
    }

    [Fact]
    public void InputOTP_RendersSeparatorsAtConfiguredIndices()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 4)
            .Add(p => p.Separator, "/")
            .Add(p => p.SeparatorIndices, new List<int> { 0, 2 }));

        var separators = cut.FindAll(".input-otp-separator");
        separators.Count.ShouldBe(2);
        separators[0].TextContent.ShouldBe("/");
        separators[1].TextContent.ShouldBe("/");
    }

    [Fact]
    public void InputOTP_DoesNotRenderSeparator_WhenSeparatorIsEmpty()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Separator, string.Empty));

        cut.FindAll(".input-otp-separator").ShouldBeEmpty();
    }

    [Fact]
    public void InputOTP_UpdatesSlotCount_WhenLengthChanges()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 2));

        cut.FindAll("input").Count.ShouldBe(2);

        cut.Render(parameters => parameters
            .Add(p => p.Length, 4));

        cut.FindAll("input").Count.ShouldBe(4);
        cut.FindAll("input")[3].GetAttribute("aria-label").ShouldBe("Digit 4 of 4");
    }

    [Fact]
    public void InputOTP_AppliesAlphaPatternAttributes()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Pattern, InputOTP.OTPPattern.Alpha));

        var input = cut.Find("input");
        input.GetAttribute("type").ShouldBe("text");
        input.GetAttribute("inputmode").ShouldBe("text");
        input.GetAttribute("aria-label").ShouldBe("Character 1 of 6");
    }

    [Fact]
    public void InputOTP_AppliesDisabledState()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Disabled, true));

        cut.Find(".input-otp").ClassList.ShouldContain("input-otp-disabled");
        cut.Find(".input-otp").GetAttribute("aria-disabled").ShouldBe("true");
        cut.FindAll("input").ShouldAllBe(input => input.HasAttribute("disabled"));
    }

    [Fact]
    public void InputOTP_AppliesReadOnlyState()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        cut.Find(".input-otp").ClassList.ShouldContain("input-otp-readonly");
        cut.FindAll("input").ShouldAllBe(input => input.HasAttribute("readonly"));
        cut.FindAll("input").ShouldAllBe(input => !input.HasAttribute("disabled"));
    }

    [Fact]
    public void InputOTP_AppliesCustomClassesAndAdditionalAttributes()
    {
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.CssClass, "otp-css-class")
            .Add(p => p.Class, "otp-class")
            .Add(p => p.AdditionalAttributes, new Dictionary<string, object>
            {
                ["data-testid"] = "otp-root"
            }));

        var group = cut.Find(".input-otp");
        group.ClassList.ShouldContain("otp-css-class");
        group.ClassList.ShouldContain("otp-class");
        group.GetAttribute("data-testid").ShouldBe("otp-root");
    }

    [Fact]
    public void InputOTP_InvokesValueChanged_ForValidInput()
    {
        string? changedValue = null;
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.Find("input").Input("7");

        changedValue.ShouldBe("7");
        cut.Find("input").GetAttribute("value").ShouldBe("7");
    }

    [Fact]
    public void InputOTP_DistributesMultiCharacterInputAcrossSlots()
    {
        var changedValues = new List<string>();
        var completedValues = new List<string>();
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 4)
            .Add(p => p.ValueChanged, value => changedValues.Add(value))
            .Add(p => p.OnComplete, value => completedValues.Add(value)));

        cut.FindAll("input")[0].Input("1234");

        changedValues.ShouldBe(["1234"]);
        completedValues.ShouldBe(["1234"]);
        cut.Find(".input-otp").ClassList.ShouldContain("input-otp-complete");
        cut.FindAll("input").Select(input => input.GetAttribute("value") ?? string.Empty)
            .ShouldBe(["1", "2", "3", "4"]);
    }

    [Fact]
    public void InputOTP_MultiCharacterInputSkipsInvalidCharacters()
    {
        var changedValues = new List<string>();
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 4)
            .Add(p => p.ValueChanged, value => changedValues.Add(value)));

        cut.Find("input").Input("1A2B");

        changedValues.ShouldBe(["12"]);
        cut.FindAll("input").Select(input => input.GetAttribute("value") ?? string.Empty)
            .ShouldBe(["1", "2", string.Empty, string.Empty]);
    }

    [Fact]
    public void InputOTP_RejectsInvalidNumericInput()
    {
        var changedValues = new List<string>();
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.ValueChanged, value => changedValues.Add(value)));

        cut.Find("input").Input("A");

        changedValues.ShouldBeEmpty();
        (cut.Find("input").GetAttribute("value") ?? string.Empty).ShouldBe(string.Empty);
    }

    [Fact]
    public void InputOTP_AlphaPatternAcceptsLettersAndRejectsDigits()
    {
        var changedValues = new List<string>();
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Pattern, InputOTP.OTPPattern.Alpha)
            .Add(p => p.ValueChanged, value => changedValues.Add(value)));

        cut.FindAll("input")[0].Input("A");
        cut.FindAll("input")[1].Input("1");

        changedValues.ShouldBe(["A"]);
        cut.FindAll("input").Select(input => input.GetAttribute("value") ?? string.Empty)
            .ShouldBe(["A", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty]);
    }

    [Fact]
    public void InputOTP_AnyPatternAcceptsPunctuation()
    {
        string? changedValue = null;
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Pattern, InputOTP.OTPPattern.Any)
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.Find("input").Input("#");

        changedValue.ShouldBe("#");
    }

    [Fact]
    public void InputOTP_DoesNotInvokeCallbacks_WhenDisabled()
    {
        var changedValues = new List<string>();
        var completedValues = new List<string>();
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 2)
            .Add(p => p.Value, "1")
            .Add(p => p.Disabled, true)
            .Add(p => p.ValueChanged, value => changedValues.Add(value))
            .Add(p => p.OnComplete, value => completedValues.Add(value)));

        cut.FindAll("input")[1].Input("2");
        cut.FindAll("input")[0].KeyDown("Backspace");

        changedValues.ShouldBeEmpty();
        completedValues.ShouldBeEmpty();
        cut.FindAll("input").Select(input => input.GetAttribute("value") ?? string.Empty)
            .ShouldBe(["1", string.Empty]);
    }

    [Fact]
    public void InputOTP_DoesNotMutateValue_WhenReadOnly()
    {
        var changedValues = new List<string>();
        var completedValues = new List<string>();
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 2)
            .Add(p => p.Value, "12")
            .Add(p => p.ReadOnly, true)
            .Add(p => p.ValueChanged, value => changedValues.Add(value))
            .Add(p => p.OnComplete, value => completedValues.Add(value)));

        cut.FindAll("input")[1].Input("9");
        cut.FindAll("input")[1].KeyDown("Backspace");

        changedValues.ShouldBeEmpty();
        completedValues.ShouldBeEmpty();
        cut.FindAll("input").Select(input => input.GetAttribute("value") ?? string.Empty)
            .ShouldBe(["1", "2"]);
    }

    [Fact]
    public void InputOTP_InvokesOnComplete_WhenLengthIsFilled()
    {
        string? completedValue = null;
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 2)
            .Add(p => p.Value, "1")
            .Add(p => p.OnComplete, value => completedValue = value));

        cut.FindAll("input")[1].Input("2");

        completedValue.ShouldBe("12");
    }

    [Fact]
    public void InputOTP_DoesNotInvokeOnComplete_UntilEverySlotHasValue()
    {
        var completedValues = new List<string>();
        var changedValues = new List<string>();
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 3)
            .Add(p => p.Value, "1")
            .Add(p => p.ValueChanged, value => changedValues.Add(value))
            .Add(p => p.OnComplete, value => completedValues.Add(value)));

        cut.FindAll("input")[2].Input("3");
        completedValues.ShouldBeEmpty();
        changedValues.ShouldBe(["1 3"]);

        cut.FindAll("input")[1].Input("2");

        completedValues.ShouldBe(["123"]);
        changedValues.ShouldBe(["1 3", "123"]);
    }

    [Fact]
    public void InputOTP_DoesNotRepeatOnComplete_ForSameValue()
    {
        var completedValues = new List<string>();
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 2)
            .Add(p => p.ValueChanged, _ => { })
            .Add(p => p.OnComplete, value => completedValues.Add(value)));

        cut.Find("input").Input("12");
        cut.FindAll("input")[1].Input("2");

        completedValues.ShouldBe(["12"]);
    }

    [Fact]
    public void InputOTP_ClearsSlot_OnEmptyInput()
    {
        string? changedValue = null;
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 2)
            .Add(p => p.Value, "12")
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.FindAll("input")[1].Input(string.Empty);

        changedValue.ShouldBe("1");
        (cut.FindAll("input")[1].GetAttribute("value") ?? string.Empty).ShouldBe(string.Empty);
    }

    [Fact]
    public void InputOTP_BackspaceClearsCurrentSlot()
    {
        string? changedValue = null;
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 2)
            .Add(p => p.Value, "12")
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.FindAll("input")[1].KeyDown("Backspace");

        changedValue.ShouldBe("1");
        (cut.FindAll("input")[1].GetAttribute("value") ?? string.Empty).ShouldBe(string.Empty);
    }

    [Fact]
    public void InputOTP_DeleteClearsCurrentSlot()
    {
        string? changedValue = null;
        var cut = Render<InputOTP>(parameters => parameters
            .Add(p => p.Length, 3)
            .Add(p => p.Value, "123")
            .Add(p => p.ValueChanged, value => changedValue = value));

        cut.FindAll("input")[1].KeyDown("Delete");

        changedValue.ShouldBe("1 3");
        (cut.FindAll("input")[1].GetAttribute("value") ?? string.Empty).ShouldBe(string.Empty);
    }
}
