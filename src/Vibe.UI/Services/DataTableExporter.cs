using System.Text;

namespace Vibe.UI.Services;

/// <summary>
/// Utility for exporting DataTable data to various formats.
/// </summary>
public static class DataTableExporter
{
    /// <summary>
    /// Exports data to CSV format.
    /// </summary>
    public static string ToCsv<T>(IEnumerable<T> items, Dictionary<string, Func<T, object?>>? columns = null)
    {
        var itemsList = items.ToList();
        if (!itemsList.Any())
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        // Get column definitions
        Dictionary<string, Func<T, object?>> columnDefs;

        if (columns != null)
        {
            columnDefs = columns;
        }
        else
        {
            // Use all public properties
            var props = typeof(T).GetProperties();
            columnDefs = props.ToDictionary(
                p => p.Name,
                p => (Func<T, object?>)(item => p.GetValue(item))
            );
        }

        // Write headers
        sb.AppendLine(string.Join(",", columnDefs.Keys.Select(EscapeCsvValue)));

        // Write rows
        foreach (var item in itemsList)
        {
            var values = columnDefs.Values.Select(func =>
            {
                var value = func(item);
                return EscapeCsvValue(value?.ToString() ?? string.Empty);
            });

            sb.AppendLine(string.Join(",", values));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Exports data to TSV (Tab-Separated Values) format.
    /// </summary>
    public static string ToTsv<T>(IEnumerable<T> items, Dictionary<string, Func<T, object?>>? columns = null)
    {
        var itemsList = items.ToList();
        if (!itemsList.Any())
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        // Get column definitions
        Dictionary<string, Func<T, object?>> columnDefs;

        if (columns != null)
        {
            columnDefs = columns;
        }
        else
        {
            var props = typeof(T).GetProperties();
            columnDefs = props.ToDictionary(
                p => p.Name,
                p => (Func<T, object?>)(item => p.GetValue(item))
            );
        }

        // Write headers
        sb.AppendLine(string.Join("\t", columnDefs.Keys));

        // Write rows
        foreach (var item in itemsList)
        {
            var values = columnDefs.Values.Select(func =>
            {
                var value = func(item);
                return (value?.ToString() ?? string.Empty).Replace("\t", " ");
            });

            sb.AppendLine(string.Join("\t", values));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Exports data to JSON format.
    /// </summary>
    public static string ToJson<T>(IEnumerable<T> items)
    {
        return System.Text.Json.JsonSerializer.Serialize(items, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
    }

    /// <summary>
    /// Exports data to HTML table format.
    /// </summary>
    public static string ToHtml<T>(IEnumerable<T> items, Dictionary<string, Func<T, object?>>? columns = null, string? tableClass = null)
    {
        var itemsList = items.ToList();
        if (!itemsList.Any())
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        // Get column definitions
        Dictionary<string, Func<T, object?>> columnDefs;

        if (columns != null)
        {
            columnDefs = columns;
        }
        else
        {
            var props = typeof(T).GetProperties();
            columnDefs = props.ToDictionary(
                p => p.Name,
                p => (Func<T, object?>)(item => p.GetValue(item))
            );
        }

        // Start table
        sb.Append("<table");
        if (!string.IsNullOrEmpty(tableClass))
        {
            sb.Append($" class=\"{tableClass}\"");
        }
        sb.AppendLine(">");

        // Write headers
        sb.AppendLine("  <thead>");
        sb.AppendLine("    <tr>");
        foreach (var header in columnDefs.Keys)
        {
            sb.AppendLine($"      <th>{EscapeHtml(header)}</th>");
        }
        sb.AppendLine("    </tr>");
        sb.AppendLine("  </thead>");

        // Write body
        sb.AppendLine("  <tbody>");
        foreach (var item in itemsList)
        {
            sb.AppendLine("    <tr>");
            foreach (var func in columnDefs.Values)
            {
                var value = func(item);
                sb.AppendLine($"      <td>{EscapeHtml(value?.ToString() ?? string.Empty)}</td>");
            }
            sb.AppendLine("    </tr>");
        }
        sb.AppendLine("  </tbody>");

        sb.AppendLine("</table>");

        return sb.ToString();
    }

    /// <summary>
    /// Generates a data URI for downloading the exported data.
    /// </summary>
    public static string ToDataUri(string content, string mimeType)
    {
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
        return $"data:{mimeType};base64,{base64}";
    }

    /// <summary>
    /// Generates a CSV data URI for downloading.
    /// </summary>
    public static string ToCsvDataUri(string csvContent)
    {
        return ToDataUri(csvContent, "text/csv;charset=utf-8");
    }

    /// <summary>
    /// Generates a JSON data URI for downloading.
    /// </summary>
    public static string ToJsonDataUri(string jsonContent)
    {
        return ToDataUri(jsonContent, "application/json;charset=utf-8");
    }

    private static string EscapeCsvValue(string value)
    {
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }

    private static string EscapeHtml(string value)
    {
        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }
}
