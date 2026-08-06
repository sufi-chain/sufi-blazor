namespace SufiChain.SufiBlazor.Components.Data;

/// <summary>
/// Specifies the freeze position for a column.
/// </summary>
public enum SbColumnFreeze
{
    /// <summary>
    /// Column is not frozen.
    /// </summary>
    None,
    
    /// <summary>
    /// Column is frozen to the start (left in LTR, right in RTL).
    /// </summary>
    Start,
    
    /// <summary>
    /// Column is frozen to the end (right in LTR, left in RTL).
    /// </summary>
    End
}
