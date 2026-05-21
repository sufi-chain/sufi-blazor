/**
 * SufiBlazor Rich Text Editor JavaScript Module
 * Built on Quill.js
 * 
 * This module is loaded on-demand when SbRichTextEditor is used.
 * Quill.js is automatically loaded from the bundled vendor files.
 */

// Editor instances registry
const editors = new Map();
let editorIdCounter = 0;

// Turndown service for Markdown conversion (lazy loaded)
let turndownService = null;

// Quill loading state
let quillLoaded = false;
let quillLoadPromise = null;

/**
 * Get the base path for static content
 */
function getContentBasePath() {
    // Find our script tag to determine the base path
    const scripts = document.querySelectorAll('script[src*="sufiblazor-editor.js"]');
    if (scripts.length > 0) {
        const src = scripts[0].src;
        return src.substring(0, src.lastIndexOf('/'));
    }
    // Fallback to standard Blazor static content path
    return '_content/SufiChain.SufiBlazor';
}

// Quill CDN fallback URLs
const QUILL_CDN_JS = 'https://cdn.quilljs.com/1.3.7/quill.min.js';
const QUILL_CDN_CSS = 'https://cdn.quilljs.com/1.3.7/quill.snow.css';

/**
 * Load a script dynamically
 * @param {string} src - Script URL
 * @returns {Promise<boolean>}
 */
function loadScript(src) {
    return new Promise((resolve) => {
        const script = document.createElement('script');
        script.src = src;
        script.onload = () => resolve(true);
        script.onerror = () => resolve(false);
        document.head.appendChild(script);
    });
}

/**
 * Load a CSS file dynamically
 * @param {string} href - CSS URL
 */
function loadCss(href) {
    if (!document.querySelector(`link[href="${href}"]`)) {
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = href;
        document.head.appendChild(link);
    }
}

/**
 * Dynamically load Quill.js and its CSS if not already loaded.
 * Tries bundled files first, falls back to CDN.
 * @returns {Promise<boolean>} - Whether Quill was successfully loaded
 */
async function ensureQuillLoaded() {
    // Already loaded
    if (typeof Quill !== 'undefined') {
        quillLoaded = true;
        return true;
    }
    
    // Already loading
    if (quillLoadPromise) {
        return quillLoadPromise;
    }
    
    quillLoadPromise = (async () => {
        const basePath = getContentBasePath();
        const bundledJs = `${basePath}/vendor/quill.min.js`;
        const bundledCss = `${basePath}/vendor/quill.snow.css`;
        
        // Try to load from bundled files first
        loadCss(bundledCss);
        let loaded = await loadScript(bundledJs);
        
        if (!loaded || typeof Quill === 'undefined') {
            // Fallback to CDN
            console.log('SufiBlazor: Loading Quill.js from CDN...');
            loadCss(QUILL_CDN_CSS);
            loaded = await loadScript(QUILL_CDN_JS);
        }
        
        if (loaded && typeof Quill !== 'undefined') {
            // Register custom font whitelist for Persian/editor fonts (Dirooz, Samim, Gandom, Sahel FD)
            try {
                const Font = Quill.import('attributors/class/font');
                Font.whitelist = ['dirooz-fd', 'samim-fd', 'gandom-fd', 'sahel-fd'];
                Quill.register(Font, true);
            } catch (e) {
                console.warn('SufiBlazor: Could not register custom fonts', e);
            }
            quillLoaded = true;
            console.log('SufiBlazor: Quill.js loaded successfully');
            return true;
        }
        
        console.error('SufiBlazor: Failed to load Quill.js');
        return false;
    })();
    
    return quillLoadPromise;
}

/**
 * Initialize a new Quill editor instance.
 * @param {HTMLElement} container - The container element
 * @param {Object} dotNetRef - .NET object reference for callbacks
 * @param {Object} options - Editor options
 * @returns {Promise<string>} - The editor ID
 */
