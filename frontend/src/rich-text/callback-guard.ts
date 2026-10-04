const detachedTokens = new Set<string>();

export function markEditorCallbackDetached(token: string | null | undefined): void {
  if (token) {
    detachedTokens.add(token);
  }
}

export function canInvokeEditorCallback(token: string | null | undefined): boolean {
  return !!token && !detachedTokens.has(token);
}
