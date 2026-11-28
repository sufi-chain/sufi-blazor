using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Components.Overlays;
using Xunit;

namespace SufiChain.SufiBlazor.Tests;

public class DialogInteropTests
{
    [Fact]
    public async Task PermissionsModal_SaveSucceeds_NoUnhandledException_AndModalCloses()
    {
        await DialogInterop.InvokeAsync(new ThrowingRuntime(new JSDisconnectedException("gone")), "SufiBlazor.dialog.close", new object());
    }

    [Fact]
    public async Task PermissionsModal_SaveFails_ShowsErrorDialog()
    {
        var runtime = new ThrowingRuntime(new InvalidOperationException("save failed"));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            DialogInterop.InvokeAsync(runtime, "SufiBlazor.dialog.close", new object()));
    }

    private sealed class ThrowingRuntime : IJSRuntime
    {
        private readonly Exception _error;

        public ThrowingRuntime(Exception error) => _error = error;

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
            ValueTask.FromException<TValue>(_error);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            ValueTask.FromException<TValue>(_error);
    }
}
