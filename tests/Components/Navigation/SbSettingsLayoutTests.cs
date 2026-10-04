using System.Globalization;
using Bunit;
using Bunit.JSInterop;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
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
        Assert.False(disabled.HasAttribute("disabled"));
        var describedBy = disabled.GetAttribute("aria-describedby");
        Assert.False(string.IsNullOrWhiteSpace(describedBy));
        var reason = cut.Find($"#{describedBy}");
        Assert.Contains("Module is off", reason.TextContent);
        Assert.Equal("sb-sr-only", reason.ClassName);
        Assert.Null(disabled.QuerySelector($"#{describedBy}"));
        Assert.Equal("true", disabled.QuerySelector(".sb-settings-item__reason")?.GetAttribute("aria-hidden"));
        Assert.Contains("Module is off", disabled.TextContent);
        Assert.Equal("Time zone", disabled.GetAttribute("title"));
        Assert.Equal("Time zone", disabled.GetAttribute("aria-label"));

        disabled.Click();
        Assert.Equal("email", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Contains("Module is off", cut.Find(".sb-settings-reason-popover").TextContent);
        Assert.Contains("section=email", Services.GetRequiredService<NavigationManager>().Uri);

        cut.Find(".sb-settings-reason-backdrop").Click();
        Assert.Empty(cut.FindAll(".sb-settings-reason-popover"));

        disabled.Click();
        Assert.NotEmpty(cut.FindAll(".sb-settings-reason-popover"));
        disabled.KeyDown(new KeyboardEventArgs { Key = "Escape" });
        Assert.Empty(cut.FindAll(".sb-settings-reason-popover"));
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
    public void DirtyGuard_ShowsThreeActionsAndFocusesKeepEditing()
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
            Section(builder, 20, "identity", "Identity", "user-cog", dirty: true, onSave: () =>
            {
                saved++;
                return Task.FromResult(true);
            });
        });

        cut.Find(".sb-settings-rail [data-section-id='identity']").Click();

        var title = cut.Find(".sb-dialog__title");
        Assert.Equal("Settings:GuardTitle", title.TextContent);
        Assert.DoesNotContain("Confirm", title.TextContent, StringComparison.Ordinal);
        Assert.Empty(cut.FindAll(".sb-dialog__description"));

        var stay = cut.Find(".sb-settings-guard__stay");
        var discard = cut.Find(".sb-settings-guard__discard");
        var save = cut.Find(".sb-settings-guard__save");
        Assert.Equal("Settings:Stay", stay.TextContent.Trim());
        Assert.Equal("true", stay.GetAttribute("autofocus"));
        Assert.Equal("Settings:Discard", discard.TextContent.Trim());
        Assert.Equal("Settings:Save", save.TextContent.Trim());
        Assert.Contains("sb-button--solid", save.ClassName);
        Assert.Contains("sb-button--primary", save.ClassName);
        Assert.DoesNotContain("sb-button--solid", stay.ClassName);
        Assert.DoesNotContain("sb-button--solid", discard.ClassName);

        stay.Click();
        Assert.Equal("email", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Empty(cut.FindAll(".sb-dialog__title"));
        Assert.Equal(0, saved);
        Assert.Equal(0, discarded);

        cut.Find(".sb-settings-rail [data-section-id='identity']").Click();
        cut.Find(".sb-settings-guard__discard").Click();
        Assert.Equal(1, discarded);
        Assert.Equal("identity", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Equal(0, saved);

        cut.Find(".sb-settings-rail [data-section-id='email']").Click();
        cut.Find(".sb-settings-guard__save").Click();
        Assert.Equal(1, saved);
        Assert.Equal("email", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
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

    [Fact]
    public void PendingSections_KeepTheDeepLinkUntilTheyResolveOutOfOrder()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("http://localhost/settings?section=email&keep=1");
        var cut = Render<PendingSectionsHost>();

        Assert.NotEmpty(cut.FindAll(".sb-settings-pending.sb-loading-skeleton"));
        Assert.DoesNotContain("Settings:EmptyTitle", cut.Markup);
        Assert.Empty(cut.FindAll("[aria-current='page']"));
        Assert.Contains("section=email", nav.Uri);
        Assert.Contains("keep=1", nav.Uri);
        Assert.DoesNotContain("section=branding", nav.Uri);

        cut.Render(parameters => parameters.Add(component => component.BrandingPending, false).Add(component => component.EmailPending, true));

        Assert.Empty(cut.FindAll("[aria-current='page']"));
        Assert.NotEmpty(cut.FindAll(".sb-settings-pending.sb-loading-skeleton"));
        Assert.DoesNotContain("Settings:EmptyTitle", cut.Markup);
        Assert.Contains("section=email", nav.Uri);
        Assert.DoesNotContain("section=branding", nav.Uri);

        cut.Render(parameters => parameters.Add(component => component.BrandingPending, false).Add(component => component.EmailPending, false));

        Assert.Equal("email", cut.Find("[aria-current='page']").GetAttribute("data-section-id"));
        Assert.Contains("Email body", cut.Markup);
        Assert.Empty(cut.FindAll(".sb-settings-pending"));
        Assert.Contains("section=email", nav.Uri);
        Assert.Contains("keep=1", nav.Uri);
    }

    [Fact]
    public void ActiveSectionChange_RunsTheDirtyGuard()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () => Task.FromResult(true), onDiscard: () => Task.CompletedTask);
            Section(builder, 20, "identity", "Identity", "user-cog");
        }, parameters => parameters.Add(component => component.ActiveSection, "email"));

        cut.Render(parameters =>
        {
            parameters.Add(component => component.AriaLabel, "Settings");
            parameters.Add(component => component.ActiveSection, "identity");
            parameters.Add(component => component.ChildContent, (RenderFragment)(builder =>
            {
                Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () => Task.FromResult(true), onDiscard: () => Task.CompletedTask);
                Section(builder, 20, "identity", "Identity", "user-cog");
            }));
        });

        Assert.NotNull(cut.Find(".sb-settings-guard__stay"));
        Assert.Equal("email", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Contains("section=email", Services.GetRequiredService<NavigationManager>().Uri);

        cut.Find(".sb-settings-guard__discard").Click();
        Assert.Equal("identity", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Contains("section=identity", Services.GetRequiredService<NavigationManager>().Uri);
    }

    [Fact]
    public void SamePathSectionQuery_RunsTheDirtyGuardThenUpdatesTheUrl()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("http://localhost/settings?section=email");
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () => Task.FromResult(true), onDiscard: () => Task.CompletedTask);
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        nav.NavigateTo("http://localhost/settings?section=identity&keep=1");
        Assert.Contains("section=email", nav.Uri);
        Assert.NotNull(cut.Find(".sb-settings-guard__stay"));
        Assert.Equal("email", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));

        cut.Find(".sb-settings-guard__stay").Click();
        Assert.Equal("email", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
        Assert.Equal("true", cut.Find(".sb-settings-rail [data-section-id='identity']").GetAttribute("data-keyboard-focus"));

        nav.NavigateTo("http://localhost/settings?section=identity&keep=1");
        cut.Find(".sb-settings-guard__discard").Click();
        Assert.Equal("identity", cut.Find("[aria-current='page']").GetAttribute("data-section-id"));
        Assert.Contains("section=identity", nav.Uri);
        Assert.Contains("keep=1", nav.Uri);
    }

    [Fact]
    public void SamePathSectionQuery_SwitchesWhenTheSectionIsClean()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("http://localhost/settings?section=email");
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail");
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        nav.NavigateTo("http://localhost/settings?section=identity");
        cut.WaitForAssertion(() =>
        {
            Assert.Equal("identity", cut.Find(".sb-settings-rail [aria-current='page']").GetAttribute("data-section-id"));
            Assert.Contains("section=identity", nav.Uri);
        });
        Assert.Empty(cut.FindAll(".sb-settings-guard__stay"));
    }

    [Fact]
    public void BeforeUnload_ArmsAndClearsWithTheDirtyFlag()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () => Task.FromResult(true), onDiscard: () => Task.CompletedTask);
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        Assert.Contains(JSInterop.Invocations, invocation => BeforeUnload(invocation, armed: true));

        cut.Find(".sb-settings-savebar__discard").Click();
        Assert.Contains(JSInterop.Invocations, invocation => BeforeUnload(invocation, armed: false));
    }

    [Fact]
    public void SectionSwitch_ScrollsTheStripItemAndPointsTheRegionAtTheHeading()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail");
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        cut.Find(".sb-settings-rail [data-section-id='identity']").Click();
        var heading = cut.Find("h2.sb-settings-savebar__title");
        Assert.Equal("Identity", heading.TextContent.Trim());
        Assert.Equal("-1", heading.GetAttribute("tabindex"));
        Assert.Equal(heading.Id, cut.Find(".sb-settings-section--active").GetAttribute("aria-labelledby"));
        Assert.Equal("region", cut.Find(".sb-settings-section--active").GetAttribute("role"));
        Assert.Contains(JSInterop.Invocations, invocation =>
            invocation.Identifier == "SufiBlazor.settingsLayout.scrollIntoView"
            && invocation.Arguments.Any(argument => argument?.ToString()?.Contains("identity") == true));
    }

    [Fact]
    public void SingleSection_HidesNavigationAndKeepsTheSaveBar()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", onSave: () => Task.FromResult(true));
        });

        Assert.Contains("sb-settings-layout--single", cut.Find(".sb-settings-layout").ClassList);
        Assert.Empty(cut.FindAll("nav"));
        Assert.NotEmpty(cut.FindAll(".sb-settings-savebar__save"));
    }

    [Fact]
    public void AutoCompactMode_UsesThePickerAtSevenSections()
    {
        var cut = RenderLayout(null, builder =>
        {
            for (var index = 0; index < 7; index++)
            {
                Section(builder, index * 20, $"section-{index}", $"Section {index}", "mail");
            }
        });

        Assert.Contains("sb-settings-layout--picker", cut.Find(".sb-settings-layout").ClassList);
        Assert.NotEmpty(cut.FindAll(".sb-settings-picker"));
        Assert.Empty(cut.FindAll(".sb-settings-strip"));
        var stripNav = cut.Find("nav.sb-settings-rail");
        Assert.Equal("Settings", stripNav.GetAttribute("aria-label"));
        Assert.Equal(7, cut.FindAll(".sb-settings-rail li").Count);
    }

    [Fact]
    public void Strip_IsANamedNavigationList()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail");
            Section(builder, 20, "identity", "Identity", "user-cog");
        }, parameters => parameters.Add(component => component.CompactMode, SbSettingsCompactMode.Strip));

        var nav = cut.Find("nav.sb-settings-strip-wrap");
        Assert.Equal("Settings", nav.GetAttribute("aria-label"));
        Assert.Equal(2, nav.QuerySelectorAll("ul > li").Count());
    }

    [Fact]
    public void OnSaveNull_HidesSaveAndKeepsBarActions()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", barActions: content => content.AddMarkupContent(0, "<button type=\"button\" class=\"add-action\">Add</button>"));
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        Assert.Empty(cut.FindAll(".sb-settings-savebar__save"));
        Assert.Equal("Add", cut.Find(".add-action").TextContent);
    }

    [Fact]
    public void CanSaveFalse_KeepsSaveDisabledWhileDirty()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, canSave: false, onSave: () => Task.FromResult(true));
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        Assert.True(cut.Find(".sb-settings-savebar__save").HasAttribute("disabled"));
        Assert.Contains("Settings:UnsavedChanges", cut.Markup);
    }

    [Fact]
    public void FailedSaveFromTheBar_KeepsDirtyAndShowsTheAlert()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () => Task.FromResult(false));
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        cut.Find(".sb-settings-savebar__save").Click();
        Assert.Contains("Settings:SaveFailed", cut.Markup);
        Assert.Contains("Settings:UnsavedChanges", cut.Markup);
        Assert.Equal("email", cut.Find("[aria-current='page']").GetAttribute("data-section-id"));
    }

    [Fact]
    public void Busy_DisablesTheRailAndSave()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () => Task.FromResult(true));
            Section(builder, 20, "identity", "Identity", "user-cog");
        }, parameters => parameters.Add(component => component.Busy, true));

        Assert.True(cut.Find(".sb-settings-rail [data-section-id='identity']").HasAttribute("disabled"));
        Assert.True(cut.Find(".sb-settings-savebar__save").HasAttribute("disabled"));
    }

    [Fact]
    public void DeepLinkRewrite_UsesReplaceHistory()
    {
        var nav = (BunitNavigationManager)Services.GetRequiredService<NavigationManager>();
        RenderLayout("http://localhost/settings?section=missing&keep=1", builder =>
        {
            Section(builder, 0, "identity", "Identity", "user-cog");
        });

        var rewritten = nav.History.Last(entry => entry.Uri.Contains("section=identity", StringComparison.Ordinal));
        Assert.True(rewritten.Options.ReplaceHistoryEntry);
        Assert.Contains("keep=1", rewritten.Uri);
    }

    [Fact]
    public void Guard_ResumesNavigationWithTheHistoryEntryState()
    {
        var nav = (BunitNavigationManager)Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("http://localhost/settings?section=email");
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () => Task.FromResult(true), onDiscard: () => Task.CompletedTask);
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        nav.NavigateTo("http://localhost/elsewhere", new NavigationOptions { HistoryEntryState = "{\"return\":1}" });
        Assert.NotNull(cut.Find(".sb-settings-guard__stay"));
        cut.Find(".sb-settings-guard__discard").Click();

        var resumed = nav.History.Last(entry => entry.Uri.Contains("/elsewhere", StringComparison.Ordinal));
        Assert.Equal("{\"return\":1}", resumed.Options.HistoryEntryState);
        Assert.False(resumed.Options.ReplaceHistoryEntry);
    }

    [Fact]
    public async Task Save_IgnoresASecondClickWhileTheFirstSaveIsRunning()
    {
        var gate = new TaskCompletionSource<bool>();
        var saved = 0;
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail", dirty: true, onSave: () =>
            {
                saved++;
                return gate.Task;
            });
            Section(builder, 20, "identity", "Identity", "user-cog");
        });

        var first = cut.Find(".sb-settings-savebar__save").ClickAsync();
        cut.Find(".sb-settings-savebar__save").Click();
        Assert.Equal(1, saved);
        gate.SetResult(true);
        await first;
        Assert.Equal(1, saved);
    }

    [Fact]
    public void Keyboard_HomeAndEndMoveToTheEnds()
    {
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail");
            Section(builder, 20, "identity", "Identity", "user-cog");
            Section(builder, 40, "links", "Links", "link");
        });

        cut.Find(".sb-settings-rail [data-section-id='email']").KeyDown(new KeyboardEventArgs { Key = "End" });
        Assert.Equal("true", cut.Find(".sb-settings-rail [data-section-id='links']").GetAttribute("data-keyboard-focus"));
        cut.Find(".sb-settings-rail [data-section-id='links']").KeyDown(new KeyboardEventArgs { Key = "Home" });
        Assert.Equal("true", cut.Find(".sb-settings-rail [data-section-id='email']").GetAttribute("data-keyboard-focus"));
    }

    [Fact]
    public void PendingChoice_UserSelectionSurvivesALaterDeepLinkResolution()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("http://localhost/settings?section=email&keep=1");
        var cut = Render<PendingChoiceHost>();

        Assert.Empty(cut.FindAll("[aria-current='page']"));
        Assert.Contains("section=email", nav.Uri);

        cut.Find(".sb-settings-rail [data-section-id='branding']").Click();
        Assert.Equal("branding", cut.Find("[aria-current='page']").GetAttribute("data-section-id"));

        cut.Render(parameters => parameters.Add(component => component.EmailPending, false));

        Assert.Equal("branding", cut.Find("[aria-current='page']").GetAttribute("data-section-id"));
        Assert.Contains("Branding body", cut.Markup);
        Assert.Contains("section=branding", nav.Uri);
        Assert.Contains("keep=1", nav.Uri);
        Assert.DoesNotContain("section=email", nav.Uri);
    }

    [Fact]
    public void Stay_NotifiesTheBoundActiveSection()
    {
        var cut = Render<BoundSectionHost>();

        cut.Find(".sb-settings-rail [data-section-id='identity']").Click();
        Assert.NotNull(cut.Find(".sb-settings-guard__stay"));
        Assert.Equal("email", cut.Instance.Active);

        cut.Find(".sb-settings-guard__stay").Click();
        Assert.Equal("email", cut.Instance.Active);
        Assert.Equal("email", cut.Find("[aria-current='page']").GetAttribute("data-section-id"));
        Assert.Empty(cut.FindAll(".sb-settings-guard__stay"));
    }

    [Fact]
    public void SamePathUnknownOrDisabledSection_RewritesBackToTheActiveId()
    {
        var nav = (BunitNavigationManager)Services.GetRequiredService<NavigationManager>();
        nav.NavigateTo("http://localhost/settings?section=email&keep=1");
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "email", "Email", "mail");
            Section(builder, 20, "identity", "Identity", "user-cog", disabled: true, reason: "Feature is off");
        });

        nav.NavigateTo("http://localhost/settings?section=missing&keep=1");
        Assert.Equal("email", cut.Find("[aria-current='page']").GetAttribute("data-section-id"));
        Assert.Contains("section=email", nav.Uri);
        Assert.Contains("keep=1", nav.Uri);
        Assert.DoesNotContain("section=missing", nav.Uri);
        Assert.Contains(nav.History, entry =>
            entry.Uri.Contains("section=email", StringComparison.Ordinal)
            && entry.Options.ReplaceHistoryEntry);

        nav.NavigateTo("http://localhost/settings?section=identity&keep=1");
        Assert.Equal("email", cut.Find("[aria-current='page']").GetAttribute("data-section-id"));
        Assert.Contains("section=email", nav.Uri);
        Assert.DoesNotContain("section=identity", nav.Uri);
        Assert.Contains("Feature is off", cut.Find(".sb-settings-reason-popover").TextContent);

        nav.NavigateTo("http://localhost/settings?section=&keep=1");
        Assert.Contains("section=email", nav.Uri);
        Assert.Contains("keep=1", nav.Uri);
    }

    [Fact]
    public void RailLabel_ExposesTheFullText()
    {
        const string label = "تنظیمات منطقه زمانی برای همه کاربران";
        var cut = RenderLayout(null, builder =>
        {
            Section(builder, 0, "timezone", label, "clock");
            Section(builder, 20, "email", "Email", "mail");
        });

        var item = cut.Find(".sb-settings-rail [data-section-id='timezone']");
        Assert.Equal(label, item.GetAttribute("title"));
        Assert.Equal(label, item.GetAttribute("aria-label"));
        Assert.Equal(label, item.QuerySelector(".sb-settings-item__label")?.TextContent);
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
        bool canSave = true,
        bool pending = false,
        RenderFragment? barActions = null)
    {
        builder.OpenComponent<SbSettingsSection>(sequence);
        builder.AddAttribute(sequence + 1, "Id", id);
        builder.AddAttribute(sequence + 2, "Label", label);
        builder.AddAttribute(sequence + 3, "Icon", icon);
        builder.AddAttribute(sequence + 4, "Visible", visible);
        builder.AddAttribute(sequence + 5, "ReadOnly", readOnly);
        builder.AddAttribute(sequence + 6, "Disabled", disabled);
        builder.AddAttribute(sequence + 7, "Pending", pending);
        if (reason != null)
        {
            builder.AddAttribute(sequence + 8, "DisabledReason", reason);
        }

        builder.AddAttribute(sequence + 9, "IsDirty", dirty);
        builder.AddAttribute(sequence + 10, "CanSave", canSave);
        if (onSave != null)
        {
            builder.AddAttribute(sequence + 11, "OnSave", onSave);
        }

        if (onDiscard != null)
        {
            builder.AddAttribute(sequence + 12, "OnDiscard", onDiscard);
        }

        if (barActions != null)
        {
            builder.AddAttribute(sequence + 13, "BarActions", barActions);
        }

        builder.AddAttribute(sequence + 14, "ChildContent", (RenderFragment)(content =>
            content.AddMarkupContent(0, body ?? $"<p>{id} body</p>")));
        builder.CloseComponent();
    }

    private static bool BeforeUnload(JSRuntimeInvocation invocation, bool armed) =>
        invocation.Identifier == "SufiBlazor.settingsLayout.setBeforeUnload"
        && invocation.Arguments.Count > 1
        && invocation.Arguments[1] is bool value
        && value == armed;
}

