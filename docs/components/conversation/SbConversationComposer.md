# SbConversationComposer

In-field conversation composer shell used by Chat, Ticketing, and other hosts.

## UX rule

All addon actions live **inside** the rounded text surface. The field is a full-width first row; tools sit on the second row:

- `.sb-conversation-composer__field` — full-width typing hit target wrapping the textarea (`order: -1`, `flex: 1 0 100%`)
- textarea — one line at rest, grows while typing
- `StartActions` — input tools (emoji, attach, voice, location) on the leading side of the tool row
- `OverflowActions` — leading addons that appear from host/copilot config (model selector, context)
- `EndActions` — send-adjacent controls (shortcut popover)
- default send — trailing icon on the tool row

Do **not** place a separate toolbar bar above the field. Pending chips may sit above the body but still inside the composer card. Dynamic tools must join the leading cluster. Do not move send or shortcut actions onto the leading side.

The textarea uses `field-sizing: content` so height grows with text. Keep it inside `.sb-conversation-composer__field` with a full-width flex basis and `min-inline-size: 100%`. Without that wrapper, an empty field shrinks to content width and clicks on the well miss the textarea.

## Related components

- `SbConversationTimeline` — message list with loading/empty/thinking states
- `SbConversationMessage` — single bubble with badge/meta/body/footer slots
- `SbConversationThinkingBubble` — canonical waiting bubble; optional live progress child content
- `SbConversationComposerSlots` — documents the slot names for host addons

## Capabilities

`SbConversationComposerCapabilities` is UI gating only. Hosts keep real authorization and upload/session logic.

## Send contract

Set `IsSending` while a request is pending. The send button shows its built-in spinner and `aria-busy`, and both button and Enter submissions are blocked. Reset it when the request completes or fails. `IsDisabled` remains available for non-loading restrictions.

`OnSend` raises `SbConversationSendRequest` with `Body` and an optional metadata dictionary. Domain adapters map this to Chat or Ticketing APIs.

For a send action that prepares text for review, such as voice transcription, set
`request.DraftAfterSend` to the resulting draft before the callback returns. The
composer displays that text and reports it through `ValueChanged`. Null (the
default) clears the draft after a successful send. The host remains responsible
for deciding whether to send a message or only prepare a draft.

The textarea uses Blazor's `oninput` bind pipeline so rapid typing remains ordered when an Interactive Server circuit has staging or internet latency.
