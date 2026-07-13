using System.Text;
using Microsoft.Playwright;

namespace Vibe.UI.Docs.E2E.Infrastructure;

internal static class VisualRegression
{
    private const int ChannelThreshold = 16;
    private const double MaximumChangedPixelRatio = 0.005;

    public static async Task AssertMatchesAsync(
        IPage page,
        string browserName,
        string snapshotName,
        byte[] actualPng)
    {
        var repositoryRoot = FindRepositoryRoot();
        var platform = GetPlatformName();
        var safeBrowserName = NormalizePathSegment(browserName);
        var safeSnapshotName = NormalizePathSegment(snapshotName);
        var baselinePath = Path.Combine(
            repositoryRoot,
            "tests",
            "Vibe.UI.Docs.E2E",
            "Baselines",
            safeBrowserName,
            platform,
            $"{safeSnapshotName}.png");
        var artifactDirectory = Path.Combine(
            repositoryRoot,
            "output",
            "playwright",
            "visual-regression",
            safeBrowserName,
            platform);
        var actualPath = Path.Combine(artifactDirectory, $"{safeSnapshotName}.actual.png");
        var diffPath = Path.Combine(artifactDirectory, $"{safeSnapshotName}.diff.png");

        if (ShouldUpdateBaselines())
        {
            Directory.CreateDirectory(Path.GetDirectoryName(baselinePath)!);
            await File.WriteAllBytesAsync(baselinePath, actualPng);
            DeleteIfExists(actualPath);
            DeleteIfExists(diffPath);
            return;
        }

        if (!File.Exists(baselinePath))
        {
            Directory.CreateDirectory(artifactDirectory);
            await File.WriteAllBytesAsync(actualPath, actualPng);
            throw new InvalidOperationException(
                $"Visual baseline is missing for '{snapshotName}'. " +
                $"Review the actual image at '{actualPath}', then regenerate with " +
                "UPDATE_VISUAL_BASELINES=true.");
        }

        var baselinePng = await File.ReadAllBytesAsync(baselinePath);
        var result = await CompareInBrowserAsync(page, baselinePng, actualPng);

        if (result.DimensionsMatch && result.ChangedPixelRatio <= MaximumChangedPixelRatio)
        {
            DeleteIfExists(actualPath);
            DeleteIfExists(diffPath);
            return;
        }

        Directory.CreateDirectory(artifactDirectory);
        await File.WriteAllBytesAsync(actualPath, actualPng);
        if (!string.IsNullOrWhiteSpace(result.DiffDataUrl))
        {
            await File.WriteAllBytesAsync(diffPath, DecodeDataUrl(result.DiffDataUrl));
        }

        var details = result.DimensionsMatch
            ? $"{result.ChangedPixelRatio:P3} of pixels changed " +
              $"(allowed {MaximumChangedPixelRatio:P3}, channel threshold {ChannelThreshold}, " +
              $"maximum channel delta {result.MaximumChannelDelta})"
            : $"dimensions changed from {result.BaselineWidth}x{result.BaselineHeight} " +
              $"to {result.ActualWidth}x{result.ActualHeight}";

        throw new InvalidOperationException(
            $"Visual regression detected for '{snapshotName}': {details}. " +
            $"Baseline: '{baselinePath}'. Actual: '{actualPath}'. Diff: '{diffPath}'.");
    }