export async function initEditor(container, dotNetRef, options) {
    const editorId = `sb-editor-${++editorIdCounter}`;
    
    // Ensure Quill is loaded
    const loaded = await ensureQuillLoaded();
    if (!loaded || typeof Quill === 'undefined') {
        console.error('SufiBlazor: Quill.js could not be loaded. Rich text editor will not work.');
        return null;
    }
    
    // Build Quill configuration - NO toolbar, we use our own Blazor-rendered toolbar
    const quillOptions = {
        theme: 'snow',
        placeholder: options.placeholder || '',
        readOnly: options.readOnly || false,
        modules: {
            toolbar: false, // Disable Quill's built-in toolbar - we render our own
            history: {
                delay: 1000,
                maxStack: 100,
                userOnly: true
            },
            clipboard: {
                matchVisual: false
            }
        }
    };
    
    // Create Quill instance
    const quill = new Quill(container, quillOptions);
    
    // Create and store instance
    const instance = {
        quill,
        dotNetRef,
        options,
        container,
        pasteCleanupOptions: options.pasteCleanup || null
    };
    editors.set(editorId, instance);
    
    // Set RTL direction if specified
    if (options.direction === 'rtl') {
        container.setAttribute('dir', 'rtl');
        quill.root.setAttribute('dir', 'rtl');
    }
    
    // Set up event handlers - notify .NET for both 'user' and 'api' so bound Value
    // updates when the user types AND when we apply formats from the toolbar (e.g. link dialog)
    quill.on('text-change', (delta, oldDelta, source) => {
        if (source === 'user' || source === 'api') {
            const html = quill.root.innerHTML;
            const text = quill.getText();
            dotNetRef.invokeMethodAsync('OnEditorChangeAsync', html, text);
        }
    });
    
    quill.on('selection-change', (range, oldRange, source) => {
        if (range) {
            // Save selection for toolbar dropdowns (focus loss when clicking select clears selection)
            instance.lastSelection = { index: range.index, length: range.length };
            const formats = quill.getFormat(range);
            // Send actual format values (not just booleans) so select controls (align, header) show correct option
            const serializable = {};
            for (const [key, value] of Object.entries(formats)) {
                if (value === undefined || value === null) continue;
                if (typeof value === 'boolean' || typeof value === 'string' || typeof value === 'number') {
                    serializable[key] = value;
                } else {
                    serializable[key] = !!value;
                }
            }
            dotNetRef.invokeMethodAsync('OnSelectionChange', serializable);
        }
    });
    
    // Focus/blur events
    quill.root.addEventListener('focus', () => {
        dotNetRef.invokeMethodAsync('OnEditorFocusAsync');
    });
    
    quill.root.addEventListener('blur', () => {
        dotNetRef.invokeMethodAsync('OnEditorBlurAsync');
    });
    
    // Apply paste cleanup - reads from instance.pasteCleanupOptions so it can be updated dynamically
    setupPasteCleanup(quill, instance);
    
    return editorId;
}

/**
 * Destroy an editor instance.
 * @param {string} editorId - The editor ID
 */
export function destroyEditor(editorId) {
    const instance = editors.get(editorId);
    if (instance) {
        // Quill doesn't have a destroy method, but we can clean up
        instance.quill.disable();
        instance.container.innerHTML = '';
        editors.delete(editorId);
    }
}

/**
 * Get the editor content in the specified format.
 * @param {string} editorId - The editor ID
 * @param {string} mode - The output mode: 'html', 'markdown', or 'plaintext'
 * @returns {string} - The content
 */
export function getContent(editorId, mode) {
    const instance = editors.get(editorId);
    if (!instance) return '';
    
    const quill = instance.quill;
    
    switch (mode?.toLowerCase()) {
        case 'markdown':
            return htmlToMarkdown(quill.root.innerHTML);
        case 'plaintext':
            return quill.getText().trim();
        case 'html':
        default:
            return quill.root.innerHTML;
    }
}

/**
 * Set the editor content.
 * @param {string} editorId - The editor ID
 * @param {string} content - The content to set
 * @param {string} mode - The input mode: 'html', 'markdown', or 'plaintext'
 */
export function setContent(editorId, content, mode) {
    const instance = editors.get(editorId);
    if (!instance) return;
    
    const quill = instance.quill;
    
    switch (mode?.toLowerCase()) {
        case 'markdown':
            // Convert markdown to HTML first
            const html = markdownToHtml(content);
            quill.root.innerHTML = html;
            break;
        case 'plaintext':
            quill.setText(content || '');
            break;
        case 'html':
        default:
            if (content) {
                quill.root.innerHTML = content;
            } else {
                quill.setText('');
            }
            break;
    }
}

/**
 * Focus the editor.
 * @param {string} editorId - The editor ID
 */
