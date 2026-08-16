using Bunit;
using Microsoft.AspNetCore.Components;
using SufiChain.SufiBlazor.Components.Conversation;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Conversation;

public class SbConversationComposerTests : BunitContext
{
    public SbConversationComposerTests()
    {
        JSInterop.SetupVoid(
            "SufiBlazor.conversationComposer.bindEnterToSend",
            _ => true);
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
        Assert.Contains("start-addon", cut.Markup);
        Assert.Contains("overflow-addon", cut.Markup);
        Assert.Contains("end-addon", cut.Markup);
        Assert.DoesNotContain("sb-conversation-composer__toolbar", cut.Markup);
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
            .Add(p => p.WaitingSenderLabel, "Copilot"));

        Assert.Contains("sb-conversation-message--thinking", cut.Markup);
        Assert.Contains("Thinking…", cut.Markup);
        Assert.Contains("Copilot", cut.Markup);
    }
}
