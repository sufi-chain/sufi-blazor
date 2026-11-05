using System.Globalization;
using Bunit;
using SufiChain.SufiBlazor.Components.Conversation;
using SufiChain.SufiBlazor.Utilities.DateUtils;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Conversation;

public class SbDisplayTimeTests : BunitContext
{
    private static readonly DateTimeOffset Utc = new(2026, 10, 30, 17, 55, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Unspecified")]
    [InlineData("Asia/Tehran")]
    [InlineData("Iran Standard Time")]
    public void CreationTime_17_55Z_displays_as_21_25(string? timeZoneId)
    {
        var text = SbDisplayTime.Format(Utc, "yyyy-MM-dd HH:mm", timeZoneId, CultureInfo.InvariantCulture);
        Assert.Equal("2026-10-30 21:25", text);
        Assert.Equal(TimeSpan.FromMinutes(210), SbDisplayTime.Resolve(timeZoneId).GetUtcOffset(Utc));
    }

    [Fact]
    public void Message_bubble_does_not_use_a_plus_three_offset()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var cut = Render<SbConversationMessage>(parameters => parameters
                .Add(component => component.Body, "hello")
                .Add(component => component.Timestamp, Utc)
                .Add(component => component.TimestampFormat, "HH:mm"));

            Assert.Contains("21:25", cut.Markup);
            Assert.DoesNotContain("20:55", cut.Markup);
            Assert.DoesNotContain("17:55", cut.Markup);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
