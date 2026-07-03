/**
 * Tests for vibe-chart.ts
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import {
  setupChartMock,
  teardownChartMock,
  createMockChartConfig,
  getLastChartInstance,
  MockChart
} from './mocks/chart.mock';
import { createMockCanvas } from './setup';

describe('vibe-chart', () => {
  beforeEach(async () => {
    setupChartMock();
    document.body.innerHTML = '';

    // Reset vibeChart's internal state to avoid pollution between tests
    const { default: vibeChart } = await import('../src/vibe-chart');
    // Clear all tracked charts
    Object.keys(vibeChart.charts).forEach(key => {
      delete vibeChart.charts[key];
    });
  });

  afterEach(() => {
    teardownChartMock();
  });

  describe('createChart', () => {
    it('should create a new chart instance', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      // Import after mocks are set up
      const { default: vibeChart } = await import('../src/vibe-chart');

      const config = createMockChartConfig();
      const result = vibeChart.createChart('test-chart', config);

      expect(result).toBe(true);
      expect(MockChart).toHaveBeenCalledTimes(1);
      expect(vibeChart.charts['test-chart']).toBeDefined();
    });

    it('should return false when canvas element not found', async () => {
      const { default: vibeChart } = await import('../src/vibe-chart');
      const config = createMockChartConfig();

      const result = vibeChart.createChart('non-existent', config);

      expect(result).toBe(false);
      expect(MockChart).not.toHaveBeenCalled();
    });

    it('should destroy existing chart before creating new one', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');
      const config = createMockChartConfig();

      // Create first chart
      vibeChart.createChart('test-chart', config);
      const firstInstance = getLastChartInstance();

      // Create second chart with same ID
      vibeChart.createChart('test-chart', config);

      expect(firstInstance?.destroyed).toBe(true);
      expect(MockChart).toHaveBeenCalledTimes(2);
    });

    it('should handle errors during chart creation', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      // Make Chart constructor throw
      MockChart.mockImplementationOnce(function () {
        throw new Error('Chart creation failed');
      });

      const { default: vibeChart } = await import('../src/vibe-chart');
      const config = createMockChartConfig();

      const result = vibeChart.createChart('test-chart', config);

      expect(result).toBe(false);
      expect(console.error).toHaveBeenCalledWith('Error creating chart:', expect.any(Error));
    });

    it('should return false when getContext returns null', async () => {
      const canvas = document.createElement('canvas');
      canvas.id = 'test-chart';
      canvas.getContext = vi.fn(() => null);
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');
      const config = createMockChartConfig();

      const result = vibeChart.createChart('test-chart', config);

      expect(result).toBe(false);
    });
  });

  describe('updateChart', () => {
    it('should update existing chart data', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');
      const config = createMockChartConfig();

      // Create chart
      vibeChart.createChart('test-chart', config);
      const instance = getLastChartInstance();

      // Update chart
      const newConfig = createMockChartConfig({
        data: {
          labels: ['A', 'B', 'C'],
          datasets: [{ label: 'New', data: [1, 2, 3] }]
        }
      });

      const result = vibeChart.updateChart('test-chart', newConfig);

      expect(result).toBe(true);
      expect(instance?.data).toEqual(newConfig.data);
      expect(instance?.updated).toBe(true);
    });

    it('should update chart options if provided', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());
      const instance = getLastChartInstance();

      const newOptions = { responsive: false };
      const result = vibeChart.updateChart('test-chart', {
        data: { datasets: [] },
        options: newOptions
      });

      expect(result).toBe(true);
      expect(instance?.options).toEqual(newOptions);
    });

    it('should create chart if it does not exist', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');
      const config = createMockChartConfig();

      const result = vibeChart.updateChart('test-chart', config);

      expect(result).toBe(true);
      expect(MockChart).toHaveBeenCalledTimes(1);
    });

    it('should handle update errors', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());
      const instance = getLastChartInstance();

      // Make update throw
      instance!.update = vi.fn(() => {
        throw new Error('Update failed');
      });

      const result = vibeChart.updateChart('test-chart', createMockChartConfig());

      expect(result).toBe(false);
      expect(console.error).toHaveBeenCalledWith('Error updating chart:', expect.any(Error));
    });
  });

  describe('destroyChart', () => {
    it('should destroy existing chart', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());
      const instance = getLastChartInstance();

      const result = vibeChart.destroyChart('test-chart');

      expect(result).toBe(true);
      expect(instance?.destroyed).toBe(true);
      expect(vibeChart.charts['test-chart']).toBeUndefined();
    });

    it('should return false for non-existent chart', async () => {
      const { default: vibeChart } = await import('../src/vibe-chart');

      const result = vibeChart.destroyChart('non-existent');

      expect(result).toBe(false);
    });

    it('should handle destroy errors', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());
      const instance = getLastChartInstance();

      // Make destroy throw
      instance!.destroy = vi.fn(() => {
        throw new Error('Destroy failed');
      });

      const result = vibeChart.destroyChart('test-chart');

      expect(result).toBe(false);
      expect(console.error).toHaveBeenCalledWith('Error destroying chart:', expect.any(Error));
    });
  });

  describe('resizeChart', () => {
    it('should resize existing chart', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());
      const instance = getLastChartInstance();

      const result = vibeChart.resizeChart('test-chart');

      expect(result).toBe(true);
      expect(instance?.resized).toBe(true);
    });

    it('should return false for non-existent chart', async () => {
      const { default: vibeChart } = await import('../src/vibe-chart');

      const result = vibeChart.resizeChart('non-existent');

      expect(result).toBe(false);
    });

    it('should handle resize errors', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());
      const instance = getLastChartInstance();

      instance!.resize = vi.fn(() => {
        throw new Error('Resize failed');
      });

      const result = vibeChart.resizeChart('test-chart');

      expect(result).toBe(false);
      expect(console.error).toHaveBeenCalledWith('Error resizing chart:', expect.any(Error));
    });
  });

  describe('getElementAtEvent', () => {
    it('should return element at event', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());
      const instance = getLastChartInstance();

      const mockElement = { datasetIndex: 0, index: 1 };
      instance?.setMockElements([mockElement]);

      const event = new MouseEvent('click');
      const result = vibeChart.getElementAtEvent('test-chart', event);

      expect(result).toEqual(mockElement);
    });

    it('should return null when no elements found', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());

      const event = new MouseEvent('click');
      const result = vibeChart.getElementAtEvent('test-chart', event);

      expect(result).toBeNull();
    });

    it('should return null for non-existent chart', async () => {
      const { default: vibeChart } = await import('../src/vibe-chart');

      const event = new MouseEvent('click');
      const result = vibeChart.getElementAtEvent('non-existent', event);

      expect(result).toBeNull();
    });

    it('should handle errors', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());
      const instance = getLastChartInstance();

      instance!.getElementsAtEventForMode = vi.fn(() => {
        throw new Error('Get element failed');
      });

      const event = new MouseEvent('click');
      const result = vibeChart.getElementAtEvent('test-chart', event);

      expect(result).toBeNull();
      expect(console.error).toHaveBeenCalledWith(
        'Error getting element at event:',
        expect.any(Error)
      );
    });
  });

  describe('toBase64Image', () => {
    it('should export chart as base64 image', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());

      const result = vibeChart.toBase64Image('test-chart');

      expect(result).toBe('data:image/png;base64,mockBase64Data');
    });

    it('should return null for non-existent chart', async () => {
      const { default: vibeChart } = await import('../src/vibe-chart');

      const result = vibeChart.toBase64Image('non-existent');

      expect(result).toBeNull();
    });

    it('should handle export errors', async () => {
      const canvas = createMockCanvas('test-chart');
      document.body.appendChild(canvas);

      const { default: vibeChart } = await import('../src/vibe-chart');

      vibeChart.createChart('test-chart', createMockChartConfig());
      const instance = getLastChartInstance();

      instance!.toBase64Image = vi.fn(() => {
        throw new Error('Export failed');
      });

      const result = vibeChart.toBase64Image('test-chart');

      expect(result).toBeNull();
      expect(console.error).toHaveBeenCalledWith('Error exporting chart:', expect.any(Error));
    });
  });
});
