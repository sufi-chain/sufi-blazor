/**
 * SufiBlazor Markdown Editor JavaScript Module
 * EasyMDE (CodeMirror 5 + marked.js) with offline mermaid/highlight.js and diff review.
 */

const editors = new Map();
const diffEditors = new Map();
let editorIdCounter = 0;

let easyMdeLoadPromise = null;
let markedLoadPromise = null;
let assetsLoadPromise = null;
let diffAssetsLoadPromise = null;

function getContentBasePath() {
    const scripts = document.querySelectorAll('script[src*="sufiblazor-markdown-editor.js"]');
    if (scripts.length > 0) {
        const src = scripts[0].src;
        return src.substring(0, src.lastIndexOf('/'));
    }
    return '_content/SufiChain.SufiBlazor';
}

function loadScript(src) {
    return new Promise((resolve) => {
        if (document.querySelector(`script[src="${src}"]`)) {
            resolve(true);
            return;
        }
        const script = document.createElement('script');
        script.src = src;
        script.onload = () => resolve(true);
        script.onerror = () => resolve(false);
        document.head.appendChild(script);
    });
}

function loadCss(href) {
    if (!document.querySelector(`link[href="${href}"]`)) {
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = href;
        document.head.appendChild(link);
    }
}

async function ensureEasyMdeLoaded() {
    if (typeof EasyMDE !== 'undefined') {
        return true;
    }
    if (easyMdeLoadPromise) {
        return easyMdeLoadPromise;
    }
    easyMdeLoadPromise = (async () => {
        const base = getContentBasePath();
        loadCss(`${base}/vendor/easymde/easymde.min.css`);
        const loaded = await loadScript(`${base}/vendor/easymde/easymde.min.js`);
        return loaded && typeof EasyMDE !== 'undefined';
    })();
    return easyMdeLoadPromise;
}

async function ensureMarkedLoaded() {
    if (typeof marked !== 'undefined') {
        return true;
    }
    if (markedLoadPromise) {
        return markedLoadPromise;
    }
    markedLoadPromise = (async () => {
        const base = getContentBasePath();
        const loaded = await loadScript(`${base}/vendor/easymde/marked.min.js`);
        return loaded && typeof marked !== 'undefined';
    })();
    return markedLoadPromise;
}

export async function ensureAssets(options = {}) {
    const base = getContentBasePath();
    const tasks = [ensureMarkedLoaded()];

    if (options.enableHighlight) {
        const theme = options.highlightTheme || 'github';
        loadCss(`${base}/vendor/highlight.js/${theme}.min.css`);
        tasks.push(loadScript(`${base}/vendor/highlight.js/highlight.min.js`));
    }

    if (options.enableMermaid) {
        tasks.push(loadScript(`${base}/vendor/mermaid/mermaid.min.js`));
    }

    if (assetsLoadPromise) {
        await assetsLoadPromise;
    }

    assetsLoadPromise = Promise.all(tasks);
    await assetsLoadPromise;

    if (options.enableMermaid && typeof mermaid !== 'undefined' && !window.__sbMermaidInitialized) {
        mermaid.initialize({ startOnLoad: false, theme: 'default' });
        window.__sbMermaidInitialized = true;
    }

    return true;
}

async function ensureDiffAssets() {
    if (typeof CodeMirror !== 'undefined' && CodeMirror.MergeView) {
        return true;
    }
    if (diffAssetsLoadPromise) {
        return diffAssetsLoadPromise;
    }
    diffAssetsLoadPromise = (async () => {
        const base = getContentBasePath();
        loadCss(`${base}/vendor/codemirror/codemirror.css`);
        loadCss(`${base}/vendor/codemirror/merge.css`);
        await loadScript(`${base}/vendor/codemirror/codemirror.js`);
        await loadScript(`${base}/vendor/codemirror/markdown.js`);
        await loadScript(`${base}/vendor/codemirror/xml.js`);
        await loadScript(`${base}/vendor/codemirror/javascript.js`);
        await loadScript(`${base}/vendor/codemirror/css.js`);
        await loadScript(`${base}/vendor/codemirror/htmlmixed.js`);
        await loadScript(`${base}/vendor/codemirror/diff-match-patch.js`);
        if (typeof diff_match_patch !== 'undefined') {
            window.diff_match_patch = diff_match_patch;
        }
        await loadScript(`${base}/vendor/codemirror/merge.js`);
        return typeof CodeMirror !== 'undefined' && !!CodeMirror.MergeView;
    })();
    return diffAssetsLoadPromise;
}

