using System.Globalization;
using System.Text.RegularExpressions;

namespace SufiChain.SufiBlazor.Tests.Components.Feedback;

public class SbAlertContrastTests
{
    private static readonly string[] Variants = ["danger", "warning", "info", "success"];

    [Fact]
    public void AlertPairsMeetWcagAaInBothThemes()
    {
        var css = File.ReadAllText(AlertStylesheetPath()).Replace("\r\n", "\n", StringComparison.Ordinal);
        var light = AlertPairs(ExtractBlock(css, ":root,\n.sb-theme-root {"));
        var dark = AlertPairs(ExtractBlock(css, "html.sufi-theme-dark .sb-theme-root,"));

        Assert.Equal(Variants.OrderBy(name => name), light.Keys.OrderBy(name => name));
        Assert.Equal(Variants.OrderBy(name => name), dark.Keys.OrderBy(name => name));

        foreach (var variant in Variants)
        {
            AssertPair(variant, light[variant], darkTextOnPaleFill: true);
            AssertPair(variant, dark[variant], darkTextOnPaleFill: false);
            Assert.Contains($"background-color: var(--sb-color-alert-{variant}-bg);", css, StringComparison.Ordinal);
            Assert.Contains($"color: var(--sb-color-alert-{variant}-text);", css, StringComparison.Ordinal);
        }

        var alertRules = css[css.IndexOf("/* SbAlert */", StringComparison.Ordinal)..css.IndexOf("/* SbProgress */", StringComparison.Ordinal)];
        Assert.Contains(".sb-alert__title,\n.sb-alert__message {\n    color: inherit;", alertRules, StringComparison.Ordinal);
        Assert.DoesNotContain("color: var(--sb-color-text)", alertRules, StringComparison.Ordinal);
    }

    private static void AssertPair(string variant, (string Background, string Text) pair, bool darkTextOnPaleFill)
    {
        var background = RelativeLuminance(pair.Background);
        var text = RelativeLuminance(pair.Text);
        var contrast = Contrast(background, text);
        Assert.True(contrast >= 4.5, $"{variant} contrast {contrast.ToString("0.00", CultureInfo.InvariantCulture)} is below 4.5:1 ({pair.Text} on {pair.Background}).");
        if (darkTextOnPaleFill)
        {
            Assert.True(text < background, $"{variant} light theme should use darker text on a pale fill.");
        }
        else
        {
            Assert.True(text > background, $"{variant} dark theme should use lighter text on a deep fill.");
        }
    }

    private static Dictionary<string, (string Background, string Text)> AlertPairs(string block)
    {
        var pairs = new Dictionary<string, (string Background, string Text)>(StringComparer.Ordinal);
        foreach (Match match in Regex.Matches(block, @"--sb-color-alert-(danger|warning|info|success)-(bg|text)\s*:\s*(#[0-9A-Fa-f]{6})\s*;"))
        {
            var variant = match.Groups[1].Value;
            var role = match.Groups[2].Value;
            var color = match.Groups[3].Value;
            pairs.TryGetValue(variant, out var pair);
            pairs[variant] = role == "bg" ? (color, pair.Text) : (pair.Background, color);
        }

        return pairs;
    }

    private static string ExtractBlock(string css, string marker)
    {
        var start = css.IndexOf(marker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing CSS block {marker}");
        var open = css.IndexOf('{', start);
        var depth = 0;
        for (var index = open; index < css.Length; index++)
        {
            if (css[index] == '{')
            {
                depth++;
            }
            else if (css[index] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return css[open..(index + 1)];
                }
            }
        }

        throw new InvalidOperationException($"Unclosed CSS block {marker}");
    }

    private static string AlertStylesheetPath()
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "src",
            "SufiChain.SufiBlazor",
            "wwwroot",
            "sufiblazor.css"));
        Assert.True(File.Exists(path), path);
        return path;
    }

    private static double Contrast(double first, double second)
    {
        var lighter = Math.Max(first, second);
        var darker = Math.Min(first, second);
        return (lighter + 0.05) / (darker + 0.05);
    }

    private static double RelativeLuminance(string hex)
    {
        var value = int.Parse(hex[1..], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return (0.2126 * Channel((value >> 16) & 255))
            + (0.7152 * Channel((value >> 8) & 255))
            + (0.0722 * Channel(value & 255));
    }

    private static double Channel(int component)
    {
        var amount = component / 255d;
        return amount <= 0.03928
            ? amount / 12.92
            : Math.Pow((amount + 0.055) / 1.055, 2.4);
    }
}
