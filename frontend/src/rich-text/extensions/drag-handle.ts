import { Extension } from "@tiptap/core";
import { NodeSelection, Plugin, PluginKey } from "@tiptap/pm/state";
import type { EditorView } from "@tiptap/pm/view";

function topLevelBlockPos(view: EditorView, pos: number): number | null {
  const max = view.state.doc.content.size;
  const clamped = Math.max(0, Math.min(pos, max));
  const $pos = view.state.doc.resolve(clamped);
  if ($pos.depth < 1) {
    return null;
  }
  return $pos.before(1);
}

/** Block drag grip. Registered when SbEditorFeatures.DragHandle is set. */
export const SufiDragHandle = Extension.create({
  name: "sufiDragHandle",
  addProseMirrorPlugins() {
    return [
      new Plugin({
        key: new PluginKey("sufiDragHandle"),
        view(view) {
          const host = view.dom.parentElement ?? view.dom;
          host.classList.add("sb-editor-drag-host");

          const handle = document.createElement("button");
          handle.type = "button";
          handle.className = "sb-editor-drag-handle";
          handle.draggable = true;
          handle.tabIndex = -1;
          handle.setAttribute("aria-hidden", "true");
          handle.textContent = "⋮⋮";
          host.appendChild(handle);

          let blockPos: number | null = null;

          const place = (pos: number | null) => {
            blockPos = pos;
            if (pos == null || !view.editable) {
              handle.hidden = true;
              return;
            }

            const node = view.state.doc.nodeAt(pos);
            if (!node) {
              handle.hidden = true;
              return;
            }

            const coords = view.coordsAtPos(Math.min(pos + 1, view.state.doc.content.size));
            const origin = host.getBoundingClientRect();
            handle.hidden = false;
            handle.style.top = `${coords.top - origin.top}px`;
          };

          const onMove = (event: MouseEvent) => {
            if (!view.editable || event.target === handle) {
              return;
            }
            const found = view.posAtCoords({ left: event.clientX, top: event.clientY });
            place(found ? topLevelBlockPos(view, found.pos) : null);
          };

          const onLeave = (event: MouseEvent) => {
            if (event.relatedTarget instanceof Node && host.contains(event.relatedTarget)) {
              return;
            }
            place(null);
          };

          const onDragStart = (event: DragEvent) => {
            if (blockPos == null) {
              event.preventDefault();
              return;
            }
            const node = view.state.doc.nodeAt(blockPos);
            if (!node || !event.dataTransfer) {
              event.preventDefault();
              return;
            }

            const slice = view.state.doc.slice(blockPos, blockPos + node.nodeSize);
            view.dispatch(view.state.tr.setSelection(NodeSelection.create(view.state.doc, blockPos)));
            view.dragging = { slice, move: true };
            event.dataTransfer.effectAllowed = "move";
            event.dataTransfer.setData("text/plain", node.textContent);
          };

          host.addEventListener("mousemove", onMove);
          host.addEventListener("mouseleave", onLeave);
          handle.addEventListener("dragstart", onDragStart);
          place(null);

          return {
            update(next) {
              if (!next.editable) {
                place(null);
              }
            },
            destroy() {
              host.removeEventListener("mousemove", onMove);
              host.removeEventListener("mouseleave", onLeave);
              handle.removeEventListener("dragstart", onDragStart);
              handle.remove();
              host.classList.remove("sb-editor-drag-host");
            },
          };
        },
      }),
    ];
  },
});
