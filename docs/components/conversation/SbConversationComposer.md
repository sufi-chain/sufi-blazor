# SbConversationComposer

In-field conversation composer shell used by Chat, Ticketing, and other hosts.

## UX rule

All addon actions live **inside** the rounded text surface, on one row with the textarea:

- `StartActions` — emoji, attach, location entry (leading side)
- textarea — one line at rest, grows while typing
- `OverflowActions` — AI / copilot / future addons
- `EndActions` — voice and related end-side controls
- default send — trailing icon

Do **not** place a separate toolbar bar above the field. Pending chips may sit above the body row but still inside the composer card.

## Related components

- `SbConversationTimeline` — message list with loading/empty/thinking states
- `SbConversationMessage` — single bubble with badge/meta/body/footer slots
- `SbConversationThinkingBubble` — canonical waiting bubble; optional live progress child content
- `SbConversationComposerSlots` — documents the slot names for host addons

## Capabilities

`SbConversationComposerCapabilities` is UI gating only. Hosts keep real authorization and upload/session logic.

## Send contract

`OnSend` raises `SbConversationSendRequest` with `Body` and an optional metadata dictionary. Domain adapters map this to Chat or Ticketing APIs.

The textarea uses Blazor's `oninput` bind pipeline so rapid typing remains ordered when an Interactive Server circuit has staging or internet latency.
