import { Node, mergeAttributes } from "@tiptap/core";

const CALLOUT_KIND_ALIASES: Record<string, string> = {
  note: "note",
  tip: "tip",
  warn: "warn",
  warning: "warn",
  alert: "alert",
  danger: "alert",
  att: "alert",
};

export function normalizeCalloutKind(value: string | undefined): string {
  const key = (value ?? "note").trim().toLowerCase();
  return CALLOUT_KIND_ALIASES[key] ?? "note";
}

export const SufiCallout = Node.create({
  name: "sufiCallout",
  group: "block",
  content: "block+",
  defining: true,
  addAttributes() {
    return {
      kind: {
        default: "note",
        parseHTML: (element) =>
          normalizeCalloutKind(
            element.getAttribute("data-sb-callout") ?? element.getAttribute("data-type") ?? undefined,
          ),
        renderHTML: (attributes) => ({ "data-sb-callout": normalizeCalloutKind(attributes.kind) }),
      },
    };
  },
  parseHTML() {
    return [{ tag: "div[data-sb-callout]" }, { tag: "div[data-admonition]" }];
  },
  renderHTML({ HTMLAttributes }) {
    const kind = normalizeCalloutKind(
      HTMLAttributes["data-sb-callout"] ?? HTMLAttributes.kind ?? "note",
    );
    return [
      "div",
      mergeAttributes(HTMLAttributes, {
        class: `sb-callout sb-callout--${kind}`,
        "data-sb-callout": kind,
      }),
      0,
    ];
  },
  markdownTokenizer: {
    name: "sufiCallout",
    level: "block",
    start: (src) => src.indexOf(":::"),
    tokenize: (src, _tokens, lexer) => {
      const match = /^:::(\w+)\n([\s\S]*?)\n:::\n?/.exec(src);
      if (!match) {
        return undefined;
      }

      return {
        type: "sufiCallout",
        raw: match[0],
        calloutKind: match[1],
        text: match[2],
        tokens: lexer.blockTokens(match[2]),
      };
    },
  },
  parseMarkdown: (token, helpers) => ({
    type: "sufiCallout",
    attrs: { kind: normalizeCalloutKind(token.calloutKind || token.admonitionType || "note") },
    content: helpers.parseChildren(token.tokens || []),
  }),
  renderMarkdown: (node, helpers) => {
    const kind = normalizeCalloutKind(node.attrs?.kind);
    const content = helpers.renderChildren(node.content || []);
    return `:::${kind}\n${content}:::\n\n`;
  },
});
