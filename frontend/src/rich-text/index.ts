import { Editor } from "@tiptap/core";
import BubbleMenu from "@tiptap/extension-bubble-menu";
import Placeholder from "@tiptap/extension-placeholder";
import CharacterCount from "@tiptap/extension-character-count";
import { serializeContent, toContentType } from "./content";
import { EditorFeature, hasFeature } from "./features";
import { createDocumentExtensions } from "./schema";
import { createEditorSnapshot } from "./snapshot";
import { SufiPasteCleanup } from "./extensions/paste-cleanup";
import {
  SufiAiSuggestion,
  setSuggestions,
  clearSuggestions,
  getSuggestionItems,
  getSuggestionRect as measureSuggestionRect,
  normalizeSuggestionItems,
  removeSuggestion,
  resolveRange,
} from "./extensions/ai-suggestion";
import { SufiKeymap } from "./extensions/keymap";
import { isAllowedUrl } from "./extensions/link";
import "./editor.css";

type DotNetRef = {
  invokeMethodAsync(method: string, ...args: unknown[]): Promise<unknown>;
};

interface InitOptions {
  placeholder?: string;
  readOnly?: boolean;
  disabled?: boolean;
  direction?: string;
  content?: string;
  contentFormat?: "markdown" | "html" | "json";
  pasteCleanup?: Record<string, unknown>;
  features?: number;
}

const editors = new Map<string, Editor>();
const editorFormats = new Map<string, string>();
let nextId = 1;

function notifyState(dotNetRef: DotNetRef, editor: Editor, editorId: string, format: string): void {
  void dotNetRef.invokeMethodAsync("OnEditorStateChanged", JSON.stringify(createEditorSnapshot(editor, editorId, format)));
}

export function initEditor(
  element: HTMLElement,
  dotNetRef: DotNetRef,
  options: InitOptions = {},
  bubbleMenuElement?: HTMLElement | null,
): string {
  const editorId = `sb-rte-${nextId++}`;
  const format = (options.contentFormat ?? "html").toLowerCase();
  const features = options.features ?? EditorFeature.Default;
  const extensions = [
    ...createDocumentExtensions({ features }),
    Placeholder.configure({ placeholder: options.placeholder ?? "" }),
    CharacterCount,
    SufiPasteCleanup.configure(options.pasteCleanup ?? {}),
    SufiAiSuggestion,
    SufiKeymap,
  ];

  if (bubbleMenuElement && hasFeature(features, EditorFeature.BubbleMenu)) {
    extensions.push(
      BubbleMenu.configure({
        element: bubbleMenuElement,
        shouldShow: ({ editor: current, from, to }) => current.isEditable && from !== to
      }),
    );
  }

  const editor = new Editor({
    element,
    editable: !(options.readOnly || options.disabled),
    content: options.content ?? "",
    contentType: toContentType(format),
    extensions,
    editorProps: {
      attributes: {
        class: "sb-editor__prose",
        dir: options.direction ?? "ltr",
        role: "textbox",
        "aria-multiline": "true",
      },
    },
    onUpdate: ({ editor: current }) => {
      void dotNetRef.invokeMethodAsync(
        "OnEditorContentChanged",
        serializeContent(current, format),
        current.getHTML(),
        current.getText(),
      );
      notifyState(dotNetRef, current, editorId, format);
    },
    onSelectionUpdate: ({ editor: current }) => {
      notifyState(dotNetRef, current, editorId, format);
    },
    onCreate: ({ editor: current }) => {
      notifyState(dotNetRef, current, editorId, format);
    },
  });

  editor.on("sufiShortcut" as never, (payload: { name: string }) => {
    void dotNetRef.invokeMethodAsync("OnEditorShortcut", payload.name);
  });

  editors.set(editorId, editor);
  editorFormats.set(editorId, format);
  return editorId;
}

export function destroyEditor(editorId: string): void {
  const editor = editors.get(editorId);
  if (!editor) {
    return;
  }
  editor.destroy();
  editors.delete(editorId);
  editorFormats.delete(editorId);
}

