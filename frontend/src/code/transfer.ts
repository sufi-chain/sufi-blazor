/** HTML auto-close stays on unless the host turns it off (the CMS HTML tab does). */
export function htmlLanguageConfig(autoCloseTags?: boolean): { autoCloseTags: boolean } {
  return { autoCloseTags: autoCloseTags !== false };
}

/** Chunked setValue is exact: a reset replaces, every later chunk appends, including at an 8,000-char boundary. */
export function applyValueChunk(current: string, chunk: string, reset: boolean): string {
  const next = chunk ?? "";
  return reset ? next : (current ?? "") + next;
}
