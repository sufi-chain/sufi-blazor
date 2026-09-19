namespace SufiChain.SufiBlazor.Components.Conversation;

/// <summary>
/// Generic send payload raised by <see cref="SbConversationComposer"/>.
/// Hosts map this into Chat / Ticketing / other domain contracts.
/// </summary>
public sealed class SbConversationSendRequest
{
    /// <summary>Draft text body.</summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// The host can set this during OnSend to leave a draft for review (for example,
    /// a voice transcript). Null keeps the default of clearing after a successful send.
    /// </summary>
    public string? DraftAfterSend { get; set; }

    /// <summary>Optional metadata bag for location, content kind, flags, etc.</summary>
    public IDictionary<string, object?> Metadata { get; set; } =
        new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
