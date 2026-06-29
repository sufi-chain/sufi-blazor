using System;
using System.Threading.Tasks;

namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Interface for contributing toolbar items to the rich text editor.
/// Implement this interface to add custom toolbar buttons from modules.
/// </summary>
public interface IRteToolbarContributor
{
    /// <summary>
    /// The order in which this contributor's items should appear.
    /// Lower values appear first. Default toolbar items use order 0-100.
    /// Use values > 100 to add items after default items, or negative values to add before.
    /// </summary>
    int Order => 100;

    /// <summary>
    /// The toolbar scope this contributor belongs to. When non-null, the
    /// contributor only runs on editors whose <c>ToolbarScope</c> parameter
    /// matches this value. When null (default), the contributor runs on every
    /// editor instance — use this for self-filtering contributors or for
    /// globally-applicable items (e.g. culture-aware font selectors).
    /// </summary>
    string? Scope => null;

    /// <summary>
    /// Configure the toolbar by adding custom items to the context.
    /// </summary>
    /// <param name="context">The toolbar context containing items and services.</param>
    Task ConfigureToolbarAsync(RteToolbarContext context);
}

/// <summary>
/// Context passed to toolbar contributors for configuring toolbar items.
/// </summary>
public class RteToolbarContext
{
    /// <summary>
    /// The collection of toolbar items. Contributors can add items to this list.
    /// </summary>
    public List<RteToolbarContributedItem> Items { get; } = new();

    /// <summary>
    /// The service provider for resolving services.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// The editor instance ID. Can be used for editor-specific operations.
    /// </summary>
    public string? EditorId { get; set; }

    /// <summary>
    /// The scope declared by the editor instance via its <c>ToolbarScope</c>
    /// parameter. Contributors whose <see cref="IRteToolbarContributor.Scope"/>
    /// is non-null only run when this value matches.
    /// </summary>
    public string? Scope { get; set; }

    public RteToolbarContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}

/// <summary>
/// A toolbar item contributed by an IRteToolbarContributor.
/// Extends SbEditorToolbarItem with callback support.
/// </summary>
public class RteToolbarContributedItem : SbEditorToolbarItem
{
    /// <summary>
    /// The group this item belongs to (e.g., "formatting", "insert", "actions").
    /// Items in the same group are placed together.
    /// </summary>
    public string Group { get; set; } = "custom";

    /// <summary>
    /// The order within the group. Lower values appear first.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Callback invoked when the toolbar button is clicked.
    /// Use this for custom actions that require C# code execution.
    /// </summary>
    public Func<RteToolbarActionContext, Task>? OnClickAsync { get; set; }

    /// <summary>
    /// Whether this item should be visible.
    /// Can be used to conditionally show/hide items.
    /// </summary>
    public Func<bool>? IsVisible { get; set; }

    /// <summary>
    /// Whether this item should be enabled.
    /// Can be used to conditionally enable/disable items.
    /// </summary>
    public Func<bool>? IsEnabled { get; set; }
}

/// <summary>
/// Context passed to toolbar item click handlers.
/// </summary>
public class RteToolbarActionContext
{
    /// <summary>
    /// The editor instance ID.
    /// </summary>
    public string EditorId { get; }

    /// <summary>
    /// The service provider for resolving services.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Action to insert HTML content at the current cursor position.
    /// </summary>
    public Func<string, Task>? InsertHtmlAsync { get; set; }

    /// <summary>
    /// Action to insert an image at the current cursor position.
    /// </summary>
    public Func<string, string?, Task>? InsertImageAsync { get; set; }

    /// <summary>
    /// Action to insert a link at the current selection.
    /// </summary>
    public Func<string, string?, Task>? InsertLinkAsync { get; set; }

    /// <summary>
    /// Action to get the current selection text.
    /// </summary>
    public Func<Task<string?>>? GetSelectionAsync { get; set; }

    /// <summary>
    /// Action to get the current HTML content.
    /// </summary>
    public Func<Task<string?>>? GetHtmlAsync { get; set; }

    public RteToolbarActionContext(string editorId, IServiceProvider serviceProvider)
    {
        EditorId = editorId;
        ServiceProvider = serviceProvider;
    }
}
