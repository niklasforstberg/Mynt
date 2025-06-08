using System.ComponentModel.DataAnnotations;

namespace Mynt.Models.DTOs.AssetValue;

/// <summary>
/// Request DTO for updating an existing asset value record
/// </summary>
public class AssetValueUpdateRequest
{
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