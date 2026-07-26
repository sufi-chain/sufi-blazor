using System.Collections.Frozen;

namespace SufiChain.SufiBlazor.Icons;

/// <summary>
/// Sufi Icons (si) registry containing all built-in icons with metadata.
/// Provides lookup by name, category filtering, and search functionality.
/// </summary>
public static class SufiIcons
{
    /// <summary>
    /// Standard SVG attributes for outline-style icons.
    /// </summary>
    private const string SvgOutlineAttrs = "viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"";

    /// <summary>
    /// Standard SVG attributes for solid/filled icons.
    /// </summary>
    private const string SvgSolidAttrs = "viewBox=\"0 0 24 24\" fill=\"currentColor\"";

    private static readonly FrozenDictionary<string, SiIconMetadata> _icons;
    private static readonly FrozenDictionary<SiIconCategory, SiIconMetadata[]> _byCategory;

    static SufiIcons()
    {
        var icons = CreateIconRegistry();
        _icons = icons.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        _byCategory = icons.Values
            .GroupBy(i => i.Category)
            .ToFrozenDictionary(g => g.Key, g => g.ToArray());
    }

    /// <summary>
    /// Gets all available icons.
    /// </summary>
    public static IReadOnlyCollection<SiIconMetadata> All => _icons.Values;

    /// <summary>
    /// Gets the total count of available icons.
    /// </summary>
    public static int Count => _icons.Count;

    /// <summary>
    /// Gets all available categories.
    /// </summary>
    public static IReadOnlyCollection<SiIconCategory> Categories => _byCategory.Keys;

    /// <summary>
    /// Gets an icon by name. Returns null if not found.
    /// </summary>
    /// <param name="name">The icon name (case-insensitive). Supports both "home" and "si-home" formats.</param>
    /// <returns>The icon metadata or null if not found.</returns>
    public static SiIconMetadata? GetIcon(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        // Support both "home" and "si-home" formats
        var normalizedName = name.StartsWith("si-", StringComparison.OrdinalIgnoreCase)
            ? name[3..]
            : name;

        return _icons.GetValueOrDefault(normalizedName);
    }

    /// <summary>
    /// Gets the SVG content for an icon by name. Returns empty string if not found.
    /// </summary>
    /// <param name="name">The icon name (case-insensitive).</param>
    /// <returns>The SVG markup string or empty string if not found.</returns>
    public static string GetSvg(string? name)
    {
        return GetIcon(name)?.Svg ?? string.Empty;
    }

    /// <summary>
    /// Gets all icons in a specific category.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <returns>Collection of icons in the category.</returns>
    public static IReadOnlyCollection<SiIconMetadata> GetByCategory(SiIconCategory category)
    {
        return _byCategory.GetValueOrDefault(category) ?? [];
    }

    /// <summary>
    /// Searches icons by name or description.
    /// </summary>
    /// <param name="query">The search query (case-insensitive).</param>
    /// <returns>Collection of matching icons.</returns>
    public static IReadOnlyCollection<SiIconMetadata> Search(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return All.ToArray();
        }

