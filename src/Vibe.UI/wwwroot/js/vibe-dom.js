/**
 * Vibe.UI DOM Utilities Module
 * CSP-safe DOM operations for Blazor components
 */

const initializedGridNavigationGuards = new WeakSet();
const initializedExpandedKeyGuards = new WeakSet();

/**
 * Gets the bounding client rect for an element by ID
 * @param {string} elementId - The element ID
 * @returns {DOMRect|null} The bounding rect or null if not found
 */
export function getBoundingRect(elementId) {
  const element = document.getElementById(elementId);
  if (!element) return null;
  return element.getBoundingClientRect();
}

/**
 * Gets the bounding client rect for an element's parent
 * @param {string} elementId - The element ID
 * @returns {DOMRect|null} The parent's bounding rect or null if not found
 */
export function getParentBoundingRect(elementId) {
  const element = document.getElementById(elementId);
  if (!element || !element.parentElement) return null;
  return element.parentElement.getBoundingClientRect();
}

/**
 * Focuses an element by ID
 * @param {string} elementId - The element ID
 */
export function focusElement(elementId) {
  return new Promise((resolve) => {
    requestAnimationFrame(() => {
      const element = document.getElementById(elementId);
      if (!element) {
        resolve(false);
        return;
      }

      element.focus({ preventScroll: true });
      resolve(document.activeElement === element);
    });
  });
}

/**
 * Prevents browser scrolling for arrow keys handled by an ARIA gridcell.
 * @param {HTMLElement} gridElement - The grid container
 */
export function initializeGridNavigationGuard(gridElement) {
  if (!gridElement || initializedGridNavigationGuards.has(gridElement)) {
    return;
  }

  gridElement.addEventListener("keydown", (event) => {
    const target = event.target;
    const isGridCell =
      target instanceof Element && target.getAttribute("role") === "gridcell";
    const isNavigationKey =
      event.key === "ArrowLeft" ||
      event.key === "ArrowRight" ||
      event.key === "ArrowUp" ||
      event.key === "ArrowDown";

    if (isGridCell && isNavigationKey) {
      event.preventDefault();
    }
  });

  initializedGridNavigationGuards.add(gridElement);
}

/**
 * Prevents the browser default for Enter/ArrowUp/ArrowDown while a combobox-style
 * control is expanded, so those keys drive the popup instead of submitting a form
 * or moving the caret. CSP-safe replacement for an inline onkeydown attribute.
 * @param {HTMLElement} element - The input element whose aria-expanded is read
 */
export function initializeExpandedKeyGuard(element) {
  if (!element || initializedExpandedKeyGuards.has(element)) {
    return;
  }

  element.addEventListener("keydown", (event) => {
    const isNavigationKey =
      event.key === "Enter" ||
      event.key === "ArrowDown" ||
      event.key === "ArrowUp";

    if (isNavigationKey && element.getAttribute("aria-expanded") === "true") {
      event.preventDefault();
    }
  });

  initializedExpandedKeyGuards.add(element);
}

/**
 * Scrolls an element into view
 * @param {string} elementId - The element ID
 * @param {object} options - ScrollIntoView options
 */
export function scrollIntoView(
  elementId,
  options = { behavior: "smooth", block: "nearest" },
) {
  const element = document.getElementById(elementId);
  if (element) element.scrollIntoView(options);
}

/**
 * Gets the scroll position of an element
 * @param {string} elementId - The element ID
 * @returns {object|null} {scrollTop, scrollLeft} or null
 */
export function getScrollPosition(elementId) {
  const element = document.getElementById(elementId);
  if (!element) return null;
  return { scrollTop: element.scrollTop, scrollLeft: element.scrollLeft };
}

/**
 * Sets the scroll position of an element
 * @param {string} elementId - The element ID
 * @param {number} top - Scroll top position
 * @param {number} left - Scroll left position
 */
export function setScrollPosition(elementId, top, left) {
  const element = document.getElementById(elementId);
  if (element) {
    element.scrollTop = top;
    element.scrollLeft = left;
  }
}

// Export for global access (fallback)
window.VibeDom = {
  getBoundingRect,
  getParentBoundingRect,
  focusElement,
  initializeGridNavigationGuard,
  scrollIntoView,
  getScrollPosition,
  setScrollPosition,
};
