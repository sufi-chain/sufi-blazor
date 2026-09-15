import { describe, expect, it } from "vitest";
import { isAllowedUrl } from "./extensions/link";

describe("SufiLink policy", () => {
  it("allows http(s), mailto, tel, and relative urls", () => {
    expect(isAllowedUrl("https://example.com")).toBe(true);
    expect(isAllowedUrl("http://example.com")).toBe(true);
    expect(isAllowedUrl("mailto:hi@example.com")).toBe(true);
    expect(isAllowedUrl("/files/a.png")).toBe(true);
    expect(isAllowedUrl("#section")).toBe(true);
  });

  it("rejects javascript and data urls", () => {
    expect(isAllowedUrl("javascript:alert(1)")).toBe(false);
    expect(isAllowedUrl("data:text/html,hi")).toBe(false);
  });
});
