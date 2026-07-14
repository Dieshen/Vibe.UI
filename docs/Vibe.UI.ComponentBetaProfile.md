# Vibe.UI component beta profile

This document defines the component behavior promised by `Vibe.UI`
`1.0.0-beta`. It narrows beta to a testable contract while stable 1.0 continues
toward complete shadcn/ui compatibility.

## Supported consumption models

The same package and source-installed components are release-gated in:

- standalone Blazor WebAssembly;
- Blazor Web App static SSR;
- Interactive Server;
- Interactive WebAssembly; and
- Interactive Auto.

The CLI must put CSS ownership in the host for a hosted client/server solution
and avoid duplicate client assets. Package validation builds both topologies
from the locally packed candidate packages.

## Public behavior contract

For documented beta components:

1. rendered examples must use real, compiling public APIs;
2. bindable parameters must expose the matching `Changed` callback;
3. disabled and read-only states must prevent mutation where applicable;
4. interactive controls must have programmatic names and visible focus;
5. overlays must support Escape and restore focus where the pattern requires it;
6. keyboard-oriented widgets must implement their documented navigation keys;
7. callbacks must fire once with the documented value; and
8. `Class` and unmatched attributes must reach the intended root or control.

## Release-gated depth

The beta gate prioritizes common and high-risk flows:

- buttons, inputs, textarea, select, checkbox, radio, switch, and validation;
- accordion, collapsible, tabs, carousel, and sheet/drawer behavior;
- dialog, alert dialog, popover, tooltip, and basic dropdown/context menu behavior;
- calendar, date picker, and date-range binding and keyboard selection;
- command, data table, toast/sonner, and representative advanced components;
- theme switching and CSS utility integration; and
- CLI/package use in standalone and hosted applications.

Advanced menu submenus, full NavigationMenu hover intent, universal typeahead,
and feature-for-feature shadcn behavior are stable 1.0 matrix work unless a
component page explicitly documents and tests them in beta.

## Evidence model

- Every one of the 111 source Razor files has a direct unit-test file.
- Route smoke tests cover every routed component catalog page and reject the
  NotFound shell.
- Focused browser tests cover behavior that bUnit cannot prove, including focus,
  layout, JS interop, and computed CSS.
- Axe serious/critical checks and reviewed visual baselines cover representative
  high-risk surfaces, not every component.

Direct test-file coverage is inventory evidence, not proof that every component
has equal behavioral or visual depth.

## Outside the beta claim

- formal WCAG certification;
- complete manual screen-reader, 200% zoom, and forced-colors evidence;
- reviewed visual baselines for every component and browser/platform pair;
- normalized public APIs across all legacy components; and
- complete shadcn/ui feature parity.

These remain explicit stable 1.0 work in
[the shadcn compatibility roadmap](Vibe.UI.ShadcnParity.md).
