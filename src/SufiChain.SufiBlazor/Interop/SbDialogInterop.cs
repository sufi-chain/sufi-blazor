using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SufiChain.SufiBlazor.Interop;

/// <summary>
/// Minimal JS interop for native dialog element operations.
/// Uses native HTMLDialogElement methods which cannot be called from C#.
/// </summary>
public sealed class SbDialogInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private
        IJSObjectReference? _module;

    public SbDialogInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync()
    {
        _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/SufiChain.SufiBlazor/sufiblazor.js");
        return _module;
    }

    /// <summary>
    /// Opens a dialog as modal using the native showModal() method.
    /// </summary>
    public async ValueTask ShowModalAsync(ElementReference dialogElement)
    {
        await _jsRuntime.InvokeVoidAsync("SufiBlazor.dialog.showModal", dialogElement);
    }

    /// <summary>
    /// Closes a dialog using the native close() method.
    /// </summary>
    public async ValueTask CloseAsync(ElementReference dialogElement)
    {
        await _jsRuntime.InvokeVoidAsync("SufiBlazor.dialog.close", dialogElement);
    }

    /// <summary>
    /// Sets focus on an element.
    /// </summary>
    public async ValueTask FocusAsync(ElementReference element)
    {
        await _jsRuntime.InvokeVoidAsync("SufiBlazor.focus.set", element);
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}
