namespace SufiChain.SufiBlazor.Contracts.Editors;

/// <summary>
/// Result of an image upload operation in the rich text editor.
/// </summary>
public class SbImageUploadResult
{
    /// <summary>
    /// Whether the upload was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The URL of the uploaded image (if successful).
    /// </summary>
    public string? Url { get; set; }
    
    /// <summary>
    /// Alternative text for the image.
    /// </summary>
    public string? Alt { get; set; }
    
    /// <summary>
    /// Error message if the upload failed.
    /// </summary>
    public string? Error { get; set; }
    
    /// <summary>
    /// Creates a successful upload result.
    /// </summary>
    public static SbImageUploadResult Succeeded(string url, string? alt = null) => new()
    {
        Success = true,
        Url = url,
        Alt = alt
    };
    
    /// <summary>
    /// Creates a failed upload result.
    /// </summary>
    public static SbImageUploadResult Failed(string error) => new()
    {
        Success = false,
        Error = error
    };
}
