namespace Mynt.Models.DTOs.AssetType;

/// <summary>
/// Request DTO for updating an existing asset type
/// </summary>
public class AssetTypeUpdateRequest
{
    /// <summary>
    /// The default name of the asset type (usually English)
    /// </summary>
    public string? DefaultName { get; set; }

    /// <summary>
    /// Indicates whether this type represents an asset (true) or liability (false)
    /// </summary>
    public bool? IsAsset { get; set; }

    /// <summary>
    /// Indicates whether this asset type is physical or digital
    /// </summary>
    public bool? IsPhysical { get; set; }

    /// <summary>
    /// Indicates whether this asset type is easily convertible to cash
    /// </summary>
    public bool? IsLiquid { get; set; }

    /// <summary>
    /// Dictionary of translations where key is language code and value is translated name
    /// </summary>
    public Dictionary<string, string> Translations { get; set; } = new();
}