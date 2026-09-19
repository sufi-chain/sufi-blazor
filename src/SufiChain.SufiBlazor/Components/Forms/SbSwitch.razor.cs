namespace SufiChain.SufiBlazor.Components.Forms;

public partial class SbSwitch
{
    [Microsoft.AspNetCore.Components.CascadingParameter]
    private SbFormFieldContext? FormFieldContext { get; set; }
    private readonly string _generatedId = $"sb-field-{Guid.NewGuid():N}";
    private string EffectiveId => Id ?? FormFieldContext?.InputId ?? _generatedId;
}
