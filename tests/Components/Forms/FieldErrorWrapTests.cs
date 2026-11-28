namespace SufiChain.SufiBlazor.Tests.Components.Forms;

public class FieldErrorWrapTests
{
    [Fact]
    public void Field_error_wraps_instead_of_clipping()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "src", "SufiChain.SufiBlazor", "wwwroot", "sufiblazor.css");
            if (File.Exists(candidate))
            {
                var css = File.ReadAllText(candidate);
                var start = css.IndexOf(".sb-text-field__error {", StringComparison.Ordinal);
                Assert.True(start >= 0);
                var end = css.IndexOf('}', start);
                var block = css[start..end];
                Assert.Contains("white-space: normal", block);
                Assert.Contains("overflow-wrap: anywhere", block);
                return;
            }

            dir = dir.Parent;
        }

        throw new FileNotFoundException("sufiblazor.css was not found from the test output directory.");
    }
}
