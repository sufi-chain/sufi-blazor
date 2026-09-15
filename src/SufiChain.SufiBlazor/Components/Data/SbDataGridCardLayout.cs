namespace SufiChain.SufiBlazor.Components.Data;

/// <summary>
/// How <see cref="SbDataGrid{TItem}"/> renders rows on compact viewports.
/// </summary>
public enum SbDataGridCardLayout
{
    /// <summary>
    /// Table on desktop; stacked cards below the compact breakpoint (640px).
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Always render the table, even on phones.
    /// </summary>
    Never,

    /// <summary>
    /// Always render cards (useful for tests and demos).
    /// </summary>
    Always
}
