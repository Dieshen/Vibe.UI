/**
 * Tests for docs.ts
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import './setup';
import {
  copyInstallCommand,
  togglePreviewTheme,
  showTab,
  showToast,
  copyCode
} from '../src/docs';
import {
  setupMockClipboard,
  getMockClipboard,
  createMockCopyButton,
  createMockMobileMenu,
  createMockTabs,
  createMockPreviewContent,
  getToasts,
  simulateClick
} from './mocks/dom.mock';

describe('docs', () => {
  beforeEach(() => {
    setupMockClipboard();
    document.body.innerHTML = '';
    document.head.innerHTML = '';
  });

  afterEach(() => {
    vi.clearAllTimers();
  });

  describe('copyInstallCommand', () => {
    it('should copy install command to clipboard', async () => {
      copyInstallCommand();

      const clipboard = getMockClipboard();
      expect(clipboard?.writeTextCalls).toHaveLength(1);
      expect(clipboard?.writeTextCalls[0]).toBe('dotnet tool install -g Vibe.UI.CLI');
    });

    it('should show success toast after copying', async () => {
      copyInstallCommand();

      // Wait for clipboard promise to resolve, then advance timers for toast animation
      await Promise.resolve(); // flush microtasks
      await vi.advanceTimersByTimeAsync(20); // allow toast animation to start

      const toasts = getToasts();
      expect(toasts.length).toBeGreaterThan(0);
      expect(toasts[0]?.textContent).toContain('Copied to clipboard!');
    });

    it('should show error toast on clipboard failure', async () => {
      const clipboard = getMockClipboard();
      if (clipboard) {
        clipboard.shouldFailWrite = true;
      }

      copyInstallCommand();

      // Wait for clipboard promise to reject, then advance timers for toast
      await Promise.resolve(); // flush microtasks
      await Promise.resolve(); // flush the catch handler
      await vi.advanceTimersByTimeAsync(20); // allow toast animation

      const toasts = getToasts();
      expect(toasts.length).toBeGreaterThan(0);
      expect(toasts[0]?.textContent).toContain('Failed to copy');
      expect(toasts[0]?.className).toContain('docs-toast-error');
    });
  });

  describe('togglePreviewTheme', () => {
    it('should toggle dark class on preview content', () => {
      const preview = createMockPreviewContent();

      expect(preview.classList.contains('dark')).toBe(false);

      togglePreviewTheme();
      expect(preview.classList.contains('dark')).toBe(true);

      togglePreviewTheme();
      expect(preview.classList.contains('dark')).toBe(false);
    });

    it('should handle missing preview content gracefully', () => {
      expect(() => togglePreviewTheme()).not.toThrow();
    });
  });

  describe('showTab', () => {
    it('should activate selected tab and panel', () => {
      const { tabs, panels } = createMockTabs();

      showTab('preview');

      const previewTab = tabs.find((t) => t.getAttribute('data-tab') === 'preview');
      const previewPanel = panels.find((p) => p.className.includes('preview-panel'));

      expect(previewTab?.classList.contains('active')).toBe(true);
      expect(previewPanel?.classList.contains('active')).toBe(true);
    });

    it('should deactivate other tabs and panels', () => {
      const { tabs, panels } = createMockTabs();

      // First activate preview
      showTab('preview');

      // Then activate code
      showTab('code');

      const previewPanel = panels.find((p) => p.className.includes('preview-panel'));
      const codePanel = panels.find((p) => p.className.includes('code-panel'));

      expect(previewPanel?.classList.contains('active')).toBe(false);
      expect(codePanel?.classList.contains('active')).toBe(true);
    });

    it('should handle non-existent tab gracefully', () => {
      createMockTabs();

      expect(() => showTab('non-existent')).not.toThrow();
    });
  });

  describe('showToast', () => {
    it('should create and show toast element', () => {
      showToast('Test message');

      vi.advanceTimersByTime(20);

      const toasts = getToasts();
      expect(toasts.length).toBe(1);
      expect(toasts[0]?.textContent).toBe('Test message');
    });

    it('should apply success class by default', () => {
      showToast('Success message');

      vi.advanceTimersByTime(20);

      const toasts = getToasts();
      expect(toasts[0]?.className).toContain('docs-toast-success');
    });

    it('should apply error class when specified', () => {
      showToast('Error message', 'error');

      vi.advanceTimersByTime(20);

      const toasts = getToasts();
      expect(toasts[0]?.className).toContain('docs-toast-error');
    });

    it('should remove previous toast before showing new one', () => {
      showToast('First message');
      vi.advanceTimersByTime(20);

      showToast('Second message');
      vi.advanceTimersByTime(20);

      const toasts = getToasts();
      expect(toasts.length).toBe(1);
      expect(toasts[0]?.textContent).toBe('Second message');
    });

    it('should show toast with animation', () => {
      showToast('Animated message');

      const toasts = getToasts();
      expect(toasts[0]?.classList.contains('show')).toBe(false);

      vi.advanceTimersByTime(20);
      expect(toasts[0]?.classList.contains('show')).toBe(true);
    });

    it('should auto-remove toast after 3 seconds', () => {
      showToast('Temporary message');

      vi.advanceTimersByTime(20);
      let toasts = getToasts();
      expect(toasts.length).toBe(1);

      // Advance to removal time (3000ms + 300ms animation)
      vi.advanceTimersByTime(3300);

      toasts = getToasts();
      expect(toasts.length).toBe(0);
    });
  });

  describe('copyCode', () => {
    it('should copy code to clipboard', () => {
      const button = createMockCopyButton('console.log("test");');

      copyCode(button);

      const clipboard = getMockClipboard();
      expect(clipboard?.writeTextCalls).toHaveLength(1);
      expect(clipboard?.writeTextCalls[0]).toBe('console.log("test");');
    });

    it('should update button text to "Copied!"', async () => {
      const button = createMockCopyButton('test code');

      copyCode(button);

      // Flush all microtasks - clipboard promise chain needs multiple ticks
      await vi.waitFor(() => {
        const textSpan = button.querySelector('span:last-child');
        if (textSpan?.textContent !== 'Copied!') {
          throw new Error('Not yet updated');
        }
      }, { timeout: 100 });

      const textSpan = button.querySelector('span:last-child');
      expect(textSpan?.textContent).toBe('Copied!');
    });

    it('should add copied class to button', async () => {
      const button = createMockCopyButton('test code');

      copyCode(button);

      // Flush all microtasks
      await vi.waitFor(() => {
        if (!button.classList.contains('copied')) {
          throw new Error('Not yet updated');
        }
      }, { timeout: 100 });

      expect(button.classList.contains('copied')).toBe(true);
    });

    it('should restore original text after 2 seconds', async () => {
      const button = createMockCopyButton('test code');
      const textSpan = button.querySelector('span:last-child');
      const originalText = textSpan?.textContent;

      copyCode(button);

      // Wait for the "Copied!" text to appear
      await vi.waitFor(() => {
        if (textSpan?.textContent !== 'Copied!') {
          throw new Error('Not yet updated');
        }
      }, { timeout: 100 });

      // Should be "Copied!"
      expect(textSpan?.textContent).toBe('Copied!');

      // Advance timers by 2 seconds
      vi.advanceTimersByTime(2000);

      // Should restore original text
      expect(textSpan?.textContent).toBe(originalText);
      expect(button.classList.contains('copied')).toBe(false);
    });

    it('should handle missing code block gracefully', () => {
      const button = document.createElement('button');
      document.body.appendChild(button);

      expect(() => copyCode(button)).not.toThrow();
    });

    it('should handle missing code element gracefully', () => {
      const button = document.createElement('button');
      const codeBlock = document.createElement('div');
      codeBlock.className = 'code-block';
      codeBlock.appendChild(button);
      document.body.appendChild(codeBlock);

      expect(() => copyCode(button)).not.toThrow();
    });
  });

  describe('mobile menu', () => {
    it('should toggle sidebar open class on menu button click', () => {
      const { sidebar, menuButton } = createMockMobileMenu();

      // Trigger DOMContentLoaded manually since we're in test env
      const event = new Event('DOMContentLoaded');
      document.dispatchEvent(event);

      expect(sidebar.classList.contains('open')).toBe(false);

      simulateClick(menuButton);
      expect(sidebar.classList.contains('open')).toBe(true);

      simulateClick(menuButton);
      expect(sidebar.classList.contains('open')).toBe(false);
    });

    it('should close sidebar when clicking outside', () => {
      const { sidebar, menuButton } = createMockMobileMenu();

      // Initialize
      const event = new Event('DOMContentLoaded');
      document.dispatchEvent(event);

      // Open sidebar
      simulateClick(menuButton);
      expect(sidebar.classList.contains('open')).toBe(true);

      // Click outside
      const outsideElement = document.createElement('div');
      document.body.appendChild(outsideElement);
      simulateClick(outsideElement);

      expect(sidebar.classList.contains('open')).toBe(false);
    });

    it('should not close sidebar when clicking inside', () => {
      const { sidebar, menuButton } = createMockMobileMenu();

      // Initialize
      const event = new Event('DOMContentLoaded');
      document.dispatchEvent(event);

      // Open sidebar
      simulateClick(menuButton);
      expect(sidebar.classList.contains('open')).toBe(true);

      // Click inside sidebar
      simulateClick(sidebar);

      expect(sidebar.classList.contains('open')).toBe(true);
    });
  });

  describe('initialization', () => {
    it('should inject toast styles on load', () => {
      const event = new Event('DOMContentLoaded');
      document.dispatchEvent(event);

      const styles = document.querySelector('style');
      expect(styles).toBeTruthy();
      expect(styles?.textContent).toContain('.docs-toast');
    });

    it('should log success message', () => {
      const consoleSpy = vi.spyOn(console, 'log');

      const event = new Event('DOMContentLoaded');
      document.dispatchEvent(event);

      expect(consoleSpy).toHaveBeenCalledWith(expect.stringContaining('Documentation loaded'));
    });
  });
});
