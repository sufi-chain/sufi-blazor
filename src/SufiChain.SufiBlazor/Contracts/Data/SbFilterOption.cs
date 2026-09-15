namespace SufiChain.SufiBlazor.Contracts.Data;

/// <summary>
/// A selectable value for a column filter (status, workspace, enum, etc.).
/// </summary>
public sealed class SbFilterOption
{
    public string Value { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public SbFilterOption()
    {
    }

    public SbFilterOption(string value, string text)
    {
        Value = value;
        Text = text;
    }
}
