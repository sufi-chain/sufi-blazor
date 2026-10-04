using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Overlays;

namespace SufiChain.SufiBlazor.Components.Navigation;

/// <summary>
/// Settings page shell: a wide icon-and-label rail, a compact strip or picker, and a per-section save bar.
/// </summary>
public partial class SbSettingsLayout : ComponentBase, IAsyncDisposable
{
    private readonly string _instanceId = Guid.NewGuid().ToString("N");
    private readonly string _pickerId = $"sb-settings-picker-{Guid.NewGuid():N}";
    private readonly List<SbSettingsSection> _sections = new();
    private ElementReference _rootRef;
    private ElementReference _paneRef;
    private ElementReference _stripRef;
    private ElementReference _headingRef;
    private ElementReference _errorRef;
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
    private bool _saving;
    private int _toastVersion;
    private readonly CancellationTokenSource _disposeCts = new();
    private string? _activeId;
    private string? _notifiedActiveId;
    private string? _pendingSectionId;
    private string? _pendingLocation;
    private string? _pendingHistoryState;
    private string? _focusedId;
    private string? _keyboardFocusId;
    private string? _reasonOpenId;
    private double _reasonTop;
    private double _reasonLeft;
    private string? _guardTriggerId;
    private bool _focusStrip;
    private bool _guardFromStrip;
    private bool _urlWriteQueued;
    private bool _rewriteCurrentSection;
    private bool _scrollStrip;
    private bool _sectionsStable;
    private bool _userChoseSection;
    private string? _trackedActiveSection;
    private bool _trackedActiveSectionSet;

    /// <summary>
    /// Section children. They register in document order.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Active section id. The first selectable section is used when this is empty.
    /// Later changes go through the same leave guard as a rail click.
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

    private SbSettingsSection? Active => _sections.FirstOrDefault(section => section.Id == _activeId && section.Visible && !section.Disabled && !section.Pending);

    private bool HasVisibleSection => _sections.Any(section => section.Visible && !section.Pending);

    private bool HasPending => _sections.Any(section => section.Pending);

    private bool ShowNavigation => _sections.Count(section => section.Visible) > 1;

    private bool UseStrip => CompactMode == SbSettingsCompactMode.Strip || (CompactMode == SbSettingsCompactMode.Auto && VisibleSections.Count <= 6);

    private bool CanPressSave => Active is { } active && active.ShowsSaveControls && active.EffectiveDirty && active.CanSave && !Busy && !_saving;

    private List<SbSettingsSection> VisibleSections => _sections.Where(section => section.Visible && !section.Pending).ToList();

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

    private string ReasonPopoverStyle =>
        $"top: {_reasonTop.ToString(CultureInfo.InvariantCulture)}px; left: {_reasonLeft.ToString(CultureInfo.InvariantCulture)}px";

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

    internal bool IsSectionActive(SbSettingsSection section) => section.Visible && !section.Disabled && !section.Pending && section.Id == _activeId;

