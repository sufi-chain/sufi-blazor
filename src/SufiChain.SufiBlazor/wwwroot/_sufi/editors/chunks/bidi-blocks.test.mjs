import assert from "node:assert/strict";
import test from "node:test";
import { applyPerBlockDirectionHtml } from "./bidi-blocks.js";

test("MixedFaEn_Paragraph_GetsDirAutoAndBdiAroundLatin", () => {
  const html = "<p>برای فعال‌سازی SufiAI Hooshvare به Settings بروید (v11.3).</p>";
  const result = applyPerBlockDirectionHtml(html);
  assert.match(result, /<p dir="auto">/);
  assert.match(result, /<bdi>SufiAI Hooshvare<\/bdi>/);
  assert.match(result, /<bdi>Settings<\/bdi>/);
  assert.match(result, /<bdi>v11\.3<\/bdi>/);
});

test("MixedFaEn_ListItems_EachGetDirAuto", () => {
  const html = "<ul><li>اول SufiAI</li><li>دوم Settings</li></ul>";
  const result = applyPerBlockDirectionHtml(html);
  assert.match(result, /<ul dir="auto">/);
  assert.match(result, /<li dir="auto">اول <bdi>SufiAI<\/bdi><\/li>/);
  assert.match(result, /<li dir="auto">دوم <bdi>Settings<\/bdi><\/li>/);
});

test("PureEn_Unchanged", () => {
  const html = "<p>Open Settings and save.</p>";
  const result = applyPerBlockDirectionHtml(html);
  assert.equal(result.includes("<bdi>"), false);
  assert.match(result, /<p dir="auto">Open Settings and save\.<\/p>/);
});

test("PureFa_Unchanged", () => {
  const html = "<p>برای فعال‌سازی به تنظیمات بروید.</p>";
  const result = applyPerBlockDirectionHtml(html);
  assert.equal(result.includes("<bdi>"), false);
  assert.match(result, /<p dir="auto">/);
});

test("CodeBlock_NotWrapped", () => {
  const html = "<pre><code>SufiAI Settings v11.3</code></pre>";
  const result = applyPerBlockDirectionHtml(html);
  assert.equal(result.includes("<bdi>"), false);
  assert.equal(result, html);
});

test("Link_NotSplit", () => {
  const html = '<p>برای <a href="https://example.com">SufiAI Settings</a> بروید.</p>';
  const result = applyPerBlockDirectionHtml(html);
  assert.match(result, /<a href="https:\/\/example.com">SufiAI Settings<\/a>/);
  assert.equal(result.includes("<bdi>"), false);
});