export function focus(editorId) {
    const instance = editors.get(editorId);
    if (instance) {
        instance.quill.focus();
    }
}

/**
 * Get the current selection range.
 * @param {string} editorId - The editor ID
 * @returns {Object|null} - The selection range { index, length }
 */
export function getSelection(editorId) {
    const instance = editors.get(editorId);
    if (!instance) return null;
    
    const range = instance.quill.getSelection();
    return range ? { index: range.index, length: range.length } : null;
}

/**
 * Set the selection range.
 * @param {string} editorId - The editor ID
 * @param {number} index - Start index
 * @param {number} length - Selection length
 */
export function setSelection(editorId, index, length) {
    const instance = editors.get(editorId);
    if (instance) {
        instance.quill.setSelection(index, length);
    }
}

/**
 * Apply a format to the current selection.
 * @param {string} editorId - The editor ID
 * @param {string} format - The format name
 * @param {*} value - The format value
 */
export function format(editorId, formatName, value) {
    const instance = editors.get(editorId);
    if (!instance) return;
    
    const quill = instance.quill;
    
    // Block-level formats (align, header, list, etc.) should apply to all content when no selection
    const isBlockFormat = ['align', 'header', 'direction'].includes(formatName);
    
    // Get current selection (don't focus yet - block formats don't need focus)
    let range = quill.getSelection(false); // false = don't focus
    
    // If no range or no selection length, and it's a block format, format all lines
    // This doesn't require focus since we're formatting the entire document
    if (isBlockFormat && (!range || range.length === 0)) {
        try {
            const length = quill.getLength();
            if (length > 0) {
                quill.formatLine(0, length, formatName, value);
            }
        } catch (error) {
            console.warn('SbEditor: formatLine operation failed', error);
        }
        return;
    }
    
    // For inline formats, we need focus to work with selection
    quill.focus();
    
    // If no range (e.g. focus was lost when user clicked toolbar dropdown), restore last selection
    if (!range && instance.lastSelection) {
        const last = instance.lastSelection;
        const len = quill.getLength();
        // Clamp to valid range (content may have changed)
        const index = Math.min(last.index, Math.max(0, len - 1));
        const length = Math.min(last.length, len - index);
        quill.setSelection(index, length);
        range = { index, length };
    }
    if (!range) {
        const length = quill.getLength();
        quill.setSelection(length > 0 ? length - 1 : 0, 0);
        range = quill.getSelection();
    }
    
    if (!range) return; // Safety check
    
    try {
        // Use explicit value when provided (including false for "remove format"); otherwise default to toggle/true
        const valueToApply = value !== undefined && value !== null ? value : true;
        if (range.length === 0) {
            // No selection - apply value (or toggle if not explicitly provided)
            const currentFormat = quill.getFormat(range);
            const newValue = value !== undefined && value !== null ? value : (currentFormat[formatName] ? false : true);
            quill.format(formatName, newValue);
        } else {
            // Has selection - apply format (false removes alignment when user selects "Left")
            quill.format(formatName, valueToApply);
        }
    } catch (error) {
        console.warn('SbEditor: format operation failed', error);
    }
}

/**
 * Insert content at the current cursor position.
 * @param {string} editorId - The editor ID
 * @param {string} type - The insert type
 * @param {*} value - The value to insert
 */
export function insert(editorId, type, value) {
    const instance = editors.get(editorId);
    if (!instance) return;
    
    const quill = instance.quill;
    const range = quill.getSelection(true);
    
    switch (type) {
        case 'text':
            quill.insertText(range.index, value);
            break;
        case 'embed':
            quill.insertEmbed(range.index, value.type, value.value);
            break;
        default:
            quill.insertText(range.index, value);
    }
}

/**
 * Insert an image at the current cursor position.
 * @param {string} editorId - The editor ID
 * @param {string} url - The image URL
 * @param {string} alt - The alt text
 * @param {string} width - Optional width (e.g. "200" for pixels or "200px")
 * @param {string} height - Optional height (e.g. "150" for pixels or "150px")
 */
