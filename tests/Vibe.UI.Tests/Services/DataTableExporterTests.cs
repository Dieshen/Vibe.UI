using System.Text;

namespace Vibe.UI.Tests.Services;

public class DataTableExporterTests
{
    [Fact]
    public void ToCsv_EscapesDelimitedQuotedMultilineAndNullValues()
    {
        var items = new[]
        {
            new ExportRow("Alice, Inc.", "Line 1\r\n\"Quoted\"", "=SUM(A1:A2)", -42, null)
        };

        var csv = DataTableExporter.ToCsv(items, CreateColumns());

        var expected = JoinLines(
            "Name,Notes,Formula,Count,Optional",
            "\"Alice, Inc.\",\"Line 1\r\n\"\"Quoted\"\"\",'=SUM(A1:A2),-42,");

        csv.ShouldBe(expected);
    }

    [Fact]
    public void ToCsv_ProtectsSpreadsheetFormulaStringsWithoutChangingNumericValues()
    {
        var items = new[]
        {
            new ExportRow("Alice", "Safe", "@cmd", -42, null)
        };

        var columns = new Dictionary<string, Func<ExportRow, object?>>
        {
            ["=Header"] = item => item.Name,
            ["Formula"] = item => item.Formula,
            ["Count"] = item => item.Count
        };

        var csv = DataTableExporter.ToCsv(items, columns);
        var lines = SplitLines(csv);

        lines[0].ShouldBe("'=Header,Formula,Count");
        lines[1].ShouldBe("Alice,'@cmd,-42");
    }

    [Fact]
    public void ToTsv_ReplacesTabsAndNewlinesPreservesQuotesAndProtectsSpreadsheetFormulas()
    {
        var items = new[]
        {
            new ExportRow("Hello\t\"World\"", "Line 1\r\nLine 2", "=cmd", 5, null)
        };

        var tsv = DataTableExporter.ToTsv(items, CreateColumns());
        var lines = SplitLines(tsv);

        lines[0].ShouldBe("Name\tNotes\tFormula\tCount\tOptional");
        lines[1].ShouldBe("Hello \"World\"\tLine 1 Line 2\t'=cmd\t5\t");
    }

    [Fact]
    public void ToHtml_EncodesHeadersValuesAndTableClass()
    {
        var items = new[]
        {
            new ExportRow("<b>Alice & Bob</b>", null, "Safe", 0, null)
        };

        var columns = new Dictionary<string, Func<ExportRow, object?>>
        {
            ["<script>alert(1)</script>"] = item => item.Name
        };

        var html = DataTableExporter.ToHtml(items, columns, "report\" onclick=\"alert(1)");

        html.ShouldContain("<table class=\"report&quot; onclick=&quot;alert(1)\">");
        html.ShouldContain("<th>&lt;script&gt;alert(1)&lt;/script&gt;</th>");
        html.ShouldContain("<td>&lt;b&gt;Alice &amp; Bob&lt;/b&gt;</td>");
        html.ShouldNotContain("<script>alert(1)</script>");
    }

    [Fact]
    public void ToJson_UsesCamelCaseAndIndentedFormatting()
    {
        var items = new[]
        {
            new JsonRow("Alice", 2)
        };

        var json = DataTableExporter.ToJson(items);

        json.ShouldContain(Environment.NewLine);
        json.ShouldContain("\"displayName\": \"Alice\"");
        json.ShouldContain("\"itemCount\": 2");
    }

    [Fact]
    public void DataUriHelpers_EncodeContentWithExpectedMimeTypes()
    {
        var plainTextUri = DataTableExporter.ToDataUri("hello", "text/plain");
        var csvUri = DataTableExporter.ToCsvDataUri("Name,Value");
        var jsonUri = DataTableExporter.ToJsonDataUri("{}");

        plainTextUri.ShouldStartWith("data:text/plain;base64,");
        csvUri.ShouldStartWith("data:text/csv;charset=utf-8;base64,");
        jsonUri.ShouldStartWith("data:application/json;charset=utf-8;base64,");

        DecodeDataUriContent(plainTextUri).ShouldBe("hello");
        DecodeDataUriContent(csvUri).ShouldBe("Name,Value");
        DecodeDataUriContent(jsonUri).ShouldBe("{}");
    }

    private static Dictionary<string, Func<ExportRow, object?>> CreateColumns()
    {
        return new Dictionary<string, Func<ExportRow, object?>>
        {
            ["Name"] = item => item.Name,
            ["Notes"] = item => item.Notes,
            ["Formula"] = item => item.Formula,
            ["Count"] = item => item.Count,
            ["Optional"] = item => item.Optional
        };
    }

    private static string JoinLines(params string[] lines)
    {
        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    private static string[] SplitLines(string content)
    {
        return content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    private static string DecodeDataUriContent(string uri)
    {
        var encodedContent = uri[(uri.IndexOf(',') + 1)..];
        return Encoding.UTF8.GetString(Convert.FromBase64String(encodedContent));
    }

    private sealed record ExportRow(string Name, string? Notes, string Formula, int Count, string? Optional);

    private sealed record JsonRow(string DisplayName, int ItemCount);
}
