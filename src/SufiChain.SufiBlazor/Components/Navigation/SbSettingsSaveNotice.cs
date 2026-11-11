namespace SufiChain.SufiBlazor.Components.Navigation;

/// <summary>
/// Carries a validation message from a settings save up to the layout alert.
/// The generic save-failed sentence stays, and this text is appended when a save produced one.
/// </summary>
public sealed class SbSettingsSaveNotice
{
    public string? Detail { get; set; }

    public void Clear() => Detail = null;
}
