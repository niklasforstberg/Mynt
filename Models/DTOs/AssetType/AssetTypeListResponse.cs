namespace Mynt.Models.DTOs.AssetType;

/// <summary>
/// Response DTO for asset type list operations
/// </summary>
public class AssetTypeListResponse
{
    /// <summary>
    /// The unique identifier for the asset type
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The display name (either translated or default name)
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Whether this represents an asset (true) or liability (false)
    /// </summary>
    public bool IsAsset { get; set; }
    
    /// <summary>
    /// Whether this is a physical asset
    /// </summary>
    public bool IsPhysical { get; set; }
} 