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
import { applyValueChunk, baselineOf, htmlLanguageConfig, isTextDirty, type TextBaseline } from "./transfer";

export { applyValueChunk, baselineOf, htmlLanguageConfig, isTextDirty };

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
  /** HTML/Scriban only. False stops CodeMirror inserting closing-tag text while typing. */
  autoCloseTags?: boolean;
}

const editors = new Map<string, EditorView>();
const baselines = new Map<string, TextBaseline>();
const suppressEditorNotify = new Set<string>();
const editorNotifyTimers = new Map<string, ReturnType<typeof setTimeout>>();
const dirtyNotifyDelayMs = 300;
let nextId = 1;

function rememberBaseline(editorId: string, text: string): void {
  baselines.set(editorId, baselineOf(text));
}

async function invokeEditor(dotNetRef: DotNetRef, stage: string, ...args: unknown[]): Promise<void> {
  try {
    await dotNetRef.invokeMethodAsync(stage, ...args);
  } catch {
    try {
      await dotNetRef.invokeMethodAsync("OnEditorTransferFailed", stage);
    } catch {
      /* The circuit is already gone. */
    }
  }
}

function notifyEditorContent(editorId: string, dotNetRef: DotNetRef, _view: EditorView): void {
  if (suppressEditorNotify.has(editorId)) {
    return;
  }

  const existing = editorNotifyTimers.get(editorId);
  if (existing) {
    clearTimeout(existing);
  }

  editorNotifyTimers.set(editorId, setTimeout(() => {
    editorNotifyTimers.delete(editorId);
    const current = editors.get(editorId);
    if (!current) {
      return;
    }
    const text = current.state.doc.toString();
    const dirty = isTextDirty(baselines.get(editorId), text);
    void invokeEditor(dotNetRef, "OnEditorDirtyChanged", dirty, text.length);
  }, dirtyNotifyDelayMs));
}

function languageExtension(language?: string, autoCloseTags?: boolean) {
  switch ((language ?? "plaintext").toLowerCase()) {
    case "json":
      return json();
    case "html":
    case "scriban":
      return html(htmlLanguageConfig(autoCloseTags));
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
          void invokeEditor(dotNetRef, "OnEditorShortcut", "save");
          return true;
        },
      },
      {
        key: "Mod-p",
        run: () => {
          void invokeEditor(dotNetRef, "OnEditorShortcut", "preview");
          return true;
        },
      },
    ]),
    languageExtension(options.language, options.autoCloseTags),
    EditorView.updateListener.of((update) => {
      if (update.docChanged) {
        notifyEditorContent(editorId, dotNetRef, update.view);
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

  element.setAttribute("dir", "ltr");
  editors.set(editorId, view);
  rememberBaseline(editorId, options.value ?? "");
  return editorId;
}

export function destroyEditor(editorId: string): void {
  const pending = editorNotifyTimers.get(editorId);
  if (pending) {
    clearTimeout(pending);
    editorNotifyTimers.delete(editorId);
  }
  const view = editors.get(editorId);
  view?.destroy();
  editors.delete(editorId);
  baselines.delete(editorId);
}

export function markSaved(editorId: string): void {
  const view = editors.get(editorId);
  if (!view) {
    return;
  }
  rememberBaseline(editorId, view.state.doc.toString());
}

export function getValue(editorId: string): string {
  return editors.get(editorId)?.state.doc.toString() ?? "";
}

export function getValueLength(editorId: string): number {
  return editors.get(editorId)?.state.doc.length ?? 0;
}

export function readValueChunk(editorId: string, offset: number, length: number): string {
  const doc = editors.get(editorId)?.state.doc;
  if (!doc) {
    return "";
  }
  const start = Math.max(0, offset);
  const end = Math.min(doc.length, start + Math.max(0, length));
  return doc.sliceString(start, end);
}

export function setValue(editorId: string, value: string): void {
  const view = editors.get(editorId);
  if (!view) {
    return;
  }
  suppressEditorNotify.add(editorId);
  try {
    view.dispatch({
      changes: { from: 0, to: view.state.doc.length, insert: value ?? "" },
    });
  } finally {
    suppressEditorNotify.delete(editorId);
    rememberBaseline(editorId, view.state.doc.toString());
  }
}

export function setValueChunk(editorId: string, chunk: string, reset: boolean): void {
  const view = editors.get(editorId);
  if (!view) {
    return;
  }
  suppressEditorNotify.add(editorId);
  try {
    const next = applyValueChunk(reset ? "" : view.state.doc.toString(), chunk ?? "", reset);
    view.dispatch({
      changes: { from: 0, to: view.state.doc.length, insert: next },
    });
  } finally {
    suppressEditorNotify.delete(editorId);
    rememberBaseline(editorId, view.state.doc.toString());
  }
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
