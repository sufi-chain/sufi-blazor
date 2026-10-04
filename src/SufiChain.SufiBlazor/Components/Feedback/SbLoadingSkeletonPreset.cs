namespace SufiChain.SufiBlazor.Components.Feedback;

/// <summary>
/// Layout preview drawn by <see cref="SbLoadingSkeleton"/> while content is loading.
/// </summary>
public enum SbLoadingSkeletonPreset
{
    /// <summary>
    /// Stacked text lines, for a table or a form section.
    /// </summary>
    Table,

    /// <summary>
    /// Stacked blocks, for card lists.
    /// </summary>
    Cards,

    /// <summary>
    /// Side-by-side tiles, for KPI counts.
    /// </summary>
    Kpi,

    /// <summary>
    /// A few short lines.
    /// </summary>
    Text
}
