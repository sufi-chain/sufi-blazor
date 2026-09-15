import { EditorState } from "@codemirror/state";
import { EditorView, keymap } from "@codemirror/view";
import { defaultKeymap, history, historyKeymap } from "@codemirror/commands";
import { MergeView } from "@codemirror/merge";
import { json } from "@codemirror/lang-json";
import { html } from "@codemirror/lang-html";
import { markdown } from "@codemirror/lang-markdown";
import { css } from "@codemirror/lang-css";
import { javascript } from "@codemirror/lang-javascript";
import { xml } from "@codemirror/lang-xml";
import "./diff.css";

type DotNetRef = { invokeMethodAsync(method: string, ...args: unknown[]): Promise<unknown> };

interface InitOptions {
  original?: string;
  suggested?: string;
  language?: string;
  readOnlySuggested?: boolean;
  direction?: string;
}

const editors = new Map<string, MergeView>();
let nextId = 1;

function languageExtension(language?: string) {
  switch ((language ?? "plaintext").toLowerCase()) {
    case "json":
      return json();
    case "html":
    case "scriban":
      return html();
    case "markdown":
      return markdown();
    case "css":
      return css();
    case "javascript":
      return javascript();
    case "typescript":
      return javascript({ typescript: true });
    case "xml":
      return xml();
    default:
      return [];
  }
}

export function initEditor(element: HTMLElement, dotNetRef: DotNetRef, options: InitOptions = {}): string {
  const editorId = `sb-diff-${nextId++}`;
  const lang = languageExtension(options.language);
  const merge = new MergeView({
    parent: element,
    a: {
      doc: options.original ?? "",
      extensions: [EditorState.readOnly.of(true), lang],
    },
    b: {
      doc: options.suggested ?? "",
      extensions: [
        history(),
        keymap.of([...defaultKeymap, ...historyKeymap]),
        lang,
        EditorView.updateListener.of((update) => {
          if (update.docChanged) {
            void dotNetRef.invokeMethodAsync("OnSuggestedChanged", update.state.doc.toString());
          }
        }),
        EditorState.readOnly.of(!!options.readOnlySuggested),
      ],
    },
    highlightChanges: true,
    gutter: true,
  });

  element.setAttribute("dir", options.direction ?? "ltr");
  editors.set(editorId, merge);
  return editorId;
}

export function destroyEditor(editorId: string): void {
  editors.get(editorId)?.destroy();
  editors.delete(editorId);
}

export function getSuggested(editorId: string): string {
  return editors.get(editorId)?.b.state.doc.toString() ?? "";
}

export function setValues(editorId: string, original: string, suggested: string): void {
  const merge = editors.get(editorId);
  if (!merge) {
    return;
  }
  merge.a.dispatch({ changes: { from: 0, to: merge.a.state.doc.length, insert: original ?? "" } });
  merge.b.dispatch({ changes: { from: 0, to: merge.b.state.doc.length, insert: suggested ?? "" } });
}

export function goToChunk(editorId: string, direction: "next" | "prev"): void {
  const merge = editors.get(editorId);
  if (!merge) {
    return;
  }
  const chunks = (merge as any).chunks as Array<{ fromB: number }> | undefined;
  if (!chunks?.length) {
    return;
  }
  const head = merge.b.state.selection.main.head;
  const index = chunks.findIndex((chunk) => chunk.fromB > head);
  const next = direction === "next"
    ? chunks[index >= 0 ? index : 0]
    : chunks[Math.max(0, (index < 0 ? chunks.length : index) - 1)];
  if (next) {
    merge.b.dispatch({ selection: { anchor: next.fromB }, scrollIntoView: true });
    merge.b.focus();
  }
}
