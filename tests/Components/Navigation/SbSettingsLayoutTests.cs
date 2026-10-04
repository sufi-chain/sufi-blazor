using System.Globalization;
using Bunit;
using Bunit.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SufiChain.SufiBlazor.Components.Navigation;
using SufiChain.SufiBlazor.Localization;

namespace SufiChain.SufiBlazor.Tests.Components.Navigation;

file class SettingsLayoutLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);

    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];
}

public class SbSettingsLayoutTests : BunitContext
{
    public SbSettingsLayoutTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new SettingsLayoutLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void DeepLink_SelectsRequestedSectionAndKeepsOtherKeys()
    {
        var cut = RenderLayout("http://localhost/settings?section=email&keep=1", builder =>
        {
            Section(builder, 0, "identity", "Identity", "user-cog");
            Section(builder, 20, "email", "Email", "mail", body: "<p>Email body</p>");
        });

        var current = cut.Find(".sb-settings-rail [aria-current='page']");
        Assert.Equal("email", current.GetAttribute("data-section-id"));
        Assert.Contains("Email body", cut.Markup);
        var uri = Services.GetRequiredService<NavigationManager>().Uri;
        Assert.Contains("section=email", uri);
        Assert.Contains("keep=1", uri);
    }

    [Fact]
    public void DeepLink_UnknownOrHiddenIdFallsBackAndRewritesUrl()
    {
        var cut = RenderLayout("http://localhost/settings?section=missing&keep=1", builder =>
        {
            Section(builder, 0, "secret", "Secret", "lock", visible: false);
            Section(builder, 20, "identity", "Identity", "user-cog");
            Section(builder, 40, "email", "Email", "mail");
        });

        Assert.Equal("identity", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Empty(cut.FindAll(".sb-settings-rail [data-section-id='secret']"));
        Assert.Empty(cut.FindAll(".sb-settings-rail [data-section-id='missing']"));
        var uri = Services.GetRequiredService<NavigationManager>().Uri;
        Assert.Contains("section=identity", uri);
        Assert.Contains("keep=1", uri);
        Assert.DoesNotContain("section=missing", uri);
    }

    [Fact]
    public void DeepLink_DisabledSectionFallsBack()
    {
        var cut = RenderLayout("http://localhost/settings?section=timezone", builder =>
        {
            Section(builder, 0, "email", "Email", "mail");
            Section(builder, 20, "timezone", "Time zone", "clock", disabled: true, reason: "Module is off");
        });

        Assert.Equal("email", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        var disabled = cut.Find(".sb-settings-rail [data-section-id='timezone']");
        Assert.Equal("true", disabled.GetAttribute("aria-disabled"));
        Assert.Contains("Module is off", disabled.TextContent);
        Assert.Contains("section=email", Services.GetRequiredService<NavigationManager>().Uri);
    }

    [Fact]
    public void ReadOnlySection_ShowsViewOnlyBadgeAndNoSave()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", readOnly: true, onSave: () => Task.FromResult(true));
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        Assert.Contains("Settings:ViewOnly", cut.Markup);
        Assert.Empty(cut.FindAll(".sb-settings-savebar__save"));
        Assert.Empty(cut.FindAll(".sb-settings-savebar__discard"));
    }

    [Fact]
    public void DirtyGuard_StayDiscardAndSave()
    {
        var saved = 0;
        var discarded = 0;
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true,
                onSave: () =>
                {
                    saved++;
                    return Task.FromResult(true);
                },
                onDiscard: () =>
                {
                    discarded++;
                    return Task.CompletedTask;
                });
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        cut.Find(".sb-settings-rail [data-section-id='identity']").Click();
        Assert.NotNull(cut.Find(".sb-settings-guard__stay"));
        Assert.Equal("email", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));

        cut.Find(".sb-settings-guard__stay").Click();
        Assert.Equal("email", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Empty(cut.FindAll(".sb-settings-guard__stay"));

        cut.Find(".sb-settings-rail [data-section-id='identity']").Click();
        cut.Find(".sb-settings-guard__discard").Click();
        Assert.Equal(1, discarded);
        Assert.Equal("identity", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Equal(0, saved);
    }

    [Fact]
    public void DirtyGuard_SaveContinuesWhenTheSaveSucceeds()
    {
        var saved = 0;
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () =>
            {
                saved++;
                return Task.FromResult(true);
            });
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        cut.Find(".sb-settings-rail [data-section-id='identity']").Click();
        cut.Find(".sb-settings-guard__save").Click();
        Assert.Equal(1, saved);
        Assert.Equal("identity", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Contains("Settings:Saved", cut.Markup);
    }

    [Fact]
    public void DirtyGuard_FailedSaveStaysOnTheSection()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () => Task.FromResult(false),
                body: "<input aria-invalid=\"true\" />");
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        cut.Find(".sb-settings-rail [data-section-id='identity']").Click();
        cut.Find(".sb-settings-guard__save").Click();

