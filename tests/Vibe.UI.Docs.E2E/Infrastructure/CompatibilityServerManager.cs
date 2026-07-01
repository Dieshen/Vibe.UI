using System.Diagnostics;
using System.Net.Http;

namespace Vibe.UI.Docs.E2E.Infrastructure;

internal static class CompatibilityServerManager
{
    private static readonly object Lock = new();
    private static readonly Dictionary<CompatibilityApp, ServerState> Servers = new();

    internal static string GetDefaultBaseUrl(CompatibilityApp app)
    {
        return app switch
        {
            CompatibilityApp.StandaloneClient => "http://localhost:5101",
            CompatibilityApp.WebApp => "http://localhost:5102",
            _ => throw new ArgumentOutOfRangeException(nameof(app), app, null)
        };
    }

    internal static string GetBaseUrl(CompatibilityApp app)
    {
        var envName = GetEnvironmentVariableName(app);
        var configured = Environment.GetEnvironmentVariable(envName);
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured.TrimEnd('/');
        }

        return GetDefaultBaseUrl(app);
    }

    internal static async Task AcquireAsync(CompatibilityApp app, string baseUrl, CancellationToken cancellationToken)
    {
        if (!ShouldManageServer(app, baseUrl))
        {
            return;
        }

        lock (Lock)
        {
            if (!Servers.TryGetValue(app, out var state))
            {
                state = new ServerState();
                Servers[app] = state;
            }

            state.ActiveUsers++;
        }

        await EnsureStartedAsync(app, baseUrl, cancellationToken);
        await WaitForReadyAsync(app, baseUrl, cancellationToken);
    }

    internal static void Release(CompatibilityApp app, string baseUrl)
    {
        if (!ShouldManageServer(app, baseUrl))
        {
            return;
        }

        ServerState? state;
        var shouldStop = false;

        lock (Lock)
        {
            if (!Servers.TryGetValue(app, out state))
            {
                return;
            }

            state.ActiveUsers = Math.Max(0, state.ActiveUsers - 1);

            if (state.ActiveUsers == 0
                && state.Process != null
                && !state.Process.HasExited)
            {
                shouldStop = true;
            }
        }

        if (!shouldStop)
        {
            return;
        }

        try
        {
            state?.Process?.Kill(entireProcessTree: true);
        }
        catch
        {
            // Ignore shutdown failures; test runner is exiting anyway.
        }
    }

    private static string GetEnvironmentVariableName(CompatibilityApp app)
    {
        return app switch
        {
            CompatibilityApp.StandaloneClient => "VIBE_STANDALONE_BASE_URL",
            CompatibilityApp.WebApp => "VIBE_WEBAPP_BASE_URL",
            _ => throw new ArgumentOutOfRangeException(nameof(app), app, null)
        };
    }

    private static bool ShouldManageServer(CompatibilityApp app, string baseUrl)
    {
        var configured = Environment.GetEnvironmentVariable(GetEnvironmentVariableName(app));
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return false;
        }

        return string.Equals(
            baseUrl.TrimEnd('/'),
            GetDefaultBaseUrl(app),
            StringComparison.OrdinalIgnoreCase);
    }

    private static Task EnsureStartedAsync(CompatibilityApp app, string baseUrl, CancellationToken cancellationToken)
    {
        lock (Lock)
        {
            var state = Servers[app];
            if (state.Process != null && !state.Process.HasExited)
            {
                return Task.CompletedTask;
            }

            var repoRoot = GetRepoRoot();
            var projectPath = GetProjectPath(app);
            var profileRoot = Path.Combine(
                Path.GetTempPath(),
                "Vibe.UI.CompatibilityE2E",
                Environment.ProcessId.ToString(),
                app.ToString());
            var localAppDataPath = Path.Combine(profileRoot, "LocalAppData");
            var appDataPath = Path.Combine(profileRoot, "AppData");
            var dataProtectionPath = Path.Combine(profileRoot, "DataProtectionKeys");

            Directory.CreateDirectory(localAppDataPath);
            Directory.CreateDirectory(appDataPath);
            Directory.CreateDirectory(dataProtectionPath);

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" -c Release --no-build --no-launch-profile --urls \"{baseUrl}\"",
                WorkingDirectory = repoRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            startInfo.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Development";
            startInfo.EnvironmentVariables["DOTNET_ENVIRONMENT"] = "Development";
            startInfo.EnvironmentVariables["Logging__EventLog__LogLevel__Default"] = "None";
            startInfo.EnvironmentVariables["LOCALAPPDATA"] = localAppDataPath;
            startInfo.EnvironmentVariables["APPDATA"] = appDataPath;
            startInfo.EnvironmentVariables["VIBE_COMPATIBILITY_DATA_PROTECTION_PATH"] = dataProtectionPath;

            state.Process = Process.Start(startInfo);
            if (state.Process == null)
            {
                throw new InvalidOperationException($"Failed to start {app} compatibility server.");
            }

            state.Process.OutputDataReceived += (_, e) => state.AddOutput(e.Data);
            state.Process.ErrorDataReceived += (_, e) => state.AddOutput(e.Data);
            state.Process.BeginOutputReadLine();
            state.Process.BeginErrorReadLine();

            AppDomain.CurrentDomain.ProcessExit += (_, __) =>
            {
                try { state.Process?.Kill(entireProcessTree: true); } catch { }
            };
        }

        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    private static async Task WaitForReadyAsync(CompatibilityApp app, string baseUrl, CancellationToken cancellationToken)
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

        var output = Servers.TryGetValue(app, out var state)
            ? string.Join(Environment.NewLine, state.RecentOutput)
            : string.Empty;

        throw new TimeoutException(
            $"{app} compatibility server did not become ready at {baseUrl} within 90s. Recent output:{Environment.NewLine}{output}",
            last);
    }

    private static string GetProjectPath(CompatibilityApp app)
    {
        return app switch
        {
            CompatibilityApp.StandaloneClient => "samples/Vibe.UI.Compatibility.StandaloneClient/Vibe.UI.Compatibility.StandaloneClient.csproj",
            CompatibilityApp.WebApp => "samples/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp/Vibe.UI.Compatibility.WebApp.csproj",
            _ => throw new ArgumentOutOfRangeException(nameof(app), app, null)
        };
    }

    private static string GetRepoRoot()
    {
        var dir = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(dir))
        {
            if (File.Exists(Path.Combine(dir, "Vibe.sln")))
            {
                return dir;
            }

            dir = Directory.GetParent(dir)?.FullName;
        }

        return Directory.GetCurrentDirectory();
    }

    private sealed class ServerState
    {
        private readonly Queue<string> _recentOutput = new();

        internal Process? Process { get; set; }
        internal int ActiveUsers { get; set; }
        internal IReadOnlyCollection<string> RecentOutput => _recentOutput.ToArray();

        internal void AddOutput(string? line)
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
    }
}
