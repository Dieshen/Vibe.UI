using static Vibe.UI.Components.Chart;

namespace Vibe.UI.Tests.Services;

public class ChartDataBuilderTests
{
    [Fact]
    public void CreatePieChart_UsesProvidedSliceColors()
    {
        var chartData = ChartDataBuilder.CreatePieChart(
            labels: ["Chrome", "Firefox", "Safari"],
            values: [64.5, 10.2, 18.3],
            colors: ["#4285F4", "#FF7139", "#00A4EF"]);

        chartData.Labels.ShouldBe(["Chrome", "Firefox", "Safari"]);
        chartData.Datasets.Count.ShouldBe(1);

        var dataset = chartData.Datasets.Single();
        dataset.Label.ShouldBe("Data");
        dataset.Data.ShouldBe([64.5, 10.2, 18.3]);
        dataset.Color.ShouldBe("#4285F4");
        dataset.BackgroundColor.ShouldBeNull();
        dataset.BorderColor.ShouldBeNull();
        dataset.BackgroundColors.ShouldBe(["#4285F4", "#FF7139", "#00A4EF"]);
        dataset.BorderColors.ShouldBe(["#4285F4", "#FF7139", "#00A4EF"]);
    }

    [Fact]
    public void CreatePieChart_GeneratesDefaultSliceColors_PerValue()
    {
        var chartData = ChartDataBuilder.CreatePieChart(
            labels: ["Q1", "Q2", "Q3", "Q4"],
            values: [10.0, 20.0, 30.0, 40.0]);

        var dataset = chartData.Datasets.Single();
        dataset.Color.ShouldBe("#3b82f6");
        dataset.BackgroundColors.ShouldBe(["#3b82f6", "#ef4444", "#10b981", "#f59e0b"]);
        dataset.BorderColors.ShouldBe(["#3b82f6", "#ef4444", "#10b981", "#f59e0b"]);
    }
}
