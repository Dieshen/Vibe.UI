using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Compatibility;

[Trait("Category", TestCategories.Compatibility)]
public class StandaloneClientCompatibilityTests : CompatibilityE2ETestBase
{
    protected override CompatibilityApp App => CompatibilityApp.StandaloneClient;

    [Fact]
    public async Task StandaloneWebAssemblySupportsVibeUiInteractions()
    {
        await NavigateToSmokePageAsync("/", "standalone-client-smoke");
        await VerifyInteractiveSmokeAsync("standalone");

        AssertNoBrowserErrors();
    }
}
