namespace SufiChain.SufiBlazor.Components.Forms;

// A form field wraps one input. Share the rendered label and description IDs
// with that input, including when callers do not supply an explicit InputId.
internal sealed record SbFormFieldContext(string InputId, string? LabelId, string? DescriptionId, bool Required, bool HasError);
