namespace Mynt.Models.DTOs.Asset;

/// <summary>
/// Summary statistics for a user's assets and debts
/// </summary>
public class AssetSummaryResponse
{
    /// <summary>
    /// Total value of assets (positive values)
    /// </summary>
    public required decimal AssetSummary { get; set; }

    /// <summary>
    /// Number of assets
    /// </summary>
    public required int AssetCount { get; set; }

    /// <summary>
    /// Total value of debts (negative values)
    /// </summary>
    public required decimal DebtSummary { get; set; }

    /// <summary>
    /// Number of debts
    /// </summary>
    public required int DebtCount { get; set; }

    /// <summary>
    /// Total summary (assets - debts)
    /// </summary>
    public required decimal TotalSummary { get; set; }

    /// <summary>
    /// Timestamp of the most recent asset value update
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Currency code for all values
    /// </summary>
    public required string CurrencyCode { get; set; }
}
