using SufiChain.SufiBlazor.Icons;

namespace SufiChain.SufiBlazor.Tests.Icons;

public class SufiIconsTests
{
    [Fact]
    public void SufiBlazorIconExists()
    {
        var icon = SufiIcons.GetIcon("sufi-blazor");

        Assert.NotNull(icon);
        Assert.Equal("sufi-blazor", icon.Name);
        Assert.Equal(SiIconCategory.Design, icon.Category);
        Assert.Contains("viewBox=\"0 0 24 24\"", icon.Svg);
        Assert.Contains("currentColor", icon.Svg);
    }

    [Fact]
    public void SufiBlazorIconCanUseSiPrefix()
    {
        Assert.True(SufiIcons.Exists("si-sufi-blazor"));
    }
}
