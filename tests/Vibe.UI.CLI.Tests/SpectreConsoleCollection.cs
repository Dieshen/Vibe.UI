using Xunit;

namespace Vibe.UI.CLI.Tests;

/// <summary>
/// Test collection for tests that use Spectre.Console interactive features.
/// Tests in this collection will not run in parallel to avoid concurrency issues
/// with Spectre.Console's global exclusivity lock for interactive operations.
/// </summary>
[CollectionDefinition("SpectreConsole")]
public class SpectreConsoleCollection
{
    // This class is never instantiated. It's just a marker for the collection.
}