export function insertImage(editorId, url, alt, width, height) {
    const instance = editors.get(editorId);
    if (!instance) return;
    if (!url || typeof url !== 'string' || !url.trim()) return;
    
    const quill = instance.quill;
    const range = quill.getSelection(true);
    
    quill.insertEmbed(range.index, 'image', url.trim());
    quill.setSelection(range.index + 1);
    
    // Apply alt, width, height via DOM (Quill embed only stores src)
    const urlTrimmed = url.trim();
    setTimeout(() => {
        const images = quill.root.querySelectorAll('img');
        const lastImage = images[images.length - 1];
        if (lastImage && lastImage.src === urlTrimmed) {
            if (alt) lastImage.alt = alt;
            if (width && String(width).trim()) lastImage.setAttribute('width', String(width).trim());
            if (height && String(height).trim()) lastImage.setAttribute('height', String(height).trim());
        }
    }, 0);
}

/**
 * Insert HTML content at the current cursor position.
 * Uses dangerouslyPasteHTML to avoid Quill.import('delta') issues in some Quill build configurations.
 * @param {string} editorId - The editor ID
 * @param {string} html - The HTML content to insert
 */
export function insertHtml(editorId, html) {
    const instance = editors.get(editorId);
    if (!instance) return;
    
    const quill = instance.quill;
    const range = quill.getSelection(true);
    const index = range ? range.index : quill.getLength();
    const length = range ? range.length : 0;
    
    // Focus editor so it's ready for insertion (e.g. after modal closed)
    quill.focus();
    
    // Remove any selected content first (replace selection)
    if (length > 0) {
        quill.deleteText(index, length, 'user');
    }
    
    // Use dangerouslyPasteHTML - inserts at index, avoids Delta/concat issues in some Quill builds
    quill.clipboard.dangerouslyPasteHTML(index, html, 'user');
    
    // Move cursor to end of inserted content (approximate from plain-text length)
    const tempDiv = document.createElement('div');
    tempDiv.innerHTML = html;
    const insertedLength = (tempDiv.textContent || '').length || 1;
    quill.setSelection(index + insertedLength, 0);
}

/**
 * Get the currently selected text.
 * @param {string} editorId - The editor ID
 * @returns {string|null} - The selected text, or null if no selection
 */
export function getSelectionText(editorId) {
    const instance = editors.get(editorId);
    if (!instance) return null;
    
    const quill = instance.quill;
    const range = quill.getSelection();
    
    if (!range || range.length === 0) {
        return null;
    }
    
    return quill.getText(range.index, range.length);
}

/**
 * Get the format at the current selection (e.g. { link: "url", bold: true }).
 * Used to pre-fill link dialog when editing an existing link.
 * @param {string} editorId - The editor ID
 * @returns {Object} - Format key-value map (values serializable for .NET)
 */
export function getFormat(editorId) {
    const instance = editors.get(editorId);
    if (!instance) return {};
    
    const quill = instance.quill;
    const range = quill.getSelection();
    
    if (!range) return {};
    
    const format = quill.getFormat(range);
    if (!format || typeof format !== 'object') return {};
    
    // Return a plain object with serializable values (no undefined)
    const result = {};
    for (const [key, value] of Object.entries(format)) {
        if (value !== undefined && value !== null) {
            result[key] = value;
        }
    }
    
    // When selection is inside a link, read target and rel from the DOM (Quill format only stores href)
    if (result.link) {
        const anchor = getLinkElementAtRange(quill, range);
        if (anchor) {
            result.target = anchor.getAttribute('target') || '';
            result.rel = anchor.getAttribute('rel') || '';
        }
    }
    
    return result;
}

/**
 * Get the <a> DOM element at the given selection range (first link in range).
 * @param {Quill} quill - Quill instance
 * @param {Object} range - { index, length }
 * @returns {HTMLAnchorElement|null}
 */
function getLinkElementAtRange(quill, range) {
    try {
        const [leaf] = quill.getLeaf(range.index);
        if (!leaf || !leaf.domNode) return null;
        let node = leaf.domNode;
        while (node && node !== quill.root) {
            if (node.tagName === 'A') return node;
            node = node.parentNode;
        }
    } catch (e) {
        // Fallback: find first <a> that contains the selection by DOM range
        const sel = quill.getSelection(true);
        if (!sel) return null;
        const cursor = document.createRange();
        const [node, offset] = quill.getLeaf(sel.index);
        if (node && node.domNode) {
            let n = node.domNode;
            while (n && n !== quill.root) {
                if (n.tagName === 'A') return n;
                n = n.parentNode;
            }
        }
    }
    return null;
}

