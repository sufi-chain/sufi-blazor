import { MarkdownManager } from "@tiptap/markdown";
import { renderToHTMLString } from "@tiptap/static-renderer/pm/html-string";
import hljs from "highlight.js/lib/core";
import javascript from "highlight.js/lib/languages/javascript";
import typescript from "highlight.js/lib/languages/typescript";
import xml from "highlight.js/lib/languages/xml";
import json from "highlight.js/lib/languages/json";
import css from "highlight.js/lib/languages/css";
import markdown from "highlight.js/lib/languages/markdown";
import csharp from "highlight.js/lib/languages/csharp";
import bash from "highlight.js/lib/languages/bash";
import { createDocumentExtensions } from "../rich-text/schema";
import "./viewer.css";

hljs.registerLanguage("javascript", javascript);
hljs.registerLanguage("typescript", typescript);
hljs.registerLanguage("xml", xml);
hljs.registerLanguage("html", xml);
hljs.registerLanguage("json", json);
hljs.registerLanguage("css", css);
hljs.registerLanguage("markdown", markdown);
hljs.registerLanguage("csharp", csharp);
hljs.registerLanguage("bash", bash);

const extensions = createDocumentExtensions();
const markdownManager = new MarkdownManager({
  extensions,
  markedOptions: {
    gfm: true,
    breaks: false,
  },
});

export async function renderDocument(
  element: HTMLElement,
  content: string,
  format: string,
  options?: { enableHighlight?: boolean; enableMermaid?: boolean; direction?: string },
): Promise<void> {
  const fmt = (format ?? "markdown").toLowerCase();
  const direction = options?.direction === "rtl" ? "rtl" : "ltr";
  let html: string;

  if (fmt === "html") {
    html = content ?? "";
  } else {
    const json = fmt === "json" ? JSON.parse(content || "{\"type\":\"doc\",\"content\":[]}") : markdownManager.parse(content ?? "");
    html = renderToHTMLString({
      extensions,
      content: json,
      staticEditorOptions: { textDirection: direction },
    });
  }

  element.innerHTML = html;

  if (options?.enableHighlight !== false) {
    element.querySelectorAll("pre code").forEach((block) => {
      hljs.highlightElement(block as HTMLElement);
    });
  }

  if (options?.enableMermaid !== false && element.querySelector("code.language-mermaid, .language-mermaid")) {
    await enhanceMermaid(element);
  }
}

async function enhanceMermaid(element: HTMLElement): Promise<void> {
  const blocks = Array.from(element.querySelectorAll("code.language-mermaid, .language-mermaid"));
  if (blocks.length === 0) {
    return;
  }

  const mermaid = await import("mermaid");
  mermaid.default.initialize({ startOnLoad: false, securityLevel: "strict" });
  let index = 0;
  for (const block of blocks) {
    const source = block.textContent ?? "";
    const host = document.createElement("div");
    host.className = "sb-mermaid";
    const id = `sb-mermaid-${index++}`;
    try {
      const { svg } = await mermaid.default.render(id, source);
      host.innerHTML = svg;
      (block.closest("pre") ?? block).replaceWith(host);
    } catch {
      // Keep the original fenced block when mermaid fails to parse.
    }
  }
}
