using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Interface for contributing toolbar items to the markdown editor.
/// </summary>
public interface IMdToolbarContributor
{
    /// <summary>
    /// The order in which this contributor's items should appear.
    /// </summary>
    int Order => 100;

    /// <summary>
    /// The toolbar scope this contributor belongs to. When non-null, the
    /// contributor only runs on editors whose <c>ToolbarScope</c> parameter
    /// matches this value. When null (default), the contributor runs on every
    /// editor instance — use this for self-filtering contributors (e.g. ones
    /// that check a host registration) or for globally-applicable items.
    /// </summary>
    string? Scope => null;

    /// <summary>
    /// Configure the toolbar by adding custom items to the context.
    /// </summary>
    Task ConfigureToolbarAsync(MdToolbarContext context);
}

/// <summary>
/// Context passed to markdown toolbar contributors.
/// </summary>
public class MdToolbarContext
{
    public List<MdToolbarContributedItem> Items { get; } = new();
    public IServiceProvider ServiceProvider { get; }
    public string? EditorId { get; set; }

    /// <summary>
    /// The scope declared by the editor instance via its <c>ToolbarScope</c>
    /// parameter. Contributors whose <see cref="IMdToolbarContributor.Scope"/>
    /// is non-null only run when this value matches.
    /// </summary>
    public string? Scope { get; set; }

    public MdToolbarContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}

/// <summary>
/// A toolbar item contributed by an <see cref="IMdToolbarContributor"/>.
/// </summary>
public class MdToolbarContributedItem : SbMarkdownToolbarItem
{
    public string Group { get; set; } = "custom";
    public int Order { get; set; }
    public Func<MdToolbarActionContext, Task>? OnClickAsync { get; set; }
    public Func<bool>? IsVisible { get; set; }
    public Func<bool>? IsEnabled { get; set; }
}

/// <summary>
/// Context passed to markdown toolbar item click handlers.
/// </summary>
public class MdToolbarActionContext
{
    public string EditorId { get; }
    public IServiceProvider ServiceProvider { get; }
    public Func<string, Task>? InsertTextAsync { get; set; }
    public Func<string, string?, Task>? InsertImageMarkdownAsync { get; set; }
    public Func<string, string?, Task>? InsertLinkMarkdownAsync { get; set; }
    public Func<Task<string?>>? GetSelectionAsync { get; set; }
    public Func<Task<string?>>? GetValueAsync { get; set; }

    public MdToolbarActionContext(string editorId, IServiceProvider serviceProvider)
    {
        EditorId = editorId;
        ServiceProvider = serviceProvider;
    }
}
