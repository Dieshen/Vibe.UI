// Vibe.UI Chart.js Integration
// This file provides JavaScript interop for Chart.js rendering in Blazor

window.vibeChart = {
    charts: {},

    // Initialize or update a chart
    createChart: function (canvasId, config) {
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

            const ctx = canvas.getContext('2d');

            // Create the chart
            this.charts[canvasId] = new Chart(ctx, config);
            return true;
        } catch (error) {
            console.error('Error creating chart:', error);
            return false;
        }
    },

    // Update chart data
    updateChart: function (canvasId, config) {
        try {
            const chart = this.charts[canvasId];
            if (!chart) {
                return this.createChart(canvasId, config);
            }

            // Update chart data
            chart.data = config.data;
            if (config.options) {
                chart.options = config.options;
            }
            chart.update();
            return true;
        } catch (error) {
            console.error('Error updating chart:', error);
            return false;
        }
    },

    // Destroy a chart
    destroyChart: function (canvasId) {
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
    resizeChart: function (canvasId) {
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
    getElementAtEvent: function (canvasId, event) {
        try {
            const chart = this.charts[canvasId];
            if (chart) {
                const elements = chart.getElementsAtEventForMode(
                    event,
                    'nearest',
                    { intersect: true },
                    false
                );
                return elements.length > 0 ? elements[0] : null;
            }
            return null;
        } catch (error) {
            console.error('Error getting element at event:', error);
            return null;
        }
    },

    // Export chart as image
    toBase64Image: function (canvasId) {
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
