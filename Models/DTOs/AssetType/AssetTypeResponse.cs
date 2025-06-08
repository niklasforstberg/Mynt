namespace Mynt.Models.DTOs.AssetType;

/// <summary>
/// Response DTO for asset type operations with full details including translations
/// </summary>
public class AssetTypeResponse
{
    /// <summary>
    /// The unique identifier for the asset type
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The default name of the asset type (usually English)
    /// </summary>
    public string DefaultName { get; set; } = "";
    
    /// <summary>
    /// Indicates whether this type represents an asset (true) or liability (false)
    /// </summary>
    public bool IsAsset { get; set; }
    
    /// <summary>
    /// Indicates whether this asset type is physical or digital
    /// </summary>
    public bool IsPhysical { get; set; }
    
    /// <summary>
    /// The date and time when the asset type was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// The date and time when the asset type was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// List of translations for this asset type
    /// </summary>
    public List<AssetTypeTranslationResponse> Translations { get; set; } = new();
} 