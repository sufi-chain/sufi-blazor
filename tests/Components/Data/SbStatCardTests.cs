using Microsoft.AspNetCore.Components;
using Bunit;
using SufiChain.SufiBlazor.Components.Data;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Data;

public class SbStatCardTests : BunitContext
{
    private IRenderedComponent<SbStatCard> RenderStatCard(
        Action<ComponentParameterCollectionBuilder<SbStatCard>>? configure = null)
    {
        return Render<SbStatCard>(p =>
        {
            p.Add(x => x.Label, "Total Revenue")
                .Add(x => x.Value, "1,234");
            configure?.Invoke(p);
        });
    }

    [Fact]
    public void RendersCardStructure()
    {
        // Arrange & Act
        var cut = RenderStatCard();

        // Assert
        var card = cut.Find(".sb-stat-card");
        Assert.NotNull(card);
        Assert.NotNull(cut.Find(".sb-stat-card__content"));
        Assert.NotNull(cut.Find(".sb-stat-card__label"));
        Assert.NotNull(cut.Find(".sb-stat-card__value"));
    }

    [Fact]
    public void RendersLabelAndValue()
    {
        // Arrange & Act
        var cut = RenderStatCard();

        // Assert
        Assert.Contains("Total Revenue", cut.Markup);
        Assert.Contains("1,234", cut.Markup);
    }

