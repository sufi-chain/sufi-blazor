import { Extension } from "@tiptap/core";
import { Plugin, PluginKey } from "@tiptap/pm/state";

export interface PasteCleanupOptions {
  stripAllFormatting?: boolean;
  cleanWordHtml?: boolean;
  removeInlineStyles?: boolean;
  removeCssClasses?: boolean;
}

function cleanHtml(html: string, options: PasteCleanupOptions): string {
  if (options.stripAllFormatting) {
    return html.replace(/<[^>]+>/g, " ");
  }

  let result = html;
  if (options.cleanWordHtml) {
    result = result
      .replace(/<!--\[if[\s\S]*?<!\[endif\]-->/gi, "")
      .replace(/\s(class|style)="Mso[^"]*"/gi, "");
  }
  if (options.removeInlineStyles) {
    result = result.replace(/\sstyle="[^"]*"/gi, "");
  }
  if (options.removeCssClasses) {
    result = result.replace(/\sclass="[^"]*"/gi, "");
  }
  result = result.replace(/<script[\s\S]*?<\/script>/gi, "");
  result = result.replace(/\son\w+="[^"]*"/gi, "");
  return result;
}

export const SufiPasteCleanup = Extension.create<PasteCleanupOptions>({
  name: "sufiPasteCleanup",
  addOptions() {
    return {
      stripAllFormatting: false,
      cleanWordHtml: true,
      removeInlineStyles: false,
      removeCssClasses: false,
    };
  },
  addProseMirrorPlugins() {
    const options = this.options;
    return [
      new Plugin({
        key: new PluginKey("sufiPasteCleanup"),
        props: {
          transformPastedHTML(html: string) {
            return cleanHtml(html, options);
          },
        },
      }),
    ];
  },
});
