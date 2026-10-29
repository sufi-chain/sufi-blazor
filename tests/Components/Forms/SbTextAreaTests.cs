using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Bunit;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbTextAreaTests : BunitContext
{
    private IRenderedComponent<SbTextArea> RenderTextArea(
        Action<ComponentParameterCollectionBuilder<SbTextArea>>? configure = null)
    {
        return Render<SbTextArea>(p => configure?.Invoke(p));
    }

    public SbTextAreaTests()
    {
        // Host and component sets write the DOM through JS. Typing must never call it.
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RendersTextAreaStructure()
    {
        // Arrange & Act
        var cut = RenderTextArea();

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.NotNull(wrapper);
        var textarea = cut.Find("textarea.sb-textarea__input");
        Assert.NotNull(textarea);
    }

    [Fact]
    public void RendersLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Label, "Description")
            .Add(x => x.Id, "desc"));

        // Assert
        var label = cut.Find(".sb-textarea__label");
        Assert.NotNull(label);
        Assert.Contains("Description", label.TextContent);
        Assert.Equal("desc", label.GetAttribute("for"));
    }

    [Fact]
    public void DoesNotRenderLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderTextArea();

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__label"));
    }

    [Fact]
    public void RendersRequiredIndicatorWhenRequiredTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Label, "Comment")
            .Add(x => x.Required, true));

        // Assert
        var required = cut.Find(".sb-textarea__required");
        Assert.NotNull(required);
        Assert.Equal("*", required.TextContent);
        Assert.Equal("true", required.GetAttribute("aria-hidden"));
    }

    [Fact]
    public void AssociatesLabelWithTextAreaId()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Label, "Notes")
            .Add(x => x.Id, "notes-field"));

        // Assert
        var label = cut.Find("label.sb-textarea__label");
        var textarea = cut.Find("textarea");
        Assert.Equal("notes-field", label.GetAttribute("for"));
        Assert.Equal("notes-field", textarea.GetAttribute("id"));
    }

    [Fact]
    public void RendersHelperTextWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.HelperText, "Enter a detailed description")
            .Add(x => x.Id, "desc"));

        // Assert
        var helper = cut.Find(".sb-textarea__helper");
        Assert.NotNull(helper);
        Assert.Contains("Enter a detailed description", helper.TextContent);
        Assert.Equal("desc-helper", helper.GetAttribute("id"));
    }

    [Fact]
    public void HidesHelperTextWhenErrorTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.HelperText, "Helper text")
            .Add(x => x.Error, true)
            .Add(x => x.ErrorMessage, "Invalid input"));

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__helper"));
    }

    [Fact]
    public void RendersErrorMessageWhenErrorTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Error, true)
            .Add(x => x.ErrorMessage, "This field is required")
            .Add(x => x.Id, "desc"));

        // Assert
        var error = cut.Find(".sb-textarea__error");
        Assert.NotNull(error);
        Assert.Contains("This field is required", error.TextContent);
        Assert.Equal("alert", error.GetAttribute("role"));
        Assert.Equal("desc-error", error.GetAttribute("id"));
    }

    [Fact]
    public void DoesNotRenderErrorMessageWhenErrorFalse()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.ErrorMessage, "Should not show"));

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__error"));
    }

    [Fact]
    public void DisplaysValue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Value, "Hello world"));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("Hello world", textarea.GetAttribute("value"));
    }

    [Fact]
    public void RapidInputReportsCompleteBrowserValue()
    {
        var reportedValues = new List<string?>();
        var cut = RenderTextArea(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(
                this,
                value => reportedValues.Add(value))));

        var textarea = cut.Find("textarea");
        textarea.Input("a");
        textarea.Input("ar");
        textarea.Input("ari");
        textarea.Input("aria");

        Assert.Equal(["a", "ar", "ari", "aria"], reportedValues);
    }

    [Fact]
    public void RapidInputSurvivesParentRerendersWithStaleValue()
    {
        var reportedValues = new List<string?>();
        var cut = RenderTextArea(p => p
            .Add(x => x.Value, string.Empty)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(
                this,
                value => reportedValues.Add(value))));

        var textarea = cut.Find("textarea");
        textarea.Input("a");
        cut.Render(p => p.Add(x => x.Value, string.Empty));
        textarea = cut.Find("textarea");
        textarea.Input("ar");
        cut.Render(p => p.Add(x => x.Value, string.Empty));
        textarea = cut.Find("textarea");
        textarea.Input("ari");
        cut.Render(p => p.Add(x => x.Value, string.Empty));
        textarea = cut.Find("textarea");
        textarea.Input("aria");
        cut.Render(p => p.Add(x => x.Value, string.Empty));

        Assert.Equal(["a", "ar", "ari", "aria"], reportedValues);
        // sufi-chain#533: the host value stays empty here, so the render value never follows the
        // keystrokes and no render can write an older value over the text the browser holds.
        Assert.Equal(string.Empty, cut.Find("textarea").GetAttribute("value"));
        Assert.Equal("aria", cut.Instance.CurrentText);
    }

    [Fact]
    public void RapidInputSurvivesDelayedParentAcknowledgements()
    {
        var reportedValues = new List<string?>();
        var cut = RenderTextArea(p => p
            .Add(x => x.Value, string.Empty)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(
                this,
                value => reportedValues.Add(value))));

        cut.Find("textarea").Input("a");
        cut.Render(p => p.Add(x => x.Value, "a"));
        cut.Find("textarea").Input("ar");
        cut.Render(p => p.Add(x => x.Value, "a"));
        cut.Find("textarea").Input("ari");
        cut.Render(p => p.Add(x => x.Value, "ar"));
        cut.Find("textarea").Input("aria");
        cut.Render(p => p.Add(x => x.Value, "ari"));

        Assert.Equal(["a", "ar", "ari", "aria"], reportedValues);
        // The 9.1 guard advanced the rendered value to the first acknowledged echo ("a").
        // That write is the drop: Blazor applies it after later keystrokes are already in the
        // browser. The attribute stays on the last external value instead.
        Assert.Equal(string.Empty, cut.Find("textarea").GetAttribute("value"));
        Assert.Equal("aria", cut.Instance.CurrentText);
    }

    [Fact]
    public void RendersPlaceholder()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Placeholder, "Type here..."));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("Type here...", textarea.GetAttribute("placeholder"));
    }

    [Fact]
    public void AppliesRowsAttribute()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Rows, 5));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("5", textarea.GetAttribute("rows"));
    }

    [Fact]
    public void AppliesMaxLengthAttribute()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.MaxLength, 500));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("500", textarea.GetAttribute("maxlength"));
    }

    [Fact]
    public void RendersCharacterCountWhenShowCharacterCountAndMaxLengthSet()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Value, "Hi")
            .Add(x => x.ShowCharacterCount, true)
            .Add(x => x.MaxLength, 100));

        // Assert
        var counter = cut.Find(".sb-textarea__counter");
        Assert.NotNull(counter);
        Assert.Contains("2 / 100", counter.TextContent);
    }

    [Fact]
    public void DoesNotRenderCharacterCountWhenShowCharacterCountFalse()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.MaxLength, 100)
            .Add(x => x.ShowCharacterCount, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__counter"));
    }

    [Fact]
    public void DoesNotRenderCharacterCountWhenMaxLengthNotSet()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.ShowCharacterCount, true));

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__counter"));
    }

    [Fact]
    public void RendersResizeHandleWhenResizableTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Resizable, true));

        // Assert
        Assert.NotNull(cut.Find(".sb-textarea__resize-handle"));
    }

    [Fact]
    public void DoesNotRenderResizeHandleWhenResizableFalse()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Resizable, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-textarea__resize-handle"));
    }

    [Fact]
    public void AppliesErrorClassWhenErrorTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Error, true));

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.Contains("sb-textarea--error", wrapper.ClassList);
    }

    [Fact]
    public void AppliesDisabledClassWhenDisabledTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Disabled, true));

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.Contains("sb-textarea--disabled", wrapper.ClassList);
        var textarea = cut.Find("textarea");
        Assert.NotNull(textarea.GetAttribute("disabled"));
    }

    [Fact]
    public void AppliesReadOnlyClassWhenReadOnlyTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.ReadOnly, true));

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.Contains("sb-textarea--readonly", wrapper.ClassList);
        var textarea = cut.Find("textarea");
        Assert.NotNull(textarea.GetAttribute("readonly"));
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Class, "my-textarea"));

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.Contains("my-textarea", wrapper.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Style, "min-height: 100px;"));

        // Assert
        var wrapper = cut.Find(".sb-textarea");
        Assert.Contains("min-height: 100px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public void SetsAriaInvalidWhenErrorTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.Error, true));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("true", textarea.GetAttribute("aria-invalid"));
    }

    [Fact]
    public void SetsAriaDescribedByToHelperWhenHelperTextProvided()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Id, "desc")
            .Add(x => x.HelperText, "Hint text"));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("desc-helper", textarea.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void SetsAriaDescribedByToErrorWhenErrorTrue()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p
            .Add(x => x.Id, "desc")
            .Add(x => x.HelperText, "Hint")
            .Add(x => x.Error, true)
            .Add(x => x.ErrorMessage, "Error"));

        // Assert - when error, aria-describedby points to error, not helper
        var textarea = cut.Find("textarea");
        Assert.Equal("desc-error", textarea.GetAttribute("aria-describedby"));
    }

    [Fact]
    public async Task InvokesValueChangedOnInput()
    {
        // Arrange
        string? received = null;
        var cut = RenderTextArea(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        var textarea = cut.Find("textarea");

        // Act
        await cut.InvokeAsync(() => textarea!.Input("New text"));

        // Assert
        Assert.Equal("New text", received);
        Assert.Equal("New text", cut.Instance.CurrentText);
        // Typing must not write the DOM through JS; the browser already holds the text.
        Assert.DoesNotContain(JSInterop.Invocations, invocation =>
            invocation.Identifier == "SufiBlazor.textInput.setValue");
    }

    [Fact]
    public async Task ChangeCommitsTextWhenInputHasNotFired()
    {
        string? received = null;
        var cut = RenderTextArea(p => p
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        await cut.InvokeAsync(() => cut.Find("textarea").Change("QA P13 desc"));

        Assert.Equal("QA P13 desc", received);
        Assert.Equal("QA P13 desc", cut.Instance.CurrentText);
    }

    [Fact]
    public void StaleParentValueDoesNotClearTypedDraft()
    {
        var cut = RenderTextArea(p => p.Add(x => x.Value, (string?)null));

        cut.Find("textarea").Input("QA P13 desc");
        cut.Render(p => p.Add(x => x.Value, (string?)null));

        Assert.Equal("QA P13 desc", cut.Instance.CurrentText);
        // The rendered value stays with the host value; the browser keeps the typed text.
        Assert.Equal(string.Empty, cut.Find("textarea").GetAttribute("value"));
    }

    [Fact]
    public async Task GetCommittedValueReadsBrowserTextWhenDraftIsEmpty()
    {
        string? received = null;
        var cut = RenderTextArea(p => p
            .Add(x => x.Value, (string?)null)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        Assert.Equal(string.Empty, cut.Instance.CurrentText);
        JSInterop.Setup<string?>("SufiBlazor.readControlValue", _ => true)
            .SetResult("QA P13 desc");

        var committed = await cut.InvokeAsync(() => cut.Instance.GetCommittedValueAsync());

        Assert.Equal("QA P13 desc", committed);
        Assert.Equal("QA P13 desc", cut.Instance.CurrentText);
        Assert.Equal("QA P13 desc", received);
    }

    [Fact]
    public async Task GetCommittedValueTreatsEmptyBrowserTextAsCleared()
    {
        string? received = "unchanged";
        var cut = RenderTextArea(p => p
            .Add(x => x.Value, "QA P13 desc")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, v => received = v)));

        JSInterop.Setup<string?>("SufiBlazor.readControlValue", _ => true)
            .SetResult(string.Empty);

        var committed = await cut.InvokeAsync(() => cut.Instance.GetCommittedValueAsync());

        Assert.Equal(string.Empty, committed);
        Assert.Equal(string.Empty, cut.Instance.CurrentText);
        Assert.Equal(string.Empty, received);
    }

    [Fact]
    public async Task GetCommittedValueKeepsDraftWhenBrowserReadFails()
    {
        var cut = RenderTextArea(p => p.Add(x => x.Value, "QA P13 desc"));
        JSInterop.Setup<string?>("SufiBlazor.readControlValue", _ => true)
            .SetException(new JSException("read failed"));

        var committed = await cut.InvokeAsync(() => cut.Instance.GetCommittedValueAsync());

        Assert.Equal("QA P13 desc", committed);
        Assert.Equal("QA P13 desc", cut.Instance.CurrentText);
    }

    [Fact]
    public void AdoptsExternalParentValueChange()
    {
        var cut = RenderTextArea(p => p
            .Add(x => x.Value, "Initial text"));

        cut.Render(p => p.Add(x => x.Value, "Changed text"));

        Assert.Equal("Changed text", cut.Find("textarea").GetAttribute("value"));
        Assert.Equal("Changed text", cut.Instance.CurrentText);
    }

    [Fact]
    public void AppliesAdditionalAttributes()
    {
        // Arrange & Act
        var cut = RenderTextArea(p => p.Add(x => x.AdditionalAttributes, new Dictionary<string, object>
        {
            { "data-testid", "description-area" },
            { "aria-label", "Description field" }
        }));

        // Assert
        var textarea = cut.Find("textarea");
        Assert.Equal("description-area", textarea.GetAttribute("data-testid"));
        Assert.Equal("Description field", textarea.GetAttribute("aria-label"));
    }

    // sufi-chain#533: the shared draft guard keeps the rendered value attribute still while the
    // user types, so a parent render that lags the circuit cannot drop characters.
    [Fact]
    public void RapidInputWithDelayedEchoKeepsEveryCharacter()
    {
        var reportedValues = new List<string?>();
        var cut = RenderTextArea(p => p
            .Add(x => x.Value, string.Empty)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, value => reportedValues.Add(value))));

        cut.Find("textarea").Input("a");
        cut.Render(p => p.Add(x => x.Value, string.Empty));
        cut.Find("textarea").Input("ar");
        cut.Render(p => p.Add(x => x.Value, "a"));
        cut.Find("textarea").Input("ari");
        cut.Render(p => p.Add(x => x.Value, "ar"));
        cut.Find("textarea").Input("aria");
        cut.Render(p => p.Add(x => x.Value, "ari"));

        Assert.Equal(["a", "ar", "ari", "aria"], reportedValues);
        Assert.Equal(string.Empty, cut.Find("textarea").GetAttribute("value"));
        Assert.Equal(0, TextInputSetValueCalls());
    }

    /// <summary>
    /// sufi-chain#533. CMS bodies and other <c>SbTextArea</c> <c>@bind-Value</c> fields echo each
    /// keystroke. The rendered value must stay on the last external text for Persian and English.
    /// </summary>
    [Theory]
    [InlineData("", "The quick brown fox jumps over the lazy dog.")]
    [InlineData("", "تست ۱ با اینتر سریع")]
    [InlineData("پیش‌نویس", "پیش‌نویس کامل")]
    public void BindEchoLeavesTheRenderedValueUntouched(string initial, string typed)
    {
        var value = initial;
        var cut = RenderTextArea(p => AddBoundAreaParameters(p, () => value, next => value = next ?? string.Empty));
        Assert.Equal(initial, cut.Find("textarea").GetAttribute("value"));

        for (var i = 1; i <= typed.Length; i++)
        {
            var next = typed[..i];
            cut.Find("textarea").Input(next);
            var echoed = value;
            cut.Render(p => AddBoundAreaParameters(p, () => echoed, updated => value = updated ?? string.Empty));
            Assert.Equal(initial, cut.Find("textarea").GetAttribute("value"));
        }

        Assert.Equal(typed, value);
        Assert.Equal(typed, cut.Instance.CurrentText);
        Assert.Equal(0, TextInputSetValueCalls());

        value = "reset by host";
        var reset = value;
        cut.Render(p => AddBoundAreaParameters(p, () => reset, updated => value = updated ?? string.Empty));
        Assert.Equal("reset by host", cut.Find("textarea").GetAttribute("value"));
    }

    private void AddBoundAreaParameters(
        ComponentParameterCollectionBuilder<SbTextArea> p,
        Func<string> read,
        Action<string?> write)
    {
        p.Add(x => x.Value, read());
        p.Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, write));
    }

    [Fact]
    public void ExternalResetAfterSaveReachesTheField()
    {
        var cut = RenderTextArea(p => p.Add(x => x.Value, "saved text"));
        cut.Find("textarea").Input("edited but not saved");
        cut.Render(p => p.Add(x => x.Value, "saved text"));
        cut.Render(p => p.Add(x => x.Value, "reset by host"));

        Assert.Equal("reset by host", cut.Find("textarea").GetAttribute("value"));
        Assert.Equal("reset by host", cut.Instance.CurrentText);
    }

    [Fact]
    public void PersianTextKeepsItsFirstLetterWhenTheHostRenderIsStale()
    {
        var reportedValues = new List<string?>();
        var cut = RenderTextArea(p => p
            .Add(x => x.Value, string.Empty)
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string?>(this, value => reportedValues.Add(value))));

        cut.Find("textarea").Input("ت");
        cut.Render(p => p.Add(x => x.Value, string.Empty));
        cut.Find("textarea").Input("تس");
        cut.Render(p => p.Add(x => x.Value, "ت"));
        cut.Find("textarea").Input("تست ۱ با اینتر");

        Assert.Equal("تست ۱ با اینتر", reportedValues[^1]);
        Assert.Equal(0, TextInputSetValueCalls());
    }

    private int TextInputSetValueCalls() =>
        JSInterop.Invocations.Count(invocation =>
            invocation.Identifier == "SufiBlazor.textInput.setValue");
}
