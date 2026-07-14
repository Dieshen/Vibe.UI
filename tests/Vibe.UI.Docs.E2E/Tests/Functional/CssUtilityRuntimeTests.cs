using Shouldly;
using Vibe.UI.Docs.E2E.Infrastructure;
using Xunit;

namespace Vibe.UI.Docs.E2E.Tests.Functional;

[Trait("Category", TestCategories.Functional)]
public class CssUtilityRuntimeTests : E2ETestBase
{
    [Fact]
    public async Task RingWidthUtility_HasAComputedShadowWithoutCustomPropertyInitialization()
    {
        await NavigateAndWaitForBlazorAsync("/components");

        var boxShadow = await Page.EvaluateAsync<string>(
            """
            () => {
                const probe = document.createElement('button');
                probe.className = 'focus:vibe-ring-2';
                probe.textContent = 'Ring probe';
                document.body.appendChild(probe);
                probe.focus();
                const value = getComputedStyle(probe).boxShadow;
                probe.remove();
                return value;
            }
            """);

        boxShadow.ShouldNotBe("none");
        boxShadow.ShouldContain("2px");
    }
}
