# Chart Component - Chart.js Integration

The Vibe.UI `Chart` component wraps Chart.js for common Blazor chart scenarios. The public component parameter for chart data is `Data`.

## Prerequisites

Load Chart.js 4.5.1 before the Vibe.UI chart interop script.

For the `Vibe.UI` NuGet package:

```html
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.5.1/dist/chart.umd.min.js"></script>
<script src="_content/Vibe.UI/js/vibe-chart.js"></script>
```

For CLI source installation, run `vibe init --with-charts` and use the copied app-local interop script:

```html
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.5.1/dist/chart.umd.min.js"></script>
<script src="js/vibe-chart.js"></script>
```

## Supported Chart Types

- `Line`
- `Bar`
- `Pie`
- `Doughnut`
- `Radar`
- `PolarArea`
- `Area`

Mixed charts are not currently supported.

## Basic Usage

### Line Chart

```razor
@using Vibe.UI.Components
@using Vibe.UI.Services
@using static Vibe.UI.Components.Chart

<Chart Data="@_chartData"
       Type="ChartType.Line"
       Title="Sales Data"
       Description="Monthly sales figures for 2024"
       Height="400" />

@code {
    private ChartData _chartData = null!;

    protected override void OnInitialized()
    {
        _chartData = new ChartDataBuilder()
            .WithLabels("Jan", "Feb", "Mar", "Apr", "May", "Jun")
            .AddDataset("Revenue", new[] { 65.0, 59.0, 80.0, 81.0, 56.0, 55.0 })
            .AddDataset("Costs", new[] { 28.0, 48.0, 40.0, 19.0, 86.0, 27.0 })
            .Build();
    }
}
```

### Bar Chart

```razor
<Chart Data="@_barData"
       Type="ChartType.Bar"
       Title="Product Sales"
       Height="350" />

@code {
    private ChartData _barData = null!;

    protected override void OnInitialized()
    {
        _barData = ChartDataBuilder.CreateBarChart(
            labels: new[] { "Product A", "Product B", "Product C", "Product D" },
            datasets: new Dictionary<string, double[]>
            {
                ["Q1"] = new[] { 12.0, 19.0, 3.0, 5.0 },
                ["Q2"] = new[] { 2.0, 3.0, 20.0, 30.0 },
                ["Q3"] = new[] { 15.0, 25.0, 18.0, 12.0 }
            });
    }
}
```

### Pie Chart With Per-Slice Colors

Use `ChartDataset.BackgroundColors` and `ChartDataset.BorderColors` when pie or doughnut slices need different colors. Keep using `Color` when a dataset only needs one color.

```razor
<Chart Data="@_pieData"
       Type="ChartType.Pie"
       Title="Market Share"
       Height="300"
       UseBuiltInLegend="true" />

@code {
    private ChartData _pieData = null!;

    protected override void OnInitialized()
    {
        _pieData = ChartDataBuilder.CreatePieChart(
            labels: new[] { "Chrome", "Firefox", "Safari", "Edge", "Other" },
            values: new[] { 64.5, 10.2, 18.3, 4.5, 2.5 },
            colors: new[] { "#4285F4", "#FF7139", "#00A4EF", "#0078D4", "#999999" });
    }
}
```

You can also build the dataset manually:

```csharp
var pieData = new ChartData
{
    Labels = ["North", "South", "West"],
    Datasets =
    [
        new ChartDataset
        {
            Label = "Share",
            Data = [35, 40, 25],
            Color = "#2563eb",
            BackgroundColors = ["#2563eb", "#14b8a6", "#f59e0b"],
            BorderColors = ["#1d4ed8", "#0f766e", "#d97706"]
        }
    ]
};
```

### Area Chart

```razor
<Chart Data="@_areaData"
       Type="ChartType.Area"
       Title="Website Traffic"
       Description="Visitors over time"
       Height="400"
       ShowGrid="true" />

@code {
    private ChartData _areaData = null!;

    protected override void OnInitialized()
    {
        _areaData = ChartDataBuilder.CreateAreaChart(
            labels: new[] { "Week 1", "Week 2", "Week 3", "Week 4" },
            datasets: new Dictionary<string, double[]>
            {
                ["Desktop"] = new[] { 1200.0, 1900.0, 1500.0, 2100.0 },
                ["Mobile"] = new[] { 800.0, 1200.0, 1400.0, 1800.0 }
            });
    }
}
```

