using System.Diagnostics;
using System.Net.Http;

namespace Vibe.UI.Docs.E2E.Infrastructure;

internal static class DocsServerManager
{
    private static readonly object _lock = new();
    private static Process? _process;
    private static int _activeUsers;
    private static readonly Queue<string> _recentOutput = new();
    private static bool _processExitHandlerRegistered;

    internal static string DefaultBaseUrl => "http://localhost:5000";

    internal static async Task AcquireAsync(string baseUrl, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            _activeUsers++;
        }

        if (!ShouldManageServer(baseUrl))
        {
            return;
        }

        await EnsureStartedAsync(cancellationToken);
        await WaitForReadyAsync(baseUrl, cancellationToken);
    }

    internal static void Release(string baseUrl)
    {
        lock (_lock)
        {
            _activeUsers = Math.Max(0, _activeUsers - 1);
        }

        if (!ShouldManageServer(baseUrl))
        {
            return;
        }

        // Keep the shared docs server alive for the test process. Restarting
        // between individual E2E tests is slow and can miss readiness windows
        // on hosted CI runners. The process-exit handler handles final cleanup.
    }

    private static Task EnsureStartedAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_process != null && !_process.HasExited)
            {
                return Task.CompletedTask;
            }

            // Start the docs site using the Blazor WASM dev server.
            // Use --no-build to avoid concurrent build output locks during test runs.
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                // Force a stable port for tests (ignore launchSettings.json which uses a random dev port).
                Arguments = "run --project \"samples/Vibe.UI.Docs/Vibe.UI.Docs.csproj\" -c Release --no-build --no-launch-profile --urls \"http://localhost:5000\"",
                WorkingDirectory = GetRepoRoot(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            startInfo.EnvironmentVariables["DOTNET_ENVIRONMENT"] = "Development";

            _process = Process.Start(startInfo);
            if (_process == null)
            {
                throw new InvalidOperationException("Failed to start docs server process.");
            }

            _process.OutputDataReceived += (_, e) => AddOutput(e.Data);
            _process.ErrorDataReceived += (_, e) => AddOutput(e.Data);
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            if (!_processExitHandlerRegistered)
            {
                AppDomain.CurrentDomain.ProcessExit += (_, __) =>
                {
                    try { _process?.Kill(entireProcessTree: true); } catch { }
                };
                _processExitHandlerRegistered = true;
            }
        }

        return Task.CompletedTask;
    }

    private static async Task WaitForReadyAsync(string baseUrl, CancellationToken cancellationToken)
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };

        var deadline = DateTimeOffset.UtcNow.AddSeconds(90);
        Exception? last = null;

        while (DateTimeOffset.UtcNow < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                using var response = await http.GetAsync(baseUrl, cancellationToken);
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 500)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                last = ex;
            }

            await Task.Delay(500, cancellationToken);
        }

        throw new TimeoutException(
            $"Docs server did not become ready at {baseUrl} within 90s. Recent output:{Environment.NewLine}{GetRecentOutput()}",
            last);
    }

    private static void AddOutput(string? line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        lock (_recentOutput)
        {
            _recentOutput.Enqueue(line);

            while (_recentOutput.Count > 40)
            {
                _recentOutput.Dequeue();
            }
        }
    }

    private static string GetRecentOutput()
    {
        lock (_recentOutput)
        {
            return string.Join(Environment.NewLine, _recentOutput);
        }
    }

    private static bool ShouldManageServer(string baseUrl)
    {
        // An explicit URL, including the default one, means the caller owns
        // the server lifecycle. This prevents a second dev server from
        // competing for the same port during externally managed E2E runs.
        if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOCS_BASE_URL")))
        {
            return false;
        }

        return string.Equals(baseUrl.TrimEnd('/'), DefaultBaseUrl, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetRepoRoot()
    {
        // Tests execute under the repo; walk up until we find the solution file.
        var dir = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(dir))
        {
            if (File.Exists(Path.Combine(dir, "Vibe.sln")))
            {
                return dir;
            }

            dir = Directory.GetParent(dir)?.FullName;
        }

        // Fallback: use current directory.
        return Directory.GetCurrentDirectory();
    }
}
