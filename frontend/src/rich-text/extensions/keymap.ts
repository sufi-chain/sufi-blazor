import { Extension } from "@tiptap/core";

export const SufiKeymap = Extension.create({
  name: "sufiKeymap",
  addKeyboardShortcuts() {
    return {
      "Mod-s": () => {
        this.editor.emit("sufiShortcut" as any, { name: "save" });
        return true;
      },
      "Mod-p": () => {
        this.editor.emit("sufiShortcut" as any, { name: "preview" });
        return true;
      },
    };
  },
});
