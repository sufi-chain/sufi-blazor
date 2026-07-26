namespace SufiChain.SufiBlazor.Components.Conversation;

/// <summary>
/// Domain-agnostic pending draft item displayed inside the composer card.
/// </summary>
public sealed class SbConversationPendingItem
{
    /// <summary>Stable id used for remove callbacks.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>Pending item kind.</summary>
    public SbConversationPendingKind Kind { get; set; }

    /// <summary>Short label shown on the chip.</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Optional icon name from SufiIcons.</summary>
    public string? Icon { get; set; }
}