file sealed class PendingChoiceHost : ComponentBase
{
    [Parameter]
    public bool EmailPending { get; set; } = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<SbSettingsLayout>(0);
        builder.AddAttribute(1, "AriaLabel", "Settings");
        builder.AddAttribute(2, "ChildContent", (RenderFragment)(content =>
        {
            content.OpenComponent<SbSettingsSection>(0);
            content.AddAttribute(1, "Id", "branding");
            content.AddAttribute(2, "Label", "Branding");
            content.AddAttribute(3, "Icon", "palette");
            content.AddAttribute(4, "Pending", false);
            content.AddAttribute(5, "ChildContent", (RenderFragment)(body => body.AddMarkupContent(0, "<p>Branding body</p>")));
            content.CloseComponent();

            content.OpenComponent<SbSettingsSection>(20);
            content.AddAttribute(21, "Id", "email");
            content.AddAttribute(22, "Label", "Email");
            content.AddAttribute(23, "Icon", "mail");
            content.AddAttribute(24, "Pending", EmailPending);
            content.AddAttribute(25, "ChildContent", (RenderFragment)(body => body.AddMarkupContent(0, "<p>Email body</p>")));
            content.CloseComponent();
        }));
        builder.CloseComponent();
    }
}

file sealed class BoundSectionHost : ComponentBase
{
    public string? Active { get; set; } = "email";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<SbSettingsLayout>(0);
        builder.AddAttribute(1, "AriaLabel", "Settings");
        builder.AddAttribute(2, "ActiveSection", Active);
        builder.AddAttribute(3, "ActiveSectionChanged", EventCallback.Factory.Create<string>(this, value => Active = value));
        builder.AddAttribute(4, "ChildContent", (RenderFragment)(content =>
        {
            content.OpenComponent<SbSettingsSection>(0);
            content.AddAttribute(1, "Id", "email");
            content.AddAttribute(2, "Label", "Email");
            content.AddAttribute(3, "Icon", "mail");
            content.AddAttribute(4, "IsDirty", true);
            content.AddAttribute(5, "OnSave", (Func<Task<bool>>)(() => Task.FromResult(true)));
            content.AddAttribute(6, "OnDiscard", (Func<Task>)(() => Task.CompletedTask));
            content.AddAttribute(7, "ChildContent", (RenderFragment)(body => body.AddMarkupContent(0, "<p>Email body</p>")));
            content.CloseComponent();

            content.OpenComponent<SbSettingsSection>(20);
            content.AddAttribute(21, "Id", "identity");
            content.AddAttribute(22, "Label", "Identity");
            content.AddAttribute(23, "Icon", "user-cog");
            content.AddAttribute(24, "ChildContent", (RenderFragment)(body => body.AddMarkupContent(0, "<p>Identity body</p>")));
            content.CloseComponent();
        }));
        builder.CloseComponent();
    }
}

