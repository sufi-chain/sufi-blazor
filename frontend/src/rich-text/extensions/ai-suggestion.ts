import { Extension } from "@tiptap/core";
import { Plugin, PluginKey } from "@tiptap/pm/state";
import { Decoration, DecorationSet, type EditorView } from "@tiptap/pm/view";

export interface SuggestionRange {
  id: string;
  kind: "insert" | "delete" | "replace";
  from: number;
  to: number;
  insertText?: string;
  deleteText?: string;
}

interface SuggestionState {
  decorations: DecorationSet;
  items: SuggestionRange[];
}

const key = new PluginKey<SuggestionState>("sufiAiSuggestion");

export function clampPos(value: number, min: number, max: number): number {
  return Math.max(min, Math.min(max, value));
}

export function resolveRange(docSize: number, item: SuggestionRange): { from: number; to: number } {
  const from = clampPos(item.from || 1, 1, docSize);
  if (item.kind === "insert" && (!item.to || item.to <= 0)) {
    return { from, to: from };
  }

  const rawTo = item.to && item.to > 0 ? item.to : docSize;
  const to = clampPos(rawTo, from, docSize);
  return { from, to };
}

export function normalizeSuggestionItems(raw: unknown): SuggestionRange[] {
  if (!Array.isArray(raw)) {
    return [];
  }

  return raw
    .map((entry) => {
      const record = (entry ?? {}) as Record<string, unknown>;
      const kindRaw = String(record.kind ?? record.Kind ?? "replace").toLowerCase();
      const kind: SuggestionRange["kind"] =
        kindRaw === "insert" || kindRaw === "delete" ? kindRaw : "replace";
      return {
        id: String(record.id ?? record.Id ?? ""),
        kind,
        from: Number(record.from ?? record.From ?? 1),
        to: Number(record.to ?? record.To ?? 0),
        insertText: record.insertText != null || record.InsertText != null
          ? String(record.insertText ?? record.InsertText)
          : undefined,
        deleteText: record.deleteText != null || record.DeleteText != null
          ? String(record.deleteText ?? record.DeleteText)
          : undefined,
      };
    })
    .filter((item) => item.id.length > 0);
}

function createInsertWidget(item: SuggestionRange): HTMLElement {
  const widget = document.createElement("span");
  widget.className = "sb-suggestion sb-suggestion--insert";
  widget.setAttribute("data-sb-suggestion", item.id);
  widget.textContent = item.insertText ?? "";
  return widget;
}

function buildDecorations(doc: { content: { size: number } }, items: SuggestionRange[]): DecorationSet {
  const decorations: Decoration[] = [];
  const size = doc.content.size;

  for (const item of items) {
    const { from, to } = resolveRange(size, item);

    if ((item.kind === "delete" || item.kind === "replace") && to > from) {
      decorations.push(
        Decoration.inline(from, to, {
          class: "sb-suggestion sb-suggestion--delete",
          "data-sb-suggestion": item.id,
        }),
      );
    }

    if ((item.kind === "insert" || item.kind === "replace") && (item.insertText ?? "").length > 0) {
      decorations.push(
        Decoration.widget(item.kind === "insert" ? from : to, () => createInsertWidget(item), {
          side: 1,
          key: `sb-suggestion-insert-${item.id}`,
        }),
      );
    }
  }

  return DecorationSet.create(doc as never, decorations);
}

export const SufiAiSuggestion = Extension.create({
  name: "sufiAiSuggestion",
  addProseMirrorPlugins() {
    return [
      new Plugin<SuggestionState>({
        key,
        state: {
          init: () => ({ decorations: DecorationSet.empty, items: [] }),
          apply(tr, value) {
            const mapped = {
              items: value.items,
              decorations: value.decorations.map(tr.mapping, tr.doc),
            };
            const meta = tr.getMeta(key) as
              | SuggestionRange[]
              | { accept?: string; reject?: string; clear?: boolean }
              | undefined;
            if (!meta) {
              return mapped;
            }
            if (Array.isArray(meta)) {
              return { items: meta, decorations: buildDecorations(tr.doc, meta) };
            }
            if (meta.clear) {
              return { items: [], decorations: DecorationSet.empty };
            }
            const remaining = mapped.items.filter((item) => item.id !== meta.accept && item.id !== meta.reject);
            return { items: remaining, decorations: buildDecorations(tr.doc, remaining) };
          },
        },
        props: {
          decorations(state) {
            return key.getState(state)?.decorations;
          },
        },
      }),
    ];
  },
});

export function getSuggestionItems(view: EditorView): SuggestionRange[] {
  return key.getState(view.state)?.items ?? [];
}

export function setSuggestions(view: EditorView, items: SuggestionRange[]): void {
  view.dispatch(view.state.tr.setMeta(key, items));
}

export function clearSuggestions(view: EditorView): void {
  view.dispatch(view.state.tr.setMeta(key, { clear: true }));
}

export function removeSuggestion(view: EditorView, id: string, action: "accept" | "reject"): void {
  view.dispatch(view.state.tr.setMeta(key, action === "accept" ? { accept: id } : { reject: id }));
}

export function getSuggestionRect(
  view: EditorView,
  id?: string,
  origin?: DOMRect,
): { top: number; left: number; width: number; height: number } | null {
  const items = getSuggestionItems(view);
  const item = (id ? items.find((entry) => entry.id === id) : items[0]) ?? null;
  if (!item) {
    return null;
  }

  const { from, to } = resolveRange(view.state.doc.content.size, item);
  let start: { top: number; left: number; bottom: number; right: number };
  let end: { top: number; left: number; bottom: number; right: number };
  try {
    start = view.coordsAtPos(from);
    end = view.coordsAtPos(Math.max(from, to));
  } catch {
    return null;
  }

  const box = origin ?? view.dom.getBoundingClientRect();
  return {
    top: Math.min(start.top, end.top) - box.top,
    left: Math.min(start.left, end.left) - box.left,
    width: Math.max(8, Math.abs(end.right - start.left)),
    height: Math.max(8, Math.abs(end.bottom - start.top)),
  };
}
