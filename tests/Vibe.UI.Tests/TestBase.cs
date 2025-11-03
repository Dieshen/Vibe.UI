using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace Vibe.UI.Tests;

/// <summary>
/// Base class for Vibe.UI component tests
/// </summary>
public abstract class TestBase : TestContext
{
    protected TestBase()
    {
        // Register Vibe.UI services
        Services.AddVibeUI();

        // Setup JSInterop for tests
        JSInterop.Mode = JSRuntimeMode.Loose;
    }
}
