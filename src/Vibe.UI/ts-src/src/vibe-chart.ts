// Vibe.UI Chart.js Integration
// This file provides JavaScript interop for Chart.js rendering in Blazor

// Chart.js type declarations (minimal subset needed)
declare global {
  interface Window {
    Chart: ChartConstructor;
    vibeChart: VibeChartAPI;
  }
}

// Chart.js constructor interface
interface ChartConstructor {
  new (context: CanvasRenderingContext2D, config: ChartConfig): ChartInstance;
}

// Chart instance interface
interface ChartInstance {
  data: ChartData;
  options: ChartOptions;
  config: ChartConfiguration;
  update(mode?: string): void;
  destroy(): void;
  resize(): void;
  toBase64Image(): string;
  getElementsAtEventForMode(
    event: Event,
    mode: string,
    options: { intersect: boolean },
    useFinalPosition: boolean
  ): ChartElement[];
}

// Chart configuration interface
export interface ChartConfig {
  type?: string;
  data: ChartData;
  options?: ChartOptions;
}

// Chart data interface
export interface ChartData {
  labels?: string[];
  datasets?: Array<{
    label?: string;
    data: number[];
    backgroundColor?: string | string[];
    borderColor?: string | string[];
    borderWidth?: number;
    [key: string]: unknown;
  }>;
  [key: string]: unknown;
}

// Chart options interface
export interface ChartOptions {
  responsive?: boolean;
  maintainAspectRatio?: boolean;
  plugins?: Record<string, unknown>;
  scales?: Record<string, unknown>;
  [key: string]: unknown;
}

interface ChartConfiguration {
  options?: ChartOptions;
  _config?: {
    options?: ChartOptions;
  };
}

// Chart element interface (returned by getElementAtEvent)
export interface ChartElement {
  datasetIndex: number;
  index: number;
  element?: {
    x: number;
    y: number;
    [key: string]: unknown;
  };
  [key: string]: unknown;
}

// Main Vibe Chart API interface
export interface VibeChartAPI {
  charts: Record<string, ChartInstance>;
  createChart(canvasId: string, config: ChartConfig): boolean;
  updateChart(canvasId: string, config: ChartConfig): boolean;
  destroyChart(canvasId: string): boolean;
  resizeChart(canvasId: string): boolean;
  getElementAtEvent(canvasId: string, event: Event): ChartElement | null;
  toBase64Image(canvasId: string): string | null;
}

interface ChartTheme {
  foreground: string;
  mutedForeground: string;
  border: string;
  popover: string;
  popoverForeground: string;
}

type ChartOptionRecord = Record<string, unknown>;

let themeObserver: MutationObserver | undefined;
let themeRefreshQueued = false;

function getThemeToken(style: CSSStyleDeclaration, token: string, fallback: string): string {
  return style.getPropertyValue(token).trim() || fallback;
}

function resolveChartTheme(): ChartTheme {
  const style = getComputedStyle(document.documentElement);

  return {
    foreground: getThemeToken(style, '--vibe-foreground', '#09090b'),
    mutedForeground: getThemeToken(style, '--vibe-muted-foreground', '#71717a'),
    border: getThemeToken(style, '--vibe-border', '#e4e4e7'),
    popover: getThemeToken(style, '--vibe-popover', '#ffffff'),
    popoverForeground: getThemeToken(style, '--vibe-popover-foreground', '#09090b')
  };
}

function asOptionRecord(value: unknown): ChartOptionRecord {
  return value !== null && typeof value === 'object' && !Array.isArray(value)
    ? value as ChartOptionRecord
    : {};
}

function getMutableChartOptions(chart: ChartInstance): ChartOptionRecord {
  return asOptionRecord(
    chart.config._config?.options
      ?? chart.config.options
      ?? chart.options);
}

function setMutableChartOptions(chart: ChartInstance, options: ChartOptions): void {
  if (chart.config._config) {
    chart.config._config.options = options;
  } else {
    chart.config.options = options;
  }

  chart.options = options;
}

function applyChartTheme(chart: ChartInstance): void {
  const theme = resolveChartTheme();
  const options = getMutableChartOptions(chart);
  const plugins = asOptionRecord(options.plugins);
  const legend = asOptionRecord(plugins.legend);
  const legendLabels = asOptionRecord(legend.labels);
  const tooltip = asOptionRecord(plugins.tooltip);
  const title = asOptionRecord(plugins.title);

  legendLabels.color = theme.mutedForeground;
  legend.labels = legendLabels;
  plugins.legend = legend;

  tooltip.backgroundColor = theme.popover;
  tooltip.titleColor = theme.popoverForeground;
  tooltip.bodyColor = theme.popoverForeground;
  tooltip.footerColor = theme.mutedForeground;
  tooltip.borderColor = theme.border;
  tooltip.borderWidth = 1;
  plugins.tooltip = tooltip;

  title.color = theme.foreground;
  plugins.title = title;
  options.plugins = plugins;

  const scales = asOptionRecord(options.scales);
  Object.entries(scales).forEach(([scaleName, rawScale]) => {
    const scale = asOptionRecord(rawScale);
    const grid = asOptionRecord(scale.grid);
    const ticks = asOptionRecord(scale.ticks);
    const border = asOptionRecord(scale.border);

    grid.color = theme.border;
    ticks.color = theme.mutedForeground;
    border.color = theme.border;
    scale.grid = grid;
    scale.ticks = ticks;
    scale.border = border;

    if (scaleName === 'r') {
      const angleLines = asOptionRecord(scale.angleLines);
      const pointLabels = asOptionRecord(scale.pointLabels);
      angleLines.color = theme.border;
      pointLabels.color = theme.mutedForeground;
      scale.angleLines = angleLines;
      scale.pointLabels = pointLabels;
    }

    scales[scaleName] = scale;
  });

  options.scales = scales;
  setMutableChartOptions(chart, options as ChartOptions);
}

