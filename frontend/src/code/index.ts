import { EditorState } from "@codemirror/state";
import { EditorView, keymap, lineNumbers, placeholder as cmPlaceholder } from "@codemirror/view";
import { defaultKeymap, history, historyKeymap, indentWithTab } from "@codemirror/commands";
import { json, jsonParseLinter } from "@codemirror/lang-json";
import { html } from "@codemirror/lang-html";
import { markdown } from "@codemirror/lang-markdown";
import { css } from "@codemirror/lang-css";
import { javascript } from "@codemirror/lang-javascript";
import { xml } from "@codemirror/lang-xml";
import { linter } from "@codemirror/lint";
import { searchKeymap } from "@codemirror/search";
import "./code.css";

type DotNetRef = { invokeMethodAsync(method: string, ...args: unknown[]): Promise<unknown> };

interface InitOptions {
  value?: string;
  language?: string;
  readOnly?: boolean;
  disabled?: boolean;
  direction?: string;
  lineNumbers?: boolean;
  wordWrap?: boolean;
  placeholder?: string;
  validateJson?: boolean;
}

const editors = new Map<string, EditorView>();
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
  const editorId = `sb-code-${nextId++}`;
  const extensions = [
    history(),
    keymap.of([
      ...defaultKeymap,
      ...historyKeymap,
      indentWithTab,
      ...searchKeymap,
      {
        key: "Mod-s",
        run: () => {
          void dotNetRef.invokeMethodAsync("OnEditorShortcut", "save");
          return true;
        },
      },
      {
        key: "Mod-p",
        run: () => {
          void dotNetRef.invokeMethodAsync("OnEditorShortcut", "preview");
          return true;
        },
      },
    ]),
    languageExtension(options.language),
    EditorView.updateListener.of((update) => {
      if (update.docChanged) {
        void dotNetRef.invokeMethodAsync("OnEditorContentChanged", update.state.doc.toString());
      }
    }),
    EditorView.editable.of(!(options.readOnly || options.disabled)),
    EditorState.readOnly.of(!!options.readOnly),
  ];

  if (options.lineNumbers !== false) {
    extensions.push(lineNumbers());
  }
  if (options.wordWrap) {
    extensions.push(EditorView.lineWrapping);
  }
  if (options.placeholder) {
    extensions.push(cmPlaceholder(options.placeholder));
  }
  if (options.validateJson && (options.language ?? "").toLowerCase() === "json") {
    extensions.push(linter(jsonParseLinter()));
  }

  const view = new EditorView({
    parent: element,
    state: EditorState.create({
      doc: options.value ?? "",
      extensions,
    }),
  });

  element.setAttribute("dir", options.direction ?? "ltr");
  editors.set(editorId, view);
  return editorId;
}

export function destroyEditor(editorId: string): void {
  const view = editors.get(editorId);
  view?.destroy();
  editors.delete(editorId);
}

export function getValue(editorId: string): string {
  return editors.get(editorId)?.state.doc.toString() ?? "";
}

export function setValue(editorId: string, value: string): void {
  const view = editors.get(editorId);
  if (!view) {
    return;
  }
  view.dispatch({
    changes: { from: 0, to: view.state.doc.length, insert: value ?? "" },
  });
}

export function insertText(editorId: string, text: string): void {
  const view = editors.get(editorId);
  if (!view) {
    return;
  }
  const { from, to } = view.state.selection.main;
  view.dispatch({ changes: { from, to, insert: text ?? "" } });
}

export function getSelection(editorId: string): string {
  const view = editors.get(editorId);
  if (!view) {
    return "";
  }
  const { from, to } = view.state.selection.main;
  return view.state.doc.sliceString(from, to);
}

export function focusEditor(editorId: string): void {
  editors.get(editorId)?.focus();
}

export function formatJson(editorId: string): boolean {
  const view = editors.get(editorId);
  if (!view) {
    return false;
  }
  try {
    const parsed = JSON.parse(view.state.doc.toString());
    const formatted = JSON.stringify(parsed, null, 2);
    view.dispatch({ changes: { from: 0, to: view.state.doc.length, insert: formatted } });
    return true;
  } catch {
    return false;
  }
}

export function validateJson(editorId: string): boolean {
  const value = getValue(editorId);
  if (!value.trim()) {
    return true;
  }
  try {
    JSON.parse(value);
    return true;
  } catch {
    return false;
  }
}
