namespace SufiChain.SufiBlazor.Components.Conversation;

/// <summary>
/// UI capability flags for <see cref="SbConversationComposer"/>.
/// Authorization remains the host's responsibility; these only gate chrome.
/// </summary>
public sealed class SbConversationComposerCapabilities
{
    /// <summary>Show rich in-field actions (attach, emoji, etc.).</summary>
    public bool CanUseRichComposer { get; set; } = true;

    /// <summary>Allow attachment actions.</summary>
    public bool CanAttachFiles { get; set; }

    /// <summary>Allow location share actions.</summary>
    public bool CanShareLocation { get; set; }

    /// <summary>Allow voice recording actions.</summary>
    public bool CanRecordVoice { get; set; }

    /// <summary>Allow emoji picker actions.</summary>
    public bool CanUseEmoji { get; set; } = true;

    /// <summary>Allow AI / overflow addon cluster.</summary>
    public bool CanUseAiAddons { get; set; }

    /// <summary>Maximum voice recording length in seconds (hint for hosts).</summary>
    public int MaxVoiceRecordingSeconds { get; set; } = 120;
}
