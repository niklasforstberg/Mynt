using System.ComponentModel.DataAnnotations;

namespace Mynt.Models;

public class PortfolioSnapshot
{
    public int Id { get; set; }

    /// <summary>
    /// User ID - always required
    /// </summary>
    public required int UserId { get; set; }

    /// <summary>
    /// Financial Group ID - null if user not in a group
    /// </summary>
    public int? FinancialGroupId { get; set; }

    /// <summary>
    /// Date of the snapshot (start of day)
    /// </summary>
    public required DateTime Date { get; set; }

    // User-level summaries
    /// <summary>
    /// Total value of user's assets
    /// </summary>
    public required decimal AssetSummary { get; set; }

    /// <summary>
    /// Total value of user's debts
    /// </summary>
    public required decimal DebtSummary { get; set; }

    /// <summary>
    /// User's net worth (assets - debts)
    /// </summary>
    public required decimal TotalSummary { get; set; }

    // Group-level summaries (null if user not in group)
    /// <summary>
    /// Total value of all assets in the financial group
    /// </summary>
    public decimal? GroupAssetSummary { get; set; }

    /// <summary>
    /// Total value of all debts in the financial group
    /// </summary>
    public decimal? GroupDebtSummary { get; set; }

    /// <summary>
    /// Financial group's net worth (assets - debts)
    /// </summary>
    public decimal? GroupTotalSummary { get; set; }

    /// <summary>
    /// Currency code for all values
    /// </summary>
    [StringLength(3)]
    public required string CurrencyCode { get; set; }

    /// <summary>
    /// Percentage change from previous snapshot (user level)
    /// </summary>
    public decimal? PercentageChange { get; set; }

    /// <summary>
    /// Percentage change from previous snapshot (group level)
    /// </summary>
    public decimal? GroupPercentageChange { get; set; }

    /// <summary>
    /// When this snapshot was calculated
    /// </summary>
    public required DateTime CalculatedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public FinancialGroup? FinancialGroup { get; set; }
}