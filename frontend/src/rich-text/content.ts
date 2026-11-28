import type { Editor } from "@tiptap/core";

export type EditorContentType = "markdown" | "html" | "json";

export function toContentType(format: string | undefined): EditorContentType {
  const value = (format ?? "html").toLowerCase();
  if (value === "markdown") {
    return "markdown";
  }
  if (value === "json") {
    return "json";
  }
  return "html";
}

/** A programmatic set must not emit onUpdate. Tiptap re-serializes HTML and that echo was saved over the code view. */
export function programmaticContentOptions(format: string | undefined): { contentType: EditorContentType; emitUpdate: false } {
  return { contentType: toContentType(format), emitUpdate: false };
}

export function serializeContent(editor: Editor, format: string | undefined): string {
  const type = toContentType(format);
  if (type === "markdown") {
    return editor.getMarkdown();
  }
  if (type === "json") {
    return JSON.stringify(editor.getJSON());
  }
  return editor.getHTML();
}
