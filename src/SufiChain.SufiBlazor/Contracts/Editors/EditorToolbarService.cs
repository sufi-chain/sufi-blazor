using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Resolves default and contributed toolbar items for an editor instance.
/// </summary>
public interface IEditorToolbarService
{
    Task<List<EditorToolbarItem>> GetToolbarItemsAsync(
        string? editorId = null,
        bool? includeDefaults = null,
        string? scope = null,
        SbEditorSurface surface = SbEditorSurface.Toolbar,
        SbContentFormat contentFormat = SbContentFormat.Html,
        ISbEditorDocument? document = null);
}

/// <summary>
/// Default <see cref="IEditorToolbarService"/> implementation.
/// </summary>
public sealed class EditorToolbarService : IEditorToolbarService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SbEditorOptions _options;

    public EditorToolbarService(IServiceProvider serviceProvider, IOptions<SbEditorOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    public async Task<List<EditorToolbarItem>> GetToolbarItemsAsync(
        string? editorId = null,
        bool? includeDefaults = null,
        string? scope = null,
        SbEditorSurface surface = SbEditorSurface.Toolbar,
        SbContentFormat contentFormat = SbContentFormat.Html,
        ISbEditorDocument? document = null)
    {
        var include = includeDefaults ?? _options.IncludeDefaultItems;
        var items = new List<EditorToolbarItem>();

        if (include)
        {
            items.AddRange(CreateDefaultItems().Where(item => item.Surfaces.HasFlag(surface)));
        }

        var context = new EditorToolbarContext(_serviceProvider)
        {
            EditorId = editorId,
            Scope = scope,
            Surface = surface,
            ContentFormat = contentFormat,
            Document = document
        };

        foreach (var contributor in GetContributors(scope, surface).OrderBy(c => c.Order))
        {
            await contributor.ConfigureAsync(context);
        }

        items.AddRange(context.Items);
        if (context.HiddenItemIds.Count > 0)
        {
            items.RemoveAll(item => !item.IsSeparator && context.HiddenItemIds.Contains(item.Id));
        }

        var groupOrder = _options.GroupOrder;
        var sorted = items
            .OrderBy(item =>
            {
                var index = groupOrder.IndexOf(item.Group);
                return index >= 0 ? index : int.MaxValue;
            })
            .ThenBy(item => item.Order)
            .ToList();

        var result = new List<EditorToolbarItem>();
        string? lastGroup = null;
        foreach (var item in sorted)
        {
            if (item.IsSeparator)
            {
                result.Add(item);
                lastGroup = item.Group;
                continue;
            }

            if (lastGroup != null && lastGroup != item.Group)
            {
                result.Add(EditorToolbarItem.Separator);
            }

            result.Add(item);
            lastGroup = item.Group;
        }

        return result;
    }

    private IEnumerable<IEditorToolbarContributor> GetContributors(string? scope, SbEditorSurface surface)
    {
        foreach (var type in _options.Contributors)
        {
            if (_serviceProvider.GetService(type) is not IEditorToolbarContributor contributor)
            {
                continue;
            }

            if (contributor.Scope != null &&
                !string.Equals(contributor.Scope, scope, StringComparison.Ordinal))
            {
                continue;
            }

            if ((contributor.Surfaces & surface) == 0)
            {
                continue;
            }

            yield return contributor;
        }
    }

    public static List<EditorToolbarItem> CreateDefaultItems()
    {
        return
        [
            new() { Id = "undo", Group = "history", Order = 0, Command = SbEditorCommand.Undo, IconName = "undo", LabelKey = "Editor:Undo", Tooltip = "Undo (Ctrl+Z)", Shortcut = "Mod-z" },
            new() { Id = "redo", Group = "history", Order = 1, Command = SbEditorCommand.Redo, IconName = "redo", LabelKey = "Editor:Redo", Tooltip = "Redo (Ctrl+Y)", Shortcut = "Mod-y" },
            new()
            {
                Id = "heading",
                Group = "heading",
                Order = 0,
                Type = SbEditorToolbarItemType.Select,
                IconName = "heading",
                LabelKey = "Editor:Heading",
                Tooltip = "Heading",
                Options = new List<SbEditorToolbarOption>
                {
                    new() { Label = "Normal", LabelKey = "Editor:HeaderNormal", Value = 0 },
                    new() { Label = "Heading 1", LabelKey = "Editor:Header1", Value = 1 },
                    new() { Label = "Heading 2", LabelKey = "Editor:Header2", Value = 2 },
                    new() { Label = "Heading 3", LabelKey = "Editor:Header3", Value = 3 },
                    new() { Label = "Heading 4", LabelKey = "Editor:Header4", Value = 4 },
                    new() { Label = "Heading 5", LabelKey = "Editor:Header5", Value = 5 },
                    new() { Label = "Heading 6", LabelKey = "Editor:Header6", Value = 6 },
                }
            },
            new() { Id = "bold", Group = "formatting", Order = 0, Command = SbEditorCommand.Bold, IconName = "bold", LabelKey = "Editor:Bold", Tooltip = "Bold (Ctrl+B)", Surfaces = SbEditorSurface.Toolbar | SbEditorSurface.BubbleMenu, IsActive = s => s.IsMarkActive("bold") },
            new() { Id = "italic", Group = "formatting", Order = 1, Command = SbEditorCommand.Italic, IconName = "italic", LabelKey = "Editor:Italic", Tooltip = "Italic (Ctrl+I)", Surfaces = SbEditorSurface.Toolbar | SbEditorSurface.BubbleMenu, IsActive = s => s.IsMarkActive("italic") },
            new() { Id = "underline", Group = "formatting", Order = 2, Command = SbEditorCommand.Underline, IconName = "underline", LabelKey = "Editor:Underline", Tooltip = "Underline (Ctrl+U)", Surfaces = SbEditorSurface.Toolbar | SbEditorSurface.BubbleMenu, IsActive = s => s.IsMarkActive("underline") },
            new() { Id = "strike", Group = "formatting", Order = 3, Command = SbEditorCommand.Strike, Icon = "S", LabelKey = "Editor:Strike", Tooltip = "Strikethrough", Surfaces = SbEditorSurface.Toolbar | SbEditorSurface.BubbleMenu, IsActive = s => s.IsMarkActive("strike") },
            new() { Id = "ol", Group = "list", Order = 0, Command = SbEditorCommand.OrderedList, Icon = "1.", LabelKey = "Editor:OrderedList", Tooltip = "Numbered list", IsActive = s => s.IsNodeActive("orderedList") },
            new() { Id = "ul", Group = "list", Order = 1, Command = SbEditorCommand.BulletList, IconName = "list", LabelKey = "Editor:BulletList", Tooltip = "Bullet list", IsActive = s => s.IsNodeActive("bulletList") },
            new() { Id = "align-left", Group = "alignment", Order = 0, Command = SbEditorCommand.AlignLeft, IconName = "align-left", LabelKey = "Editor:AlignLeft", Tooltip = "Align left", IsActive = s => s.IsAlignActive("left") },
            new() { Id = "align-center", Group = "alignment", Order = 1, Command = SbEditorCommand.AlignCenter, IconName = "align-center", LabelKey = "Editor:AlignCenter", Tooltip = "Align center", IsActive = s => s.IsAlignActive("center") },
            new() { Id = "align-right", Group = "alignment", Order = 2, Command = SbEditorCommand.AlignRight, IconName = "align-right", LabelKey = "Editor:AlignRight", Tooltip = "Align right", IsActive = s => s.IsAlignActive("right") },
            new() { Id = "link", Group = "insert", Order = 0, Command = SbEditorCommand.InsertLink, IconName = "link", LabelKey = "Editor:Link", Tooltip = "Insert link", Surfaces = SbEditorSurface.Toolbar | SbEditorSurface.BubbleMenu, IsActive = s => s.IsMarkActive("link") },
            new() { Id = "image", Group = "insert", Order = 1, Command = SbEditorCommand.InsertImage, IconName = "image", LabelKey = "Editor:Image", Tooltip = "Insert image" },
            new() { Id = "blockquote", Group = "blocks", Order = 0, Command = SbEditorCommand.Blockquote, Icon = "❝", LabelKey = "Editor:Quote", Tooltip = "Quote", Surfaces = SbEditorSurface.Toolbar | SbEditorSurface.SlashMenu, IsActive = s => s.IsNodeActive("blockquote") },
            new() { Id = "code-block", Group = "blocks", Order = 1, Command = SbEditorCommand.CodeBlock, IconName = "code", LabelKey = "Editor:CodeBlock", Tooltip = "Code block", Surfaces = SbEditorSurface.Toolbar | SbEditorSurface.SlashMenu, IsActive = s => s.IsNodeActive("codeBlock") },
            new() { Id = "task-list", Group = "list", Order = 2, Command = SbEditorCommand.TaskList, IconName = "clipboard-list", Icon = "☑", LabelKey = "Editor:TaskList", Tooltip = "Task list", Surfaces = SbEditorSurface.Toolbar | SbEditorSurface.SlashMenu, IsActive = s => s.IsNodeActive("taskList") },
            new() { Id = "table", Group = "insert", Order = 2, Command = SbEditorCommand.InsertTable, IconName = "table", LabelKey = "Editor:Table", Tooltip = "Insert table", Surfaces = SbEditorSurface.Toolbar | SbEditorSurface.SlashMenu },
            new() { Id = "callout", Group = "blocks", Order = 2, Command = SbEditorCommand.InsertCallout, IconName = "info", LabelKey = "Editor:Callout", Tooltip = "Insert callout", Surfaces = SbEditorSurface.Toolbar | SbEditorSurface.SlashMenu, IsActive = s => s.IsNodeActive("sufiCallout") },
            new() { Id = "clean", Group = "actions", Order = 0, Command = SbEditorCommand.ClearFormatting, Icon = "✕", LabelKey = "Editor:ClearFormatting", Tooltip = "Clear formatting" }
        ];
    }
}
