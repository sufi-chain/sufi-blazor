import Link from "@tiptap/extension-link";

const ALLOWED = new Set(["http:", "https:", "mailto:", "tel:"]);

export function isAllowedUrl(href: string): boolean {
  const value = (href ?? "").trim();
  if (!value) {
    return false;
  }
  if (value.startsWith("/") || value.startsWith("#") || value.startsWith("./")) {
    return true;
  }
  try {
    const url = new URL(value, "https://sufi.local");
    if (value.toLowerCase().startsWith("javascript:") || value.toLowerCase().startsWith("data:")) {
      return false;
    }
    return ALLOWED.has(url.protocol);
  } catch {
    return false;
  }
}

export const SufiLink = Link.extend({
  name: "link",
}).configure({
  openOnClick: false,
  autolink: true,
  defaultProtocol: "https",
  protocols: ["http", "https", "mailto", "tel"],
  HTMLAttributes: {
    rel: "noopener noreferrer",
  },
  validate: isAllowedUrl,
  isAllowedUri: (url) => isAllowedUrl(url ?? ""),
  shouldAutoLink: isAllowedUrl,
});
