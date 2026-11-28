import { describe, expect, it } from "vitest";
import { applyValueChunk, baselineOf, htmlLanguageConfig, planDirtyNotices } from "./transfer";
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

  it("notify_doesNotSendDocumentOnKeystroke", () => {
    const baseline = baselineOf("body { color: black; }");
    const next = baselineOf("body { color: black; }").length + "x".length;
    const notices = planDirtyNotices(baseline, ["body { color: black; }x"]);
    expect(notices).toHaveLength(1);
    expect(notices[0]?.documentSent).toBe(false);
    expect(notices[0]?.method).toBe("OnEditorDirtyChanged");
    expect(JSON.stringify(notices[0])).not.toContain("color: black");
    expect(notices[0]?.length).toBe(next);
  });

  it("notify_debouncesDirtyFlag", () => {
    const baseline = baselineOf("");
    const notices = planDirtyNotices(baseline, ["a", "ab", "abc"]);
    expect(notices).toHaveLength(1);
    expect(notices[0]?.dirty).toBe(true);
    expect(notices[0]?.length).toBe(3);
  });

  it("notify_insert300CharsIntoSmallDoc_FlagsDirtyOnce", () => {
    const start = "p { margin: 0; }";
    const inserted = start + "x".repeat(300);
    const notices = planDirtyNotices(baselineOf(start), [inserted]);
    expect(notices).toEqual([{
      method: "OnEditorDirtyChanged",
      dirty: true,
      length: inserted.length,
      documentSent: false,
    }]);
    expect(inserted.length).toBeLessThanOrEqual(8000);
  });
});
