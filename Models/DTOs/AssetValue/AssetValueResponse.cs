namespace Mynt.Models.DTOs.AssetValue;

/// <summary>
/// Response DTO for asset value operations
/// </summary>
public class AssetValueResponse
{
    /// <summary>
    /// The unique identifier for the asset value record
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The ID of the asset this value belongs to
    /// </summary>
    public int AssetId { get; set; }
    
    /// <summary>
    /// The name of the asset this value belongs to
    /// </summary>
    public string AssetName { get; set; } = "";
    
    /// <summary>
    /// The monetary value of the asset at the recorded time
    /// </summary>
    public decimal Value { get; set; }
    
    /// <summary>
    /// The date and time when this value was recorded
    /// </summary>
    public DateTime RecordedAt { get; set; }
} 