using Bunit;
using Bunit.JSInterop;
using SufiChain.SufiBlazor.Components.Common;
using SufiChain.SufiBlazor.Contracts.Editors;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Common;

public class SbDocumentViewerTests : BunitContext
{
    public SbDocumentViewerTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Completed_reply_stays_in_the_bubble_after_another_render()
    {
        const string reply = "The assistant reply is visible without a reload.";
        var cut = Render<SbDocumentViewer>(p => p
            .Add(x => x.Content, reply)
            .Add(x => x.Format, SbContentFormat.Markdown));

        Assert.Contains(reply, cut.Markup);
        cut.Render();
        Assert.Contains(reply, cut.Markup);
        Assert.NotNull(cut.Find(".sb-document-viewer__source"));
    }

    [Fact]
    public void RendersViewerSurface()
    {
        var cut = Render<SbDocumentViewer>(p => p
            .Add(x => x.Content, "# Hello")
            .Add(x => x.Format, SbContentFormat.Markdown));

        Assert.NotNull(cut.Find(".sb-document-viewer__content"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        var cut = Render<SbDocumentViewer>(p => p.Add(x => x.Class, "chat-body"));

        Assert.Contains("chat-body", cut.Find(".sb-document-viewer").ClassList);
    }

    [Fact]
    public void SetsRtlDirection()
    {
        var cut = Render<SbDocumentViewer>(p => p.Add(x => x.RightToLeft, true));

        Assert.Equal("rtl", cut.Find(".sb-document-viewer").GetAttribute("dir"));
    }

    [Fact]
    public void TextDirectionAutoFollowsContent()
    {
        var cut = Render<SbDocumentViewer>(p => p
            .Add(x => x.RightToLeft, true)
            .Add(x => x.TextDirection, "auto"));

        Assert.Equal("auto", cut.Find(".sb-document-viewer").GetAttribute("dir"));
    }
}
