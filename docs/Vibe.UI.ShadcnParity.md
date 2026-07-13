# Vibe.UI shadcn/ui Parity Roadmap

Goal: make `Vibe.UI` feel like “shadcn/ui for Blazor/.NET”: beautiful defaults, composable primitives, predictable APIs, and a copy‑into‑your‑app workflow that stays ergonomic as the library grows.

This document is a pragmatic checklist (phased) rather than a strict 1:1 port. Some shadcn/ui pieces map to `Vibe.UI.CSS` (utilities) and some map to `Vibe.UI` (components + patterns).

## Current Inventory (Repo Snapshot)

`src/Vibe.UI/Components` currently contains ~`110` Razor components grouped into:
- Layout, Inputs, Form, DataDisplay, Navigation, Overlay, Feedback, Disclosure, DateTime, Utility, Theme, Advanced

Examples present already:
- Core primitives: `Button`, `Card`, `Input`, `Tabs`, `Dialog`, `Popover`, `Tooltip`, `DropdownMenu`, `Toast`
- “Shadcn-ish” patterns: `Command`, `Menubar`, `NavigationMenu`, `AlertDialog`
- Extras beyond shadcn: `Chart`, `KanbanBoard`, `RichTextEditor`, `VirtualScroll`

## Phase 0 — Define “Parity” and Guardrails

This repo already made several “shadcn-style” decisions. Phase 0 is about documenting them explicitly (and closing the few remaining gaps), so new components and CLI installs stay consistent.

In this context:
- “Parity” means we deliver the same *developer experience* as shadcn/ui, not necessarily a 1:1 clone:
  - API feel: naming, variants/sizes, slot/composition patterns, defaults
  - UX/a11y: keyboard behavior, focus management, aria labeling, escape handling
  - Visual language: radii, spacing, shadows, typography, light/dark behavior
  - Workflow: the CLI-based “copy into your app” path stays the primary, ergonomic path
- “Guardrails” are the non-negotiable conventions that keep the above consistent:
  - theming contract (tokens + `.dark`)
  - component parameter patterns and class composition rules
  - JS usage policy
  - CLI install/update behavior and file layout conventions

### What we already have (today)

- Theming model: CSS variables (`--vibe-*`) + `.dark` class toggling (shadcn-like).
  - Source: `src/Vibe.UI/wwwroot/css/vibe-base.css` (tokens + reset) and component styles using `var(--vibe-*)`.
  - Runtime toggling: `src/Vibe.UI/wwwroot/js/vibe-theme.js` + `ThemeToggle`/`ThemeProvider` components.
- Styling approach: components ship with `.razor.css` and are designed to work with tokens out of the box.
- Component class/attrs model: common `Class` + `AdditionalAttributes` pattern via `src/Vibe.UI/Base/VibeComponent.cs` (`VibeComponent`).
- Copy/paste workflow (“shadcn for Blazor”):
  - `vibe init` copies infrastructure to `Vibe/` and CSS foundation files to `wwwroot/css/`.
  - `vibe add <component>` installs components (flat by default) and installs dependencies first.
  - Config stored in `vibe.json` (`ComponentsDirectory`, theme, etc.).
  - Source: `src/Vibe.UI.CLI/Commands/InitCommand.cs`, `src/Vibe.UI.CLI/Commands/AddCommand.cs`, `src/Vibe.UI.CLI/Services/ComponentService.cs`.

### Guardrails to document (so it stays consistent)

- Class handling: every component should expose `Class` and `AdditionalAttributes` and use `VibeComponent.CombineClasses(...)` or `CombinedClass` consistently.
- Naming conventions:
  - parameters: `Variant`, `Size`, `Disabled`, `Class`, `ChildContent` (where applicable)
  - enums: `XxxVariant`, `XxxSize` (or a shared pattern)
  - files: `Component.razor` + optional `Component.razor.css`
- Composition patterns:
  - overlays/menus: Root/Trigger/Content (you already have `DialogRoot`, `DialogTrigger`, etc.)
  - table/menu item composition: consistent slot patterns vs monolithic components
- JS policy:
  - minimal JS, only where needed (positioning, measuring, drag/resize); ship JS via CLI templates when required.
- CLI install contract:
  - “flat install by default” is the baseline; docs and examples should assume this.
  - dependency metadata must be accurate (CLI currently hardcodes component list + deps in `ComponentService`).

