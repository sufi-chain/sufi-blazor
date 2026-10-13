using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SufiChain.SufiBlazor.Tests;

public class ResxKeyParityTests
{
    private static readonly Regex PlaceholderRegex = new(@"\{\d+\}", RegexOptions.Compiled);

    [Theory]
    [InlineData("SufiBlazorResource.fa.resx")]
    [InlineData("SufiBlazorResource.ar.resx")]
    [InlineData("SufiBlazorResource.es.resx")]
    public void CultureResxHasEveryBaseKeyWithNonEmptyValueAndMatchingPlaceholders(string cultureFileName)
    {
        var localizationDir = FindLocalizationDirectory();
        var baseEntries = LoadResxEntries(Path.Combine(localizationDir, "SufiBlazorResource.resx"));
        var cultureEntries = LoadResxEntries(Path.Combine(localizationDir, cultureFileName));

        Assert.NotEmpty(baseEntries);

        var missing = baseEntries.Keys
            .Where(key => !cultureEntries.ContainsKey(key))
            .OrderBy(key => key, StringComparer.Ordinal)
            .ToList();
        Assert.True(
            missing.Count == 0,
            $"{cultureFileName} is missing keys: {string.Join(", ", missing)}");

        foreach (var (key, baseValue) in baseEntries)
        {
            Assert.True(
                cultureEntries.TryGetValue(key, out var cultureValue),
                $"{cultureFileName} is missing key '{key}'.");
            Assert.False(
                string.IsNullOrWhiteSpace(cultureValue),
                $"{cultureFileName} key '{key}' has an empty value.");

            var basePlaceholders = ExtractPlaceholders(baseValue);
            var culturePlaceholders = ExtractPlaceholders(cultureValue);
            Assert.Equal(basePlaceholders, culturePlaceholders);
        }
    }

    private static string FindLocalizationDirectory()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root != null &&
               !File.Exists(Path.Combine(root.FullName, "src", "SufiChain.SufiBlazor", "Localization", "SufiBlazorResource.resx")))
        {
            root = root.Parent;
        }

        Assert.NotNull(root);
        return Path.Combine(root!.FullName, "src", "SufiChain.SufiBlazor", "Localization");
    }

    private static Dictionary<string, string> LoadResxEntries(string path)
    {
        Assert.True(File.Exists(path), $"Resx file not found: {path}");

        var document = XDocument.Load(path);
        return document.Root!
            .Elements("data")
            .Select(element =>
            {
                var name = element.Attribute("name")?.Value;
                var value = element.Element("value")?.Value ?? string.Empty;
                return (Name: name, Value: value);
            })
            .Where(entry => !string.IsNullOrEmpty(entry.Name))
            .ToDictionary(entry => entry.Name!, entry => entry.Value, StringComparer.Ordinal);
    }

    private static string[] ExtractPlaceholders(string value) =>
        PlaceholderRegex.Matches(value)
            .Select(match => match.Value)
            .OrderBy(placeholder => placeholder, StringComparer.Ordinal)
            .ToArray();
}
