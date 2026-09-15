import type { Editor } from "@tiptap/core";

const MARKS = ["bold", "italic", "underline", "strike", "code", "highlight", "link"] as const;
const NODES = [
  "heading",
  "paragraph",
  "blockquote",
  "codeBlock",
  "bulletList",
  "orderedList",
  "taskList",
  "table",
  "sufiCallout",
] as const;

function attrString(value: unknown): string | undefined {
  if (value == null || value === "") {
    return undefined;
  }
  return String(value);
}

export function createEditorSnapshot(editor: Editor, editorId: string, format: string) {
  const { from, to } = editor.state.selection;
  const activeMarks = MARKS.filter((name) => editor.isActive(name));
  const activeNode =
    NODES.find((name) => editor.isActive(name)) ?? editor.state.selection.$from.parent.type.name;
  const headingAttrs = editor.getAttributes("heading");
  const linkAttrs = editor.getAttributes("link");
  const nodeAttrs = editor.getAttributes(activeNode);
  const textAlign =
    attrString(editor.getAttributes("paragraph").textAlign) ??
    attrString(headingAttrs.textAlign) ??
    attrString(nodeAttrs.textAlign);
  const words =
    editor.storage.characterCount?.words?.() ??
    editor.getText().trim().split(/\s+/).filter(Boolean).length;
  const characters = editor.storage.characterCount?.characters?.() ?? editor.getText().length;

  return {
    editorId,
    canUndo: editor.can().undo(),
    canRedo: editor.can().redo(),
    isEmpty: editor.isEmpty,
    selectionText: editor.state.doc.textBetween(from, to, " "),
    selectionFrom: from,
    selectionTo: to,
    activeNode,
    headingLevel: editor.isActive("heading") ? Number(headingAttrs.level ?? 0) : 0,
    contentFormat: format,
    characterCount: characters,
    wordCount: words,
    activeMarks,
    activeMarkAttrs: {
      href: editor.isActive("link") ? attrString(linkAttrs.href) : undefined,
    },
    activeNodeAttrs: {
      ...Object.fromEntries(
        Object.entries(nodeAttrs ?? {}).map(([key, value]) => [key, attrString(value)]),
      ),
      textAlign,
    },
  };
}
