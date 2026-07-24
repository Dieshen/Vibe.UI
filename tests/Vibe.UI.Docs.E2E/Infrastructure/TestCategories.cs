namespace Vibe.UI.Docs.E2E.Infrastructure;

/// <summary>
/// Test category constants for organizing and filtering E2E tests
/// </summary>
public static class TestCategories
{
    /// <summary>
    /// Smoke tests - Quick validation that critical functionality works
    /// </summary>
    public const string Smoke = "Smoke";

    /// <summary>
    /// Functional tests - Detailed validation of specific features
    /// </summary>
    public const string Functional = "Functional";

    /// <summary>
    /// Integration tests - Validation of component interactions and JS interop
    /// </summary>
    public const string Integration = "Integration";

    /// <summary>
    /// Mobile tests - Tests requiring mobile viewport or responsive behavior
    /// </summary>
    public const string Mobile = "Mobile";

    /// <summary>
    /// Visual tests - Deterministic screenshot comparison against reviewed baselines
    /// </summary>
    public const string Visual = "Visual";

    /// <summary>
    /// Compatibility tests - Browser smoke tests for supported hosting models
    /// </summary>
    public const string Compatibility = "Compatibility";
}
