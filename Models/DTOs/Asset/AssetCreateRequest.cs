using System.ComponentModel.DataAnnotations;

namespace Mynt.Models.DTOs.Asset;

/// <summary>
/// Request DTO for creating a new asset
/// </summary>
public class AssetCreateRequest
{
    /// <summary>
    /// The name of the asset
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Optional description of the asset
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The ID of the financial group this asset belongs to
    /// </summary>
    public int? FinancialGroupId { get; set; }

    /// <summary>
    /// The ID of the asset type
    /// </summary>
    public int? AssetTypeId { get; set; }

    /// <summary>
    /// Optional initial value of the asset. If provided, will create an initial AssetValue record.
    /// </summary>
    public decimal? InitialValue { get; set; }
}