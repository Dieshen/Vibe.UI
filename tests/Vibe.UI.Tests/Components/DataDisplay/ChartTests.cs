using System.Text.Json;

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
    public void Chart_HidesVisualLegend_WhenDisabledOrBuiltIn()
    {
        var hidden = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.ShowLegend, false));

        var builtIn = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.UseBuiltInLegend, true));

        hidden.FindAll(".vibe-chart-legend").ShouldBeEmpty();
        builtIn.Find(".vibe-chart-legend--screen-reader-only").GetAttribute("role").ShouldBe("list");
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
        var summaryId = cut.Find(".vibe-chart-accessible-data p").Id;
        var canvas = cut.Find("canvas.vibe-chart-canvas");

        canvas.GetAttribute("aria-labelledby").ShouldBe(titleId);
        canvas.GetAttribute("aria-describedby").ShouldBe($"{descriptionId} {summaryId}");
        canvas.HasAttribute("aria-label").ShouldBeFalse();
    }

    [Fact]
    public void Chart_RendersAccessibleDataSummaryAndTable()
    {
        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Data, CreateData())
            .Add(p => p.Title, "Revenue"));

        var summary = cut.Find(".vibe-chart-accessible-data p");
        var table = cut.Find(".vibe-chart-accessible-data table");
        var canvas = cut.Find("canvas.vibe-chart-canvas");

        summary.TextContent.ShouldContain("Revenue has 2 data points across 2 series.");
        table.QuerySelector("caption")!.TextContent.ShouldBe("Data table for Revenue");
        table.QuerySelectorAll("thead th")[1].TextContent.ShouldBe("Series A");
        table.QuerySelectorAll("tbody tr")[0].TextContent.ShouldContain("Jan");
        table.QuerySelectorAll("tbody tr")[0].TextContent.ShouldContain("1");
        canvas.GetAttribute("aria-describedby").ShouldBe(summary.Id);
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

    [Fact]
    public void Chart_UsesStructuredSliceColorsAndSemanticSliceLegend_ForPieDatasets()
    {
        JSInterop.Setup<bool>("vibeChart.createChart", _ => true).SetResult(true);

        var cut = Render<Chart>(parameters => parameters
            .Add(p => p.Type, Chart.ChartType.Pie)
            .Add(p => p.Data, new Chart.ChartData
            {
                Labels = ["North", "South", "West"],
                Datasets =
                [
                    new()
                    {
                        Label = "Share",
                        Data = [35, 40, 25],
                        Color = "#111111",
                        BackgroundColors = ["#ff6384", "#36a2eb", "#ffce56"],
                        BorderColors = ["#c21d52", "#1f6ea5", "#d4a315"]
                    }
                ]
            }));

        var invocation = JSInterop.Invocations.Single();
        invocation.Identifier.ShouldBe("vibeChart.createChart");

        var configJson = JsonSerializer.Serialize(invocation.Arguments[1]);
        configJson.ShouldContain(@"""backgroundColor"":[""#ff6384"",""#36a2eb"",""#ffce56""]");
        configJson.ShouldContain(@"""borderColor"":[""#c21d52"",""#1f6ea5"",""#d4a315""]");
        var legendItems = cut.FindAll(".vibe-chart-legend-item");
        legendItems.Count.ShouldBe(3);
        legendItems[0].TextContent.ShouldContain("North");
        legendItems[0].TextContent.ShouldContain("35");
        legendItems[1].TextContent.ShouldContain("South");
        legendItems[1].QuerySelector(".vibe-chart-legend-color")!.GetAttribute("style")!.ShouldContain("background-color: #36a2eb");
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
