namespace SufiChain.SufiBlazor.Components.Conversation;

/// <summary>
/// Kind of pending draft item shown above the composer field.
/// </summary>
public enum SbConversationPendingKind
{
    /// <summary>File or media attachment.</summary>
    Attachment,

    /// <summary>Shared location.</summary>
    Location,

    /// <summary>Voice recording.</summary>
    Voice,

    /// <summary>Custom host-defined pending content.</summary>
    Custom
}
