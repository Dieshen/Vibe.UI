namespace Vibe.UI.CSS.Scanner;

/// <summary>
/// Reserved Vibe.UI component and runtime tokens that share the utility prefix
/// but are not generated utility classes.
/// </summary>
internal static class VibeComponentClassManifest
{
    internal static IReadOnlySet<string> Classes { get; } =
        new HashSet<string>(StringComparer.Ordinal)
        {
            "vibe-button-disabled",
            "vibe-checkbox-indeterminate",
            "vibe-color-picker-alpha-slider",
            "vibe-color-picker-disabled",
            "vibe-color-picker-hue-slider",
            "vibe-color-picker-open",
            "vibe-datepicker",
            "vibe-dialog-actions",
            "vibe-dialog-button",
            "vibe-dialog-button-primary",
            "vibe-dialog-button-secondary",
            "vibe-dialog-close-disabled",
            "vibe-dialog-close-icon",
            "vibe-dialog-form",
            "vibe-dialog-input",
            "vibe-dialog-message",
            "vibe-dialog-trigger-disabled",
            "vibe-form-field",
            "vibe-image-cropper-empty",
            "vibe-image-cropper-with-image",
            "vibe-input-error-state",
            "vibe-input-has-value",
            "vibe-kanban-disabled",
            "vibe-kanban-readonly",
            "vibe-menu-open",
            "vibe-skeleton-animated",
            "vibe-slider-error",
            "vibe-switch-error",
            "vibe-tag-input",
            "vibe-tag-label",
            "vibe-tag-removable",
            "vibe-theme",
            "vibe-virtual-scroll-empty",
        };
}