file sealed class PendingSectionsHost : ComponentBase
{
    [Parameter]
    public bool BrandingPending { get; set; } = true;

    [Parameter]
    public bool EmailPending { get; set; } = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<SbSettingsLayout>(0);
        builder.AddAttribute(1, "AriaLabel", "Settings");
        builder.AddAttribute(2, "ChildContent", (RenderFragment)(content =>
        {
            content.OpenComponent<SbSettingsSection>(0);
            content.AddAttribute(1, "Id", "branding");
            content.AddAttribute(2, "Label", "Branding");
            content.AddAttribute(3, "Icon", "palette");
            content.AddAttribute(4, "Pending", BrandingPending);
            content.AddAttribute(5, "ChildContent", (RenderFragment)(body => body.AddMarkupContent(0, "<p>Branding body</p>")));
            content.CloseComponent();

            content.OpenComponent<SbSettingsSection>(20);
            content.AddAttribute(21, "Id", "email");
            content.AddAttribute(22, "Label", "Email");
            content.AddAttribute(23, "Icon", "mail");
            content.AddAttribute(24, "Pending", EmailPending);
            content.AddAttribute(25, "ChildContent", (RenderFragment)(body => body.AddMarkupContent(0, "<p>Email body</p>")));
            content.CloseComponent();
        }));
        builder.CloseComponent();
    }
}

