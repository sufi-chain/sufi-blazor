# SbConversationTimeline

Domain-agnostic message list for chat-like threads.

## Parameters

| Parameter | Description |
|-----------|-------------|
| `Messages` | `IEnumerable<SbConversationMessageModel>` |
| `IsLoading` | Shows loading state |
| `IsWaitingForResponse` | Shows an assistant thinking bubble after the last message |
| `WaitingText` / `WaitingSenderLabel` / `WaitingContent` | Thinking-bubble presentation |
| `EmptyText` / `EmptyContent` | Empty thread presentation |
| `LoadingText` / `LoadingContent` | Loading-state presentation |
| `MessageTemplate` | Optional custom bubble render |

Default rendering uses `SbConversationMessage` with start/end alignment, badges, timestamps, and attachment labels. When `IsWaitingForResponse` is true, a thinking bubble is appended after the message list.

Hosts supply localized badge text and map their DTOs into `SbConversationMessageModel` (see Ticketing `TicketDetail` and Chat adapters).