/**
 * Set target and rel on all <a> elements that intersect the given range.
 * @param {Quill} quill - Quill instance
 * @param {Object} range - { index, length }
 * @param {string} target - e.g. '_blank', '_self'
 * @param {string} rel - e.g. 'noopener noreferrer'
 */
function setLinkAttributesInRange(quill, range, target, rel) {
    const anchor = getLinkElementAtRange(quill, range);
    if (!anchor) return;
    if (target) anchor.setAttribute('target', target); else anchor.removeAttribute('target');
    if (rel) anchor.setAttribute('rel', rel); else anchor.removeAttribute('rel');
}

/**
 * Insert a link at the current selection.
 * @param {string} editorId - The editor ID
 * @param {string} url - The link URL
 * @param {string} text - Optional text to insert (if no selection)
 * @param {string} target - Optional target (e.g. '_blank', '_self')
 * @param {string} rel - Optional rel (e.g. 'noopener noreferrer')
 */
export function insertLink(editorId, url, text, target, rel) {
    const instance = editors.get(editorId);
    if (!instance) return;
    
    const quill = instance.quill;
    const range = quill.getSelection(true);
    
    if (range.length > 0) {
        // Has selection - apply link to selection
        quill.format('link', url);
    } else if (text) {
        // No selection but text provided - insert text with link
        quill.insertText(range.index, text, 'link', url);
        quill.setSelection(range.index + text.length);
    } else {
        // No selection and no text - insert the URL as link text
        quill.insertText(range.index, url, 'link', url);
        quill.setSelection(range.index + url.length);
    }
    
    // Apply target and rel via DOM (Quill format only stores href)
    const appliedRange = quill.getSelection(true);
    if (appliedRange && (target || rel)) {
        setLinkAttributesInRange(quill, appliedRange, target || '', rel || '');
    }
}

/**
 * Undo the last change.
 * @param {string} editorId - The editor ID
 */
export function undo(editorId) {
    const instance = editors.get(editorId);
    if (!instance) return;
    
    try {
        instance.quill.history.undo();
    } catch (error) {
        console.warn('SbEditor: undo operation failed', error);
    }
}

/**
 * Redo the last undone change.
 * @param {string} editorId - The editor ID
 */
export function redo(editorId) {
    const instance = editors.get(editorId);
    if (!instance) return;
    
    try {
        instance.quill.history.redo();
    } catch (error) {
        console.warn('SbEditor: redo operation failed', error);
    }
}

/**
 * Clear all formatting from the current selection.
 * @param {string} editorId - The editor ID
 */
export function clearFormatting(editorId) {
    const instance = editors.get(editorId);
    if (!instance) return;
    
    const quill = instance.quill;
    
    try {
        // Ensure focus
        quill.focus();
        
        const range = quill.getSelection();
        
        if (range && range.length > 0) {
            quill.removeFormat(range.index, range.length);
        }
    } catch (error) {
        console.warn('SbEditor: clearFormatting operation failed', error);
    }
}

/**
 * Enable or disable the editor.
 * @param {string} editorId - The editor ID
 * @param {boolean} enabled - Whether to enable the editor
 */
export function setEnabled(editorId, enabled) {
    const instance = editors.get(editorId);
    if (instance) {
        if (enabled) {
            instance.quill.enable();
        } else {
            instance.quill.disable();
        }
    }
}

/**
 * Set the text direction.
 * @param {string} editorId - The editor ID
 * @param {string} direction - 'ltr' or 'rtl'
 */
export function setDirection(editorId, direction) {
    const instance = editors.get(editorId);
    if (instance) {
        instance.container.setAttribute('dir', direction);
        instance.quill.root.setAttribute('dir', direction);
        
        // Also update align format for existing content
        if (direction === 'rtl') {
            instance.quill.format('direction', 'rtl');
        } else {
            instance.quill.format('direction', false);
        }
    }
}

// ============================================
// Helper Functions
// ============================================

function getDefaultToolbar() {
    return [
        ['undo', 'redo'],
        [{ 'header': [1, 2, 3, false] }],
        ['bold', 'italic', 'underline', 'strike'],
        [{ 'list': 'ordered' }, { 'list': 'bullet' }],
        [{ 'align': [] }],
        ['link', 'image'],
        ['blockquote', 'code-block'],
        ['clean']
    ];
}

