using Mynt.Models.DTOs.Asset;

namespace Mynt.Services;

public interface ISnapshotService
{
    /// <summary>
    /// Creates daily snapshots for all users if needed
    /// </summary>
    Task CreateDailySnapshotsAsync();

    /// <summary>
    /// Gets historical snapshots for a user (includes group data if user is in a group)
    /// </summary>
    Task<IEnumerable<PortfolioSnapshotResponse>> GetUserSnapshotsAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
}