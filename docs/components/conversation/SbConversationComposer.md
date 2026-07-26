# SbConversationComposer

In-field conversation composer shell used by Chat, Ticketing, and other hosts.

## UX rule

All addon actions live **inside** the rounded text surface:

- `StartActions` — emoji, attach, location entry
- `EndActions` — voice and related end-side controls
- `OverflowActions` — AI / copilot / future addons

Do **not** place a separate toolbar bar above the field. Pending chips may sit above the textarea but still inside the composer card.

## Related components

- `SbConversationTimeline` — message list with loading/empty states
- `SbConversationMessage` — single bubble with badge/meta/body/footer slots
- `SbConversationComposerSlots` — documents the slot names for host addons

## Capabilities

`SbConversationComposerCapabilities` is UI gating only. Hosts keep real authorization and upload/session logic.

## Send contract

`OnSend` raises `SbConversationSendRequest` with `Body` and an optional metadata dictionary. Domain adapters map this to Chat or Ticketing APIs.
