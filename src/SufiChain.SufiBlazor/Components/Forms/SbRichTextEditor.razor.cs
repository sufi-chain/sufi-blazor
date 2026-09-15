using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Contracts.Editors;
using SufiChain.SufiBlazor.Interop;
using SufiChain.SufiBlazor.Localization;

namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// Canonical rich document editor backed by Tiptap. JS is limited to the editor runtime.
/// </summary>
public partial class SbRichTextEditor : ComponentBase, IAsyncDisposable, ISbEditorDocument
{
    private ElementReference _editorContainer;
    private SbRichTextEditorInterop? _interop;
    private DotNetObjectReference<SbRichTextEditor>? _dotNetRef;
    private string? _editorId;
    private bool _useFallback;
    private bool _disposed;
    private string? _lastValue;
    private List<EditorToolbarItem> _toolbarItems = EditorToolbarService.CreateDefaultItems();
    private EditorState _state = new();
    private readonly List<SbEditorSuggestion> _suggestions = new();
    private string? _suggestionBarStyle;
    private bool _aiMenuOpen;
    private string _aiPrompt = "";
    private string? _aiStatus;
    private bool _linkDialogOpen;
    private string _linkUrl = "";
    private string _linkText = "";
    private bool _imageDialogOpen;
    private string _imageUrl = "";
    private string _imageAlt = "";
    private ElementReference _bubbleMenu;
    private List<EditorToolbarItem> _bubbleItems = [];
    private List<EditorToolbarItem> _slashItems = [];
    private bool _slashMenuOpen;
    private string? _appliedDirection;

    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public SbEditorMode Mode { get; set; } = SbEditorMode.Wysiwyg;
    [Parameter] public SbContentFormat ContentFormat { get; set; } = SbContentFormat.Html;
    [Parameter] public SbEditorLayout Layout { get; set; } = SbEditorLayout.Document;
    [Parameter] public SbEditorFeatures Features { get; set; } = SbEditorFeatures.Default;
    [Parameter] public IReadOnlyList<SbEditorToolbarItem>? ToolbarItems { get; set; }
    [Parameter] public bool UseToolbarContributors { get; set; }
    [Parameter] public bool IncludeDefaultToolbarItems { get; set; } = true;
    [Parameter] public string? ToolbarScope { get; set; }
    [Parameter] public bool HideToolbar { get; set; }
    [Parameter] public bool SourceEditorEnabled { get; set; }
    [Parameter] public SbPasteCleanupOptions? PasteCleanupOptions { get; set; }
    [Parameter] public Func<IBrowserFile, Task<SbImageUploadResult>>? OnImageUpload { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool? RightToLeft { get; set; }
    [Parameter] public string? Height { get; set; }
    [Parameter] public string? MinHeight { get; set; }
    [Parameter] public string? MaxHeight { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public bool ShowCharacterCount { get; set; }
    [Parameter] public bool ShowWordCount { get; set; }
    [Parameter] public string WordCountFormat { get; set; } = "{0} words";
    [Parameter] public string CharacterCountFormat { get; set; } = "{0} characters";
    [Parameter] public string ToolbarAriaLabel { get; set; } = "Editor toolbar";
    [Parameter] public string EditorAriaLabel { get; set; } = "Rich text editor";
    [Parameter] public EventCallback<string> OnShortcut { get; set; }
    [Parameter] public EventCallback OnChange { get; set; }
    [Parameter] public EventCallback OnFocus { get; set; }
    [Parameter] public EventCallback OnBlur { get; set; }
    [Parameter] public string LinkDialogTitle { get; set; } = "Insert link";
    [Parameter] public string ImageDialogTitle { get; set; } = "Insert image";
    [Parameter] public string UrlLabel { get; set; } = "URL";
    [Parameter] public string LinkTextLabel { get; set; } = "Text";
    [Parameter] public string ImageUrlLabel { get; set; } = "Image URL";
    [Parameter] public string AltTextLabel { get; set; } = "Alt text";
    [Parameter] public string? CancelText { get; set; }
    [Parameter] public string? ApplyText { get; set; }
    [Parameter] public string InsertText { get; set; } = "Insert";
    [Parameter] public string BubbleMenuAriaLabel { get; set; } = "Selection toolbar";
    [Parameter] public string SlashMenuAriaLabel { get; set; } = "Insert block";
    [Parameter] public string? AiCopilotKey { get; set; }

    protected bool EffectiveRightToLeft =>
        RightToLeft ?? CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

    private bool IsReadOnly => ReadOnly || Mode == SbEditorMode.ReadOnly;

    private bool ShowBubbleMenu =>
        Features.HasFlag(SbEditorFeatures.BubbleMenu) && !IsReadOnly;

    private bool ShowSlashMenu =>
        (Features.HasFlag(SbEditorFeatures.SlashCommands) || Features.HasFlag(SbEditorFeatures.FloatingMenu))
        && !IsReadOnly;

    private SbContentFormat EffectiveFormat =>
        Mode == SbEditorMode.Markdown ? SbContentFormat.Markdown : ContentFormat;

    protected override async Task OnParametersSetAsync()
    {
        if (ToolbarItems != null)
        {
            _toolbarItems = ToolbarItems.Select(MapLegacyItem).ToList();
        }

        if (_editorId != null && _interop != null && Value != _lastValue)
        {
            await _interop.SetContentAsync(_editorId, Value ?? "", EffectiveFormat);
            _lastValue = Value;
        }

        await ApplyDirectionAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_disposed)
        {
            return;
        }

        if (!firstRender)
        {
            return;
        }

        await LoadContributedToolbarAsync();

        _interop = new SbRichTextEditorInterop(JSRuntime);
        _dotNetRef = DotNetObjectReference.Create(this);
        try
        {
            _editorId = await _interop.InitializeAsync(
                _editorContainer,
                _dotNetRef,
                new SbRichTextEditorInitOptions
                {
                    Placeholder = Placeholder,
                    ReadOnly = IsReadOnly,
                    Disabled = Disabled,
                    Direction = EffectiveRightToLeft ? "rtl" : "ltr",
                    Content = Value,
                    ContentFormat = EffectiveFormat.ToString().ToLowerInvariant(),
                    PasteCleanup = PasteCleanupOptions,
                    Features = (int)Features
                },
                ShowBubbleMenu ? _bubbleMenu : null);
            _lastValue = Value;
            _appliedDirection = EffectiveRightToLeft ? "rtl" : "ltr";
        }
        catch (JSException)
        {
            _useFallback = true;
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task ApplyDirectionAsync()
    {
        if (_editorId == null || _interop == null || _useFallback)
        {
            return;
        }

        var direction = EffectiveRightToLeft ? "rtl" : "ltr";
        if (string.Equals(_appliedDirection, direction, StringComparison.Ordinal))
        {
            return;
        }

        await _interop.SetDirectionAsync(_editorId, direction);
        _appliedDirection = direction;
    }

    private async Task LoadContributedToolbarAsync()
    {
        var service = ServiceProvider.GetService<IEditorToolbarService>();
        if (service == null)
        {
            _bubbleItems = EditorToolbarService.CreateDefaultItems()
                .Where(item => item.Surfaces.HasFlag(SbEditorSurface.BubbleMenu))
                .ToList();
            _slashItems = EditorToolbarService.CreateDefaultItems()
                .Where(item => item.Surfaces.HasFlag(SbEditorSurface.SlashMenu))
                .ToList();
            return;
        }

        if (ToolbarItems == null && UseToolbarContributors)
        {
            _toolbarItems = await service.GetToolbarItemsAsync(
                _editorId,
                IncludeDefaultToolbarItems,
                ToolbarScope,
                SbEditorSurface.Toolbar,
                EffectiveFormat,
                this);
        }

        _slashItems = (await service.GetToolbarItemsAsync(
            _editorId,
            true,
            ToolbarScope,
            SbEditorSurface.SlashMenu,
            EffectiveFormat,
            this)).Where(item => !item.IsSeparator).ToList();

        _bubbleItems = (await service.GetToolbarItemsAsync(
            _editorId,
            true,
            ToolbarScope,
            SbEditorSurface.BubbleMenu,
            EffectiveFormat,
            this)).Where(item => !item.IsSeparator).ToList();
    }

    private static EditorToolbarItem MapLegacyItem(SbEditorToolbarItem item)
    {
        return new EditorToolbarItem
        {
            Id = item.Id,
            Icon = item.Icon,
            IconName = item.IconName,
            Tooltip = item.Tooltip,
            IsSeparator = item.IsSeparator,
            Type = item.Type,
            Options = item.Options,
            Command = item.CustomAction switch
            {
                "undo" => SbEditorCommand.Undo,
                "redo" => SbEditorCommand.Redo,
                "clean" => SbEditorCommand.ClearFormatting,
                _ => item.Format switch
                {
                    "bold" => SbEditorCommand.Bold,
                    "italic" => SbEditorCommand.Italic,
                    "underline" => SbEditorCommand.Underline,
                    "strike" => SbEditorCommand.Strike,
                    "link" => SbEditorCommand.InsertLink,
                    "image" => SbEditorCommand.InsertImage,
                    "blockquote" => SbEditorCommand.Blockquote,
                    "code-block" => SbEditorCommand.CodeBlock,
                    "list" when item.FormatValue is "ordered" => SbEditorCommand.OrderedList,
                    "list" => SbEditorCommand.BulletList,
                    _ => null
                }
            }
        };
    }

    private string GetContainerClass()
    {
        var classes = new List<string> { "sb-editor", "sb-editor--tiptap" };
        if (Disabled)
        {
            classes.Add("sb-editor--disabled");
        }

        if (IsReadOnly)
        {
            classes.Add("sb-editor--readonly");
        }

        if (EffectiveRightToLeft)
        {
            classes.Add("sb-editor--rtl");
        }

        if (Layout == SbEditorLayout.Compact)
        {
            classes.Add("sb-editor--compact");
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(' ', classes);
    }

    private string GetContainerStyle()
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(Height))
        {
            parts.Add($"--sb-editor-height: {Height}");
        }

        if (!string.IsNullOrWhiteSpace(MinHeight))
        {
            parts.Add($"min-height: {MinHeight}");
        }

        if (!string.IsNullOrWhiteSpace(MaxHeight))
        {
            parts.Add($"max-height: {MaxHeight}");
        }

        if (!string.IsNullOrWhiteSpace(Style))
        {
            parts.Add(Style);
        }

        return string.Join("; ", parts);
    }

    private async Task OnToolbarItemClickAsync(EditorToolbarItem item)
    {
        if (item.OnClickAsync != null)
        {
            await item.OnClickAsync(CreateActionContext());
            return;
        }

        if (item.Command == SbEditorCommand.InsertLink)
        {
            _linkDialogOpen = true;
            return;
        }

        if (item.Command == SbEditorCommand.InsertImage)
        {
            _imageDialogOpen = true;
            return;
        }

        if (item.Id is "ai" or "ask-ai")
        {
            _aiMenuOpen = true;
            return;
        }

        if (item.Command.HasValue)
        {
            await ExecuteCommandAsync(item.Command.Value);
        }
    }

    private async Task OnToolbarSelectChangedAsync((EditorToolbarItem Item, string? Value) args)
    {
        if (args.Item.Id is "heading" or "header")
        {
            if (int.TryParse(args.Value, out var level) && level > 0)
            {
                await ApplyBlockAsync("heading", new Dictionary<string, object?> { ["level"] = level });
            }
            else
            {
                await ExecuteCommandAsync(SbEditorCommand.Paragraph);
            }

            return;
        }

        if (args.Item.Id.Contains("font", StringComparison.OrdinalIgnoreCase))
        {
            await ApplyMarkAsync("textStyle", new Dictionary<string, object?> { ["fontFamily"] = args.Value });
        }
    }

    private EditorActionContext CreateActionContext() =>
        new(_editorId ?? "", ServiceProvider, this)
        {
            Surface = SbEditorSurface.Toolbar,
            ContentFormat = EffectiveFormat,
            State = _state
        };

    private async Task OnFallbackValueChangedAsync(string? value)
    {
        _lastValue = value;
        await ValueChanged.InvokeAsync(value);
    }

    private Task CloseAiMenu()
    {
        _aiMenuOpen = false;
        return Task.CompletedTask;
    }

    private Task OnAiMenuOpenChanged(bool open)
    {
        _aiMenuOpen = open;
        return Task.CompletedTask;
    }

    private Task OnAiPromptChanged(string prompt)
    {
        _aiPrompt = prompt;
        return Task.CompletedTask;
    }

    private async Task OnAiSubmitAsync()
    {
        var assistant = ServiceProvider.GetService<ISbEditorAiAssistant>();
        if (assistant == null)
        {
            _aiStatus = "AI is not configured.";
            return;
        }

        _aiStatus = "Working...";
        var selection = await GetSelectionAsync();
        var document = await GetDocumentAsync(EffectiveFormat);
        try
        {
            var result = await assistant.AssistAsync(new SbEditorAiRequest
            {
                Prompt = _aiPrompt,
                Document = document,
                Selection = selection.Text,
                ContentFormat = EffectiveFormat,
                Culture = CultureInfo.CurrentUICulture.Name,
                CopilotKey = AiCopilotKey
            });

            if (result.Suggestions.Count > 0)
            {
                await ShowSuggestionsAsync(result.Suggestions);
            }
            else if (!string.IsNullOrWhiteSpace(result.ReplacementText))
            {
                if (result.ReplaceDocument)
                {
                    await ReplaceDocumentAsync(result.ReplacementText, EffectiveFormat);
                }
                else
                {
                    await ReplaceSelectionAsync(result.ReplacementText, EffectiveFormat);
                }
            }

            _aiStatus = result.Message;
            _aiMenuOpen = false;
            _aiPrompt = "";
        }
        catch (Exception)
        {
            _aiStatus = "AI request failed.";
        }
    }

    private Task ToggleSlashMenu()
    {
        _slashMenuOpen = !_slashMenuOpen;
        return Task.CompletedTask;
    }

    private async Task OnSlashItemClickAsync(EditorToolbarItem item)
    {
        _slashMenuOpen = false;
        await OnToolbarItemClickAsync(item);
    }

    private Task OnLinkDialogOpenChanged(bool open)
    {
        _linkDialogOpen = open;
        return Task.CompletedTask;
    }

    private Task OnImageDialogOpenChanged(bool open)
    {
        _imageDialogOpen = open;
        return Task.CompletedTask;
    }

    private Task CloseLinkDialogAsync()
    {
        _linkDialogOpen = false;
        return Task.CompletedTask;
    }

    private Task CloseImageDialogAsync()
    {
        _imageDialogOpen = false;
        return Task.CompletedTask;
    }

    private async Task ApplyLinkAsync()
    {
        await InsertLinkAsync(_linkUrl, _linkText);
        _linkDialogOpen = false;
        _linkUrl = "";
        _linkText = "";
    }

    private async Task ApplyImageAsync()
    {
        await InsertImageAsync(_imageUrl, _imageAlt);
        _imageDialogOpen = false;
        _imageUrl = "";
        _imageAlt = "";
    }

    [JSInvokable]
    public async Task OnEditorContentChanged(string value, string html, string text)
    {
        if (value == _lastValue)
        {
            return;
        }

        _lastValue = value;
        await ValueChanged.InvokeAsync(value);
        await OnChange.InvokeAsync();
    }

    [JSInvokable]
    public Task OnEditorStateChanged(string json)
    {
        try
        {
            _state = JsonSerializer.Deserialize<EditorState>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? _state;
        }
        catch (JsonException)
        {
        }

        return InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public Task OnEditorShortcut(string name) => OnShortcut.InvokeAsync(name);

    public async Task<string> GetDocumentAsync(SbContentFormat format)
    {
        if (_editorId == null || _interop == null)
        {
            return Value ?? "";
        }

        return await _interop.GetContentAsync(_editorId, format);
    }

    public async Task SetDocumentAsync(string text, SbContentFormat format)
    {
        Value = text;
        _lastValue = text;
        if (_editorId != null && _interop != null)
        {
            await _interop.SetContentAsync(_editorId, text, format);
        }

        await ValueChanged.InvokeAsync(text);
    }

    public async Task<SbEditorSelection> GetSelectionAsync()
    {
        if (_editorId == null || _interop == null)
        {
            return new SbEditorSelection { Text = _state.SelectionText ?? "" };
        }

        return await _interop.GetSelectionAsync(_editorId);
    }

    public Task ReplaceSelectionAsync(string text, SbContentFormat format) =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.ReplaceSelectionAsync(_editorId, text, format);

    public Task ReplaceDocumentAsync(string text, SbContentFormat format) => SetDocumentAsync(text, format);

    public Task InsertContentAsync(string text, SbContentFormat format) =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.InsertContentAsync(_editorId, text, format);

    public Task InsertLinkAsync(string url, string? text = null, string? target = null, string? rel = null) =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.InsertLinkAsync(_editorId, url, text, target, rel);

    public Task InsertImageAsync(string url, string? alt = null, string? width = null, string? height = null) =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.InsertImageAsync(_editorId, url, alt, width, height);

    public Task InsertFileAsync(string url, string name, string? mime = null) =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.InsertFileAsync(_editorId, url, name);

    public Task InsertMentionAsync(string id, string label) => InsertContentAsync($"@{label}", EffectiveFormat);

    public Task InsertNodeAsync(string nodeName, IReadOnlyDictionary<string, object?>? attrs = null) =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.ApplyBlockAsync(_editorId, nodeName, attrs);

    public Task ApplyMarkAsync(string mark, IReadOnlyDictionary<string, object?>? attrs = null) =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.ApplyMarkAsync(_editorId, mark, attrs);

    public Task ApplyBlockAsync(string block, IReadOnlyDictionary<string, object?>? attrs = null) =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.ApplyBlockAsync(_editorId, block, attrs);

    public Task ExecuteCommandAsync(SbEditorCommand command) =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.ExecCommandAsync(_editorId, command);

    public async Task ShowSuggestionsAsync(IReadOnlyList<SbEditorSuggestion> suggestions)
    {
        _suggestions.Clear();
        _suggestions.AddRange(suggestions);
        if (_editorId != null && _interop != null)
        {
            await _interop.ShowSuggestionsAsync(_editorId, suggestions);
            await RefreshSuggestionBarAsync();
        }

        await InvokeAsync(StateHasChanged);
    }

    public async Task AcceptSuggestionAsync(string id)
    {
        if (_editorId != null && _interop != null)
        {
            await _interop.AcceptSuggestionAsync(_editorId, id);
        }

        _suggestions.RemoveAll(s => s.Id == id);
        await RefreshSuggestionBarAsync();
        await InvokeAsync(StateHasChanged);
    }

    public async Task RejectSuggestionAsync(string id)
    {
        if (_editorId != null && _interop != null)
        {
            await _interop.RejectSuggestionAsync(_editorId, id);
        }

        _suggestions.RemoveAll(s => s.Id == id);
        await RefreshSuggestionBarAsync();
        await InvokeAsync(StateHasChanged);
    }

    public async Task AcceptAllSuggestionsAsync()
    {
        foreach (var suggestion in _suggestions.ToList())
        {
            await AcceptSuggestionAsync(suggestion.Id);
        }
    }

    public async Task RejectAllSuggestionsAsync()
    {
        _suggestions.Clear();
        _suggestionBarStyle = null;
        if (_editorId != null && _interop != null)
        {
            await _interop.ClearSuggestionsAsync(_editorId);
        }

        await InvokeAsync(StateHasChanged);
    }

    private Task AcceptCurrentSuggestionAsync()
    {
        var current = _suggestions.FirstOrDefault();
        return current == null ? Task.CompletedTask : AcceptSuggestionAsync(current.Id);
    }

    private Task RejectCurrentSuggestionAsync()
    {
        var current = _suggestions.FirstOrDefault();
        return current == null ? Task.CompletedTask : RejectSuggestionAsync(current.Id);
    }

    private async Task RefreshSuggestionBarAsync()
    {
        _suggestionBarStyle = null;
        if (_suggestions.Count == 0 || _editorId == null || _interop == null)
        {
            return;
        }

        var rect = await _interop.GetSuggestionRectAsync(_editorId, _suggestions[0].Id);
        if (rect == null)
        {
            _suggestionBarStyle = "inset-block-end: 0.75rem; inset-inline-end: 0.75rem;";
            return;
        }

        var top = Math.Max(8, rect.Top + rect.Height + 8);
        var left = Math.Max(8, rect.Left);
        _suggestionBarStyle = $"top: {top.ToString(CultureInfo.InvariantCulture)}px; inset-inline-start: {left.ToString(CultureInfo.InvariantCulture)}px;";
    }

    public Task StreamInsertAsync(string chunk) =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.StreamInsertAsync(_editorId, chunk);

    public Task FocusAsync() =>
        _editorId == null || _interop == null ? Task.CompletedTask : _interop.FocusAsync(_editorId);

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_editorId != null && _interop != null)
        {
            try
            {
                await _interop.DestroyAsync(_editorId);
            }
            catch (JSDisconnectedException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        if (_interop != null)
        {
            try
            {
                await _interop.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        _dotNetRef?.Dispose();
    }
}
