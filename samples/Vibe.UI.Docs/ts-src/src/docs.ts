/* ==========================================================================
   Vibe.UI Documentation Site JavaScript
   ========================================================================== */

// Type definitions for toast types
export type ToastType = 'success' | 'error';

// Interface for docs utilities
export interface DocsUtils {
  copyInstallCommand(): void;
  togglePreviewTheme(): void;
  showTab(tabName: string): void;
  showToast(message: string, type?: ToastType): void;
  copyCode(button: HTMLElement): void;
}

/**
 * Copy Install Command
 */
function copyInstallCommand(): void {
  const command = 'dotnet tool install -g Vibe.UI.CLI --version 1.0.0-beta';

  navigator.clipboard
    .writeText(command)
    .then(() => {
      showToast('Copied to clipboard!');
    })
    .catch((err) => {
      console.error('Failed to copy:', err);
      showToast('Failed to copy', 'error');
    });
}

/**
 * Toggle Preview Theme
 */
function togglePreviewTheme(): void {
  const preview = document.querySelector('.preview-content');
  if (preview) {
    preview.classList.toggle('dark');
  }
}

/**
 * Show Tab
 */
function showTab(tabName: string): void {
  const tabs = document.querySelectorAll('.preview-tab');
  const panels = document.querySelectorAll('.preview-panel, .code-panel');

  tabs.forEach((tab) => tab.classList.remove('active'));
  panels.forEach((panel) => panel.classList.remove('active'));

  // Find and activate the clicked tab (using event.target would require event parameter)
  const activeTab = document.querySelector(`.preview-tab[data-tab="${tabName}"]`);
  if (activeTab) {
    activeTab.classList.add('active');
  }

  const panel = document.querySelector(`.${tabName}-panel`);
  if (panel) {
    panel.classList.add('active');
  }
}

/**
 * Toast Notification
 */
function showToast(message: string, type: ToastType = 'success'): void {
  // Remove existing toast if present
  const existingToast = document.querySelector('.docs-toast');
  if (existingToast) {
    existingToast.remove();
  }

  const toast = document.createElement('div');
  toast.className = `docs-toast docs-toast-${type}`;
  toast.textContent = message;
  document.body.appendChild(toast);

  // Trigger animation
  setTimeout(() => toast.classList.add('show'), 10);

  // Remove after 3 seconds
  setTimeout(() => {
    toast.classList.remove('show');
    setTimeout(() => toast.remove(), 300);
  }, 3000);
}

/**
 * Copy Code Button
 */
function copyCode(button: HTMLElement): void {
  const codeBlock = button.closest('.code-block');
  if (!codeBlock) return;

  const codeElement = codeBlock.querySelector('code');
  if (!codeElement) return;

  const code = codeElement.textContent || '';

  navigator.clipboard.writeText(code).then(() => {
    button.classList.add('copied');
    const textSpan = button.querySelector('span:last-child');
    if (textSpan) {
      const originalText = textSpan.textContent || '';
      textSpan.textContent = 'Copied!';

      setTimeout(() => {
        button.classList.remove('copied');
        textSpan.textContent = originalText;
      }, 2000);
    }
  });
}

/**
 * Toast Styles (injected dynamically)
 */
const toastStyles = `
.docs-toast {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  padding: 1rem 1.5rem;
  background: var(--docs-surface);
  border: 1px solid var(--docs-border);
  border-radius: 0.5rem;
  box-shadow: 0 10px 15px -3px rgb(0 0 0 / 0.1),
              0 4px 6px -4px rgb(0 0 0 / 0.1);
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--docs-text);
  opacity: 0;
  transform: translateX(100%);
  transition: all 0.3s ease;
  z-index: 1000;
}

.docs-toast.show {
  opacity: 1;
  transform: translateX(0);
}

.docs-toast-success {
  border-left: 4px solid #22c55e;
}

.docs-toast-error {
  border-left: 4px solid #ef4444;
}

@media (max-width: 768px) {
  .docs-toast {
    left: 1rem;
    right: 1rem;
    bottom: 1rem;
  }
}
`;

/**
 * Inject toast styles
 */
function injectToastStyles(): void {
  const styleSheet = document.createElement('style');
  styleSheet.textContent = toastStyles;
  document.head.appendChild(styleSheet);
}

/**
 * Initialize mobile menu
 */
function initializeMobileMenu(): void {
  const mobileMenuBtn = document.querySelector('.mobile-menu-btn');
  const sidebar = document.querySelector('.docs-sidebar');

  if (mobileMenuBtn && sidebar) {
    mobileMenuBtn.addEventListener('click', () => {
      sidebar.classList.toggle('open');
    });

    // Close sidebar when clicking outside
    document.addEventListener('click', (e: MouseEvent) => {
      const target = e.target as Node;
      if (!sidebar.contains(target) && !mobileMenuBtn.contains(target)) {
        sidebar.classList.remove('open');
      }
    });
  }
}

/**
 * Initialize documentation utilities
 */
function initialize(): void {
  injectToastStyles();
  console.log('Vibe.UI Documentation loaded ✨');
}

// DOM Content Loaded - Initialize
document.addEventListener('DOMContentLoaded', () => {
  initialize();
  initializeMobileMenu();
});

// Export functions for global access and testing
const docsUtils: DocsUtils = {
  copyInstallCommand,
  togglePreviewTheme,
  showTab,
  showToast,
  copyCode
};

// Make functions available globally
if (typeof window !== 'undefined') {
  (window as typeof window & DocsUtils) = Object.assign(window, docsUtils);
}

export { copyInstallCommand, togglePreviewTheme, showTab, showToast, copyCode };
export default docsUtils;
