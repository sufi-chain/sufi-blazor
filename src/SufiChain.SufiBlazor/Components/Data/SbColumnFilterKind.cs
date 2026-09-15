namespace SufiChain.SufiBlazor.Components.Data;

/// <summary>
/// Controls the inline filter-row editor for a column.
/// </summary>
public enum SbColumnFilterKind
{
    /// <summary>
    /// Infer from field type and <c>FilterItems</c>: select when options exist or the field is bool/enum; otherwise text.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Free-text filter (Contains).
    /// </summary>
    Text,

    /// <summary>
    /// Single-select from <c>FilterItems</c> or inferred enum values (Equals).
    /// </summary>
    Select,

    /// <summary>
    /// Yes/No select for boolean fields (Equals).
    /// </summary>
    Boolean
}
