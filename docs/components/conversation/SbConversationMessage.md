# SbConversationMessage

Single conversation bubble with optional badge, sender, timestamp, body, and footer slots. Used directly or via `SbConversationTimeline` default rendering.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Body | string? | null | Plain-text body when `ChildContent` is null |
| SenderLabel | string? | null | Sender name above the bubble |
| BadgeLabel | string? | null | Badge text (e.g. "Internal note") |
| IsHighlightedBadge | bool | false | Highlight badge styling |
| Timestamp | DateTimeOffset? | null | Message time |
| TimestampFormat | string | `"g"` | `DateTimeOffset.ToString` format |
| Alignment | SbConversationAlignment | Start | Bubble side (`Start` / `End`) |
| AttachmentLabels | IReadOnlyList\<string\> | empty | Default footer attachment chips |
| HeaderContent | RenderFragment? | null | Custom meta row (replaces badge/sender/time) |
| ChildContent | RenderFragment? | null | Custom bubble body |
| FooterContent | RenderFragment? | null | Custom footer (replaces attachment labels) |
| Class / Style | string? | null | Root styling |

## Example

```razor
<SbConversationMessage SenderLabel="Support Agent"
                       BadgeLabel="Public reply"
                       Timestamp="@message.SentAt"
                       Alignment="SbConversationAlignment.Start"
                       Body="@message.Text" />
```

Custom body:

```razor
<SbConversationMessage Alignment="SbConversationAlignment.End" Timestamp="@DateTimeOffset.Now">
    <ChildContent>
        <SbMapPreview Latitude="35.7" Longitude="51.4" Label="Shared location" />
    </ChildContent>
</SbConversationMessage>
```

## See also

- [SbConversationTimeline](./SbConversationTimeline.md) — message list
- [SbConversationComposer](./SbConversationComposer.md) — input composer