function configureMarked() {
    if (!marked || window.__sbMarkedConfigured) {
        return;
    }

    const renderer = new marked.Renderer();
    const originalCode = renderer.code.bind(renderer);

    renderer.code = (code, infostring) => {
        const language = (infostring || '').trim().toLowerCase();
        if (['note', 'tip', 'warn', 'att', 'alert'].includes(language)) {
            const cssClass = language === 'att' ? 'alert' : language;
            return `<div class="sb-markdown-alert sb-markdown-alert--${cssClass}">${marked.parse(code)}</div>`;
        }
        if (language === 'mermaid') {
            return `<pre class="mermaid-source"><code class="language-mermaid">${escapeHtml(code)}</code></pre>`;
        }
        return originalCode(code, infostring, false);
    };

    marked.use({ renderer, gfm: true, breaks: true });
    window.__sbMarkedConfigured = true;
}

function escapeHtml(text) {
    return text
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;');
}

function buildHighlightFn(enableHighlight, enableMermaid) {
    return (code, language) => {
        const lang = (language || '').trim().toLowerCase();
        if (lang === 'mermaid' && enableMermaid) {
            const id = `sb-mermaid-${Date.now()}-${Math.random().toString(36).slice(2)}`;
            return `<div class="mermaid" id="${id}">${escapeHtml(code)}</div>`;
        }
        if (enableHighlight && typeof hljs !== 'undefined' && lang) {
            try {
                if (hljs.getLanguage(lang)) {
                    return hljs.highlight(code, { language: lang }).value;
                }
            } catch {
                // fall through
            }
        }
        return escapeHtml(code);
    };
}

async function runMermaidOnElement(root) {
    if (typeof mermaid === 'undefined' || !root) {
        return;
    }
    const nodes = root.querySelectorAll('.mermaid, .language-mermaid');
    for (const node of nodes) {
        if (node.dataset.processed === 'true') {
            continue;
        }
        try {
            const source = node.textContent || '';
            const id = `mermaid-${Math.random().toString(36).slice(2)}`;
            const { svg } = await mermaid.render(id, source);
            node.innerHTML = svg;
            node.dataset.processed = 'true';
        } catch (err) {
            node.innerHTML = `<pre class="sb-markdown-mermaid-error">${escapeHtml(String(err))}</pre>`;
        }
    }
}

async function runHighlightOnElement(root) {
    if (typeof hljs === 'undefined' || !root) {
        return;
    }
    root.querySelectorAll('pre code').forEach((block) => {
        if (!block.classList.contains('language-mermaid')) {
            hljs.highlightElement(block);
        }
    });
}

function extractBodyHtml(html) {
    const match = html.match(/<body[^>]*>([\s\S]*?)<\/body>/i);
    return match ? match[1] : html;
}

export async function renderMarkdown(content, options = {}) {
    await ensureAssets({
        enableMermaid: options.enableMermaid !== false,
        enableHighlight: options.enableHighlight !== false,
        highlightTheme: options.highlightTheme
    });
    configureMarked();
    const html = marked.parse(content || '');
    return html;
}

export async function enhanceRenderedMarkdown(element, options = {}) {
    if (!element) {
        return;
    }
    await ensureAssets({
        enableMermaid: options.enableMermaid !== false,
        enableHighlight: options.enableHighlight !== false,
        highlightTheme: options.highlightTheme
    });
    await runHighlightOnElement(element);
    await runMermaidOnElement(element);
}

