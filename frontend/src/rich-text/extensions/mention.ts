import { InputRule, mergeAttributes, Node } from "@tiptap/core";

const MENTION_PATTERN = /(?:^|\s)@([A-Za-z0-9_.-]{1,64})\s$/;
const MENTION_MARKDOWN = /^@\[([^\]]+)\]\(([^)\s]+)\)/;

export function mentionLabel(label: string | null | undefined, id: string | null | undefined): string {
  const text = (label ?? id ?? "").trim();
  return text.length === 0 ? "" : text;
}

/** Inline mention atom. Registered when SbEditorFeatures.Mentions is set. */
export const SufiMention = Node.create({
  name: "mention",
  group: "inline",
  inline: true,
  atom: true,
  selectable: true,
  addAttributes() {
    return {
      id: { default: null },
      label: { default: null },
    };
  },
  parseHTML() {
    return [{ tag: "span[data-type='mention']" }];
  },
  renderHTML({ node, HTMLAttributes }) {
    const label = mentionLabel(node.attrs.label, node.attrs.id);
    return [
      "span",
      mergeAttributes(HTMLAttributes, {
        class: "sb-mention",
        "data-type": "mention",
        "data-id": node.attrs.id,
        "data-label": label,
      }),
      label.length === 0 ? "@" : `@${label}`,
    ];
  },
  markdownTokenizer: {
    name: "mention",
    level: "inline",
    start: (src: string) => src.indexOf("@["),
    tokenize: (src: string) => {
      const match = MENTION_MARKDOWN.exec(src);
      if (!match) {
        return undefined;
      }
      return {
        type: "mention",
        raw: match[0],
        mentionLabel: match[1],
        mentionId: match[2],
      };
    },
  },
  parseMarkdown: (token) => ({
    type: "mention",
    attrs: {
      id: token.mentionId ?? token.mentionLabel,
      label: token.mentionLabel ?? token.mentionId,
    },
  }),
  renderMarkdown: (node) => {
    const label = mentionLabel(node.attrs?.label, node.attrs?.id);
    const id = String(node.attrs?.id ?? label);
    return `@[${label}](${id})`;
  },
  addInputRules() {
    return [
      new InputRule({
        find: MENTION_PATTERN,
        handler: ({ range, match, commands }) => {
          const label = match[1];
          if (!label) {
            return;
          }
          const prefix = match[0].startsWith("@") ? 0 : 1;
          commands.insertContentAt({ from: range.from + prefix, to: range.to }, [
            { type: this.name, attrs: { id: label, label } },
            { type: "text", text: " " },
          ]);
        },
      }),
    ];
  },
});
