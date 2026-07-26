// SufiBlazor JavaScript Interop
// Version: 0.1.0

window.SufiBlazor = window.SufiBlazor || {};

(function (sb) {
  "use strict";

  /**
   * Focus management utilities
   */
  sb.focus = {
    /**
     * Focus an element by reference
     * @param {HTMLElement} element - Element to focus
     */
    set: function (element) {
      if (element && typeof element.focus === "function") {
        element.focus();
      }
    },

    /**
     * Get the currently focused element
     * @returns {HTMLElement} - Currently focused element
     */
    getActive: function () {
      return document.activeElement;
    },

    /**
     * Trap focus within a container
     * @param {HTMLElement} container - Container element
     * @returns {Function} - Cleanup function
     */
    trap: function (container) {
      if (!container) return function () {};

      const focusableSelector =
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])';
      const focusableElements = container.querySelectorAll(focusableSelector);
      const firstFocusable = focusableElements[0];
      const lastFocusable = focusableElements[focusableElements.length - 1];

      function handleKeyDown(e) {
        if (e.key !== "Tab") return;

        if (e.shiftKey) {
          if (document.activeElement === firstFocusable) {
            lastFocusable.focus();
            e.preventDefault();
          }
        } else {
          if (document.activeElement === lastFocusable) {
            firstFocusable.focus();
            e.preventDefault();
          }
        }
      }

      container.addEventListener("keydown", handleKeyDown);

      return function () {
        container.removeEventListener("keydown", handleKeyDown);
      };
    },
  };

  /**
   * DOM measurement utilities
   */
  sb.dom = {
    /**
     * Get bounding rect of an element
     * @param {HTMLElement} element - Element to measure
     * @returns {DOMRect} - Bounding client rect
     */
    getBoundingRect: function (element) {
      if (!element) return null;
      return element.getBoundingClientRect();
    },

    /**
     * Get bounding rect as a plain object (for Blazor interop serialization).
     * @param {HTMLElement} element - Element to measure
     * @returns {{ top: number, left: number, bottom: number, right: number, width: number, height: number } | null}
     */
    getBoundingRectObject: function (element) {
      if (!element) return null;
      var r = element.getBoundingClientRect();
      var dir = (element.closest && element.closest("[dir]")) || document.documentElement;
      var isRtl = (dir && dir.getAttribute("dir")) === "rtl";
      return {
        top: r.top, left: r.left, bottom: r.bottom, right: r.right,
        width: r.width, height: r.height, isRtl: isRtl,
        viewportWidth: window.innerWidth
      };
    },

    /**
     * Get viewport dimensions
     * @returns {Object} - Viewport width and height
     */
    getViewport: function () {
      return {
        width: window.innerWidth,
        height: window.innerHeight,
      };
    },

    /**
     * Check if viewport is in compact/mobile mode (e.g. for responsive pagination)
     * @param {number} breakpoint - Viewport width breakpoint (default 640)
     * @returns {boolean}
     */
    isCompactView: function (breakpoint) {
      return window.innerWidth < (breakpoint || 640);
    },

    /**
     * Scroll element into view
     * @param {HTMLElement} element - Element to scroll into view
     * @param {Object} options - Scroll options
     */
    scrollIntoView: function (element, options) {
      if (element && typeof element.scrollIntoView === "function") {
        element.scrollIntoView(
          options || { behavior: "smooth", block: "nearest" }
        );
      }
    },
  };

  /**
   * Viewport/resize watcher for responsive components (e.g. pagination)
   */
  sb.viewport = {
    _subscriptions: new Map(),

    /**
     * Watch for compact viewport changes and invoke .NET when breakpoint is crossed
     * @param {string} id - Unique subscription ID
     * @param {Object} dotNetRef - .NET object reference
     * @param {number} breakpoint - Width breakpoint (default 640)
     * @returns {boolean} - Initial isCompact value
     */
    watchCompact: function (id, dotNetRef, breakpoint) {
      breakpoint = breakpoint || 640;
      var lastCompact = window.innerWidth < breakpoint;

      function handler() {
        var isCompact = window.innerWidth < breakpoint;
        if (lastCompact !== isCompact) {
          lastCompact = isCompact;
          dotNetRef.invokeMethodAsync("OnViewportCompactChanged", isCompact);
        }
      }

      window.addEventListener("resize", handler);
      sb.viewport._subscriptions.set(id, handler);
      return lastCompact;
    },

    unwatchCompact: function (id) {
      var handler = sb.viewport._subscriptions.get(id);
      if (handler) {
        window.removeEventListener("resize", handler);
        sb.viewport._subscriptions.delete(id);
      }
    },
  };

  /**
   * Click outside detection
   */
  sb.clickAway = {
    handlers: new Map(),

    /**
     * Register click-away handler
     * @param {HTMLElement} element - Element to watch
     * @param {Object} dotNetRef - .NET object reference for callback
     * @param {string} methodName - Method name to invoke
     */
    register: function (element, dotNetRef, methodName) {
      if (!element) return;

      function handler(e) {
        if (!element.contains(e.target)) {
          dotNetRef.invokeMethodAsync(methodName);
        }
      }

      sb.clickAway.handlers.set(element, handler);
      // Defer so the click that opened the menu doesn't immediately trigger close
      setTimeout(function () {
        if (sb.clickAway.handlers.get(element) === handler) {
          document.addEventListener("click", handler);
        }
      }, 0);
    },

    /**
     * Unregister click-away handler
     * @param {HTMLElement} element - Element to stop watching
     */
    unregister: function (element) {
      const handler = sb.clickAway.handlers.get(element);
      if (handler) {
        document.removeEventListener("click", handler);
        sb.clickAway.handlers.delete(element);
      }
    },
  };

  /**
   * Select dropdown utilities
   */
  sb.select = {
    /**
     * Check if dropdown should flip up to avoid viewport overflow
     * @param {HTMLElement} anchorElement - The select anchor element
     * @param {number} dropdownHeight - Expected dropdown height in pixels
     * @returns {boolean} - True if should flip up
     */
    shouldFlipUp: function (anchorElement, dropdownHeight) {
      if (!anchorElement) return false;
      
      const rect = anchorElement.getBoundingClientRect();
      const viewportHeight = window.innerHeight;
      const spaceBelow = viewportHeight - rect.bottom;
      const spaceAbove = rect.top;
      
      // If there's not enough space below but enough above, flip up
      return spaceBelow < dropdownHeight && spaceAbove > spaceBelow;
    },
  };

  /**
   * Popover utilities - constrain overlay to viewport
   */
  sb.popover = {
    /**
     * Compute shift (in px) so the popover stays inside the viewport.
     * Call after popover is rendered and laid out (e.g. in requestAnimationFrame).
     * @param {HTMLElement} anchorElement - Anchor element (for reference)
     * @param {HTMLElement} popoverElement - The popover element to constrain
     * @param {number} padding - Minimum gap from viewport edge in pixels
     * @returns {Promise<{ shiftX: number, shiftY: number }>}
     */
    constrainToViewport: function (anchorElement, popoverElement, padding) {
      return new Promise(function (resolve) {
        requestAnimationFrame(function () {
          var pad = padding != null ? padding : 8;
          if (!popoverElement) {
            resolve({ shiftX: 0, shiftY: 0 });
            return;
          }
          var r = popoverElement.getBoundingClientRect();
          var vw = window.innerWidth;
          var vh = window.innerHeight;
          var shiftX = 0;
          var shiftY = 0;
          if (r.right > vw - pad) shiftX = (vw - pad) - r.right;
          if (r.left + shiftX < pad) shiftX = pad - r.left;
          if (r.bottom > vh - pad) shiftY = (vh - pad) - r.bottom;
          if (r.top + shiftY < pad) shiftY = pad - r.top;
          resolve({ shiftX: shiftX, shiftY: shiftY });
        });
      });
    },
  };

  /**
   * Slider utilities
   */
  sb.slider = {
    /**
     * Initialize slider drag functionality
     * @param {HTMLElement} trackElement - The track container element
     * @param {DotNetObjectReference} dotNetRef - Reference to .NET component
     * @param {number} min - Minimum value
     * @param {number} max - Maximum value
     * @param {boolean} isRtl - When true, invert position so drag right = increase value
     */
    initDrag: function (trackElement, dotNetRef, min, max, isRtl) {
      if (!trackElement) return;

      let rafId = null;
      let lastClientX = null;
      /** Frozen track rect for the duration of the drag so value-display resize doesn't shift the thumb (RTL). */
      let dragRect = null;

      const valueFromClientX = (clientX, useFrozenRect) => {
        const rect = (useFrozenRect && dragRect) ? dragRect : trackElement.getBoundingClientRect();
        if (rect.width <= 0) return null;
        const offsetX = clientX - rect.left;
        let percentage = Math.max(0, Math.min(1, offsetX / rect.width));
        if (isRtl) percentage = 1 - percentage;
        return min + percentage * (max - min);
      };

      const flush = () => {
        rafId = null;
        if (lastClientX === null) return;
        const value = valueFromClientX(lastClientX, true);
        lastClientX = null;
        if (value !== null) dotNetRef.invokeMethodAsync('OnSliderMove', value);
      };

      const scheduleMove = (clientX) => {
        lastClientX = clientX;
        if (rafId === null) rafId = requestAnimationFrame(flush);
      };

      const handleMouseMove = (e) => {
        e.preventDefault();
        scheduleMove(e.clientX);
      };

      const handleTouchMove = (e) => {
        e.preventDefault();
        if (e.touches.length > 0) scheduleMove(e.touches[0].clientX);
      };

      const handleEnd = () => {
        document.removeEventListener('mousemove', handleMouseMove);
        document.removeEventListener('mouseup', handleEnd);
        document.removeEventListener('touchmove', handleTouchMove);
        document.removeEventListener('touchend', handleEnd);
        document.removeEventListener('touchcancel', handleEnd);
        if (rafId !== null) cancelAnimationFrame(rafId);
        rafId = null;
        if (lastClientX !== null) {
          const value = valueFromClientX(lastClientX, true);
          lastClientX = null;
          if (value !== null) dotNetRef.invokeMethodAsync('OnSliderMove', value);
        }
        dragRect = null;
      };

      const handleStart = (e) => {
        e.preventDefault();
        var clientX = e.type === 'mousedown' ? e.clientX : (e.touches.length > 0 ? e.touches[0].clientX : null);
        if (clientX === null) return;
        var value = valueFromClientX(clientX, false);
        if (value !== null) dotNetRef.invokeMethodAsync('OnSliderMove', value);
        dragRect = trackElement.getBoundingClientRect();
        if (e.type === 'mousedown') {
          document.addEventListener('mousemove', handleMouseMove);
          document.addEventListener('mouseup', handleEnd);
        } else {
          document.addEventListener('touchmove', handleTouchMove, { passive: false });
          document.addEventListener('touchend', handleEnd);
          document.addEventListener('touchcancel', handleEnd);
        }
      };

      trackElement.addEventListener('mousedown', handleStart);
      trackElement.addEventListener('touchstart', handleStart, { passive: false });

      // Return cleanup function
      return () => {
        trackElement.removeEventListener('mousedown', handleStart);
        trackElement.removeEventListener('touchstart', handleStart);
        handleEnd();
      };
    },
  };


  /**
   * Popover API - elevates element to top layer without making page inert (for toasts)
   */
  sb.popover = {
    show: function (element) {
      if (element && typeof element.showPopover === "function") {
        element.showPopover();
      }
    },
    hide: function (element) {
      if (element && typeof element.hidePopover === "function") {
        element.hidePopover();
      }
    },
  };

  /**
   * Native dialog element operations
   * These are minimal - just calling native HTMLDialogElement methods
   */
  sb.dialog = {
    /**
     * Open dialog as modal
     * @param {HTMLDialogElement} element - Dialog element
     */
    showModal: function (element) {
      if (element && !element.open && typeof element.showModal === "function") {
        element.showModal();
      }
    },

    /**
     * Close dialog
     * @param {HTMLDialogElement} element - Dialog element
     */
    close: function (element) {
      if (element && element.open && typeof element.close === "function") {
        element.close();
      }
    },
  };

  /**
   * DataGrid utilities
   */
  sb.dataGrid = {
    activeResize: null,
    
    /**
     * Start column resize operation
     * @param {Object} dotNetRef - .NET object reference for callback
     * @param {string} field - Column field name
     * @param {number} startX - Initial mouse X position
     * @param {number} startWidth - Initial column width
     * @param {string} minWidth - Minimum width (CSS value)
     * @param {string} maxWidth - Maximum width (CSS value)
     */
    startResize: function(dotNetRef, field, startX, startWidth, minWidth, maxWidth) {
      if (sb.dataGrid.activeResize) return;
      
      const minW = parseInt(minWidth) || 50;
      const maxW = parseInt(maxWidth) || 1000;
      
      sb.dataGrid.activeResize = {
        dotNetRef: dotNetRef,
        field: field,
        startX: startX,
        startWidth: startWidth,
        minWidth: minW,
        maxWidth: maxW
      };
      
      document.body.classList.add('sb-resizing');
      document.addEventListener('mousemove', sb.dataGrid._onMouseMove);
      document.addEventListener('mouseup', sb.dataGrid._onMouseUp);
    },
    
    _onMouseMove: function(e) {
      const resize = sb.dataGrid.activeResize;
      if (!resize) return;
      
      const delta = e.clientX - resize.startX;
      let newWidth = resize.startWidth + delta;
      
      // Clamp to min/max
      newWidth = Math.max(resize.minWidth, Math.min(resize.maxWidth, newWidth));
      
      // Notify Blazor of resize in progress
      resize.dotNetRef.invokeMethodAsync('OnColumnResizing', resize.field, newWidth);
    },
    
    _onMouseUp: function(e) {
      const resize = sb.dataGrid.activeResize;
      if (!resize) return;
      
      const delta = e.clientX - resize.startX;
      let newWidth = resize.startWidth + delta;
      newWidth = Math.max(resize.minWidth, Math.min(resize.maxWidth, newWidth));
      
      // Notify Blazor of resize complete
      resize.dotNetRef.invokeMethodAsync('OnColumnResizeComplete', resize.field, newWidth);
      
      // Cleanup
      document.body.classList.remove('sb-resizing');
      document.removeEventListener('mousemove', sb.dataGrid._onMouseMove);
      document.removeEventListener('mouseup', sb.dataGrid._onMouseUp);
      
      sb.dataGrid.activeResize = null;
    },
    
    /**
     * Download a file
     * @param {string} filename - File name
     * @param {string} base64Content - Base64 encoded content
     * @param {string} mimeType - MIME type
     */
    downloadFile: function(filename, base64Content, mimeType) {
      const link = document.createElement('a');
      link.href = `data:${mimeType};base64,${base64Content}`;
      link.download = filename;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  };

  /**
   * Theme utilities
   */
  sb.theme = {
    /**
     * Get system color scheme preference
     * @returns {string} - 'dark' or 'light'
     */
    getSystemPreference: function () {
      if (
        window.matchMedia &&
        window.matchMedia("(prefers-color-scheme: dark)").matches
      ) {
        return "dark";
      }
      return "light";
    },

    /**
     * Watch for system color scheme changes
     * @param {Object} dotNetRef - .NET object reference for callback
     * @param {string} methodName - Method name to invoke
     * @returns {Function} - Cleanup function
     */
    watchSystemPreference: function (dotNetRef, methodName) {
      const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");

      function handler(e) {
        dotNetRef.invokeMethodAsync(methodName, e.matches ? "dark" : "light");
      }

      mediaQuery.addEventListener("change", handler);

      return function () {
        mediaQuery.removeEventListener("change", handler);
      };
    },
  };

  sb.markdown = {
    setInnerHtml: function (element, html) {
      if (element) {
        element.innerHTML = html || "";
      }
    },
  };

  /**
   * Split pane divider drag (pointer, not HTML5 DnD).
   */
  sb.splitPane = {
    active: null,

    /**
     * @param {HTMLElement} container
     * @param {Object} dotNetRef
     * @param {string} orientation - "horizontal" | "vertical"
     */
    startDrag: function (container, dotNetRef, orientation) {
      if (!container || !dotNetRef) return;
      if (sb.splitPane.active) return;

      sb.splitPane.active = {
        container: container,
        dotNetRef: dotNetRef,
        orientation: (orientation || "horizontal").toLowerCase(),
      };

      document.body.classList.add("sb-resizing");
      document.addEventListener("mousemove", sb.splitPane._onMouseMove);
      document.addEventListener("mouseup", sb.splitPane._onMouseUp);
    },

    _onMouseMove: function (e) {
      var active = sb.splitPane.active;
      if (!active || !active.container) return;

      var rect = active.container.getBoundingClientRect();
      var position;
      if (active.orientation === "vertical") {
        if (rect.height <= 0) return;
        position = ((e.clientY - rect.top) / rect.height) * 100;
      } else {
        if (rect.width <= 0) return;
        position = ((e.clientX - rect.left) / rect.width) * 100;
      }

      active.dotNetRef.invokeMethodAsync("UpdatePosition", position);
    },

    _onMouseUp: function () {
      var active = sb.splitPane.active;
      if (!active) return;

      active.dotNetRef.invokeMethodAsync("EndDrag");
      document.body.classList.remove("sb-resizing");
      document.removeEventListener("mousemove", sb.splitPane._onMouseMove);
      document.removeEventListener("mouseup", sb.splitPane._onMouseUp);
      sb.splitPane.active = null;
    },
  };
})(window.SufiBlazor);
