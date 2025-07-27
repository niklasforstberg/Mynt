namespace Mynt.Models;

public class SnapshotConfiguration
{
    public int Id { get; set; }

    /// <summary>
    /// User ID - null for global defaults
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Minimum percentage change required to create a new snapshot
    /// </summary>
    public required decimal ChangeThreshold { get; set; } = 1.0m;

    /// <summary>
    /// How often to check for snapshots (in hours)
    /// </summary>
    public required int CheckIntervalHours { get; set; } = 24;

    /// <summary>
    /// Whether snapshots are enabled for this user
    /// </summary>
    public required bool IsEnabled { get; set; } = true;

    /// <summary>
    /// When this configuration was last updated
    /// </summary>
    public required DateTime UpdatedAt { get; set; }

    // Navigation property
    public User? User { get; set; }
}