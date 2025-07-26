namespace Mynt.Models.DTOs.Asset;

/// <summary>
/// Summary statistics for a user's assets
/// </summary>
public class AssetSummaryResponse
{
    /// <summary>
    /// Total value of all assets converted to preferred currency
    /// </summary>
    public required decimal TotalValue { get; set; }

    /// <summary>
    /// Total number of assets owned by the user
    /// </summary>
    public required int AssetCount { get; set; }

    /// <summary>
    /// Number of assets that have at least one value recorded
    /// </summary>
    public required int AssetsWithValues { get; set; }

    /// <summary>
    /// Timestamp of the most recent asset value update
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Currency code for the total value
    /// </summary>
    public required string PreferredCurrency { get; set; }
}
