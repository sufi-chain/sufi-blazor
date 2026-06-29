using Microsoft.AspNetCore.Components;

namespace SufiChain.SufiBlazor.Components.Forms;

/// <summary>
/// Context interface for SbSelect to receive option registrations from SbSelectOption children.
/// </summary>
/// <typeparam name="TValue">The value type of the select options.</typeparam>
internal interface ISbSelectOptionContext<TValue>
{
    /// <summary>
    /// Registers an option with the parent select component.
    /// </summary>
    /// <param name="option">The option information to register.</param>
    void RegisterOption(SbSelectOptionInfo<TValue> option);

    /// <summary>
    /// Removes a previously registered option (e.g. when its SbSelectOption is disposed).
    /// </summary>
    /// <param name="value">The value of the option to unregister.</param>
    void UnregisterOption(TValue? value);
}

/// <summary>
/// Information about a select option defined via SbSelectOption component.
/// </summary>
/// <typeparam name="TValue">The value type of the option.</typeparam>
internal sealed class SbSelectOptionInfo<TValue>
{
    /// <summary>
    /// The value of the option.
    /// </summary>
    public TValue? Value { get; set; }

    /// <summary>
    /// The display text (extracted from ChildContent if needed).
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// The render fragment for custom display.
    /// </summary>
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether the option is disabled.
    /// </summary>
    public bool Disabled { get; set; }
}
