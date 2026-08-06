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
let codeAssetsLoadPromise = null;

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
        const isDark = document.body.classList.contains('sb-theme-dark')
            || document.documentElement.classList.contains('sb-theme-dark')
            || document.querySelector('.sb-theme-dark');
        mermaid.initialize({
            startOnLoad: false,
            theme: isDark ? 'dark' : 'default',
            securityLevel: 'loose',
            fontFamily: 'inherit',
            themeVariables: isDark ? {
                primaryColor: '#1e293b',
                primaryTextColor: '#f1f5f9',
                primaryBorderColor: '#475569',
                lineColor: '#64748b',
                secondaryColor: '#0f172a',
                tertiaryColor: '#1e293b',
                clusterBkg: '#1e293b',
                clusterBorder: '#475569',
                edgeLabelBackground: '#1e293b',
                nodeBorder: '#475569',
            } : {
                primaryColor: '#f8fafc',
                primaryTextColor: '#1e293b',
                primaryBorderColor: '#e2e8f0',
                lineColor: '#94a3b8',
                secondaryColor: '#f1f5f9',
                tertiaryColor: '#f8fafc',
                clusterBkg: '#f8fafc',
                clusterBorder: '#e2e8f0',
                edgeLabelBackground: '#f8fafc',
                nodeBorder: '#e2e8f0',
            }
        });
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

async function ensureCodeAssets() {
    if (typeof CodeMirror !== 'undefined' && CodeMirror.modes?.htmlmixed) {
        return true;
    }
    if (codeAssetsLoadPromise) {
        return codeAssetsLoadPromise;
    }
    codeAssetsLoadPromise = (async () => {
        const base = getContentBasePath();
        loadCss(`${base}/vendor/codemirror/codemirror.css`);
        await loadScript(`${base}/vendor/codemirror/codemirror.js`);
        await loadScript(`${base}/vendor/codemirror/xml.js`);
        await loadScript(`${base}/vendor/codemirror/javascript.js`);
        await loadScript(`${base}/vendor/codemirror/css.js`);
        await loadScript(`${base}/vendor/codemirror/htmlmixed.js`);
        await loadScript(`${base}/vendor/codemirror/markdown.js`);
        return typeof CodeMirror !== 'undefined';
    })();
    return codeAssetsLoadPromise;
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
        // Let marked produce standard <pre><code class="language-X"> output for all languages including mermaid
        return originalCode(code, infostring, false);
    };

    marked.use({ renderer, gfm: true, breaks: true });
    window.__sbMarkedConfigured = true;
}

function escapeHtml(text) {
    return (text ?? '')
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;');
}