public class SettingsStripFadeContractTests
{
    [Fact]
    public void Strip_fade_attributes_are_true_and_the_masks_match_presence()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root != null && !File.Exists(Path.Combine(root.FullName, "src", "SufiChain.SufiBlazor", "wwwroot", "sufiblazor.js")))
        {
            root = root.Parent;
        }

        Assert.NotNull(root);
        var js = File.ReadAllText(Path.Combine(root!.FullName, "src", "SufiChain.SufiBlazor", "wwwroot", "sufiblazor.js"));
        var css = File.ReadAllText(Path.Combine(root.FullName, "src", "SufiChain.SufiBlazor", "wwwroot", "sufiblazor.css"));

        Assert.Contains("setAttribute(name, \"true\")", js, StringComparison.Ordinal);
        Assert.Contains("removeAttribute(name)", js, StringComparison.Ordinal);
        Assert.DoesNotContain("toggleAttribute(\"data-fade-start\"", js, StringComparison.Ordinal);
        Assert.DoesNotContain("toggleAttribute(\"data-fade-end\"", js, StringComparison.Ordinal);
        Assert.Contains("[data-fade-start]:not([data-fade-end])", css, StringComparison.Ordinal);
        Assert.Contains("[data-fade-end]:not([data-fade-start])", css, StringComparison.Ordinal);
        Assert.Contains("[data-fade-start][data-fade-end]", css, StringComparison.Ordinal);
        Assert.DoesNotContain("[data-fade-start=\"true\"]", css, StringComparison.Ordinal);
        Assert.DoesNotContain("[data-fade-end=\"true\"]", css, StringComparison.Ordinal);
        Assert.Contains(".sb-settings-rail__item .sb-settings-item__label", css, StringComparison.Ordinal);
        Assert.Contains("-webkit-line-clamp: 2", css, StringComparison.Ordinal);
        Assert.Contains(".sb-settings-item--disabled > .sb-settings-item__label", css, StringComparison.Ordinal);
        Assert.Contains("color: var(--sb-color-text-muted)", css, StringComparison.Ordinal);
        Assert.Contains(".sb-settings-reason-backdrop", css, StringComparison.Ordinal);
        Assert.Contains("@media (min-width: 769px)", css, StringComparison.Ordinal);
    }
}

