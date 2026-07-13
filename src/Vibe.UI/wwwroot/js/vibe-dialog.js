/**
 * Vibe.UI Dialog Utilities Module
 * Focus trap + scroll lock + focus restore for modal dialogs.
 */

const dialogStates = new Map();
const activeDialogKeys = [];

let bodyLockCount = 0;
let previousBodyOverflow = null;

function lockBodyScroll() {
  if (bodyLockCount === 0) {
    previousBodyOverflow = document.body.style.overflow || '';
    document.body.style.overflow = 'hidden';
  }
  bodyLockCount += 1;
}

function unlockBodyScroll() {
  bodyLockCount = Math.max(0, bodyLockCount - 1);
  if (bodyLockCount === 0 && previousBodyOverflow !== null) {
    document.body.style.overflow = previousBodyOverflow;
    previousBodyOverflow = null;
  }
}

function isFocusable(el) {
  if (!el) return false;
  if (el.hasAttribute('disabled')) return false;
  if (el.getAttribute('aria-disabled') === 'true') return false;
  const style = window.getComputedStyle(el);
  if (style.display === 'none' || style.visibility === 'hidden') return false;
  return true;
}

function getFocusableElements(container) {
  const candidates = container.querySelectorAll(
    'a[href], button, input, textarea, select, details, [tabindex]:not([tabindex="-1"])'
  );
  return Array.from(candidates).filter((el) => isFocusable(el));
}

function focusInitial(container) {
  const focusables = getFocusableElements(container);
  if (focusables.length > 0) {
    focusables[0].focus();
    return;
  }

  if (!container.hasAttribute('tabindex')) {
    container.setAttribute('tabindex', '-1');
  }
  container.focus();
}

/**
 * Activates focus management for a dialog instance.
 * @param {string} key - Stable key per dialog instance.
 * @param {HTMLElement} dialogElement - Root dialog element.
 */
export function activate(key, dialogElement, dotNetRef = null, closeOnEscape = false) {
  if (!dialogElement) return;
  if (dialogStates.has(key)) return;

  const prevFocused = document.activeElement instanceof HTMLElement ? document.activeElement : null;
  lockBodyScroll();

  const onKeyDown = (e) => {
    if (activeDialogKeys[activeDialogKeys.length - 1] !== key) return;

    if (e.key === 'Escape' && closeOnEscape && dotNetRef) {
      e.preventDefault();
      e.stopImmediatePropagation();
      dotNetRef.invokeMethodAsync('HandleDialogEscape');
      return;
    }

    if (e.key !== 'Tab') return;

    const focusables = getFocusableElements(dialogElement);
    if (focusables.length === 0) {
      e.preventDefault();
      dialogElement.focus();
      return;
    }

    const first = focusables[0];
    const last = focusables[focusables.length - 1];
    const active = document.activeElement;

    if (e.shiftKey) {
      if (active === first || !dialogElement.contains(active)) {
        e.preventDefault();
        last.focus();
      }
      return;
    }

    if (active === last) {
      e.preventDefault();
      first.focus();
    }
  };

  // Use capture so the trap runs even if inner elements stop propagation.
  document.addEventListener('keydown', onKeyDown, true);

  dialogStates.set(key, { prevFocused, onKeyDown });
  activeDialogKeys.push(key);

  // Let the DOM settle before focusing.
  setTimeout(() => focusInitial(dialogElement), 0);
}

/**
 * Deactivates focus management for a dialog instance.
 * @param {string} key - Stable key per dialog instance.
 */
export function deactivate(key) {
  const state = dialogStates.get(key);
  if (!state) return;

  document.removeEventListener('keydown', state.onKeyDown, true);
  const activeIndex = activeDialogKeys.lastIndexOf(key);
  if (activeIndex >= 0) {
    activeDialogKeys.splice(activeIndex, 1);
  }
  unlockBodyScroll();

  if (state.prevFocused && document.contains(state.prevFocused)) {
    setTimeout(() => state.prevFocused.focus(), 0);
  }

  dialogStates.delete(key);
}

// Global access fallback
window.VibeDialog = { activate, deactivate };