export function getContent(editorId: string, format: string): string {
  const editor = editors.get(editorId);
  return editor ? serializeContent(editor, format) : "";
}

export function setContent(editorId: string, content: string, format: string): void {
  const editor = editors.get(editorId);
  if (!editor) {
    return;
  }
  editor.commands.setContent(content ?? "", { contentType: toContentType(format) });
}

export function focusEditor(editorId: string): void {
  editors.get(editorId)?.commands.focus();
}

export function setEditable(editorId: string, editable: boolean): void {
  const editor = editors.get(editorId);
  if (editor) {
    editor.setEditable(editable);
  }
}

export function setDirection(editorId: string, direction: string): void {
  editors.get(editorId)?.view.dom.setAttribute("dir", direction);
}

export function execCommand(editorId: string, command: string, value?: unknown): boolean {
  const editor = editors.get(editorId);
  if (!editor) {
    return false;
  }
  const chain = editor.chain().focus();
  switch (command) {
    case "Undo":
      return chain.undo().run();
    case "Redo":
      return chain.redo().run();
    case "Bold":
      return chain.toggleBold().run();
    case "Italic":
      return chain.toggleItalic().run();
    case "Underline":
      return chain.toggleUnderline().run();
    case "Strike":
      return chain.toggleStrike().run();
    case "Code":
      return chain.toggleCode().run();
    case "Highlight":
      return chain.toggleHighlight().run();
    case "Blockquote":
      return chain.toggleBlockquote().run();
    case "CodeBlock":
      return chain.toggleCodeBlock().run();
    case "BulletList":
      return chain.toggleBulletList().run();
    case "OrderedList":
      return chain.toggleOrderedList().run();
    case "TaskList":
      return chain.toggleTaskList().run();
    case "Heading1":
    case "Heading2":
    case "Heading3":
    case "Heading4":
    case "Heading5":
    case "Heading6":
      return chain.toggleHeading({ level: Number(command.replace("Heading", "")) as 1 | 2 | 3 | 4 | 5 | 6 }).run();
    case "Paragraph":
      return chain.setParagraph().run();
    case "AlignLeft":
      return chain.setTextAlign("left").run();
    case "AlignCenter":
      return chain.setTextAlign("center").run();
    case "AlignRight":
      return chain.setTextAlign("right").run();
    case "AlignJustify":
      return chain.setTextAlign("justify").run();
    case "HorizontalRule":
      return chain.setHorizontalRule().run();
    case "ClearFormatting":
      return chain.unsetAllMarks().clearNodes().run();
    case "InsertTable":
      return chain.insertTable({ rows: 3, cols: 3, withHeaderRow: true }).run();
    case "InsertCallout":
      return chain
        .insertContent({
          type: "sufiCallout",
          attrs: { kind: (value as string) || "note" },
          content: [{ type: "paragraph" }],
        })
        .run();
    default:
      return false;
  }
}

export function insertContent(editorId: string, text: string, format: string): void {
  const editor = editors.get(editorId);
  if (!editor) {
    return;
  }
  editor.chain().focus().insertContent(text ?? "", { contentType: toContentType(format) }).run();
}

export function insertLink(editorId: string, url: string, text?: string, target?: string, rel?: string): void {
  if (!isAllowedUrl(url)) {
    return;
  }
  const editor = editors.get(editorId);
  if (!editor) {
    return;
  }
  if (editor.state.selection.empty && text) {
    editor.chain().focus().insertContent(text).run();
  }
  editor
    .chain()
    .focus()
    .extendMarkRange("link")
    .setLink({ href: url, target: target || null, rel: rel || "noopener noreferrer" })
    .run();
}

export function insertImage(editorId: string, url: string, alt?: string, width?: string, height?: string): void {
  editors.get(editorId)?.chain().focus().setImage({ src: url, alt: alt ?? "", width, height } as never).run();
}

export function insertFile(editorId: string, url: string, name: string): void {
  insertLink(editorId, url, name, "_blank", "noopener noreferrer");
}