function buildHighlightFn(enableHighlight, enableMermaid) {
    return (code, language) => {
        const lang = (language || '').trim().toLowerCase();
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

function convertMermaidPreToSvg(node) {
    // Converts <pre><code class="language-mermaid">content</code></pre> to a mermaid SVG div
    const code = node.querySelector('code.language-mermaid');
    if (!code) return;
    
    const content = code.textContent || '';
    const div = document.createElement('div');
    div.className = 'mermaid';
    div.textContent = content;
    div.dataset.processed = 'true';
    node.parentNode.replaceChild(div, node);
    return { element: div, source: content };
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
            let source;
            let targetNode = node;
            
            // Handle <pre><code class="language-mermaid"> from marked standard output
            if (node.tagName === 'PRE' && node.querySelector('code.language-mermaid')) {
                const result = convertMermaidPreToSvg(node);
                if (!result) continue;
                targetNode = result.element;
                source = result.source;
            } else if (node.tagName === 'CODE' && node.classList.contains('language-mermaid') && node.parentElement?.tagName === 'PRE') {
                // For <code> inside <pre>, process the <pre> parent
                const preNode = node.parentElement;
                const result = convertMermaidPreToSvg(preNode);
                if (!result) continue;
                targetNode = result.element;
                source = result.source;
            } else {
                source = node.textContent || '';
            }
            
            const id = `mermaid-${Math.random().toString(36).slice(2)}`;
            const { svg } = await mermaid.render(id, source);
            targetNode.innerHTML = svg || source;
            targetNode.dataset.processed = 'true';
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

function resolveEditorDirection(options) {
    return options?.direction === 'rtl' ? 'rtl' : 'ltr';
}

function applyPreviewDirection(easyMDE, direction) {
    const wrapper = easyMDE?.codemirror?.getWrapperElement?.();
    if (!wrapper) {
        return;
    }

    const container = wrapper.closest('.EasyMDEContainer');
    if (!container) {
        return;
    }

    container.querySelectorAll('.editor-preview, .editor-preview-side').forEach((element) => {
        element.setAttribute('dir', direction);
        element.style.direction = direction;
        element.style.textAlign = direction === 'rtl' ? 'right' : '';
    });
}

function buildPreviewRender(options) {
    const direction = resolveEditorDirection(options);

    return (plainText) => {
        configureMarked();
        const html = marked.parse(plainText || '');
        const wrapper = document.createElement('div');
        wrapper.className = 'sb-markdown-preview';
        wrapper.setAttribute('dir', direction);
        if (direction === 'rtl') {
            wrapper.style.textAlign = 'right';
        }
        wrapper.innerHTML = html;
        
        // Run highlight.js on the wrapper
        runHighlightOnElement(wrapper);
        
        // Schedule mermaid rendering after the preview is inserted into DOM
        setTimeout(() => {
            const previewElements = document.querySelectorAll('.editor-preview-active, .editor-preview-side, .editor-preview');
            for (const previewEl of previewElements) {
                if (previewEl.querySelector('.mermaid') || previewEl.querySelector('code.language-mermaid')) {
                    runMermaidOnElement(previewEl);
                }
            }
        }, 0);
        
        return wrapper.outerHTML;
    };
}

function resolveCodeMirrorMode(language) {
    switch ((language || '').toLowerCase()) {
        case 'html':
            return 'htmlmixed';
        case 'json':
            return { name: 'javascript', json: true };
        default:
            return 'markdown';
    }
}

function resolveDiffCodeMirrorMode(options) {
    if (options.editorMode === 'source') {
        return resolveCodeMirrorMode(options.sourceLanguage);
    }

    return 'markdown';
}

export async function initEditor(textarea, dotNetRef, options) {
    const editorId = options.editorId || `sb-md-editor-${++editorIdCounter}`;
    const sourceMode = options.editorMode === 'source';

    const easyLoaded = await ensureEasyMdeLoaded();
    if (!easyLoaded) {
        return null;
    }

    if (sourceMode) {
        await ensureCodeAssets();
    } else {
        await ensureAssets({
            enableMermaid: options.enableMermaid,
            enableHighlight: options.enableHighlight,
            highlightTheme: options.highlightTheme
        });
    }

    const direction = resolveEditorDirection(options);

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
        direction,
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
        easyOptions.sideBySideFullscreen = false;
        easyOptions.previewRender = buildPreviewRender(options);
        easyOptions.renderingConfig.markedOptions = {
            highlight: buildHighlightFn(options.enableHighlight, options.enableMermaid)
        };
    }

    const easyMDE = new EasyMDE(easyOptions);
    applyPreviewDirection(easyMDE, direction);

    if (sourceMode) {
        const language = (options.sourceLanguage || '').toLowerCase();
        const mode = resolveCodeMirrorMode(language);
        easyMDE.codemirror.setOption('mode', mode);
        easyMDE.codemirror.setOption('htmlMode', language === 'html');
    }

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
        mode: resolveDiffCodeMirrorMode(options),
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

function toggleContainerFullscreen(stored) {
    const wrapper = stored.easyMDE.codemirror.getWrapperElement();
    const container = wrapper?.closest('.sb-markdown-editor');
    if (!container) {
        return;
    }

    container.classList.toggle('sb-markdown-editor--fullscreen');
    document.documentElement.classList.toggle(
        'sb-markdown-editor-fullscreen-active',
        container.classList.contains('sb-markdown-editor--fullscreen'));
    stored.easyMDE.codemirror.refresh();
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
        case 'preview':
            easyMDE.togglePreview();
            applyPreviewDirection(easyMDE, resolveEditorDirection(stored.options));
            break;
        case 'side-by-side':
            easyMDE.toggleSideBySide();
            applyPreviewDirection(easyMDE, resolveEditorDirection(stored.options));
            break;
        case 'fullscreen': toggleContainerFullscreen(stored); break;
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
        applyPreviewDirection(stored.easyMDE, resolveEditorDirection(stored.options));
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
    applyPreviewDirection(stored.easyMDE, resolveEditorDirection(stored.options));
}

export function destroyEditor(editorId) {
    const diff = diffEditors.get(editorId);
    if (diff) {
        const wrapper = diff.mergeView?.wrapper;
        if (wrapper?.parentNode) {
            wrapper.parentNode.removeChild(wrapper);
        }
        diffEditors.delete(editorId);
        return;
    }
    const stored = editors.get(editorId);
    if (stored) {
        stored.easyMDE?.toTextArea?.();
        stored.easyMDE?.clearAutosavedValue?.();
        editors.delete(editorId);
    }
}

export function destroyDiffReview(editorId) {
    destroyEditor(editorId);
}
