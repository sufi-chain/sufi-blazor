namespace SufiChain.SufiBlazor.Components.Conversation;

/// <summary>
/// Generic send payload raised by <see cref="SbConversationComposer"/>.
/// Hosts map this into Chat / Ticketing / other domain contracts.
/// </summary>
public sealed class SbConversationSendRequest
{
    /// <summary>Draft text body.</summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>Optional metadata bag for location, content kind, flags, etc.</summary>
    public IDictionary<string, object?> Metadata { get; set; } =
        new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
}