function setupPasteCleanup(quill, instance) {
    // Get Delta class from Quill
    const Delta = Quill.import('delta');
    
    // Custom clipboard matchers for paste cleanup
    // Reads from instance.pasteCleanupOptions so options can be updated dynamically
    quill.clipboard.addMatcher(Node.ELEMENT_NODE, (node, delta) => {
        const options = instance.pasteCleanupOptions;
        if (!options) return delta;
        
        // Support both PascalCase (from C#) and camelCase property names
        if (options.StripAllFormatting || options.stripAllFormatting) {
            // Return plain text only
            return new Delta().insert(node.textContent || '');
        }
        
        if (options.CleanWordHtml || options.cleanWordHtml) {
            // Remove Word-specific styles and elements
            cleanWordHtml(node);
        }
        
        if ((options.RemoveInlineStyles || options.removeInlineStyles) && node.style) {
            node.style.cssText = '';
        }
        
        if (options.RemoveCssClasses || options.removeCssClasses) {
            node.className = '';
        }
        
        return delta;
    });
}

/**
 * Update paste cleanup options for an editor.
 * @param {string} editorId - The editor ID
 * @param {Object} options - The new paste cleanup options
 */
export function setPasteCleanupOptions(editorId, options) {
    const instance = editors.get(editorId);
    if (instance) {
        instance.pasteCleanupOptions = options;
    }
}

function cleanWordHtml(node) {
    // Remove MSO-specific styles
    if (node.style) {
        const style = node.style.cssText;
        if (style.includes('mso-') || style.includes('MsoNormal')) {
            node.style.cssText = style.replace(/mso-[^;]+;?/gi, '');
        }
    }
    
    // Remove Word-specific classes
    if (node.className && typeof node.className === 'string') {
        node.className = node.className.replace(/Mso\w+/g, '').trim();
    }
    
    // Recursively clean children
    for (const child of node.children) {
        cleanWordHtml(child);
    }
}

function htmlToMarkdown(html) {
    // Simple HTML to Markdown conversion
    // For production, consider using Turndown.js
    if (!html) return '';
    
    let md = html;
    
    // Convert headings
    md = md.replace(/<h1[^>]*>(.*?)<\/h1>/gi, '# $1\n\n');
    md = md.replace(/<h2[^>]*>(.*?)<\/h2>/gi, '## $1\n\n');
    md = md.replace(/<h3[^>]*>(.*?)<\/h3>/gi, '### $1\n\n');
    md = md.replace(/<h4[^>]*>(.*?)<\/h4>/gi, '#### $1\n\n');
    md = md.replace(/<h5[^>]*>(.*?)<\/h5>/gi, '##### $1\n\n');
    md = md.replace(/<h6[^>]*>(.*?)<\/h6>/gi, '###### $1\n\n');
    
    // Convert formatting
    md = md.replace(/<strong[^>]*>(.*?)<\/strong>/gi, '**$1**');
    md = md.replace(/<b[^>]*>(.*?)<\/b>/gi, '**$1**');
    md = md.replace(/<em[^>]*>(.*?)<\/em>/gi, '*$1*');
    md = md.replace(/<i[^>]*>(.*?)<\/i>/gi, '*$1*');
    md = md.replace(/<u[^>]*>(.*?)<\/u>/gi, '$1');
    md = md.replace(/<s[^>]*>(.*?)<\/s>/gi, '~~$1~~');
    md = md.replace(/<strike[^>]*>(.*?)<\/strike>/gi, '~~$1~~');
    
    // Convert links
    md = md.replace(/<a[^>]+href="([^"]*)"[^>]*>(.*?)<\/a>/gi, '[$2]($1)');
    
    // Convert images
    md = md.replace(/<img[^>]+src="([^"]*)"[^>]*alt="([^"]*)"[^>]*\/?>/gi, '![$2]($1)');
    md = md.replace(/<img[^>]+src="([^"]*)"[^>]*\/?>/gi, '![]($1)');
    
    // Convert lists
    md = md.replace(/<ul[^>]*>(.*?)<\/ul>/gis, (match, content) => {
        return content.replace(/<li[^>]*>(.*?)<\/li>/gis, '- $1\n') + '\n';
    });
    md = md.replace(/<ol[^>]*>(.*?)<\/ol>/gis, (match, content) => {
        let i = 1;
        return content.replace(/<li[^>]*>(.*?)<\/li>/gis, () => `${i++}. $1\n`) + '\n';
    });
    
    // Convert blockquotes
    md = md.replace(/<blockquote[^>]*>(.*?)<\/blockquote>/gis, '> $1\n\n');
    
    // Convert code blocks
    md = md.replace(/<pre[^>]*><code[^>]*>(.*?)<\/code><\/pre>/gis, '```\n$1\n```\n\n');
    md = md.replace(/<code[^>]*>(.*?)<\/code>/gi, '`$1`');
    
    // Convert paragraphs and line breaks
    md = md.replace(/<p[^>]*>(.*?)<\/p>/gis, '$1\n\n');
    md = md.replace(/<br\s*\/?>/gi, '\n');
    md = md.replace(/<div[^>]*>(.*?)<\/div>/gis, '$1\n');
    
    // Remove remaining HTML tags
    md = md.replace(/<[^>]+>/g, '');
    
    // Decode HTML entities
    md = md.replace(/&nbsp;/g, ' ');
    md = md.replace(/&amp;/g, '&');
    md = md.replace(/&lt;/g, '<');
    md = md.replace(/&gt;/g, '>');
    md = md.replace(/&quot;/g, '"');
    
    // Clean up extra whitespace
    md = md.replace(/\n{3,}/g, '\n\n');
    
    return md.trim();
}

