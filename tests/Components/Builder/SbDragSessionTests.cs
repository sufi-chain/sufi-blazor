using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SufiChain.SufiBlazor.Components.Builder;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Builder;

public class SbDragSessionTests : BunitContext
{
    [Fact]
    public async Task DraggableItem_DragStart_BeginsSessionBeforeAnyJsCall()
    {
        var session = new SbDragSession();
        var js = new PendingConsoleJs();
        SbDropEventArgs? dropped = null;
        Services.AddSingleton<IJSRuntime>(js);
        var cut = Render<DragHost>(parameters => parameters
            .Add(component => component.Session, session)
            .Add(component => component.Data, "deal-1")
            .Add(component => component.OnItemDropped, EventCallback.Factory.Create<SbDropEventArgs>(this, args => dropped = args)));

        var dragStart = cut.Find(".sb-draggable-item").TriggerEventAsync("ondragstart", new DragEventArgs());
        await Task.Delay(30);

        Assert.True(session.IsDragging);
        Assert.Equal("deal-1", session.Data);
        Assert.Equal("crm-deal", session.ItemType);

        await cut.Find(".sb-dropzone").TriggerEventAsync("ondrop", new DragEventArgs());
        Assert.NotNull(dropped);
        Assert.Equal("deal-1", dropped!.Data);

        js.Release();
        await dragStart;
    }

    [Fact]
    public async Task DropZone_DropAfterDragStart_InvokesOnItemDroppedWithData()
    {
        var session = new SbDragSession();
        SbDropEventArgs? dropped = null;
        var cut = Render<DragHost>(parameters => parameters
            .Add(component => component.Session, session)
            .Add(component => component.Data, "deal-1")
            .Add(component => component.OnItemDropped, EventCallback.Factory.Create<SbDropEventArgs>(this, args => dropped = args)));

        await cut.Find(".sb-draggable-item").TriggerEventAsync("ondragstart", new DragEventArgs());
        await cut.Find(".sb-dropzone").TriggerEventAsync("ondragover", new DragEventArgs());
        await cut.Find(".sb-dropzone").TriggerEventAsync("ondrop", new DragEventArgs());

        Assert.NotNull(dropped);
        Assert.Equal("deal-1", dropped!.Data);
        Assert.Equal("crm-deal", dropped.DataType);
    }

    [Fact]
    public async Task DropZone_DropWithoutSession_InvokesOnDropRejected()
    {
        var rejected = false;
        var cut = Render<DragHost>(parameters => parameters
            .Add(component => component.Session, new SbDragSession())
            .Add(component => component.OnDropRejected, EventCallback.Factory.Create(this, () => rejected = true)));

        await cut.Find(".sb-dropzone").TriggerEventAsync("ondrop", new DragEventArgs());

        Assert.True(rejected);
    }

    private sealed class DragHost : ComponentBase
    {
        [Parameter]
        public SbDragSession Session { get; set; } = new();

        [Parameter]
        public object? Data { get; set; }

        [Parameter]
        public EventCallback<SbDropEventArgs> OnItemDropped { get; set; }

        [Parameter]
        public EventCallback OnDropRejected { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<SbDragSession>>(0);
            builder.AddComponentParameter(1, "Value", Session);
            builder.AddComponentParameter(2, "IsFixed", true);
            builder.AddComponentParameter(3, "ChildContent", (RenderFragment)(child =>
            {
                child.OpenComponent<SbDraggableItem>(0);
                child.AddComponentParameter(1, "Data", Data);
                child.AddComponentParameter(2, "ItemType", "crm-deal");
                child.AddComponentParameter(3, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Card")));
                child.CloseComponent();

                child.OpenComponent<SbDropZone>(10);
                child.AddComponentParameter(11, "AcceptedTypes", new[] { "crm-deal" });
                child.AddComponentParameter(12, "OnItemDropped", OnItemDropped);
                child.AddComponentParameter(13, "OnDropRejected", OnDropRejected);
                child.AddComponentParameter(14, "ChildContent", (RenderFragment)(content => content.AddContent(0, "Zone")));
                child.CloseComponent();
            }));
            builder.CloseComponent();
        }
    }

    private sealed class PendingConsoleJs : IJSRuntime
    {
        private readonly TaskCompletionSource<object?> _gate = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public void Release() => _gate.TrySetResult(null);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            if (identifier == "console.log")
            {
                return new ValueTask<TValue>(_gate.Task.ContinueWith(
                    _ => default(TValue)!,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default));
            }

            return ValueTask.FromResult(default(TValue)!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
            InvokeAsync<TValue>(identifier, args);
    }
}
