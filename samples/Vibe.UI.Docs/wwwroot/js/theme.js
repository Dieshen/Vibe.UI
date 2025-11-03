// Theme management for Vibe.UI Docs
window.vibeDocsTheme = {
    applyTheme: function(variables) {
        const root = document.documentElement;
        for (const [key, value] of Object.entries(variables)) {
            root.style.setProperty(key, value);
        }
    }
};
