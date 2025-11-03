namespace Vibe.UI.Docs.Services;

public class CodeFormattingService
{
    public string FormatCode(string code, string language)
    {
        // Basic code formatting - can be enhanced with more sophisticated formatting
        if (string.IsNullOrWhiteSpace(code))
            return string.Empty;

        // Remove extra leading/trailing whitespace
        code = code.Trim();

        // Normalize line endings
        code = code.Replace("\r\n", "\n");

        return code;
    }

    public string EscapeHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        return html
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }
}
