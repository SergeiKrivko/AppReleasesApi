using AppReleases.Core.Abstractions;

namespace AppReleases.Api.BackgroundServices;

public class TempFileCleanerService(ILogger<TempFileCleanerService> logger, IFileRepository fileRepository)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Запуск очистки временных файлов: {time}", DateTime.Now);

                await ClearOldTempFiles(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка в фоновой задаче");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ClearOldTempFiles(CancellationToken cancellationToken)
    {
        var count = await fileRepository.ClearFilesCreatedBefore(FileRepositoryBucket.Temp, DateTime.UtcNow.AddHours(-1),
            cancellationToken);
        logger.LogInformation("Удалено {count} временных файлов", count);
    }
}