function refreshChartThemes(api: VibeChartAPI): void {
  Object.values(api.charts).forEach(chart => {
    try {
      applyChartTheme(chart);
      chart.update('none');
    } catch (error) {
      console.error('Error applying chart theme:', error);
    }
  });
}

function queueThemeRefresh(api: VibeChartAPI): void {
  if (themeRefreshQueued) {
    return;
  }

  themeRefreshQueued = true;
  queueMicrotask(() => {
    themeRefreshQueued = false;
    refreshChartThemes(api);
  });
}

function observeDocumentTheme(api: VibeChartAPI): void {
  if (themeObserver || typeof MutationObserver === 'undefined') {
    return;
  }

  themeObserver = new MutationObserver(records => {
    if (records.some(record => record.type === 'attributes' && record.attributeName === 'class')) {
      queueThemeRefresh(api);
    }
  });

  themeObserver.observe(document.documentElement, {
    attributes: true,
    attributeFilter: ['class']
  });
}

// Implementation
const vibeChart: VibeChartAPI = {
  charts: {},

  // Initialize or update a chart
  createChart: function (canvasId: string, config: ChartConfig): boolean {
    try {
      // Destroy existing chart if it exists
      if (this.charts[canvasId]) {
        this.charts[canvasId].destroy();
      }

      const canvas = document.getElementById(canvasId);
      if (!canvas) {
        console.error(`Canvas element with id '${canvasId}' not found`);
        return false;
      }

      const ctx = (canvas as HTMLCanvasElement).getContext('2d');
      if (!ctx) {
        console.error(`Failed to get 2D context for canvas '${canvasId}'`);
        return false;
      }

      // Apply Vibe tokens after Chart.js creates its normalized option objects.
      const chart = new window.Chart(ctx, config);
      this.charts[canvasId] = chart;
      applyChartTheme(chart);
      chart.update('none');
      observeDocumentTheme(this);
      return true;
    } catch (error) {
      console.error('Error creating chart:', error);
      return false;
    }
  },

  // Update chart data
  updateChart: function (canvasId: string, config: ChartConfig): boolean {
    try {
      const chart = this.charts[canvasId];
      if (!chart) {
        return this.createChart(canvasId, config);
      }

      // Update chart data
      chart.data = config.data;
      if (config.options) {
        setMutableChartOptions(chart, config.options);
      }
      applyChartTheme(chart);
      chart.update();
      return true;
    } catch (error) {
      console.error('Error updating chart:', error);
      return false;
    }
  },

  // Destroy a chart
  destroyChart: function (canvasId: string): boolean {
    try {
      if (this.charts[canvasId]) {
        this.charts[canvasId].destroy();
        delete this.charts[canvasId];
        return true;
      }
      return false;
    } catch (error) {
      console.error('Error destroying chart:', error);
      return false;
    }
  },

  // Resize a chart
  resizeChart: function (canvasId: string): boolean {
    try {
      const chart = this.charts[canvasId];
      if (chart) {
        chart.resize();
        return true;
      }
      return false;
    } catch (error) {
      console.error('Error resizing chart:', error);
      return false;
    }
  },

  // Get chart data at a specific point
  getElementAtEvent: function (canvasId: string, event: Event): ChartElement | null {
    try {
      const chart = this.charts[canvasId];
      if (chart) {
        const elements = chart.getElementsAtEventForMode(
          event,
          'nearest',
          { intersect: true },
          false
        );
        return elements.length > 0 ? (elements[0] ?? null) : null;
      }
      return null;
    } catch (error) {
      console.error('Error getting element at event:', error);
      return null;
    }
  },

  // Export chart as image
  toBase64Image: function (canvasId: string): string | null {
    try {
      const chart = this.charts[canvasId];
      if (chart) {
        return chart.toBase64Image();
      }
      return null;
    } catch (error) {
      console.error('Error exporting chart:', error);
      return null;
    }
  }
};

// Expose to window for Blazor interop
window.vibeChart = vibeChart;

// Export for testing
export default vibeChart;
