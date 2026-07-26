namespace SufiChain.SufiBlazor.Components.Conversation;

/// <summary>
/// Domain-agnostic message model for <see cref="SbConversationTimeline"/>.
/// </summary>
public sealed class SbConversationMessageModel
{
    /// <summary>Message id.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Primary body text.</summary>
    public string? Body { get; set; }

    /// <summary>Optional sender display name.</summary>
    public string? SenderLabel { get; set; }

    /// <summary>Optional badge text (e.g. Internal note / Public reply).</summary>
    public string? BadgeLabel { get; set; }

    /// <summary>Optional badge color token name mapped by the host via template, or default styling.</summary>
    public bool IsHighlightedBadge { get; set; }

    /// <summary>When the message occurred.</summary>
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>Bubble alignment.</summary>
    public SbConversationAlignment Alignment { get; set; } = SbConversationAlignment.Start;

    /// <summary>Optional attachment labels for default rendering.</summary>
    public IReadOnlyList<string> AttachmentLabels { get; set; } = Array.Empty<string>();

    /// <summary>Opaque host payload for custom templates.</summary>
    public object? Payload { get; set; }
}
