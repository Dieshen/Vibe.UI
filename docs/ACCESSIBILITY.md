# Accessibility

Vibe.UI components are built around native HTML semantics, explicit accessible
names, keyboard interaction, visible focus, and state communication. The beta
test suite directly exercises all 110 source components, including focused ARIA
and keyboard assertions for interactive components.

This is an implementation guide, not a claim of formal WCAG certification.
Automated component and browser tests do not replace manual keyboard, screen
reader, zoom, contrast, and forced-colors evaluation in a consuming application.

## Core rules

### Prefer native elements

Use native controls when they provide the required behavior:

```razor
<button type="button">Save</button>
<input type="email" />
<select>...</select>
```

Custom interaction patterns must expose the equivalent role, state, accessible
name, and keyboard behavior. Examples in the library include `Combobox`,
`Command`, `MultiSelect`, and `TransferList`.

### Provide an accessible name

Interactive controls need a visible label, `aria-label`, or
`aria-labelledby`. Icon-only buttons use an explicit label and hide decorative
icons from assistive technology.

Dialogs prefer a visible title:

```html
<div role="dialog"
     aria-modal="true"
     aria-labelledby="dialog-title"
     aria-describedby="dialog-description">
```

When no visible title is available, components expose an `AriaLabel` parameter
or provide a conservative fallback.

### Communicate validation state

Input-like components compose validation attributes rather than replacing
consumer-provided descriptions:

```html
<input aria-invalid="true"
       aria-errormessage="email-error"
       aria-describedby="email-error email-help" />
```

The shared behavior is:

- `aria-invalid="true"` identifies an invalid control.
- `aria-errormessage` points to the active error message when supported.
- `aria-describedby` combines error, helper, and consumer-provided IDs.
- Visible error styling uses `--vibe-destructive`.
- Error text uses `role="alert"` or an appropriate live-region policy.

`Input`, `TextArea`, `Select`, `Checkbox`, `Radio`, `Combobox`,
`ValidatedInput`, and `FormField` have direct tests for these contracts.

### Use visible focus

Interactive components use `:focus-visible` so keyboard focus remains obvious
without forcing pointer-only focus decoration:

```css
.component:focus-visible {
    outline: 2px solid var(--vibe-ring);
    outline-offset: 2px;
}
```

Do not remove outlines unless an equal or stronger visible focus treatment is
provided. Focus colors must continue to work against both light and dark theme
tokens.

### Keep disabled behavior consistent

Native controls use `disabled`. Link-like or custom controls use
`aria-disabled="true"`, prevent activation, and leave a clear visual state:

```css
.component:disabled,
.component[aria-disabled="true"] {
    cursor: not-allowed;
    opacity: 0.5;
}
```

`aria-disabled` alone does not suppress events. Component logic must prevent the
action as well.

## Keyboard interaction

Keyboard behavior follows the interaction model of the rendered role:

- Buttons activate with Enter and Space.
- Menus, tabs, listboxes, and composite widgets manage directional navigation.
- Escape closes dismissible overlays when their `CloseOnEscape` option is enabled.
- Tab order remains predictable; roving-tabindex widgets expose one active item.
- Custom draggable or resizable controls provide keyboard alternatives where implemented.

Representative keyboard coverage lives under
`tests/Vibe.UI.Tests/Components`, including `TabsTests`, `MenuTests`,
`ComboboxTests`, `TooltipTests`, `DialogTests`, and `ResizableTests`.

## Modal focus management

`Dialog` and `AlertDialog` load `wwwroot/js/vibe-dialog.js`. The module:

- records the previously focused element;
- locks body scrolling while one or more dialogs are active;
- moves initial focus into the dialog;
- traps Tab and Shift+Tab within the dialog;
- restores focus when the dialog closes; and
- removes listeners and scroll locks during deactivation.

The module exports `activate(key, dialogElement)` and `deactivate(key)`. Package
components import it from `./_content/Vibe.UI/js/vibe-dialog.js`; CLI source
installations rewrite component imports to the copied `./js/` path.

Escape handling remains in the component so `CloseOnEscape` and state callbacks
stay under Blazor control.

## Live updates

Dynamic content uses the least disruptive announcement policy that still
communicates the change:

- `role="status"` or `aria-live="polite"` for progress and non-urgent updates;
- `role="alert"` or `aria-live="assertive"` for errors and urgent feedback;
- no live region for decorative motion unless the consumer opts in.

Examples include `FormMessage`, `Toast`, `ToastContainer`, `Sonner`,
`NotificationCenter`, `Spinner`, `FileUpload`, `Calendar`, and `Carousel`.

## Component author checklist

Before merging an interactive component:

1. Confirm the native element or ARIA role matches the behavior.
2. Provide an accessible name and wire descriptions with stable IDs.
3. Expose selected, expanded, invalid, checked, busy, or disabled state as applicable.
4. Implement the expected keyboard model, including Escape for dismissible overlays.
5. Add a visible `:focus-visible` treatment.
6. Prevent callbacks and state changes while disabled or read-only.
7. Clean up JavaScript listeners, timers, observers, and focus traps.
8. Add direct component tests for semantics, keyboard behavior, callbacks, and edge states.
9. Add browser coverage for behavior that depends on real focus, layout, or JavaScript.

## Validation

Run the component suite:

```bash
dotnet test tests/Vibe.UI.Tests/Vibe.UI.Tests.csproj --configuration Release
```

Run docs browser coverage after building the solution and installing Playwright:

```bash
BROWSER=chromium dotnet test tests/Vibe.UI.Docs.E2E/Vibe.UI.Docs.E2E.csproj \
  --configuration Release --no-build --filter Category=Accessibility
```

The browser accessibility suite runs Axe against six flagship component pages
in light and dark themes, scans the open AlertDialog state, and directly checks
focus trapping/restoration, programmatic labels, roving tab focus, alert
semantics, and mobile viewport fit. Serious or critical Axe violations fail the
build. This remains an automated baseline rather than formal WCAG certification.

Hosting-specific browser coverage is documented in [Compatibility](Compatibility.md).
Screenshot comparison and baseline-update policy are documented in
[Visual regression](VISUAL-REGRESSION.md).
The component coverage policy is documented in
[Beta component hardening](Beta-Component-Hardening.md).
