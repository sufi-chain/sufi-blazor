using SufiChain.SufiBlazor.Demo.Localization;
using SufiChain.SufiAbp.UI.Navigation;

namespace SufiChain.SufiBlazor.Demo.Menus;

/// <summary>
/// Menu contributor for Sufi Blazor Demo pages.
/// </summary>
public class SufiBlazorDemoMenuContributor : IMenuContributor
{
    public Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main)
        {
            return Task.CompletedTask;
        }

        var demoMenu = context.Menu.GetDemo();
        var l = context.GetLocalizer<SufiBlazorDemoResource>();

        var sufiBlazorDemo = new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.GroupName,
            displayName: l["Menu:Demo:SufiBlazorDemo"],
            url: "/demo/sufi-blazor",
            icon: "palette",
            order: 1
        )
        {
            IsCollapsed = false // Children and sub-children start collapsed on load
        };

        // Introduction (landing page)
        sufiBlazorDemo.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Introduction, l["Menu:Demo:Introduction"], "/demo/sufi-blazor", "home", order: 0));
        // Getting Started
        sufiBlazorDemo.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.GettingStarted, l["Menu:Demo:GettingStarted"], "/demo/sufi-blazor/getting-started", "rocket", order: 1));

        // Actions
        var actionsGroup = new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.Actions,
            displayName: l["Menu:Demo:Actions"],
            icon: "play",
            order: 2
        );
        actionsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Button, l["Menu:Demo:Button"], "/demo/sufi-blazor/button", order: 1));
        actionsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.IconButton, l["Menu:Demo:IconButton"], "/demo/sufi-blazor/icon-button", order: 2));
        actionsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Link, l["Menu:Demo:Link"], "/demo/sufi-blazor/link", order: 3));
        sufiBlazorDemo.AddItem(actionsGroup);

        // Forms
        var formsGroup = new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.Forms,
            displayName: l["Menu:Demo:Forms"],
            icon: "edit",
            order: 3
        );
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.TextField, l["Menu:Demo:TextField"], "/demo/sufi-blazor/text-field", order: 1));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.TextArea, l["Menu:Demo:TextArea"], "/demo/sufi-blazor/text-area", order: 2));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.NumberField, l["Menu:Demo:NumberField"], "/demo/sufi-blazor/number-field", order: 3));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Select, l["Menu:Demo:Select"], "/demo/sufi-blazor/select", order: 4));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.MultiSelect, l["Menu:Demo:MultiSelect"], "/demo/sufi-blazor/multi-select", order: 5));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Autocomplete, l["Menu:Demo:Autocomplete"], "/demo/sufi-blazor/autocomplete", order: 6));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Checkbox, l["Menu:Demo:Checkbox"], "/demo/sufi-blazor/checkbox", order: 7));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Radio, l["Menu:Demo:Radio"], "/demo/sufi-blazor/radio", order: 8));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Switch, l["Menu:Demo:Switch"], "/demo/sufi-blazor/switch", order: 9));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DatePicker, l["Menu:Demo:DatePicker"], "/demo/sufi-blazor/date-picker", order: 10));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.TimePicker, l["Menu:Demo:TimePicker"], "/demo/sufi-blazor/time-picker", order: 11));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.ColorPicker, l["Menu:Demo:ColorPicker"], "/demo/sufi-blazor/color-picker", order: 12));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Slider, l["Menu:Demo:Slider"], "/demo/sufi-blazor/slider", order: 13));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.TagInput, l["Menu:Demo:TagInput"], "/demo/sufi-blazor/tag-input", order: 14));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.FileUpload, l["Menu:Demo:FileUpload"], "/demo/sufi-blazor/file-upload", order: 15));
        formsGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.RichTextEditor, l["Menu:Demo:RichTextEditor"], "/demo/sufi-blazor/rich-text-editor", order: 16));
        sufiBlazorDemo.AddItem(formsGroup);

        // Data - 3 levels: Data → Data Grid (with children) + Table, Pagination, Stat Card
        var dataGroup = new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.Data,
            displayName: l["Menu:Demo:Data"],
            icon: "table",
            order: 4
        );
        var dataGridParent = new ApplicationMenuItem(SufiBlazorDemoMenus.DataGrid, l["Menu:Demo:DataGrid"], "/demo/sufi-blazor/data-grid", order: 1);
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGrid + ".Overview", l["Menu:Demo:Overview"], "/demo/sufi-blazor/data-grid", order: 0));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridBasic, l["Menu:Demo:Basic"], "/demo/sufi-blazor/data-grid/basic", order: 1));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridSelection, l["Menu:Demo:Selection"], "/demo/sufi-blazor/data-grid/selection", order: 2));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridStyled, l["Menu:Demo:StripedAndBordered"], "/demo/sufi-blazor/data-grid/styled", order: 3));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridCompact, l["Menu:Demo:Compact"], "/demo/sufi-blazor/data-grid/compact", order: 4));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridEmpty, l["Menu:Demo:EmptyState"], "/demo/sufi-blazor/data-grid/empty", order: 5));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridDetail, l["Demo:DataGridExpandableRows"], "/demo/sufi-blazor/data-grid/detail", order: 6));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridServer, l["Demo:DataGridServerSide"], "/demo/sufi-blazor/data-grid/server", order: 7));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridCellTemplates, l["Demo:DataGridCellTemplates"], "/demo/sufi-blazor/data-grid/cell-templates", order: 8));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridVirtualization, l["Demo:DataGridVirtualization"], "/demo/sufi-blazor/data-grid/virtualization", order: 9));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridColumnFilter, l["Demo:DataGridColumnFilter"], "/demo/sufi-blazor/data-grid/column-filter", order: 10));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridProgrammaticFilter, l["Demo:DataGridProgrammaticFilter"], "/demo/sufi-blazor/data-grid/programmatic-filter", order: 11));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridInlineEdit, l["Demo:DataGridInlineEdit"], "/demo/sufi-blazor/data-grid/inline-edit", order: 12));
        dataGridParent.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.DataGridExportCsv, l["Demo:DataGridExportCsv"], "/demo/sufi-blazor/data-grid/export-csv", order: 13));
        dataGroup.AddItem(dataGridParent);
        dataGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Table, l["Menu:Demo:Table"], "/demo/sufi-blazor/table", order: 2));
        dataGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Pagination, l["Menu:Demo:Pagination"], "/demo/sufi-blazor/pagination", order: 3));
        dataGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.StatCard, l["Menu:Demo:StatCard"], "/demo/sufi-blazor/stat-card", order: 4));
        sufiBlazorDemo.AddItem(dataGroup);

        // Layout
        var layoutGroup = new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.Layout,
            displayName: l["Menu:Demo:Layout"],
            icon: "component",
            order: 5
        );
        layoutGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Stack, l["Menu:Demo:Stack"], "/demo/sufi-blazor/stack", order: 1));
        layoutGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Grid, l["Menu:Demo:Grid"], "/demo/sufi-blazor/grid", order: 2));
        layoutGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Container, l["Menu:Demo:Container"], "/demo/sufi-blazor/container", order: 3));
        layoutGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Spacer, l["Menu:Demo:Spacer"], "/demo/sufi-blazor/spacer", order: 4));
        sufiBlazorDemo.AddItem(layoutGroup);

        // Navigation
        var navigationGroup = new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.Navigation,
            displayName: l["Menu:Demo:Navigation"],
            icon: "compass",
            order: 6
        );
        navigationGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Tabs, l["Menu:Demo:Tabs"], "/demo/sufi-blazor/tabs", order: 1));
        navigationGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Stepper, l["Menu:Demo:Stepper"], "/demo/sufi-blazor/stepper", order: 2));
        navigationGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Breadcrumb, l["Menu:Demo:Breadcrumb"], "/demo/sufi-blazor/breadcrumb", order: 3));
        navigationGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Accordion, l["Menu:Demo:Accordion"], "/demo/sufi-blazor/accordion", order: 4));
        navigationGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.NavMenu, l["Menu:Demo:NavMenu"], "/demo/sufi-blazor/nav-menu", order: 5));
        navigationGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.TreeView, l["Menu:Demo:TreeView"], "/demo/sufi-blazor/tree-view", order: 6));
        sufiBlazorDemo.AddItem(navigationGroup);

        // Overlays
        var overlaysGroup = new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.Overlays,
            displayName: l["Menu:Demo:Overlays"],
            icon: "layers",
            order: 7
        );
        overlaysGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Dialog, l["Menu:Demo:Dialog"], "/demo/sufi-blazor/dialog", order: 1));
        overlaysGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Drawer, l["Menu:Demo:Drawer"], "/demo/sufi-blazor/drawer", order: 2));
        overlaysGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.ConfirmDialog, l["Menu:Demo:ConfirmDialog"], "/demo/sufi-blazor/confirm-dialog", order: 3));
        overlaysGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Popover, l["Menu:Demo:Popover"], "/demo/sufi-blazor/popover", order: 4));
        overlaysGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Tooltip, l["Menu:Demo:Tooltip"], "/demo/sufi-blazor/tooltip", order: 5));
        overlaysGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Menu, l["Menu:Demo:Menu"], "/demo/sufi-blazor/menu", order: 6));
        sufiBlazorDemo.AddItem(overlaysGroup);

        // Feedback
        var feedbackGroup = new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.Feedback,
            displayName: l["Menu:Demo:Feedback"],
            icon: "info",
            order: 8
        );
        feedbackGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Alert, l["Menu:Demo:Alert"], "/demo/sufi-blazor/alert", order: 1));
        feedbackGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Toast, l["Menu:Demo:Toast"], "/demo/sufi-blazor/toast", order: 2));
        feedbackGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Banner, l["Menu:Demo:Banner"], "/demo/sufi-blazor/banner", order: 3));
        feedbackGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Badge, l["Menu:Demo:Badge"], "/demo/sufi-blazor/badge", order: 4));
        feedbackGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Progress, l["Menu:Demo:Progress"], "/demo/sufi-blazor/progress", order: 5));
        feedbackGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Skeleton, l["Menu:Demo:Skeleton"], "/demo/sufi-blazor/skeleton", order: 6));
        feedbackGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.EmptyState, l["Menu:Demo:EmptyState"], "/demo/sufi-blazor/empty-state", order: 7));
        sufiBlazorDemo.AddItem(feedbackGroup);

        // Surfaces
        var surfacesGroup = new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.Surfaces,
            displayName: l["Menu:Demo:Surfaces"],
            icon: "maximize",
            order: 9
        );
        surfacesGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Card, l["Menu:Demo:Card"], "/demo/sufi-blazor/card", order: 1));
        surfacesGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Surface, l["Menu:Demo:Surface"], "/demo/sufi-blazor/surface", order: 2));
        surfacesGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Divider, l["Menu:Demo:Divider"], "/demo/sufi-blazor/divider", order: 3));
        sufiBlazorDemo.AddItem(surfacesGroup);

        // Typography & Common
        var typographyGroup = new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.Typography,
            displayName: l["Menu:Demo:Typography"],
            icon: "bold",
            order: 10
        );
        typographyGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Text, l["Menu:Demo:Text"], "/demo/sufi-blazor/text", order: 1));
        typographyGroup.AddItem(new ApplicationMenuItem(SufiBlazorDemoMenus.Icon, l["Menu:Demo:Icon"], "/demo/sufi-blazor/icon", order: 2));
        sufiBlazorDemo.AddItem(typographyGroup);

        demoMenu.AddItem(sufiBlazorDemo);

        // Error Handling (separate from SufiBlazor components, still under Demo)
        demoMenu.AddItem(new ApplicationMenuItem(
            name: SufiBlazorDemoMenus.ErrorHandling,
            displayName: l["Menu:Demo:ErrorHandling"],
            url: "/demo/error-handling",
            icon: "warning",
            order: 2
        ));

        return Task.CompletedTask;
    }
}
