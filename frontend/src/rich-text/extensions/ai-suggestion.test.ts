import { describe, expect, it } from "vitest";
import { clampPos, normalizeSuggestionItems, resolveRange } from "./ai-suggestion";

describe("AI suggestion ranges", () => {
  it("normalizes camelCase and PascalCase payloads", () => {
    const items = normalizeSuggestionItems([
      { Id: "a", Kind: "Replace", From: 2, To: 8, InsertText: "next" },
      { id: "b", kind: "insert", from: 1, insertText: "hi" },
    ]);

    expect(items).toEqual([
      { id: "a", kind: "replace", from: 2, to: 8, insertText: "next", deleteText: undefined },
      { id: "b", kind: "insert", from: 1, to: 0, insertText: "hi", deleteText: undefined },
    ]);
  });

  it("treats a zero to-position as the document end for replace", () => {
    expect(resolveRange(42, { id: "a", kind: "replace", from: 1, to: 0 })).toEqual({ from: 1, to: 42 });
    expect(clampPos(99, 1, 10)).toBe(10);
  });

  it("treats a zero to-position as the document end for replace", () => {
    expect(resolveRange(42, { id: "a", kind: "replace", from: 1, to: 0 })).toEqual({ from: 1, to: 42 });
    expect(clampPos(99, 1, 10)).toBe(10);
  });

  it("keeps an insert with no to-position at the insert site", () => {
    expect(resolveRange(42, { id: "b", kind: "insert", from: 7, to: 0 })).toEqual({ from: 7, to: 7 });
  });
});
