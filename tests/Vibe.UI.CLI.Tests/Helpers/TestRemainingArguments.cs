using Spectre.Console.Cli;

namespace Vibe.UI.CLI.Tests.Helpers;

/// <summary>
/// Mock implementation of IRemainingArguments for testing
/// </summary>
public class TestRemainingArguments : IRemainingArguments
{
    private readonly string[] _raw;
    private readonly Dictionary<string, string[]> _parsed;

    public TestRemainingArguments()
        : this(Array.Empty<string>())
    {
    }

    public TestRemainingArguments(string[] raw)
        : this(raw, new Dictionary<string, string[]>())
    {
    }

    public TestRemainingArguments(string[] raw, Dictionary<string, string[]> parsed)
    {
        _raw = raw ?? Array.Empty<string>();
        _parsed = parsed ?? new Dictionary<string, string[]>();
    }

    public int Count => _raw.Length;

    public ILookup<string, string?> Parsed => _parsed
        .SelectMany(pair => pair.Value.Select(value => new { pair.Key, Value = value }))
        .ToLookup(x => x.Key, x => (string?)x.Value);

    public IReadOnlyList<string> Raw => _raw;
}
