using Microsoft.JSInterop;

namespace SufiChain.SufiBlazor.Components.Overlays;

public static class DialogInterop
{
    public static async Task InvokeAsync(IJSRuntime js, string identifier, params object?[] args)
    {
        try
        {
            await js.InvokeVoidAsync(identifier, args);
        }
        catch (Exception ex) when (ex is JSException or JSDisconnectedException or ObjectDisposedException)
        {
            // A circuit that is already gone must not raise the generic error bar.
        }
    }
}
