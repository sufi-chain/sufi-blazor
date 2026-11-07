using SufiChain.SufiBlazor.Components.Forms;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class HtmlFileAcceptTests
{
    private const string CmsMedia = ".jpg,.jpeg,.png,.gif,.webp,.avif,.svg,.ico";

    [Theory]
    [InlineData(".jpg,.jpeg,.png,.gif,.webp,.avif,.svg,.ico", ".jpg,.jpeg,.png,.gif,.webp,.avif,.svg,.ico")]
    [InlineData(".jpg .jpeg .png .gif .webp .avif .svg .ico", ".jpg,.jpeg,.png,.gif,.webp,.avif,.svg,.ico")]
    [InlineData("JPG, JPEG, PNG, GIF, WEBP, AVIF, SVG, ICO", ".jpg,.jpeg,.png,.gif,.webp,.avif,.svg,.ico")]
    [InlineData(" .JPG ; *.JPEG, .png ,.PNG ", ".jpg,.jpeg,.png")]
    [InlineData("image/*, .PDF, image/png, IMAGE/*, application/pdf", "image/*,.pdf,image/png,application/pdf")]
    [InlineData("image/svg+xml, .svg", "image/svg+xml,.svg")]
    [InlineData("audio/*,video/*,IMAGE/*", "audio/*,video/*,image/*")]
    [InlineData("*/*", "*/*")]
    [InlineData(".png, */*", ".png")]
    [InlineData("   ", "")]
    [InlineData(null, "")]
    [InlineData("pdf,zip,7z,rar", ".pdf,.zip,.7z,.rar")]
    [InlineData(".tar.gz,.zip", ".tar.gz,.zip")]
    public void Normalize_EmitsCommaSeparatedAcceptTokens(string? input, string expected)
    {
        var accept = HtmlFileAccept.Normalize(input);

        Assert.Equal(expected, accept);
        Assert.Equal(accept, HtmlFileAccept.Normalize(accept));
        if (expected.Contains(','))
        {
            Assert.DoesNotContain(" ", accept);
        }

        Assert.DoesNotContain("*.*", accept);
    }

    [Fact]
    public void Normalize_KeepsAvifAsItsOwnToken()
    {
        var accept = HtmlFileAccept.Normalize("jpg,jpeg,png,gif,webp,avif");

        Assert.Equal(".jpg,.jpeg,.png,.gif,.webp,.avif", accept);
        Assert.DoesNotContain(".avi,", accept + ",");
        Assert.DoesNotContain(".avi", accept.Split(','));
    }

    [Theory]
    [InlineData("request-1.png", "image/png", true)]
    [InlineData("request-2.PNG", "IMAGE/PNG", true)]
    [InlineData("result.jpg", "image/jpeg", true)]
    [InlineData("PHOTO.JPEG", "image/jpeg", true)]
    [InlineData("anim.GIF", "image/gif", true)]
    [InlineData("hero.webp", "image/webp", true)]
    [InlineData("pic.avif", "image/avif", true)]
    [InlineData("favicon.ICO", "image/vnd.microsoft.icon", true)]
    [InlineData("logo.svg", "image/svg+xml", true)]
    [InlineData("notes.exe", "application/octet-stream", false)]
    [InlineData("archive.zip", "application/zip", false)]
    [InlineData("clip.avi", "video/x-msvideo", false)]
    public void Allows_CmsMediaExtensions_MatchesTheScreenshotFiles(string fileName, string mimeType, bool allowed)
    {
        Assert.Equal(allowed, HtmlFileAccept.Allows(CmsMedia, fileName, mimeType));
        Assert.Equal(allowed, HtmlFileAccept.Allows(".jpg .jpeg .png .gif .webp .avif .svg .ico", fileName, mimeType));
    }

    [Fact]
    public void Allows_ArchiveAndDocumentTokens_WhenThoseTypesAreListed()
    {
        const string accept = "pdf, zip, 7z, application/pdf";

        Assert.True(HtmlFileAccept.Allows(accept, "bundle.ZIP", "application/zip"));
        Assert.True(HtmlFileAccept.Allows(accept, "notes.7z", "application/x-7z-compressed"));
        Assert.True(HtmlFileAccept.Allows(accept, "report.pdf", "application/pdf"));
        Assert.False(HtmlFileAccept.Allows(accept, "photo.png", "image/png"));
    }

    [Fact]
    public void Allows_MimeWildcard_MatchesThatFamilyOnly()
    {
        Assert.True(HtmlFileAccept.Allows("image/*", "photo.png", "image/png"));
        Assert.False(HtmlFileAccept.Allows("image/*", "clip.mp4", "video/mp4"));
        Assert.True(HtmlFileAccept.Allows("video/*", "clip.mp4", "video/mp4"));
        Assert.True(HtmlFileAccept.Allows("audio/*", "voice.mp3", "audio/mpeg"));
    }

    [Fact]
    public void Allows_EmptyOrAllFilesToken_DoesNotAddATypeConstraint()
    {
        Assert.True(HtmlFileAccept.Allows(null, "anything.bin", "application/octet-stream"));
        Assert.True(HtmlFileAccept.Allows("*/*", "anything.bin", "application/octet-stream"));
    }
}
