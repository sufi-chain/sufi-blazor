namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// Selection behavior for <see cref="SbToggleGroup{TValue}"/>.
/// </summary>
public enum SbToggleSelectionMode
{
    /// <summary>
    /// One item stays selected. Choosing another item replaces the current value.
    /// </summary>
    SingleSelection,

    /// <summary>
    /// Any number of items can be selected. Bind <c>Values</c>.
    /// </summary>
    MultiSelection,

    /// <summary>
    /// One item can be selected, and choosing it again clears the value.
    /// </summary>
    ToggleSelection
}
