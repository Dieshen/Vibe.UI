namespace Vibe.UI.Services;

/// <summary>
/// Helper class for Lucide icon SVG paths.
/// Contains commonly used icons from the Lucide icon set.
/// </summary>
public static class LucideIcons
{
    public class IconPath
    {
        public string Element { get; set; } = "path";
        public Dictionary<string, string> Attributes { get; set; } = new();
    }

    private static readonly Dictionary<string, List<IconPath>> Icons = new()
    {
        // Navigation & UI
        ["menu"] = new() { new() { Attributes = new() { ["d"] = "M3 12h18M3 6h18M3 18h18" } } },
        ["x"] = new() { new() { Attributes = new() { ["d"] = "M18 6 6 18M6 6l12 12" } } },
        ["chevron-down"] = new() { new() { Attributes = new() { ["d"] = "m6 9 6 6 6-6" } } },
        ["chevron-up"] = new() { new() { Attributes = new() { ["d"] = "m18 15-6-6-6 6" } } },
        ["chevron-left"] = new() { new() { Attributes = new() { ["d"] = "m15 18-6-6 6-6" } } },
        ["chevron-right"] = new() { new() { Attributes = new() { ["d"] = "m9 18 6-6-6-6" } } },
        ["chevrons-up-down"] = new() { new() { Attributes = new() { ["d"] = "m7 15 5 5 5-5M7 9l5-5 5 5" } } },
        ["arrow-left"] = new() { new() { Attributes = new() { ["d"] = "m12 19-7-7 7-7M5 12h14" } } },
        ["arrow-right"] = new() { new() { Attributes = new() { ["d"] = "M5 12h14m-7-7 7 7-7 7" } } },
        ["arrow-up"] = new() { new() { Attributes = new() { ["d"] = "m5 12 7-7 7 7M12 19V5" } } },
        ["arrow-down"] = new() { new() { Attributes = new() { ["d"] = "M12 5v14m-7-7 7 7 7-7" } } },

        // Common Actions
        ["check"] = new() { new() { Attributes = new() { ["d"] = "M20 6 9 17l-5-5" } } },
        ["plus"] = new() { new() { Attributes = new() { ["d"] = "M5 12h14M12 5v14" } } },
        ["minus"] = new() { new() { Attributes = new() { ["d"] = "M5 12h14" } } },
        ["edit"] = new() {
            new() { Attributes = new() { ["d"] = "M21.174 6.812a1 1 0 0 0-3.986-3.987L3.842 16.174a2 2 0 0 0-.5.83l-1.321 4.352a.5.5 0 0 0 .623.622l4.353-1.32a2 2 0 0 0 .83-.497z" } },
            new() { Attributes = new() { ["d"] = "m15 5 4 4" } }
        },
        ["trash"] = new() {
            new() { Attributes = new() { ["d"] = "M3 6h18M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2" } },
            new() { Attributes = new() { ["d"] = "m10 11 0 6" } },
            new() { Attributes = new() { ["d"] = "m14 11 0 6" } }
        },
        ["copy"] = new() {
            new() { Element = "rect", Attributes = new() { ["width"] = "14", ["height"] = "14", ["x"] = "8", ["y"] = "8", ["rx"] = "2", ["ry"] = "2" } },
            new() { Attributes = new() { ["d"] = "M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2" } }
        },
        ["save"] = new() {
            new() { Attributes = new() { ["d"] = "M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z" } },
            new() { Attributes = new() { ["d"] = "M17 21v-8H7v8M7 3v5h8" } }
        },
        ["download"] = new() { new() { Attributes = new() { ["d"] = "M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4M7 10l5 5 5-5M12 15V3" } } },
        ["upload"] = new() { new() { Attributes = new() { ["d"] = "M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4M17 8l-5-5-5 5M12 3v12" } } },
        ["search"] = new() {
            new() { Element = "circle", Attributes = new() { ["cx"] = "11", ["cy"] = "11", ["r"] = "8" } },
            new() { Attributes = new() { ["d"] = "m21 21-4.3-4.3" } }
        },
        ["filter"] = new() { new() { Attributes = new() { ["d"] = "M22 3H2l8 9.46V19l4 2v-8.54z" } } },
        ["refresh-cw"] = new() {
            new() { Attributes = new() { ["d"] = "M3 12a9 9 0 0 1 9-9 9.75 9.75 0 0 1 6.74 2.74L21 8" } },
            new() { Attributes = new() { ["d"] = "M21 3v5h-5M21 12a9 9 0 0 1-9 9 9.75 9.75 0 0 1-6.74-2.74L3 16" } },
            new() { Attributes = new() { ["d"] = "M3 21v-5h5" } }
        },

        // Files & Folders
        ["file"] = new() { new() { Attributes = new() { ["d"] = "M15 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7Z" } }, new() { Attributes = new() { ["d"] = "M14 2v4a2 2 0 0 0 2 2h4" } } },
        ["file-text"] = new() {
            new() { Attributes = new() { ["d"] = "M15 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7Z" } },
            new() { Attributes = new() { ["d"] = "M14 2v4a2 2 0 0 0 2 2h4M10 9H8m8 4H8m8 4H8" } }
        },
        ["folder"] = new() { new() { Attributes = new() { ["d"] = "M20 20a2 2 0 0 0 2-2V8a2 2 0 0 0-2-2h-7.9a2 2 0 0 1-1.69-.9L9.6 3.9A2 2 0 0 0 7.93 3H4a2 2 0 0 0-2 2v13a2 2 0 0 0 2 2Z" } } },
        ["folder-open"] = new() {
            new() { Attributes = new() { ["d"] = "m6 14 1.45-2.9A2 2 0 0 1 9.24 10H20a2 2 0 0 1 1.94 2.5l-1.55 6a2 2 0 0 1-1.94 1.5H4a2 2 0 0 1-2-2V5c0-1.1.9-2 2-2h3.93a2 2 0 0 1 1.66.9l.82 1.2a2 2 0 0 0 1.66.9H18a2 2 0 0 1 2 2v2" } }
        },

        // Communication
        ["mail"] = new() {
            new() { Element = "rect", Attributes = new() { ["width"] = "20", ["height"] = "16", ["x"] = "2", ["y"] = "4", ["rx"] = "2" } },
            new() { Attributes = new() { ["d"] = "m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7" } }
        },
        ["message-circle"] = new() { new() { Attributes = new() { ["d"] = "M7.9 20A9 9 0 1 0 4 16.1L2 22Z" } } },
        ["message-square"] = new() { new() { Attributes = new() { ["d"] = "M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" } } },
        ["phone"] = new() { new() { Attributes = new() { ["d"] = "M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z" } } },
        ["send"] = new() { new() { Attributes = new() { ["d"] = "m22 2-7 20-4-9-9-4ZM22 2 11 13" } } },

        // Media
        ["image"] = new() {
            new() { Element = "rect", Attributes = new() { ["width"] = "18", ["height"] = "18", ["x"] = "3", ["y"] = "3", ["rx"] = "2", ["ry"] = "2" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "9", ["cy"] = "9", ["r"] = "2" } },
            new() { Attributes = new() { ["d"] = "m21 15-3.086-3.086a2 2 0 0 0-2.828 0L6 21" } }
        },
        ["video"] = new() {
            new() { Attributes = new() { ["d"] = "m16 13 5.223 3.482a.5.5 0 0 0 .777-.416V7.87a.5.5 0 0 0-.752-.432L16 10.5" } },
            new() { Element = "rect", Attributes = new() { ["width"] = "14", ["height"] = "12", ["x"] = "2", ["y"] = "6", ["rx"] = "2" } }
        },
        ["music"] = new() { new() { Attributes = new() { ["d"] = "M9 18V5l12-2v13" } }, new() { Element = "circle", Attributes = new() { ["cx"] = "6", ["cy"] = "18", ["r"] = "3" } }, new() { Element = "circle", Attributes = new() { ["cx"] = "18", ["cy"] = "16", ["r"] = "3" } } },
        ["camera"] = new() {
            new() { Attributes = new() { ["d"] = "M14.5 4h-5L7 7H4a2 2 0 0 0-2 2v9a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2V9a2 2 0 0 0-2-2h-3l-2.5-3z" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "13", ["r"] = "3" } }
        },

        // Status & Indicators
        ["alert-circle"] = new() { new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "12", ["r"] = "10" } }, new() { Attributes = new() { ["d"] = "M12 8v4m0 4h.01" } } },
        ["alert-triangle"] = new() { new() { Attributes = new() { ["d"] = "m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3M12 9v4m0 4h.01" } } },
        ["info"] = new() { new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "12", ["r"] = "10" } }, new() { Attributes = new() { ["d"] = "M12 16v-4m0-4h.01" } } },
        ["check-circle"] = new() { new() { Attributes = new() { ["d"] = "M22 11.08V12a10 10 0 1 1-5.93-9.14" } }, new() { Attributes = new() { ["d"] = "m9 11 3 3L22 4" } } },
        ["x-circle"] = new() { new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "12", ["r"] = "10" } }, new() { Attributes = new() { ["d"] = "m15 9-6 6m0-6 6 6" } } },
        ["help-circle"] = new() { new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "12", ["r"] = "10" } }, new() { Attributes = new() { ["d"] = "M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3m.08 4h.01" } } },

        // User & Account
        ["user"] = new() { new() { Attributes = new() { ["d"] = "M19 21v-2a4 4 0 0 0-4-4H9a4 4 0 0 0-4 4v2" } }, new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "7", ["r"] = "4" } } },
        ["users"] = new() {
            new() { Attributes = new() { ["d"] = "M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "9", ["cy"] = "7", ["r"] = "4" } },
            new() { Attributes = new() { ["d"] = "M22 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75" } }
        },
        ["user-plus"] = new() {
            new() { Attributes = new() { ["d"] = "M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "9", ["cy"] = "7", ["r"] = "4" } },
            new() { Attributes = new() { ["d"] = "M22 11h-6m3-3v6" } }
        },
        ["user-minus"] = new() {
            new() { Attributes = new() { ["d"] = "M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "9", ["cy"] = "7", ["r"] = "4" } },
            new() { Attributes = new() { ["d"] = "M22 11h-6" } }
        },

        // Settings & Configuration
        ["settings"] = new() {
            new() { Attributes = new() { ["d"] = "M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "12", ["r"] = "3" } }
        },
        ["sliders"] = new() { new() { Attributes = new() { ["d"] = "M4 21v-7m0-4V3m8 18v-9m0-4V3m8 18v-5m0-4V3M1 14h6m2-6h6m2 8h6" } } },

        // Misc
        ["home"] = new() { new() { Attributes = new() { ["d"] = "m3 9 9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" } }, new() { Attributes = new() { ["d"] = "M9 22V12h6v10" } } },
        ["heart"] = new() { new() { Attributes = new() { ["d"] = "M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.3 1.5 4.05 3 5.5l7 7Z" } } },
        ["star"] = new() { new() { Attributes = new() { ["d"] = "M11.525 2.295a.53.53 0 0 1 .95 0l2.31 4.679a2.123 2.123 0 0 0 1.595 1.16l5.166.756a.53.53 0 0 1 .294.904l-3.736 3.638a2.123 2.123 0 0 0-.611 1.878l.882 5.14a.53.53 0 0 1-.771.56l-4.618-2.428a2.122 2.122 0 0 0-1.973 0L6.396 21.01a.53.53 0 0 1-.77-.56l.881-5.139a2.122 2.122 0 0 0-.611-1.879L2.16 9.795a.53.53 0 0 1 .294-.906l5.165-.755a2.122 2.122 0 0 0 1.597-1.16z" } } },
        ["calendar"] = new() {
            new() { Element = "rect", Attributes = new() { ["width"] = "18", ["height"] = "18", ["x"] = "3", ["y"] = "4", ["rx"] = "2", ["ry"] = "2" } },
            new() { Attributes = new() { ["d"] = "M16 2v4M8 2v4M3 10h18" } }
        },
        ["clock"] = new() { new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "12", ["r"] = "10" } }, new() { Attributes = new() { ["d"] = "M12 6v6l4 2" } } },
        ["bell"] = new() { new() { Attributes = new() { ["d"] = "M6 8a6 6 0 0 1 12 0c0 7 3 9 3 9H3s3-2 3-9M10.3 21a1.94 1.94 0 0 0 3.4 0" } } },
        ["lock"] = new() {
            new() { Element = "rect", Attributes = new() { ["width"] = "18", ["height"] = "11", ["x"] = "3", ["y"] = "11", ["rx"] = "2", ["ry"] = "2" } },
            new() { Attributes = new() { ["d"] = "M7 11V7a5 5 0 0 1 10 0v4" } }
        },
        ["unlock"] = new() {
            new() { Element = "rect", Attributes = new() { ["width"] = "18", ["height"] = "11", ["x"] = "3", ["y"] = "11", ["rx"] = "2", ["ry"] = "2" } },
            new() { Attributes = new() { ["d"] = "M7 11V7a5 5 0 0 1 9.9-1" } }
        },
        ["eye"] = new() {
            new() { Attributes = new() { ["d"] = "M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "12", ["r"] = "3" } }
        },
        ["eye-off"] = new() {
            new() { Attributes = new() { ["d"] = "M9.88 9.88a3 3 0 1 0 4.24 4.24M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68" } },
            new() { Attributes = new() { ["d"] = "M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61M2 2l20 20" } }
        },
        ["loader"] = new() { new() { Attributes = new() { ["d"] = "M12 2v4m0 12v4M4.93 4.93l2.83 2.83m8.48 8.48 2.83 2.83M2 12h4m12 0h4M4.93 19.07l2.83-2.83m8.48-8.48 2.83-2.83" } } },
        ["more-horizontal"] = new() {
            new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "12", ["r"] = "1" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "19", ["cy"] = "12", ["r"] = "1" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "5", ["cy"] = "12", ["r"] = "1" } }
        },
        ["more-vertical"] = new() {
            new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "12", ["r"] = "1" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "5", ["r"] = "1" } },
            new() { Element = "circle", Attributes = new() { ["cx"] = "12", ["cy"] = "19", ["r"] = "1" } }
        },
    };

    /// <summary>
    /// Gets the SVG paths for a given icon name.
    /// </summary>
    public static List<IconPath>? GetIcon(string name)
    {
        var key = name.ToLowerInvariant();
        return Icons.TryGetValue(key, out var paths) ? paths : null;
    }

    /// <summary>
    /// Gets all available icon names.
    /// </summary>
    public static IEnumerable<string> GetAllIconNames()
    {
        return Icons.Keys.OrderBy(k => k);
    }

    /// <summary>
    /// Checks if an icon exists.
    /// </summary>
    public static bool IconExists(string name)
    {
        return Icons.ContainsKey(name.ToLowerInvariant());
    }
}
