/**
 * Tests for shiki-interop.ts
 */

import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import './setup';
import { normalizeLanguage, escapeHtml, WEB_BUNDLE_LANGUAGES } from '../src/shiki-interop';
import {
  mockShikiImport,
  restoreShikiImport,
  getMockHighlighterInstance,
  setShikiInitFailure
} from './mocks/shiki.mock';

describe('shiki-interop', () => {
  beforeEach(() => {
    mockShikiImport();
    // Clear any existing highlighter state
    vi.clearAllMocks();
  });

  afterEach(() => {
    restoreShikiImport();
    vi.clearAllTimers();
  });

  describe('normalizeLanguage', () => {
    it('should normalize common aliases', () => {
      expect(normalizeLanguage('js')).toBe('javascript');
      expect(normalizeLanguage('ts')).toBe('typescript');
      expect(normalizeLanguage('md')).toBe('markdown');
    });

    it('should map C# variants to TypeScript', () => {
      expect(normalizeLanguage('cs')).toBe('typescript');
      expect(normalizeLanguage('csharp')).toBe('typescript');
      expect(normalizeLanguage('c#')).toBe('typescript');
    });

    it('should map Razor variants to HTML', () => {
      expect(normalizeLanguage('razor')).toBe('html');
      expect(normalizeLanguage('cshtml')).toBe('html');
      expect(normalizeLanguage('blazor')).toBe('html');
    });

    it('should map shell variants to JavaScript', () => {
      expect(normalizeLanguage('sh')).toBe('javascript');
      expect(normalizeLanguage('shell')).toBe('javascript');
      expect(normalizeLanguage('bash')).toBe('javascript');
    });

    it('should map PowerShell variants to JavaScript', () => {
      expect(normalizeLanguage('ps')).toBe('javascript');
      expect(normalizeLanguage('ps1')).toBe('javascript');
      expect(normalizeLanguage('powershell')).toBe('javascript');
    });

    it('should handle case insensitivity', () => {
      expect(normalizeLanguage('JavaScript')).toBe('javascript');
      expect(normalizeLanguage('TYPESCRIPT')).toBe('typescript');
      expect(normalizeLanguage('Html')).toBe('html');
    });

    it('should return web bundle languages unchanged', () => {
      WEB_BUNDLE_LANGUAGES.forEach((lang) => {
        expect(normalizeLanguage(lang)).toBe(lang);
      });
    });

    it('should fallback to javascript for unknown languages', () => {
      expect(normalizeLanguage('foobar')).toBe('javascript');
      expect(normalizeLanguage('unknown')).toBe('javascript');
    });

    it('should handle empty string', () => {
      expect(normalizeLanguage('')).toBe('text');
    });
  });

  describe('escapeHtml', () => {
    it('should escape HTML characters', () => {
      const input = '<div class="test">Hello & "World"</div>';
      const result = escapeHtml(input);

      expect(result).toContain('&lt;');
      expect(result).toContain('&gt;');
      expect(result).not.toContain('<div');
    });

    it('should handle special characters', () => {
      const input = '& < > " \'';
      const result = escapeHtml(input);

      expect(result).not.toContain('<');
      expect(result).not.toContain('>');
    });

    it('should handle empty string', () => {
      expect(escapeHtml('')).toBe('');
    });

    it('should handle plain text', () => {
      const input = 'Hello World';
      expect(escapeHtml(input)).toBe(input);
    });
  });

  // NOTE: Tests in 'highlightCode' are skipped because they require browser-level Shiki
  // initialization semantics that are currently covered by E2E tests.
  // These scenarios are covered by E2E tests that run in the browser.
  describe.skip('highlightCode (requires browser environment)', () => {
    it('should highlight code with default settings', async () => {
      const code = 'const x = 42;';
      const result = await window.highlightCode(code, 'javascript', 'light');

      expect(result).toContain('<pre');
      expect(result).toContain('const x = 42;');
    });

    it('should use correct theme', async () => {
      const code = 'const x = 42;';

      const lightResult = await window.highlightCode(code, 'javascript', 'light');
      expect(lightResult).toContain('github-light');

      const darkResult = await window.highlightCode(code, 'javascript', 'dark');
      expect(darkResult).toContain('github-dark');
    });

    it('should normalize language before highlighting', async () => {
      const code = 'const x = 42;';

      // Test alias normalization
      await window.highlightCode(code, 'js', 'light');
      const instance = getMockHighlighterInstance();
      expect(instance?.highlightCalls[0]?.lang).toBe('javascript');
    });

    it('should fallback to escaped HTML on error', async () => {
      setShikiInitFailure(true);

      const code = '<div>Test</div>';
      const result = await window.highlightCode(code, 'html', 'light');

      expect(result).toContain('<pre><code>');
      expect(result).toContain('&lt;div&gt;');
      expect(result).not.toContain('<div>');
    });

    it('should handle empty code', async () => {
      const result = await window.highlightCode('', 'javascript', 'light');

      expect(result).toBeDefined();
      expect(typeof result).toBe('string');
    });

    it('should log highlighting details', async () => {
      const consoleSpy = vi.spyOn(console, 'log');

      await window.highlightCode('test', 'javascript', 'light');

      expect(consoleSpy).toHaveBeenCalledWith(
        expect.stringContaining('[Shiki] Highlighting')
      );
    });
  });

  // NOTE: All tests below require browser-level Shiki initialization semantics.
  // These scenarios are covered by E2E tests that run in the browser.
  describe.skip('isHighlighterReady (requires browser environment)', () => {
    it('should return false initially', () => {
      expect(window.isHighlighterReady()).toBe(false);
    });

    it('should return true after initialization', async () => {
      await window.highlightCode('test', 'javascript', 'light');

      // After first highlight, should be ready
      expect(window.isHighlighterReady()).toBe(true);
    });
  });

  describe.skip('preloadHighlighter (requires browser environment)', () => {
    it('should initialize highlighter', async () => {
      const result = await window.preloadHighlighter();

      expect(result).toBe(true);
      expect(window.isHighlighterReady()).toBe(true);
    });

    it('should return false on initialization failure', async () => {
      setShikiInitFailure(true);

      const result = await window.preloadHighlighter();

      expect(result).toBe(false);
    });

    it('should be idempotent', async () => {
      await window.preloadHighlighter();
      await window.preloadHighlighter();

      // Should only initialize once
      expect(window.isHighlighterReady()).toBe(true);
    });
  });

  describe.skip('waitForHighlighter (requires browser environment)', () => {
    it('should wait for highlighter to be ready', async () => {
      // Preload in background
      window.preloadHighlighter();

      const result = await window.waitForHighlighter(1000);

      expect(result).toBe(true);
      expect(window.isHighlighterReady()).toBe(true);
    });

    it('should timeout if highlighter fails', async () => {
      setShikiInitFailure(true);

      const result = await window.waitForHighlighter(500);

      expect(result).toBe(false);
    });

    it('should return immediately if already ready', async () => {
      await window.preloadHighlighter();

      const startTime = Date.now();
      const result = await window.waitForHighlighter(5000);
      const duration = Date.now() - startTime;

      expect(result).toBe(true);
      expect(duration).toBeLessThan(100);
    });

    it('should use default timeout', async () => {
      const result = await window.waitForHighlighter();

      expect(result).toBe(true);
    });
  });

  describe.skip('auto-preload (requires browser environment)', () => {
    it('should attempt to preload after timeout', async () => {
      // Fast-forward past auto-preload timer
      vi.advanceTimersByTime(1000);

      // Wait for async operations
      await new Promise((resolve) => setTimeout(resolve, 100));

      // Should have attempted preload (may fail if mock not set up)
      expect(console.log).toHaveBeenCalledWith(
        expect.stringContaining('[Shiki] Initializing')
      );
    });
  });

  describe.skip('initialization retry logic (requires browser environment)', () => {
    it('should retry initialization on failure', async () => {
      let attemptCount = 0;

      // Mock import to fail twice then succeed
      // @ts-ignore
      globalThis.import = vi.fn(async (url: string) => {
        if (url.includes('shiki')) {
          attemptCount++;
          if (attemptCount < 3) {
            throw new Error('Load failed');
          }
          // Return success on third attempt
          const { mockShikiBundle } = await import('./mocks/shiki.mock');
          return mockShikiBundle;
        }
      });

      await window.highlightCode('test', 'javascript', 'light');

      expect(attemptCount).toBeGreaterThanOrEqual(1);
    });

    it('should fail after max attempts', async () => {
      setShikiInitFailure(true);

      const result = await window.highlightCode('test', 'javascript', 'light');

      // Should fallback to escaped HTML
      expect(result).toContain('<pre><code>');
      expect(console.error).toHaveBeenCalledWith(
        expect.stringContaining('[Shiki] Highlight error')
      );
    });
  });
});
