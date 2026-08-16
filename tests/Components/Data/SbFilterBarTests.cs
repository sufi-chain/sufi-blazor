using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SufiChain.SufiBlazor.Components.Data;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Data;

file class FilterBarStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);

    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        Array.Empty<LocalizedString>();
}

public class SbFilterBarTests : BunitContext
{
    public SbFilterBarTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new FilterBarStringLocalizer());
    }

    [Fact]
    public async Task RapidInputReportsCompleteBrowserValueAfterDebounce()
    {
        var reportedValue = new TaskCompletionSource<string?>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var cut = Render<SbFilterBar>(parameters => parameters
            .Add(p => p.SearchDebounceMs, 50)
            .Add(p => p.SearchValueChanged, EventCallback.Factory.Create<string?>(
                this,
                value => reportedValue.TrySetResult(value))));

        var input = cut.Find(".sb-filter-bar__search-input");
        input.Input("a");
        input.Input("ar");
        input.Input("ari");
        input.Input("aria");

        Assert.Equal(
            "aria",
            await reportedValue.Task.WaitAsync(TimeSpan.FromSeconds(5)));
    }
}
