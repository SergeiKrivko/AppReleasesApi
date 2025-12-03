using AppReleases.Core.Abstractions;

namespace AppReleases.Api.BackgroundServices;

public class ReleaseCleanerService(ILogger<ReleaseCleanerService> logger, IServiceProvider services)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ClearOldReleases(cancellationToken);
                await ClearUnusedAssets(cancellationToken);
                await ClearOldInstallers(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка в фоновой задаче");
            }

            await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
        }
    }

    private async Task ClearOldReleases(CancellationToken cancellationToken)
    {
        logger.LogInformation("Запуск очистки релизов: {time}", DateTime.Now);
        var scope = services.CreateScope();
        var cleanerService = scope.ServiceProvider.GetRequiredService<ICleanerService>();

        var count = await cleanerService.ClearOldReleasesAsync(cancellationToken);
        logger.LogInformation("Удалено {count} релизов", count);
    }

    private async Task ClearUnusedAssets(CancellationToken cancellationToken)
    {
        logger.LogInformation("Запуск очистки неиспользуемых ассетов: {time}", DateTime.Now);
        var scope = services.CreateScope();
        var cleanerService = scope.ServiceProvider.GetRequiredService<ICleanerService>();

        var count = await cleanerService.ClearUnusedAssetsAsync(cancellationToken);
        logger.LogInformation("Удалено {count} неиспользуемых ассетов", count);
    }

    private async Task ClearOldInstallers(CancellationToken cancellationToken)
    {
        logger.LogInformation("Запуск очистки старых установщиков: {time}", DateTime.Now);
        var scope = services.CreateScope();
        var cleanerService = scope.ServiceProvider.GetRequiredService<ICleanerService>();

        var count = await cleanerService.ClearOldInstallersAsync(cancellationToken);
        logger.LogInformation("Удалено {count} установщиков", count);
    }
}