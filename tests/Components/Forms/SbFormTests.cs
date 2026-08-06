using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Bunit;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbFormTests : BunitContext
{
    private IRenderedComponent<SbForm> RenderForm(
        Action<ComponentParameterCollectionBuilder<SbForm>>? configure = null)
    {
        return Render<SbForm>(p =>
        {
            p.Add(x => x.Model, new TestFormModel());
            p.AddChildContent("<span>Form content</span>");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersFormStructure()
    {
        // Arrange & Act
        var cut = RenderForm();

        // Assert
        var form = cut.Find("form.sb-form");
        Assert.NotNull(form);
        Assert.NotNull(cut.Find(".sb-form__content"));
    }

    [Fact]
    public void RendersChildContent()
    {
        // Arrange & Act
        var cut = RenderForm();

        // Assert
        var content = cut.Find(".sb-form__content");
        Assert.NotNull(content);
        Assert.Contains("Form content", content.InnerHtml);
    }

    [Fact]
    public void RendersFooterContentWhenProvided()
    {
        // Arrange & Act
        var cut = Render<SbForm>(p => p
            .Add(x => x.Model, new TestFormModel())
            .AddChildContent("<span>Body</span>")
            .Add(x => x.FooterContent, (RenderFragment)(b => b.AddMarkupContent(0, "<button type=\"submit\">Save</button>"))));

        // Assert
        var footer = cut.Find(".sb-form__footer");
        Assert.NotNull(footer);
        Assert.NotNull(cut.Find("button[type='submit']"));
    }

    [Fact]
    public void DoesNotRenderFooterWhenFooterContentNull()
    {
        // Arrange & Act
        var cut = RenderForm();

        // Assert
        Assert.Empty(cut.FindAll(".sb-form__footer"));
    }

    [Fact]
    public void RendersDataAnnotationsValidatorWhenEnableDataAnnotationsValidationTrue()
    {
        // Arrange & Act
        var cut = RenderForm(p => p.Add(x => x.EnableDataAnnotationsValidation, true));

        // Assert - DataAnnotationsValidator renders no visible output but is in the component tree
        var validator = cut.FindComponent<DataAnnotationsValidator>();
        Assert.NotNull(validator);
    }

    [Fact]
    public void DoesNotRenderDataAnnotationsValidatorWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderForm(p => p.Add(x => x.EnableDataAnnotationsValidation, false));

        // Assert
        Assert.Throws<Bunit.Rendering.ComponentNotFoundException>(() => cut.FindComponent<DataAnnotationsValidator>());
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderForm(p => p.Add(x => x.Class, "my-form"));

        // Assert
        var form = cut.Find("form");
        Assert.Contains("sb-form", form.ClassList);
        Assert.Contains("my-form", form.ClassList);
    }

    [Fact]
    public void AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderForm(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "main-form" },
            { "aria-label", "Test form" }
        }));

        // Assert
        var form = cut.Find("form");
        Assert.Equal("main-form", form.GetAttribute("data-testid"));
        Assert.Equal("Test form", form.GetAttribute("aria-label"));
    }

    [Fact]
    public async Task InvokesOnValidSubmitWhenFormValid()
    {
        // Arrange
        EditContext? receivedContext = null;
        var model = new TestFormModel { Name = "Valid Name" };
        var cut = Render<SbForm>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.OnValidSubmit, EventCallback.Factory.Create<EditContext>(this, ctx => receivedContext = ctx))
            .Add(x => x.OnInvalidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { }))
            .AddChildContent(FormFieldsWithSubmit(model)));

        // Act
        var submitBtn = cut.Find("button[type='submit']");
        await cut.InvokeAsync(() => submitBtn!.Click());

        // Assert
        Assert.NotNull(receivedContext);
        Assert.True(receivedContext!.GetValidationMessages().Count() == 0);
    }

    [Fact]
    public async Task InvokesOnInvalidSubmitWhenFormInvalid()
    {
        // Arrange - empty Name triggers [Required]
        EditContext? receivedContext = null;
        var model = new TestFormModel { Name = "" };
        var cut = Render<SbForm>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.OnValidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { }))
            .Add(x => x.OnInvalidSubmit, EventCallback.Factory.Create<EditContext>(this, ctx => receivedContext = ctx))
            .AddChildContent(FormFieldsWithSubmit(model)));

        // Act
        var submitBtn = cut.Find("button[type='submit']");
        await cut.InvokeAsync(() => submitBtn!.Click());

        // Assert
        Assert.NotNull(receivedContext);
        Assert.True(receivedContext!.GetValidationMessages().Count() > 0);
    }

    [Fact]
    public async Task ShowsValidationSummaryAfterInvalidSubmit()
    {
        // Arrange
        var model = new TestFormModel { Name = "" };
        var cut = Render<SbForm>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.OnInvalidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { }))
            .AddChildContent(FormFieldsWithSubmit(model)));

        // Assert - not shown before submit
        Assert.Empty(cut.FindAll(".sb-form__validation-summary"));

        // Act
        var submitBtn = cut.Find("button[type='submit']");
        await cut.InvokeAsync(() => submitBtn!.Click());

        // Assert - validation summary shown after invalid submit
        var summary = cut.Find(".sb-form__validation-summary");
        Assert.NotNull(summary);
        Assert.NotNull(cut.FindComponent<ValidationSummary>());
    }

    [Fact]
    public async Task DoesNotShowValidationSummaryWhenShowValidationSummaryFalse()
    {
        // Arrange
        var model = new TestFormModel { Name = "" };
        var cut = Render<SbForm>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.ShowValidationSummary, false)
            .Add(x => x.OnInvalidSubmit, EventCallback.Factory.Create<EditContext>(this, _ => { }))
            .AddChildContent(FormFieldsWithSubmit(model)));

        // Act
        var submitBtn = cut.Find("button[type='submit']");
        await cut.InvokeAsync(() => submitBtn!.Click());

        // Assert
        Assert.Empty(cut.FindAll(".sb-form__validation-summary"));
    }

    private RenderFragment FormFieldsWithSubmit(TestFormModel model) => builder =>
    {
        builder.OpenComponent<InputText>(0);
        builder.AddAttribute(1, "Value", model.Name);
        builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string>(this, v => model.Name = v ?? ""));
        builder.AddAttribute(3, "ValueExpression", (Expression<Func<string>>)(() => model.Name));
        builder.AddAttribute(4, "Id", "name");
        builder.CloseComponent();
        builder.AddMarkupContent(5, "<button type=\"submit\">Submit</button>");
    };
}

internal class TestFormModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = "";
}
