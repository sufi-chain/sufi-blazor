using Microsoft.AspNetCore.Components;

namespace SufiChain.SufiBlazor.Components.Forms.Editor;

public partial class SbEditorFooter : ComponentBase
{
    [Parameter] public int WordCount { get; set; }
    [Parameter] public int CharacterCount { get; set; }
    [Parameter] public bool ShowWordCount { get; set; }
    [Parameter] public bool ShowCharacterCount { get; set; }
    [Parameter] public string WordCountFormat { get; set; } = "{0} words";
    [Parameter] public string CharacterCountFormat { get; set; } = "{0} characters";
}
