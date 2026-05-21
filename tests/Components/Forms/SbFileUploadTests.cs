using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using Bunit;
using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class SbFileUploadTests : BunitContext
{
    private IRenderedComponent<SbFileUpload> RenderFileUpload(
        Action<ComponentParameterCollectionBuilder<SbFileUpload>>? configure = null)
    {
        return Render<SbFileUpload>(p =>
        {
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersFileUploadStructure()
    {
        // Arrange & Act
        var cut = RenderFileUpload();

        // Assert
        var wrapper = cut.Find(".sb-file-upload");
        Assert.NotNull(wrapper);
        Assert.NotNull(cut.Find(".sb-file-upload__dropzone"));
        Assert.NotNull(cut.Find(".sb-file-upload__input"));
        Assert.NotNull(cut.Find(".sb-file-upload__icon"));
        Assert.NotNull(cut.Find(".sb-file-upload__text"));
    }

    [Fact]
    public void RendersDefaultDragText()
    {
        // Arrange & Act
        var cut = RenderFileUpload();

        // Assert
        var textSpan = cut.Find(".sb-file-upload__text");
        Assert.NotNull(textSpan);
        Assert.Contains("Drag and drop", textSpan.TextContent);
        Assert.Contains("browse", textSpan.TextContent);
    }

    [Fact]
    public void RendersCustomDragTextWhenProvided()
    {
        // Arrange & Act
        var cut = Render<SbFileUpload>(p => p.Add(x => x.DragText, builder =>
        {
            builder.AddContent(0, "Upload your documents here");
        }));

        // Assert
        var textSpans = cut.FindAll(".sb-file-upload__text");
        var dragText = textSpans.FirstOrDefault(s => s.TextContent.Contains("Upload your documents"));
        Assert.NotNull(dragText);
    }

    [Fact]
    public void AppliesDisabledClassWhenDisabled()
    {
        // Arrange & Act
        var cut = RenderFileUpload(p => p.Add(x => x.Disabled, true));

        // Assert
        var dropzone = cut.Find(".sb-file-upload__dropzone");
        Assert.Contains("sb-file-upload__dropzone--disabled", dropzone.ClassList);
    }

    [Fact]
    public void RendersConstraintsTextWhenAcceptProvided()
    {
        // Arrange & Act
        var cut = RenderFileUpload(p => p.Add(x => x.Accept, "image/*,.pdf"));

        // Assert
        var constraintsSpan = cut.FindAll(".sb-file-upload__text")
            .FirstOrDefault(s => s.TextContent.Contains("Allowed:"));
        Assert.NotNull(constraintsSpan);
        Assert.Contains("image/*,.pdf", constraintsSpan.TextContent);
    }

    [Fact]
    public void RendersConstraintsTextWhenMaxFileSizeProvided()
    {
        // Arrange & Act
        var cut = RenderFileUpload(p => p.Add(x => x.MaxFileSizeBytes, 5_242_880L)); // 5 MB

        // Assert
        var constraintsSpan = cut.FindAll(".sb-file-upload__text")
            .FirstOrDefault(s => s.TextContent.Contains("Max size:"));
        Assert.NotNull(constraintsSpan);
        Assert.Contains("5 MB", constraintsSpan.TextContent);
    }

    [Fact]
    public async Task InvokesOnFilesSelectedWhenFileUploaded()
    {
        // Arrange
        IReadOnlyList<IBrowserFile>? received = null;
        var cut = RenderFileUpload(p => p
            .Add(x => x.OnFilesSelected, EventCallback.Factory.Create<IReadOnlyList<IBrowserFile>>(this, f => received = f)));

        var fileContent = InputFileContent.CreateFromText("test content", "document.txt");

        // Act
        cut.FindComponent<InputFile>().UploadFiles(fileContent);

        // Assert
        Assert.NotNull(received);
        Assert.Single(received!);
        Assert.Equal("document.txt", received![0].Name);
    }

    [Fact]
    public async Task ShowsFileInListAfterUpload()
    {
        // Arrange
        var cut = RenderFileUpload();
        var fileContent = InputFileContent.CreateFromText("content", "report.pdf");

        // Act
        cut.FindComponent<InputFile>().UploadFiles(fileContent);

        // Assert
        var list = cut.Find(".sb-file-upload__list");
        Assert.NotNull(list);
        var item = cut.Find(".sb-file-upload__item");
        Assert.NotNull(item);
        Assert.Contains("report.pdf", cut.Find(".sb-file-upload__item-name").TextContent);
    }

    [Fact]
    public async Task InvokesOnFileRemovedWhenRemoveClicked()
    {
        // Arrange
        IBrowserFile? removedFile = null;
        var cut = RenderFileUpload(p => p
            .Add(x => x.OnFilesSelected, EventCallback.Factory.Create<IReadOnlyList<IBrowserFile>>(this, _ => { }))
            .Add(x => x.OnFileRemoved, EventCallback.Factory.Create<IBrowserFile>(this, f => removedFile = f)));

        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromText("x", "to-remove.txt"));

        // Act
        var removeBtn = cut.Find(".sb-file-upload__item-remove");
        await cut.InvokeAsync(() => removeBtn!.Click());

        // Assert
        Assert.NotNull(removedFile);
        Assert.Equal("to-remove.txt", removedFile!.Name);
    }

    [Fact]
    public async Task RemovesFileFromListWhenRemoveClicked()
    {
        // Arrange
        var cut = RenderFileUpload();
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromText("x", "temp.txt"));

        // Act
        var removeBtn = cut.Find(".sb-file-upload__item-remove");
        await cut.InvokeAsync(() => removeBtn!.Click());

        // Assert
        Assert.Empty(cut.FindAll(".sb-file-upload__list"));
    }

    [Fact]
    public void DoesNotRenderFileListWhenNoFiles()
    {
        // Arrange & Act
        var cut = RenderFileUpload();

        // Assert
        Assert.Empty(cut.FindAll(".sb-file-upload__list"));
    }

    [Fact]
    public async Task AddsDraggingClassOnDragEnter()
    {
        // Arrange
        var cut = RenderFileUpload();
        var dropzone = cut.Find(".sb-file-upload__dropzone");

        // Act
        await dropzone!.TriggerEventAsync("ondragenter", new DragEventArgs());

        // Assert
        Assert.Contains("sb-file-upload__dropzone--dragging", cut.Find(".sb-file-upload__dropzone").ClassList);
    }

    [Fact]
    public async Task DoesNotAddDraggingClassWhenDisabled()
    {
        // Arrange
        var cut = RenderFileUpload(p => p.Add(x => x.Disabled, true));
        var dropzone = cut.Find(".sb-file-upload__dropzone");

        // Act
        await dropzone!.TriggerEventAsync("ondragenter", new DragEventArgs());

        // Assert - HandleDragEnter returns early when Disabled
        Assert.DoesNotContain("sb-file-upload__dropzone--dragging", cut.Find(".sb-file-upload__dropzone").ClassList);
    }

    [Fact]
    public async Task SkipsFilesLargerThanMaxFileSize()
    {
        // Arrange
        IReadOnlyList<IBrowserFile>? received = null;
        var cut = RenderFileUpload(p => p
            .Add(x => x.MaxFileSizeBytes, 10L) // 10 bytes max
            .Add(x => x.OnFilesSelected, EventCallback.Factory.Create<IReadOnlyList<IBrowserFile>>(this, f => received = f)));

        // Act - "this is way more than 10 bytes" is 28 chars
        var fileContent = InputFileContent.CreateFromText("this is way more than 10 bytes", "big.txt");
        cut.FindComponent<InputFile>().UploadFiles(fileContent);

        // Assert - file should be skipped (too large)
        Assert.NotNull(received);
        Assert.Empty(received!);
    }

    [Fact]
    public async Task AcceptsFileWithinMaxFileSize()
    {
        // Arrange
        IReadOnlyList<IBrowserFile>? received = null;
        var cut = RenderFileUpload(p => p
            .Add(x => x.MaxFileSizeBytes, 1024L)
            .Add(x => x.OnFilesSelected, EventCallback.Factory.Create<IReadOnlyList<IBrowserFile>>(this, f => received = f)));

        var fileContent = InputFileContent.CreateFromText("small", "tiny.txt");

        // Act
        cut.FindComponent<InputFile>().UploadFiles(fileContent);

        // Assert
        Assert.NotNull(received);
        Assert.Single(received!);
    }
}
