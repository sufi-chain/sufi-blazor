using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Components;

namespace SufiChain.SufiBlazor.Components.Navigation;

/// <summary>
/// Settings page shell: a wide icon-and-label rail, a compact strip or picker, and a per-section save bar.
/// </summary>
public partial class SbSettingsLayout : ComponentBase, IAsyncDisposable
{
    private readonly string _instanceId = Guid.NewGuid().ToString("N");
    private readonly string _pickerId = $"sb-settings-picker-{Guid.NewGuid():N}";
    private readonly List<SbSettingsSection> _sections = new();
    private ElementReference _paneRef;
    private ElementReference _headingRef;
    private IDisposable? _locationRegistration;
    private bool _locationRegistered;
    private bool _jsReady;
    private bool _isRtl;
    private bool _focusHeading;
    private bool _saveFailed;
    private bool _toastVisible;
    private bool _guardOpen;
    private bool _suppressLeaveGuard;
    private bool _beforeUnloadArmed;
    private int _toastVersion;
    private readonly CancellationTokenSource _disposeCts = new();
    private string? _activeId;
    private string? _notifiedActiveId;
    private string? _pendingSectionId;
    private string? _pendingLocation;
    private string? _focusedId;
    private bool _focusStrip;
    private bool _urlWriteQueued;
    private bool _sectionsStable;
    private bool _userChoseSection;

    /// <summary>
    /// Section children. They register in document order.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Active section id. The first selectable section is used when this is empty.
    /// </summary>
    [Parameter]
    public string? ActiveSection { get; set; }

    /// <summary>
    /// Raised when the active section changes.
    /// </summary>
    [Parameter]
    public EventCallback<string> ActiveSectionChanged { get; set; }

    /// <summary>
    /// Query-string key for the deep link.
    /// </summary>
    [Parameter]
    public string QueryParameter { get; set; } = "section";

    /// <summary>
    /// Read and write the active section in the query string, replacing the history entry.
    /// </summary>
    [Parameter]
    public bool DeepLink { get; set; } = true;

    /// <summary>
    /// Accessible name of the section navigation.
    /// </summary>
    [Parameter, EditorRequired]
    public string AriaLabel { get; set; } = "";

    /// <summary>
    /// Compact presentation. Wide screens always use the rail.
    /// </summary>
    [Parameter]
    public SbSettingsCompactMode CompactMode { get; set; } = SbSettingsCompactMode.Auto;

    /// <summary>
    /// CSS <c>inset-block-start</c> for the sticky save bar and compact nav.
    /// </summary>
    [Parameter]
    public string StickyOffset { get; set; } = "var(--sb-app-header-height, 0px)";

    /// <summary>
    /// Ask before leaving a section or page that has unsaved changes.
    /// </summary>
    [Parameter]
    public bool ConfirmOnLeave { get; set; } = true;

    /// <summary>
    /// Disables the rail and the save bar while loading or saving.
    /// </summary>
    [Parameter]
    public bool Busy { get; set; }

    /// <summary>
    /// Shown when every section is hidden. The default is an empty state.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyContent { get; set; }

    /// <summary>
    /// Extra CSS classes on the root.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    internal string HeadingId { get; } = $"sb-settings-heading-{Guid.NewGuid():N}";

    private string? ActiveId => _activeId;

    private SbSettingsSection? Active => _sections.FirstOrDefault(section => section.Id == _activeId && section.Visible && !section.Disabled);

    private bool HasVisibleSection => _sections.Any(section => section.Visible);

    private bool ShowNavigation => VisibleSections.Count > 1;

    private bool UseStrip => CompactMode == SbSettingsCompactMode.Strip || (CompactMode == SbSettingsCompactMode.Auto && VisibleSections.Count <= 6);

    private bool CanPressSave => Active is { } active && active.ShowsSaveControls && active.EffectiveDirty && active.CanSave && !Busy;

    private List<SbSettingsSection> VisibleSections => _sections.Where(section => section.Visible).ToList();

    private IEnumerable<(SbSettingsSection Section, int Index)> RailEntries =>
        VisibleSections.Select((section, index) => (section, index));

