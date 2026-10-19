using Cinema.Api.Dtos;
using Cinema.Api.Hubs;
using Cinema.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace Cinema.Api.Services;

public class SeatLockCleanupService(
    ISeatLockService locks, IHubContext<SeatHub> hub, ILogger<SeatLockCleanupService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    foreach (var expired in locks.RemoveExpired())
                        await hub.Clients.Group(SeatHub.Group(expired.SessionId))
                            .SendAsync("SeatReleased",
                                new SeatEvent(expired.SessionId, expired.SeatIds, null), stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "Seat lock cleanup failed");
                }
            }
        }
        catch (OperationCanceledException) {  }
    }
}