namespace Vibe.UI.Tests.Components.DataDisplay;

public class ChartTests : TestBase
{
    [Fact]
    public void Chart_RendersContainerCanvasAndHeader()
    {
        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.Title, "Revenue")
            .Add(p => p.Description, "Monthly revenue")
            .Add(p => p.Height, 420));

        var chart = cut.Find(".vibe-chart");
        chart.ClassList.ShouldContain("vibe-chart-line");
        cut.Find(".vibe-chart-title").TextContent.ShouldBe("Revenue");
        cut.Find(".vibe-chart-description").TextContent.ShouldBe("Monthly revenue");
        cut.Find(".vibe-chart-container").GetAttribute("style").ShouldBe("height: 420px;");
        cut.Find("canvas.vibe-chart-canvas").GetAttribute("id").ShouldStartWith("chart-");
        cut.Find("canvas.vibe-chart-canvas").GetAttribute("role").ShouldBe("img");
    }

    [Fact]
    public void Chart_RendersCustomLegendByDefault()
    {
        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData()));

        var legend = cut.Find(".vibe-chart-legend");
        legend.TextContent.ShouldContain("Series A");
        legend.TextContent.ShouldContain("Series B");
        cut.FindAll(".vibe-chart-legend-color")[0].GetAttribute("style")!.ShouldContain("background-color: #111111");
    }

    [Fact]
    public void Chart_HidesCustomLegend_WhenDisabledOrBuiltIn()
    {
        var hidden = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.ShowLegend, false));

        var builtIn = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.UseBuiltInLegend, true));

        hidden.FindAll(".vibe-chart-legend").ShouldBeEmpty();
        builtIn.FindAll(".vibe-chart-legend").ShouldBeEmpty();
    }

    [Fact]
    public void Chart_AppliesTypeAndCustomClass()
    {
        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.Type, Chart.ChartType.Bar)
            .Add(p => p.CssClass, "dashboard-chart")
            .Add(p => p.Class, "outer-chart")
            .AddUnmatched("data-chart", "revenue"));

        var chart = cut.Find(".vibe-chart");
        chart.ClassList.ShouldContain("vibe-chart-bar");
        chart.ClassList.ShouldContain("dashboard-chart");
        chart.ClassList.ShouldContain("outer-chart");
        chart.GetAttribute("data-chart").ShouldBe("revenue");
    }

    [Fact]
    public void Chart_RendersFooterContent()
    {
        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.FooterContent, builder => builder.AddMarkupContent(0, "<button>Export</button>")));

        cut.Find(".vibe-chart-footer").InnerHtml.ShouldContain("Export");
    }

    [Fact]
    public async Task Chart_ExportReturnsNull_WhenJsInteropFails()
    {
        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData()));

        var result = await cut.Instance.ExportAsImageAsync();

        result.ShouldBeNull();
    }

    [Fact]
    public void Chart_RendersEmptyState_AndSkipsJsInterop_WhenDataIsEmpty()
    {
        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, new Chart.ChartData())
            .Add(p => p.EmptyMessage, "Nothing to chart"));

        cut.FindAll("canvas.vibe-chart-canvas").ShouldBeEmpty();
        cut.Find(".vibe-chart-empty").TextContent.ShouldBe("Nothing to chart");
        JSInterop.Invocations.ShouldBeEmpty();
    }

    [Fact]
    public void Chart_ClampsHeight_AndNormalizesInvalidType()
    {
        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.Type, (Chart.ChartType)999)
            .Add(p => p.Height, -50));

        var chart = cut.Find(".vibe-chart");
        chart.ClassList.ShouldContain("vibe-chart-line");
        chart.ClassList.ShouldNotContain("vibe-chart-999");
        cut.Find(".vibe-chart-container").GetAttribute("style").ShouldBe("height: 0px;");
    }

    [Fact]
    public void Chart_UsesTitleAndDescriptionForCanvasAccessibility()
    {
        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.Title, "Revenue")
            .Add(p => p.Description, "Monthly revenue"));

        var titleId = cut.Find(".vibe-chart-title").Id;
        var descriptionId = cut.Find(".vibe-chart-description").Id;
        var canvas = cut.Find("canvas.vibe-chart-canvas");

        canvas.GetAttribute("aria-labelledby").ShouldBe(titleId);
        canvas.GetAttribute("aria-describedby").ShouldBe(descriptionId);
        canvas.HasAttribute("aria-label").ShouldBeFalse();
    }

    [Fact]
    public void Chart_UsesAriaLabel_WhenNoVisibleTitleIsProvided()
    {
        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.AriaLabel, "Quarterly revenue chart"));

        var canvas = cut.Find("canvas.vibe-chart-canvas");
        canvas.GetAttribute("aria-label").ShouldBe("Quarterly revenue chart");
        canvas.HasAttribute("aria-labelledby").ShouldBeFalse();
    }

    [Fact]
    public void Chart_UpdatesViaJsInterop_AfterParametersChange()
    {
        JSInterop.Setup<bool>("vibeChart.createChart", _ => true).SetResult(true);
        JSInterop.Setup<bool>("vibeChart.updateChart", _ => true).SetResult(true);

        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData()));

        JSInterop.Invocations.Count.ShouldBe(1);
        JSInterop.Invocations.ElementAt(0).Identifier.ShouldBe("vibeChart.createChart");

        cut.Render(parameters => parameters
            .Add(p => p.Data, new Chart.ChartData
            {
                Labels = ["Mar"],
                Datasets = [new() { Label = "Series C", Data = [5], Color = "#333333" }]
            }));

        JSInterop.Invocations.Count.ShouldBe(2);
        JSInterop.Invocations.ElementAt(1).Identifier.ShouldBe("vibeChart.updateChart");
    }

    private static Chart.ChartData CreateData() => new()
    {
        Labels = ["Jan", "Feb"],
        Datasets =
        [
            new() { Label = "Series A", Data = [1, 2], Color = "#111111" },
            new() { Label = "Series B", Data = [3, 4], Color = "#222222", BackgroundColor = "#abcdef" }
        ]
    };
}
