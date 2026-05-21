using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Bunit;
using SufiChain.SufiBlazor.Components.Navigation;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Navigation;

file class StubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbStepperTests : BunitContext
{
    public SbStepperTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new StubStringLocalizer());
    }

    private IRenderedComponent<SbStepper> RenderStepper(
        Action<ComponentParameterCollectionBuilder<SbStepper>>? configure = null)
    {
        return Render<SbStepper>(p => configure?.Invoke(p));
    }

    private IRenderedComponent<SbStep> RenderStep(
        Action<ComponentParameterCollectionBuilder<SbStep>>? configure = null)
    {
        return Render<SbStep>(p => configure?.Invoke(p));
    }

    // --- SbStepper tests ---

    [Fact]
    public void RendersStepperStructure()
    {
        // Arrange & Act
        var cut = RenderStepper(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbStep>(0);
            b.AddAttribute(1, "Label", "Step 1");
            b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<p>Content 1</p>")));
            b.CloseComponent();
        }));

        // Assert
        var stepper = cut.Find(".sb-stepper");
        Assert.NotNull(stepper);
        Assert.NotNull(cut.Find(".sb-stepper__steps"));
        Assert.NotNull(cut.Find(".sb-stepper__content"));
    }

    [Fact]
    public void UsesHorizontalOrientationByDefault()
    {
        // Arrange & Act
        var cut = RenderStepper(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbStep>(0);
            b.AddAttribute(1, "Label", "Step 1");
            b.CloseComponent();
        }));

        // Assert
        var stepper = cut.Find(".sb-stepper");
        Assert.Contains("sb-stepper--horizontal", stepper.ClassList);
    }

    [Theory]
    [InlineData(SbStepperOrientation.Horizontal, "horizontal")]
    [InlineData(SbStepperOrientation.Vertical, "vertical")]
    public void AppliesOrientationClass(SbStepperOrientation orientation, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderStepper(p => p
            .Add(x => x.Orientation, orientation)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
            }));

        // Assert
        var stepper = cut.Find(".sb-stepper");
        Assert.Contains($"sb-stepper--{expectedClass}", stepper.ClassList);
    }

    [Fact]
    public void RendersStepLabels()
    {
        // Arrange & Act
        var cut = RenderStepper(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbStep>(0);
            b.AddAttribute(1, "Label", "First");
            b.CloseComponent();
            b.OpenComponent<SbStep>(2);
            b.AddAttribute(3, "Label", "Second");
            b.CloseComponent();
        }));

        // Assert
        Assert.Contains("First", cut.Markup);
        Assert.Contains("Second", cut.Markup);
        var stepButtons = cut.FindAll(".sb-stepper__step-button");
        Assert.Equal(2, stepButtons.Count);
    }

    [Fact]
    public void ShowsNavigationByDefault()
    {
        // Arrange & Act - 2 steps so we get Next button (single step shows Finish)
        var cut = RenderStepper(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbStep>(0);
            b.AddAttribute(1, "Label", "Step 1");
            b.CloseComponent();
            b.OpenComponent<SbStep>(2);
            b.AddAttribute(3, "Label", "Step 2");
            b.CloseComponent();
        }));

        // Assert
        var nav = cut.Find(".sb-stepper__navigation");
        Assert.NotNull(nav);
        Assert.NotNull(cut.Find("button.sb-stepper__back"));
        Assert.NotNull(cut.Find("button.sb-stepper__next"));
    }

    [Fact]
    public void HidesNavigationWhenShowNavigationFalse()
    {
        // Arrange & Act
        var cut = RenderStepper(p => p
            .Add(x => x.ShowNavigation, false)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
            }));

        // Assert
        Assert.Empty(cut.FindAll(".sb-stepper__navigation"));
    }

    [Fact]
    public void BackButtonDisabledOnFirstStep()
    {
        // Arrange & Act
        var cut = RenderStepper(p => p
            .Add(x => x.ActiveIndex, 0)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
                b.OpenComponent<SbStep>(2);
                b.AddAttribute(3, "Label", "Step 2");
                b.CloseComponent();
            }));

        // Assert
        var backBtn = cut.Find("button.sb-stepper__back");
        Assert.NotNull(backBtn.GetAttribute("disabled"));
    }

    [Fact]
    public void ShowsFinishButtonOnLastStep()
    {
        // Arrange & Act
        var cut = RenderStepper(p => p
            .Add(x => x.ActiveIndex, 1)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
                b.OpenComponent<SbStep>(2);
                b.AddAttribute(3, "Label", "Step 2");
                b.CloseComponent();
            }));

        // Assert
        Assert.NotNull(cut.Find("button.sb-stepper__finish"));
        Assert.Empty(cut.FindAll("button.sb-stepper__next"));
    }

    [Fact]
    public void AppliesCustomButtonText()
    {
        // Arrange & Act - 2 steps at index 1 shows Back + Finish; at index 0 shows Back + Next
        var cut = RenderStepper(p => p
            .Add(x => x.ActiveIndex, 1)
            .Add(x => x.BackText, "Go Back")
            .Add(x => x.NextText, "Continue")
            .Add(x => x.FinishText, "Done")
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
                b.OpenComponent<SbStep>(2);
                b.AddAttribute(3, "Label", "Step 2");
                b.CloseComponent();
            }));

        // Assert - on last step: Back + Finish visible
        Assert.Contains("Go Back", cut.Markup);
        Assert.Contains("Done", cut.Markup);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderStepper(p => p
            .Add(x => x.Class, "my-stepper")
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
            }));

        // Assert
        var stepper = cut.Find(".sb-stepper");
        Assert.Contains("my-stepper", stepper.ClassList);
    }

    [Fact]
    public void StepButtonsHaveCorrectStatusClasses()
    {
        // Arrange & Act - 3 steps, active at index 1
        var cut = RenderStepper(p => p
            .Add(x => x.ActiveIndex, 1)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
                b.OpenComponent<SbStep>(2);
                b.AddAttribute(3, "Label", "Step 2");
                b.CloseComponent();
                b.OpenComponent<SbStep>(4);
                b.AddAttribute(5, "Label", "Step 3");
                b.CloseComponent();
            }));

        // Assert - completed, active, pending
        var steps = cut.FindAll(".sb-stepper__step");
        Assert.Equal(3, steps.Count);
        Assert.Contains("sb-stepper__step--completed", steps[0].ClassList);
        Assert.Contains("sb-stepper__step--active", steps[1].ClassList);
        Assert.Contains("sb-stepper__step--pending", steps[2].ClassList);
    }

    [Fact]
    public async Task NextButtonAdvancesActiveIndex()
    {
        // Arrange
        var received = -1;
        var cut = RenderStepper(p => p
            .Add(x => x.ActiveIndex, 0)
            .Add(x => x.ActiveIndexChanged, EventCallback.Factory.Create<int>(this, i => received = i))
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
                b.OpenComponent<SbStep>(2);
                b.AddAttribute(3, "Label", "Step 2");
                b.CloseComponent();
            }));

        // Act
        var nextBtn = cut.Find("button.sb-stepper__next");
        await cut.InvokeAsync(() => nextBtn.Click());

        // Assert
        Assert.Equal(1, received);
    }

    [Fact]
    public async Task BackButtonDecrementsActiveIndex()
    {
        // Arrange
        var received = -1;
        var cut = RenderStepper(p => p
            .Add(x => x.ActiveIndex, 1)
            .Add(x => x.ActiveIndexChanged, EventCallback.Factory.Create<int>(this, i => received = i))
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
                b.OpenComponent<SbStep>(2);
                b.AddAttribute(3, "Label", "Step 2");
                b.CloseComponent();
            }));

        // Act
        var backBtn = cut.Find("button.sb-stepper__back");
        await cut.InvokeAsync(() => backBtn.Click());

        // Assert
        Assert.Equal(0, received);
    }

    [Fact]
    public async Task FinishButtonInvokesOnFinish()
    {
        // Arrange
        var finished = false;
        var cut = RenderStepper(p => p
            .Add(x => x.ActiveIndex, 1)
            .Add(x => x.OnFinish, EventCallback.Factory.Create(this, () => finished = true))
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
                b.OpenComponent<SbStep>(2);
                b.AddAttribute(3, "Label", "Step 2");
                b.CloseComponent();
            }));

        // Act
        var finishBtn = cut.Find("button.sb-stepper__finish");
        await cut.InvokeAsync(() => finishBtn.Click());

        // Assert
        Assert.True(finished);
    }

    [Fact]
    public void LinearModeDisablesFutureStepButtons()
    {
        // Arrange & Act - Linear=true (default), ActiveIndex=0
        var cut = RenderStepper(p => p
            .Add(x => x.ActiveIndex, 0)
            .Add(x => x.Linear, true)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
                b.OpenComponent<SbStep>(2);
                b.AddAttribute(3, "Label", "Step 2");
                b.CloseComponent();
            }));

        // Assert - first step button enabled, second disabled
        var buttons = cut.FindAll(".sb-stepper__step-button");
        Assert.Null(buttons[0].GetAttribute("disabled"));
        Assert.NotNull(buttons[1].GetAttribute("disabled"));
    }

    [Fact]
    public void NonLinearModeEnablesAllStepButtons()
    {
        // Arrange & Act
        var cut = RenderStepper(p => p
            .Add(x => x.ActiveIndex, 0)
            .Add(x => x.Linear, false)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.CloseComponent();
                b.OpenComponent<SbStep>(2);
                b.AddAttribute(3, "Label", "Step 2");
                b.CloseComponent();
            }));

        // Assert
        var buttons = cut.FindAll(".sb-stepper__step-button");
        Assert.Null(buttons[0].GetAttribute("disabled"));
        Assert.Null(buttons[1].GetAttribute("disabled"));
    }

    [Fact]
    public void RendersOptionalLabelWhenStepOptional()
    {
        // Arrange & Act
        var cut = RenderStepper(p => p
            .Add(x => x.OptionalText, "Optional")
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Required");
                b.CloseComponent();
                b.OpenComponent<SbStep>(2);
                b.AddAttribute(3, "Label", "Optional step");
                b.AddAttribute(4, "Optional", true);
                b.CloseComponent();
            }));

        // Assert
        Assert.Contains("Optional", cut.Markup);
        var optionalStep = cut.FindAll(".sb-stepper__step--optional");
        Assert.Single(optionalStep);
    }

    [Fact]
    public void RendersStepDescription()
    {
        // Arrange & Act
        var cut = RenderStepper(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbStep>(0);
            b.AddAttribute(1, "Label", "Step 1");
            b.AddAttribute(2, "Description", "Enter your details");
            b.CloseComponent();
        }));

        // Assert
        Assert.Contains("Enter your details", cut.Markup);
    }

    // --- SbStep tests ---

    [Fact]
    public void Step_RendersContentWhenActive()
    {
        // Arrange & Act
        var cut = Render<SbStepper>(p => p
            .Add(x => x.ActiveIndex, 0)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<p>Active content</p>")));
                b.CloseComponent();
            }));

        // Assert
        var content = cut.Find(".sb-step-content");
        Assert.NotNull(content);
        Assert.Contains("Active content", cut.Markup);
    }

    [Fact]
    public void Step_DoesNotRenderContentWhenNotActive()
    {
        // Arrange & Act - ActiveIndex 1, so first step's content is hidden
        var cut = Render<SbStepper>(p => p
            .Add(x => x.ActiveIndex, 1)
            .AddChildContent(b =>
            {
                b.OpenComponent<SbStep>(0);
                b.AddAttribute(1, "Label", "Step 1");
                b.AddAttribute(2, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<p>Hidden content</p>")));
                b.CloseComponent();
                b.OpenComponent<SbStep>(3);
                b.AddAttribute(4, "Label", "Step 2");
                b.AddAttribute(5, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "<p>Visible content</p>")));
                b.CloseComponent();
            }));

        // Assert - only Step 2 content visible
        Assert.Contains("Visible content", cut.Markup);
        Assert.DoesNotContain("Hidden content", cut.Markup);
    }

    [Fact]
    public void Step_WorksInsideSbStepper()
    {
        // Arrange & Act
        var cut = Render<SbStepper>(p => p.AddChildContent(b =>
        {
            b.OpenComponent<SbStep>(0);
            b.AddAttribute(1, "Label", "First Step");
            b.AddAttribute(2, "Description", "Description text");
            b.AddAttribute(3, "ChildContent", (RenderFragment)(c => c.AddMarkupContent(0, "Step body")));
            b.CloseComponent();
        }));

        // Assert
        var stepper = cut.Find(".sb-stepper");
        Assert.NotNull(stepper);
        Assert.Contains("First Step", cut.Markup);
        Assert.Contains("Description text", cut.Markup);
        Assert.Contains("Step body", cut.Markup);
    }
}
