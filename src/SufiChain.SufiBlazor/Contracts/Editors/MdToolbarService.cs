using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Service for managing markdown editor toolbar contributors.
/// </summary>
public class MdToolbarService : IMdToolbarService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MdToolbarOptions _options;

    public MdToolbarService(IServiceProvider serviceProvider, IOptions<MdToolbarOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    public async Task<List<SbMarkdownToolbarItem>> GetToolbarItemsAsync(
        string? editorId = null,
        bool? includeDefaults = null,
        bool includeContributors = true,
        string? scope = null)
    {
        var shouldIncludeDefaults = includeDefaults ?? _options.IncludeDefaultItems;
        var allItems = new List<MdToolbarContributedItem>();

        if (shouldIncludeDefaults)
        {
            allItems.AddRange(GetDefaultItems());
        }

        if (includeContributors)
        {
            var context = new MdToolbarContext(_serviceProvider)
            {
                EditorId = editorId,
                Scope = scope
            };
            foreach (var contributor in GetContributors(scope).OrderBy(c => c.Order))
            {
                await contributor.ConfigureToolbarAsync(context);
            }

            allItems.AddRange(context.Items);
        }

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

        var result = new List<SbMarkdownToolbarItem>();
        string? lastGroup = null;

        foreach (var item in sortedItems)
        {
            if (lastGroup != null && lastGroup != item.Group && !item.IsSeparator)
            {
                result.Add(SbMarkdownToolbarItem.Separator);
            }
            result.Add(item);
            lastGroup = item.Group;
        }

        return result;
    }

    public async Task ExecuteItemActionAsync(MdToolbarContributedItem item, MdToolbarActionContext actionContext)
    {
        if (item.OnClickAsync != null)
        {
            await item.OnClickAsync(actionContext);
        }
    }

    private IEnumerable<IMdToolbarContributor> GetContributors(string? scope)
    {
        foreach (var contributorType in _options.Contributors)
        {
            if (_serviceProvider.GetService(contributorType) is IMdToolbarContributor contributor
                && (contributor.Scope == null || string.Equals(contributor.Scope, scope, StringComparison.Ordinal)))
            {
                yield return contributor;
            }
        }
    }

    private static List<MdToolbarContributedItem> GetDefaultItems()
    {
        return new List<MdToolbarContributedItem>
        {
            new() { Id = "undo", Group = "history", Order = 0, Action = "undo", IconName = "undo", Tooltip = "Undo (Ctrl+Z)" },
            new() { Id = "redo", Group = "history", Order = 1, Action = "redo", IconName = "redo", Tooltip = "Redo (Ctrl+Y)" },
            new() { Id = "bold", Group = "formatting", Order = 0, Action = "bold", IconName = "bold", Icon = "B", Tooltip = "Bold (Ctrl+B)" },
            new() { Id = "italic", Group = "formatting", Order = 1, Action = "italic", IconName = "italic", Icon = "I", Tooltip = "Italic (Ctrl+I)" },
            new() { Id = "strikethrough", Group = "formatting", Order = 2, Action = "strikethrough", Icon = "S", Tooltip = "Strikethrough" },
            new() { Id = "heading-2", Group = "heading", Order = 0, Action = "heading-2", IconName = "heading", Tooltip = "Heading" },
            new() { Id = "unordered-list", Group = "list", Order = 0, Action = "unordered-list", IconName = "list", Icon = "•", Tooltip = "Bullet List" },
            new() { Id = "ordered-list", Group = "list", Order = 1, Action = "ordered-list", Icon = "1.", Tooltip = "Numbered List" },
            new() { Id = "quote", Group = "list", Order = 2, Action = "quote", Icon = "❝", Tooltip = "Quote" },
            new() { Id = "code", Group = "list", Order = 3, Action = "code", Icon = "</>", Tooltip = "Code Block" },
            new() { Id = "link", Group = "insert", Order = 0, Action = "link", Icon = "🔗", Tooltip = "Insert Link" },
            new() { Id = "image", Group = "insert", Order = 1, Action = "image", Icon = "🖼️", Tooltip = "Insert Image" },
            new() { Id = "preview", Group = "view", Order = 0, Action = "preview", IconName = "eye", Tooltip = "Toggle Preview" },
            new() { Id = "side-by-side", Group = "view", Order = 1, Action = "side-by-side", IconName = "columns", Tooltip = "Side by Side" },
            new() { Id = "fullscreen", Group = "view", Order = 2, Action = "fullscreen", IconName = "expand", Tooltip = "Fullscreen" }
        };
    }
}

/// <summary>
/// Interface for the markdown toolbar service.
/// </summary>
public interface IMdToolbarService
{
    Task<List<SbMarkdownToolbarItem>> GetToolbarItemsAsync(
        string? editorId = null,
        bool? includeDefaults = null,
        bool includeContributors = true,
        string? scope = null);
    Task ExecuteItemActionAsync(MdToolbarContributedItem item, MdToolbarActionContext actionContext);
}