export function applyMark(editorId: string, mark: string, attrs?: Record<string, unknown>): void {
  const editor = editors.get(editorId);
  if (!editor) {
    return;
  }
  if (mark === "textStyle" || mark === "fontFamily") {
    editor.chain().focus().setFontFamily(String(attrs?.fontFamily ?? attrs?.font ?? "")).run();
    return;
  }
  editor.chain().focus().setMark(mark, attrs ?? {}).run();
}

export function applyBlock(editorId: string, block: string, attrs?: Record<string, unknown>): void {
  const editor = editors.get(editorId);
  if (!editor) {
    return;
  }
  if (block === "heading") {
    editor.chain().focus().setHeading({ level: Number(attrs?.level ?? 1) as 1 | 2 | 3 | 4 | 5 | 6 }).run();
    return;
  }
  if (block === "paragraph") {
    editor.chain().focus().setParagraph().run();
  }
}

export function getSelection(editorId: string): { text: string; from: number; to: number; nodeType: string } {
  const editor = editors.get(editorId);
  if (!editor) {
    return { text: "", from: 0, to: 0, nodeType: "" };
  }
  const { from, to } = editor.state.selection;
  return {
    text: editor.state.doc.textBetween(from, to, " "),
    from,
    to,
    nodeType: activeNodeName(editor),
  };
}

function activeNodeName(editor: Editor): string {
  return (
    ["heading", "paragraph", "blockquote", "codeBlock", "bulletList", "orderedList", "taskList", "table", "sufiCallout"].find(
      (name) => editor.isActive(name),
    ) ?? editor.state.selection.$from.parent.type.name
  );
}

export function replaceSelection(editorId: string, text: string, format: string): void {
  const editor = editors.get(editorId);
  editor?.chain().focus().insertContent(text ?? "", { contentType: toContentType(format) }).run();
}

export function showSuggestions(editorId: string, itemsJson: string): void {
  const editor = editors.get(editorId);
  if (!editor) {
    return;
  }
  const items = normalizeSuggestionItems(JSON.parse(itemsJson || "[]"));
  setSuggestions(editor.view, items);
}

export function acceptSuggestion(editorId: string, id: string): void {
  const editor = editors.get(editorId);
  if (!editor) {
    return;
  }

  const item = getSuggestionItems(editor.view).find((entry) => entry.id === id);
  if (!item) {
    return;
  }

  const format = editorFormats.get(editorId) ?? "html";
  const { from, to } = resolveRange(editor.state.doc.content.size, item);
  const content = item.insertText ?? "";
  const spansDocument = from <= 1 && to >= editor.state.doc.content.size;

  if (item.kind === "delete") {
    editor.chain().focus().deleteRange({ from, to }).run();
  } else if (item.kind === "insert") {
    editor.chain().focus().insertContentAt(from, content, { contentType: toContentType(format) }).run();
  } else if (spansDocument) {
    editor.commands.setContent(content, { contentType: toContentType(format) });
  } else {
    editor.chain().focus().insertContentAt({ from, to }, content, { contentType: toContentType(format) }).run();
  }

  removeSuggestion(editor.view, id, "accept");
}

export function rejectSuggestion(editorId: string, id: string): void {
  const editor = editors.get(editorId);
  if (!editor) {
    return;
  }
  removeSuggestion(editor.view, id, "reject");
}

export function clearEditorSuggestions(editorId: string): void {
  const editor = editors.get(editorId);
  if (editor) {
    clearSuggestions(editor.view);
  }
}

export function getSuggestionRect(editorId: string, id?: string): {
  top: number;
  left: number;
  width: number;
  height: number;
} | null {
  const editor = editors.get(editorId);
  if (!editor) {
    return null;
  }
  const origin =
    editor.view.dom.closest(".sb-editor-host")?.getBoundingClientRect() ??
    editor.view.dom.closest(".sb-editor")?.getBoundingClientRect();
  return measureSuggestionRect(editor.view, id, origin);
}

export function streamInsert(editorId: string, chunk: string): void {
  insertContent(editorId, chunk, "html");
}

export function getState(editorId: string, format: string): string {
  const editor = editors.get(editorId);
  return editor ? JSON.stringify(createEditorSnapshot(editor, editorId, format)) : "{}";
}