    private string CssClass
    {
        get
        {
            var classes = new List<string> { "sb-settings-layout" };
            if (!ShowNavigation)
            {
                classes.Add("sb-settings-layout--single");
            }
            else if (UseStrip)
            {
                classes.Add("sb-settings-layout--strip");
            }
            else
            {
                classes.Add("sb-settings-layout--picker");
            }

            if (!string.IsNullOrWhiteSpace(Class))
            {
                classes.Add(Class);
            }

            return string.Join(' ', classes);
        }
    }

    private string RootStyle => $"--sb-settings-sticky-offset: {StickyOffset}";

    internal void Register(SbSettingsSection section)
    {
        if (_sections.Contains(section))
        {
            return;
        }

        _sections.Add(section);
        ReconcileActive();
        _ = InvokeAsync(StateHasChanged);
    }

    internal void Unregister(SbSettingsSection section)
    {
        if (!_sections.Remove(section))
        {
            return;
        }

        ReconcileActive();
        _ = InvokeAsync(StateHasChanged);
    }

    internal void NotifySectionChanged()
    {
        ReconcileActive();
        _ = InvokeAsync(StateHasChanged);
    }

    internal bool IsSectionActive(SbSettingsSection section) => section.Visible && !section.Disabled && section.Id == _activeId;

    internal bool ShouldRenderBody(SbSettingsSection section)
    {
        if (!section.Visible || section.Disabled)
        {
            return false;
        }

        if (!section.Lazy)
        {
            return true;
        }

        return section.HasBeenActivated || section.Id == _activeId;
    }

    protected override void OnInitialized()
    {
        ReconcileActive();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _sectionsStable = true;
            ReconcileActive();
            await StartJsAsync();
            RegisterLocationHandler();
        }

        if (_urlWriteQueued)
        {
            _urlWriteQueued = false;
            WriteSectionUrl(_activeId);
        }

        if (_activeId != null && !string.Equals(_notifiedActiveId, _activeId, StringComparison.Ordinal))
        {
            _notifiedActiveId = _activeId;
            if (ActiveSectionChanged.HasDelegate)
            {
                await ActiveSectionChanged.InvokeAsync(_activeId);
            }
        }

        await SyncBeforeUnloadAsync();

        if (_focusHeading)
        {
            _focusHeading = false;
            try
            {
                await _headingRef.FocusAsync();
            }
            catch (Exception ex) when (ex is JSException or InvalidOperationException)
            {
            }
        }