## ChartDataBuilder Helper

`ChartDataBuilder` provides a fluent API for assembling chart data:

```csharp
var chartData = new ChartDataBuilder()
    .WithLabels("Jan", "Feb", "Mar", "Apr", "May", "Jun")
    .AddDataset("Sales", new[] { 65.0, 59.0, 80.0, 81.0, 56.0, 55.0 }, "#3b82f6")
    .AddDataset(dataset =>
    {
        dataset.Label = "Profit";
        dataset.Data = [28.0, 48.0, 40.0, 19.0, 86.0, 27.0];
        dataset.Color = "#10b981";
        dataset.BorderWidth = 3;
        dataset.Fill = true;
    })
    .Build();
```

Helper shortcuts are available for:

- `CreateLineChart`
- `CreateBarChart`
- `CreatePieChart`
- `CreateAreaChart`
- `CreateSampleData`

## Custom Dataset Configuration

```csharp
var dataset = new ChartDataset
{
    Label = "Revenue",
    Data = [12.0, 19.0, 3.0, 5.0, 2.0, 3.0],
    Color = "#3b82f6",
    BackgroundColor = "rgba(59, 130, 246, 0.2)",
    BorderColor = "#3b82f6",
    BorderWidth = 2,
    Fill = true
};
```

For pie and doughnut datasets, prefer `BackgroundColors` and `BorderColors` when you need slice-level colors.

## Component Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Data` | `ChartData` | Required | The chart data to render |
| `Type` | `ChartType` | `Line` | The chart type |
| `Title` | `string?` | `null` | Visible chart title |
| `Description` | `string?` | `null` | Visible chart description |
| `ShowLegend` | `bool` | `true` | Shows the legend |
| `UseBuiltInLegend` | `bool` | `false` | Uses Chart.js legend instead of the custom HTML legend |
| `ShowGrid` | `bool` | `true` | Shows grid lines for cartesian charts |
| `Height` | `int` | `300` | Chart height in pixels |
| `AriaLabel` | `string?` | `null` | Accessible label used when no visible title is provided |
| `EmptyMessage` | `string?` | `"No chart data available"` | Empty-state message |
| `Options` | `ChartOptions?` | `null` | Advanced chart options |
| `FooterContent` | `RenderFragment?` | `null` | Optional footer content |
| `CssClass` | `string?` | `null` | Additional CSS classes |

## Public Methods

### RefreshAsync

Call `RefreshAsync()` after mutating the existing `Data` instance:

```csharp
_chartData.Datasets[0].Data = [12.0, 18.0, 21.0];
await _chartRef.RefreshAsync();
```

### ExportAsImageAsync

Exports the current chart image as a base64 string:

```csharp
var base64Image = await _chartRef.ExportAsImageAsync();
```

Returns `null` if the chart has not been created yet or if JavaScript interop fails.

## Options

```csharp
var options = new ChartOptions
{
    Responsive = true,
    MaintainAspectRatio = false,
    Animation = new ChartAnimation
    {
        Duration = 750,
        Easing = "easeInOutQuart"
    },
    Tooltip = new ChartTooltip
    {
        Enabled = true,
        Mode = "index",
        Intersect = false
    }
};
```

## Accessibility Notes

- The canvas is rendered with `role="img"`.
- `Title` and `Description` are wired to `aria-labelledby` and `aria-describedby`.
- `AriaLabel` is used when no visible title is present.
- Empty charts render a status message instead of an empty canvas.
- The custom legend is rendered as an HTML list when enabled.

## Troubleshooting

### Chart not rendering

1. Ensure Chart.js is loaded before the package (`_content/Vibe.UI/js/vibe-chart.js`) or CLI (`js/vibe-chart.js`) interop script.
2. Confirm `Data` is not null and contains at least one dataset with values.
3. Check the browser console for Chart.js or script-loading errors.

### Chart not updating

If you mutate the existing `Data` object in place, call `RefreshAsync()` after the mutation.

### Chart.js version compatibility

This component is tested with Chart.js v4.5.1. Other v4.x versions should work. Chart.js v3.x is not supported.
