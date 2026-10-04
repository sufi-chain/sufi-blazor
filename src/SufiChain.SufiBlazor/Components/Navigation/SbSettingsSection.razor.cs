using Microsoft.AspNetCore.Components;

namespace SufiChain.SufiBlazor.Components.Navigation;

/// <summary>
/// One section inside <see cref="SbSettingsLayout"/>. The id is stable and is never localized.
/// </summary>
public partial class SbSettingsSection : ComponentBase, IDisposable
{
    private SbSettingsLayout? _layout;
    private bool _locallyClean;
    private bool _seenIsDirty;
    private bool _seenIsDirtyReady;
    private string? _parameterSnapshot;
    private bool _hasBeenActivated;

    [CascadingParameter]
    private SbSettingsLayout? Layout { get; set; }

    /// <summary>
    /// Stable kebab-case id used in the URL. Never localized.
    /// </summary>
    [Parameter, EditorRequired]
    public string Id { get; set; } = "";

    /// <summary>
    /// Rail label. Keep it to three words or fewer.
    /// </summary>
    [Parameter, EditorRequired]
    public string Label { get; set; } = "";

    /// <summary>
    /// Registered SufiIcons name.
    /// </summary>
    [Parameter, EditorRequired]
    public string Icon { get; set; } = "";

    /// <summary>
    /// Save-bar heading. Falls back to <see cref="Label"/>.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// One line under the save-bar heading.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// False hides the section from the rail and from deep links.
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// The section is still deciding whether it is visible, for example while a permission check is running.
    /// The layout waits, and does not fall back, rewrite the URL, or show the empty state.
    /// </summary>
    [Parameter]
    public bool Pending { get; set; }

    /// <summary>
    /// View-only section. The save bar shows a badge and no Save or Discard.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Feature unavailable. The section stays in the rail with <see cref="DisabledReason"/>, and deep links fall back.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Why the section is disabled. Shown as a rail tooltip.
    /// </summary>
    [Parameter]
    public string? DisabledReason { get; set; }

    /// <summary>
    /// Optional rail badge, such as a count.
    /// </summary>
    [Parameter]
    public RenderFragment? Badge { get; set; }

    /// <summary>
    /// The section content reports unsaved changes.
    /// Bind with <c>@bind-IsDirty</c>. Discard and a successful save set this back to false.
    /// A parent that leaves it true after discard stays clean until the parent changes the value.
    /// </summary>
    [Parameter]
    public bool IsDirty { get; set; }

    /// <summary>
    /// Raised when the layout clears the dirty flag after a successful save or a discard.
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsDirtyChanged { get; set; }

    /// <summary>
    /// Saves the section. Return false to keep the dirty state. Null means the section has no form to save.
    /// </summary>
    [Parameter]
    public Func<Task<bool>>? OnSave { get; set; }

    /// <summary>
    /// Restores the last saved values.
    /// </summary>
    [Parameter]
    public Func<Task>? OnDiscard { get; set; }

    /// <summary>
    /// False while the form is invalid. Save stays visible and disabled.
    /// </summary>
    [Parameter]
    public bool CanSave { get; set; } = true;

    /// <summary>
    /// Secondary actions in the save bar. At most two.
    /// </summary>
    [Parameter]
    public RenderFragment? BarActions { get; set; }

    /// <summary>
    /// Content renders on first activation and stays mounted.
    /// </summary>
    [Parameter]
    public bool Lazy { get; set; } = true;

    /// <summary>
    /// Section body. Use H3 sub-sections, not tabs.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    internal string PanelDomId => $"sb-settings-section-{Id}";

    internal string HeadingText => string.IsNullOrWhiteSpace(Title) ? Label : Title;

    internal bool HasBeenActivated => _hasBeenActivated;

    internal bool EffectiveDirty => IsDirty && !_locallyClean;

    internal bool ShowsSaveControls => OnSave != null && !ReadOnly && !Disabled;

    protected override void OnInitialized()
    {
        _layout = Layout;
        _layout?.Register(this);
    }

    protected override void OnParametersSet()
    {
        if (!_seenIsDirtyReady)
        {
            _seenIsDirty = IsDirty;
            _seenIsDirtyReady = true;
        }
        else if (IsDirty != _seenIsDirty)
        {
            _seenIsDirty = IsDirty;
            _locallyClean = false;
        }

        if (!IsDirty)
        {
            _locallyClean = false;
        }

        if (!ReferenceEquals(_layout, Layout))
        {
            _layout?.Unregister(this);
            _layout = Layout;
            _layout?.Register(this);
        }

        var snapshot = string.Join('|', Visible, Pending, Disabled, ReadOnly, IsDirty, CanSave, Label, Icon, HeadingText, Description, DisabledReason);
        if (!string.Equals(snapshot, _parameterSnapshot, StringComparison.Ordinal))
        {
            _parameterSnapshot = snapshot;
            _layout?.NotifySectionChanged();
        }
    }

    internal void MarkActivated() => _hasBeenActivated = true;

    internal async Task MarkCleanAsync()
    {
        _locallyClean = true;
        if (IsDirtyChanged.HasDelegate)
        {
            await IsDirtyChanged.InvokeAsync(false);
        }
    }

    public void Dispose()
    {
        _layout?.Unregister(this);
        _layout = null;
    }
}
