using Vibe.UI.Components;
using static Vibe.UI.Components.Chart;

namespace Vibe.UI.Services;

/// <summary>
/// Helper class for building chart data configurations.
/// </summary>
public class ChartDataBuilder
{
    private readonly ChartData _chartData;

    public ChartDataBuilder()
    {
        _chartData = new ChartData();
    }

    /// <summary>
    /// Sets the labels for the chart.
    /// </summary>
    public ChartDataBuilder WithLabels(params string[] labels)
    {
        _chartData.Labels = labels.ToList();
        return this;
    }

    /// <summary>
    /// Sets the labels for the chart.
    /// </summary>
    public ChartDataBuilder WithLabels(IEnumerable<string> labels)
    {
        _chartData.Labels = labels.ToList();
        return this;
    }

    /// <summary>
    /// Adds a dataset to the chart.
    /// </summary>
    public ChartDataBuilder AddDataset(string label, IEnumerable<double> data, string? color = null)
    {
        var dataset = new ChartDataset
        {
            Label = label,
            Data = data.ToList(),
            Color = color ?? GetDefaultColor(_chartData.Datasets.Count)
        };

        _chartData.Datasets.Add(dataset);
        return this;
    }

    /// <summary>
    /// Adds a dataset with custom configuration.
    /// </summary>
    public ChartDataBuilder AddDataset(Action<ChartDataset> configure)
    {
        var dataset = new ChartDataset
        {
            Color = GetDefaultColor(_chartData.Datasets.Count)
        };

        configure(dataset);
        _chartData.Datasets.Add(dataset);
        return this;
    }

    /// <summary>
    /// Builds and returns the chart data.
    /// </summary>
    public ChartData Build()
    {
        return _chartData;
    }

    /// <summary>
    /// Creates a simple line chart data configuration.
    /// </summary>
    public static ChartData CreateLineChart(string[] labels, Dictionary<string, double[]> datasets)
    {
        var builder = new ChartDataBuilder().WithLabels(labels);

        foreach (var (label, data) in datasets)
        {
            builder.AddDataset(label, data);
        }

        return builder.Build();
    }

    /// <summary>
    /// Creates a simple bar chart data configuration.
    /// </summary>
    public static ChartData CreateBarChart(string[] labels, Dictionary<string, double[]> datasets)
    {
        return CreateLineChart(labels, datasets);
    }

    /// <summary>
    /// Creates a pie or doughnut chart data configuration.
    /// </summary>
    public static ChartData CreatePieChart(string[] labels, double[] values, string[]? colors = null)
    {
        var builder = new ChartDataBuilder().WithLabels(labels);

        builder.AddDataset(dataset =>
        {
            dataset.Label = "Data";
            dataset.Data = values.ToList();

            if (colors != null && colors.Length > 0)
            {
                // For pie/doughnut charts, we need multiple background colors
                dataset.BackgroundColor = string.Join(",", colors);
            }
            else
            {
                // Generate default colors for each slice
                var defaultColors = values.Select((_, i) => GetDefaultColor(i)).ToArray();
                dataset.BackgroundColor = string.Join(",", defaultColors);
            }
        });

        return builder.Build();
    }

    /// <summary>
    /// Creates an area chart data configuration.
    /// </summary>
    public static ChartData CreateAreaChart(string[] labels, Dictionary<string, double[]> datasets)
    {
        var builder = new ChartDataBuilder().WithLabels(labels);

        var index = 0;
        foreach (var (label, data) in datasets)
        {
            var color = GetDefaultColor(index);
            builder.AddDataset(dataset =>
            {
                dataset.Label = label;
                dataset.Data = data.ToList();
                dataset.Color = color;
                dataset.BackgroundColor = AddAlpha(color, 0.2);
                dataset.BorderColor = color;
                dataset.Fill = true;
            });
            index++;
        }

        return builder.Build();
    }

    /// <summary>
    /// Creates sample data for demonstration purposes.
    /// </summary>
    public static ChartData CreateSampleData(ChartType type)
    {
        var labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
        var values1 = new[] { 65.0, 59.0, 80.0, 81.0, 56.0, 55.0 };
        var values2 = new[] { 28.0, 48.0, 40.0, 19.0, 86.0, 27.0 };

        return type switch
        {
            ChartType.Line => CreateLineChart(labels, new Dictionary<string, double[]>
            {
                ["Dataset 1"] = values1,
                ["Dataset 2"] = values2
            }),

            ChartType.Bar => CreateBarChart(labels, new Dictionary<string, double[]>
            {
                ["Dataset 1"] = values1,
                ["Dataset 2"] = values2
            }),

            ChartType.Pie or ChartType.Doughnut => CreatePieChart(labels, values1),

            ChartType.Area => CreateAreaChart(labels, new Dictionary<string, double[]>
            {
                ["Dataset 1"] = values1,
                ["Dataset 2"] = values2
            }),

            _ => new ChartData()
        };
    }

    private static string GetDefaultColor(int index)
    {
        var colors = new[]
        {
            "#3b82f6", // blue
            "#ef4444", // red
            "#10b981", // green
            "#f59e0b", // amber
            "#8b5cf6", // violet
            "#ec4899", // pink
            "#06b6d4", // cyan
            "#f97316", // orange
            "#84cc16", // lime
            "#6366f1"  // indigo
        };

        return colors[index % colors.Length];
    }

    private static string AddAlpha(string color, double alpha)
    {
        // Convert hex to rgba with alpha
        if (color.StartsWith("#") && color.Length == 7)
        {
            var r = Convert.ToInt32(color.Substring(1, 2), 16);
            var g = Convert.ToInt32(color.Substring(3, 2), 16);
            var b = Convert.ToInt32(color.Substring(5, 2), 16);
            return $"rgba({r}, {g}, {b}, {alpha})";
        }

        return color;
    }
}
