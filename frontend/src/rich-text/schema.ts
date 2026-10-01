import type { AnyExtension } from "@tiptap/core";
import StarterKit from "@tiptap/starter-kit";
import Highlight from "@tiptap/extension-highlight";
import TextAlign from "@tiptap/extension-text-align";
import { Color } from "@tiptap/extension-color";
import { TextStyle } from "@tiptap/extension-text-style";
import FontFamily from "@tiptap/extension-font-family";
import Image from "@tiptap/extension-image";
import { TableKit } from "@tiptap/extension-table";
import TaskList from "@tiptap/extension-task-list";
import TaskItem from "@tiptap/extension-task-item";
import { Markdown } from "@tiptap/markdown";
import { SufiCallout } from "./extensions/callout";
import { SufiDragHandle } from "./extensions/drag-handle";
import { SufiEmoji } from "./extensions/emoji";
import { SufiLink } from "./extensions/link";
import { SufiMention } from "./extensions/mention";
import { EditorFeature, hasFeature } from "./features";

export interface DocumentExtensionOptions {
  features?: number;
}

export function createDocumentExtensions(options: DocumentExtensionOptions = {}): AnyExtension[] {
  const features = options.features ?? EditorFeature.Default;
  const extensions: AnyExtension[] = [
    StarterKit.configure({
      link: false,
    }),
    Markdown.configure({
      markedOptions: {
        gfm: true,
        breaks: false,
        pedantic: false,
      },
    }),
    TextStyle,
    FontFamily,
    TextAlign.configure({ types: ["heading", "paragraph"] }),
    Image.configure({ inline: false, allowBase64: true }),
    SufiLink,
  ];

  if (hasFeature(features, EditorFeature.Highlight)) {
    extensions.push(Highlight);
  }

  if (hasFeature(features, EditorFeature.TextColor)) {
    extensions.push(Color);
  }

  if (hasFeature(features, EditorFeature.Tables)) {
    extensions.push(TableKit.configure({ table: { resizable: false } }));
  }

  if (hasFeature(features, EditorFeature.TaskLists)) {
    extensions.push(TaskList, TaskItem.configure({ nested: true }));
  }

  if (hasFeature(features, EditorFeature.Callouts)) {
    extensions.push(SufiCallout);
  }

  if (hasFeature(features, EditorFeature.DragHandle)) {
    extensions.push(SufiDragHandle);
  }

  if (hasFeature(features, EditorFeature.Emoji)) {
    extensions.push(SufiEmoji);
  }

  if (hasFeature(features, EditorFeature.Mentions)) {
    extensions.push(SufiMention);
  }

  return extensions;
}
