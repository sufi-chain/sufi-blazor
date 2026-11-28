// Per-block direction for mixed fa/en document HTML.
// Walks tags and text nodes. Does not split a tag with a pattern.

const blockTags = new Set(["p", "li", "h1", "h2", "h3", "h4", "h5", "h6", "blockquote", "td", "th", "ul", "ol"]);
const skipTags = new Set(["code", "pre", "a", "bdi", "script", "style"]);
const voidTags = new Set(["area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "source", "track", "wbr"]);
const rtlChar = /[\u0590-\u05FF\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF]/;
const latinRun = /[A-Za-z0-9](?:[A-Za-z0-9:/_~?#&=%@.+-]| (?=[A-Za-z0-9]))*/g;

export function applyPerBlockDirection(root) {
  if (!root) {
    return;
  }

  root.innerHTML = applyPerBlockDirectionHtml(root.innerHTML);
}

export function applyPerBlockDirectionHtml(html) {
  const root = parseHtml(html || "");
  walk(root, false);
  return serialize(root);
}

function parseHtml(html) {
  const root = { tag: "#root", attrs: "", children: [], selfClosing: false };
  const stack = [root];
  let i = 0;
  while (i < html.length) {
    if (html[i] !== "<") {
      const next = html.indexOf("<", i);
      const end = next === -1 ? html.length : next;
      stack[stack.length - 1].children.push({ text: html.slice(i, end) });
      i = end;
      continue;
    }

    const close = html.indexOf(">", i);
    if (close === -1) {
      stack[stack.length - 1].children.push({ text: html.slice(i) });
      break;
    }

    const raw = html.slice(i, close + 1);
    i = close + 1;
    if (raw.startsWith("<!--") || raw.startsWith("<!") || raw.startsWith("<?")) {
      stack[stack.length - 1].children.push({ text: raw });
      continue;
    }

    if (raw.startsWith("</")) {
      const name = tagName(raw);
      for (let n = stack.length - 1; n > 0; n--) {
        if (stack[n].tag === name) {
          stack.length = n;
          break;
        }
      }
      continue;
    }

    const name = tagName(raw);
    const selfClosing = raw.endsWith("/>") || voidTags.has(name);
    const node = {
      tag: name,
      attrs: raw.slice(raw.indexOf(name) + name.length, raw.endsWith("/>") ? raw.length - 2 : raw.length - 1),
      children: [],
      selfClosing
    };
    stack[stack.length - 1].children.push(node);
    if (!selfClosing) {
      stack.push(node);
    }
  }

  return root;
}

function tagName(raw) {
  const match = /^<\/?\s*([A-Za-z0-9]+)/.exec(raw);
  return match ? match[1].toLowerCase() : "";
}

function walk(node, skipped) {
  if (node.text !== undefined) {
    return;
  }

  const nowSkipped = skipped || skipTags.has(node.tag);
  if (blockTags.has(node.tag) && !hasDir(node.attrs)) {
    node.attrs = ` dir="auto"${node.attrs}`;
  }

  const rtl = !nowSkipped && blockTags.has(node.tag) && firstStrongIsRtl(node);
  for (const child of node.children) {
    if (child.text !== undefined) {
      if (rtl && !nowSkipped) {
        child.text = isolateLatin(child.text);
      }
    } else {
      walk(child, nowSkipped);
    }
  }
}

function hasDir(attrs) {
  return /(?:^|\s)dir\s*=/i.test(attrs || "");
}

function firstStrongIsRtl(node) {
  const text = collectText(node);
  for (const ch of text) {
    if (rtlChar.test(ch)) {
      return true;
    }

    if (/[A-Za-z]/.test(ch)) {
      return false;
    }
  }

  return false;
}

function collectText(node) {
  if (node.text !== undefined) {
    return node.text;
  }

  let value = "";
  for (const child of node.children) {
    value += collectText(child);
  }

  return value;
}

function isolateLatin(text) {
  if (!rtlChar.test(text) || !/[A-Za-z0-9]/.test(text)) {
    return text;
  }

  return text.replace(latinRun, (run) => `<bdi>${run}</bdi>`);
}

function serialize(node) {
  if (node.text !== undefined) {
    return node.text;
  }

  let html = "";
  for (const child of node.children) {
    if (child.text !== undefined) {
      html += child.text;
      continue;
    }

    const slash = child.selfClosing ? " /" : "";
    html += `<${child.tag}${child.attrs}${slash}>`;
    if (!child.selfClosing) {
      html += serialize(child);
      html += `</${child.tag}>`;
    }
  }

  return html;
}
