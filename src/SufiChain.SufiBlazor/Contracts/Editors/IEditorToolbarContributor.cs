namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Contributes toolbar, bubble, slash, or block-menu items to any Sb editor surface.
/// </summary>
public interface IEditorToolbarContributor
{
    int Order => 100;

    string? Scope => null;

    SbEditorSurface Surfaces => SbEditorSurface.Toolbar;

    Task ConfigureAsync(EditorToolbarContext context);
}

/// <summary>
/// Context passed to <see cref="IEditorToolbarContributor.ConfigureAsync"/>.
/// </summary>
public sealed class EditorToolbarContext
{
    public List<EditorToolbarItem> Items { get; } = new();
    public HashSet<string> HiddenItemIds { get; } = new(StringComparer.Ordinal);
    public IServiceProvider ServiceProvider { get; }
    public string? EditorId { get; set; }
    public string? Scope { get; set; }
    public SbEditorSurface Surface { get; set; } = SbEditorSurface.Toolbar;
    public SbContentFormat ContentFormat { get; set; } = SbContentFormat.Html;
    public ISbEditorDocument? Document { get; set; }

    public EditorToolbarContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}

/// <summary>
/// A contributed or built-in editor toolbar item.
/// </summary>
public class EditorToolbarItem
{
    public string Id { get; set; } = "";
    public string Group { get; set; } = "custom";
    public int Order { get; set; }
    public string? Icon { get; set; }
    public string? IconName { get; set; }
    public string? Label { get; set; }
    public string? LabelKey { get; set; }
    public string? Tooltip { get; set; }
    public string? Shortcut { get; set; }
    public bool IsSeparator { get; set; }
    public SbEditorToolbarItemType Type { get; set; } = SbEditorToolbarItemType.Button;
    public SbEditorCommand? Command { get; set; }
    public IReadOnlyList<SbEditorToolbarOption>? Options { get; set; }
    public List<EditorToolbarItem> Children { get; set; } = new();
    public SbEditorSurface Surfaces { get; set; } = SbEditorSurface.Toolbar;
    public Func<EditorState, bool>? IsVisible { get; set; }
    public Func<EditorState, bool>? IsEnabled { get; set; }
    public Func<EditorState, bool>? IsActive { get; set; }
    public Func<EditorActionContext, Task>? OnClickAsync { get; set; }

    public static EditorToolbarItem Separator => new() { IsSeparator = true };
}

/// <summary>
/// Click context for a toolbar item. Exposes <see cref="ISbEditorDocument"/> instead of engine APIs.
/// </summary>
public sealed class EditorActionContext
{
    public string EditorId { get; }
    public IServiceProvider ServiceProvider { get; }
    public SbEditorSurface Surface { get; set; }
    public SbContentFormat ContentFormat { get; set; }
    public ISbEditorDocument Document { get; }
    public EditorState? State { get; set; }

    public EditorActionContext(
        string editorId,
        IServiceProvider serviceProvider,
        ISbEditorDocument document)
    {
        EditorId = editorId;
        ServiceProvider = serviceProvider;
        Document = document;
    }
}