function buildPreviewRender(options) {
    return (plainText) => {
        configureMarked();
        const html = marked.parse(plainText || '');
        const wrapper = document.createElement('div');
        wrapper.className = 'sb-markdown-preview';
        wrapper.innerHTML = html;
        setTimeout(async () => {
            await runHighlightOnElement(wrapper);
            await runMermaidOnElement(wrapper);
        }, 0);
        return wrapper.innerHTML;
    };
}

export async function initEditor(textarea, dotNetRef, options) {
    const editorId = options.editorId || `sb-md-editor-${++editorIdCounter}`;
    const sourceMode = options.editorMode === 'source';

    const easyLoaded = await ensureEasyMdeLoaded();
    if (!easyLoaded) {
        return null;
    }

    if (!sourceMode) {
        await ensureAssets({
            enableMermaid: options.enableMermaid,
            enableHighlight: options.enableHighlight,
            highlightTheme: options.highlightTheme
        });
    }

    const easyOptions = {
        element: textarea,
        initialValue: options.value || '',
        placeholder: options.placeholder || '',
        spellChecker: false,
        status: false,
        toolbar: false,
        minHeight: options.minHeight || '200px',
        maxHeight: options.maxHeight,
        lineNumbers: options.lineNumbers !== false,
        lineWrapping: options.lineWrapping !== false,
        direction: options.direction || 'ltr',
        readOnly: options.readOnly || false,
        autoDownloadFontAwesome: false,
        renderingConfig: {
            codeSyntaxHighlighting: options.enableHighlight !== false
        }
    };

    if (sourceMode || options.enablePreview === false) {
        easyOptions.preview = false;
        easyOptions.sideBySideFullscreen = false;
    } else {
        easyOptions.previewRender = buildPreviewRender(options);
        easyOptions.renderingConfig.markedOptions = {
            highlight: buildHighlightFn(options.enableHighlight, options.enableMermaid)
        };
    }

    const easyMDE = new EasyMDE(easyOptions);

    easyMDE.codemirror.on('change', () => {
        const value = easyMDE.value();
        let html = '';
        if (!sourceMode && options.enablePreview !== false) {
            try {
                html = extractBodyHtml(easyMDE.options.previewRender(value));
            } catch {
                html = '';
            }
        }
        dotNetRef.invokeMethodAsync('OnEditorChangeAsync', value, html);
    });

    if (options.readOnly) {
        easyMDE.codemirror.setOption('readOnly', true);
    }

    easyMDE.codemirror.setOption('extraKeys', {
        ...(easyMDE.codemirror.getOption('extraKeys') || {}),
        'Ctrl-S': () => dotNetRef.invokeMethodAsync('NotifyShortcutAsync', 'save'),
        'Cmd-S': () => dotNetRef.invokeMethodAsync('NotifyShortcutAsync', 'save'),
        'Ctrl-P': () => dotNetRef.invokeMethodAsync('NotifyShortcutAsync', 'preview'),
        'Cmd-P': () => dotNetRef.invokeMethodAsync('NotifyShortcutAsync', 'preview')
    });

    editors.set(editorId, { easyMDE, dotNetRef, sourceMode, options });
    return editorId;
}

export async function initDiffReview(container, dotNetRef, options) {
    const editorId = options.editorId || `sb-md-diff-${++editorIdCounter}`;
    const loaded = await ensureDiffAssets();
    if (!loaded) {
        return null;
    }

    const mergeView = CodeMirror.MergeView(container, {
        value: options.modified || '',
        orig: options.original || '',
        lineNumbers: true,
        mode: options.editorMode === 'source' ? 'htmlmixed' : 'markdown',
        highlightChanges: true,
        connect: 'align',
        collapseIdentical: false,
        revertButtons: true,
        readOnly: options.readOnly || false,
        showDifferences: true,
        revertChunk: () => true
    });

    mergeView.edit.on('change', () => {
        dotNetRef.invokeMethodAsync('OnDiffModifiedChangedAsync', mergeView.edit.getValue());
    });

    diffEditors.set(editorId, { mergeView, dotNetRef });
    return editorId;
}