    internal bool ShouldRenderBody(SbSettingsSection section)
    {
        if (!section.Visible || section.Disabled || section.Pending)
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

    protected override void OnParametersSet()
    {
        if (!_trackedActiveSectionSet)
        {
            _trackedActiveSection = ActiveSection;
            _trackedActiveSectionSet = true;
            return;
        }

        if (string.Equals(ActiveSection, _trackedActiveSection, StringComparison.Ordinal))
        {
            return;
        }

        _trackedActiveSection = ActiveSection;
        if (_sectionsStable && !string.IsNullOrEmpty(ActiveSection) && !string.Equals(ActiveSection, _activeId, StringComparison.Ordinal))
        {
            _ = RequestSectionAsync(ActiveSection, fromStrip: false);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var renderAgain = false;
        if (firstRender)
        {
            _sectionsStable = true;
            var previousActive = _activeId;
            ReconcileActive();
            renderAgain = !string.Equals(previousActive, _activeId, StringComparison.Ordinal);
            await StartJsAsync();
            RegisterLocationHandler();
        }

        if (_urlWriteQueued || _rewriteCurrentSection)
        {
            var rewriteCurrent = _rewriteCurrentSection;
            _urlWriteQueued = false;
            _rewriteCurrentSection = false;
            WriteSectionUrl(_activeId, rewriteCurrent);
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
        await UpdateStripFadesAsync();

        if (_scrollStrip && _activeId != null)
        {
            _scrollStrip = false;
            await ScrollStripItemAsync(_activeId);
        }

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
                await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.focusSelector", _rootRef, selector);
                await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.scrollIntoView", _rootRef, selector);
            }
            catch (Exception ex) when (ex is JSException or InvalidOperationException)
            {
            }
        }

        if (renderAgain)
        {
            StateHasChanged();
        }
    }

    internal async Task RequestSectionAsync(string? id, bool fromStrip)
    {
        if (_saving || string.IsNullOrEmpty(id))
        {
            return;
        }

        var known = _sections.FirstOrDefault(section => section.Id == id);
        if (known?.Disabled == true)
        {
            _reasonOpenId = _reasonOpenId == id ? null : id;
            return;
        }

        if (Busy)
        {
            return;
        }

        if (id == _activeId)
        {
            _reasonOpenId = null;
            _focusHeading = true;
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
            _pendingHistoryState = null;
            _guardTriggerId = id;
            _guardFromStrip = fromStrip;
            _guardOpen = true;
            _reasonOpenId = null;
            return;
        }

        await CommitSectionAsync(target);
    }

    private Task OnPickerChanged(string? id) => RequestSectionAsync(id, fromStrip: false);

    private async Task OnItemKeyDown(KeyboardEventArgs args, int index, bool horizontal)
    {
        if (args.Key == "Escape")
        {
            if (_reasonOpenId != null && !_guardOpen)
            {
                _reasonOpenId = null;
            }

            return;
        }

        var sections = VisibleSections;
        if (sections.Count == 0 || Busy || _saving)
        {
            return;
        }

        if (args.Key is "Home" or "End")
        {
            var next = args.Key == "Home" ? 0 : sections.Count - 1;
            var step = args.Key == "Home" ? 1 : -1;
            while (sections[next].Disabled && next >= 0 && next < sections.Count)
            {
                next += step;
                if (next < 0 || next >= sections.Count)
                {
                    return;
                }
            }

            _focusedId = sections[next].Id;
            _keyboardFocusId = _focusedId;
            _focusStrip = horizontal;
            return;
        }

        var delta = KeyDelta(args.Key, horizontal);
        if (delta == 0)
        {
            return;
        }

        var cursor = index;
        for (var step = 0; step < sections.Count; step++)
        {
            cursor = (cursor + delta + sections.Count) % sections.Count;
            if (!sections[cursor].Disabled)
            {
                break;
            }
        }

        _focusedId = sections[cursor].Id;
        _keyboardFocusId = _focusedId;
        _focusStrip = horizontal;
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
        if (active == null || Busy || _saving)
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

    private async Task StayAsync()
    {
        _guardOpen = false;
        _pendingSectionId = null;
        _pendingLocation = null;
        _pendingHistoryState = null;
        _trackedActiveSection = _activeId;
        if (ActiveSectionChanged.HasDelegate && !string.IsNullOrEmpty(_activeId))
        {
            await ActiveSectionChanged.InvokeAsync(_activeId);
        }

        if (!string.IsNullOrEmpty(_guardTriggerId))
        {
            _focusedId = _guardTriggerId;
            _keyboardFocusId = _guardTriggerId;
            _focusStrip = _guardFromStrip;
        }
    }

    private async Task DiscardAndLeaveAsync()
    {
        if (_saving)
        {
            return;
        }

        await DiscardFromBarAsync();
        _guardOpen = false;
        await CompletePendingAsync();
    }

    private async Task SaveAndLeaveAsync()
    {
        if (_saving || !CanPressSave)
        {
            return;
        }

        var saved = await SaveActiveAsync();
        if (!saved)
        {
            _guardOpen = false;
            _pendingSectionId = null;
            _pendingLocation = null;
            _pendingHistoryState = null;
            return;
        }

        _guardOpen = false;
        await CompletePendingAsync();
    }

    private Task OnGuardClose(SbDialogCloseReason reason)
    {
        if (reason == SbDialogCloseReason.Escape && _guardOpen)
        {
            return StayAsync();
        }

        return Task.CompletedTask;
    }

    private void DismissToast() => _toastVisible = false;

    private RenderFragment RailButton(SbSettingsSection section, int index, bool horizontal) => builder =>
    {
        var active = IsSectionActive(section);
        var focused = section.Id == _keyboardFocusId;
        var reasonId = ReasonId(section, horizontal);
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "type", "button");
        builder.AddAttribute(2, "class", ItemClass(section, active, horizontal));
        builder.AddAttribute(3, "data-section-id", section.Id);
        builder.AddAttribute(4, "data-keyboard-focus", focused ? "true" : "false");
        builder.AddAttribute(5, "title", section.Label);
        builder.AddAttribute(6, "aria-label", ItemAccessibleName(section));
        if (active)
        {
            builder.AddAttribute(7, "aria-current", "page");
        }

        if (section.Disabled)
        {
            builder.AddAttribute(8, "aria-disabled", "true");
            if (!string.IsNullOrWhiteSpace(section.DisabledReason))
            {
                builder.AddAttribute(9, "aria-describedby", reasonId);
            }
        }
        else if (Busy || _saving)
        {
            builder.AddAttribute(10, "disabled", true);
        }

        builder.AddAttribute(11, "aria-controls", section.PanelDomId);
        builder.AddAttribute(12, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, args => OnItemClick(section.Id, horizontal, args)));
        builder.AddEventStopPropagationAttribute(13, "onclick", true);
        builder.AddAttribute(14, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, args => OnItemKeyDown(args, index, horizontal)));
        builder.AddContent(15, ItemContent(section));
        builder.CloseElement();

