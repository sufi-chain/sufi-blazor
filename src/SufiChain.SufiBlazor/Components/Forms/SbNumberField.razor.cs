namespace SufiChain.SufiBlazor.Components.Forms;

public partial class SbNumberField<TNumber> where TNumber : struct, IComparable<TNumber>
{
    [Microsoft.AspNetCore.Components.CascadingParameter]
    private SbFormFieldContext? FormFieldContext { get; set; }
    private readonly string _generatedId = $"sb-number-field-{Guid.NewGuid():N}";
    private string EffectiveId => Id ?? FormFieldContext?.InputId ?? _generatedId;
    private string? InputDescription => string.Join(" ", new[] { FormFieldContext?.DescriptionId, string.IsNullOrEmpty(HelperText) ? null : $"{EffectiveId}-helper" }.Where(id => !string.IsNullOrEmpty(id)));
}
