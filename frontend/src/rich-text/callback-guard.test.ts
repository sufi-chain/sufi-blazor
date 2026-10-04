import { describe, expect, it } from "vitest";
import { canInvokeEditorCallback, markEditorCallbackDetached } from "./callback-guard";

describe("editor callback guard", () => {
  it("stops .NET callbacks after the editor reference is detached", () => {
    const token = `kb-editor-${Math.random().toString(16).slice(2)}`;
    expect(canInvokeEditorCallback(token)).toBe(true);

    markEditorCallbackDetached(token);

    expect(canInvokeEditorCallback(token)).toBe(false);
    expect(canInvokeEditorCallback("")).toBe(false);
    expect(canInvokeEditorCallback(undefined)).toBe(false);
  });
});
