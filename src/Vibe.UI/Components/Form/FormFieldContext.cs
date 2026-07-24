namespace Vibe.UI.Components;

internal sealed record FormFieldContext(
    string ControlId,
    string? DescribedBy,
    string? ErrorId,
    bool IsInvalid,
    bool IsRequired);
