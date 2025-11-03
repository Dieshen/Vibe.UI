# Chart Component - Chart.js Integration

The Vibe.UI Chart component provides full Chart.js integration for creating beautiful, interactive data visualizations in your Blazor applications.

## Prerequisites

To use the Chart component, you need to include Chart.js in your application. Add the following to your `index.html` (Blazor WebAssembly) or `_Host.cshtml` (Blazor Server):

```html
<!-- Chart.js CDN -->
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>

<!-- Vibe.UI Chart Interop -->
<script src="_content/Vibe.UI/js/vibe-chart.js"></script>
```

Alternatively, you can install Chart.js via npm and bundle it with your application.

## Supported Chart Types

- **Line** - Line charts with smooth curves
- **Bar** - Vertical bar charts
- **Pie** - Circular pie charts
- **Doughnut** - Doughnut/donut charts
- **Radar** - Radar/spider charts
- **PolarArea** - Polar area charts
- **Area** - Filled area charts

## Basic Usage

### Simple Line Chart

```razor
@using Vibe.UI.Components
@using Vibe.UI.Services

<Chart ChartData="@_chartData"
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
<Chart ChartData="@_barData"
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
            }
        );
    }
}
```

### Pie Chart

```razor
<Chart ChartData="@_pieData"
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
            colors: new[] { "#4285F4", "#FF7139", "#00A4EF", "#0078D4", "#999999" }
        );
    }
}
```

### Area Chart

```razor
<Chart ChartData="@_areaData"
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
            }
        );
    }
}
```

## ChartDataBuilder Helper

The `ChartDataBuilder` class provides a fluent API for creating chart data:

```csharp
var chartData = new ChartDataBuilder()
    .WithLabels("Jan", "Feb", "Mar", "Apr", "May", "Jun")
    .AddDataset("Sales", new[] { 65.0, 59.0, 80.0, 81.0, 56.0, 55.0 }, "#3b82f6")
    .AddDataset(dataset => {
        dataset.Label = "Profit";
        dataset.Data = new List<double> { 28.0, 48.0, 40.0, 19.0, 86.0, 27.0 };
        dataset.Color = "#10b981";
        dataset.BorderWidth = 3;
        dataset.Fill = true;
    })
    .Build();
```

### Quick Sample Data

For testing, you can use the built-in sample data generator:

```csharp
var sampleData = ChartDataBuilder.CreateSampleData(ChartType.Line);
```

## Advanced Configuration

### Custom Options

```razor
<Chart ChartData="@_data"
       Type="ChartType.Line"
       Options="@_chartOptions"
       Height="400" />

@code {
    private ChartOptions _chartOptions = new()
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
}
```

### Custom Dataset Configuration

```csharp
var dataset = new ChartDataset
{
    Label = "Revenue",
    Data = new List<double> { 12.0, 19.0, 3.0, 5.0, 2.0, 3.0 },
    Color = "#3b82f6",                    // Primary color
    BackgroundColor = "rgba(59, 130, 246, 0.2)",  // Fill color
    BorderColor = "#3b82f6",              // Border color
    BorderWidth = 2,
    Fill = true                           // Fill area under line
};
```

## Component Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChartData` | `ChartData` | Required | The data to display in the chart |
| `Type` | `ChartType` | `Line` | The type of chart to render |
| `Title` | `string?` | `null` | Chart title |
| `Description` | `string?` | `null` | Chart description |
| `ShowLegend` | `bool` | `true` | Whether to show the legend |
| `UseBuiltInLegend` | `bool` | `false` | Use Chart.js built-in legend instead of HTML |
| `ShowGrid` | `bool` | `true` | Show grid lines (doesn't apply to pie/doughnut) |
| `Height` | `int` | `300` | Chart height in pixels |
| `Options` | `ChartOptions?` | `null` | Advanced chart options |
| `FooterContent` | `RenderFragment?` | `null` | Custom footer content |
| `CssClass` | `string?` | `null` | Additional CSS classes |

## Public Methods

### RefreshAsync()

Manually refresh/update the chart:

```csharp
@ref="_chartRef"

<Button OnClick="UpdateData">Update Chart</Button>

@code {
    private Chart _chartRef = null!;

    private async Task UpdateData()
    {
        // Update chart data
        _chartData.Datasets[0].Data = GetNewData();

        // Refresh the chart
        await _chartRef.RefreshAsync();
    }
}
```

### ExportAsImageAsync()

Export the chart as a base64 image:

```csharp
<Button OnClick="ExportChart">Export Chart</Button>

@code {
    private Chart _chartRef = null!;

    private async Task ExportChart()
    {
        var base64Image = await _chartRef.ExportAsImageAsync();
        if (base64Image != null)
        {
            // Use the image data (e.g., download, display, etc.)
            await JSRuntime.InvokeVoidAsync("downloadImage", base64Image, "chart.png");
        }
    }
}
```

## Responsive Charts

Charts are responsive by default. To customize responsiveness:

```csharp
var options = new ChartOptions
{
    Responsive = true,           // Resize with container
    MaintainAspectRatio = false  // Allow flexible height
};
```

## Theming

Charts automatically use Vibe.UI theme colors. You can override colors per dataset:

```csharp
var dataset = new ChartDataset
{
    Label = "My Data",
    Data = myData,
    Color = "var(--vibe-primary)",        // Use CSS variable
    BackgroundColor = "rgba(59, 130, 246, 0.1)"
};
```

## Real-time Updates

For real-time data updates:

```razor
<Chart @ref="_liveChart" ChartData="@_liveData" Type="ChartType.Line" />

@code {
    private Chart _liveChart = null!;
    private ChartData _liveData = null!;
    private Timer? _timer;

    protected override void OnInitialized()
    {
        _liveData = new ChartDataBuilder()
            .WithLabels(Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray())
            .AddDataset("Live Data", Enumerable.Repeat(0.0, 10))
            .Build();

        _timer = new Timer(async _ =>
        {
            // Add new data point
            var newValue = Random.Shared.NextDouble() * 100;
            _liveData.Datasets[0].Data.RemoveAt(0);
            _liveData.Datasets[0].Data.Add(newValue);

            await InvokeAsync(async () =>
            {
                await _liveChart.RefreshAsync();
                StateHasChanged();
            });
        }, null, 1000, 1000);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
```

## Troubleshooting

### Chart not rendering

1. **Ensure Chart.js is loaded**: Check browser console for errors
2. **Check script order**: Vibe chart script must load after Chart.js
3. **Verify data**: Ensure `ChartData` is not null and has valid data

### Chart not updating

Call `RefreshAsync()` after updating the chart data:

```csharp
_chartData.Datasets[0].Data = newData;
await chartRef.RefreshAsync();
```

### Chart.js version compatibility

This component is tested with Chart.js v4.4.0. Other v4.x versions should work, but v3.x is not compatible.

## Examples

Check the `samples/Vibe.UI.Showcase` project for complete examples of all chart types.
