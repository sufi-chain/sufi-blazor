namespace SufiChain.SufiBlazor.Components.Conversation;

/// <summary>
/// Marker documenting the in-composer addon extension points on
/// <see cref="SbConversationComposer"/>.
/// Hosts plug actions into <c>StartActions</c>, <c>EndActions</c>, or
/// <c>OverflowActions</c> — never into an external bar above the field.
/// </summary>
public static class SbConversationComposerSlots
{
    /// <summary>Inside-field start cluster (emoji, attach menu, location entry).</summary>
    public const string StartActions = nameof(SbConversationComposer.StartActions);

    /// <summary>Inside-field end cluster (voice, send companions).</summary>
    public const string EndActions = nameof(SbConversationComposer.EndActions);

    /// <summary>Inside-field overflow cluster for AI / future addons.</summary>
    public const string OverflowActions = nameof(SbConversationComposer.OverflowActions);
}