export function getValue(editorId) {
    const diff = diffEditors.get(editorId);
    if (diff) {
        return diff.mergeView.edit.getValue();
    }
    const stored = editors.get(editorId);
    return stored ? stored.easyMDE.value() : '';
}

export function setValue(editorId, value, originalValue) {
    const diff = diffEditors.get(editorId);
    if (diff) {
        const mv = diff.mergeView;
        if (typeof originalValue === 'string') {
            const origEditor = mv.leftOriginal?.() || mv.rightOriginal?.();
            if (origEditor) {
                origEditor.setValue(originalValue);
            }
        }
        mv.edit.setValue(value || '');
        return;
    }
    const stored = editors.get(editorId);
    if (stored) {
        stored.easyMDE.value(value || '');
    }
}

export function insertTextAtCursor(editorId, text) {
    const stored = editors.get(editorId);
    if (!stored) {
        return;
    }
    const cm = stored.easyMDE.codemirror;
    const doc = cm.getDoc();
    const cursor = doc.getCursor();
    doc.replaceRange(text || '', cursor);
    cm.focus();
}

export function execAction(editorId, action, value) {
    const stored = editors.get(editorId);
    if (!stored) {
        return;
    }
    const easyMDE = stored.easyMDE;
    switch (action) {
        case 'bold': easyMDE.toggleBold(); break;
        case 'italic': easyMDE.toggleItalic(); break;
        case 'strikethrough': easyMDE.toggleStrikethrough(); break;
        case 'heading': easyMDE.toggleHeadingSmaller(); break;
        case 'heading-1': easyMDE.toggleHeading1(); break;
        case 'heading-2': easyMDE.toggleHeading2(); break;
        case 'heading-3': easyMDE.toggleHeading3(); break;
        case 'code': easyMDE.toggleCodeBlock(); break;
        case 'quote': easyMDE.toggleBlockquote(); break;
        case 'unordered-list': easyMDE.toggleUnorderedList(); break;
        case 'ordered-list': easyMDE.toggleOrderedList(); break;
        case 'link': easyMDE.drawLink(); break;
        case 'image': easyMDE.drawImage(); break;
        case 'preview': easyMDE.togglePreview(); break;
        case 'side-by-side': easyMDE.toggleSideBySide(); break;
        case 'fullscreen': easyMDE.toggleFullScreen(); break;
        case 'undo': easyMDE.codemirror.undo(); break;
        case 'redo': easyMDE.codemirror.redo(); break;
        default:
            if (value) {
                insertTextAtCursor(editorId, String(value));
            }
            break;
    }
}

export function togglePreview(editorId) {
    const stored = editors.get(editorId);
    if (stored) {
        stored.easyMDE.togglePreview();
    }
}

export function setPreview(editorId, show) {
    const stored = editors.get(editorId);
    if (!stored) {
        return;
    }
    const isPreview = stored.easyMDE.isPreviewActive();
    if (show && !isPreview) {
        stored.easyMDE.togglePreview();
    } else if (!show && isPreview) {
        stored.easyMDE.togglePreview();
    }
}

export function destroyEditor(editorId) {
    const diff = diffEditors.get(editorId);
    if (diff) {
        diff.mergeView.wrapper.parentNode?.removeChild(diff.mergeView.wrapper);
        diffEditors.delete(editorId);
        return;
    }
    const stored = editors.get(editorId);
    if (stored) {
        stored.easyMDE.toTextArea();
        stored.easyMDE.clearAutosavedValue?.();
        editors.delete(editorId);
    }
}

export function destroyDiffReview(editorId) {
    destroyEditor(editorId);
}
