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

    [Fact]
    public void SufiComIconExists()
    {
        var icon = SufiIcons.GetIcon("sufi-com");

        Assert.NotNull(icon);
        Assert.Equal("sufi-com", icon.Name);
        Assert.Equal(SiIconCategory.Communication, icon.Category);
        Assert.Contains("viewBox=\"0 0 24 24\"", icon.Svg);
        Assert.Contains("currentColor", icon.Svg);
    }

    [Fact]
    public void SufiComIconCanUseSiPrefix()
    {
        Assert.True(SufiIcons.Exists("si-sufi-com"));
    }

    [Fact]
    public void FormIconExists()
    {
        var icon = SufiIcons.GetIcon("form");

        Assert.NotNull(icon);
        Assert.Equal("form", icon.Name);
        Assert.Equal(SiIconCategory.Actions, icon.Category);
        Assert.Equal("SufiForms — form definitions, surveys, and fillable fields", icon.Description);
        Assert.Contains("viewBox=\"0 0 24 24\"", icon.Svg);
        Assert.Contains("currentColor", icon.Svg);
        Assert.Contains("<rect", icon.Svg);
    }

    [Fact]
    public void FormIconCanUseSiPrefix()
    {
        Assert.True(SufiIcons.Exists("si-form"));
    }

    [Theory]
    [InlineData("layout", SiIconCategory.Layout)]
    [InlineData("chat-dots", SiIconCategory.Communication)]
    [InlineData("contact", SiIconCategory.Users)]
    [InlineData("form", SiIconCategory.Actions)]
    public void MenuReferencedIconsExist(string name, SiIconCategory category)
    {
        var icon = SufiIcons.GetIcon(name);

        Assert.NotNull(icon);
        Assert.Equal(name, icon.Name);
        Assert.Equal(category, icon.Category);
        Assert.False(string.IsNullOrWhiteSpace(icon.Svg));
        Assert.Contains("viewBox=\"0 0 24 24\"", icon.Svg);
    }
}
