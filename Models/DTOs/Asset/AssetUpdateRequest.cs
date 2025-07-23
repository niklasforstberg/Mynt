using System.ComponentModel.DataAnnotations;

namespace Mynt.Models.DTOs.Asset;

/// <summary>
/// Request DTO for updating an existing asset
/// </summary>
public class AssetUpdateRequest
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
    /// The currency code for this asset
    /// </summary>
    [StringLength(3)]
    public string? CurrencyCode { get; set; }
}