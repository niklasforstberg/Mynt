using Microsoft.EntityFrameworkCore;
using Mynt.Data;
using Mynt.Models;
using Mynt.Models.DTOs.Asset;

namespace Mynt.Services;

public class SnapshotService : ISnapshotService
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrencyConversionService _conversionService;
    private readonly ILogger<SnapshotService> _logger;

    public SnapshotService(
        ApplicationDbContext db,
        ICurrencyConversionService conversionService,
        ILogger<SnapshotService> logger)
    {
        _db = db;
        _conversionService = conversionService;
        _logger = logger;
    }

    public async Task CreateDailySnapshotsAsync()
    {
        _logger.LogInformation("Starting daily snapshot creation");

        var today = DateTime.UtcNow.Date;

        // Get all users with their configurations and group memberships
        var users = await _db.Users
            .Include(u => u.SnapshotConfiguration)
            .Include(u => u.FinancialGroupMemberships)
            .ThenInclude(fgm => fgm.FinancialGroup)
            .ToListAsync();

        foreach (var user in users)
        {
            var config = user.SnapshotConfiguration ?? GetDefaultConfiguration();

            if (!config.IsEnabled)
                continue;

            // Check if we already have a snapshot for today
            var existingSnapshot = await _db.PortfolioSnapshots
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.Date == today);

            if (existingSnapshot != null)
                continue;

            // Calculate user portfolio value
            var userPortfolio = await CalculateUserPortfolioAsync(user.Id, user.GetPreferredCurrency());

            if (userPortfolio == null)
                continue;

            // Calculate group portfolio value if user is in a group
            (decimal AssetSummary, decimal DebtSummary, decimal TotalSummary)? groupPortfolio = null;
            int? userFinancialGroupId = null;

            var userGroupMembership = user.FinancialGroupMemberships.FirstOrDefault();
            if (userGroupMembership != null)
            {
                userFinancialGroupId = userGroupMembership.FinancialGroupId;
                groupPortfolio = await CalculateFinancialGroupPortfolioAsync(userFinancialGroupId.Value, user.GetPreferredCurrency());
            }

            // Get last snapshot for comparison
            var lastSnapshot = await _db.PortfolioSnapshots
                .Where(s => s.UserId == user.Id)
                .OrderByDescending(s => s.Date)
                .FirstOrDefaultAsync();

            decimal? userPercentageChange = null;
            decimal? groupPercentageChange = null;

            if (lastSnapshot != null)
            {
                userPercentageChange = CalculatePercentageChange(lastSnapshot.TotalSummary, userPortfolio.Value.TotalSummary);

                if (groupPortfolio.HasValue && lastSnapshot.GroupTotalSummary.HasValue)
                {
                    groupPercentageChange = CalculatePercentageChange(lastSnapshot.GroupTotalSummary.Value, groupPortfolio.Value.TotalSummary);
                }
            }

            // Only create snapshot if user change exceeds threshold or no previous snapshot exists
            if (lastSnapshot == null || Math.Abs(userPercentageChange ?? 0) >= config.ChangeThreshold)
            {
                var snapshot = new PortfolioSnapshot
                {
                    UserId = user.Id,
                    FinancialGroupId = userFinancialGroupId,
                    Date = today,
                    AssetSummary = userPortfolio.Value.AssetSummary,
                    DebtSummary = userPortfolio.Value.DebtSummary,
                    TotalSummary = userPortfolio.Value.TotalSummary,
                    GroupAssetSummary = groupPortfolio?.AssetSummary,
                    GroupDebtSummary = groupPortfolio?.DebtSummary,
                    GroupTotalSummary = groupPortfolio?.TotalSummary,
                    CurrencyCode = userPortfolio.Value.CurrencyCode,
                    PercentageChange = userPercentageChange,
                    GroupPercentageChange = groupPercentageChange,
                    CalculatedAt = DateTime.UtcNow
                };

                _db.PortfolioSnapshots.Add(snapshot);
                _logger.LogInformation("Created snapshot for user {UserId} with {UserPercentageChange}% user change and {GroupPercentageChange}% group change",
                    user.Id, userPercentageChange, groupPercentageChange);
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Completed daily snapshot creation");
    }

    public async Task<IEnumerable<PortfolioSnapshotResponse>> GetUserSnapshotsAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _db.PortfolioSnapshots.Where(s => s.UserId == userId);

        if (fromDate.HasValue)
            query = query.Where(s => s.Date >= fromDate.Value);
        if (toDate.HasValue)
            query = query.Where(s => s.Date <= toDate.Value);

        var snapshots = await query.OrderByDescending(s => s.Date).ToListAsync();

        return snapshots.Select(s => new PortfolioSnapshotResponse
        {
            Date = s.Date,
            Personal = new PortfolioSummary
            {
                AssetSummary = s.AssetSummary,
                DebtSummary = s.DebtSummary,
                TotalSummary = s.TotalSummary,
                PercentageChange = s.PercentageChange
            },
            Group = s.GroupTotalSummary.HasValue ? new PortfolioSummary
            {
                AssetSummary = s.GroupAssetSummary!.Value,
                DebtSummary = s.GroupDebtSummary!.Value,
                TotalSummary = s.GroupTotalSummary.Value,
                PercentageChange = s.GroupPercentageChange
            } : null,
            CurrencyCode = s.CurrencyCode,
            CalculatedAt = s.CalculatedAt
        });
    }

    private async Task<(decimal AssetSummary, decimal DebtSummary, decimal TotalSummary, string CurrencyCode)?> CalculateUserPortfolioAsync(int userId, string preferredCurrency)
    {
        var assets = await _db.Assets
            .Include(a => a.AssetType)
            .Include(a => a.AssetValues.OrderByDescending(av => av.RecordedAt).Take(1))
            .Where(a => a.UserId == userId)
            .ToListAsync();

        var assetSummary = 0m;
        var debtSummary = 0m;

        foreach (var asset in assets)
        {
            var latestValue = asset.AssetValues.FirstOrDefault();
            if (latestValue != null)
            {
                var assetCurrency = asset.CurrencyCode ?? preferredCurrency;
                var convertedValue = await _conversionService.ConvertCurrencyAsync(
                    latestValue.Value,
                    assetCurrency,
                    preferredCurrency);

                if (convertedValue.HasValue)
                {
                    var isAsset = asset.AssetType?.IsAsset ?? true;
                    if (isAsset)
                        assetSummary += convertedValue.Value;
                    else
                        debtSummary += convertedValue.Value;
                }
            }
        }

        return (assetSummary, debtSummary, assetSummary - debtSummary, preferredCurrency);
    }

    private async Task<(decimal AssetSummary, decimal DebtSummary, decimal TotalSummary)?> CalculateFinancialGroupPortfolioAsync(int financialGroupId, string preferredCurrency)
    {
        var assets = await _db.Assets
            .Include(a => a.AssetType)
            .Include(a => a.AssetValues.OrderByDescending(av => av.RecordedAt).Take(1))
            .Where(a => a.FinancialGroupId == financialGroupId)
            .ToListAsync();

        var assetSummary = 0m;
        var debtSummary = 0m;

        foreach (var asset in assets)
        {
            var latestValue = asset.AssetValues.FirstOrDefault();
            if (latestValue != null)
            {
                var assetCurrency = asset.CurrencyCode ?? preferredCurrency;
                var convertedValue = await _conversionService.ConvertCurrencyAsync(
                    latestValue.Value,
                    assetCurrency,
                    preferredCurrency);

                if (convertedValue.HasValue)
                {
                    var isAsset = asset.AssetType?.IsAsset ?? true;
                    if (isAsset)
                        assetSummary += convertedValue.Value;
                    else
                        debtSummary += convertedValue.Value;
                }
            }
        }

        return (assetSummary, debtSummary, assetSummary - debtSummary);
    }

    private static decimal CalculatePercentageChange(decimal oldValue, decimal newValue)
    {
        if (oldValue == 0)
            return newValue > 0 ? 100 : 0;

        return ((newValue - oldValue) / oldValue) * 100;
    }

    private static SnapshotConfiguration GetDefaultConfiguration()
    {
        return new SnapshotConfiguration
        {
            ChangeThreshold = 1.0m,
            CheckIntervalHours = 24,
            IsEnabled = true,
            UpdatedAt = DateTime.UtcNow
        };
    }
}