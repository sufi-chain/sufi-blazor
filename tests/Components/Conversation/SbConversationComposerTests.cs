using Bunit;
using Microsoft.AspNetCore.Components;
using SufiChain.SufiBlazor.Components.Conversation;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Conversation;

public class SbConversationComposerTests : BunitContext
{
    public SbConversationComposerTests()
    {
        // The composer clears the textarea through this call before raising OnSend.
        JSInterop.SetupVoid("SufiBlazor.conversationComposer.setValue", _ => true).SetVoidResult();
        JSInterop.SetupVoid(
            "SufiBlazor.conversationComposer.bindEnterToSend",
            _ => true);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Existing draft")]
    public async Task Transcript_Remains_For_Review_And_Is_Sent_Only_On_The_Next_Click(string initialDraft)
    {
        var bodies = new List<string>();
        var values = new List<string>();
        var transcript = string.IsNullOrEmpty(initialDraft)
            ? "متن ضبط شده"
            : initialDraft + "\n" + "متن ضبط شده";
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.Value, initialDraft)
            .Add(p => p.CanSend, true)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, value => values.Add(value)))
            .Add(p => p.OnSend, EventCallback.Factory.Create<SbConversationSendRequest>(this, async request =>
            {
                bodies.Add(request.Body);
                if (bodies.Count == 1)
                {
                    await Task.Yield();
                    request.DraftAfterSend = transcript;
                }
            })));

        await cut.Find(".sb-conversation-composer__send").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        Assert.Single(bodies);
        Assert.Equal(transcript, Assert.Single(values));
        Assert.Equal(transcript, cut.Find("textarea").GetAttribute("value"));

        await cut.Find(".sb-conversation-composer__send").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        Assert.Equal(transcript, bodies[1]);
        Assert.Equal(string.Empty, values[1]);
        Assert.Equal(string.Empty, cut.Find("textarea").GetAttribute("value"));
    }

    [Fact]
    public async Task RetainDraftWhileSending_Keeps_Text_Disabled_Until_The_Host_Clears_It()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.Value, "Ask the model")
            .Add(p => p.RetainDraftWhileSending, true)
            .Add(p => p.OnSend, EventCallback.Factory.Create<SbConversationSendRequest>(this, async request =>
            {
                Assert.Equal("Ask the model", request.Body);
                started.TrySetResult();
                await release.Task;
            })));

        var click = cut.Find(".sb-conversation-composer__send").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cut.WaitForAssertion(() =>
        {
            var textarea = cut.Find("textarea");
            Assert.Equal("Ask the model", textarea.GetAttribute("value"));
            Assert.True(textarea.HasAttribute("disabled"));
            Assert.Contains("sb-conversation-composer--awaiting", cut.Markup);
            Assert.Equal("true", cut.Find(".sb-conversation-composer__send").GetAttribute("aria-busy"));
        });

        release.TrySetResult();
        await click;

        var afterSend = cut.Find("textarea");
        Assert.Equal("Ask the model", afterSend.GetAttribute("value"));
        Assert.False(afterSend.HasAttribute("disabled"));

        cut.Render(parameters => parameters
            .Add(p => p.Value, string.Empty)
            .Add(p => p.RetainDraftWhileSending, true));

        Assert.Equal(string.Empty, cut.Find("textarea").GetAttribute("value"));
    }

    [Fact]
    public async Task Submit_Latch_Clears_When_The_Host_Renders_During_Send()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.Value, "Ask the model")
            .Add(p => p.RetainDraftWhileSending, true)
            .Add(p => p.OnSend, EventCallback.Factory.Create<SbConversationSendRequest>(this, async _ =>
            {
                started.TrySetResult();
                await release.Task;
            })));

        var click = cut.Find(".sb-conversation-composer__send").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));

        cut.Render(parameters => parameters
            .Add(p => p.Value, "Ask the model")
            .Add(p => p.RetainDraftWhileSending, true)
            .Add(p => p.IsSending, false));
        Assert.True(cut.Find("textarea").HasAttribute("disabled"));

        release.TrySetResult();
        await click;

        var textarea = cut.Find("textarea");
        Assert.False(textarea.HasAttribute("disabled"));
        Assert.DoesNotContain("sb-conversation-composer--awaiting", cut.Markup);
    }

    [Fact]
    public void Renders_InField_Action_Clusters_Without_External_Toolbar()
    {
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.Placeholder, "Write a message")
            .Add(p => p.StartActions, (RenderFragment)(b => b.AddMarkupContent(0, "<span class=\"start-addon\">attach</span>")))
            .Add(p => p.OverflowActions, (RenderFragment)(b => b.AddMarkupContent(0, "<span class=\"overflow-addon\">ai</span>")))
            .Add(p => p.EndActions, (RenderFragment)(b => b.AddMarkupContent(0, "<span class=\"end-addon\">mic</span>"))));

        Assert.Contains("sb-conversation-composer__shell", cut.Markup);
        Assert.Contains("sb-conversation-composer__actions", cut.Markup);
        Assert.Contains("sb-conversation-composer__field", cut.Markup);
        Assert.Contains("start-addon", cut.Markup);
        Assert.Contains("overflow-addon", cut.Markup);
        Assert.Contains("end-addon", cut.Markup);
        Assert.DoesNotContain("sb-conversation-composer__toolbar", cut.Markup);

        var start = cut.Find(".sb-conversation-composer__actions-start");
        var end = cut.Find(".sb-conversation-composer__actions-end");
        Assert.Contains("start-addon", start.InnerHtml);
        Assert.Contains("overflow-addon", start.InnerHtml);
        Assert.DoesNotContain("end-addon", start.InnerHtml);
        Assert.Contains("end-addon", end.InnerHtml);
        Assert.Contains("sb-conversation-composer__send", end.InnerHtml);
        Assert.DoesNotContain("overflow-addon", end.InnerHtml);
    }

    [Fact]
    public async Task Sending_Shows_Busy_Button_And_Blocks_Enter_Submission()
    {
        var sends = 0;
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.Value, "Pending message")
            .Add(p => p.IsSending, true)
            .Add(p => p.OnSend, EventCallback.Factory.Create<SbConversationSendRequest>(
                this, _ => sends++)));

        var button = cut.Find(".sb-conversation-composer__send");
        Assert.True(button.HasAttribute("disabled"));
        Assert.Equal("true", button.GetAttribute("aria-busy"));
        Assert.Single(button.QuerySelectorAll(".sb-icon-button__spinner"));
        await cut.InvokeAsync(() => cut.Instance.OnEnterSendAsync());
        Assert.Equal(0, sends);
    }

    [Fact]
    public void Rapid_Input_Reports_Complete_Browser_Value()
    {
        var reportedValues = new List<string>();
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(
                this,
                value => reportedValues.Add(value))));

        var textarea = cut.Find("textarea");
        textarea.Input("a");
        textarea.Input("ar");
        textarea.Input("ari");
        textarea.Input("aria");

        Assert.Equal(["a", "ar", "ari", "aria"], reportedValues);
        // sufi-chain#386: typing never re-renders the value, so a slow circuit cannot
        // write an older value back over characters the user typed since.
        Assert.Equal(string.Empty, cut.Find("textarea").GetAttribute("value"));
        Assert.Equal(0, SetValueCalls());
    }

    [Fact]
    public void Host_Rerender_While_Typing_Does_Not_Rewrite_The_Textarea()
    {
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.Value, string.Empty));

        cut.Find("textarea").Input("تست");
        // A poll, presence tick or timeline update re-renders the host with its stale Value.
        cut.Render(parameters => parameters
            .Add(p => p.Value, string.Empty)
            .Add(p => p.Placeholder, "Write a message"));
        cut.Find("textarea").Input("تست ۱");
        cut.Render(parameters => parameters
            .Add(p => p.Value, string.Empty)
            .Add(p => p.Placeholder, "Write a message"));

        Assert.Equal(string.Empty, cut.Find("textarea").GetAttribute("value"));
        Assert.Equal(0, SetValueCalls());
    }

    /// <summary>
    /// HelpDesk replies and the live-chat operator composer echo the draft on the next host
    /// render. That echo must not advance the textarea value attribute (sufi-chain#533 / #386).
    /// </summary>
    [Theory]
    [InlineData("", "The quick brown fox jumps over the lazy dog.")]
    [InlineData("", "تست ۱ با اینتر سریع")]
    [InlineData("draft", "draft plus")]
    public void Bind_Echo_Leaves_The_Rendered_Value_Untouched(string initial, string typed)
    {
        var value = initial;
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.Value, value)
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, next => value = next)));

        Assert.Equal(initial, cut.Find("textarea").GetAttribute("value"));

        for (var i = 1; i <= typed.Length; i++)
        {
            cut.Find("textarea").Input(typed[..i]);
            var echoed = value;
            cut.Render(parameters => parameters
                .Add(p => p.Value, echoed)
                .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, next => value = next)));
            Assert.Equal(initial, cut.Find("textarea").GetAttribute("value"));
        }

        Assert.Equal(typed, value);
        Assert.Equal(0, SetValueCalls());
    }

    [Fact]
    public async Task Human_Chat_Stays_Editable_While_Sending_And_Keeps_The_Follow_Up()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var values = new List<string>();
        var bodies = new List<string>();
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.ValueChanged, EventCallback.Factory.Create<string>(this, value => values.Add(value)))
            .Add(p => p.OnSend, EventCallback.Factory.Create<SbConversationSendRequest>(this, async request =>
            {
                bodies.Add(request.Body);
                started.TrySetResult();
                await release.Task;
            })));

        cut.Find("textarea").Input("پیام اول");
        var click = cut.Find(".sb-conversation-composer__send").ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5));

        // The send is still running (a slow circuit or server). The field must accept typing.
        cut.WaitForAssertion(() => Assert.False(cut.Find("textarea").HasAttribute("disabled")));
        cut.Find("textarea").Input("پیام دوم");

        release.TrySetResult();
        await click;

        Assert.Equal("پیام اول", Assert.Single(bodies));
        Assert.Equal("پیام دوم", values[^1]);
        // Only the clear before the send touched the DOM; the follow-up was not wiped.
        Assert.Equal(1, SetValueCalls());
        Assert.False(cut.Find("textarea").HasAttribute("disabled"));
    }

    [Fact]
    public void Host_Clear_After_Typing_Resets_The_Dom_Value()
    {
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.Value, "draft"));

        cut.Find("textarea").Input("draft edited");
        cut.Render(parameters => parameters.Add(p => p.Value, string.Empty));

        Assert.Equal(string.Empty, cut.Find("textarea").GetAttribute("value"));
        Assert.Contains(JSInterop.Invocations, invocation =>
            invocation.Identifier == "SufiBlazor.conversationComposer.setValue"
            && Equals(invocation.Arguments[1], string.Empty));
    }

    private int SetValueCalls() =>
        JSInterop.Invocations.Count(invocation =>
            invocation.Identifier == "SufiBlazor.conversationComposer.setValue");

    [Fact]
    public void Timeline_Renders_Messages()
    {
        var messages = new List<SbConversationMessageModel>
        {
            new()
            {
                Id = "1",
                Body = "Hello",
                Alignment = SbConversationAlignment.Start
            }
        };

        var cut = Render<SbConversationTimeline>(parameters => parameters
            .Add(p => p.Messages, messages));

        Assert.Contains("Hello", cut.Markup);
        Assert.Contains("sb-conversation-message", cut.Markup);
    }

    [Fact]
    public void Timeline_Renders_Thinking_Bubble_When_Waiting()
    {
        var messages = new List<SbConversationMessageModel>
        {
            new()
            {
                Id = "1",
                Body = "Hello",
                Alignment = SbConversationAlignment.End
            }
        };

        var cut = Render<SbConversationTimeline>(parameters => parameters
            .Add(p => p.Messages, messages)
            .Add(p => p.IsWaitingForResponse, true)
            .Add(p => p.WaitingText, "Thinking…")
            .Add(p => p.WaitingSenderLabel, "Hooshvare"));

        Assert.Contains("sb-conversation-message--thinking", cut.Markup);
        Assert.Contains("Thinking…", cut.Markup);
        Assert.Contains("Hooshvare", cut.Markup);
    }

    [Fact]
    public void Thinking_Bubble_Renders_Dots_And_Text()
    {
        var cut = Render<SbConversationThinkingBubble>(parameters => parameters
            .Add(p => p.Text, "Searching knowledge base…")
            .Add(p => p.SenderLabel, "Assistant"));

        Assert.Contains("sb-conversation-message--thinking", cut.Markup);
        Assert.Contains("sb-conversation-timeline__thinking-dots", cut.Markup);
        Assert.Contains("Searching knowledge base…", cut.Markup);
        Assert.Contains("Assistant", cut.Markup);
    }

    [Fact]
    public void Thinking_Bubble_Renders_Live_Progress_Child_Content()
    {
        var cut = Render<SbConversationThinkingBubble>(parameters => parameters
            .Add(p => p.Text, "Thinking…")
            .Add(p => p.ChildContent, (RenderFragment)(builder =>
                builder.AddMarkupContent(0, "<ul class=\"progress-rows\"><li>searching_kb</li></ul>"))));

        Assert.Contains("sb-conversation-message__bubble--thinking-detail", cut.Markup);
        Assert.Contains("searching_kb", cut.Markup);
    }

    [Fact]
    public void Enter_To_Send_Binds_Once_When_The_First_Render_Succeeds()
    {
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.Value, "Hello")
            .Add(p => p.CanSend, true));

        Assert.Equal(1, BindCalls());

        cut.Render(parameters => parameters
            .Add(p => p.Value, "Hello")
            .Add(p => p.Placeholder, "Write a message"));

        Assert.Equal(1, BindCalls());
    }

    private int BindCalls() =>
        JSInterop.Invocations.Count(invocation =>
            invocation.Identifier == "SufiBlazor.conversationComposer.bindEnterToSend");
}

public class SbConversationComposerEnterRetryTests : BunitContext
{
    public SbConversationComposerEnterRetryTests()
    {
        JSInterop.SetupVoid("SufiBlazor.conversationComposer.setValue", _ => true).SetVoidResult();
        JSInterop.SetupVoid("SufiBlazor.conversationComposer.bindEnterToSend", _ => true)
            .SetException(new InvalidOperationException("Static prerender"));
    }

    [Fact]
    public void Enter_To_Send_Retries_When_The_First_Render_Cannot_Bind()
    {
        var cut = Render<SbConversationComposer>(parameters => parameters
            .Add(p => p.Value, "Hello")
            .Add(p => p.CanSend, true));

        Assert.Equal(1, BindCalls());

        cut.Render(parameters => parameters
            .Add(p => p.Value, "Hello")
            .Add(p => p.Placeholder, "Write a message"));

        Assert.Equal(2, BindCalls());
    }

    private int BindCalls() =>
        JSInterop.Invocations.Count(invocation =>
            invocation.Identifier == "SufiBlazor.conversationComposer.bindEnterToSend");
}
