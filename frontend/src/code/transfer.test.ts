import { describe, expect, it } from "vitest";
import { applyValueChunk, htmlLanguageConfig } from "./transfer";
import { programmaticContentOptions } from "../rich-text/content";

describe("CMS HTML editor transfer", () => {
  it("turns auto-close off only when the host asks", () => {
    expect(htmlLanguageConfig(false).autoCloseTags).toBe(false);
    expect(htmlLanguageConfig(undefined).autoCloseTags).toBe(true);
    expect(htmlLanguageConfig(true).autoCloseTags).toBe(true);
  });

  it("setValueChunk is exact across an 8000-character boundary", () => {
    const head = "ن".repeat(8000);
    const tail = " <strong>پررنگ</strong> و <a href=\"/x\">پیوند</a>";
    let value = applyValueChunk("", head, true);
    value = applyValueChunk(value, tail, false);
    expect(value).toBe(head + tail);
    expect(value.startsWith("ن".repeat(8000))).toBe(true);
    expect(value.endsWith("پیوند</a>")).toBe(true);
  });

  it("a programmatic rich-text set does not emit an update", () => {
    expect(programmaticContentOptions("html")).toEqual({ contentType: "html", emitUpdate: false });
  });
});
