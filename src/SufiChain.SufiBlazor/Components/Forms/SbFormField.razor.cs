namespace SufiChain.SufiBlazor.Components.Forms;

public partial class SbFormField
{
    private readonly string _descriptionId = $"sb-form-field-{Guid.NewGuid():N}";
    private string EffectiveInputId => InputId ?? _descriptionId;
    private SbFormFieldContext FieldContext => new(EffectiveInputId,
        string.IsNullOrEmpty(Label) ? null : $"{EffectiveInputId}-field-label",
        !string.IsNullOrEmpty(ErrorText) ? ErrorId : !string.IsNullOrEmpty(HelperText) ? HelperId : null,
        Required, !string.IsNullOrEmpty(ErrorText));
}
