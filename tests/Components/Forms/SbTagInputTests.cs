using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Bunit;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbTagInputTests : BunitContext
{
    private IRenderedComponent<SbTagInput> RenderTagInput(
        Action<ComponentParameterCollectionBuilder<SbTagInput>>? configure = null)
    {
        return Render<SbTagInput>(p =>
        {
            p.Add(x => x.Tags, new List<string>());
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersTagInputStructure()
    {
        // Arrange & Act
        var cut = RenderTagInput();

        // Assert
        var wrapper = cut.Find(".sb-tag-input");
        Assert.NotNull(wrapper);
        Assert.NotNull(cut.Find(".sb-tag-input__container"));
        Assert.NotNull(cut.Find(".sb-tag-input__input"));
    }

    [Fact]
    public void RendersPlaceholderWhenNoTags()
    {
        // Arrange & Act
        var cut = RenderTagInput();

        // Assert
        var input = cut.Find(".sb-tag-input__input");
        Assert.Equal("Add tag...", input.GetAttribute("placeholder"));
    }

    [Fact]
    public void RendersCustomPlaceholderWhenProvided()
    {
        // Arrange & Act
        var cut = RenderTagInput(p => p.Add(x => x.Placeholder, "Type and press Enter..."));

        // Assert
        var input = cut.Find(".sb-tag-input__input");
        Assert.Equal("Type and press Enter...", input.GetAttribute("placeholder"));
    }

    [Fact]
    public void HidesPlaceholderWhenTagsPresent()
    {
        // Arrange & Act
        var tags = new List<string> { "tag1" };
        var cut = Render<SbTagInput>(p => p.Add(x => x.Tags, tags));

        // Assert
        var input = cut.Find(".sb-tag-input__input");
        Assert.Equal("", input.GetAttribute("placeholder"));
    }

    [Fact]
    public void RendersTagsWhenProvided()
    {
        // Arrange & Act
        var tags = new List<string> { "alpha", "beta", "gamma" };
        var cut = Render<SbTagInput>(p => p.Add(x => x.Tags, tags));

        // Assert
        var tagElements = cut.FindAll(".sb-tag-input__tag");
        Assert.Equal(3, tagElements.Count);
        Assert.Contains("alpha", cut.Markup);
        Assert.Contains("beta", cut.Markup);
        Assert.Contains("gamma", cut.Markup);
    }

    [Fact]
    public void RendersRemoveButtonsWhenNotDisabledOrReadOnly()
    {
        // Arrange & Act
        var tags = new List<string> { "tag1" };
        var cut = Render<SbTagInput>(p => p.Add(x => x.Tags, tags));

        // Assert
        var removeButtons = cut.FindAll(".sb-tag-input__tag-remove");
        Assert.Single(removeButtons);
        Assert.Equal("Remove tag1", removeButtons[0].GetAttribute("aria-label"));
    }

    [Fact]
    public void DoesNotRenderRemoveButtonsWhenDisabled()
    {
        // Arrange & Act
        var tags = new List<string> { "tag1" };
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.Disabled, true));

        // Assert
        Assert.Empty(cut.FindAll(".sb-tag-input__tag-remove"));
    }

    [Fact]
    public void DoesNotRenderRemoveButtonsWhenReadOnly()
    {
        // Arrange & Act
        var tags = new List<string> { "tag1" };
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.ReadOnly, true));

        // Assert
        Assert.Empty(cut.FindAll(".sb-tag-input__tag-remove"));
    }

    [Fact]
    public void DoesNotRenderInputWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderTagInput(p => p.Add(x => x.Disabled, true));

        // Assert
        Assert.Empty(cut.FindAll(".sb-tag-input__input"));
    }

    [Fact]
    public void DoesNotRenderInputWhenReadOnly()
    {
        // Arrange & Act
        var cut = RenderTagInput(p => p.Add(x => x.ReadOnly, true));

        // Assert
        Assert.Empty(cut.FindAll(".sb-tag-input__input"));
    }

    [Fact]
    public void RendersCounterWhenMaxTagsSet()
    {
        // Arrange & Act
        var tags = new List<string> { "a", "b" };
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.MaxTags, 5));

        // Assert
        var counter = cut.Find(".sb-tag-input__counter");
        Assert.NotNull(counter);
        Assert.Contains("2 / 5", counter.TextContent);
    }

    [Fact]
    public void DoesNotRenderInputWhenMaxTagsReached()
    {
        // Arrange & Act
        var tags = new List<string> { "a", "b" };
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.MaxTags, 2));

        // Assert
        Assert.Empty(cut.FindAll(".sb-tag-input__input"));
    }

    [Fact]
    public void AppliesDisabledClassWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderTagInput(p => p.Add(x => x.Disabled, true));

        // Assert
        var wrapper = cut.Find(".sb-tag-input");
        Assert.Contains("sb-tag-input--disabled", wrapper.ClassList);
    }

    [Fact]
    public void AppliesClassParameter()
    {
        // Arrange & Act
        var cut = RenderTagInput(p => p.Add(x => x.Class, "my-tag-input"));

        // Assert
        var wrapper = cut.Find(".sb-tag-input");
        Assert.Contains("my-tag-input", wrapper.ClassList);
    }

    [Fact]
    public void AppliesStyleParameter()
    {
        // Arrange & Act
        var cut = RenderTagInput(p => p.Add(x => x.Style, "min-width: 200px;"));

        // Assert
        var wrapper = cut.Find(".sb-tag-input");
        Assert.Contains("min-width: 200px", wrapper.GetAttribute("style"));
    }

    [Fact]
    public void AppliesIdToInput()
    {
        // Arrange & Act
        var cut = RenderTagInput(p => p.Add(x => x.Id, "skills-input"));

        // Assert
        var input = cut.Find(".sb-tag-input__input");
        Assert.Equal("skills-input", input.GetAttribute("id"));
    }

    [Fact]
    public async Task InvokesTagsChangedWhenTagAddedViaEnter()
    {
        // Arrange
        var tags = new List<string>();
        List<string>? received = null;
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.TagsChanged, EventCallback.Factory.Create<List<string>>(this, list => received = list)));

        var input = cut.Find(".sb-tag-input__input");
        await cut.InvokeAsync(() => input!.Input("newtag"));
        await cut.InvokeAsync(() => input.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" }));

        // Assert
        Assert.NotNull(received);
        Assert.Single(received);
        Assert.Equal("newtag", received[0]);
    }

    [Fact]
    public async Task InvokesTagsChangedWhenTagAddedViaComma()
    {
        // Arrange
        var tags = new List<string>();
        List<string>? received = null;
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.TagsChanged, EventCallback.Factory.Create<List<string>>(this, list => received = list)));

        var input = cut.Find(".sb-tag-input__input");
        await cut.InvokeAsync(() => input!.Input("tag1, tag2"));

        // Assert
        Assert.NotNull(received);
        Assert.Equal(2, received!.Count);
        Assert.Equal("tag1", received[0]);
        Assert.Equal("tag2", received[1]);
    }

    [Fact]
    public async Task InvokesTagsChangedWhenTagRemoved()
    {
        // Arrange
        var tags = new List<string> { "keep", "remove" };
        List<string>? received = null;
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.TagsChanged, EventCallback.Factory.Create<List<string>>(this, list => received = list)));

        var removeButtons = cut.FindAll(".sb-tag-input__tag-remove");
        var removeSecond = removeButtons.FirstOrDefault(b => b.GetAttribute("aria-label") == "Remove remove");
        Assert.NotNull(removeSecond);

        // Act
        await cut.InvokeAsync(() => removeSecond!.Click());

        // Assert
        Assert.NotNull(received);
        Assert.Single(received);
        Assert.Equal("keep", received[0]);
    }

    [Fact]
    public async Task RemovesLastTagWhenBackspacePressedWithEmptyInput()
    {
        // Arrange
        var tags = new List<string> { "first", "second" };
        List<string>? received = null;
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.TagsChanged, EventCallback.Factory.Create<List<string>>(this, list => received = list)));

        var input = cut.Find(".sb-tag-input__input");
        await cut.InvokeAsync(() => input!.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Backspace" }));

        // Assert
        Assert.NotNull(received);
        Assert.Single(received);
        Assert.Equal("first", received[0]);
    }

    [Fact]
    public async Task InvokesTagsChangedWhenTagAddedViaBlur()
    {
        // Arrange
        var tags = new List<string>();
        List<string>? received = null;
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.TagsChanged, EventCallback.Factory.Create<List<string>>(this, list => received = list)));

        var input = cut.Find(".sb-tag-input__input");
        await cut.InvokeAsync(() => input!.Input("blurtag"));
        await cut.InvokeAsync(() => input.TriggerEventAsync("onblur", EventArgs.Empty));

        // Assert
        Assert.NotNull(received);
        Assert.Single(received);
        Assert.Equal("blurtag", received[0]);
    }

    [Fact]
    public async Task DoesNotAddDuplicateWhenAllowDuplicatesFalse()
    {
        // Arrange
        var tags = new List<string> { "existing" };
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.AllowDuplicates, false));

        var input = cut.Find(".sb-tag-input__input");
        await cut.InvokeAsync(() => input!.Input("existing"));
        await cut.InvokeAsync(() => input.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" }));

        // Assert
        Assert.Single(tags);
        Assert.Equal("existing", tags[0]);
    }

    [Fact]
    public async Task AddsDuplicateWhenAllowDuplicatesTrue()
    {
        // Arrange
        var tags = new List<string> { "existing" };
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.AllowDuplicates, true));

        var input = cut.Find(".sb-tag-input__input");
        await cut.InvokeAsync(() => input!.Input("existing"));
        await cut.InvokeAsync(() => input.TriggerEventAsync("onkeydown", new KeyboardEventArgs { Key = "Enter" }));

        // Assert
        Assert.Equal(2, tags.Count);
        Assert.Equal("existing", tags[0]);
        Assert.Equal("existing", tags[1]);
    }

    [Fact]
    public async Task DoesNotAddTagWhenMaxTagsReached()
    {
        // Arrange - 1 tag, max 2. Input "b, c, d" tries to add 3; only b should be added (then at max)
        var tags = new List<string> { "a" };
        List<string>? received = null;
        var cut = Render<SbTagInput>(p => p
            .Add(x => x.Tags, tags)
            .Add(x => x.MaxTags, 2)
            .Add(x => x.TagsChanged, EventCallback.Factory.Create<List<string>>(this, list => received = list)));

        var input = cut.Find(".sb-tag-input__input");
        await cut.InvokeAsync(() => input!.Input("b, c, d"));

        // Assert - "b" added (2 tags), "c" and "d" rejected (would exceed max)
        Assert.NotNull(received);
        Assert.Equal(2, received!.Count);
        Assert.Equal("a", received[0]);
        Assert.Equal("b", received[1]);
    }
}