    [Fact]
    public void RendersPrefixWhenProvided()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p.Add(x => x.Prefix, "$"));

        // Assert
        var prefix = cut.Find(".sb-stat-card__prefix");
        Assert.NotNull(prefix);
        Assert.Contains("$", prefix.TextContent);
    }

    [Fact]
    public void DoesNotRenderPrefixWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderStatCard();

        // Assert
        Assert.Empty(cut.FindAll(".sb-stat-card__prefix"));
    }

    [Fact]
    public void RendersSuffixWhenProvided()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p.Add(x => x.Suffix, "K"));

        // Assert
        var suffix = cut.Find(".sb-stat-card__suffix");
        Assert.NotNull(suffix);
        Assert.Contains("K", suffix.TextContent);
    }

    [Fact]
    public void DoesNotRenderSuffixWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderStatCard();

        // Assert
        Assert.Empty(cut.FindAll(".sb-stat-card__suffix"));
    }

    [Fact]
    public void RendersIconWhenProvided()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p
            .Add(x => x.Icon, i => i.AddMarkupContent(0, "<span class=\"custom-icon\">📊</span>")));

        // Assert
        var iconContainer = cut.Find(".sb-stat-card__icon");
        Assert.NotNull(iconContainer);
        var customIcon = cut.Find(".custom-icon");
        Assert.NotNull(customIcon);
    }

    [Fact]
    public void DoesNotRenderIconWhenNull()
    {
        // Arrange & Act
        var cut = RenderStatCard();

        // Assert
        Assert.Empty(cut.FindAll(".sb-stat-card__icon"));
    }

    [Fact]
    public void RendersPositiveTrendWithArrowUp()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p.Add(x => x.Trend, 12.5));

        // Assert
        var trend = cut.Find(".sb-stat-card__trend");
        Assert.NotNull(trend);
        Assert.Contains("sb-stat-card__trend--positive", trend.ClassList);
        Assert.Contains("12.5%", cut.Markup);
        Assert.NotNull(cut.Find(".sb-stat-card__trend-icon"));
    }

    [Fact]
    public void RendersNegativeTrendWithArrowDown()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p.Add(x => x.Trend, -8.3));

        // Assert
        var trend = cut.Find(".sb-stat-card__trend");
        Assert.NotNull(trend);
        Assert.Contains("sb-stat-card__trend--negative", trend.ClassList);
        Assert.Contains("8.3%", cut.Markup);
    }

    [Fact]
    public void RendersZeroTrendAsPositive()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p.Add(x => x.Trend, 0.0));

        // Assert
        var trend = cut.Find(".sb-stat-card__trend");
        Assert.NotNull(trend);
        Assert.Contains("sb-stat-card__trend--positive", trend.ClassList);
        Assert.Contains("0%", cut.Markup);
    }

    [Fact]
    public void DoesNotRenderTrendWhenTrendNull()
    {
        // Arrange & Act
        var cut = RenderStatCard();

        // Assert
        Assert.Empty(cut.FindAll(".sb-stat-card__trend"));
    }

    [Fact]
    public void DoesNotRenderTrendWhenShowTrendFalse()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p
            .Add(x => x.Trend, 10.0)
            .Add(x => x.ShowTrend, false));

        // Assert
        Assert.Empty(cut.FindAll(".sb-stat-card__trend"));
    }

    [Fact]
    public void RendersTrendLabelWhenProvided()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p
            .Add(x => x.Trend, 5.0)
            .Add(x => x.TrendLabel, "vs last month"));

        // Assert
        var trendLabel = cut.Find(".sb-stat-card__trend-label");
        Assert.NotNull(trendLabel);
        Assert.Contains("vs last month", trendLabel.TextContent);
    }

    [Fact]
    public void DoesNotRenderTrendLabelWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p.Add(x => x.Trend, 5.0));

        // Assert
        Assert.Empty(cut.FindAll(".sb-stat-card__trend-label"));
    }

    [Fact]
    public void RendersDescriptionWhenProvided()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p.Add(x => x.Description, "Year over year growth"));

        // Assert
        var description = cut.Find(".sb-stat-card__description");
        Assert.NotNull(description);
        Assert.Contains("Year over year growth", description.TextContent);
    }

    [Fact]
    public void DoesNotRenderDescriptionWhenEmpty()
    {
        // Arrange & Act
        var cut = RenderStatCard();

        // Assert
        Assert.Empty(cut.FindAll(".sb-stat-card__description"));
    }

    [Fact]
    public void RendersActionsWhenProvided()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p
            .Add(x => x.Actions, a => a.AddMarkupContent(0, "<button class=\"action-btn\">View</button>")));

        // Assert
        var actionsContainer = cut.Find(".sb-stat-card__actions");
        Assert.NotNull(actionsContainer);
        var actionBtn = cut.Find(".action-btn");
        Assert.NotNull(actionBtn);
        Assert.Contains("View", actionBtn.TextContent);
    }

    [Fact]
    public void DoesNotRenderActionsWhenNull()
    {
        // Arrange & Act
        var cut = RenderStatCard();

        // Assert
        Assert.Empty(cut.FindAll(".sb-stat-card__actions"));
    }

    [Fact]
    public void AppliesCustomClass()
    {
        // Arrange & Act
        var cut = RenderStatCard(p => p.Add(x => x.Class, "my-stat-card"));

        // Assert
        var card = cut.Find(".sb-stat-card");
        Assert.Contains("my-stat-card", card.ClassList);
    }

    [Fact]
    public void RendersCompleteCardWithAllOptionalParts()
    {
        // Arrange & Act
        var cut = Render<SbStatCard>(p => p
            .Add(x => x.Label, "Users")
            .Add(x => x.Value, "2,500")
            .Add(x => x.Suffix, " active")
            .Add(x => x.Icon, i => i.AddMarkupContent(0, "<span>👥</span>"))
            .Add(x => x.Trend, 15.2)
            .Add(x => x.TrendLabel, "vs last week")
            .Add(x => x.Description, "Monthly active users")
            .Add(x => x.Actions, a => a.AddMarkupContent(0, "<a href=\"#\">Details</a>")));

        // Assert
        Assert.Contains("Users", cut.Markup);
        Assert.Contains("2,500", cut.Markup);
        Assert.Contains("active", cut.Markup);
        Assert.Contains("15.2%", cut.Markup);
        Assert.Contains("vs last week", cut.Markup);
        Assert.Contains("Monthly active users", cut.Markup);
        Assert.Contains("Details", cut.Markup);
    }
}
