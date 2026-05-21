using System.Text.Json.Serialization;

namespace SufiChain.SufiBlazor.Components.Overlays;

internal class PopoverConstraintResult
{
    [JsonPropertyName("shiftX")]
    public int ShiftX { get; set; }

    [JsonPropertyName("shiftY")]
    public int ShiftY { get; set; }
}