        var q = query.Trim();
        return _icons.Values
            .Where(i => i.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                        i.Description.Contains(q, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    /// <summary>
    /// Searches icons by name, description, or category.
    /// </summary>
    /// <param name="query">The search query (case-insensitive).</param>
    /// <param name="category">Optional category filter.</param>
    /// <returns>Collection of matching icons.</returns>
    public static IReadOnlyCollection<SiIconMetadata> Search(string? query, SiIconCategory? category)
    {
        var results = category.HasValue
            ? GetByCategory(category.Value)
            : All;

        if (string.IsNullOrWhiteSpace(query))
        {
            return results.ToArray();
        }

        var q = query.Trim();
        return results
            .Where(i => i.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                        i.Description.Contains(q, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    /// <summary>
    /// Checks if an icon exists by name.
    /// </summary>
    /// <param name="name">The icon name to check.</param>
    /// <returns>True if the icon exists.</returns>
    public static bool Exists(string? name)
    {
        return GetIcon(name) != null;
    }

    private static Dictionary<string, SiIconMetadata> CreateIconRegistry()
    {
        return new Dictionary<string, SiIconMetadata>(StringComparer.OrdinalIgnoreCase)
        {
            // ============================================
            // Navigation Icons
            // ============================================
            ["chevron-down"] = new("chevron-down", SiIconCategory.Navigation, "Downward chevron indicator",
                $"<svg {SvgOutlineAttrs}><path d=\"M6 9l6 6 6-6\"/></svg>"),
            ["chevron-up"] = new("chevron-up", SiIconCategory.Navigation, "Upward chevron indicator",
                $"<svg {SvgOutlineAttrs}><path d=\"M18 15l-6-6-6 6\"/></svg>"),
            ["chevron-left"] = new("chevron-left", SiIconCategory.Navigation, "Left chevron indicator",
                $"<svg {SvgOutlineAttrs}><path d=\"M15 18l-6-6 6-6\"/></svg>"),
            ["chevron-right"] = new("chevron-right", SiIconCategory.Navigation, "Right chevron indicator",
                $"<svg {SvgOutlineAttrs}><path d=\"M9 18l6-6-6-6\"/></svg>"),
            ["chevrons-left"] = new("chevrons-left", SiIconCategory.Navigation, "Double left chevron",
                $"<svg {SvgOutlineAttrs}><polyline points=\"11 17 6 12 11 7\"/><polyline points=\"18 17 13 12 18 7\"/></svg>"),
            ["chevrons-right"] = new("chevrons-right", SiIconCategory.Navigation, "Double right chevron",
                $"<svg {SvgOutlineAttrs}><polyline points=\"13 17 18 12 13 7\"/><polyline points=\"6 17 11 12 6 7\"/></svg>"),
            ["arrow-up"] = new("arrow-up", SiIconCategory.Navigation, "Arrow pointing up",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 19V5M5 12l7-7 7 7\"/></svg>"),
            ["arrow-down"] = new("arrow-down", SiIconCategory.Navigation, "Arrow pointing down",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 5v14M19 12l-7 7-7-7\"/></svg>"),
            ["arrow-left"] = new("arrow-left", SiIconCategory.Navigation, "Arrow pointing left",
                $"<svg {SvgOutlineAttrs}><path d=\"M19 12H5M12 19l-7-7 7-7\"/></svg>"),
            ["arrow-right"] = new("arrow-right", SiIconCategory.Navigation, "Arrow pointing right",
                $"<svg {SvgOutlineAttrs}><path d=\"M5 12h14M12 5l7 7-7 7\"/></svg>"),
            ["home"] = new("home", SiIconCategory.Navigation, "Home or dashboard",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z\"/><path d=\"M9 22V12h6v10\"/></svg>"),
            ["menu"] = new("menu", SiIconCategory.Navigation, "Main menu hamburger",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 12h18M3 6h18M3 18h18\"/></svg>"),
            ["corner-up-left"] = new("corner-up-left", SiIconCategory.Navigation, "Corner arrow up-left",
                $"<svg {SvgOutlineAttrs}><polyline points=\"9 14 4 9 9 4\"/><path d=\"M20 20v-7a4 4 0 00-4-4H4\"/></svg>"),
            ["corner-up-right"] = new("corner-up-right", SiIconCategory.Navigation, "Corner arrow up-right",
                $"<svg {SvgOutlineAttrs}><polyline points=\"15 14 20 9 15 4\"/><path d=\"M4 20v-7a4 4 0 014-4h12\"/></svg>"),
            ["corner-down-left"] = new("corner-down-left", SiIconCategory.Navigation, "Corner arrow down-left",
                $"<svg {SvgOutlineAttrs}><polyline points=\"9 10 4 15 9 20\"/><path d=\"M20 4v7a4 4 0 01-4 4H4\"/></svg>"),
            ["corner-down-right"] = new("corner-down-right", SiIconCategory.Navigation, "Corner arrow down-right",
                $"<svg {SvgOutlineAttrs}><polyline points=\"15 10 20 15 15 20\"/><path d=\"M4 4v7a4 4 0 004 4h12\"/></svg>"),
            ["move"] = new("move", SiIconCategory.Navigation, "Move or drag indicator",
                $"<svg {SvgOutlineAttrs}><polyline points=\"5 9 2 12 5 15\"/><polyline points=\"9 5 12 2 15 5\"/><polyline points=\"15 19 12 22 9 19\"/><polyline points=\"19 9 22 12 19 15\"/><line x1=\"2\" y1=\"12\" x2=\"22\" y2=\"12\"/><line x1=\"12\" y1=\"2\" x2=\"12\" y2=\"22\"/></svg>"),

            // ============================================
            // Action Icons
            // ============================================
            ["plus"] = new("plus", SiIconCategory.Actions, "Add or create action",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 5v14M5 12h14\"/></svg>"),
            ["minus"] = new("minus", SiIconCategory.Actions, "Remove or subtract action",
                $"<svg {SvgOutlineAttrs}><path d=\"M5 12h14\"/></svg>"),
            ["x"] = new("x", SiIconCategory.Actions, "Close or cancel",
                $"<svg {SvgOutlineAttrs}><path d=\"M18 6L6 18M6 6l12 12\"/></svg>"),
            ["check"] = new("check", SiIconCategory.Actions, "Confirm or success",
                $"<svg {SvgOutlineAttrs}><path d=\"M20 6L9 17l-5-5\"/></svg>"),
            ["refresh"] = new("refresh", SiIconCategory.Actions, "Reload or sync",
                $"<svg {SvgOutlineAttrs}><path d=\"M23 4v6h-6M1 20v-6h6\"/><path d=\"M3.51 9a9 9 0 0114.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0020.49 15\"/></svg>"),
            ["search"] = new("search", SiIconCategory.Actions, "Search or find",
                $"<svg {SvgOutlineAttrs}><circle cx=\"11\" cy=\"11\" r=\"8\"/><path d=\"M21 21l-4.35-4.35\"/></svg>"),
            ["edit"] = new("edit", SiIconCategory.Actions, "Edit or modify",
                $"<svg {SvgOutlineAttrs}><path d=\"M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7\"/><path d=\"M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z\"/></svg>"),
            ["trash"] = new("trash", SiIconCategory.Actions, "Delete item",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 6h18M19 6v14a2 2 0 01-2 2H7a2 2 0 01-2-2V6m3 0V4a2 2 0 012-2h4a2 2 0 012 2v2\"/></svg>"),
            ["copy"] = new("copy", SiIconCategory.Actions, "Duplicate or copy",
                $"<svg {SvgOutlineAttrs}><rect x=\"9\" y=\"9\" width=\"13\" height=\"13\" rx=\"2\"/><path d=\"M5 15H4a2 2 0 01-2-2V4a2 2 0 012-2h9a2 2 0 012 2v1\"/></svg>"),
            ["save"] = new("save", SiIconCategory.Actions, "Save changes",
                $"<svg {SvgOutlineAttrs}><path d=\"M19 21H5a2 2 0 01-2-2V5a2 2 0 012-2h11l5 5v11a2 2 0 01-2 2z\"/><polyline points=\"17 21 17 13 7 13 7 21\"/><polyline points=\"7 3 7 8 15 8\"/></svg>"),
            ["filter"] = new("filter", SiIconCategory.Actions, "Filter results",
                $"<svg {SvgOutlineAttrs}><polygon points=\"22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3\"/></svg>"),
            ["sort"] = new("sort", SiIconCategory.Actions, "Sort items",
                $"<svg {SvgOutlineAttrs}><path d=\"M11 5h10M11 9h7M11 13h4M3 17l4 4 4-4M7 3v18\"/></svg>"),
            ["sort-asc"] = new("sort-asc", SiIconCategory.Actions, "Sort ascending",
                $"<svg {SvgOutlineAttrs}><path d=\"M11 12h4M11 16h7M11 20h10M3 8l4-4 4 4M7 4v16\"/></svg>"),
            ["sort-desc"] = new("sort-desc", SiIconCategory.Actions, "Sort descending",
                $"<svg {SvgOutlineAttrs}><path d=\"M11 5h10M11 9h7M11 12h4M3 16l4 4 4-4M7 20V4\"/></svg>"),
            ["sort-none"] = new("sort-none", SiIconCategory.Actions, "No sorting state",
                $"<svg {SvgOutlineAttrs}><path d=\"M8 9l4-4 4 4M16 15l-4 4-4-4\"/></svg>"),
            ["more-vertical"] = new("more-vertical", SiIconCategory.Actions, "More options vertical",
                $"<svg {SvgSolidAttrs}><circle cx=\"12\" cy=\"12\" r=\"1.5\"/><circle cx=\"12\" cy=\"6\" r=\"1.5\"/><circle cx=\"12\" cy=\"18\" r=\"1.5\"/></svg>"),
            ["more-horizontal"] = new("more-horizontal", SiIconCategory.Actions, "More options horizontal",
                $"<svg {SvgSolidAttrs}><circle cx=\"12\" cy=\"12\" r=\"1.5\"/><circle cx=\"6\" cy=\"12\" r=\"1.5\"/><circle cx=\"18\" cy=\"12\" r=\"1.5\"/></svg>"),
            ["undo"] = new("undo", SiIconCategory.Actions, "Undo action",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 7v6h6\"/><path d=\"M21 17a9 9 0 00-9-9 9 9 0 00-6 2.3L3 13\"/></svg>"),
            ["redo"] = new("redo", SiIconCategory.Actions, "Redo action",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 7v6h-6\"/><path d=\"M3 17a9 9 0 019-9 9 9 0 016 2.3l3 2.7\"/></svg>"),
            ["clipboard"] = new("clipboard", SiIconCategory.Actions, "Clipboard",
                $"<svg {SvgOutlineAttrs}><path d=\"M16 4h2a2 2 0 012 2v14a2 2 0 01-2 2H6a2 2 0 01-2-2V6a2 2 0 012-2h2\"/><rect x=\"8\" y=\"2\" width=\"8\" height=\"4\" rx=\"1\" ry=\"1\"/></svg>"),
            ["clipboard-check"] = new("clipboard-check", SiIconCategory.Actions, "Checklist completed",
                $"<svg {SvgOutlineAttrs}><path d=\"M16 4h2a2 2 0 012 2v14a2 2 0 01-2 2H6a2 2 0 01-2-2V6a2 2 0 012-2h2\"/><rect x=\"8\" y=\"2\" width=\"8\" height=\"4\" rx=\"1\" ry=\"1\"/><path d=\"M9 14l2 2 4-4\"/></svg>"),
            ["clipboard-list"] = new("clipboard-list", SiIconCategory.Actions, "Checklist items",
                $"<svg {SvgOutlineAttrs}><path d=\"M16 4h2a2 2 0 012 2v14a2 2 0 01-2 2H6a2 2 0 01-2-2V6a2 2 0 012-2h2\"/><rect x=\"8\" y=\"2\" width=\"8\" height=\"4\" rx=\"1\" ry=\"1\"/><path d=\"M12 11h4M12 16h4M8 11h.01M8 16h.01\"/></svg>"),
            // SufiForms product mark: fillable form sheet with checkboxes + input field
            // (distinct from clipboard-list — no clip; reads clearly at icon-rail size).
            ["form"] = new("form", SiIconCategory.Actions, "SufiForms — form definitions, surveys, and fillable fields",
                $"<svg {SvgOutlineAttrs}>" +
                "<rect x=\"4\" y=\"2\" width=\"16\" height=\"20\" rx=\"2\"/>" +
                "<rect x=\"7\" y=\"6\" width=\"3\" height=\"3\" rx=\"0.5\"/>" +
                "<path d=\"M12 7.5h5\"/>" +
                "<rect x=\"7\" y=\"11\" width=\"3\" height=\"3\" rx=\"0.5\"/>" +
                "<path d=\"M12 12.5h5\"/>" +
                "<rect x=\"7\" y=\"16\" width=\"10\" height=\"3\" rx=\"0.5\"/>" +
                "</svg>"),
            ["cut"] = new("cut", SiIconCategory.Actions, "Cut or SufiChainsor action",
                $"<svg {SvgOutlineAttrs}><circle cx=\"6\" cy=\"6\" r=\"3\"/><circle cx=\"6\" cy=\"18\" r=\"3\"/><line x1=\"20\" y1=\"4\" x2=\"8.12\" y2=\"15.88\"/><line x1=\"14.47\" y1=\"14.48\" x2=\"20\" y2=\"20\"/><line x1=\"8.12\" y1=\"8.12\" x2=\"12\" y2=\"12\"/></svg>"),
            ["paste"] = new("paste", SiIconCategory.Actions, "Paste from clipboard",
                $"<svg {SvgOutlineAttrs}><path d=\"M15 2H9a1 1 0 00-1 1v2c0 .6.4 1 1 1h6c.6 0 1-.4 1-1V3c0-.6-.4-1-1-1z\"/><path d=\"M8 4H6a2 2 0 00-2 2v14a2 2 0 002 2h12a2 2 0 002-2M16 4h2a2 2 0 012 2v2\"/><path d=\"M21 14H11\"/><path d=\"M15 10l-4 4 4 4\"/></svg>"),
            ["drag"] = new("drag", SiIconCategory.Actions, "Drag handle",
                $"<svg {SvgOutlineAttrs}><circle cx=\"9\" cy=\"5\" r=\"1\"/><circle cx=\"9\" cy=\"12\" r=\"1\"/><circle cx=\"9\" cy=\"19\" r=\"1\"/><circle cx=\"15\" cy=\"5\" r=\"1\"/><circle cx=\"15\" cy=\"12\" r=\"1\"/><circle cx=\"15\" cy=\"19\" r=\"1\"/></svg>"),
            ["thumbtack"] = new("thumbtack", SiIconCategory.Actions, "Thumbtack or pin to board",
                $"<svg {SvgOutlineAttrs}><line x1=\"12\" y1=\"17\" x2=\"12\" y2=\"22\"/><path d=\"M5 17h14v-1.76a2 2 0 00-1.11-1.79l-1.78-.9A2 2 0 0115 10.76V6h1a2 2 0 000-4H8a2 2 0 000 4h1v4.76a2 2 0 01-1.11 1.79l-1.78.9A2 2 0 005 15.24z\"/></svg>"),
            ["unpin"] = new("unpin", SiIconCategory.Actions, "Unpin or detach",
                $"<svg {SvgOutlineAttrs}><line x1=\"2\" y1=\"2\" x2=\"22\" y2=\"22\"/><line x1=\"12\" y1=\"17\" x2=\"12\" y2=\"22\"/><path d=\"M9 9v1.76a2 2 0 01-1.11 1.79l-1.78.9A2 2 0 005 15.24V17h12\"/><path d=\"M15 9.34V6h1a2 2 0 000-4H7.89\"/></svg>"),
            ["zoom-in"] = new("zoom-in", SiIconCategory.Actions, "Zoom in magnify",
                $"<svg {SvgOutlineAttrs}><circle cx=\"11\" cy=\"11\" r=\"8\"/><line x1=\"21\" y1=\"21\" x2=\"16.65\" y2=\"16.65\"/><line x1=\"11\" y1=\"8\" x2=\"11\" y2=\"14\"/><line x1=\"8\" y1=\"11\" x2=\"14\" y2=\"11\"/></svg>"),
            ["zoom-out"] = new("zoom-out", SiIconCategory.Actions, "Zoom out shrink",
                $"<svg {SvgOutlineAttrs}><circle cx=\"11\" cy=\"11\" r=\"8\"/><line x1=\"21\" y1=\"21\" x2=\"16.65\" y2=\"16.65\"/><line x1=\"8\" y1=\"11\" x2=\"14\" y2=\"11\"/></svg>"),
            ["rotate"] = new("rotate", SiIconCategory.Actions, "Rotate or spin",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 2v6h-6\"/><path d=\"M3 12a9 9 0 0115-6.7L21 8\"/><path d=\"M3 22v-6h6\"/><path d=\"M21 12a9 9 0 01-15 6.7L3 16\"/></svg>"),
            ["rotate-cw"] = new("rotate-cw", SiIconCategory.Actions, "Rotate clockwise (right)",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 2v6h-6\"/><path d=\"M3 12a9 9 0 0115-6.7L21 8\"/><path d=\"M3 22v-6h6\"/><path d=\"M21 12a9 9 0 01-15 6.7L3 16\"/></svg>"),
            ["rotate-ccw"] = new("rotate-ccw", SiIconCategory.Actions, "Rotate counter-clockwise (left)",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 22v-6h6\"/><path d=\"M21 12a9 9 0 01-15 6.7L3 16\"/><path d=\"M21 2v6h-6\"/><path d=\"M3 12a9 9 0 0115-6.7L21 8\"/></svg>"),
            ["rotate-right"] = new("rotate-right", SiIconCategory.Actions, "Rotate right (clockwise circular arrow)",
                $"<svg {SvgOutlineAttrs}><path d=\"M5.5 18.5A9.5 9.5 0 0119 11\"/><polyline points=\"16 14 19 11 16 8\"/></svg>"),
            ["rotate-left"] = new("rotate-left", SiIconCategory.Actions, "Rotate left (counter-clockwise circular arrow)",
                $"<svg {SvgOutlineAttrs}><path d=\"M18.5 5.5A9.5 9.5 0 015 13\"/><polyline points=\"8 10 5 13 8 16\"/></svg>"),
            ["flip-horizontal"] = new("flip-horizontal", SiIconCategory.Actions, "Flip horizontally",
                $"<svg {SvgOutlineAttrs}><path d=\"M8 3H5a2 2 0 00-2 2v14c0 1.1.9 2 2 2h3\"/><path d=\"M16 3h3a2 2 0 012 2v14a2 2 0 01-2 2h-3\"/><path d=\"M12 20v2\"/><path d=\"M12 14v2\"/><path d=\"M12 8v2\"/><path d=\"M12 2v2\"/></svg>"),
            ["flip-vertical"] = new("flip-vertical", SiIconCategory.Actions, "Flip vertically (horizontal dashed line, top/bottom panels)",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 8V5a2 2 0 012-2h14a2 2 0 012 2v3a2 2 0 01-2 2H5a2 2 0 00-2-2z\"/><path d=\"M3 16v3a2 2 0 002 2h14a2 2 0 002-2v-3a2 2 0 00-2-2H5a2 2 0 010-2z\"/><path d=\"M2 12h2\"/><path d=\"M8 12h2\"/><path d=\"M14 12h2\"/><path d=\"M20 12h2\"/></svg>"),
            ["expand"] = new("expand", SiIconCategory.Actions, "Expand or show full",
                $"<svg {SvgOutlineAttrs}><path d=\"M15 3h6v6\"/><path d=\"M9 21H3v-6\"/><path d=\"M21 3l-7 7\"/><path d=\"M3 21l7-7\"/></svg>"),

            // ============================================
            // Media Icons
            // ============================================
            ["upload"] = new("upload", SiIconCategory.Media, "Upload content",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 15v4a2 2 0 01-2 2H5a2 2 0 01-2-2v-4M17 8l-5-5-5 5M12 3v12\"/></svg>"),
            ["download"] = new("download", SiIconCategory.Media, "Download content",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 15v4a2 2 0 01-2 2H5a2 2 0 01-2-2v-4M7 10l5 5 5-5M12 15V3\"/></svg>"),
            ["image"] = new("image", SiIconCategory.Media, "Single image",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\"/><circle cx=\"8.5\" cy=\"8.5\" r=\"1.5\"/><path d=\"M21 15l-5-5L5 21\"/></svg>"),
            ["images"] = new("images", SiIconCategory.Media, "Image gallery",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\"/><circle cx=\"8.5\" cy=\"8.5\" r=\"1.5\"/><path d=\"M21 15l-5-5L5 21\"/></svg>"),
            ["video"] = new("video", SiIconCategory.Media, "Video or media player",
                $"<svg {SvgOutlineAttrs}><polygon points=\"23 7 16 12 23 17 23 7\"/><rect x=\"1\" y=\"5\" width=\"15\" height=\"14\" rx=\"2\" ry=\"2\"/></svg>"),
            ["music"] = new("music", SiIconCategory.Media, "Music or audio",
                $"<svg {SvgOutlineAttrs}><path d=\"M9 18V5l12-2v13\"/><circle cx=\"6\" cy=\"18\" r=\"3\"/><circle cx=\"18\" cy=\"16\" r=\"3\"/></svg>"),
            ["microphone"] = new("microphone", SiIconCategory.Media, "Voice input or recording",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 1a3 3 0 00-3 3v8a3 3 0 006 0V4a3 3 0 00-3-3z\"/><path d=\"M19 10v2a7 7 0 01-14 0v-2\"/><line x1=\"12\" y1=\"19\" x2=\"12\" y2=\"23\"/><line x1=\"8\" y1=\"23\" x2=\"16\" y2=\"23\"/></svg>"),
            ["camera"] = new("camera", SiIconCategory.Media, "Camera or capture",
                $"<svg {SvgOutlineAttrs}><path d=\"M23 19a2 2 0 01-2 2H3a2 2 0 01-2-2V8a2 2 0 012-2h4l2-3h6l2 3h4a2 2 0 012 2z\"/><circle cx=\"12\" cy=\"13\" r=\"4\"/></svg>"),
            ["play"] = new("play", SiIconCategory.Media, "Play action",
                $"<svg {SvgOutlineAttrs}><polygon points=\"5 3 19 12 5 21 5 3\"/></svg>"),
            ["pause"] = new("pause", SiIconCategory.Media, "Pause action",
                $"<svg {SvgOutlineAttrs}><rect x=\"6\" y=\"4\" width=\"4\" height=\"16\"/><rect x=\"14\" y=\"4\" width=\"4\" height=\"16\"/></svg>"),
            ["volume"] = new("volume", SiIconCategory.Media, "Audio volume medium",
                $"<svg {SvgOutlineAttrs}><polygon points=\"11 5 6 9 2 9 2 15 6 15 11 19 11 5\"/><path d=\"M15.54 8.46a5 5 0 010 7.07\"/></svg>"),
            ["volume-off"] = new("volume-off", SiIconCategory.Media, "Audio muted",
                $"<svg {SvgOutlineAttrs}><polygon points=\"11 5 6 9 2 9 2 15 6 15 11 19 11 5\"/><line x1=\"23\" y1=\"9\" x2=\"17\" y2=\"15\"/><line x1=\"17\" y1=\"9\" x2=\"23\" y2=\"15\"/></svg>"),
            ["volume-low"] = new("volume-low", SiIconCategory.Media, "Audio volume low",
                $"<svg {SvgOutlineAttrs}><polygon points=\"11 5 6 9 2 9 2 15 6 15 11 19 11 5\"/><path d=\"M15.54 8.46a5 5 0 010 7.07\"/></svg>"),
            ["volume-high"] = new("volume-high", SiIconCategory.Media, "Audio volume high",
                $"<svg {SvgOutlineAttrs}><polygon points=\"11 5 6 9 2 9 2 15 6 15 11 19 11 5\"/><path d=\"M19.07 4.93a10 10 0 010 14.14M15.54 8.46a5 5 0 010 7.07\"/></svg>"),
            ["stop"] = new("stop", SiIconCategory.Media, "Stop playback",
                $"<svg {SvgOutlineAttrs}><rect x=\"6\" y=\"6\" width=\"12\" height=\"12\"/></svg>"),
            ["skip-forward"] = new("skip-forward", SiIconCategory.Media, "Skip to next",
                $"<svg {SvgOutlineAttrs}><polygon points=\"5 4 15 12 5 20 5 4\"/><line x1=\"19\" y1=\"5\" x2=\"19\" y2=\"19\"/></svg>"),

            // ============================================
            // File Icons
            // ============================================
            ["file"] = new("file", SiIconCategory.Files, "Generic file (any type or unknown format)",
                $"<svg {SvgOutlineAttrs}><path d=\"M13 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V9z\"/><path d=\"M13 2v7h7\"/></svg>"),
            ["file-text"] = new("file-text", SiIconCategory.Files, "Text document (.txt)",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><polyline points=\"14 2 14 8 20 8\"/><line x1=\"16\" y1=\"13\" x2=\"8\" y2=\"13\"/><line x1=\"16\" y1=\"17\" x2=\"8\" y2=\"17\"/><polyline points=\"10 9 9 9 8 9\"/></svg>"),
            ["file-pdf"] = new("file-pdf", SiIconCategory.Files, "PDF document (.pdf)",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v7h7\"/><path d=\"M9 12v4\"/><path d=\"M12 12v4\"/><path d=\"M15 12h2a2 2 0 012 2v2a2 2 0 01-2 2h-2\"/></svg>"),
            ["file-doc"] = new("file-doc", SiIconCategory.Files, "Word document (.doc, .docx)",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M8 12h8\"/><path d=\"M8 16h8\"/><path d=\"M8 8h4\"/></svg>"),
            ["file-excel"] = new("file-excel", SiIconCategory.Files, "Excel spreadsheet (.xls, .xlsx)",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M8 10h8\"/><path d=\"M8 14h8\"/><path d=\"M8 18h5\"/><path d=\"M12 10v8\"/></svg>"),
            ["file-csv"] = new("file-csv", SiIconCategory.Files, "CSV / spreadsheet data (.csv)",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M8 13h2\"/><path d=\"M8 17h2\"/><path d=\"M14 13h2\"/><path d=\"M14 17h2\"/></svg>"),
            ["file-ppt"] = new("file-ppt", SiIconCategory.Files, "PowerPoint presentation (.ppt, .pptx)",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><rect x=\"8\" y=\"12\" width=\"8\" height=\"6\" rx=\"1\"/><path d=\"M12 12v6\"/></svg>"),
            ["file-json"] = new("file-json", SiIconCategory.Files, "JSON file (.json)",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M10 12h1\"/><path d=\"M13 12h1\"/><path d=\"M9 16c0 .5.5 1 1.5 1s1.5-.5 1.5-1\"/><path d=\"M9 8c0 .5.5 1 1.5 1s1.5-.5 1.5-1\"/></svg>"),
            ["file-xml"] = new("file-xml", SiIconCategory.Files, "XML file (.xml)",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M10 11l2 2-2 2\"/><path d=\"M14 11l2 2-2 2\"/><path d=\"M12 9v8\"/></svg>"),
            ["folder"] = new("folder", SiIconCategory.Files, "Folder closed",
                $"<svg {SvgOutlineAttrs}><path d=\"M22 19a2 2 0 01-2 2H4a2 2 0 01-2-2V5a2 2 0 012-2h5l2 3h9a2 2 0 012 2z\"/></svg>"),
            ["folder-open"] = new("folder-open", SiIconCategory.Files, "Folder open",
                $"<svg {SvgOutlineAttrs}><path d=\"M22 19a2 2 0 01-2 2H4a2 2 0 01-2-2V5a2 2 0 012-2h5l2 3h9a2 2 0 012 2v1\"/><path d=\"M2 10h20l-2 9H4l-2-9z\"/></svg>"),
            ["folder-tree"] = new("folder-tree", SiIconCategory.Files, "Folder tree structure",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 3v18h18\"/><rect x=\"7\" y=\"6\" width=\"6\" height=\"4\" rx=\"1\"/><rect x=\"7\" y=\"14\" width=\"6\" height=\"4\" rx=\"1\"/><rect x=\"15\" y=\"10\" width=\"6\" height=\"4\" rx=\"1\"/><path d=\"M10 10v4M13 12h2\"/></svg>"),
            ["folder-plus"] = new("folder-plus", SiIconCategory.Files, "Add folder or sub-folder",
                $"<svg {SvgOutlineAttrs}><path d=\"M22 19a2 2 0 01-2 2H4a2 2 0 01-2-2V5a2 2 0 012-2h5l2 3h9a2 2 0 012 2z\"/><line x1=\"12\" y1=\"11\" x2=\"12\" y2=\"17\"/><line x1=\"9\" y1=\"14\" x2=\"15\" y2=\"14\"/></svg>"),
            ["cloud"] = new("cloud", SiIconCategory.Files, "Cloud storage",
                $"<svg {SvgOutlineAttrs}><path d=\"M18 10h-1.26A8 8 0 109 20h9a5 5 0 000-10z\"/></svg>"),
            ["cloud-upload"] = new("cloud-upload", SiIconCategory.Files, "Upload to cloud",
                $"<svg {SvgOutlineAttrs}><path d=\"M16 16l-4-4-4 4\"/><path d=\"M12 12v9\"/><path d=\"M20.39 18.39A5 5 0 0018 9h-1.26A8 8 0 103 16.3\"/></svg>"),
            ["cloud-download"] = new("cloud-download", SiIconCategory.Files, "Download from cloud",
                $"<svg {SvgOutlineAttrs}><path d=\"M8 17l4 4 4-4\"/><path d=\"M12 12v9\"/><path d=\"M20.88 18.09A5 5 0 0018 9h-1.26A8 8 0 103 16.29\"/></svg>"),
            ["archive"] = new("archive", SiIconCategory.Files, "Archive storage",
                $"<svg {SvgOutlineAttrs}><polyline points=\"21 8 21 21 3 21 3 8\"/><rect x=\"1\" y=\"3\" width=\"22\" height=\"5\"/><line x1=\"10\" y1=\"12\" x2=\"14\" y2=\"12\"/></svg>"),
            ["bookmark"] = new("bookmark", SiIconCategory.Files, "Bookmark or save",
                $"<svg {SvgOutlineAttrs}><path d=\"M19 21l-7-5-7 5V5a2 2 0 012-2h10a2 2 0 012 2z\"/></svg>"),
            ["tag"] = new("tag", SiIconCategory.Files, "Tag or label",
                $"<svg {SvgOutlineAttrs}><path d=\"M20.59 13.41l-7.17 7.17a2 2 0 01-2.83 0L2 12V2h10l8.59 8.59a2 2 0 010 2.82z\"/><line x1=\"7\" y1=\"7\" x2=\"7.01\" y2=\"7\"/></svg>"),
            ["flag"] = new("flag", SiIconCategory.Files, "Flag or mark",
                $"<svg {SvgOutlineAttrs}><path d=\"M4 15s1-1 4-1 5 2 8 2 4-1 4-1V3s-1 1-4 1-5-2-8-2-4 1-4 1z\"/><line x1=\"4\" y1=\"22\" x2=\"4\" y2=\"15\"/></svg>"),
            ["file-code"] = new("file-code", SiIconCategory.Files, "Code file",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M10 13l-2 2 2 2\"/><path d=\"M14 13l2 2-2 2\"/></svg>"),
            ["file-image"] = new("file-image", SiIconCategory.Files, "Image file",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><circle cx=\"10\" cy=\"13\" r=\"2\"/><path d=\"M20 17l-1.09-1.09a2 2 0 00-2.82 0L10 22\"/></svg>"),
            ["file-audio"] = new("file-audio", SiIconCategory.Files, "Audio file",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><circle cx=\"10\" cy=\"16\" r=\"2\"/><path d=\"M12 12v4\"/></svg>"),
            ["file-video"] = new("file-video", SiIconCategory.Files, "Video file",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M10 11l5 3-5 3z\"/></svg>"),
            ["file-archive"] = new("file-archive", SiIconCategory.Files, "Archive or compressed file",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M10 12h4\"/><path d=\"M10 16h4\"/><path d=\"M10 20h4\"/></svg>"),

            // ============================================
            // User Icons
            // ============================================
            ["user"] = new("user", SiIconCategory.Users, "Single user",
                $"<svg {SvgOutlineAttrs}><path d=\"M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2\"/><circle cx=\"12\" cy=\"7\" r=\"4\"/></svg>"),
            ["users"] = new("users", SiIconCategory.Users, "Multiple users",
                $"<svg {SvgOutlineAttrs}><path d=\"M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2\"/><circle cx=\"9\" cy=\"7\" r=\"4\"/><path d=\"M23 21v-2a4 4 0 00-3-3.87\"/><path d=\"M16 3.13a4 4 0 010 7.75\"/></svg>"),
            ["user-plus"] = new("user-plus", SiIconCategory.Users, "Add user",
                $"<svg {SvgOutlineAttrs}><path d=\"M16 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2\"/><circle cx=\"8.5\" cy=\"7\" r=\"4\"/><line x1=\"20\" y1=\"8\" x2=\"20\" y2=\"14\"/><line x1=\"23\" y1=\"11\" x2=\"17\" y2=\"11\"/></svg>"),
            ["user-minus"] = new("user-minus", SiIconCategory.Users, "Remove user",
                $"<svg {SvgOutlineAttrs}><path d=\"M16 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2\"/><circle cx=\"8.5\" cy=\"7\" r=\"4\"/><line x1=\"23\" y1=\"11\" x2=\"17\" y2=\"11\"/></svg>"),
            ["id-card"] = new("id-card", SiIconCategory.Users, "User identity card",
                $"<svg {SvgOutlineAttrs}><rect x=\"2\" y=\"5\" width=\"20\" height=\"14\" rx=\"2\"/><line x1=\"2\" y1=\"10\" x2=\"22\" y2=\"10\"/></svg>"),
            ["contact"] = new("contact", SiIconCategory.Users, "Contact or address book entry",
                $"<svg {SvgOutlineAttrs}><path d=\"M17 18a2 2 0 00-2-2H9a2 2 0 00-2 2\"/><rect x=\"3\" y=\"4\" width=\"18\" height=\"18\" rx=\"2\"/><circle cx=\"12\" cy=\"10\" r=\"2\"/><path d=\"M8 2v2M16 2v2\"/></svg>"),
            ["user-check"] = new("user-check", SiIconCategory.Users, "Verified user",
                $"<svg {SvgOutlineAttrs}><path d=\"M16 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2\"/><circle cx=\"8.5\" cy=\"7\" r=\"4\"/><polyline points=\"17 11 19 13 23 9\"/></svg>"),
            ["user-x"] = new("user-x", SiIconCategory.Users, "Remove or blocked user",
                $"<svg {SvgOutlineAttrs}><path d=\"M16 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2\"/><circle cx=\"8.5\" cy=\"7\" r=\"4\"/><line x1=\"18\" y1=\"8\" x2=\"23\" y2=\"13\"/><line x1=\"23\" y1=\"8\" x2=\"18\" y2=\"13\"/></svg>"),
            ["users-group"] = new("users-group", SiIconCategory.Users, "Group of users",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 19a6 6 0 00-12 0\"/><circle cx=\"8\" cy=\"9\" r=\"4\"/><path d=\"M22 19a6 6 0 00-6-6 4 4 0 100-8\"/></svg>"),
            ["user-cog"] = new("user-cog", SiIconCategory.Users, "User settings",
                $"<svg {SvgOutlineAttrs}><path d=\"M16 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2\"/><circle cx=\"8.5\" cy=\"7\" r=\"4\"/><circle cx=\"19\" cy=\"11\" r=\"2\"/><path d=\"M19 8v1\"/><path d=\"M19 13v1\"/><path d=\"M16.5 9.5l.9.4\"/><path d=\"M20.7 12.1l.9.4\"/><path d=\"M16.5 12.5l.9-.4\"/><path d=\"M20.7 9.9l.9-.4\"/></svg>"),
            ["avatar"] = new("avatar", SiIconCategory.Users, "User avatar",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><circle cx=\"12\" cy=\"10\" r=\"3\"/><path d=\"M7 20.662V19a2 2 0 012-2h6a2 2 0 012 2v1.662\"/></svg>"),

            // ============================================
            // Security Icons
            // ============================================
            ["lock"] = new("lock", SiIconCategory.Security, "Locked or secure",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"11\" width=\"18\" height=\"11\" rx=\"2\" ry=\"2\"/><path d=\"M7 11V7a5 5 0 0110 0v4\"/></svg>"),
            ["unlock"] = new("unlock", SiIconCategory.Security, "Unlocked state",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"11\" width=\"18\" height=\"11\" rx=\"2\" ry=\"2\"/><path d=\"M7 11V7a5 5 0 019.9-1\"/></svg>"),
            ["shield"] = new("shield", SiIconCategory.Security, "Protection or security",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z\"/></svg>"),
            ["shield-check"] = new("shield-check", SiIconCategory.Security, "Verified security",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z\"/><path d=\"M9 12l2 2 4-4\"/></svg>"),
            ["shield-alert"] = new("shield-alert", SiIconCategory.Security, "Security alert",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z\"/><path d=\"M12 8v4M12 16h.01\"/></svg>"),
            ["key"] = new("key", SiIconCategory.Security, "Access key",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 2l-2 2m-7.61 7.61a5.5 5.5 0 11-7.778 7.778 5.5 5.5 0 017.777-7.777zm0 0L15.5 7.5m0 0l3 3L22 7l-3-3m-3.5 3.5L19 4\"/></svg>"),
            ["eye"] = new("eye", SiIconCategory.Security, "Visible state",
                $"<svg {SvgOutlineAttrs}><path d=\"M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z\"/><circle cx=\"12\" cy=\"12\" r=\"3\"/></svg>"),
            ["eye-off"] = new("eye-off", SiIconCategory.Security, "Hidden state",
                $"<svg {SvgOutlineAttrs}><path d=\"M17.94 17.94A10.07 10.07 0 0112 20c-7 0-11-8-11-8a18.45 18.45 0 015.06-5.94M9.9 4.24A9.12 9.12 0 0112 4c7 0 11 8 11 8a18.5 18.5 0 01-2.16 3.19m-6.72-1.07a3 3 0 11-4.24-4.24\"/><path d=\"M1 1l22 22\"/></svg>"),
            ["fingerprint"] = new("fingerprint", SiIconCategory.Security, "Biometric fingerprint",
                $"<svg {SvgOutlineAttrs}><path d=\"M2 12C2 6.5 6.5 2 12 2a10 10 0 018 4\"/><path d=\"M5 19.5C5.5 18 6 15 6 12c0-.7.12-1.37.34-2\"/><path d=\"M17.29 21.02c.12-.6.43-2.3.5-3.02\"/><path d=\"M12 10a2 2 0 00-2 2c0 1.02-.1 2.51-.26 4\"/><path d=\"M8.65 22c.21-.66.45-1.32.57-2\"/><path d=\"M14 13.12c0 2.38 0 6.38-1 8.88\"/><path d=\"M2 16h.01\"/><path d=\"M21.8 16c.2-2 .131-5.354 0-6\"/><path d=\"M9 6.8a6 6 0 019 5.2c0 .47 0 1.17-.02 2\"/></svg>"),
            ["log-out"] = new("log-out", SiIconCategory.Security, "Sign out",
                $"<svg {SvgOutlineAttrs}><path d=\"M9 21H5a2 2 0 01-2-2V5a2 2 0 012-2h4\"/><polyline points=\"16 17 21 12 16 7\"/><line x1=\"21\" y1=\"12\" x2=\"9\" y2=\"12\"/></svg>"),
            ["shield-off"] = new("shield-off", SiIconCategory.Security, "Disabled security",
                $"<svg {SvgOutlineAttrs}><path d=\"M19.69 14a6.9 6.9 0 00.31-2V5l-8-3-3.16 1.18\"/><path d=\"M4.73 4.73L4 5v7c0 6 8 10 8 10a20.29 20.29 0 005.62-4.38\"/><line x1=\"1\" y1=\"1\" x2=\"23\" y2=\"23\"/></svg>"),
            ["scan-face"] = new("scan-face", SiIconCategory.Security, "Face recognition scan",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 7V5a2 2 0 012-2h2\"/><path d=\"M17 3h2a2 2 0 012 2v2\"/><path d=\"M21 17v2a2 2 0 01-2 2h-2\"/><path d=\"M7 21H5a2 2 0 01-2-2v-2\"/><circle cx=\"12\" cy=\"12\" r=\"4\"/><path d=\"M12 8v0\"/></svg>"),
            ["two-factor"] = new("two-factor", SiIconCategory.Security, "Two-factor authentication",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"11\" width=\"18\" height=\"11\" rx=\"2\" ry=\"2\"/><path d=\"M7 11V7a5 5 0 0110 0v4\"/><circle cx=\"12\" cy=\"16\" r=\"1\"/></svg>"),
            ["vault"] = new("vault", SiIconCategory.Security, "Secure vault",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\"/><circle cx=\"12\" cy=\"12\" r=\"4\"/><path d=\"M12 8v8\"/><path d=\"M8 12h8\"/></svg>"),
            ["incognito"] = new("incognito", SiIconCategory.Security, "Private or incognito mode",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 12.5a3.5 3.5 0 100-7 3.5 3.5 0 000 7z\"/><path d=\"M5.5 12.5a3.5 3.5 0 100-7\"/><path d=\"M18.5 12.5a3.5 3.5 0 100-7\"/><path d=\"M3 21v-1a7 7 0 0114 0v1\"/></svg>"),

            // ============================================
            // System Icons
            // ============================================
            ["demo"] = new("demo", SiIconCategory.System, "Demo or showcase application",
                $"<svg {SvgOutlineAttrs}><path d=\"M9 2h6v5l3 11H6l3-11V2z\"/></svg>"),
            ["settings"] = new("settings", SiIconCategory.System, "Settings or configuration",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"3\"/><path d=\"M19.4 15a1.65 1.65 0 00.33 1.82l.06.06a2 2 0 010 2.83 2 2 0 01-2.83 0l-.06-.06a1.65 1.65 0 00-1.82-.33 1.65 1.65 0 00-1 1.51V21a2 2 0 01-2 2 2 2 0 01-2-2v-.09A1.65 1.65 0 009 19.4a1.65 1.65 0 00-1.82.33l-.06.06a2 2 0 01-2.83 0 2 2 0 010-2.83l.06-.06a1.65 1.65 0 00.33-1.82 1.65 1.65 0 00-1.51-1H3a2 2 0 01-2-2 2 2 0 012-2h.09A1.65 1.65 0 004.6 9a1.65 1.65 0 00-.33-1.82l-.06-.06a2 2 0 010-2.83 2 2 0 012.83 0l.06.06a1.65 1.65 0 001.82.33H9a1.65 1.65 0 001-1.51V3a2 2 0 012-2 2 2 0 012 2v.09a1.65 1.65 0 001 1.51 1.65 1.65 0 001.82-.33l.06-.06a2 2 0 012.83 0 2 2 0 010 2.83l-.06.06a1.65 1.65 0 00-.33 1.82V9a1.65 1.65 0 001.51 1H21a2 2 0 012 2 2 2 0 01-2 2h-.09a1.65 1.65 0 00-1.51 1z\"/></svg>"),
            ["sliders"] = new("sliders", SiIconCategory.System, "Adjust controls",
                $"<svg {SvgOutlineAttrs}><line x1=\"4\" y1=\"21\" x2=\"4\" y2=\"14\"/><line x1=\"4\" y1=\"10\" x2=\"4\" y2=\"3\"/><line x1=\"12\" y1=\"21\" x2=\"12\" y2=\"12\"/><line x1=\"12\" y1=\"8\" x2=\"12\" y2=\"3\"/><line x1=\"20\" y1=\"21\" x2=\"20\" y2=\"16\"/><line x1=\"20\" y1=\"12\" x2=\"20\" y2=\"3\"/><line x1=\"1\" y1=\"14\" x2=\"7\" y2=\"14\"/><line x1=\"9\" y1=\"8\" x2=\"15\" y2=\"8\"/><line x1=\"17\" y1=\"16\" x2=\"23\" y2=\"16\"/></svg>"),
            ["bell"] = new("bell", SiIconCategory.System, "Notifications",
                $"<svg {SvgOutlineAttrs}><path d=\"M18 8A6 6 0 006 8c0 7-3 9-3 9h18s-3-2-3-9\"/><path d=\"M13.73 21a2 2 0 01-3.46 0\"/></svg>"),
            ["info"] = new("info", SiIconCategory.System, "Information",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M12 16v-4M12 8h.01\"/></svg>"),
            ["warning"] = new("warning", SiIconCategory.System, "Warning alert",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 9v4M12 17h.01\"/><path d=\"M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z\"/></svg>"),
            ["error"] = new("error", SiIconCategory.System, "Error state",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M15 9l-6 6M9 9l6 6\"/></svg>"),
            ["success"] = new("success", SiIconCategory.System, "Success state",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M9 12l2 2 4-4\"/></svg>"),
            ["help-circle"] = new("help-circle", SiIconCategory.System, "Help or support",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M9.09 9a3 3 0 015.83 1c0 2-3 3-3 3\"/><line x1=\"12\" y1=\"17\" x2=\"12.01\" y2=\"17\"/></svg>"),
            ["life-buoy"] = new("life-buoy", SiIconCategory.System, "Support lifebuoy",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><circle cx=\"12\" cy=\"12\" r=\"4\"/><line x1=\"4.93\" y1=\"4.93\" x2=\"9.17\" y2=\"9.17\"/><line x1=\"14.83\" y1=\"14.83\" x2=\"19.07\" y2=\"19.07\"/><line x1=\"14.83\" y1=\"9.17\" x2=\"19.07\" y2=\"4.93\"/><line x1=\"14.83\" y1=\"9.17\" x2=\"18.36\" y2=\"5.64\"/><line x1=\"4.93\" y1=\"19.07\" x2=\"9.17\" y2=\"14.83\"/></svg>"),
            ["layers"] = new("layers", SiIconCategory.System, "Layered content",
                $"<svg {SvgOutlineAttrs}><polygon points=\"12 2 2 7 12 12 22 7 12 2\"/><polyline points=\"2 17 12 22 22 17\"/><polyline points=\"2 12 12 17 22 12\"/></svg>"),
            ["moon"] = new("moon", SiIconCategory.System, "Dark mode or night",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 12.79A9 9 0 1111.21 3 7 7 0 0021 12.79z\"/></svg>"),
            ["sun"] = new("sun", SiIconCategory.System, "Light mode or day",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"5\"/><line x1=\"12\" y1=\"1\" x2=\"12\" y2=\"3\"/><line x1=\"12\" y1=\"21\" x2=\"12\" y2=\"23\"/><line x1=\"4.22\" y1=\"4.22\" x2=\"5.64\" y2=\"5.64\"/><line x1=\"18.36\" y1=\"18.36\" x2=\"19.78\" y2=\"19.78\"/><line x1=\"1\" y1=\"12\" x2=\"3\" y2=\"12\"/><line x1=\"21\" y1=\"12\" x2=\"23\" y2=\"12\"/><line x1=\"4.22\" y1=\"19.78\" x2=\"5.64\" y2=\"18.36\"/><line x1=\"18.36\" y1=\"5.64\" x2=\"19.78\" y2=\"4.22\"/></svg>"),

            // ============================================
            // Chart Icons
            // ============================================
            ["chart-bar"] = new("chart-bar", SiIconCategory.Charts, "Bar chart",
                $"<svg {SvgOutlineAttrs}><line x1=\"12\" y1=\"20\" x2=\"12\" y2=\"10\"/><line x1=\"18\" y1=\"20\" x2=\"18\" y2=\"4\"/><line x1=\"6\" y1=\"20\" x2=\"6\" y2=\"16\"/></svg>"),
            ["bar-chart"] = new("bar-chart", SiIconCategory.Charts, "Bar chart (alias)",
                $"<svg {SvgOutlineAttrs}><line x1=\"12\" y1=\"20\" x2=\"12\" y2=\"10\"/><line x1=\"18\" y1=\"20\" x2=\"18\" y2=\"4\"/><line x1=\"6\" y1=\"20\" x2=\"6\" y2=\"16\"/></svg>"),
            ["chart-line"] = new("chart-line", SiIconCategory.Charts, "Line chart",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 3v18h18\"/><path d=\"M18.7 8l-5.1 5.2-2.8-2.7L7 14.3\"/></svg>"),
            ["chart-pie"] = new("chart-pie", SiIconCategory.Charts, "Pie chart",
                $"<svg {SvgOutlineAttrs}><path d=\"M21.21 15.89A10 10 0 118 2.83\"/><path d=\"M22 12A10 10 0 0012 2v10z\"/></svg>"),
            ["trend-up"] = new("trend-up", SiIconCategory.Charts, "Upward trend",
                $"<svg {SvgOutlineAttrs}><polyline points=\"23 6 13.5 15.5 8.5 10.5 1 18\"/><polyline points=\"17 6 23 6 23 12\"/></svg>"),
            ["trend-down"] = new("trend-down", SiIconCategory.Charts, "Downward trend",
                $"<svg {SvgOutlineAttrs}><polyline points=\"23 18 13.5 8.5 8.5 13.5 1 6\"/><polyline points=\"17 18 23 18 23 12\"/></svg>"),
            ["dollar-sign"] = new("dollar-sign", SiIconCategory.Charts, "Currency dollar",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 1v22M17 5H9.5a3.5 3.5 0 000 7h5a3.5 3.5 0 010 7H6\"/></svg>"),

            // ============================================
            // Time Icons
            // ============================================
            ["calendar"] = new("calendar", SiIconCategory.Time, "Calendar date",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"4\" width=\"18\" height=\"18\" rx=\"2\"/><path d=\"M16 2v4M8 2v4M3 10h18\"/></svg>"),
            ["clock"] = new("clock", SiIconCategory.Time, "Time or duration",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M12 6v6l4 2\"/></svg>"),
            ["timer"] = new("timer", SiIconCategory.Time, "Timer or countdown",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"13\" r=\"8\"/><path d=\"M12 9v4l2 2\"/><path d=\"M5 3L2 6\"/><path d=\"M22 6l-3-3\"/><path d=\"M12 5V2\"/></svg>"),
            ["calendar-check"] = new("calendar-check", SiIconCategory.Time, "Calendar with check",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"4\" width=\"18\" height=\"18\" rx=\"2\"/><path d=\"M16 2v4M8 2v4M3 10h18\"/><path d=\"M9 16l2 2 4-4\"/></svg>"),
            ["calendar-x"] = new("calendar-x", SiIconCategory.Time, "Calendar with cancel",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"4\" width=\"18\" height=\"18\" rx=\"2\"/><path d=\"M16 2v4M8 2v4M3 10h18\"/><path d=\"M10 14l4 4M14 14l-4 4\"/></svg>"),
            ["calendar-days"] = new("calendar-days", SiIconCategory.Time, "Calendar with day grid",
                $"<svg {SvgOutlineAttrs}><path d=\"M8 2v4M16 2v4\"/><rect x=\"3\" y=\"4\" width=\"18\" height=\"18\" rx=\"2\"/><path d=\"M3 10h18\"/><path d=\"M8 14h.01M12 14h.01M16 14h.01M8 18h.01M12 18h.01M16 18h.01\"/></svg>"),
            ["repeat"] = new("repeat", SiIconCategory.Time, "Repeat or loop",
                $"<svg {SvgOutlineAttrs}><polyline points=\"17 1 21 5 17 9\"/><path d=\"M3 11V9a4 4 0 014-4h14\"/><polyline points=\"7 23 3 19 7 15\"/><path d=\"M21 13v2a4 4 0 01-4 4H3\"/></svg>"),

            // ============================================
            // Communication Icons
            // ============================================
            ["mail"] = new("mail", SiIconCategory.Communication, "Email message",
                $"<svg {SvgOutlineAttrs}><path d=\"M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z\"/><polyline points=\"22,6 12,13 2,6\"/></svg>"),
            ["chat"] = new("chat", SiIconCategory.Communication, "Chat or message",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 15a2 2 0 01-2 2H7l-4 4V5a2 2 0 012-2h14a2 2 0 012 2z\"/></svg>"),
            ["send"] = new("send", SiIconCategory.Communication, "Send message",
                $"<svg {SvgOutlineAttrs}><line x1=\"22\" y1=\"2\" x2=\"11\" y2=\"13\"/><polygon points=\"22 2 15 22 11 13 2 9 22 2\"/></svg>"),
            ["phone"] = new("phone", SiIconCategory.Communication, "Phone call",
                $"<svg {SvgOutlineAttrs}><path d=\"M22 16.92v3a2 2 0 01-2.18 2 19.79 19.79 0 01-8.63-3.07 19.5 19.5 0 01-6-6 19.79 19.79 0 01-3.07-8.67A2 2 0 014.11 2h3a2 2 0 012 1.72 12.84 12.84 0 00.7 2.81 2 2 0 01-.45 2.11L8.09 9.91a16 16 0 006 6l1.27-1.27a2 2 0 012.11-.45 12.84 12.84 0 002.81.7A2 2 0 0122 16.92z\"/></svg>"),
            ["link"] = new("link", SiIconCategory.Communication, "Link or attach URL",
                $"<svg {SvgOutlineAttrs}><path d=\"M10 13a5 5 0 007.54.54l3-3a5 5 0 00-7.07-7.07l-1.72 1.71\"/><path d=\"M14 11a5 5 0 00-7.54-.54l-3 3a5 5 0 007.07 7.07l1.71-1.71\"/></svg>"),
            ["external-link"] = new("external-link", SiIconCategory.Communication, "Open external link",
                $"<svg {SvgOutlineAttrs}><path d=\"M18 13v6a2 2 0 01-2 2H5a2 2 0 01-2-2V8a2 2 0 012-2h6\"/><polyline points=\"15 3 21 3 21 9\"/><line x1=\"10\" y1=\"14\" x2=\"21\" y2=\"3\"/></svg>"),
            ["paperclip"] = new("paperclip", SiIconCategory.Communication, "Attachment",
                $"<svg {SvgOutlineAttrs}><path d=\"M21.44 11.05l-9.19 9.19a6 6 0 01-8.49-8.49l9.19-9.19a4 4 0 015.66 5.66l-9.2 9.19a2 2 0 01-2.83-2.83l8.49-8.48\"/></svg>"),
            ["share"] = new("share", SiIconCategory.Communication, "Share item",
                $"<svg {SvgOutlineAttrs}><circle cx=\"18\" cy=\"5\" r=\"3\"/><circle cx=\"6\" cy=\"12\" r=\"3\"/><circle cx=\"18\" cy=\"19\" r=\"3\"/><line x1=\"8.59\" y1=\"13.51\" x2=\"15.42\" y2=\"17.49\"/><line x1=\"15.41\" y1=\"6.51\" x2=\"8.59\" y2=\"10.49\"/></svg>"),
            ["message-square"] = new("message-square", SiIconCategory.Communication, "Message bubble square",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 15a2 2 0 01-2 2H7l-4 4V5a2 2 0 012-2h14a2 2 0 012 2z\"/></svg>"),
            ["message-circle"] = new("message-circle", SiIconCategory.Communication, "Message bubble round",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 11.5a8.38 8.38 0 01-.9 3.8 8.5 8.5 0 01-7.6 4.7 8.38 8.38 0 01-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 01-.9-3.8 8.5 8.5 0 014.7-7.6 8.38 8.38 0 013.8-.9h.5a8.48 8.48 0 018 8v.5z\"/></svg>"),
            ["chat-dots"] = new("chat-dots", SiIconCategory.Communication, "Chat with typing indicator",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 15a2 2 0 01-2 2H7l-4 4V5a2 2 0 012-2h14a2 2 0 012 2z\"/><circle cx=\"8\" cy=\"10\" r=\"1\" fill=\"currentColor\" stroke=\"none\"/><circle cx=\"12\" cy=\"10\" r=\"1\" fill=\"currentColor\" stroke=\"none\"/><circle cx=\"16\" cy=\"10\" r=\"1\" fill=\"currentColor\" stroke=\"none\"/></svg>"),
            ["reply"] = new("reply", SiIconCategory.Communication, "Reply to message",
                $"<svg {SvgOutlineAttrs}><polyline points=\"9 17 4 12 9 7\"/><path d=\"M20 18v-2a4 4 0 00-4-4H4\"/></svg>"),
            ["forward"] = new("forward", SiIconCategory.Communication, "Forward message",
                $"<svg {SvgOutlineAttrs}><polyline points=\"15 17 20 12 15 7\"/><path d=\"M4 18v-2a4 4 0 014-4h12\"/></svg>"),
            ["inbox"] = new("inbox", SiIconCategory.Communication, "Inbox tray",
                $"<svg {SvgOutlineAttrs}><polyline points=\"22 12 16 12 14 15 10 15 8 12 2 12\"/><path d=\"M5.45 5.11L2 12v6a2 2 0 002 2h16a2 2 0 002-2v-6l-3.45-6.89A2 2 0 0016.76 4H7.24a2 2 0 00-1.79 1.11z\"/></svg>"),
            ["outbox"] = new("outbox", SiIconCategory.Communication, "Outbox tray",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 3v9\"/><path d=\"M8 8l4 4 4-4\"/><path d=\"M3 12v6a2 2 0 002 2h14a2 2 0 002-2v-6\"/><path d=\"M5.45 18.89L2 12v-6a2 2 0 012-2h2.76a2 2 0 001.79 1.11L12 8h4.45\"/></svg>"),
            // SufiCom product mark: dual conversations + multi-channel signal arcs
            // (messaging, chat, SMS/email/voice/Telegram channels — not a plain mail envelope).
            ["sufi-com"] = new("sufi-com", SiIconCategory.Communication, "SufiCom messaging, chat, and channels",
                $"<svg {SvgOutlineAttrs}>" +
                "<path d=\"M9 3h8a2 2 0 012 2v6a2 2 0 01-2 2h-1\"/>" +
                "<path d=\"M4 7h10a2 2 0 012 2v6a2 2 0 01-2 2H10l-3.5 2.5V17H4a2 2 0 01-2-2V9a2 2 0 012-2z\"/>" +
                "<path d=\"M7 11h5M7 14h3\"/>" +
                "<path d=\"M18.5 13a3.5 3.5 0 010 5\"/>" +
                "<path d=\"M21 11a6 6 0 010 9\"/>" +
                "</svg>"),

            // ============================================
            // Location Icons
            // ============================================
            ["map"] = new("map", SiIconCategory.Location, "Map or location",
                $"<svg {SvgOutlineAttrs}><polygon points=\"1 6 1 22 8 18 16 22 23 18 23 2 16 6 8 2 1 6\"/><line x1=\"8\" y1=\"2\" x2=\"8\" y2=\"18\"/><line x1=\"16\" y1=\"6\" x2=\"16\" y2=\"22\"/></svg>"),
            ["pin"] = new("pin", SiIconCategory.Location, "Location pin",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0118 0z\"/><circle cx=\"12\" cy=\"10\" r=\"3\"/></svg>"),
            ["globe"] = new("globe", SiIconCategory.Location, "Global or world",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M2 12h20M12 2a15.3 15.3 0 014 10 15.3 15.3 0 01-4 10 15.3 15.3 0 01-4-10 15.3 15.3 0 014-10z\"/></svg>"),
            ["building"] = new("building", SiIconCategory.Location, "Building or office",
                $"<svg {SvgOutlineAttrs}><rect x=\"4\" y=\"2\" width=\"16\" height=\"20\" rx=\"2\"/><path d=\"M9 22v-4h6v4\"/><path d=\"M8 6h.01M16 6h.01M8 10h.01M16 10h.01M8 14h.01M16 14h.01\"/></svg>"),
            ["building-2"] = new("building-2", SiIconCategory.Location, "Office building alternate",
                $"<svg {SvgOutlineAttrs}><path d=\"M6 22V2h12v20\"/><path d=\"M6 12H2v10h4\"/><path d=\"M18 12h4v10h-4\"/><path d=\"M10 6h4M10 10h4M10 14h4M10 18h4\"/></svg>"),
            ["compass"] = new("compass", SiIconCategory.Location, "Compass navigation",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><polygon points=\"16.24 7.76 14.12 14.12 7.76 16.24 9.88 9.88 16.24 7.76\"/></svg>"),

            // ============================================
            // Logistics Icons
            // ============================================
            ["truck"] = new("truck", SiIconCategory.Logistics, "Delivery or transport",
                $"<svg {SvgOutlineAttrs}><rect x=\"1\" y=\"3\" width=\"15\" height=\"13\"/><polygon points=\"16 8 20 8 23 11 23 16 16 16 16 8\"/><circle cx=\"5.5\" cy=\"18.5\" r=\"2.5\"/><circle cx=\"18.5\" cy=\"18.5\" r=\"2.5\"/></svg>"),
            ["package"] = new("package", SiIconCategory.Logistics, "Package or box",
                $"<svg {SvgOutlineAttrs}><line x1=\"16.5\" y1=\"9.4\" x2=\"7.5\" y2=\"4.21\"/><path d=\"M21 16V8a2 2 0 00-1-1.73l-7-4a2 2 0 00-2 0l-7 4A2 2 0 003 8v8a2 2 0 001 1.73l7 4a2 2 0 002 0l7-4A2 2 0 0021 16z\"/><polyline points=\"3.27 6.96 12 12.01 20.73 6.96\"/><line x1=\"12\" y1=\"22.08\" x2=\"12\" y2=\"12\"/></svg>"),
            ["car"] = new("car", SiIconCategory.Logistics, "Car transport",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 16H9m10 0h3v-3.15a1 1 0 00-.84-.99L16 11l-2.7-3.6a1 1 0 00-.8-.4H5.24a2 2 0 00-1.8 1.1l-.8 1.63A6 6 0 002 12.42V16h2\"/><circle cx=\"6.5\" cy=\"16.5\" r=\"2.5\"/><circle cx=\"16.5\" cy=\"16.5\" r=\"2.5\"/></svg>"),
            ["warehouse"] = new("warehouse", SiIconCategory.Logistics, "Warehouse storage",
                $"<svg {SvgOutlineAttrs}><path d=\"M22 8.35V20a2 2 0 01-2 2H4a2 2 0 01-2-2V8.35A2 2 0 013.26 6.5l8-3.2a2 2 0 011.48 0l8 3.2A2 2 0 0122 8.35z\"/><path d=\"M6 18h12\"/><path d=\"M6 14h12\"/><rect x=\"9\" y=\"18\" width=\"6\" height=\"4\"/></svg>"),
            ["forklift"] = new("forklift", SiIconCategory.Logistics, "Forklift vehicle",
                $"<svg {SvgOutlineAttrs}><path d=\"M5 11h14\"/><path d=\"M5 15v-4\"/><path d=\"M2 15h11a2 2 0 012 2v3\"/><path d=\"M15 22v-9\"/><path d=\"M15 13h6v9\"/><circle cx=\"5\" cy=\"17\" r=\"2\"/><circle cx=\"13\" cy=\"20\" r=\"2\"/></svg>"),
            ["container"] = new("container", SiIconCategory.Logistics, "Shipping container",
                $"<svg {SvgOutlineAttrs}><rect x=\"2\" y=\"6\" width=\"20\" height=\"12\" rx=\"2\"/><path d=\"M6 6v12\"/><path d=\"M10 6v12\"/><path d=\"M14 6v12\"/><path d=\"M18 6v12\"/></svg>"),
            ["delivery"] = new("delivery", SiIconCategory.Logistics, "Delivery service",
                $"<svg {SvgOutlineAttrs}><rect x=\"1\" y=\"6\" width=\"14\" height=\"10\" rx=\"2\"/><path d=\"M15 11l4 2v5h-4\"/><circle cx=\"5.5\" cy=\"18.5\" r=\"2.5\"/><circle cx=\"16.5\" cy=\"18.5\" r=\"2.5\"/><path d=\"M8 16h5\"/></svg>"),
            ["tracking"] = new("tracking", SiIconCategory.Logistics, "Package tracking",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><polygon points=\"12 2 19 21 12 17 5 21 12 2\"/></svg>"),

            // ============================================
            // Commerce Icons
            // ============================================
            ["shopping-cart"] = new("shopping-cart", SiIconCategory.Commerce, "Shopping cart",
                $"<svg {SvgOutlineAttrs}><circle cx=\"9\" cy=\"21\" r=\"1\"/><circle cx=\"20\" cy=\"21\" r=\"1\"/><path d=\"M1 1h4l2.68 13.39a2 2 0 002 1.61h9.72a2 2 0 002-1.61L23 6H6\"/></svg>"),
            ["credit-card"] = new("credit-card", SiIconCategory.Commerce, "Payment card",
                $"<svg {SvgOutlineAttrs}><rect x=\"1\" y=\"4\" width=\"22\" height=\"16\" rx=\"2\" ry=\"2\"/><line x1=\"1\" y1=\"10\" x2=\"23\" y2=\"10\"/></svg>"),
            ["wallet"] = new("wallet", SiIconCategory.Commerce, "Wallet or balance",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 12V7H5a2 2 0 010-4h14v4\"/><path d=\"M3 5v14a2 2 0 002 2h16v-5\"/><path d=\"M18 12a2 2 0 100 4h4v-4z\"/></svg>"),
            ["receipt"] = new("receipt", SiIconCategory.Commerce, "Receipt or transaction record",
                $"<svg {SvgOutlineAttrs}><path d=\"M4 2v20l2-1 2 1 2-1 2 1 2-1 2 1 2-1 2 1V2l-2 1-2-1-2 1-2-1-2 1-2-1-2 1-2-1z\"/><path d=\"M8 10h8M8 14h4\"/></svg>"),
            ["invoice"] = new("invoice", SiIconCategory.Commerce, "Invoice document",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M8 13h8M8 17h5\"/><path d=\"M8 9h1\"/></svg>"),
            ["coins"] = new("coins", SiIconCategory.Commerce, "Currency coins",
                $"<svg {SvgOutlineAttrs}><circle cx=\"8\" cy=\"8\" r=\"6\"/><path d=\"M18.09 10.37A6 6 0 1110.34 18\"/><path d=\"M7 6h2v4\"/></svg>"),
            ["bank"] = new("bank", SiIconCategory.Commerce, "Bank or financial institution",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 21h18\"/><path d=\"M3 10h18\"/><path d=\"M5 6l7-3 7 3\"/><path d=\"M4 10v11\"/><path d=\"M20 10v11\"/><path d=\"M8 10v11\"/><path d=\"M12 10v11\"/><path d=\"M16 10v11\"/></svg>"),
            ["percent"] = new("percent", SiIconCategory.Commerce, "Percentage or discount",
                $"<svg {SvgOutlineAttrs}><line x1=\"19\" y1=\"5\" x2=\"5\" y2=\"19\"/><circle cx=\"6.5\" cy=\"6.5\" r=\"2.5\"/><circle cx=\"17.5\" cy=\"17.5\" r=\"2.5\"/></svg>"),
            ["barcode-scan"] = new("barcode-scan", SiIconCategory.Commerce, "Scan barcode",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 7V5a2 2 0 012-2h2\"/><path d=\"M17 3h2a2 2 0 012 2v2\"/><path d=\"M21 17v2a2 2 0 01-2 2h-2\"/><path d=\"M7 21H5a2 2 0 01-2-2v-2\"/><path d=\"M7 8v8\"/><path d=\"M12 8v8\"/><path d=\"M17 8v8\"/></svg>"),
            ["price-tag"] = new("price-tag", SiIconCategory.Commerce, "Price tag",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 2L2 7l10 5 10-5-10-5z\"/><path d=\"M2 17l10 5 10-5\"/><path d=\"M2 12l10 5 10-5\"/></svg>"),
            ["gift"] = new("gift", SiIconCategory.Commerce, "Gift or present",
                $"<svg {SvgOutlineAttrs}><polyline points=\"20 12 20 22 4 22 4 12\"/><rect x=\"2\" y=\"7\" width=\"20\" height=\"5\"/><line x1=\"12\" y1=\"22\" x2=\"12\" y2=\"7\"/><path d=\"M12 7H7.5a2.5 2.5 0 010-5C11 2 12 7 12 7z\"/><path d=\"M12 7h4.5a2.5 2.5 0 000-5C13 2 12 7 12 7z\"/></svg>"),
            ["coupon"] = new("coupon", SiIconCategory.Commerce, "Coupon or voucher",
                $"<svg {SvgOutlineAttrs}><path d=\"M2 9a3 3 0 010-6h20a3 3 0 110 6\"/><path d=\"M2 15a3 3 0 000 6h20a3 3 0 100-6\"/><path d=\"M2 9v6\"/><path d=\"M22 9v6\"/><path d=\"M9 13v2\"/><path d=\"M9 9v2\"/></svg>"),
            ["money"] = new("money", SiIconCategory.Commerce, "Money or cash",
                $"<svg {SvgOutlineAttrs}><rect x=\"2\" y=\"6\" width=\"20\" height=\"12\" rx=\"2\"/><circle cx=\"12\" cy=\"12\" r=\"3\"/><path d=\"M2 10h2\"/><path d=\"M20 10h2\"/><path d=\"M2 14h2\"/><path d=\"M20 14h2\"/></svg>"),

            // ============================================
            // Layout Icons
            // ============================================
            ["maximize"] = new("maximize", SiIconCategory.Layout, "Expand to full size",
                $"<svg {SvgOutlineAttrs}><path d=\"M8 3H5a2 2 0 00-2 2v3m18 0V5a2 2 0 00-2-2h-3m0 18h3a2 2 0 002-2v-3M3 16v3a2 2 0 002 2h3\"/></svg>"),
            ["minimize"] = new("minimize", SiIconCategory.Layout, "Minimize window",
                $"<svg {SvgOutlineAttrs}><path d=\"M8 3v3a2 2 0 01-2 2H3m18 0h-3a2 2 0 01-2-2V3m0 18v-3a2 2 0 012-2h3M3 16h3a2 2 0 012 2v3\"/></svg>"),
            ["columns"] = new("columns", SiIconCategory.Layout, "Column layout",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 3h7a2 2 0 012 2v14a2 2 0 01-2 2h-7m0-18H5a2 2 0 00-2 2v14a2 2 0 002 2h7m0-18v18\"/></svg>"),
            ["grid"] = new("grid", SiIconCategory.Layout, "Grid layout",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"7\" height=\"7\"/><rect x=\"14\" y=\"3\" width=\"7\" height=\"7\"/><rect x=\"14\" y=\"14\" width=\"7\" height=\"7\"/><rect x=\"3\" y=\"14\" width=\"7\" height=\"7\"/></svg>"),
            ["list"] = new("list", SiIconCategory.Layout, "List layout",
                $"<svg {SvgOutlineAttrs}><line x1=\"8\" y1=\"6\" x2=\"21\" y2=\"6\"/><line x1=\"8\" y1=\"12\" x2=\"21\" y2=\"12\"/><line x1=\"8\" y1=\"18\" x2=\"21\" y2=\"18\"/><line x1=\"3\" y1=\"6\" x2=\"3.01\" y2=\"6\"/><line x1=\"3\" y1=\"12\" x2=\"3.01\" y2=\"12\"/><line x1=\"3\" y1=\"18\" x2=\"3.01\" y2=\"18\"/></svg>"),
            ["layout"] = new("layout", SiIconCategory.Layout, "Page or panel layout",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\"/><path d=\"M3 9h18\"/><path d=\"M9 21V9\"/></svg>"),
            ["layout-sidebar-left"] = new("layout-sidebar-left", SiIconCategory.Layout, "Sidebar on the left",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"5\" height=\"18\" rx=\"1\"/><rect x=\"11\" y=\"3\" width=\"10\" height=\"18\" rx=\"1\"/></svg>"),
            ["layout-sidebar-right"] = new("layout-sidebar-right", SiIconCategory.Layout, "Sidebar on the right",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"10\" height=\"18\" rx=\"1\"/><rect x=\"16\" y=\"3\" width=\"5\" height=\"18\" rx=\"1\"/></svg>"),
            ["component"] = new("component", SiIconCategory.Layout, "UI component block",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"7\" height=\"7\" rx=\"1\"/><rect x=\"14\" y=\"3\" width=\"7\" height=\"7\" rx=\"1\"/><rect x=\"14\" y=\"14\" width=\"7\" height=\"7\" rx=\"1\"/><rect x=\"3\" y=\"14\" width=\"7\" height=\"7\" rx=\"1\"/><path d=\"M10 6h4M6 10v4M18 10v4M10 18h4\"/></svg>"),
            ["cms"] = new("cms", SiIconCategory.Layout, "Content management system",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"13\" height=\"18\" rx=\"2\"/><path d=\"M7 8h6\"/><path d=\"M7 12h6\"/><path d=\"M7 16h4\"/><path d=\"M18 13l3 3-6 6h-3v-3z\"/></svg>"),
            ["newspaper"] = new("newspaper", SiIconCategory.Layout, "News or published content",
                $"<svg {SvgOutlineAttrs}><path d=\"M4 22h16a2 2 0 002-2V4a2 2 0 00-2-2H8a2 2 0 00-2 2v16a2 2 0 01-2 2\"/><path d=\"M10 6h8\"/><path d=\"M10 10h8\"/><path d=\"M10 14h5\"/></svg>"),
            ["align-start"] = new("align-start", SiIconCategory.Layout, "Align content to start",
                $"<svg {SvgOutlineAttrs}><rect x=\"2\" y=\"5\" width=\"5\" height=\"14\" rx=\"1\"/><rect x=\"9\" y=\"7\" width=\"5\" height=\"10\" rx=\"1\"/><path d=\"M22 3v18\"/></svg>"),
            ["align-end"] = new("align-end", SiIconCategory.Layout, "Align content to end",
                $"<svg {SvgOutlineAttrs}><path d=\"M2 3v18\"/><rect x=\"10\" y=\"7\" width=\"5\" height=\"10\" rx=\"1\"/><rect x=\"17\" y=\"5\" width=\"5\" height=\"14\" rx=\"1\"/></svg>"),
            ["align-stretch"] = new("align-stretch", SiIconCategory.Layout, "Stretch content to fill",
                $"<svg {SvgOutlineAttrs}><path d=\"M4 2v4\"/><path d=\"M20 2v4\"/><path d=\"M4 18v4\"/><path d=\"M20 18v4\"/><path d=\"M4 6h16\"/><path d=\"M4 12h16\"/><path d=\"M4 18h16\"/></svg>"),
            ["align-space-between"] = new("align-space-between", SiIconCategory.Layout, "Distribute with space between",
                $"<svg {SvgOutlineAttrs}><rect x=\"2\" y=\"5\" width=\"5\" height=\"14\" rx=\"1\"/><rect x=\"9.5\" y=\"7\" width=\"5\" height=\"10\" rx=\"1\"/><rect x=\"17\" y=\"5\" width=\"5\" height=\"14\" rx=\"1\"/></svg>"),
            ["align-distribute"] = new("align-distribute", SiIconCategory.Layout, "Distribute with space around",
                $"<svg {SvgOutlineAttrs}><rect x=\"4\" y=\"5\" width=\"4\" height=\"14\" rx=\"1\"/><rect x=\"10\" y=\"7\" width=\"4\" height=\"10\" rx=\"1\"/><rect x=\"16\" y=\"5\" width=\"4\" height=\"14\" rx=\"1\"/></svg>"),

            // ============================================
            // Typography Icons
            // ============================================
            ["align-left"] = new("align-left", SiIconCategory.Typography, "Align text left",
                $"<svg {SvgOutlineAttrs}><line x1=\"17\" y1=\"10\" x2=\"3\" y2=\"10\"/><line x1=\"21\" y1=\"6\" x2=\"3\" y2=\"6\"/><line x1=\"21\" y1=\"14\" x2=\"3\" y2=\"14\"/><line x1=\"17\" y1=\"18\" x2=\"3\" y2=\"18\"/></svg>"),
            ["align-center"] = new("align-center", SiIconCategory.Typography, "Align text center",
                $"<svg {SvgOutlineAttrs}><line x1=\"18\" y1=\"10\" x2=\"6\" y2=\"10\"/><line x1=\"21\" y1=\"6\" x2=\"3\" y2=\"6\"/><line x1=\"21\" y1=\"14\" x2=\"3\" y2=\"14\"/><line x1=\"18\" y1=\"18\" x2=\"6\" y2=\"18\"/></svg>"),
            ["align-right"] = new("align-right", SiIconCategory.Typography, "Align text right",
                $"<svg {SvgOutlineAttrs}><line x1=\"21\" y1=\"10\" x2=\"7\" y2=\"10\"/><line x1=\"21\" y1=\"6\" x2=\"3\" y2=\"6\"/><line x1=\"21\" y1=\"14\" x2=\"3\" y2=\"14\"/><line x1=\"21\" y1=\"18\" x2=\"7\" y2=\"18\"/></svg>"),
            ["align-justify"] = new("align-justify", SiIconCategory.Typography, "Justify text",
                $"<svg {SvgOutlineAttrs}><line x1=\"21\" y1=\"10\" x2=\"3\" y2=\"10\"/><line x1=\"21\" y1=\"6\" x2=\"3\" y2=\"6\"/><line x1=\"21\" y1=\"14\" x2=\"3\" y2=\"14\"/><line x1=\"21\" y1=\"18\" x2=\"3\" y2=\"18\"/></svg>"),
            ["bold"] = new("bold", SiIconCategory.Typography, "Bold text style",
                $"<svg {SvgOutlineAttrs}><path d=\"M6 4h8a4 4 0 014 4 4 4 0 01-4 4H6z\"/><path d=\"M6 12h9a4 4 0 014 4 4 4 0 01-4 4H6z\"/></svg>"),
            ["italic"] = new("italic", SiIconCategory.Typography, "Italic text style",
                $"<svg {SvgOutlineAttrs}><line x1=\"19\" y1=\"4\" x2=\"10\" y2=\"4\"/><line x1=\"14\" y1=\"20\" x2=\"5\" y2=\"20\"/><line x1=\"15\" y1=\"4\" x2=\"9\" y2=\"20\"/></svg>"),
            ["underline"] = new("underline", SiIconCategory.Typography, "Underline text",
                $"<svg {SvgOutlineAttrs}><path d=\"M6 3v7a6 6 0 006 6 6 6 0 006-6V3\"/><line x1=\"4\" y1=\"21\" x2=\"20\" y2=\"21\"/></svg>"),

            // ============================================
            // Feedback Icons
            // ============================================
            ["star"] = new("star", SiIconCategory.Feedback, "Favorite or star",
                $"<svg {SvgOutlineAttrs}><polygon points=\"12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2\"/></svg>"),
            ["heart"] = new("heart", SiIconCategory.Feedback, "Like or favorite",
                $"<svg {SvgOutlineAttrs}><path d=\"M20.84 4.61a5.5 5.5 0 00-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 00-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 000-7.78z\"/></svg>"),
            ["thumbs-up"] = new("thumbs-up", SiIconCategory.Feedback, "Approve or like",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 9V5a3 3 0 00-3-3l-4 9v11h11.28a2 2 0 002-1.7l1.38-9a2 2 0 00-2-2.3zM7 22H4a2 2 0 01-2-2v-7a2 2 0 012-2h3\"/></svg>"),
            ["thumbs-down"] = new("thumbs-down", SiIconCategory.Feedback, "Disapprove",
                $"<svg {SvgOutlineAttrs}><path d=\"M10 15v4a3 3 0 003 3l4-9V2H5.72a2 2 0 00-2 1.7l-1.38 9a2 2 0 002 2.3zm7-13h2.67A2.31 2.31 0 0122 4v7a2.31 2.31 0 01-2.33 2H17\"/></svg>"),

            // ============================================
            // Device Icons
            // ============================================
            ["printer"] = new("printer", SiIconCategory.Devices, "Print document",
                $"<svg {SvgOutlineAttrs}><polyline points=\"6 9 6 2 18 2 18 9\"/><path d=\"M6 18H4a2 2 0 01-2-2v-5a2 2 0 012-2h16a2 2 0 012 2v5a2 2 0 01-2 2h-2\"/><rect x=\"6\" y=\"14\" width=\"12\" height=\"8\"/></svg>"),
            ["scanner"] = new("scanner", SiIconCategory.Devices, "Scan document",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 7V5a2 2 0 012-2h2\"/><path d=\"M17 3h2a2 2 0 012 2v2\"/><path d=\"M21 17v2a2 2 0 01-2 2h-2\"/><path d=\"M7 21H5a2 2 0 01-2-2v-2\"/><rect x=\"7\" y=\"7\" width=\"10\" height=\"10\" rx=\"1\"/></svg>"),
            ["qr-code"] = new("qr-code", SiIconCategory.Devices, "QR code",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"7\" height=\"7\"/><rect x=\"14\" y=\"3\" width=\"7\" height=\"7\"/><rect x=\"3\" y=\"14\" width=\"4\" height=\"4\"/><rect x=\"9\" y=\"14\" width=\"4\" height=\"4\"/><rect x=\"15\" y=\"14\" width=\"4\" height=\"4\"/><path d=\"M7 14v4M14 7h4M14 11h2M11 14h2\"/></svg>"),
            ["barcode"] = new("barcode", SiIconCategory.Devices, "Barcode",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 5v14\"/><path d=\"M8 5v14\"/><path d=\"M12 5v14\"/><path d=\"M17 5v14\"/><path d=\"M21 5v14\"/><path d=\"M5 9h2\"/><path d=\"M10 9h1\"/><path d=\"M14 9h1\"/><path d=\"M19 9h2\"/></svg>"),
            ["monitor"] = new("monitor", SiIconCategory.Devices, "Desktop monitor",
                $"<svg {SvgOutlineAttrs}><rect x=\"2\" y=\"3\" width=\"20\" height=\"14\" rx=\"2\" ry=\"2\"/><line x1=\"8\" y1=\"21\" x2=\"16\" y2=\"21\"/><line x1=\"12\" y1=\"17\" x2=\"12\" y2=\"21\"/></svg>"),
            ["laptop"] = new("laptop", SiIconCategory.Devices, "Laptop device",
                $"<svg {SvgOutlineAttrs}><path d=\"M20 16V7a2 2 0 00-2-2H6a2 2 0 00-2 2v9m16 0H4m16 0l1.28 2.55a1 1 0 01-.9 1.45H3.62a1 1 0 01-.9-1.45L4 16\"/></svg>"),
            ["tablet"] = new("tablet", SiIconCategory.Devices, "Tablet device",
                $"<svg {SvgOutlineAttrs}><rect x=\"4\" y=\"2\" width=\"16\" height=\"20\" rx=\"2\" ry=\"2\"/><line x1=\"12\" y1=\"18\" x2=\"12.01\" y2=\"18\"/></svg>"),
            ["smartphone"] = new("smartphone", SiIconCategory.Devices, "Mobile phone",
                $"<svg {SvgOutlineAttrs}><rect x=\"5\" y=\"2\" width=\"14\" height=\"20\" rx=\"2\" ry=\"2\"/><line x1=\"12\" y1=\"18\" x2=\"12.01\" y2=\"18\"/></svg>"),
            ["wifi"] = new("wifi", SiIconCategory.Devices, "Wi-Fi signal",
                $"<svg {SvgOutlineAttrs}><path d=\"M5 12.55a11 11 0 0114.08 0\"/><path d=\"M1.42 9a16 16 0 0121.16 0\"/><path d=\"M8.53 16.11a6 6 0 016.95 0\"/><line x1=\"12\" y1=\"20\" x2=\"12.01\" y2=\"20\"/></svg>"),
            ["bluetooth"] = new("bluetooth", SiIconCategory.Devices, "Bluetooth connection",
                $"<svg {SvgOutlineAttrs}><polyline points=\"6.5 6.5 17.5 17.5 12 23 12 1 17.5 6.5 6.5 17.5\"/></svg>"),
            ["battery"] = new("battery", SiIconCategory.Devices, "Battery level",
                $"<svg {SvgOutlineAttrs}><rect x=\"1\" y=\"6\" width=\"18\" height=\"12\" rx=\"2\" ry=\"2\"/><line x1=\"23\" y1=\"13\" x2=\"23\" y2=\"11\"/></svg>"),
            ["battery-charging"] = new("battery-charging", SiIconCategory.Devices, "Charging battery",
                $"<svg {SvgOutlineAttrs}><path d=\"M15 7l-4 6h4l-2 6\"/><rect x=\"1\" y=\"6\" width=\"18\" height=\"12\" rx=\"2\" ry=\"2\"/><line x1=\"23\" y1=\"13\" x2=\"23\" y2=\"11\"/></svg>"),
            ["plug"] = new("plug", SiIconCategory.Devices, "Power plug",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 22v-5\"/><path d=\"M9 8V2\"/><path d=\"M15 8V2\"/><path d=\"M18 8v5a4 4 0 01-4 4h-4a4 4 0 01-4-4V8\"/></svg>"),
            ["power"] = new("power", SiIconCategory.Devices, "Power button",
                $"<svg {SvgOutlineAttrs}><path d=\"M18.36 6.64a9 9 0 11-12.73 0\"/><line x1=\"12\" y1=\"2\" x2=\"12\" y2=\"12\"/></svg>"),
            ["tv"] = new("tv", SiIconCategory.Devices, "Television screen",
                $"<svg {SvgOutlineAttrs}><rect x=\"2\" y=\"7\" width=\"20\" height=\"15\" rx=\"2\" ry=\"2\"/><polyline points=\"17 2 12 7 7 2\"/></svg>"),
            ["watch"] = new("watch", SiIconCategory.Devices, "Smart watch",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"7\"/><polyline points=\"12 9 12 12 13.5 13.5\"/><path d=\"M16.51 17.35l-.35 3.83a2 2 0 01-2 1.82H9.83a2 2 0 01-2-1.82l-.35-3.83m.01-10.7l.35-3.83A2 2 0 019.83 1h4.35a2 2 0 012 1.82l.35 3.83\"/></svg>"),
            ["headphones"] = new("headphones", SiIconCategory.Devices, "Audio headphones",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 18v-6a9 9 0 0118 0v6\"/><path d=\"M21 19a2 2 0 01-2 2h-1a2 2 0 01-2-2v-3a2 2 0 012-2h3zM3 19a2 2 0 002 2h1a2 2 0 002-2v-3a2 2 0 00-2-2H3z\"/></svg>"),
            ["speaker"] = new("speaker", SiIconCategory.Devices, "Audio speaker",
                $"<svg {SvgOutlineAttrs}><rect x=\"4\" y=\"2\" width=\"16\" height=\"20\" rx=\"2\" ry=\"2\"/><circle cx=\"12\" cy=\"14\" r=\"4\"/><line x1=\"12\" y1=\"6\" x2=\"12.01\" y2=\"6\"/></svg>"),
            ["usb"] = new("usb", SiIconCategory.Devices, "USB port",
                $"<svg {SvgOutlineAttrs}><circle cx=\"10\" cy=\"7\" r=\"1\"/><circle cx=\"4\" cy=\"20\" r=\"1\"/><path d=\"M4.5 19.5l5.5-5.5\"/><circle cx=\"20\" cy=\"15\" r=\"1\"/><path d=\"M19.5 15.5l-5.5-5.5\"/><path d=\"M10 7v9a6 6 0 006 6\"/><path d=\"M14 7l-4 4\"/></svg>"),

            // ============================================
            // Development Icons
            // ============================================
            ["database"] = new("database", SiIconCategory.Development, "Database storage",
                $"<svg {SvgOutlineAttrs}><ellipse cx=\"12\" cy=\"5\" rx=\"9\" ry=\"3\"/><path d=\"M21 12c0 1.66-4 3-9 3s-9-1.34-9-3\"/><path d=\"M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5\"/></svg>"),
            ["server"] = new("server", SiIconCategory.Development, "Server rack",
                $"<svg {SvgOutlineAttrs}><rect x=\"2\" y=\"2\" width=\"20\" height=\"8\" rx=\"2\" ry=\"2\"/><rect x=\"2\" y=\"14\" width=\"20\" height=\"8\" rx=\"2\" ry=\"2\"/><line x1=\"6\" y1=\"6\" x2=\"6.01\" y2=\"6\"/><line x1=\"6\" y1=\"18\" x2=\"6.01\" y2=\"18\"/></svg>"),
            ["code"] = new("code", SiIconCategory.Development, "Code brackets",
                $"<svg {SvgOutlineAttrs}><polyline points=\"16 18 22 12 16 6\"/><polyline points=\"8 6 2 12 8 18\"/></svg>"),
            ["terminal"] = new("terminal", SiIconCategory.Development, "Terminal console",
                $"<svg {SvgOutlineAttrs}><polyline points=\"4 17 10 11 4 5\"/><line x1=\"12\" y1=\"19\" x2=\"20\" y2=\"19\"/></svg>"),
            ["bug"] = new("bug", SiIconCategory.Development, "Bug or issue",
                $"<svg {SvgOutlineAttrs}><rect x=\"8\" y=\"6\" width=\"8\" height=\"14\" rx=\"4\"/><path d=\"M19 8l-2 2m5 2h-4m3 4l-2-2M5 8l2 2m-5 2h4m-3 4l2-2\"/><path d=\"M12 6V2\"/></svg>"),
            ["rocket"] = new("rocket", SiIconCategory.Development, "Launch or deploy",
                $"<svg {SvgOutlineAttrs}><path d=\"M4.5 16.5c-1.5 1.26-2 5-2 5s3.74-.5 5-2c.71-.84.7-2.13-.09-2.91a2.18 2.18 0 00-2.91-.09z\"/><path d=\"M12 15l-3-3a22 22 0 012-3.95A12.88 12.88 0 0122 2c0 2.72-.78 7.5-6 11a22.35 22.35 0 01-4 2z\"/><path d=\"M9 12H4s.55-3.03 2-4c1.62-1.08 5 0 5 0\"/><path d=\"M12 15v5s3.03-.55 4-2c1.08-1.62 0-5 0-5\"/></svg>"),
            ["git-branch"] = new("git-branch", SiIconCategory.Development, "Git branch",
                $"<svg {SvgOutlineAttrs}><line x1=\"6\" y1=\"3\" x2=\"6\" y2=\"15\"/><circle cx=\"18\" cy=\"6\" r=\"3\"/><circle cx=\"6\" cy=\"18\" r=\"3\"/><path d=\"M18 9a9 9 0 01-9 9\"/></svg>"),
            ["git-commit"] = new("git-commit", SiIconCategory.Development, "Git commit",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"4\"/><line x1=\"1.05\" y1=\"12\" x2=\"7\" y2=\"12\"/><line x1=\"17.01\" y1=\"12\" x2=\"22.96\" y2=\"12\"/></svg>"),
            ["git-merge"] = new("git-merge", SiIconCategory.Development, "Git merge",
                $"<svg {SvgOutlineAttrs}><circle cx=\"18\" cy=\"18\" r=\"3\"/><circle cx=\"6\" cy=\"6\" r=\"3\"/><path d=\"M6 21V9a9 9 0 009 9\"/></svg>"),
            ["git-pull-request"] = new("git-pull-request", SiIconCategory.Development, "Git pull request",
                $"<svg {SvgOutlineAttrs}><circle cx=\"18\" cy=\"18\" r=\"3\"/><circle cx=\"6\" cy=\"6\" r=\"3\"/><path d=\"M13 6h3a2 2 0 012 2v7\"/><line x1=\"6\" y1=\"9\" x2=\"6\" y2=\"21\"/></svg>"),
            ["git-fork"] = new("git-fork", SiIconCategory.Development, "Git fork repository",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"18\" r=\"3\"/><circle cx=\"6\" cy=\"6\" r=\"3\"/><circle cx=\"18\" cy=\"6\" r=\"3\"/><path d=\"M18 9v2c0 .6-.4 1-1 1H7c-.6 0-1-.4-1-1V9\"/><path d=\"M12 12v3\"/></svg>"),
            ["github"] = new("github", SiIconCategory.Development, "GitHub logo",
                $"<svg {SvgOutlineAttrs}><path d=\"M15 22v-4a4.8 4.8 0 00-1-3.5c3 0 6-2 6-5.5.08-1.25-.27-2.48-1-3.5.28-1.15.28-2.35 0-3.5 0 0-1 0-3 1.5-2.64-.5-5.36-.5-8 0C6 2 5 2 5 2c-.3 1.15-.3 2.35 0 3.5A5.403 5.403 0 004 9c0 3.5 3 5.5 6 5.5-.39.49-.68 1.05-.85 1.65-.17.6-.22 1.23-.15 1.85v4\"/><path d=\"M9 18c-4.51 2-5-2-7-2\"/></svg>"),
            ["api-endpoint"] = new("api-endpoint", SiIconCategory.Development, "API endpoint or route",
                $"<svg {SvgOutlineAttrs}><path d=\"M10 13a5 5 0 007.54.54l3-3a5 5 0 00-7.07-7.07l-1.72 1.71\"/><path d=\"M14 11a5 5 0 00-7.54-.54l-3 3a5 5 0 007.07 7.07l1.71-1.71\"/><circle cx=\"12\" cy=\"12\" r=\"2\"/></svg>"),
            ["webhook"] = new("webhook", SiIconCategory.Development, "Webhook or callback",
                $"<svg {SvgOutlineAttrs}><path d=\"M18 16.98h-5.99c-1.66 0-3.01 1.34-3.01 3s1.35 3.01 3.01 3.01\"/><path d=\"M18 2.98c-1.66 0-3.01 1.34-3.01 3 0 .83.34 1.58.88 2.12l-4.01 6.92\"/><circle cx=\"18\" cy=\"5.98\" r=\"3\"/><circle cx=\"6\" cy=\"18\" r=\"3\"/></svg>"),

            // ============================================
            // Tool Icons
            // ============================================
            ["wrench"] = new("wrench", SiIconCategory.Tools, "Wrench tool",
                $"<svg {SvgOutlineAttrs}><path d=\"M14.7 6.3a1 1 0 000 1.4l1.6 1.6a1 1 0 001.4 0l3.77-3.77a6 6 0 01-7.94 7.94l-6.91 6.91a2.12 2.12 0 01-3-3l6.91-6.91a6 6 0 017.94-7.94l-3.76 3.76z\"/></svg>"),
            ["hammer"] = new("hammer", SiIconCategory.Tools, "Hammer tool",
                $"<svg {SvgOutlineAttrs}><path d=\"M15 12l-8.5 8.5c-.83.83-2.17.83-3 0 0 0 0 0 0 0a2.12 2.12 0 010-3L12 9\"/><path d=\"M17.64 15L22 10.64\"/><path d=\"M20.91 11.7l-1.25-1.25c-.6-.6-.93-1.4-.93-2.25v-.86L16.01 4.6a5.56 5.56 0 00-3.94-1.64H9l.92.82A6.18 6.18 0 0112 8.4v1.56l2 2h2.47l2.26 1.91\"/></svg>"),
            ["toolbox"] = new("toolbox", SiIconCategory.Tools, "Toolbox",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 7h18a2 2 0 012 2v10a2 2 0 01-2 2H3a2 2 0 01-2-2V9a2 2 0 012-2z\"/><path d=\"M8 7V5a2 2 0 012-2h4a2 2 0 012 2v2\"/><path d=\"M12 12v4\"/><path d=\"M8 12h8\"/></svg>"),
            ["screwdriver"] = new("screwdriver", SiIconCategory.Tools, "Screwdriver tool",
                $"<svg {SvgOutlineAttrs}><path d=\"M14.27 3.75L17.17 6.65 8.17 15.65 5.27 12.75z\"/><path d=\"M8.17 15.65l-4.31 4.31a1 1 0 001.41 1.41l4.31-4.31\"/><path d=\"M17.17 6.65l2.12-2.12a1 1 0 000-1.41l-1.41-1.41a1 1 0 00-1.41 0l-2.12 2.12\"/></svg>"),
            ["ruler"] = new("ruler", SiIconCategory.Tools, "Ruler measurement",
                $"<svg {SvgOutlineAttrs}><path d=\"M21.72 12.48l-8.2 8.2a2 2 0 01-2.83 0l-8.41-8.41a2 2 0 010-2.83l8.2-8.2a2 2 0 012.83 0l8.41 8.41a2 2 0 010 2.83z\"/><path d=\"M14.59 8L12 10.59\"/><path d=\"M10.59 12L8 14.59\"/><path d=\"M6.59 16L4 18.59\"/></svg>"),
            ["measure"] = new("measure", SiIconCategory.Tools, "Tape measure",
                $"<svg {SvgOutlineAttrs}><path d=\"M22 12h-4l-3 9L9 3l-3 9H2\"/></svg>"),
            ["SufiChainsors"] = new("SufiChainsors", SiIconCategory.Tools, "SufiChainsors cutting",
                $"<svg {SvgOutlineAttrs}><circle cx=\"6\" cy=\"6\" r=\"3\"/><circle cx=\"6\" cy=\"18\" r=\"3\"/><line x1=\"20\" y1=\"4\" x2=\"8.12\" y2=\"15.88\"/><line x1=\"14.47\" y1=\"14.48\" x2=\"20\" y2=\"20\"/><line x1=\"8.12\" y1=\"8.12\" x2=\"12\" y2=\"12\"/></svg>"),
            ["tape"] = new("tape", SiIconCategory.Tools, "Adhesive tape",
                $"<svg {SvgOutlineAttrs}><path d=\"M4.5 16.5c-1.5 1.26-2 5-2 5s3.74-.5 5-2c.71-.84.7-2.13-.09-2.91a2.18 2.18 0 00-2.91-.09z\"/><path d=\"M12 12l-9.5 9.5\"/><circle cx=\"12\" cy=\"12\" r=\"10\"/><path d=\"M17 12a5 5 0 11-10 0 5 5 0 0110 0z\"/></svg>"),

            // ============================================
            // Design Icons
            // ============================================
            ["palette"] = new("palette", SiIconCategory.Design, "Color palette",
                $"<svg {SvgOutlineAttrs}><circle cx=\"13.5\" cy=\"6.5\" r=\".5\"/><circle cx=\"17.5\" cy=\"10.5\" r=\".5\"/><circle cx=\"8.5\" cy=\"7.5\" r=\".5\"/><circle cx=\"6.5\" cy=\"12.5\" r=\".5\"/><path d=\"M12 2C6.5 2 2 6.5 2 12s4.5 10 10 10c.926 0 1.648-.746 1.648-1.688 0-.437-.18-.835-.437-1.125-.29-.289-.438-.652-.438-1.125a1.64 1.64 0 011.668-1.668h1.996c3.051 0 5.555-2.503 5.555-5.555C21.965 6.012 17.461 2 12 2z\"/></svg>"),
            ["brush"] = new("brush", SiIconCategory.Design, "Brush or paint",
                $"<svg {SvgOutlineAttrs}><path d=\"M9.06 11.9l8.07-8.06a2.85 2.85 0 114.03 4.03l-8.06 8.08\"/><path d=\"M7.07 14.94c-1.66 0-3 1.35-3 3.02 0 1.33-2.5 1.52-2 2.02 1.08 1.1 2.49 2.02 4 2.02 2.2 0 4-1.8 4-4.04a3.01 3.01 0 00-3-3.02z\"/></svg>"),
            ["sufi-blazor"] = new("sufi-blazor", SiIconCategory.Design, "SufiBlazor component library mark",
                "<svg viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.7\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><rect x=\"6.6\" y=\"2\" width=\"14.2\" height=\"4\" rx=\"1.1\" fill=\"currentColor\" stroke=\"none\" opacity=\".92\"/><path d=\"M3.4 6.7V5.1c0-1.15.95-2.1 2.1-2.1h1.1\"/><rect x=\"7.1\" y=\"6.4\" width=\"13.7\" height=\"5.1\" rx=\"1.1\" fill=\"currentColor\" stroke=\"none\" opacity=\".4\"/><rect x=\"1.8\" y=\"8.2\" width=\"12\" height=\"8.2\" rx=\"2\" fill=\"currentColor\" stroke=\"none\" opacity=\".95\"/><path d=\"M13.8 10h1.5c1.15 0 2.1.95 2.1 2.1v1.4\"/><rect x=\"4.7\" y=\"16.2\" width=\"11.8\" height=\"5.8\" rx=\"1.25\" fill=\"currentColor\" stroke=\"none\" opacity=\".62\"/><path d=\"M8.5 18.4h4.6M8.5 20.3h4.6\"/><rect x=\"18.1\" y=\"12.7\" width=\"4.1\" height=\"4.1\" rx=\"1\" fill=\"currentColor\" stroke=\"none\" opacity=\".98\"/><path d=\"M21.6 17.4v1.5c0 1.15-.95 2.1-2.1 2.1h-1.4\"/><path d=\"M17.4 10.2V9c0-1.15.95-2.1 2.1-2.1h1.2\"/></svg>"),
            ["pen-tool"] = new("pen-tool", SiIconCategory.Design, "Pen tool for drawing",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 19l7-7 3 3-7 7-3-3z\"/><path d=\"M18 13l-1.5-7.5L2 2l3.5 14.5L13 18l5-5z\"/><path d=\"M2 2l7.586 7.586\"/><circle cx=\"11\" cy=\"11\" r=\"2\"/></svg>"),
            ["dropper"] = new("dropper", SiIconCategory.Design, "Color dropper or eyedropper",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 2.69l5.66 5.66a8 8 0 11-11.31 0z\"/></svg>"),
            ["layers-2"] = new("layers-2", SiIconCategory.Design, "Stacked layers",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 16l-7-3.5 7-3.5 7 3.5-7 3.5z\"/><path d=\"M5 12.5l7 3.5 7-3.5\"/><path d=\"M5 16l7 3.5 7-3.5\"/></svg>"),
            ["crop"] = new("crop", SiIconCategory.Design, "Crop or trim image",
                $"<svg {SvgOutlineAttrs}><path d=\"M6.13 1L6 16a2 2 0 002 2h15\"/><path d=\"M1 6.13L16 6a2 2 0 012 2v15\"/></svg>"),
            ["color-picker"] = new("color-picker", SiIconCategory.Design, "Color picker tool",
                $"<svg {SvgOutlineAttrs}><path d=\"M11 2a2 2 0 00-2 2v5l-2 2 6 6 2-2h5a2 2 0 002-2\"/><path d=\"M7.5 16.5l-5 5\"/><path d=\"M18 2l4 4-6 6-4-4z\"/></svg>"),
            ["eraser"] = new("eraser", SiIconCategory.Design, "Eraser tool",
                $"<svg {SvgOutlineAttrs}><path d=\"M7 21h10\"/><path d=\"M5.5 13.5L9 17l7-7-3.5-3.5L5.5 13.5a2.12 2.12 0 000 3l2.5 2.5\"/><path d=\"M9.5 6.5L19 3l-3.5 9.5\"/></svg>"),

            // ============================================
            // AI Icons
            // ============================================
            ["sparkles"] = new("sparkles", SiIconCategory.Ai, "Magic or enhancement",
                $"<svg {SvgOutlineAttrs}><path d=\"M9.937 15.5A2 2 0 008.5 14.063l-6.135-1.582a.5.5 0 010-.962L8.5 9.936A2 2 0 009.937 8.5l1.582-6.135a.5.5 0 01.962 0L14.063 8.5A2 2 0 0015.5 9.937l6.135 1.582a.5.5 0 010 .962L15.5 14.063a2 2 0 00-1.437 1.437l-1.582 6.135a.5.5 0 01-.962 0z\"/><path d=\"M20 3v4\"/><path d=\"M22 5h-4\"/><path d=\"M4 17v2\"/><path d=\"M5 18H3\"/></svg>"),
            ["bot"] = new("bot", SiIconCategory.Ai, "Bot or automation",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"11\" width=\"18\" height=\"10\" rx=\"2\"/><circle cx=\"12\" cy=\"5\" r=\"2\"/><path d=\"M12 7v4\"/><line x1=\"8\" y1=\"16\" x2=\"8\" y2=\"16\"/><line x1=\"16\" y1=\"16\" x2=\"16\" y2=\"16\"/></svg>"),
            ["brain"] = new("brain", SiIconCategory.Ai, "Intelligence or AI",
                $"<svg {SvgOutlineAttrs}><path d=\"M9.5 2A2.5 2.5 0 0112 4.5v15a2.5 2.5 0 01-4.96.44 2.5 2.5 0 01-2.96-3.08 3 3 0 01-.34-5.58 2.5 2.5 0 011.32-4.24 2.5 2.5 0 014.44-1.54\"/><path d=\"M14.5 2A2.5 2.5 0 0012 4.5v15a2.5 2.5 0 004.96.44 2.5 2.5 0 002.96-3.08 3 3 0 00.34-5.58 2.5 2.5 0 00-1.32-4.24 2.5 2.5 0 00-4.44-1.54\"/></svg>"),
            ["cpu"] = new("cpu", SiIconCategory.Ai, "CPU or chip",
                $"<svg {SvgOutlineAttrs}><rect x=\"4\" y=\"4\" width=\"16\" height=\"16\" rx=\"2\"/><rect x=\"9\" y=\"9\" width=\"6\" height=\"6\"/><path d=\"M9 1v3\"/><path d=\"M15 1v3\"/><path d=\"M9 20v3\"/><path d=\"M15 20v3\"/><path d=\"M20 9h3\"/><path d=\"M20 14h3\"/><path d=\"M1 9h3\"/><path d=\"M1 14h3\"/></svg>"),
            ["chip-ai"] = new("chip-ai", SiIconCategory.Ai, "AI processor chip",
                $"<svg {SvgOutlineAttrs}><rect x=\"5\" y=\"5\" width=\"14\" height=\"14\" rx=\"2\"/><path d=\"M9 9h6v6H9z\"/><path d=\"M5 9H2\"/><path d=\"M5 15H2\"/><path d=\"M19 9h3\"/><path d=\"M19 15h3\"/><path d=\"M9 5V2\"/><path d=\"M15 5V2\"/><path d=\"M9 22v-3\"/><path d=\"M15 22v-3\"/></svg>"),
            ["neural-net"] = new("neural-net", SiIconCategory.Ai, "Neural network",
                $"<svg {SvgOutlineAttrs}><circle cx=\"4\" cy=\"6\" r=\"2\"/><circle cx=\"4\" cy=\"18\" r=\"2\"/><circle cx=\"12\" cy=\"12\" r=\"2\"/><circle cx=\"20\" cy=\"6\" r=\"2\"/><circle cx=\"20\" cy=\"18\" r=\"2\"/><path d=\"M6 6h4.17\"/><path d=\"M6 18h4.17\"/><path d=\"M13.83 10.83L18 6\"/><path d=\"M13.83 13.17L18 18\"/></svg>"),
            ["vector"] = new("vector", SiIconCategory.Ai, "Vector or embedding",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 3v18h18\"/><circle cx=\"8\" cy=\"16\" r=\"2\"/><circle cx=\"13\" cy=\"11\" r=\"2\"/><circle cx=\"18\" cy=\"6\" r=\"2\"/><path d=\"M8 14v-3l5-5\"/><path d=\"M13 9l5-5\"/></svg>"),
            ["embedding"] = new("embedding", SiIconCategory.Ai, "Data embedding",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\"/><path d=\"M8 8h2v2H8z\"/><path d=\"M14 8h2v2h-2z\"/><path d=\"M8 14h2v2H8z\"/><path d=\"M14 14h2v2h-2z\"/><path d=\"M11 11h2v2h-2z\"/></svg>"),
            ["model"] = new("model", SiIconCategory.Ai, "AI model",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 2L2 7l10 5 10-5-10-5z\"/><path d=\"M2 17l10 5 10-5\"/><path d=\"M2 12l10 5 10-5\"/></svg>"),
            ["prompt"] = new("prompt", SiIconCategory.Ai, "AI prompt input",
                $"<svg {SvgOutlineAttrs}><path d=\"M4 12h16\"/><path d=\"M4 6h16\"/><path d=\"M4 18h8\"/><circle cx=\"17\" cy=\"18\" r=\"3\"/><path d=\"M17 15v6\"/></svg>"),
            ["assistant"] = new("assistant", SiIconCategory.Ai, "AI assistant",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"8\" r=\"5\"/><path d=\"M20 21a8 8 0 00-16 0\"/><path d=\"M12 8v0\"/><path d=\"M9 8h6\"/></svg>"),
            ["wand"] = new("wand", SiIconCategory.Ai, "Magic wand",
                $"<svg {SvgOutlineAttrs}><path d=\"M15 4V2\"/><path d=\"M15 16v-2\"/><path d=\"M8 9h2\"/><path d=\"M20 9h2\"/><path d=\"M17.8 11.8L19 13\"/><path d=\"M15 9a2 2 0 110-4 2 2 0 010 4z\"/><path d=\"M17.8 6.2L19 5\"/><path d=\"m3 21 9-9\"/><path d=\"M12.2 6.2L11 5\"/></svg>"),

            // ============================================
            // Health Icons
            // ============================================
            ["heart-pulse"] = new("heart-pulse", SiIconCategory.Health, "Health pulse",
                $"<svg {SvgOutlineAttrs}><path d=\"M19 14c1.49-1.46 3-3.21 3-5.5A5.5 5.5 0 0016.5 3c-1.76 0-3 .5-4.5 2-1.5-1.5-2.74-2-4.5-2A5.5 5.5 0 002 8.5c0 2.3 1.5 4.05 3 5.5l7 7Z\"/><path d=\"M3.22 12H9.5l.5-1 2 4.5 2-7 1.5 3.5h5.27\"/></svg>"),
            ["stethoscope"] = new("stethoscope", SiIconCategory.Health, "Medical stethoscope",
                $"<svg {SvgOutlineAttrs}><path d=\"M4.8 2.3A.3.3 0 105 2H4a2 2 0 00-2 2v5a6 6 0 006 6v0a6 6 0 006-6V4a2 2 0 00-2-2h-1a.2.2 0 10.3.3\"/><path d=\"M8 15v1a6 6 0 006 6v0a6 6 0 006-6v-4\"/><circle cx=\"20\" cy=\"10\" r=\"2\"/></svg>"),
            ["pill"] = new("pill", SiIconCategory.Health, "Medicine pill",
                $"<svg {SvgOutlineAttrs}><path d=\"M10.5 20.5L3.5 13.5a4.95 4.95 0 117 -7l7 7a4.95 4.95 0 01-7 7z\"/><path d=\"M8.5 8.5l7 7\"/></svg>"),
            ["dna"] = new("dna", SiIconCategory.Health, "DNA helix",
                $"<svg {SvgOutlineAttrs}><path d=\"M2 15c6.667-6 13.333 0 20-6\"/><path d=\"M9 22c1.798-1.998 2.518-3.995 2.807-5.993\"/><path d=\"M15 2c-1.798 1.998-2.518 3.995-2.807 5.993\"/><path d=\"M17 6l-2.5-2.5\"/><path d=\"M14 8l-3-3\"/><path d=\"M7 18l2.5 2.5\"/><path d=\"M3.5 14.5l.5.5\"/><path d=\"M20 9l.5.5\"/><path d=\"M6.5 12.5l1 1\"/><path d=\"M16.5 10.5l1 1\"/><path d=\"M10 16l-1.5 1.5\"/></svg>"),
            ["thermometer"] = new("thermometer", SiIconCategory.Health, "Temperature thermometer",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 4v10.54a4 4 0 11-4 0V4a2 2 0 014 0z\"/></svg>"),
            ["syringe"] = new("syringe", SiIconCategory.Health, "Medical syringe",
                $"<svg {SvgOutlineAttrs}><path d=\"M18 2l4 4\"/><path d=\"M17 7l3-3\"/><path d=\"M19 9l-7 7-4-4 7-7\"/><path d=\"M11 14l-3 3\"/><path d=\"M5 17l-3 3\"/><path d=\"M9 11l-6 6\"/></svg>"),
            ["bandage"] = new("bandage", SiIconCategory.Health, "Medical bandage",
                $"<svg {SvgOutlineAttrs}><path d=\"M18.5 4.5a4.95 4.95 0 00-7 7l-6 6a4.95 4.95 0 107 7l6-6a4.95 4.95 0 000-7z\"/><path d=\"M9 9l6 6\"/><path d=\"M12.5 6.5l.5.5\"/><path d=\"M17 11l.5.5\"/><path d=\"M6.5 12.5l.5.5\"/><path d=\"M11 17l.5.5\"/></svg>"),
            ["hospital"] = new("hospital", SiIconCategory.Health, "Hospital building",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 6v4\"/><path d=\"M14 8h-4\"/><rect x=\"4\" y=\"2\" width=\"16\" height=\"20\" rx=\"2\"/><path d=\"M4 10h16\"/><path d=\"M12 10v12\"/><path d=\"M4 18h16\"/></svg>"),
            ["ambulance"] = new("ambulance", SiIconCategory.Health, "Ambulance vehicle",
                $"<svg {SvgOutlineAttrs}><path d=\"M10 10H6\"/><path d=\"M14 18V6a2 2 0 00-2-2H4a2 2 0 00-2 2v11a1 1 0 001 1h2\"/><path d=\"M19 18h2a1 1 0 001-1v-3.28a1 1 0 00-.684-.948l-1.923-.641-1.119-4.476A1 1 0 0017.307 8H14\"/><path d=\"M8 8v4\"/><path d=\"M9 18h6\"/><circle cx=\"17\" cy=\"18\" r=\"2\"/><circle cx=\"7\" cy=\"18\" r=\"2\"/></svg>"),
            ["medical-cross"] = new("medical-cross", SiIconCategory.Health, "Medical cross symbol",
                $"<svg {SvgOutlineAttrs}><path d=\"M8 2h8v6h6v8h-6v6H8v-6H2V8h6z\"/></svg>"),

            // ============================================
            // Travel Icons
            // ============================================
            ["plane"] = new("plane", SiIconCategory.Travel, "Air travel",
                $"<svg {SvgOutlineAttrs}><path d=\"M17.8 19.2L16 11l3.5-3.5C21 6 21.5 4 21 3c-1-.5-3 0-4.5 1.5L13 8 4.8 6.2c-.5-.1-.9.1-1.1.5l-.3.5c-.2.5-.1 1 .3 1.3L9 12l-2 3H4l-1 1 3 2 2 3 1-1v-3l3-2 3.5 5.3c.3.4.8.5 1.3.3l.5-.2c.4-.3.6-.7.5-1.2z\"/></svg>"),
            ["ship"] = new("ship", SiIconCategory.Travel, "Sea transport",
                $"<svg {SvgOutlineAttrs}><path d=\"M2 21c.6.5 1.2 1 2.5 1 2.5 0 2.5-2 5-2 1.3 0 1.9.5 2.5 1 .6.5 1.2 1 2.5 1 2.5 0 2.5-2 5-2 1.3 0 1.9.5 2.5 1\"/><path d=\"M19.38 20A11.6 11.6 0 0021 14l-9-4-9 4c0 2.9.94 5.34 2.81 7.76\"/><path d=\"M19 13V7a2 2 0 00-2-2H7a2 2 0 00-2 2v6\"/><path d=\"M12 10v4\"/><path d=\"M12 2v3\"/></svg>"),
            ["hotel"] = new("hotel", SiIconCategory.Travel, "Hotel accommodation",
                $"<svg {SvgOutlineAttrs}><path d=\"M18 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V4a2 2 0 00-2-2z\"/><path d=\"M9 22v-4h6v4\"/><path d=\"M8 6h.01\"/><path d=\"M16 6h.01\"/><path d=\"M8 10h.01\"/><path d=\"M16 10h.01\"/><path d=\"M8 14h.01\"/><path d=\"M16 14h.01\"/></svg>"),
            ["suitcase"] = new("suitcase", SiIconCategory.Travel, "Travel suitcase",
                $"<svg {SvgOutlineAttrs}><rect x=\"2\" y=\"7\" width=\"20\" height=\"14\" rx=\"2\"/><path d=\"M16 7V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v3\"/><path d=\"M2 14h20\"/><path d=\"M7 14v3\"/><path d=\"M17 14v3\"/></svg>"),
            ["passport"] = new("passport", SiIconCategory.Travel, "Travel passport",
                $"<svg {SvgOutlineAttrs}><rect x=\"4\" y=\"2\" width=\"16\" height=\"20\" rx=\"2\"/><circle cx=\"12\" cy=\"10\" r=\"3\"/><path d=\"M8 17h8\"/><path d=\"M6 22h12\"/></svg>"),
            ["train"] = new("train", SiIconCategory.Travel, "Train transport",
                $"<svg {SvgOutlineAttrs}><rect x=\"4\" y=\"3\" width=\"16\" height=\"16\" rx=\"2\"/><path d=\"M4 11h16\"/><path d=\"M12 3v8\"/><circle cx=\"8\" cy=\"15\" r=\"1\"/><circle cx=\"16\" cy=\"15\" r=\"1\"/><path d=\"M8 19l-2 3\"/><path d=\"M18 22l-2-3\"/></svg>"),
            ["bus"] = new("bus", SiIconCategory.Travel, "Bus transport",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"18\" height=\"14\" rx=\"2\"/><path d=\"M3 10h18\"/><path d=\"M8 17v2\"/><path d=\"M16 17v2\"/><circle cx=\"7\" cy=\"14\" r=\"1\"/><circle cx=\"17\" cy=\"14\" r=\"1\"/></svg>"),
            ["taxi"] = new("taxi", SiIconCategory.Travel, "Taxi cab",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 5h4l3 5v7a1 1 0 01-1 1h-1\"/><path d=\"M5 17H4a1 1 0 01-1-1v-7l3-5h4\"/><rect x=\"9\" y=\"2\" width=\"6\" height=\"3\"/><circle cx=\"6.5\" cy=\"17.5\" r=\"2.5\"/><circle cx=\"17.5\" cy=\"17.5\" r=\"2.5\"/><path d=\"M9 17h6\"/></svg>"),

            // ============================================
            // Data Icons
            // ============================================
            ["table"] = new("table", SiIconCategory.Data, "Data table",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 3v18\"/><rect x=\"3\" y=\"3\" width=\"18\" height=\"18\" rx=\"2\"/><path d=\"M3 9h18\"/><path d=\"M3 15h18\"/></svg>"),
            ["import"] = new("import", SiIconCategory.Data, "Import data",
                $"<svg {SvgOutlineAttrs}><path d=\"M12 3v12\"/><path d=\"M8 11l4 4 4-4\"/><path d=\"M8 5H4a2 2 0 00-2 2v10a2 2 0 002 2h16a2 2 0 002-2V7a2 2 0 00-2-2h-4\"/></svg>"),
            ["export"] = new("export", SiIconCategory.Data, "Export data",
                $"<svg {SvgOutlineAttrs}><path d=\"M21 15v4a2 2 0 01-2 2H5a2 2 0 01-2-2v-4\"/><polyline points=\"17 8 12 3 7 8\"/><line x1=\"12\" y1=\"3\" x2=\"12\" y2=\"15\"/></svg>"),
            ["api"] = new("api", SiIconCategory.Data, "API endpoint",
                $"<svg {SvgOutlineAttrs}><path d=\"M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z\"/><path d=\"M7 8l3 4-3 4\"/><path d=\"M14 8h3v8h-3\"/></svg>"),
            ["schema"] = new("schema", SiIconCategory.Data, "Data schema",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"6\" height=\"6\" rx=\"1\"/><rect x=\"15\" y=\"3\" width=\"6\" height=\"6\" rx=\"1\"/><rect x=\"9\" y=\"15\" width=\"6\" height=\"6\" rx=\"1\"/><path d=\"M6 9v3a3 3 0 003 3h6a3 3 0 003-3V9\"/><path d=\"M12 12v3\"/></svg>"),
            ["pipeline"] = new("pipeline", SiIconCategory.Data, "Data pipeline",
                $"<svg {SvgOutlineAttrs}><path d=\"M2 12h4\"/><path d=\"M18 12h4\"/><rect x=\"6\" y=\"8\" width=\"4\" height=\"8\" rx=\"1\"/><rect x=\"10\" y=\"6\" width=\"4\" height=\"12\" rx=\"1\"/><rect x=\"14\" y=\"8\" width=\"4\" height=\"8\" rx=\"1\"/></svg>"),
            ["graph-node"] = new("graph-node", SiIconCategory.Data, "Graph node",
                $"<svg {SvgOutlineAttrs}><circle cx=\"5\" cy=\"6\" r=\"3\"/><circle cx=\"19\" cy=\"6\" r=\"3\"/><circle cx=\"12\" cy=\"18\" r=\"3\"/><path d=\"M7.5 7.5l4.5 7.5\"/><path d=\"M16.5 7.5l-4.5 7.5\"/></svg>"),
            ["workflow"] = new("workflow", SiIconCategory.Data, "Workflow process",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"6\" height=\"6\" rx=\"1\"/><rect x=\"15\" y=\"15\" width=\"6\" height=\"6\" rx=\"1\"/><path d=\"M9 6h6\"/><path d=\"M9 18h6\"/><path d=\"M6 9v6\"/><path d=\"M18 9v6\"/></svg>"),
            ["json"] = new("json", SiIconCategory.Data, "JSON format",
                $"<svg {SvgOutlineAttrs}><path d=\"M4 6c0-1.1.9-2 2-2\"/><path d=\"M20 6c0-1.1-.9-2-2-2\"/><path d=\"M4 18c0 1.1.9 2 2 2\"/><path d=\"M20 18c0 1.1-.9 2-2 2\"/><path d=\"M6 4h2\"/><path d=\"M16 4h2\"/><path d=\"M6 20h2\"/><path d=\"M16 20h2\"/><path d=\"M4 8v8\"/><path d=\"M20 8v8\"/><path d=\"M8 12h8\"/></svg>"),
            ["xml"] = new("xml", SiIconCategory.Data, "XML format",
                $"<svg {SvgOutlineAttrs}><path d=\"M5 8l4 4-4 4\"/><path d=\"M19 8l-4 4 4 4\"/><path d=\"M14 4l-4 16\"/></svg>"),
            ["csv"] = new("csv", SiIconCategory.Data, "CSV format",
                $"<svg {SvgOutlineAttrs}><path d=\"M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z\"/><path d=\"M14 2v6h6\"/><path d=\"M8 13h2\"/><path d=\"M8 17h2\"/><path d=\"M14 13h2\"/><path d=\"M14 17h2\"/></svg>"),

            // ============================================
            // Misc Icons (shapes, etc.)
            // ============================================
            ["circle"] = new("circle", SiIconCategory.System, "Circle shape",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/></svg>"),
            ["circle-dot"] = new("circle-dot", SiIconCategory.System, "Circle with dot",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"10\"/><circle cx=\"12\" cy=\"12\" r=\"1\" fill=\"currentColor\"/></svg>"),

            // ============================================
            // Additional AI Management Icons
            // ============================================
            ["workspace"] = new("workspace", SiIconCategory.Ai, "AI workspace",
                $"<svg {SvgOutlineAttrs}><rect x=\"2\" y=\"3\" width=\"20\" height=\"14\" rx=\"2\"/><path d=\"M8 21h8\"/><path d=\"M12 17v4\"/><path d=\"M7 7h10\"/><path d=\"M7 11h10\"/></svg>"),
            ["flask"] = new("flask", SiIconCategory.System, "Laboratory flask for testing",
                $"<svg {SvgOutlineAttrs}><path d=\"M9 3h6\"/><path d=\"M10 3v7.31a2 2 0 01-.33 1.1l-4.34 6.5A2 2 0 007 21h10a2 2 0 001.67-3.09l-4.34-6.5a2 2 0 01-.33-1.1V3\"/><path d=\"M7 17h10\"/></svg>"),
            ["beaker"] = new("beaker", SiIconCategory.System, "Laboratory beaker for testing",
                $"<svg {SvgOutlineAttrs}><path d=\"M6 3h12\"/><path d=\"M8 3v15a3 3 0 003 3h2a3 3 0 003-3V3\"/><path d=\"M9 12h6\"/></svg>"),
            ["activity"] = new("activity", SiIconCategory.System, "Activity or pulse indicator",
                $"<svg {SvgOutlineAttrs}><polyline points=\"22 12 18 12 15 21 9 3 6 12 2 12\"/></svg>"),
            ["analytics"] = new("analytics", SiIconCategory.Charts, "Analytics dashboard",
                $"<svg {SvgOutlineAttrs}><path d=\"M3 3v18h18\"/><path d=\"M18 17V9\"/><path d=\"M13 17V5\"/><path d=\"M8 17v-3\"/></svg>"),
            ["test-tube"] = new("test-tube", SiIconCategory.System, "Test tube for experiments",
                $"<svg {SvgOutlineAttrs}><path d=\"M14.5 2v17.5c0 1.4-1.1 2.5-2.5 2.5s-2.5-1.1-2.5-2.5V2\"/><path d=\"M8.5 2h7\"/><path d=\"M9.5 14.5h5\"/></svg>"),
            ["multimodal"] = new("multimodal", SiIconCategory.Ai, "Multi-modal AI",
                $"<svg {SvgOutlineAttrs}><rect x=\"3\" y=\"3\" width=\"7\" height=\"7\" rx=\"1\"/><rect x=\"14\" y=\"3\" width=\"7\" height=\"7\" rx=\"1\"/><rect x=\"14\" y=\"14\" width=\"7\" height=\"7\" rx=\"1\"/><rect x=\"3\" y=\"14\" width=\"7\" height=\"7\" rx=\"1\"/></svg>"),
            ["indexing"] = new("indexing", SiIconCategory.Data, "Data indexing",
                $"<svg {SvgOutlineAttrs}><path d=\"M16 6l-4 14\"/><path d=\"M12 6l-4 14\"/><path d=\"M8 8l12 0\"/><path d=\"M4 16l12 0\"/></svg>"),
            ["mcp"] = new("mcp", SiIconCategory.Ai, "Model Context Protocol",
                $"<svg {SvgOutlineAttrs}><circle cx=\"12\" cy=\"12\" r=\"3\"/><path d=\"M12 1v6\"/><path d=\"M12 17v6\"/><path d=\"M4.22 4.22l4.24 4.24\"/><path d=\"M15.54 15.54l4.24 4.24\"/><path d=\"M1 12h6\"/><path d=\"M17 12h6\"/><path d=\"M4.22 19.78l4.24-4.24\"/><path d=\"M15.54 8.46l4.24-4.24\"/></svg>"),
            ["rag"] = new("rag", SiIconCategory.Ai, "Retrieval Augmented Generation",
                $"<svg {SvgOutlineAttrs}><circle cx=\"11\" cy=\"11\" r=\"8\"/><path d=\"M21 21l-4.35-4.35\"/><path d=\"M11 8v6\"/><path d=\"M8 11h6\"/></svg>"),
        };
    }
}
