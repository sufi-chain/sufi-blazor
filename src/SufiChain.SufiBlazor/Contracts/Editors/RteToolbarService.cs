using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Service for managing rich text editor toolbar contributors.
/// </summary>
public class RteToolbarService : IRteToolbarService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RteToolbarOptions _options;

    public RteToolbarService(
        IServiceProvider serviceProvider,
        IOptions<RteToolbarOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    /// <summary>
    /// Get all toolbar items including default and contributed items.
    /// </summary>
    /// <param name="editorId">The editor instance ID.</param>
    /// <param name="includeDefaults">Whether to include default toolbar items.</param>
    public async Task<List<SbEditorToolbarItem>> GetToolbarItemsAsync(
        string? editorId = null,
        bool? includeDefaults = null)
    {
        var shouldIncludeDefaults = includeDefaults ?? _options.IncludeDefaultItems;
        var allItems = new List<RteToolbarContributedItem>();

        // Add default items if requested
        if (shouldIncludeDefaults)
        {
            allItems.AddRange(GetDefaultItems());
        }

        // Get contributed items from all registered contributors
        var context = new RteToolbarContext(_serviceProvider) { EditorId = editorId };

        var contributors = GetContributors();
        foreach (var contributor in contributors.OrderBy(c => c.Order))
        {
            await contributor.ConfigureToolbarAsync(context);
        }

        allItems.AddRange(context.Items);

        // Sort by group order, then by item order within group
        var groupOrder = _options.GroupOrder;
        var sortedItems = allItems
            .Where(item => item.IsVisible?.Invoke() ?? true)
            .OrderBy(item => 
            {
                var groupIndex = groupOrder.IndexOf(item.Group);
                return groupIndex >= 0 ? groupIndex : int.MaxValue;
            })
            .ThenBy(item => item.Order)
            .ToList();

        // Insert separators between groups
        var result = new List<SbEditorToolbarItem>();
        string? lastGroup = null;

        foreach (var item in sortedItems)
        {
            if (lastGroup != null && lastGroup != item.Group && !item.IsSeparator)
            {
                result.Add(SbEditorToolbarItem.Separator);
            }
            result.Add(item);
            lastGroup = item.Group;
        }

        return result;
    }

    /// <summary>
    /// Get the contributed items only (without defaults).
    /// </summary>
    public async Task<List<RteToolbarContributedItem>> GetContributedItemsAsync(string? editorId = null)
    {
        var context = new RteToolbarContext(_serviceProvider) { EditorId = editorId };

        var contributors = GetContributors();
        foreach (var contributor in contributors.OrderBy(c => c.Order))
        {
            await contributor.ConfigureToolbarAsync(context);
        }

        return context.Items;
    }

    /// <summary>
    /// Execute a contributed toolbar item's click handler.
    /// </summary>
    public async Task ExecuteItemActionAsync(
        RteToolbarContributedItem item,
        RteToolbarActionContext actionContext)
    {
        if (item.OnClickAsync != null)
        {
            await item.OnClickAsync(actionContext);
        }
    }

    private IEnumerable<IRteToolbarContributor> GetContributors()
    {
        foreach (var contributorType in _options.Contributors)
        {
            var contributor = _serviceProvider.GetService(contributorType) as IRteToolbarContributor;
            if (contributor != null)
            {
                yield return contributor;
            }
        }
    }

    private static List<RteToolbarContributedItem> GetDefaultItems()
    {
        return new List<RteToolbarContributedItem>
        {
            // History group
            new() { Id = "undo", Group = "history", Order = 0, CustomAction = "undo", Icon = "↶", Tooltip = "Undo (Ctrl+Z)" },
            new() { Id = "redo", Group = "history", Order = 1, CustomAction = "redo", Icon = "↷", Tooltip = "Redo (Ctrl+Y)" },
            
            // Heading group
            new()
            {
                Id = "header",
                Group = "heading",
                Order = 0,
                Type = SbEditorToolbarItemType.Select,
                Format = "header",
                Tooltip = "Heading",
                Options = new List<SbEditorToolbarOption>
                {
                    new() { Label = "Normal", LabelKey = "Rte:HeaderNormal", Value = false },
                    new() { Label = "Heading 1", LabelKey = "Rte:Header1", Value = 1 },
                    new() { Label = "Heading 2", LabelKey = "Rte:Header2", Value = 2 },
                    new() { Label = "Heading 3", LabelKey = "Rte:Header3", Value = 3 },
                    new() { Label = "Heading 4", LabelKey = "Rte:Header4", Value = 4 },
                    new() { Label = "Heading 5", LabelKey = "Rte:Header5", Value = 5 },
                    new() { Label = "Heading 6", LabelKey = "Rte:Header6", Value = 6 },
                }
            },
            
            // Formatting group
            new() { Id = "bold", Group = "formatting", Order = 0, Format = "bold", Icon = "B", Tooltip = "Bold (Ctrl+B)" },
            new() { Id = "italic", Group = "formatting", Order = 1, Format = "italic", Icon = "I", Tooltip = "Italic (Ctrl+I)" },
            new() { Id = "underline", Group = "formatting", Order = 2, Format = "underline", Icon = "U", Tooltip = "Underline (Ctrl+U)" },
            new() { Id = "strike", Group = "formatting", Order = 3, Format = "strike", Icon = "S", Tooltip = "Strikethrough" },
            
            // List group
            new() { Id = "ol", Group = "list", Order = 0, Format = "list", FormatValue = "ordered", Icon = "1.", Tooltip = "Numbered List" },
            new() { Id = "ul", Group = "list", Order = 1, Format = "list", FormatValue = "bullet", Icon = "•", Tooltip = "Bullet List" },
            
            // Alignment group
            new()
            {
                Id = "align",
                Group = "alignment",
                Order = 0,
                Type = SbEditorToolbarItemType.Select,
                Format = "align",
                Tooltip = "Alignment",
                Options = new List<SbEditorToolbarOption>
                {
                    new() { Label = "Left", LabelKey = "Rte:AlignLeft", Value = false },
                    new() { Label = "Center", LabelKey = "Rte:AlignCenter", Value = "center" },
                    new() { Label = "Right", LabelKey = "Rte:AlignRight", Value = "right" },
                    new() { Label = "Justify", LabelKey = "Rte:AlignJustify", Value = "justify" },
                }
            },
            
            // Insert group
            new() { Id = "link", Group = "insert", Order = 0, Format = "link", Icon = "🔗", Tooltip = "Insert Link" },
            new() { Id = "image", Group = "insert", Order = 1, Format = "image", Icon = "🖼️", Tooltip = "Insert Image" },
            
            // Blocks group
            new() { Id = "blockquote", Group = "blocks", Order = 0, Format = "blockquote", Icon = "❝", Tooltip = "Quote" },
            new() { Id = "code-block", Group = "blocks", Order = 1, Format = "code-block", Icon = "</>", Tooltip = "Code Block" },
            
            // Actions group
            new() { Id = "clean", Group = "actions", Order = 0, CustomAction = "clean", Icon = "✕", Tooltip = "Clear Formatting" }
        };
    }
}

/// <summary>
/// Interface for the toolbar service.
/// </summary>
public interface IRteToolbarService
{
    /// <summary>
    /// Get all toolbar items including default and contributed items.
    /// </summary>
    Task<List<SbEditorToolbarItem>> GetToolbarItemsAsync(string? editorId = null, bool? includeDefaults = null);

    /// <summary>
    /// Get the contributed items only (without defaults).
    /// </summary>
    Task<List<RteToolbarContributedItem>> GetContributedItemsAsync(string? editorId = null);

    /// <summary>
    /// Execute a contributed toolbar item's click handler.
    /// </summary>
    Task ExecuteItemActionAsync(RteToolbarContributedItem item, RteToolbarActionContext actionContext);
}
