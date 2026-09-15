import { describe, expect, it } from "vitest";
import { normalizeCalloutKind } from "./extensions/callout";
import { toContentType } from "./content";

describe("SufiCallout markdown kinds", () => {
  it("keeps first-class kinds", () => {
    expect(normalizeCalloutKind("note")).toBe("note");
    expect(normalizeCalloutKind("tip")).toBe("tip");
    expect(normalizeCalloutKind("warn")).toBe("warn");
    expect(normalizeCalloutKind("alert")).toBe("alert");
  });

  it("maps common admonition aliases from the Tiptap guide", () => {
    expect(normalizeCalloutKind("warning")).toBe("warn");
    expect(normalizeCalloutKind("danger")).toBe("alert");
    expect(normalizeCalloutKind("att")).toBe("alert");
  });

  it("falls back to note for unknown types", () => {
    expect(normalizeCalloutKind("unknown")).toBe("note");
    expect(normalizeCalloutKind(undefined)).toBe("note");
  });
});

describe("Markdown contentType", () => {
  it("maps formats to the Tiptap contentType option", () => {
    expect(toContentType("markdown")).toBe("markdown");
    expect(toContentType("html")).toBe("html");
    expect(toContentType("json")).toBe("json");
    expect(toContentType(undefined)).toBe("html");
  });
});
