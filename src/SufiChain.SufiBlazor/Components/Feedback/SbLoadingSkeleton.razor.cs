namespace SufiChain.SufiBlazor.Components.Feedback;

public partial class SbLoadingSkeleton
{
    /// <summary>
    /// Which placeholder layout to draw.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public SbLoadingSkeletonPreset Preset { get; set; } = SbLoadingSkeletonPreset.Table;

    /// <summary>
    /// How many placeholder bars or tiles to draw.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public int Count { get; set; } = 4;

    /// <summary>
    /// Announced loading text. Falls back to the localized <c>Loading</c> key.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Extra CSS classes on the status region.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// When false, the bars do not pulse.
    /// </summary>
    [Microsoft.AspNetCore.Components.Parameter]
    public bool Animation { get; set; } = true;

    private int SafeCount => Count < 1 ? 1 : Count;

    private string RootClass =>
        string.IsNullOrWhiteSpace(Class)
            ? $"sb-loading-skeleton sb-loading-skeleton--{PresetName}"
            : $"sb-loading-skeleton sb-loading-skeleton--{PresetName} {Class}";

    private string PresetName => Preset.ToString().ToLowerInvariant();

    private string BarWidth(int index) =>
        index % 3 == 2 ? "70%" : index % 2 == 1 ? "88%" : "100%";
}