        if (section.Disabled && !string.IsNullOrWhiteSpace(section.DisabledReason))
        {
            builder.OpenElement(20, "span");
            builder.AddAttribute(21, "id", reasonId);
            builder.AddAttribute(22, "class", "sb-sr-only");
            builder.AddContent(23, section.DisabledReason);
            builder.CloseElement();
        }
    };

    private RenderFragment ItemContent(SbSettingsSection section) => builder =>
    {
        builder.OpenComponent<SufiChain.SufiBlazor.Components.Common.SbIcon>(0);
        builder.AddAttribute(1, "Name", section.Icon);
        builder.AddAttribute(2, "Size", SbSize.Md);
        builder.AddAttribute(3, "Mirror", MirrorsInRtl(section.Icon));
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
            builder.AddAttribute(18, "class", "sb-settings-item__reason");
            builder.AddAttribute(19, "aria-hidden", "true");
            builder.AddContent(20, section.DisabledReason);
            builder.CloseElement();
        }
    };

    private string ItemAccessibleName(SbSettingsSection section)
    {
        if (section.EffectiveDirty && section.ShowsSaveControls)
        {
            return $"{section.Label}. {L["Settings:UnsavedChanges"]}";
        }

        return section.Label;
    }

    private Task OnItemClick(string id, bool fromStrip, MouseEventArgs args)
    {
        _reasonTop = args.ClientY;
        _reasonLeft = args.ClientX;
        return RequestSectionAsync(id, fromStrip);
    }

    private void CloseReason() => _reasonOpenId = null;

    private void OnLayoutKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Escape" && _reasonOpenId != null && !_guardOpen)
        {
            _reasonOpenId = null;
        }
    }

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

    private string? ReasonText(string? id)
    {
        var section = _sections.FirstOrDefault(item => item.Id == id);
        return string.IsNullOrWhiteSpace(section?.DisabledReason) ? null : section.DisabledReason;
    }

    private string ReasonId(SbSettingsSection section, bool horizontal) =>
        $"sb-settings-reason-{(horizontal ? "strip" : "rail")}-{_instanceId}-{section.Id}";

    private static bool MirrorsInRtl(string icon) =>
        icon.StartsWith("arrow-", StringComparison.Ordinal)
        || icon.StartsWith("chevron-", StringComparison.Ordinal)
        || string.Equals(icon, "external-link", StringComparison.Ordinal);

    private void ReconcileActive()
    {
        if (HasPending)
        {
            var ready = SelectableSections();
            if (_userChoseSection && _activeId != null && ready.Any(section => section.Id == _activeId))
            {
                ready.First(section => section.Id == _activeId).MarkActivated();
                return;
            }

            var requested = ReadSectionQuery() ?? ActiveSection;
            var pendingMatch = ready.FirstOrDefault(section => section.Id == requested);
            if (pendingMatch != null)
            {
                _activeId = pendingMatch.Id;
                pendingMatch.MarkActivated();
            }
            else if (_activeId != null && !ready.Any(section => section.Id == _activeId))
            {
                _activeId = null;
            }

            return;
        }

        var selectable = SelectableSections();
        if (selectable.Count == 0)
        {
            NoteUnavailable(_activeId);
            _activeId = null;
            return;
        }

        if (!_userChoseSection)
        {
            var requested = ReadSectionQuery() ?? ActiveSection;
            if (!string.IsNullOrEmpty(requested))
            {
                var match = selectable.FirstOrDefault(section => section.Id == requested);
                if (match != null)
                {
                    _activeId = match.Id;
                    match.MarkActivated();
                    QueueUrlIfNeeded();
                    _scrollStrip = true;
                    return;
                }

                if (!_sectionsStable)
                {
                    return;
                }
            }
        }

        if (_activeId != null && selectable.Any(section => section.Id == _activeId))
        {
            selectable.First(section => section.Id == _activeId).MarkActivated();
            QueueUrlIfNeeded();
            return;
        }

        NoteUnavailable(_activeId);
        _activeId = selectable[0].Id;
        selectable[0].MarkActivated();
        QueueUrlIfNeeded();
        _scrollStrip = true;
    }

    private void NoteUnavailable(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        var previous = _sections.FirstOrDefault(section => section.Id == id);
        if (previous is { EffectiveDirty: true, ShowsSaveControls: true })
        {
            Trace.TraceInformation("Settings section {0} became unavailable while it had unsaved changes.", id);
        }
    }

    private List<SbSettingsSection> SelectableSections() =>
        _sections.Where(section => section.Visible && !section.Disabled && !section.Pending).ToList();

    private bool NeedsLeaveGuard()
    {
        var active = Active;
        return ConfirmOnLeave && active is { ShowsSaveControls: true, EffectiveDirty: true };
    }

    private async Task CommitSectionAsync(SbSettingsSection section)
    {
        _saveFailed = false;
        _reasonOpenId = null;
        _userChoseSection = true;
        _activeId = section.Id;
        section.MarkActivated();
        _focusHeading = true;
        _scrollStrip = true;
        _urlWriteQueued = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task<bool> SaveActiveAsync()
    {
        var active = Active;
        if (active?.OnSave == null || !active.ShowsSaveControls || _saving)
        {
            return false;
        }

        _saving = true;
        bool saved;
        try
        {
            saved = await active.OnSave();
        }
        catch (Exception)
        {
            saved = false;
        }
        finally
        {
            _saving = false;
        }

        if (!saved)
        {
            _saveFailed = true;
            try
            {
                var focused = await JS.InvokeAsync<bool>("SufiBlazor.settingsLayout.focusFirstInvalid", _paneRef);
                if (!focused)
                {
                    await _errorRef.FocusAsync();
                }
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
        var historyState = _pendingHistoryState;
        _pendingSectionId = null;
        _pendingLocation = null;
        _pendingHistoryState = null;

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
            var sectionInLocation = ReadSectionFrom(location);
            if (!string.IsNullOrEmpty(sectionInLocation))
            {
                var target = SelectableSections().FirstOrDefault(section => section.Id == sectionInLocation);
                if (target != null)
                {
                    await CommitSectionAsync(target);
                }
            }

            _suppressLeaveGuard = true;
            try
            {
                Navigation.NavigateTo(location, new NavigationOptions
                {
                    ForceLoad = false,
                    ReplaceHistoryEntry = false,
                    HistoryEntryState = historyState
                });
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
        if (_suppressLeaveGuard)
        {
            return ValueTask.CompletedTask;
        }

        if (IsSamePage(context.TargetLocation))
        {
            var next = ReadSectionFrom(context.TargetLocation);
            if (!string.IsNullOrEmpty(next) && string.Equals(next, _activeId, StringComparison.Ordinal))
            {
                return ValueTask.CompletedTask;
            }

            var known = string.IsNullOrEmpty(next) ? null : _sections.FirstOrDefault(section => section.Id == next);
            var selectable = known is { Visible: true, Disabled: false, Pending: false };
            if (!selectable)
            {
                context.PreventNavigation();
                if (known?.Disabled == true)
                {
                    _reasonTop = 0;
                    _reasonLeft = 0;
                    _reasonOpenId = _reasonOpenId == next ? null : next;
                }

                if (!string.IsNullOrEmpty(_activeId))
                {
                    _rewriteCurrentSection = true;
                }

                _ = InvokeAsync(StateHasChanged);
                return ValueTask.CompletedTask;
            }

            if (NeedsLeaveGuard())
            {
                context.PreventNavigation();
                _pendingSectionId = next;
                _pendingLocation = context.TargetLocation;
                _pendingHistoryState = context.HistoryEntryState;
                _guardTriggerId = next;
                _guardFromStrip = false;
                _guardOpen = true;
                _ = InvokeAsync(StateHasChanged);
                return ValueTask.CompletedTask;
            }

            _ = RequestSectionAsync(next, fromStrip: false);
            return ValueTask.CompletedTask;
        }

        if (!NeedsLeaveGuard())
        {
            return ValueTask.CompletedTask;
        }

        context.PreventNavigation();
        _pendingLocation = context.TargetLocation;
        _pendingHistoryState = context.HistoryEntryState;
        _pendingSectionId = null;
        _guardTriggerId = _activeId;
        _guardFromStrip = false;
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

    private string? ReadSectionQuery() => ReadSectionFrom(Navigation.Uri);

    private string? ReadSectionFrom(string uri)
    {
        if (!DeepLink || string.IsNullOrEmpty(QueryParameter))
        {
            return null;
        }

        var query = Navigation.ToAbsoluteUri(uri).Query;
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

    private void QueueUrlIfNeeded()
    {
        if (!_sectionsStable || HasPending || !DeepLink || string.IsNullOrEmpty(_activeId))
        {
            return;
        }

        if (!string.Equals(ReadSectionQuery(), _activeId, StringComparison.Ordinal))
        {
            _urlWriteQueued = true;
        }
    }

    private void WriteSectionUrl(string? sectionId, bool rewriteCurrent = false)
    {
        if (!DeepLink || string.IsNullOrEmpty(sectionId) || HasPending)
        {
            return;
        }

        if (!rewriteCurrent && string.Equals(ReadSectionQuery(), sectionId, StringComparison.Ordinal))
        {
            return;
        }

        _suppressLeaveGuard = true;
        try
        {
            Navigation.NavigateTo(Navigation.GetUriWithQueryParameter(QueryParameter, sectionId), new NavigationOptions
            {
                ReplaceHistoryEntry = true
            });
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

    private async Task UpdateStripFadesAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("SufiBlazor.settingsLayout.updateStripFades", _stripRef);
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException)
        {
        }
    }

    private async Task ScrollStripItemAsync(string id)
    {
        try
        {
            await JS.InvokeVoidAsync(
                "SufiBlazor.settingsLayout.scrollIntoView",
                _rootRef,
                $".sb-settings-strip [data-section-id='{CssEscape(id)}']");
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