### Remaining decisions (worth making explicit)

- Are components primarily “styled components” (CSS in `.razor.css`) or should more move toward composition with `vibe-*` utilities?
- How strict should CLI updates be:
  - overwrite-only (current)
  - diff/patch workflow (future)
- What is the canonical “import story”:
  - `@using MyApp.Components.vibe` (current default) vs a different namespace/layout.

## Phase 1 — Core shadcn/ui Primitives (Highest adoption)

### Buttons and inputs (polish + completeness)
- [x] `Button` hardening: icon buttons, loading state, link behavior, and focus-ring consistency
- [x] `Input` hardening: disabled/invalid states, prefix/suffix slots, and consistent control heights
- [x] `Textarea` hardening: resize rules, consistent spacing, and error state
- [x] `Select` hardening: keyboard behavior and consistent item spacing
- [x] `Checkbox`/`Radio` hardening: hit targets, label alignment, and indeterminate state
- [x] `Switch` hardening: animations and disabled/checked styles

### Overlay primitives (composition model)
- [x] `Dialog`/`AlertDialog` hardening: focus trap, scroll lock, escape handling, and ARIA labeling
- [x] `Popover` hardening: collision handling and focus behavior
- [x] `Tooltip` hardening: delay, hover/focus behavior, and positioning
- [x] `DropdownMenu`/`ContextMenu` hardening: keyboard navigation, submenus, and typeahead

### Navigation primitives
- [x] `Tabs` hardening: keyboard navigation, disabled tabs, and vertical orientation
- [x] `Breadcrumb` hardening: separators and truncation
- [x] `Pagination` hardening: accessible labels and responsive behavior
- [x] `NavigationMenu` hardening: hover intent and focus management

## Phase 2 — shadcn/ui “Patterns” Components (Docs site feel)

These are often what makes a shadcn site feel like a shadcn site.

- [x] `Command` hardening: filtering, groups, shortcuts, empty state, and accessibility
- [x] `DataTable` hardening: sorting, filtering, pagination, and selection
- [x] `Toast/Sonner` hardening: stacking behavior, dismissal, and variants
- [x] `Accordion/Collapsible` hardening: keyboard behavior and ARIA
- [x] `Carousel` hardening: touch, snap, buttons, and dots
- [x] `Sheet/Drawer` hardening: placement variants, focus, and scrolling

## Phase 3 — Theming + Styling Model (Vibe.UI ↔ Vibe.UI.CSS contract)

- [x] Define the contract between Vibe.UI and Vibe.UI.CSS:
  - Vibe.UI uses semantic tokens (`--vibe-*`) and/or `vibe-*` utilities
  - minimal bespoke component CSS where possible
- [x] Apply the shared hardening contract across all components:
  - dark-theme styles
  - visible focus treatment
  - disabled/readonly behavior where applicable
- [ ] Complete reviewed light/dark visual coverage for all 110 components
- [ ] Add a “headless mode” guideline (optional): allow consumers to fully override classes

## Phase 4 — Copy/Paste Workflow (shadcn CLI parity)

shadcn/ui parity is as much workflow as it is visuals.

- [ ] CLI parity:
  - [x] `vibe add <component>` supports dependencies graph (adds required peers)
  - [ ] `vibe diff` and `vibe update` (optional but huge DX win)
  - [ ] component registry metadata (name, deps, files, docs links)
- [ ] Provide canonical “recipes” (patterns):
  - auth forms, settings pages, dashboards
  - modal + form flows
  - table + filters

## Phase 5 — Quality Bar (What makes it “shadcn quality”)

- [x] Automated accessibility tests for key components (Axe serious/critical, keyboard, focus, and ARIA); manual WCAG evaluation remains separate
- [x] Reviewed Chromium/Windows visual regression screenshots for six flagship surfaces in light/dark desktop/mobile states
- [ ] Performance checks for heavy components (DataTable, VirtualScroll, Chart)
- [ ] API consistency review (naming: `Variant`, `Size`, `Class`, slots)

## Suggested Next Steps

1. Add performance budgets for `DataTable`, `VirtualScroll`, and `Chart`.
2. Complete the public API naming and composition review across all 110 components.
3. Expand reviewed visual coverage beyond the six highest-risk surfaces and add manual screen-reader, zoom, and forced-colors release evidence.

