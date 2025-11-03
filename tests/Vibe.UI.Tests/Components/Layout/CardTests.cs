namespace Vibe.UI.Tests.Components.Layout;

public class CardTests : TestBase
{
    [Fact]
    public void Card_Renders_WithDefaultProps()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .AddChildContent("Card content"));

        // Assert
        var card = cut.Find(".vibe-card");
        card.ShouldNotBeNull();
        card.TextContent.ShouldContain("Card content");
    }

    [Fact]
    public void Card_Renders_WithHeader()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .Add(p => p.Header, "Card Header")
            .AddChildContent("Body"));

        // Assert
        cut.Markup.ShouldContain("Card Header");
    }

    [Fact]
    public void Card_Renders_WithFooter()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .Add(p => p.Footer, "Card Footer")
            .AddChildContent("Body"));

        // Assert
        cut.Markup.ShouldContain("Card Footer");
    }

    [Fact]
    public void Card_WithEmptyContent_RendersEmpty()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .AddChildContent(""));

        // Assert
        var content = cut.Find(".vibe-card-content");
        content.InnerHtml.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void Card_WithNullHeader_OnlyRendersBody()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .AddChildContent("Body content"));

        // Assert
        cut.Markup.ShouldContain("Body content");
        cut.FindAll(".vibe-card-header").ShouldBeEmpty();
    }

    [Fact]
    public void Card_WithNullFooter_OnlyRendersHeaderAndBody()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .Add(p => p.Header, "Header")
            .AddChildContent("Body"));

        // Assert
        cut.Markup.ShouldContain("Header");
        cut.Markup.ShouldContain("Body");
        cut.FindAll(".vibe-card-footer").ShouldBeEmpty();
    }

    [Fact]
    public void Card_WithAllSections_RendersAllCorrectly()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .Add(p => p.Header, "Card Header")
            .Add(p => p.Footer, "Card Footer")
            .AddChildContent("Card Body"));

        // Assert
        cut.Find(".vibe-card-header").TextContent.ShouldContain("Card Header");
        cut.Find(".vibe-card-content").TextContent.ShouldContain("Card Body");
        cut.Find(".vibe-card-footer").TextContent.ShouldContain("Card Footer");
    }

    [Fact]
    public void Card_WithCustomClass_AppliesCorrectly()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .Add(p => p.Class, "custom-card-class")
            .AddChildContent("Content"));

        // Assert
        var card = cut.Find(".vibe-card");
        card.ClassList.ShouldContain("custom-card-class");
    }

    [Fact]
    public void Card_WithAdditionalAttributes_AppliesCorrectly()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .AddChildContent("Content")
            .AddUnmatched("data-testid", "test-card")
            .AddUnmatched("aria-label", "Test Card"));

        // Assert
        var card = cut.Find(".vibe-card");
        card.GetAttribute("data-testid").ShouldBe("test-card");
        card.GetAttribute("aria-label").ShouldBe("Test Card");
    }

    [Fact]
    public void Card_WithComplexHeader_RendersCorrectly()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .Add(p => p.Header, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "card-title");
                builder.AddContent(2, "Complex Header");
                builder.CloseElement();
            })
            .AddChildContent("Body"));

        // Assert
        var header = cut.Find(".vibe-card-header");
        header.InnerHtml.ShouldContain("card-title");
        header.InnerHtml.ShouldContain("Complex Header");
    }

    [Fact]
    public void Card_WithComplexFooter_RendersCorrectly()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .AddChildContent("Body")
            .Add(p => p.Footer, builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "card-actions");
                builder.AddContent(2, "Action Buttons");
                builder.CloseElement();
            }));

        // Assert
        var footer = cut.Find(".vibe-card-footer");
        footer.InnerHtml.ShouldContain("card-actions");
        footer.InnerHtml.ShouldContain("Action Buttons");
    }

    [Fact]
    public void Card_WithVeryLargeContent_HandlesOverflow()
    {
        // Arrange
        var largeContent = new string('X', 10000);

        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .AddChildContent(largeContent));

        // Assert
        var content = cut.Find(".vibe-card-content");
        content.TextContent.ShouldContain("XXX");
        content.TextContent.Length.ShouldBeGreaterThan(5000);
    }

    [Fact]
    public void Card_WithNestedComponents_RendersCorrectly()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .AddChildContent(builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "nested-content");
                builder.OpenElement(2, "p");
                builder.AddContent(3, "Paragraph 1");
                builder.CloseElement();
                builder.OpenElement(4, "p");
                builder.AddContent(5, "Paragraph 2");
                builder.CloseElement();
                builder.CloseElement();
            }));

        // Assert
        var content = cut.Find(".vibe-card-content");
        content.InnerHtml.ShouldContain("nested-content");
        content.InnerHtml.ShouldContain("Paragraph 1");
        content.InnerHtml.ShouldContain("Paragraph 2");
    }

    [Fact]
    public void Card_WithOnlyHeader_RendersHeaderAndBody()
    {
        // Act
        var cut = RenderComponent<Card>(parameters => parameters
            .Add(p => p.Header, "Only Header")
            .AddChildContent("Body"));

        // Assert
        cut.Find(".vibe-card-header").ShouldNotBeNull();
        cut.Find(".vibe-card-content").ShouldNotBeNull();
        cut.FindAll(".vibe-card-footer").ShouldBeEmpty();
    }
}