        Assert.Equal("email", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Contains("Settings:SaveFailed", cut.Markup);
        Assert.Empty(cut.FindAll(".sb-settings-guard__save"));
    }

    [Fact]
    public void DirtyGuard_BlocksInAppNavigationUntilDiscard()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("http://localhost/settings?section=email");
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () => Task.FromResult(true), onDiscard: () => Task.CompletedTask);
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        nav.NavigateTo("http://localhost/elsewhere");
        Assert.Contains("/settings", nav.Uri);
        Assert.NotNull(cut.Find(".sb-settings-guard__stay"));

        cut.Find(".sb-settings-guard__discard").Click();
        Assert.Contains("/elsewhere", nav.Uri);
    }

    [Fact]
    public void SaveStaysDisabledUntilTheSectionIsDirty()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", onSave: () => Task.FromResult(true));
            Section(builder, 20, "identity", "Identity", "user-cog", onSave: () => Task.FromResult(true));
        });

        var save = cut.Find(".sb-settings-savebar__save");
        Assert.True(save.HasAttribute("disabled"));
    }

    [Fact]
    public void Keyboard_ArrowDownMovesFocusAndRtlMirrorsTheStrip()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail");
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        cut.Find(".sb-settings-rail [data-section-id='email']")
            .KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Assert.Equal("true", cut.Find(".sb-settings-rail [data-section-id='identity']").GetAttribute("data-keyboard-focus"));

        cut.Instance.OnDirectionChanged(true);
        cut.Find(".sb-settings-strip [data-section-id='email']")
            .KeyDown(new KeyboardEventArgs { Key = "ArrowLeft" });
        Assert.Equal("true", cut.Find(".sb-settings-strip [data-section-id='identity']").GetAttribute("data-keyboard-focus"));
    }

    [Fact]
    public void CompactMode_PickerClassOverridesTheSectionCount()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail");
            Section(builder, 20, "identity", "Identity", "user-cog");
        }, parameters => parameters.Add(component => component.CompactMode, SbSettingsCompactMode.Picker));

        Assert.Contains("sb-settings-layout--picker", cut.Find(".sb-settings-layout").ClassList);
    }

    private IRenderedComponent<SbSettingsLayout> RenderLayout(
        string? startUrl,
        Action<Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder> sections,
        Action<ComponentParameterCollectionBuilder<SbSettingsLayout>>? configure = null)
    {
        if (startUrl != null)
        {
            Services.GetRequiredService<NavigationManager>().NavigateTo(startUrl);
        }

        return Render<SbSettingsLayout>(parameters =>
        {
            parameters.Add(component => component.AriaLabel, "Settings");
            configure?.Invoke(parameters);
            parameters.Add(component => component.ChildContent, (RenderFragment)(builder => sections(builder)));
        });
    }

    private static void Section(
        Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder,
        int sequence,
        string id,
        string label,
        string icon,
        bool visible = true,
        bool readOnly = false,
        bool disabled = false,
        string? reason = null,
        bool dirty = false,
        Func<Task<bool>>? onSave = null,
        Func<Task>? onDiscard = null,
        string? body = null,
        bool canSave = true)
    {
        builder.OpenComponent<SbSettingsSection>(sequence);
        builder.AddAttribute(sequence + 1, "Id", id);
        builder.AddAttribute(sequence + 2, "Label", label);
        builder.AddAttribute(sequence + 3, "Icon", icon);
        builder.AddAttribute(sequence + 4, "Visible", visible);
        builder.AddAttribute(sequence + 5, "ReadOnly", readOnly);
        builder.AddAttribute(sequence + 6, "Disabled", disabled);
        if (reason != null)
        {
            builder.AddAttribute(sequence + 7, "DisabledReason", reason);
        }

        builder.AddAttribute(sequence + 8, "IsDirty", dirty);
        builder.AddAttribute(sequence + 9, "CanSave", canSave);
        if (onSave != null)
        {
            builder.AddAttribute(sequence + 10, "OnSave", onSave);
        }

        if (onDiscard != null)
        {
            builder.AddAttribute(sequence + 11, "OnDiscard", onDiscard);
        }

        builder.AddAttribute(sequence + 12, "ChildContent", (RenderFragment)(content =>
            content.AddMarkupContent(0, body ?? $"<p>{id} body</p>")));
        builder.CloseComponent();
    }
}