        if (_focusedId != null)
        {
            var target = _focusedId;
            var strip = _focusStrip;
            _focusedId = null;
            try
            {
                var selector = strip
                    ? $".sb-settings-strip [data-section-id='{CssEscape(target)}']"
                    : $".sb-settings-rail [data-section-id='{CssEscape(target)}']";
                await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.focusSelector", selector);
                await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.scrollIntoView", selector);
            }
            catch (Exception ex) when (ex is JSException or InvalidOperationException)
            {
            }
        }
    }

    internal async Task RequestSectionAsync(string? id)
    {
        if (Busy || string.IsNullOrEmpty(id) || id == _activeId)
        {
            if (id == _activeId)
            {
                _focusHeading = true;
            }

            return;
        }

        var target = SelectableSections().FirstOrDefault(section => section.Id == id);
        if (target == null)
        {
            return;
        }

        if (NeedsLeaveGuard())
        {
            _pendingSectionId = id;
            _pendingLocation = null;
            _guardOpen = true;
            return;
        }

        await CommitSectionAsync(target);
    }

    private async Task OnPickerChanged(string? id) => await RequestSectionAsync(id);

    private async Task OnItemKeyDown(KeyboardEventArgs args, int index, bool horizontal)
    {
        var sections = VisibleSections;
        if (sections.Count == 0 || Busy)
        {
            return;
        }

        var delta = KeyDelta(args.Key, horizontal);
        if (delta != 0)
        {
            var next = index;
            for (var step = 0; step < sections.Count; step++)
            {
                next = (next + delta + sections.Count) % sections.Count;
                if (!sections[next].Disabled)
                {
                    break;
                }
            }

            _focusedId = sections[next].Id;
            _focusStrip = horizontal;
            return;
        }

        if (args.Key is "Enter" or " ")
        {
            await RequestSectionAsync(sections[index].Id);
        }
    }

    private int KeyDelta(string key, bool horizontal)
    {
        if (!horizontal)
        {
            return key switch
            {
                "ArrowDown" => 1,
                "ArrowUp" => -1,
                _ => 0
            };
        }

        var forward = _isRtl ? "ArrowLeft" : "ArrowRight";
        var backward = _isRtl ? "ArrowRight" : "ArrowLeft";
        if (key == forward)
        {
            return 1;
        }

        return key == backward ? -1 : 0;
    }

    private async Task SaveFromBarAsync()
    {
        if (!CanPressSave)
        {
            return;
        }

        await SaveActiveAsync();
    }

    private async Task DiscardFromBarAsync()
    {
        var active = Active;
        if (active == null || Busy)
        {
            return;
        }

        if (active.OnDiscard != null)
        {
            await active.OnDiscard();
        }

        await active.MarkCleanAsync();
        _saveFailed = false;
    }

    private Task StayAsync()
    {
        _guardOpen = false;
        _pendingSectionId = null;
        _pendingLocation = null;
        return Task.CompletedTask;
    }

    private async Task DiscardAndLeaveAsync()
    {
        await DiscardFromBarAsync();
        _guardOpen = false;
        await CompletePendingAsync();
    }

    private async Task SaveAndLeaveAsync()
    {
        var saved = await SaveActiveAsync();
        if (!saved)
        {
            _guardOpen = false;
            _pendingSectionId = null;
            _pendingLocation = null;
            return;
        }

        _guardOpen = false;
        await CompletePendingAsync();
    }

    private void DismissToast() => _toastVisible = false;

    private RenderFragment RailButton(SbSettingsSection section, int index, bool horizontal) => builder =>
    {
        var active = IsSectionActive(section);
        var focused = section.Id == _focusedId;
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "type", "button");
        builder.AddAttribute(2, "class", ItemClass(section, active, horizontal));
        builder.AddAttribute(3, "data-section-id", section.Id);
        builder.AddAttribute(4, "data-keyboard-focus", focused ? "true" : "false");
        if (active)
        {
            builder.AddAttribute(5, "aria-current", "page");
        }

        if (section.Disabled)
        {
            builder.AddAttribute(6, "aria-disabled", "true");
        }

        if (Busy || section.Disabled)
        {
            builder.AddAttribute(7, "disabled", true);
        }

        builder.AddAttribute(8, "aria-controls", section.PanelDomId);
        builder.AddAttribute(9, "onclick", EventCallback.Factory.Create(this, () => RequestSectionAsync(section.Id)));
        builder.AddAttribute(10, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, args => OnItemKeyDown(args, index, horizontal)));
        builder.AddContent(12, ItemContent(section));
        builder.CloseElement();
    };

    private RenderFragment ItemContent(SbSettingsSection section) => builder =>
    {
        builder.OpenComponent<SufiChain.SufiBlazor.Components.Common.SbIcon>(0);
        builder.AddAttribute(1, "Name", section.Icon);
        builder.AddAttribute(2, "Size", SbSize.Md);
        builder.AddAttribute(3, "Mirror", MirrorsInRtl(section.Icon));
        builder.AddAttribute(4, "Class", MirrorClass(section.Icon));
        builder.CloseComponent();

        builder.OpenElement(5, "span");
        builder.AddAttribute(6, "class", "sb-settings-item__label");
        builder.AddContent(7, section.Label);
        builder.CloseElement();

        if (section.Badge != null)
        {
            builder.OpenElement(8, "span");
            builder.AddAttribute(9, "class", "sb-settings-item__badge");
            builder.AddContent(10, section.Badge);
            builder.CloseElement();
        }

        if (section.EffectiveDirty && section.ShowsSaveControls)
        {
            builder.OpenElement(11, "span");
            builder.AddAttribute(12, "class", "sb-settings-item__dirty");
            builder.AddAttribute(13, "aria-hidden", "true");
            builder.CloseElement();
            builder.OpenElement(14, "span");
            builder.AddAttribute(15, "class", "sb-sr-only");
            builder.AddContent(16, L["Settings:UnsavedChanges"]);
            builder.CloseElement();
        }

        if (section.Disabled && !string.IsNullOrWhiteSpace(section.DisabledReason))
        {
            builder.OpenElement(17, "span");
            builder.AddAttribute(18, "class", "sb-settings-item__tooltip");
            builder.AddAttribute(19, "role", "tooltip");
            builder.AddContent(20, section.DisabledReason);
            builder.CloseElement();
        }
    };

    private static string ItemClass(SbSettingsSection section, bool active, bool horizontal)
    {
        var classes = new List<string> { horizontal ? "sb-settings-strip__item" : "sb-settings-rail__item" };
        if (active)
        {
            classes.Add(horizontal ? "sb-settings-strip__item--active" : "sb-settings-rail__item--active");
        }

        if (section.Disabled)
        {
            classes.Add("sb-settings-item--disabled");
        }

        return string.Join(' ', classes);
    }

    private string PickerText(SbSettingsSection section)
    {
        if (section.Disabled && !string.IsNullOrWhiteSpace(section.DisabledReason))
        {
            return $"{section.Label} — {section.DisabledReason}";
        }

        return section.Label;
    }

    private static bool MirrorsInRtl(string icon) =>
        icon.StartsWith("arrow-", StringComparison.Ordinal)
        || icon.StartsWith("chevron-", StringComparison.Ordinal)
        || string.Equals(icon, "external-link", StringComparison.Ordinal);

    private static string? MirrorClass(string icon) => MirrorsInRtl(icon) ? "sb-icon--mirror-rtl" : null;

    private void ReconcileActive()
    {
        var selectable = SelectableSections();
        if (selectable.Count == 0)
        {
            _activeId = null;
            if (_sectionsStable && DeepLink && ReadSectionQuery() != null)
            {
                _urlWriteQueued = true;
            }

            return;
        }

        if (_userChoseSection && _activeId != null && selectable.Any(section => section.Id == _activeId))
        {
            selectable.First(section => section.Id == _activeId).MarkActivated();
            return;
        }

        if (_userChoseSection)
        {
            _userChoseSection = false;
        }

        var requested = ReadSectionQuery() ?? ActiveSection;
        if (!string.IsNullOrEmpty(requested))
        {
            var match = selectable.FirstOrDefault(section => section.Id == requested);
            if (match != null)
            {
                _activeId = match.Id;
                match.MarkActivated();
                if (_sectionsStable && DeepLink && !string.Equals(ReadSectionQuery(), _activeId, StringComparison.Ordinal))
                {
                    _urlWriteQueued = true;
                }

                return;
            }

            if (!_sectionsStable)
            {
                if (_activeId == null || !selectable.Any(section => section.Id == _activeId))
                {
                    _activeId = selectable[0].Id;
                    selectable[0].MarkActivated();
                }

                return;
            }
        }

        if (_activeId == null || !selectable.Any(section => section.Id == _activeId))
        {
            _activeId = selectable[0].Id;
        }

        selectable.First(section => section.Id == _activeId).MarkActivated();
        if (_sectionsStable && DeepLink && !string.Equals(ReadSectionQuery(), _activeId, StringComparison.Ordinal))
        {
            _urlWriteQueued = true;
        }
    }

    private List<SbSettingsSection> SelectableSections() =>
        _sections.Where(section => section.Visible && !section.Disabled).ToList();

    private bool NeedsLeaveGuard()
    {
        var active = Active;
        return ConfirmOnLeave && active is { ShowsSaveControls: true, EffectiveDirty: true };
    }

    private async Task CommitSectionAsync(SbSettingsSection section)
    {
        _saveFailed = false;
        _userChoseSection = true;
        _activeId = section.Id;
        section.MarkActivated();
        _focusHeading = true;
        _urlWriteQueued = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task<bool> SaveActiveAsync()
    {
        var active = Active;
        if (active?.OnSave == null || !active.ShowsSaveControls)
        {
            return false;
        }

        bool saved;
        try
        {
            saved = await active.OnSave();
        }
        catch (Exception)
        {
            saved = false;
        }

        if (!saved)
        {
            _saveFailed = true;
            try
            {
                await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.focusFirstInvalid", _paneRef);
            }
            catch (Exception ex) when (ex is JSException or InvalidOperationException)
            {
            }

            return false;
        }

        _saveFailed = false;
        await active.MarkCleanAsync();
        _ = ShowToastAsync();
        return true;
    }

    private async Task ShowToastAsync()
    {
        var version = ++_toastVersion;
        _toastVisible = true;
        try
        {
            await InvokeAsync(StateHasChanged);
            await Task.Delay(4000, _disposeCts.Token);
            if (version == _toastVersion)
            {
                _toastVisible = false;
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException or OperationCanceledException)
        {
        }
    }

    private async Task CompletePendingAsync()
    {
        var sectionId = _pendingSectionId;
        var location = _pendingLocation;
        _pendingSectionId = null;
        _pendingLocation = null;

        if (!string.IsNullOrEmpty(sectionId))
        {
            var target = SelectableSections().FirstOrDefault(section => section.Id == sectionId);
            if (target != null)
            {
                await CommitSectionAsync(target);
            }
        }

        if (!string.IsNullOrEmpty(location))
        {
            _suppressLeaveGuard = true;
            try
            {
                Navigation.NavigateTo(location);
            }
            finally
            {
                _suppressLeaveGuard = false;
            }
        }
    }

    private void RegisterLocationHandler()
    {
        if (_locationRegistered)
        {
            return;
        }

        _locationRegistration = Navigation.RegisterLocationChangingHandler(OnLocationChanging);
        _locationRegistered = true;
    }

    private ValueTask OnLocationChanging(LocationChangingContext context)
    {
        if (_suppressLeaveGuard || !NeedsLeaveGuard())
        {
            return ValueTask.CompletedTask;
        }

        if (IsSamePage(context.TargetLocation))
        {
            return ValueTask.CompletedTask;
        }

        context.PreventNavigation();
        _pendingLocation = context.TargetLocation;
        _pendingSectionId = null;
        _guardOpen = true;
        _ = InvokeAsync(StateHasChanged);
        return ValueTask.CompletedTask;
    }

    private bool IsSamePage(string targetLocation)
    {
        var current = Navigation.ToAbsoluteUri(Navigation.Uri);
        var target = Navigation.ToAbsoluteUri(targetLocation);
        return string.Equals(current.AbsolutePath, target.AbsolutePath, StringComparison.OrdinalIgnoreCase);
    }

    private string? ReadSectionQuery()
    {
        if (!DeepLink || string.IsNullOrEmpty(QueryParameter))
        {
            return null;
        }

        var query = Navigation.ToAbsoluteUri(Navigation.Uri).Query;
        if (string.IsNullOrEmpty(query))
        {
            return null;
        }

        foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = pair.Split('=', 2);
            var key = Uri.UnescapeDataString(parts[0].Replace("+", " ", StringComparison.Ordinal));
            if (!string.Equals(key, QueryParameter, StringComparison.Ordinal))
            {
                continue;
            }

            return parts.Length == 2
                ? Uri.UnescapeDataString(parts[1].Replace("+", " ", StringComparison.Ordinal))
                : "";
        }

        return null;
    }

    private void WriteSectionUrl(string? sectionId)
    {
        if (!DeepLink || string.IsNullOrEmpty(sectionId))
        {
            return;
        }

        if (string.Equals(ReadSectionQuery(), sectionId, StringComparison.Ordinal))
        {
            return;
        }

        _suppressLeaveGuard = true;
        try
        {
            Navigation.NavigateTo(Navigation.GetUriWithQueryParameter(QueryParameter, sectionId), replace: true);
        }
        catch (NavigationException)
        {
        }
        finally
        {
            _suppressLeaveGuard = false;
        }
    }

    private async Task StartJsAsync()
    {
        try
        {
            var reference = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.watchCompact", _instanceId, reference);
            await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.watchDirection", _instanceId, reference);
            _jsReady = true;
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException)
        {
        }
    }

    private async Task SyncBeforeUnloadAsync()
    {
        var arm = NeedsLeaveGuard();
        if (arm == _beforeUnloadArmed)
        {
            return;
        }

        _beforeUnloadArmed = arm;
        try
        {
            await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.setBeforeUnload", _instanceId, arm);
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException)
        {
        }
    }

    [JSInvokable]
    public void OnDirectionChanged(bool rtl)
    {
        if (_isRtl == rtl)
        {
            return;
        }

        _isRtl = rtl;
        _ = InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public void OnCompactChanged(bool compact)
    {
        _ = compact;
    }

    private static string CssEscape(string value) => value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("'", "\\'", StringComparison.Ordinal);

    public async ValueTask DisposeAsync()
    {
        _locationRegistration?.Dispose();
        _toastVersion++;
        _disposeCts.Cancel();
        _disposeCts.Dispose();
        if (!_jsReady)
        {
            return;
        }

        try
        {
            await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.unwatch", _instanceId);
            await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.setBeforeUnload", _instanceId, false);
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException)
        {
        }
    }
}
