namespace Mynt.Models.DTOs.Asset;

/// <summary>
/// Response DTO for portfolio snapshot data
/// </summary>
public class PortfolioSnapshotResponse
{
    /// <summary>
    /// Date of the snapshot
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    /// User's portfolio summary
    /// </summary>
    public required PortfolioSummary Personal { get; set; }

    /// <summary>
    /// Financial group portfolio summary (null if user not in group)
    /// </summary>
    public PortfolioSummary? Group { get; set; }

    /// <summary>
    /// Currency code for all values
    /// </summary>
    public required string CurrencyCode { get; set; }

    /// <summary>
    /// When this snapshot was calculated
    /// </summary>
    public required DateTime CalculatedAt { get; set; }
}

/// <summary>
/// Portfolio summary data
/// </summary>
public class PortfolioSummary
{
    /// <summary>
    /// Total value of all assets
    /// </summary>
    public required decimal AssetSummary { get; set; }

    /// <summary>
    /// Total value of all debts
    /// </summary>
    public required decimal DebtSummary { get; set; }

    /// <summary>
    /// Net worth (assets - debts)
    /// </summary>
    public required decimal TotalSummary { get; set; }

    /// <summary>
    /// Percentage change from previous snapshot
    /// </summary>
    public decimal? PercentageChange { get; set; }
}