public class SettingsLeaveGuardCopyTests
{
    [Fact]
    public void Leave_guard_title_is_the_localized_question()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root != null && !File.Exists(Path.Combine(root.FullName, "src", "SufiChain.SufiBlazor", "Localization", "SufiBlazorResource.resx")))
        {
            root = root.Parent;
        }

        Assert.NotNull(root);
        var localization = Path.Combine(root!.FullName, "src", "SufiChain.SufiBlazor", "Localization");

        Assert.Contains(
            "<data name=\"Settings:GuardTitle\" xml:space=\"preserve\"><value>Save your changes before you go?</value></data>",
            File.ReadAllText(Path.Combine(localization, "SufiBlazorResource.resx")),
            StringComparison.Ordinal);
        Assert.Contains(
            "<data name=\"Settings:GuardTitle\" xml:space=\"preserve\"><value>پیش از رفتن، تغییرات ذخیره شود؟</value></data>",
            File.ReadAllText(Path.Combine(localization, "SufiBlazorResource.fa.resx")),
            StringComparison.Ordinal);
        Assert.Contains(
            "<data name=\"Settings:GuardTitle\" xml:space=\"preserve\"><value>هل تريد حفظ التغييرات قبل المغادرة؟</value></data>",
            File.ReadAllText(Path.Combine(localization, "SufiBlazorResource.ar.resx")),
            StringComparison.Ordinal);
        Assert.Contains(
            "<data name=\"Settings:GuardTitle\" xml:space=\"preserve\"><value>¿Quieres guardar los cambios antes de salir?</value></data>",
            File.ReadAllText(Path.Combine(localization, "SufiBlazorResource.es.resx")),
            StringComparison.Ordinal);
    }
}
