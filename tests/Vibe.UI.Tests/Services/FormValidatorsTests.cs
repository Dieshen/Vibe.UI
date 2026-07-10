namespace Vibe.UI.Tests.Services;

public class FormValidatorsTests
{
    [Fact]
    public void CreditCard_RejectsNumbersShorterThanStandardCardLengths()
    {
        var validator = FormValidators.CreditCard();

        var result = validator("0");

        result.ShouldBe("Card number must be between 12 and 19 digits");
    }

    [Theory]
    [InlineData("4111 1111 1111 1111")]
    [InlineData("3782-822463-10005")]
    [InlineData("4222 2222 2222 2")]
    public void CreditCard_AcceptsConventionalValidCardNumbers(string cardNumber)
    {
        var validator = FormValidators.CreditCard();

        var result = validator(cardNumber);

        result.ShouldBeNull();
    }

    [Fact]
    public void CreditCard_RejectsNonDigitCharactersAfterRemovingAcceptedSeparators()
    {
        var validator = FormValidators.CreditCard();

        var result = validator("4111 1111 1111 111a");

        result.ShouldBe("Card number must contain only digits");
    }

    [Fact]
    public void CreditCard_RejectsNumbersLongerThanStandardCardLengths()
    {
        var validator = FormValidators.CreditCard();

        var result = validator("41111111111111111111");

        result.ShouldBe("Card number must be between 12 and 19 digits");
    }
}
