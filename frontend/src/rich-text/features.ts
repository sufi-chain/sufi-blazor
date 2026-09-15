/** Mirrors SufiChain.SufiBlazor.Contracts.Editors.SbEditorFeatures. */
export const EditorFeature = {
  None: 0,
  BubbleMenu: 1,
  FloatingMenu: 2,
  SlashCommands: 4,
  DragHandle: 8,
  Tables: 16,
  TaskLists: 32,
  Highlight: 64,
  TextColor: 128,
  Mermaid: 256,
  Callouts: 512,
  Emoji: 1024,
  Mentions: 2048,
  FindReplace: 4096,
  Ai: 8192,
  Default: 1 | 2 | 4 | 16 | 32 | 64 | 128 | 256 | 512 | 4096 | 8192,
} as const;

export function hasFeature(features: number | undefined, flag: number): boolean {
  return ((features ?? EditorFeature.Default) & flag) === flag;
}
