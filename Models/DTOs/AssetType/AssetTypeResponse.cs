namespace Mynt.Models.DTOs.AssetType;

/// <summary>
/// Response DTO for asset type operations
/// </summary>
public class AssetTypeResponse
{
    /// <summary>
    /// The unique identifier for the asset type
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The default name for the asset type
    /// </summary>
    public required string DefaultName { get; set; }
    
    /// <summary>
    /// Whether this represents an asset (true) or liability (false)
    /// </summary>
    public bool IsAsset { get; set; }
    
    /// <summary>
    /// Whether this is a physical asset
    /// </summary>
    public bool IsPhysical { get; set; }
    
    /// <summary>
    /// Available translations for this asset type
    /// </summary>
    public List<AssetTypeTranslationResponse> Translations { get; set; } = new();
} 