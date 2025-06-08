namespace Mynt.Models.DTOs.Asset;

/// <summary>
/// Response DTO for asset operations
/// </summary>
public class AssetResponse
{
    /// <summary>
    /// The unique identifier for the asset
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the asset
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Optional description of the asset
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// The ID of the financial group this asset belongs to
    /// </summary>
    public int? FinancialGroupId { get; set; }
    
    /// <summary>
    /// The name of the financial group this asset belongs to
    /// </summary>
    public string? FinancialGroupName { get; set; }
    
    /// <summary>
    /// The ID of the asset type
    /// </summary>
    public int? AssetTypeId { get; set; }
    
    /// <summary>
    /// The name of the asset type
    /// </summary>
    public string? AssetTypeName { get; set; }
    
    /// <summary>
    /// The date and time when the asset was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// The current value of the asset
    /// </summary>
    public decimal? CurrentValue { get; set; }
} 