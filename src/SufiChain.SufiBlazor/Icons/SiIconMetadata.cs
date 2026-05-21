namespace SufiChain.SufiBlazor.Icons;

/// <summary>
/// Metadata for a Sufi Icon (si) including name, category, description, and SVG content.
/// </summary>
/// <param name="Name">The kebab-case icon name (e.g., "chevron-down").</param>
/// <param name="Category">The category this icon belongs to.</param>
/// <param name="Description">A human-readable description of the icon.</param>
/// <param name="Svg">The complete SVG markup string for the icon.</param>
public record SiIconMetadata(
    string Name,
    SiIconCategory Category,
    string Description,
    string Svg
);
