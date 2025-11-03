namespace Vibe.UI.Tests.Components.Input;

public class RatingTests : TestBase
{
    [Fact]
    public void Rating_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Rating>();

        // Assert
        var rating = cut.Find(".vibe-rating");
        rating.ShouldNotBeNull();
        rating.GetAttribute("role").ShouldBe("radiogroup");
    }

    [Fact]
    public void Rating_Renders_DefaultFiveStars()
    {
        // Act
        var cut = RenderComponent<Rating>();

        // Assert
        var stars = cut.FindAll(".rating-star");
        stars.Count.ShouldBe(5);
    }

    [Fact]
    public void Rating_Renders_CustomMaxRating()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.MaxRating, 10));

        // Assert
        var stars = cut.FindAll(".rating-star");
        stars.Count.ShouldBe(10);
    }

    [Fact]
    public void Rating_Displays_CurrentValue()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 3)
            .Add(p => p.MaxRating, 5));

        // Assert
        var filledStars = cut.FindAll(".star-filled");
        filledStars.Count.ShouldBe(3);
    }

    [Fact]
    public void Rating_ShowsValue_WhenShowValueIsTrue()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 4.5)
            .Add(p => p.MaxRating, 5)
            .Add(p => p.ShowValue, true));

        // Assert
        var valueDisplay = cut.Find(".rating-value");
        valueDisplay.TextContent.ShouldContain("4.5");
        valueDisplay.TextContent.ShouldContain("5");
    }

    [Fact]
    public void Rating_HidesValue_WhenShowValueIsFalse()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 3)
            .Add(p => p.ShowValue, false));

        // Assert
        cut.FindAll(".rating-value").ShouldBeEmpty();
    }

    [Fact]
    public void Rating_Applies_SizeClass()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Size, Rating.RatingSize.Large));

        // Assert
        cut.Find(".vibe-rating").ClassList.ShouldContain("rating-large");
    }

    [Fact]
    public void Rating_Applies_DisabledState()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Disabled, true));

        // Assert
        cut.Find(".vibe-rating").ClassList.ShouldContain("rating-disabled");
        var stars = cut.FindAll(".rating-star");
        foreach (var star in stars)
        {
            star.HasAttribute("disabled").ShouldBeTrue();
        }
    }

    [Fact]
    public void Rating_Applies_ReadOnlyState()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.ReadOnly, true));

        // Assert
        cut.Find(".vibe-rating").ClassList.ShouldContain("rating-readonly");
        var stars = cut.FindAll(".rating-star");
        foreach (var star in stars)
        {
            star.HasAttribute("disabled").ShouldBeTrue();
        }
    }

    [Fact]
    public void Rating_InvokesValueChanged_WhenStarClicked()
    {
        // Arrange
        double newValue = 0;
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 0)
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act
        var stars = cut.FindAll(".rating-star");
        stars[2].Click(); // Click the 3rd star

        // Assert
        newValue.ShouldBe(3);
    }

    [Fact]
    public void Rating_SupportsHalfStars_WhenAllowHalfIsTrue()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 2.5)
            .Add(p => p.AllowHalf, true));

        // Assert
        var halfStars = cut.FindAll(".star-half");
        halfStars.ShouldNotBeEmpty();
    }

    [Fact]
    public void Rating_HasAccessibilityLabel()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.AriaLabel, "Product Rating"));

        // Assert
        var rating = cut.Find(".vibe-rating");
        rating.GetAttribute("aria-label").ShouldBe("Product Rating");
    }

    [Fact]
    public void Rating_DoesNotInvokeCallback_WhenDisabled()
    {
        // Arrange
        double newValue = 0;
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Disabled, true)
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act
        var stars = cut.FindAll(".rating-star");
        stars[2].Click();

        // Assert
        newValue.ShouldBe(0);
    }

    [Fact]
    public void Rating_Value0_AllStarsEmpty()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 0));

        // Assert
        var filledStars = cut.FindAll(".star-filled");
        filledStars.ShouldBeEmpty();
    }

    [Fact]
    public void Rating_ValueEqualToMax_AllStarsFilled()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 5)
            .Add(p => p.MaxRating, 5));

        // Assert
        var filledStars = cut.FindAll(".star-filled");
        filledStars.Count.ShouldBe(5);
    }

    [Fact]
    public void Rating_NegativeValue_ShowsEmpty()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, -1));

        // Assert
        var filledStars = cut.FindAll(".star-filled");
        filledStars.ShouldBeEmpty();
    }

    [Fact]
    public void Rating_ValueAboveMax_ShowsAllFilled()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 10)
            .Add(p => p.MaxRating, 5));

        // Assert
        var filledStars = cut.FindAll(".star-filled");
        filledStars.Count.ShouldBe(5);
    }

    [Fact]
    public void Rating_HalfRating_RendersHalfStar()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 2.5)
            .Add(p => p.AllowHalf, true));

        // Assert
        var halfStars = cut.FindAll(".star-half");
        halfStars.Count.ShouldBe(1);
        var filledStars = cut.FindAll(".star-filled");
        filledStars.Count.ShouldBe(2);
    }

    [Fact]
    public void Rating_AllowHalf_ClickSameRating_GivesHalf()
    {
        // Arrange
        double newValue = 0;
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 3)
            .Add(p => p.AllowHalf, true)
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act - Click the 3rd star when value is already 3
        var stars = cut.FindAll(".rating-star");
        stars[2].Click();

        // Assert - Should give half rating (2.5)
        newValue.ShouldBe(2.5);
    }

    [Fact]
    public void Rating_AllowHalf_False_NoHalfStars()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 2.5)
            .Add(p => p.AllowHalf, false));

        // Assert
        var halfStars = cut.FindAll(".star-half");
        halfStars.ShouldBeEmpty();
    }

    [Fact]
    public void Rating_ReadOnly_ClickDoesNothing()
    {
        // Arrange
        double newValue = 0;
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 2)
            .Add(p => p.ReadOnly, true)
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act
        var stars = cut.FindAll(".rating-star");
        stars[3].Click();

        // Assert
        newValue.ShouldBe(0);
    }

    [Fact]
    public void Rating_CustomMaxRating10_Renders10Stars()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.MaxRating, 10));

        // Assert
        var stars = cut.FindAll(".rating-star");
        stars.Count.ShouldBe(10);
    }

    [Fact]
    public void Rating_CustomMaxRating1_RendersSingleStar()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.MaxRating, 1));

        // Assert
        var stars = cut.FindAll(".rating-star");
        stars.Count.ShouldBe(1);
    }

    [Fact]
    public void Rating_ShowValue_DisplaysCurrentAndMax()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Value, 3.5)
            .Add(p => p.MaxRating, 10)
            .Add(p => p.ShowValue, true));

        // Assert
        var valueDisplay = cut.Find(".rating-value");
        valueDisplay.TextContent.ShouldContain("3.5");
        valueDisplay.TextContent.ShouldContain("10");
    }

    [Fact]
    public void Rating_SizeSmall_HasSmallClass()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Size, Rating.RatingSize.Small));

        // Assert
        cut.Find(".vibe-rating").ClassList.ShouldContain("rating-small");
    }

    [Fact]
    public void Rating_SizeLarge_HasLargeClass()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Size, Rating.RatingSize.Large));

        // Assert
        cut.Find(".vibe-rating").ClassList.ShouldContain("rating-large");
    }

    [Fact]
    public void Rating_SizeDefault_HasDefaultClass()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.Size, Rating.RatingSize.Default));

        // Assert
        cut.Find(".vibe-rating").ClassList.ShouldContain("rating-default");
    }

    [Fact]
    public void Rating_DefaultAriaLabel_IsRating()
    {
        // Act
        var cut = RenderComponent<Rating>();

        // Assert
        var rating = cut.Find(".vibe-rating");
        rating.GetAttribute("aria-label").ShouldBe("Rating");
    }

    [Fact]
    public void Rating_StarButtons_HaveAriaLabels()
    {
        // Act
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.MaxRating, 3));

        // Assert
        var stars = cut.FindAll(".rating-star");
        stars[0].GetAttribute("aria-label").ShouldBe("1 star");
        stars[1].GetAttribute("aria-label").ShouldBe("2 stars");
        stars[2].GetAttribute("aria-label").ShouldBe("3 stars");
    }

    [Fact]
    public void Rating_ClickFirstStar_SetsValue1()
    {
        // Arrange
        double newValue = 0;
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act
        var stars = cut.FindAll(".rating-star");
        stars[0].Click();

        // Assert
        newValue.ShouldBe(1);
    }

    [Fact]
    public void Rating_ClickLastStar_SetsMaxValue()
    {
        // Arrange
        double newValue = 0;
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.MaxRating, 7)
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act
        var stars = cut.FindAll(".rating-star");
        stars[6].Click();

        // Assert
        newValue.ShouldBe(7);
    }

    [Fact]
    public void Rating_DoesNotInvokeCallback_WhenReadOnly()
    {
        // Arrange
        double newValue = 0;
        var cut = RenderComponent<Rating>(parameters => parameters
            .Add(p => p.ReadOnly, true)
            .Add(p => p.ValueChanged, value => newValue = value));

        // Act
        var stars = cut.FindAll(".rating-star");
        stars[2].Click();

        // Assert
        newValue.ShouldBe(0);
    }

    [Fact]
    public void Rating_AllStarsHaveButtonType()
    {
        // Act
        var cut = RenderComponent<Rating>();

        // Assert
        var stars = cut.FindAll(".rating-star");
        foreach (var star in stars)
        {
            star.GetAttribute("type").ShouldBe("button");
        }
    }
}
