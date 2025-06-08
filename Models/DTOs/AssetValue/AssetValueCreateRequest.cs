using System.ComponentModel.DataAnnotations;

namespace Mynt.Models.DTOs.AssetValue;

/// <summary>
/// Request DTO for creating a new asset value record
/// </summary>
public class AssetValueCreateRequest
{
    /// <summary>
    /// The ID of the asset this value belongs to
    /// </summary>
    [Required]
    public required int AssetId { get; set; }
    
    /// <summary>
    /// The monetary value of the asset at the recorded time
    /// </summary>
    [Required]
    public required decimal Value { get; set; }
    
    /// <summary>
    /// The date and time when this value was recorded
    /// </summary>
    [Required]
    public required DateTime RecordedAt { get; set; }
} 