using System.Globalization;
using System.Text;
using Bunit;
using Bunit.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SufiChain.SufiBlazor.Components.Forms;
using SufiChain.SufiBlazor.Interop;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

file class ReadStubStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<LocalizedString>();
}

public class SbCodeEditorReadTests : BunitContext
{
    public SbCodeEditorReadTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new ReadStubStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public async Task SbCodeEditor_ReadValue_136KbCssWith35kCharLine_RoundTripsExactly()
    {
        var text = new string('a', 136 * 1024);
        text = string.Concat(new string('b', 35_000), text.AsSpan(35_000));
        var read = await ReadLikeTheEditorAsync(text);
        Assert.Equal(text, read);
        Assert.Contains(new string('b', 35_000), read, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SbCodeEditor_ReadValue_136KbPlus5KbAppend_RoundTripsExactly()
    {
        var text = new string('c', (136 * 1024) + (5 * 1024));
        var read = await ReadLikeTheEditorAsync(text);
        Assert.Equal(text, read);
        Assert.Equal((136 * 1024) + (5 * 1024), read.Length);
    }

    [Fact]
    public async Task SbCodeEditor_ChunkReadFails_RaisesTransferFailedAndKeepsDirty()
    {
        var cut = Render<SbCodeEditor>(parameters => parameters.Add(editor => editor.Value, "body{}"));
        await cut.InvokeAsync(() => cut.Instance.OnEditorDirtyChanged(true, 6));
        await cut.InvokeAsync(() => cut.Instance.OnEditorTransferFailed("read"));

        Assert.Equal(SbCodeEditorTransferState.Failed, cut.Instance.TransferState);
        Assert.True(cut.Instance.IsDirty);
    }

    private static Task<string> ReadLikeTheEditorAsync(string text)
    {
        return SbEditorTextTransfer.ReadInChunksAsync(text.Length, (offset, take) =>
        {
            if (take > SbEditorTextTransfer.SingleMessageChars)
            {
                throw new InvalidOperationException("A single editor message exceeded 32 KB.");
            }

            var chunk = text.Substring(offset, take);
            if (Encoding.UTF8.GetByteCount(chunk) > 32 * 1024)
            {
                throw new InvalidOperationException("A single editor message exceeded 32 KB.");
            }

            return Task.FromResult<string?>(chunk);
        });
    }
}