    private static async Task<PixelDiffResult> CompareInBrowserAsync(
        IPage page,
        byte[] baselinePng,
        byte[] actualPng)
    {
        var input = new Dictionary<string, object>
        {
            ["baselineDataUrl"] = ToDataUrl(baselinePng),
            ["actualDataUrl"] = ToDataUrl(actualPng),
            ["channelThreshold"] = ChannelThreshold
        };

        return await page.EvaluateAsync<PixelDiffResult>(
            """
            async input => {
                const loadImage = source => new Promise((resolve, reject) => {
                    const image = new Image();
                    image.onload = () => resolve(image);
                    image.onerror = () => reject(new Error('Unable to decode screenshot PNG.'));
                    image.src = source;
                });

                const [baselineImage, actualImage] = await Promise.all([
                    loadImage(input.baselineDataUrl),
                    loadImage(input.actualDataUrl)
                ]);

                const dimensionsMatch = baselineImage.width === actualImage.width &&
                    baselineImage.height === actualImage.height;

                if (!dimensionsMatch) {
                    return {
                        DimensionsMatch: false,
                        BaselineWidth: baselineImage.width,
                        BaselineHeight: baselineImage.height,
                        ActualWidth: actualImage.width,
                        ActualHeight: actualImage.height,
                        ChangedPixelRatio: 1,
                        MaximumChannelDelta: 255,
                        DiffDataUrl: null
                    };
                }

                const width = baselineImage.width;
                const height = baselineImage.height;
                const canvas = document.createElement('canvas');
                canvas.width = width;
                canvas.height = height;
                const context = canvas.getContext('2d', { willReadFrequently: true });

                context.drawImage(baselineImage, 0, 0);
                const baseline = context.getImageData(0, 0, width, height);
                context.clearRect(0, 0, width, height);
                context.drawImage(actualImage, 0, 0);
                const actual = context.getImageData(0, 0, width, height);
                const diff = context.createImageData(width, height);

                let changedPixels = 0;
                let maximumChannelDelta = 0;

                for (let index = 0; index < baseline.data.length; index += 4) {
                    const redDelta = Math.abs(baseline.data[index] - actual.data[index]);
                    const greenDelta = Math.abs(baseline.data[index + 1] - actual.data[index + 1]);
                    const blueDelta = Math.abs(baseline.data[index + 2] - actual.data[index + 2]);
                    const alphaDelta = Math.abs(baseline.data[index + 3] - actual.data[index + 3]);
                    const pixelDelta = Math.max(redDelta, greenDelta, blueDelta, alphaDelta);
                    maximumChannelDelta = Math.max(maximumChannelDelta, pixelDelta);

                    if (pixelDelta > input.channelThreshold) {
                        changedPixels++;
                        diff.data[index] = 244;
                        diff.data[index + 1] = 63;
                        diff.data[index + 2] = 94;
                        diff.data[index + 3] = 255;
                    } else {
                        const luminance = Math.round(
                            baseline.data[index] * 0.2126 +
                            baseline.data[index + 1] * 0.7152 +
                            baseline.data[index + 2] * 0.0722);
                        const muted = Math.round(248 - luminance * 0.15);
                        diff.data[index] = muted;
                        diff.data[index + 1] = muted;
                        diff.data[index + 2] = muted;
                        diff.data[index + 3] = 255;
                    }
                }

                context.putImageData(diff, 0, 0);

                return {
                    DimensionsMatch: true,
                    BaselineWidth: width,
                    BaselineHeight: height,
                    ActualWidth: width,
                    ActualHeight: height,
                    ChangedPixelRatio: changedPixels / (width * height),
                    MaximumChannelDelta: maximumChannelDelta,
                    DiffDataUrl: canvas.toDataURL('image/png')
                };
            }
            """,
            input);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Vibe.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not find Vibe.sln above '{AppContext.BaseDirectory}'.");
    }

    private static bool ShouldUpdateBaselines() =>
        bool.TryParse(Environment.GetEnvironmentVariable("UPDATE_VISUAL_BASELINES"), out var update) && update;

    private static string GetPlatformName()
    {
        if (OperatingSystem.IsWindows())
        {
            return "windows";
        }

        if (OperatingSystem.IsLinux())
        {
            return "linux";
        }

        if (OperatingSystem.IsMacOS())
        {
            return "macos";
        }

        return "unknown";
    }

    private static string NormalizePathSegment(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value.ToLowerInvariant())
        {
            builder.Append(char.IsAsciiLetterOrDigit(character) ? character : '-');
        }

        return builder.ToString().Trim('-');
    }

    private static string ToDataUrl(byte[] png) =>
        $"data:image/png;base64,{Convert.ToBase64String(png)}";

    private static byte[] DecodeDataUrl(string dataUrl)
    {
        var separatorIndex = dataUrl.IndexOf(',', StringComparison.Ordinal);
        if (separatorIndex < 0)
        {
            throw new InvalidDataException("Screenshot diff did not return a valid data URL.");
        }

        return Convert.FromBase64String(dataUrl[(separatorIndex + 1)..]);
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private sealed class PixelDiffResult
    {
        public bool DimensionsMatch { get; set; }
        public int BaselineWidth { get; set; }
        public int BaselineHeight { get; set; }
        public int ActualWidth { get; set; }
        public int ActualHeight { get; set; }
        public double ChangedPixelRatio { get; set; }
        public int MaximumChannelDelta { get; set; }
        public string? DiffDataUrl { get; set; }
    }
}
