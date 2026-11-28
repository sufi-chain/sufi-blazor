/** HTML auto-close stays on unless the host turns it off (the CMS HTML tab does). */
export function htmlLanguageConfig(autoCloseTags?: boolean): { autoCloseTags: boolean } {
  return { autoCloseTags: autoCloseTags !== false };
}

/** Chunked setValue is exact: a reset replaces, every later chunk appends, including at an 8,000-char boundary. */
export function applyValueChunk(current: string, chunk: string, reset: boolean): string {
  const next = chunk ?? "";
  return reset ? next : (current ?? "") + next;
}

export interface TextBaseline {
  length: number;
  hash: number;
}

/** FNV-1a over UTF-16 code units. Compared with length so a keystroke never has to ship the document. */
export function hashText(text: string): number {
  let hash = 2166136261;
  const value = text ?? "";
  for (let i = 0; i < value.length; i++) {
    hash ^= value.charCodeAt(i);
    hash = Math.imul(hash, 16777619);
  }
  return hash >>> 0;
}

export function baselineOf(text: string): TextBaseline {
  const value = text ?? "";
  return { length: value.length, hash: hashText(value) };
}

export function isTextDirty(baseline: TextBaseline | null | undefined, text: string): boolean {
  const value = text ?? "";
  if (!baseline) {
    return value.length > 0;
  }
  return baseline.length !== value.length || baseline.hash !== hashText(value);
}

export interface DirtyNotice {
  method: "OnEditorDirtyChanged";
  dirty: boolean;
  length: number;
  documentSent: false;
}

/**
 * Keystrokes only schedule a dirty flag. The document stays in the editor until the host reads it.
 * A burst collapses to one notice.
 */
export function planDirtyNotices(
  baseline: TextBaseline | null,
  keystrokes: string[],
): DirtyNotice[] {
  if (keystrokes.length === 0) {
    return [];
  }
  const latest = keystrokes[keystrokes.length - 1] ?? "";
  return [{
    method: "OnEditorDirtyChanged",
    dirty: isTextDirty(baseline, latest),
    length: latest.length,
    documentSent: false,
  }];
}
