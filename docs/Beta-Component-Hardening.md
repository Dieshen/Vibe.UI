# Beta Component Hardening Tracker

This tracker defines the beta confidence bar for component hardening. It is intentionally stricter than "the solution builds" and separates source coverage, browser smoke coverage, and follow-up test quality.

## Target

- Source component set: 110 Razor components in `src/Vibe.UI/Components`.
- Beta hardening target: at least 80% direct unit-test coverage across source components.
- Numeric target: 88 of 110 components with direct component tests.
- Current direct unit coverage: 110 of 110 components, or 100.0%.
- Remaining direct unit tests needed for 80%: 0 components.
- Current docs browser smoke coverage: 53 component routes.
- Browser smoke target: every top-level documented component page must stay in `Category=Smoke`.

Direct unit coverage means a component has an explicit `*Tests.cs` file under `tests/Vibe.UI.Tests/Components`. Browser smoke coverage means the docs route is included in `tests/Vibe.UI.Docs.E2E/Tests/Smoke/AllComponentsRenderTests.cs`.

## Current Category Snapshot

| Category | Total | Direct unit tests | Docs smoke routes |
| --- | ---: | ---: | ---: |
| Advanced | 4 | 4 | 3 |
| DataDisplay | 8 | 8 | 7 |
| DateTime | 3 | 3 | 3 |
| Disclosure | 5 | 5 | 3 |
| Feedback | 9 | 9 | 4 |
| Form | 7 | 7 | 3 |
| Inputs | 23 | 23 | 11 |
| Layout | 12 | 12 | 7 |
| Navigation | 14 | 14 | 6 |
| Overlay | 17 | 17 | 4 |
| Theme | 2 | 2 | 1 |
| Utility | 6 | 6 | 1 |

## 80% Target Status

The 80% direct component unit-test target is complete at 110 of 110 components.

Post-target hardening can continue with deeper browser coverage for JS-heavy components, accessibility-focused interaction checks, and docs parity for top-level component routes.

## Depth-Hardening Progress

Direct test-file coverage is complete, but production hardening is tracked separately. A depth-hardened component has focused assertions for meaningful behavior, accessibility semantics, disabled/read-only states, callback contracts, null/edge inputs, and browser or JS boundaries where applicable.

Depth-hardened components completed on the beta-readiness branch:

- Advanced: KanbanBoard, TreeView, TreeViewNode, VirtualScroll.
- DateTime: Calendar, DatePicker, DateRangePicker.
- DataDisplay: Avatar, Badge, Chart, DataTable, Progress, Table, Tag, Timeline.
- Disclosure: AccordionItem, Carousel.
- Feedback: Alert, Confetti, EmptyState, NotificationCenter, Skeleton, Sonner, Spinner, Toast, ToastContainer.
- Inputs: ColorPicker, FileUpload, ImageCropper, InputOTP, Mentions, RadioGroupItem, RichTextEditor, TransferList.
- Layout: AspectRatio, Card, Container, Sheet.
- Navigation: BreadcrumbItem, Link, Menu, NavigationMenu, NavigationMenuItem, Pagination, Sidebar, Stepper, TabItem, Tabs.
- Overlay: AlertDialog, ContextMenu, ContextMenuItem.
- Utility: Command, DropdownMenu, Icon.

Current depth-hardened count on this branch: 54 components.

Next priority depth candidates:

- Disclosure: Accordion, CarouselItem, Collapsible.
- Form: Combobox, Form, FormField, FormLabel.
- Inputs: Button, Checkbox, Input, Select, Switch.

## Mutation-Style Quality Checks

For each new component test file, prefer assertions that would catch these breakages:

- Required classes or ARIA attributes disappear.
- Disabled/open/selected state is inverted.
- Event callbacks stop firing or fire with the wrong value.
- Null or omitted optional content throws.
- Child content stops rendering.
- Keyboard or focus state regressions for interactive components.
- JS interop paths are not invoked or cleanup paths stop running.

## Browser Coverage Rules

- Top-level docs pages stay covered by `Category=Smoke`.
- Hosting compatibility stays covered by `Category=Compatibility` across standalone WebAssembly and Blazor Web App static/server/client/auto paths.
- Components with browser-only behavior, JS interop, timers, drag/drop, or focus management should get either a docs smoke route or targeted E2E coverage in addition to unit tests.
