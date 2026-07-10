/**
 * Mock Chart.js for testing
 */

import { vi } from 'vitest';
import type { ChartConfig, ChartData, ChartOptions, ChartElement } from '../../src/vibe-chart';

// Mock Chart instance
export class MockChartInstance {
  public data: ChartData;
  public options: ChartOptions;
  public destroyed = false;
  public updated = false;
  public resized = false;

  private mockElements: ChartElement[] = [];

  constructor(
    public context: CanvasRenderingContext2D,
    config: ChartConfig
  ) {
    this.data = config.data;
    this.options = config.options || {};
  }

  update(): void {
    if (this.destroyed) {
      throw new Error('Cannot update destroyed chart');
    }
    this.updated = true;
  }

  destroy(): void {
    this.destroyed = true;
  }

  resize(): void {
    if (this.destroyed) {
      throw new Error('Cannot resize destroyed chart');
    }
    this.resized = true;
  }

  toBase64Image(): string {
    if (this.destroyed) {
      throw new Error('Cannot export destroyed chart');
    }
    return 'data:image/png;base64,mockBase64Data';
  }

  getElementsAtEventForMode(
    _event: Event,
    _mode: string,
    _options: { intersect: boolean },
    _useFinalPosition: boolean
  ): ChartElement[] {
    if (this.destroyed) {
      throw new Error('Cannot get elements from destroyed chart');
    }
    return this.mockElements;
  }

  // Test helper: Set mock elements to be returned
  setMockElements(elements: ChartElement[]): void {
    this.mockElements = elements;
  }
}

// Track instances created during tests
let chartInstances: MockChartInstance[] = [];
let lastChartInstance: MockChartInstance | null = null;

// Mock Chart constructor
export const MockChart = vi.fn(function (ctx: CanvasRenderingContext2D, config: ChartConfig) {
  const instance = new MockChartInstance(ctx, config);
  chartInstances.push(instance);
  lastChartInstance = instance;
  return instance;
});

// Setup function to install mock in window
export function setupChartMock(): void {
  // Reset tracking state
  chartInstances = [];
  lastChartInstance = null;
  MockChart.mockClear();
  (window as unknown as { Chart: typeof MockChart }).Chart = MockChart;
}

// Teardown function
export function teardownChartMock(): void {
  delete (window as unknown as { Chart?: typeof MockChart }).Chart;
  MockChart.mockClear();
  lastChartInstance = null;
  chartInstances = [];
}

// Helper: Get all chart instances created
export function getChartInstances(): MockChartInstance[] {
  return chartInstances;
}

// Helper: Get last chart instance created
export function getLastChartInstance(): MockChartInstance | undefined {
  return lastChartInstance ?? undefined;
}

// Export mock data builders
export function createMockChartData(overrides?: Partial<ChartData>): ChartData {
  return {
    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May'],
    datasets: [
      {
        label: 'Dataset 1',
        data: [10, 20, 30, 40, 50],
        backgroundColor: '#3b82f6',
        borderColor: '#2563eb'
      }
    ],
    ...overrides
  };
}

export function createMockChartOptions(overrides?: Partial<ChartOptions>): ChartOptions {
  return {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'top'
      },
      title: {
        display: false,
        text: 'Chart Title'
      }
    },
    ...overrides
  };
}

export function createMockChartConfig(overrides?: Partial<ChartConfig>): ChartConfig {
  return {
    type: 'line',
    data: createMockChartData(),
    options: createMockChartOptions(),
    ...overrides
  };
}
