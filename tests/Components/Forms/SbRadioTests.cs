using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbRadioTests : BunitContext
{
    private IRenderedComponent<SbRadioGroup<string>> RenderRadioGroup(
        Action<ComponentParameterCollectionBuilder<SbRadioGroup<string>>>? configure = null)
    {
        return Render<SbRadioGroup<string>>(p =>
        {
            p.AddChildContent(builder =>
            {
                builder.OpenComponent<SbRadio<string>>(0);
                builder.AddAttribute(1, "Value", "a");
                builder.AddAttribute(2, "Label", "Option A");
                builder.CloseComponent();
                builder.OpenComponent<SbRadio<string>>(3);
                builder.AddAttribute(4, "Value", "b");
                builder.AddAttribute(5, "Label", "Option B");
                builder.CloseComponent();
                builder.OpenComponent<SbRadio<string>>(6);
                builder.AddAttribute(7, "Value", "c");
                builder.AddAttribute(8, "Label", "Option C");
                builder.CloseComponent();
            });
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersRadioStructure()
    {
        // Arrange & Act
        var cut = RenderRadioGroup();

        // Assert
        var radios = cut.FindAll("label.sb-radio");
        Assert.Equal(3, radios.Count);
        Assert.NotNull(cut.Find("input.sb-radio__input[type='radio']"));
        Assert.NotNull(cut.Find(".sb-radio__circle"));
        Assert.NotNull(cut.Find(".sb-radio__dot"));
        Assert.NotNull(cut.Find(".sb-radio__text"));
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderRadioGroup();

        // Assert
        var labels = cut.FindAll(".sb-radio__label");
        Assert.Contains(labels, l => l.TextContent.Contains("Option A"));
        Assert.Contains(labels, l => l.TextContent.Contains("Option B"));
    }

    [Fact]
    public void RendersCheckedClassWhenValueMatches()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.Value, "b"));

        // Assert
        var radios = cut.FindAll("label.sb-radio");
        var checkedRadio = radios.FirstOrDefault(r => r.ClassList.Contains("sb-radio--checked"));
        Assert.NotNull(checkedRadio);
        Assert.Contains("Option B", checkedRadio.TextContent);
    }

    [Fact]
    public void InputHasCheckedAttributeWhenSelected()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.Value, "a"));

        // Assert
        var inputs = cut.FindAll("input.sb-radio__input");
        var checkedInput = inputs.FirstOrDefault(i => i.GetAttribute("value") == "a");
        Assert.NotNull(checkedInput);
        Assert.NotNull(checkedInput.GetAttribute("checked"));
    }

    [Fact]
    public void InputsShareSameNameFromGroup()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.Name, "test-group"));

        // Assert
        var inputs = cut.FindAll("input[type='radio']");
        var name = inputs[0].GetAttribute("name");
        Assert.NotNull(name);
        Assert.All(inputs, i => Assert.Equal(name, i.GetAttribute("name")));
    }

    [Fact]
    public void RendersHelperTextWhenProvided()
    {
        // Arrange & Act
        var cut = Render<SbRadioGroup<string>>(p => p
            .Add(x => x.Value, "pro")
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SbRadio<string>>(0);
                builder.AddAttribute(1, "Value", "free");
                builder.AddAttribute(2, "Label", "Free");
                builder.AddAttribute(3, "HelperText", "Basic features");
                builder.CloseComponent();
                builder.OpenComponent<SbRadio<string>>(4);
                builder.AddAttribute(5, "Value", "pro");
                builder.AddAttribute(6, "Label", "Pro");
                builder.AddAttribute(7, "HelperText", "Advanced features");
                builder.CloseComponent();
            }));

        // Assert
        var helpers = cut.FindAll(".sb-radio__helper");
        Assert.Equal(2, helpers.Count);
        Assert.Contains(helpers, h => h.TextContent.Contains("Basic features"));
        Assert.Contains(helpers, h => h.TextContent.Contains("Advanced features"));
    }

    [Fact]
    public void RendersChildContentInsteadOfLabel()
    {
        // Arrange & Act
        var cut = Render<SbRadioGroup<string>>(p => p
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SbRadio<string>>(0);
                builder.AddAttribute(1, "Value", "custom");
                builder.AddAttribute(2, "ChildContent", (RenderFragment)(b => b.AddMarkupContent(0, "<strong>Custom option</strong>")));
                builder.CloseComponent();
            }));

        // Assert
        var labelSpan = cut.Find(".sb-radio__label");
        Assert.NotNull(labelSpan);
        Assert.Contains("Custom option", labelSpan.InnerHtml);
    }

    [Fact]
    public void AppliesDisabledClassWhenRadioDisabled()
    {
        // Arrange & Act
        var cut = Render<SbRadioGroup<string>>(p => p
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SbRadio<string>>(0);
                builder.AddAttribute(1, "Value", "enabled");
                builder.AddAttribute(2, "Label", "Enabled");
                builder.CloseComponent();
                builder.OpenComponent<SbRadio<string>>(3);
                builder.AddAttribute(4, "Value", "disabled");
                builder.AddAttribute(5, "Label", "Disabled");
                builder.AddAttribute(6, "Disabled", true);
                builder.CloseComponent();
            }));

        // Assert
        var radios = cut.FindAll("label.sb-radio");
        var disabledRadio = radios.FirstOrDefault(r => r.ClassList.Contains("sb-radio--disabled"));
        Assert.NotNull(disabledRadio);
        Assert.Contains("Disabled", disabledRadio.TextContent);
    }

    [Fact]
    public void AppliesDisabledClassWhenGroupDisabled()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.Disabled, true));

        // Assert
        var radios = cut.FindAll("label.sb-radio");
        Assert.All(radios, r => Assert.Contains("sb-radio--disabled", r.ClassList));
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = Render<SbRadioGroup<string>>(p => p
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SbRadio<string>>(0);
                builder.AddAttribute(1, "Value", "x");
                builder.AddAttribute(2, "Label", "X");
                builder.AddAttribute(3, "Class", "my-radio");
                builder.CloseComponent();
            }));

        // Assert
        var radio = cut.Find("label.sb-radio");
        Assert.Contains("my-radio", radio.ClassList);
    }

    [Fact]
    public void AppliesIdToInput()
    {
        // Arrange & Act
        var cut = Render<SbRadioGroup<string>>(p => p
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SbRadio<string>>(0);
                builder.AddAttribute(1, "Value", "x");
                builder.AddAttribute(2, "Label", "X");
                builder.AddAttribute(3, "Id", "radio-x");
                builder.CloseComponent();
            }));

        // Assert
        var input = cut.Find("input.sb-radio__input");
        Assert.Equal("radio-x", input.GetAttribute("id"));
    }

    [Fact]
    public async Task InvokesValueChangedWhenRadioSelected()
    {
        // Arrange
        string? received = null;
        var cut = RenderRadioGroup(p => p
            .Add(x => x.Value, "a")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, v => received = v)));

        // Act - select Option B via input change (radio uses @onchange)
        var inputB = cut.FindAll("input.sb-radio__input").First(i => i.GetAttribute("value") == "b");
        await cut.InvokeAsync(() => inputB!.Change("b"));

        // Assert
        Assert.Equal("b", received);
    }

    [Fact]
    public void RendersRadioGroupStructure()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.Label, "Choose one"));

        // Assert
        var group = cut.Find(".sb-radio-group");
        Assert.NotNull(group);
        Assert.Equal("radiogroup", group.GetAttribute("role"));
        Assert.NotNull(cut.Find(".sb-radio-group__label"));
        Assert.Contains("Choose one", cut.Markup);
    }

    [Fact]
    public void AppliesAdditionalAttributesToInput()
    {
        // Arrange & Act
        var cut = Render<SbRadioGroup<string>>(p => p
            .AddChildContent(builder =>
            {
                builder.OpenComponent<SbRadio<string>>(0);
                builder.AddAttribute(1, "Value", "x");
                builder.AddAttribute(2, "Label", "X");
                builder.AddAttribute(3, "AdditionalAttributes", new Dictionary<string, object>
                {
                    { "data-testid", "radio-x" },
                    { "aria-label", "Select X" }
                });
                builder.CloseComponent();
            }));

        // Assert
        var input = cut.Find("input.sb-radio__input");
        Assert.Equal("radio-x", input.GetAttribute("data-testid"));
        Assert.Equal("Select X", input.GetAttribute("aria-label"));
    }

    // SbRadioGroup-specific tests

    [Fact]
    public void AppliesVerticalOrientationByDefault()
    {
        // Arrange & Act
        var cut = RenderRadioGroup();

        // Assert
        var group = cut.Find(".sb-radio-group");
        Assert.Contains("sb-radio-group--vertical", group.ClassList);
    }

    [Fact]
    public void AppliesHorizontalOrientationWhenSet()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.Orientation, SbOrientation.Horizontal));

        // Assert
        var group = cut.Find(".sb-radio-group");
        Assert.Contains("sb-radio-group--horizontal", group.ClassList);
    }

    [Fact]
    public void UsesAriaLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p
            .Add(x => x.AriaLabel, "Select an option")
            .Add(x => x.Label, "Choose one"));

        // Assert - AriaLabel takes precedence over Label
        var group = cut.Find(".sb-radio-group");
        Assert.Equal("Select an option", group.GetAttribute("aria-label"));
    }

    [Fact]
    public void UsesLabelAsAriaLabelWhenAriaLabelNotSet()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.Label, "Choose one"));

        // Assert
        var group = cut.Find(".sb-radio-group");
        Assert.Equal("Choose one", group.GetAttribute("aria-label"));
    }

    [Fact]
    public void AppliesGroupClassParameter()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.Class, "horizontal-group"));

        // Assert
        var group = cut.Find(".sb-radio-group");
        Assert.Contains("horizontal-group", group.ClassList);
    }

    [Fact]
    public void AppliesGroupStyleParameter()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.Style, "gap: 1rem;"));

        // Assert
        var group = cut.Find(".sb-radio-group");
        Assert.Contains("gap: 1rem", group.GetAttribute("style"));
    }

    [Fact]
    public void AppliesGroupAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "radio-group" },
            { "aria-describedby", "group-desc" }
        }));

        // Assert
        var group = cut.Find(".sb-radio-group");
        Assert.Equal("radio-group", group.GetAttribute("data-testid"));
        Assert.Equal("group-desc", group.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void AppliesGroupDisabledClassWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderRadioGroup(p => p.Add(x => x.Disabled, true));

        // Assert
        var group = cut.Find(".sb-radio-group");
        Assert.Contains("sb-radio-group--disabled", group.ClassList);
    }
}
