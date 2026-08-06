# SufiBlazor Components

Feature-oriented catalog. See **SufiChain.SufiBlazor.Demo** and component XML docs for API details.

## Actions

- **SbButton** — Variants (solid, outline, ghost, link), colors, sizes, loading, full-width. Optional `Href` for link style. Start/end icons.  
- **SbIconButton** — Icon-only; use `AriaLabel`.  
- **SbLink** — Styled link.

## Forms

- **SbTextField** — Label, placeholder, type (text, password, etc.), required, error, helper text, clearable, adornments.  
- **SbTextArea** — Multi-line.  
- **SbSelect** / **SbSelectOption** — Single or multiple; options from markup or collection.  
- **SbTagAutocomplete** — Multi-select autocomplete with chips; search, create-on-type, max tags. See [SbTagAutocomplete](components/forms/SbTagAutocomplete.md).  
- **SbCheckbox**, **SbRadio**, **SbSwitch** — Booleans.  
- **SbDatePicker**, **SbTimePicker** — Date/time.  
- **SbRichTextEditor**, **SbMarkdownEditor** — Rich text and markdown with on-demand vendor JS.  
- **SbMarkEditor** — Unified markdown/markup/JSON editor over `SbMarkdownEditor`. See [SbMarkEditor](components/forms/SbMarkEditor.md).  
- **SbForm** / **SbFormField** — Form wrapper and labeled field with validation.

## Data

- **SbDataGrid** — See [DataGrid](data-grid.md).  
- **SbTable** — Simple table markup.  
- **SbPagination** — Page index/size, total count, optional page-size selector.  
- **SbChart** — SVG line, bar, donut, and sparkline charts (no third-party JS). See [SbChart](components/data/SbChart.md).

## Maps

OpenStreetMap / Leaflet map surfaces.

- **SbMap** — Interactive map canvas (tiles, center, zoom, click).  
- **SbMapMarker** — Pin with optional accuracy circle; nest inside `SbMap`.  
- **SbMapPreview** — Compact read-only location card (chat bubbles, CRM).  
- **SbMapPicker** — Dialog to search, geolocate, click, or drag a pin.  
- **SbGeolocateButton** — Browser geolocation capture control.  
- **SbPlaceSearch** — Nominatim place autocomplete.

## Conversation

Chat-like UI primitives (domain-agnostic).

- **SbConversationComposer** — In-field composer with start/end/overflow action slots. See [SbConversationComposer](components/conversation/SbConversationComposer.md).  
- **SbConversationTimeline** — Message list with loading, empty, and thinking states. See [SbConversationTimeline](components/conversation/SbConversationTimeline.md).  
- **SbConversationMessage** — Single bubble with badge, meta, body, and footer slots. See [SbConversationMessage](components/conversation/SbConversationMessage.md).

## Layout

- **SbStack** — Flex row/column, gap, justify, align.  
- **SbGrid** — CSS grid; column counts, gap.  
- **SbContainer**, **SbSpacer** — Max-width container and flex spacer.

> App shell chrome (`SbAppShell`, `SbSidebar`, etc.) was removed from SufiBlazor. Use **SufiTheme** for layout shells. Details: [Architecture decisions](architecture/decisions.md).

## Builder

Visual editor and page-builder primitives. See [Builder Components](builder.md).

- **SbDraggableItem** / **SbDropZone** / **SbDragHandle** — HTML5 drag-and-drop palette and canvas.  
- **SbSortableList\<T\>** — Drag-to-reorder lists (layers, tabs, tasks).  
- **SbInspectorPanel** / **SbInspectorSection** — Collapsible property sidebar.  
- **SbPropertyGrid** — Auto-generated property editors (Text, Number, Checkbox, Select, Color).  
- **SbSplitPane** / **SbResizable** — Resizable canvas + inspector layout.  
- **SbInlineToolbar** — Floating context actions on selection.  
- **SbAddButton** — Add-block affordance with optional dropdown.

## Navigation

- **SbTabs** / **SbTab** — Tabbed content.  
- **SbBreadcrumb** / **SbBreadcrumbItem** — Breadcrumbs.  
- **SbNavMenu** / **SbNavItem** — Hierarchical nav; used in SufiTheme layouts.

## Overlays

- **SbDialog** — Modal; title, body, footer, size, close behavior.  
- **SbDrawer** — Slide-out panel; placement.  
- **SbPopover**, **SbTooltip** — Floating content.  
- **SbMenu** / **SbMenuItem** — Dropdown.

## Feedback

- **SbAlert** — Severity, dismissible, optional title.  
- **SbToast** / **SbToastHost** — Toasts; use service or local state.  
- **SbProgress** — Linear or circular; determinate or indeterminate.  
- **SbSkeleton** — Loading placeholders.  
- **SbBadge**, **SbChip** — Labels and tags.

## Surfaces & Typography

- **SbCard** — Header, body, footer; optional clickable.  
- **SbSurface**, **SbDivider**.  
- **SbHeading** (levels 1–6), **SbText** (variants, alignment).