function markdownToHtml(markdown) {
    // Simple Markdown to HTML conversion
    // For production, consider using Marked.js or similar
    if (!markdown) return '';
    
    let html = markdown;
    
    // Escape HTML
    html = html.replace(/&/g, '&amp;');
    html = html.replace(/</g, '&lt;');
    html = html.replace(/>/g, '&gt;');
    
    // Convert headings
    html = html.replace(/^###### (.+)$/gm, '<h6>$1</h6>');
    html = html.replace(/^##### (.+)$/gm, '<h5>$1</h5>');
    html = html.replace(/^#### (.+)$/gm, '<h4>$1</h4>');
    html = html.replace(/^### (.+)$/gm, '<h3>$1</h3>');
    html = html.replace(/^## (.+)$/gm, '<h2>$1</h2>');
    html = html.replace(/^# (.+)$/gm, '<h1>$1</h1>');
    
    // Convert formatting
    html = html.replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>');
    html = html.replace(/\*(.+?)\*/g, '<em>$1</em>');
    html = html.replace(/~~(.+?)~~/g, '<s>$1</s>');
    html = html.replace(/`(.+?)`/g, '<code>$1</code>');
    
    // Convert links and images
    html = html.replace(/!\[([^\]]*)\]\(([^)]+)\)/g, '<img src="$2" alt="$1">');
    html = html.replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2">$1</a>');
    
    // Convert blockquotes
    html = html.replace(/^> (.+)$/gm, '<blockquote>$1</blockquote>');
    
    // Convert unordered lists
    html = html.replace(/^- (.+)$/gm, '<li>$1</li>');
    html = html.replace(/(<li>.*<\/li>\n?)+/g, '<ul>$&</ul>');
    
    // Convert ordered lists
    html = html.replace(/^\d+\. (.+)$/gm, '<li>$1</li>');
    
    // Convert code blocks
    html = html.replace(/```\n?([\s\S]*?)\n?```/g, '<pre><code>$1</code></pre>');
    
    // Convert paragraphs
    html = html.replace(/\n\n/g, '</p><p>');
    html = '<p>' + html + '</p>';
    html = html.replace(/<p><\/p>/g, '');
    html = html.replace(/<p>(<h[1-6]>)/g, '$1');
    html = html.replace(/(<\/h[1-6]>)<\/p>/g, '$1');
    html = html.replace(/<p>(<ul>)/g, '$1');
    html = html.replace(/(<\/ul>)<\/p>/g, '$1');
    html = html.replace(/<p>(<blockquote>)/g, '$1');
    html = html.replace(/(<\/blockquote>)<\/p>/g, '$1');
    html = html.replace(/<p>(<pre>)/g, '$1');
    html = html.replace(/(<\/pre>)<\/p>/g, '$1');
    
    // Convert line breaks
    html = html.replace(/\n/g, '<br>');
    
    return html;
}
