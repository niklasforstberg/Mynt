namespace Mynt.Models.DTOs.AssetType;

/// <summary>
/// Response DTO for asset type translations
/// </summary>
public class AssetTypeTranslationResponse
{
    /// <summary>
    /// The unique identifier for the translation
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The translated name
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// The language code (e.g., en, es, fr, de)
    /// </summary>
    public required string LanguageCode { get; set; }
} 