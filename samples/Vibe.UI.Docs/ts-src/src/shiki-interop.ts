/**
 * Shiki Syntax Highlighting Interop
 * Provides VSCode-quality syntax highlighting for code blocks
 */

import { createHighlighter as createShikiHighlighter } from 'shiki/bundle/web';

// Theme type
export type Theme = 'light' | 'dark';

// Type definitions for Shiki
interface ShikiHighlighter {
  codeToHtml(code: string, options: {
    lang: string;
    themes: Record<Theme, string>;
    defaultColor: Theme;
  }): string;
}

// Supported languages (type-safe)
export type SupportedLanguage =
  | 'html'
  | 'css'
  | 'javascript'
  | 'typescript'
  | 'json'
  | 'markdown'
  | 'yaml'
  | 'xml'
  | 'text';

// Extend window interface
declare global {
  interface Window {
    highlightCode(code: string, language: string, theme: string): Promise<string>;
    isHighlighterReady(): boolean;
    preloadHighlighter(): Promise<boolean>;
    waitForHighlighter(timeoutMs?: number): Promise<boolean>;
  }
}

// Shiki highlighter instance (lazy loaded)
let highlighter: ShikiHighlighter | null = null;
let highlighterPromise: Promise<ShikiHighlighter> | null = null;
let initializationAttempts = 0;
const MAX_INIT_ATTEMPTS = 3;

// Languages included in the Shiki web bundle
// See: https://shiki.style/languages for full list
// The web bundle includes: html, css, javascript, typescript, json, markdown, yaml, xml
// Languages NOT in web bundle that we need to handle: csharp, razor, powershell, bash
const WEB_BUNDLE_LANGUAGES: readonly SupportedLanguage[] = [
  'html',
  'css',
  'javascript',
  'typescript',
  'json',
  'markdown',
  'yaml',
  'xml'
] as const;

// Theme mapping
const THEMES: Record<Theme, string> = {
  light: 'github-light',
  dark: 'github-dark'
};

/**
 * Initialize the Shiki highlighter with retry logic
 */
async function initHighlighter(): Promise<ShikiHighlighter> {
  if (highlighter) return highlighter;
  if (highlighterPromise) return highlighterPromise;

  const pendingHighlighter = (async (): Promise<ShikiHighlighter> => {
    while (initializationAttempts < MAX_INIT_ATTEMPTS) {
      try {
        initializationAttempts++;
        console.log(`[Shiki] Initializing highlighter (attempt ${initializationAttempts})...`);

        highlighter = (await createShikiHighlighter({
          themes: [THEMES.light, THEMES.dark],
          langs: [...WEB_BUNDLE_LANGUAGES]
        })) as ShikiHighlighter;

        console.log('[Shiki] Highlighter initialized successfully with local web bundle languages');
        return highlighter;
      } catch (error) {
        console.error(`[Shiki] Initialization attempt ${initializationAttempts} failed:`, error);
        if (initializationAttempts >= MAX_INIT_ATTEMPTS) {
          throw error;
        }
        // Wait before retry
        await new Promise((resolve) => setTimeout(resolve, 500));
      }
    }

    throw new Error('[Shiki] Failed to initialize after maximum attempts');
  })();

  highlighterPromise = pendingHighlighter;

  try {
    return await pendingHighlighter;
  } catch (error) {
    highlighterPromise = null;
    initializationAttempts = 0;
    throw error;
  }
}

/**
 * Normalize and map language names to web bundle equivalents
 */
function normalizeLanguage(language: string): SupportedLanguage {
  // Handle empty/null/undefined
  if (!language || language.trim() === '') {
    return 'text';
  }

  const lang = language.toLowerCase();

  // Handle common aliases
  if (lang === 'js') return 'javascript';
  if (lang === 'ts') return 'typescript';
  if (lang === 'md') return 'markdown';
  if (lang === 'text' || lang === 'plaintext' || lang === 'txt') return 'text';

  // Languages NOT in web bundle - map to closest alternative
  if (lang === 'cs' || lang === 'csharp' || lang === 'c#') return 'typescript'; // C# -> TypeScript (similar syntax)
  if (lang === 'razor' || lang === 'cshtml' || lang === 'blazor') return 'html'; // Razor -> HTML (best match for markup)
  if (lang === 'sh' || lang === 'shell' || lang === 'bash') return 'javascript'; // Shell -> JS (basic highlighting)
  if (lang === 'ps' || lang === 'ps1' || lang === 'powershell') return 'javascript'; // PowerShell -> JS

  // Check if language is in web bundle
  if (!WEB_BUNDLE_LANGUAGES.includes(lang as SupportedLanguage)) {
    console.log(`[Shiki] Language '${lang}' not in web bundle, using 'javascript' as fallback`);
    return 'javascript'; // Default fallback with decent syntax highlighting
  }

  return lang as SupportedLanguage;
}

/**
 * Highlight code with Shiki
 * @param code - The code to highlight
 * @param language - The programming language
 * @param theme - 'light' or 'dark'
 * @returns HTML string with highlighted code
 */
window.highlightCode = async function (
  code: string,
  language: string,
  _theme: string
): Promise<string> {
  try {
    const hl = await initHighlighter();

    // Normalize language name
    const lang = normalizeLanguage(language);
    console.log(`[Shiki] Highlighting as '${lang}' with dual themes`);

    const html = hl.codeToHtml(code, {
      lang: lang,
      themes: THEMES,
      defaultColor: 'light'
    });

    return html;
  } catch (error) {
    console.error('[Shiki] Highlight error:', error);
    // Fallback to escaped HTML
    return `<pre><code>${escapeHtml(code)}</code></pre>`;
  }
};

/**
 * Check if highlighter is ready
 */
window.isHighlighterReady = function (): boolean {
  return highlighter !== null;
};

/**
 * Pre-initialize the highlighter (call early for faster first highlight)
 */
window.preloadHighlighter = async function (): Promise<boolean> {
  try {
    await initHighlighter();
    return true;
  } catch {
    return false;
  }
};

/**
 * Wait for highlighter to be ready with timeout
 * @param timeoutMs - Maximum time to wait in milliseconds
 * @returns True if ready, false if timeout
 */
window.waitForHighlighter = async function (timeoutMs = 5000): Promise<boolean> {
  const startTime = Date.now();

  while (Date.now() - startTime < timeoutMs) {
    try {
      await initHighlighter();
      return true;
    } catch {
      // If init fails, wait a bit and check again
      await new Promise((resolve) => setTimeout(resolve, 100));
    }
  }

  return highlighter !== null;
};

/**
 * Escape HTML characters for fallback display
 */
function escapeHtml(text: string): string {
  const div = document.createElement('div');
  div.textContent = text;
  return div.innerHTML;
}

// Auto-preload highlighter when script loads (non-blocking)
setTimeout(() => {
  window.preloadHighlighter().catch(() => {
    // Ignore preload errors - will retry on first use
  });
}, 1000);

// Export for testing
export {
  initHighlighter,
  normalizeLanguage,
  escapeHtml,
  WEB_BUNDLE_LANGUAGES,
  THEMES,
  MAX_INIT_ATTEMPTS
};
