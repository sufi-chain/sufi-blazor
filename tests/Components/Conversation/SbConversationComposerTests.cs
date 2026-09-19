using Bunit;
using Microsoft.AspNetCore.Components;
using SufiChain.SufiBlazor.Components.Conversation;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Conversation;

public class SbConversationComposerTests : BunitContext
{
    public SbConversationComposerTests()
    {
        JSInterop.SetupVoid("SufiBlazor.conversationComposer.setValue", _ => true);
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
            : initialDraft + Environment.NewLine + "متن ضبط شده";
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
        Assert.Equal("aria", textarea.GetAttribute("value"));
    }

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
}
