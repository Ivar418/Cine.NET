using API.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Services.Background.Implementations;

public class DailyShowingsGeneratorService : BackgroundService, IDailyShowingsGeneratorService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyShowingsGeneratorService> _logger;

    public DailyShowingsGeneratorService(IServiceScopeFactory scopeFactory, ILogger<DailyShowingsGeneratorService> logger) {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task GenerateDailyShowingsAsync(CancellationToken cancellationToken = default) {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

        await DbSeeder.GenerateNextDayShowingsAsync(db, cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        var delay = GetDelayUntilNextRunUtc();

        _logger.LogInformation("Daily showings generator scheduled to run in {Delay}.", delay);

        try {
            await Task.Delay(delay, stoppingToken);
        }
        catch (OperationCanceledException) {
            return;
        }

        while (!stoppingToken.IsCancellationRequested) {
            try {
                await GenerateDailyShowingsAsync(stoppingToken);
                _logger.LogInformation("Daily showings generation completed.");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) {
                return;
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Failed to generate daily showings.");
            }

            try {
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
            catch (OperationCanceledException) {
                return;
            }
        }
    }

    private static TimeSpan GetDelayUntilNextRunUtc() {
        var now = DateTimeOffset.UtcNow;
        var nextRun = now.Date.AddDays(1).AddMinutes(5);

        if (nextRun <= now) {
            nextRun = nextRun.AddDays(1);
        }

        return nextRun - now;
    }
}