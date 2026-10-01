using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbToggleGroupTests : BunitContext
{
    private IRenderedComponent<SbToggleGroup<string>> RenderGroup(
        Action<ComponentParameterCollectionBuilder<SbToggleGroup<string>>>? configure = null)
    {
        return Render<SbToggleGroup<string>>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<SbToggleItem<string>>(0);
                builder.AddAttribute(1, "Value", "a");
                builder.AddAttribute(2, "Text", "Alpha");
                builder.CloseComponent();
                builder.OpenComponent<SbToggleItem<string>>(3);
                builder.AddAttribute(4, "Value", "b");
                builder.AddAttribute(5, "Text", "Beta");
                builder.CloseComponent();
                builder.OpenComponent<SbToggleItem<string>>(6);
                builder.AddAttribute(7, "Value", "c");
                builder.AddAttribute(8, "Text", "Gamma");
                builder.CloseComponent();
            });
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersItemsAndLabel()
    {
        var cut = RenderGroup(p => p.Add(x => x.Label, "Choose"));

        Assert.Equal(3, cut.FindAll(".sb-toggle-item").Count);
        Assert.Contains("Choose", cut.Find(".sb-toggle-group__label").TextContent);
        Assert.Equal("group", cut.Find(".sb-toggle-group").GetAttribute("role"));
    }

    [Fact]
    public void MarksTheSingleSelectedItem()
    {
        var cut = RenderGroup(p => p.Add(x => x.Value, "b"));

        var selected = cut.FindAll(".sb-toggle-item")
            .Single(button => button.ClassList.Contains("sb-toggle-item--selected"));
        Assert.Contains("Beta", selected.TextContent);
        Assert.Equal("true", selected.GetAttribute("aria-pressed"));
    }

    [Fact]
    public async Task SingleSelectionReplacesTheCurrentValue()
    {
        string? received = "a";
        var cut = RenderGroup(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.SelectionMode, SbToggleSelectionMode.SingleSelection)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, value => received = value)));

        await cut.InvokeAsync(() => cut.FindAll(".sb-toggle-item")[1].Click());

        Assert.Equal("b", received);
    }

    [Fact]
    public async Task SingleSelectionKeepsTheCurrentItemSelected()
    {
        var calls = 0;
        var cut = RenderGroup(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, _ => calls++)));

        await cut.InvokeAsync(() => cut.FindAll(".sb-toggle-item")[0].Click());

        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task ToggleSelectionClearsTheCurrentValue()
    {
        string? received = "a";
        var changed = false;
        var cut = RenderGroup(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.SelectionMode, SbToggleSelectionMode.ToggleSelection)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, value =>
            {
                received = value;
                changed = true;
            })));

        await cut.InvokeAsync(() => cut.FindAll(".sb-toggle-item")[0].Click());

        Assert.True(changed);
        Assert.Null(received);
    }

    [Fact]
    public async Task MultiSelectionAddsAndRemovesValues()
    {
        IReadOnlyList<string?> received = new List<string?> { "a" };
        var cut = RenderGroup(p => p
            .Add(x => x.SelectionMode, SbToggleSelectionMode.MultiSelection)
            .Add(x => x.Values, received)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IEnumerable<string?>?>(this, values =>
            {
                received = values?.ToList() ?? [];
            })));

        await cut.InvokeAsync(() => cut.FindAll(".sb-toggle-item")[1].Click());
        Assert.Equal(new string?[] { "a", "b" }, received);

        cut.Render(p => p
            .Add(x => x.SelectionMode, SbToggleSelectionMode.MultiSelection)
            .Add(x => x.Values, received)
            .Add(x => x.ValuesChanged, EventCallback.Factory.Create<IEnumerable<string?>?>(this, values =>
            {
                received = values?.ToList() ?? [];
            }))
            .AddChildContent(Items));

        await cut.InvokeAsync(() => cut.FindAll(".sb-toggle-item")[0].Click());
        Assert.Equal(new string?[] { "b" }, received);
    }

    [Fact]
    public void CustomContentReceivesSelectionState()
    {
        var cut = Render<SbToggleGroup<string>>(p => p
            .Add(x => x.Value, "pro")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SbToggleItem<string>>(0);
                builder.AddAttribute(1, "Value", "basic");
                builder.AddAttribute(2, "ChildContent", (RenderFragment<bool>)(selected => b =>
                    b.AddContent(0, selected ? "basic-on" : "basic-off")));
                builder.CloseComponent();
                builder.OpenComponent<SbToggleItem<string>>(3);
                builder.AddAttribute(4, "Value", "pro");
                builder.AddAttribute(5, "ChildContent", (RenderFragment<bool>)(selected => b =>
                    b.AddContent(0, selected ? "pro-on" : "pro-off")));
                builder.CloseComponent();
            }));

        var buttons = cut.FindAll(".sb-toggle-item");
        Assert.Contains("basic-off", buttons[0].TextContent);
        Assert.Contains("pro-on", buttons[1].TextContent);
    }

    [Fact]
    public void AppliesSelectedClassOnlyToTheSelectedItem()
    {
        var cut = RenderGroup(p => p
            .Add(x => x.Value, "c")
            .Add(x => x.SelectedClass, "sb-toggle-selected-gradient"));

        var buttons = cut.FindAll(".sb-toggle-item");
        Assert.DoesNotContain("sb-toggle-selected-gradient", buttons[0].ClassList);
        Assert.Contains("sb-toggle-selected-gradient", buttons[2].ClassList);
        Assert.Contains("sb-toggle-item--selected", buttons[2].ClassList);
    }

    [Fact]
    public void RendersCheckMarkOnTheSelectedItem()
    {
        var cut = RenderGroup(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.CheckMark, true));

        var buttons = cut.FindAll(".sb-toggle-item");
        Assert.Contains("sb-toggle-item__mark", buttons[0].InnerHtml);
        Assert.Contains("sb-icon", buttons[0].InnerHtml);
        Assert.DoesNotContain("sb-icon", buttons[1].InnerHtml);
    }

    [Fact]
    public async Task DisabledGroupDoesNotChangeTheValue()
    {
        var calls = 0;
        var cut = RenderGroup(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.Disabled, true)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, _ => calls++)));

        await cut.InvokeAsync(() => cut.FindAll(".sb-toggle-item")[1].Click());

        Assert.Equal(0, calls);
        Assert.Contains("sb-toggle-group--disabled", cut.Find(".sb-toggle-group").ClassList);
    }

    [Fact]
    public void AppliesColorSizeAndOrientationClasses()
    {
        var cut = RenderGroup(p => p
            .Add(x => x.Color, SbColor.Success)
            .Add(x => x.Size, SbSize.Sm)
            .Add(x => x.Orientation, SbOrientation.Vertical)
            .Add(x => x.FullWidth, true));

        var group = cut.Find(".sb-toggle-group");
        Assert.Contains("sb-toggle-group--success", group.ClassList);
        Assert.Contains("sb-toggle-group--sm", group.ClassList);
        Assert.Contains("sb-toggle-group--vertical", group.ClassList);
        Assert.Contains("sb-toggle-group--full", group.ClassList);
    }

    private static void Items(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenComponent<SbToggleItem<string>>(0);
        builder.AddAttribute(1, "Value", "a");
        builder.AddAttribute(2, "Text", "Alpha");
        builder.CloseComponent();
        builder.OpenComponent<SbToggleItem<string>>(3);
        builder.AddAttribute(4, "Value", "b");
        builder.AddAttribute(5, "Text", "Beta");
        builder.CloseComponent();
        builder.OpenComponent<SbToggleItem<string>>(6);
        builder.AddAttribute(7, "Value", "c");
        builder.AddAttribute(8, "Text", "Gamma");
        builder.CloseComponent();
    }
}
