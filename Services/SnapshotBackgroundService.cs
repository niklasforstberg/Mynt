using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mynt.Services;

public class SnapshotBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SnapshotBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

    public SnapshotBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<SnapshotBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Snapshot background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CreateSnapshotsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating snapshots");
            }

            // Wait for next check interval
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CreateSnapshotsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var snapshotService = scope.ServiceProvider.GetRequiredService<ISnapshotService>();

        await snapshotService.CreateDailySnapshotsAsync();
    }
}