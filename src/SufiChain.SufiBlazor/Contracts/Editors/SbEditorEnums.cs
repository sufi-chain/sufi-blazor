namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Editor chrome mode for <c>SbRichTextEditor</c>.
/// </summary>
public enum SbEditorMode
{
    Html = 0,
    Markdown = 1,
    PlainText = 2,
    Wysiwyg = 3,
    ReadOnly = 4
}

/// <summary>
/// Serialized document format for get/set/insert operations.
/// </summary>
public enum SbContentFormat
{
    Html = 0,
    Markdown = 1,
    Json = 2
}

/// <summary>
/// CodeMirror language for <c>SbCodeEditor</c> and <c>SbDocumentDiffEditor</c>.
/// </summary>
public enum SbCodeLanguage
{
    PlainText = 0,
    Markdown = 1,
    Html = 2,
    Json = 3,
    Xml = 4,
    Css = 5,
    JavaScript = 6,
    TypeScript = 7,
    Scriban = 8
}

/// <summary>
/// Built-in editor commands executed through <see cref="ISbEditorDocument"/>.
/// </summary>
public enum SbEditorCommand
{
    Undo,
    Redo,
    Bold,
    Italic,
    Underline,
    Strike,
    Code,
    Highlight,
    Blockquote,
    CodeBlock,
    BulletList,
    OrderedList,
    TaskList,
    Heading1,
    Heading2,
    Heading3,
    Heading4,
    Heading5,
    Heading6,
    Paragraph,
    AlignLeft,
    AlignCenter,
    AlignRight,
    AlignJustify,
    HorizontalRule,
    ClearFormatting,
    InsertLink,
    InsertImage,
    InsertTable,
    InsertCallout
}

/// <summary>
/// Surfaces a toolbar contributor or item may target.
/// </summary>
[Flags]
public enum SbEditorSurface
{
    None = 0,
    Toolbar = 1,
    BubbleMenu = 2,
    SlashMenu = 4,
    BlockMenu = 8,
    Code = 16,
    FloatingMenu = 32
}

/// <summary>
/// Optional Tiptap-backed editor capabilities.
/// Mirrors <c>frontend/src/rich-text/features.ts</c>.
/// </summary>
[Flags]
public enum SbEditorFeatures
{
    None = 0,
    BubbleMenu = 1,
    FloatingMenu = 2,
    SlashCommands = 4,
    DragHandle = 8,
    Tables = 16,
    TaskLists = 32,
    Highlight = 64,
    TextColor = 128,
    Mermaid = 256,
    Callouts = 512,
    Emoji = 1024,
    Mentions = 2048,
    FindReplace = 4096,
    Ai = 8192,
    Default = BubbleMenu | FloatingMenu | SlashCommands | Tables | TaskLists | Highlight | TextColor | Mermaid | Callouts | FindReplace | Ai
}

/// <summary>
/// Visual density of the rich-text chrome.
/// </summary>
public enum SbEditorLayout
{
    Document = 0,
    Compact = 1
}
