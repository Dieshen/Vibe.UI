using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Compatibility;

[Trait("Category", TestCategories.Compatibility)]
public class WebAppCompatibilityTests : CompatibilityE2ETestBase
{
    protected override CompatibilityApp App => CompatibilityApp.WebApp;

    [Fact]
    public async Task StaticSsrRendersVibeUiComponents()
    {
        await NavigateToSmokePageAsync("/", "webapp-static-smoke");

        await Page.Locator("text=Static render path").WaitForAsync();
        await Page.Locator("text=Interactive Server").WaitForAsync();
        await Page.Locator("text=Interactive WebAssembly").WaitForAsync();
        await Page.Locator("text=Interactive Auto").WaitForAsync();

        AssertNoBrowserErrors();
    }

    [Theory]
    [InlineData("/server", "webapp-server-smoke", "server")]
    [InlineData("/client", "webapp-client-smoke", "client")]
    [InlineData("/auto", "webapp-auto-smoke", "auto")]
    public async Task InteractiveRenderModesSupportVibeUiInteractions(string path, string smokeTestId, string prefix)
    {
        await NavigateToSmokePageAsync(path, smokeTestId);
        await VerifyInteractiveSmokeAsync(prefix);

        AssertNoBrowserErrors();
    }
}
