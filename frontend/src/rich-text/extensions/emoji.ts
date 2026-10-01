import { Extension, InputRule } from "@tiptap/core";

/** Shortcodes and emoticons inserted as characters. Registered when SbEditorFeatures.Emoji is set. */
const SHORTCODES: Record<string, string> = {
  smile: "😄",
  grin: "😁",
  joy: "😂",
  wink: "😉",
  heart: "❤️",
  thumbsup: "👍",
  thumbsdown: "👎",
  check: "✅",
  warning: "⚠️",
  fire: "🔥",
  wave: "👋",
  thinking: "🤔",
  tada: "🎉",
  rocket: "🚀",
  bulb: "💡",
  memo: "📝",
  eyes: "👀",
  question: "❓",
};

const EMOTICONS: Record<string, string> = {
  ":-)": "🙂",
  ":)": "🙂",
  ":-(": "🙁",
  ":(": "🙁",
  ":-D": "😃",
  ":D": "😃",
  ";-)": "😉",
  ";)": "😉",
};

export function emojiForToken(token: string): string | null {
  const key = token.trim();
  if (key.startsWith(":") && key.endsWith(":") && key.length > 2) {
    return SHORTCODES[key.slice(1, -1).toLowerCase()] ?? null;
  }
  return EMOTICONS[key] ?? null;
}

function replaceToken(name: string, emoji: string, pattern: RegExp): InputRule {
  return new InputRule({
    find: pattern,
    handler: ({ range, match, commands }) => {
      const prefix = match[1]?.length ?? 0;
      commands.insertContentAt({ from: range.from + prefix, to: range.to }, emoji);
    },
  });
}

export const SufiEmoji = Extension.create({
  name: "sufiEmoji",
  addInputRules() {
    const rules = Object.entries(SHORTCODES).map(([name, emoji]) =>
      replaceToken(name, emoji, new RegExp(`(^|\\s):${name}:$`, "i")),
    );
    for (const [token, emoji] of Object.entries(EMOTICONS)) {
      const escaped = token.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
      rules.push(replaceToken(token, emoji, new RegExp(`(^|\\s)${escaped}$`)));
    }
    return rules;
  },
});
