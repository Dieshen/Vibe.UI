/**
 * Vibe.UI rich text editor DOM helpers.
 */

export function executeCommand(editor, command, value) {
    if (!editor || typeof document.execCommand !== 'function') {
        return false;
    }

    if (typeof editor.focus === 'function') {
        editor.focus();
    }

    return document.execCommand(command, false, value ?? null);
}

export function promptForUrl(message) {
    return window.prompt(message);
}

export function getHtml(editor) {
    return editor?.innerHTML ?? '';
}
