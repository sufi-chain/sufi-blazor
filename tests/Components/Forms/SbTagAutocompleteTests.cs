using System.Globalization;
using Bunit;
using Bunit.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SufiChain.SufiBlazor.Components.Forms;
using SufiChain.SufiBlazor.Localization;
using Xunit;

namespace SufiChain.SufiBlazor.Tests.Components.Forms;

file class TagAutocompleteStringLocalizer : IStringLocalizer<SufiBlazorResource>
{
    public LocalizedString this[string name] => new(name, name);
    public LocalizedString this[string name, params object[] arguments] =>
        new(name, string.Format(CultureInfo.InvariantCulture, name, arguments));
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        Array.Empty<LocalizedString>();
}

public class SbTagAutocompleteTests : BunitContext
{
    public SbTagAutocompleteTests()
    {
        Services.AddSingleton<IStringLocalizer<SufiBlazorResource>>(new TagAutocompleteStringLocalizer());
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void RapidSearchInputPreservesCompleteBrowserValue()
    {
        var cut = Render<SbTagAutocomplete<TagAutocompleteTestItem>>(parameters => parameters
            .Add(component => component.MinLength, 1)
            .Add(component => component.DebounceMs, 0)
            .Add(component => component.TextField,
                (Func<TagAutocompleteTestItem, string>)(item => item.Name))
            .Add(component => component.SearchFunc,
                (Func<string, Task<IEnumerable<TagAutocompleteTestItem>>>)(query =>
                    Task.FromResult<IEnumerable<TagAutocompleteTestItem>>(
                        [new TagAutocompleteTestItem { Id = 1, Name = query }]))));

        var input = cut.Find(".sb-tag-autocomplete__input");
        input.Input("a");
        input.Input("ar");
        input.Input("ari");
        input.Input("aria");

        Assert.Equal("aria", cut.Find(".sb-tag-autocomplete__input").GetAttribute("value"));
    }

    private sealed class TagAutocompleteTestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
