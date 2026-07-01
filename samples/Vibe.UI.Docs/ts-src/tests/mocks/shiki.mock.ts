/**
 * Mock Shiki highlighter for testing
 */

import { vi } from 'vitest';

// Mock highlighter instance
export class MockShikiHighlighter {
  public highlightCalls: Array<{ code: string; lang: string; theme: string }> = [];

  codeToHtml(code: string, options: { lang: string; theme: string }): string {
    this.highlightCalls.push({ code, lang: options.lang, theme: options.theme });

    return `<pre class="shiki ${options.theme}"><code>${this.escapeHtml(code)}</code></pre>`;
  }

  private escapeHtml(text: string): string {
    return text
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#039;');
  }
}

// Mock Shiki bundle
let mockHighlighterInstance: MockShikiHighlighter | null = null;
let shouldFailInit = false;
let initAttempts = 0;

export const mockShikiBundle = {
  createHighlighter: vi.fn(
    async (_options: { themes: string[]; langs: string[] }): Promise<MockShikiHighlighter> => {
      initAttempts++;

      if (shouldFailInit) {
        throw new Error('Mock Shiki initialization failed');
      }

      await new Promise((resolve) => setTimeout(resolve, 10));

      mockHighlighterInstance = new MockShikiHighlighter();
      return mockHighlighterInstance;
    }
  )
};

// Setup function to mock the local Shiki web bundle import
export function setupShikiMock(): void {
  vi.doMock('shiki/bundle/web', () => mockShikiBundle);

  // Reset state
  mockHighlighterInstance = null;
  shouldFailInit = false;
  initAttempts = 0;
  mockShikiBundle.createHighlighter.mockClear();
}

// Teardown function
export function teardownShikiMock(): void {
  vi.doUnmock('shiki/bundle/web');
  mockHighlighterInstance = null;
  shouldFailInit = false;
  initAttempts = 0;
}

// Test helpers
export function getMockHighlighterInstance(): MockShikiHighlighter | null {
  return mockHighlighterInstance;
}

export function setShikiInitFailure(shouldFail: boolean): void {
  shouldFailInit = shouldFail;
}

export function getInitAttempts(): number {
  return initAttempts;
}

export function resetInitAttempts(): void {
  initAttempts = 0;
}

// Mock the Shiki import at module level
export function mockShikiImport(): void {
  // @ts-ignore - Mocking global import
  const originalImport = globalThis.import;

  // @ts-ignore - Mocking global import
  globalThis.import = vi.fn(async (url: string) => {
    if (url.includes('shiki')) {
      if (shouldFailInit && initAttempts < 3) {
        initAttempts++;
        throw new Error('Mock Shiki bundle load failed');
      }
      initAttempts++;
      await new Promise((resolve) => setTimeout(resolve, 10));
      mockHighlighterInstance = new MockShikiHighlighter();
      return mockShikiBundle;
    }
    return originalImport?.(url);
  });
}

export function restoreShikiImport(): void {
  // @ts-ignore - Restore original import
  delete globalThis.import;
}
