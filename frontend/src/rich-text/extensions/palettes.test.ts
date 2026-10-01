import { describe, expect, it } from "vitest";
import { emojiForToken } from "./emoji";
import { mentionLabel } from "./mention";
import { EditorFeature } from "../features";

describe("editor palettes", () => {
  it("keeps drag handle, emoji, and mentions out of the default mask", () => {
    expect(EditorFeature.Default & EditorFeature.DragHandle).toBe(0);
    expect(EditorFeature.Default & EditorFeature.Emoji).toBe(0);
    expect(EditorFeature.Default & EditorFeature.Mentions).toBe(0);
  });

  it("maps emoji shortcodes and emoticons", () => {
    expect(emojiForToken(":smile:")).toBe("😄");
    expect(emojiForToken(":Smile:")).toBe("😄");
    expect(emojiForToken(":)")).toBe("🙂");
    expect(emojiForToken(":unknown:")).toBeNull();
  });

  it("prefers a mention label over its id", () => {
    expect(mentionLabel("Ada", "ada")).toBe("Ada");
    expect(mentionLabel("  ", "ada")).toBe("ada");
    expect(mentionLabel(null, null)).toBe("");
  });
});
