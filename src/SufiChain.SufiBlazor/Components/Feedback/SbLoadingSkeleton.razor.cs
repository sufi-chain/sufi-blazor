namespace SufiChain.SufiBlazor.Components.Feedback;

public partial class SbLoadingSkeleton
{
    private const int ShowDelayMilliseconds = 200;
    private const int MinimumVisibleMilliseconds = 400;

    private bool _showVisual;
    private bool _holding;
    private bool _disposed;
    private int _generation;
    private long _shownAt;
    private string _announced = "";
    private bool _wasBusy;

    /// <summary>
    /// Which placeholder layout to draw.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public SbLoadingSkeletonPreset Preset { get; set; } = SbLoadingSkeletonPreset.Table;

    /// <summary>
    /// How many placeholder rows or tiles to draw.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public int Count { get; set; } = 4;

    /// <summary>
    /// Announced loading text. Falls back to the localized <c>Loading</c> key.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Extra CSS classes on the placeholder container.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// When false, the bars do not pulse.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public bool Animation { get; set; } = true;

    /// <summary>
    /// When true, this placeholder is the page's one live region.
    /// Extra placeholders on the same page set this to false.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public bool Announce { get; set; } = true;

    /// <summary>
    /// True while the read is still running. The bars wait about 200ms, then stay at least 400ms.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public bool Busy { get; set; } = true;

    /// <summary>
    /// Shown after the placeholder has finished, when <see cref="Busy"/> is false.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public Microsoft.AspNetCore.Components.RenderFragment? ChildContent { get; set; }

    private int SafeCount => Count < 1 ? 1 : Count;

    private string LabelText => string.IsNullOrWhiteSpace(Label) ? L["Loading"] : Label;

    private bool ShowChild => ChildContent != null && !Busy && !_showVisual && !_holding;

    private string RootClass =>
        string.IsNullOrWhiteSpace(Class)
            ? $"sb-loading-skeleton sb-loading-skeleton--{PresetName}"
            : $"sb-loading-skeleton sb-loading-skeleton--{PresetName} {Class}";

    private string PresetName => Preset.ToString().ToLowerInvariant();

    protected override void OnParametersSet()
    {
        if (Busy == _wasBusy)
        {
            return;
        }

        _wasBusy = Busy;
        var generation = ++_generation;
        _ = SyncVisualAsync(Busy, generation);
    }

    public void Dispose()
    {
        _disposed = true;
        _generation++;
    }

    private async Task SyncVisualAsync(bool busy, int generation)
    {
        try
        {
            if (busy)
            {
                if (_showVisual)
                {
                    return;
                }

                await Task.Delay(ShowDelayMilliseconds);
                if (!StillCurrent(generation) || !Busy)
                {
                    return;
                }

                _showVisual = true;
                _holding = false;
                _shownAt = Environment.TickCount64;
                _announced = Announce ? LabelText : "";
                await InvokeAsync(StateHasChanged);
                return;
            }

            if (!_showVisual)
            {
                _announced = "";
                return;
            }

            var remaining = MinimumVisibleMilliseconds - (int)(Environment.TickCount64 - _shownAt);
            if (remaining > 0)
            {
                _holding = true;
                await Task.Delay(remaining);
                if (!StillCurrent(generation) || Busy)
                {
                    return;
                }
            }

            _holding = false;
            _showVisual = false;
            _announced = "";
            await InvokeAsync(StateHasChanged);
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private bool StillCurrent(int generation) => !_disposed && generation == _generation;
}
