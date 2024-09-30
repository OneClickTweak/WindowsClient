using OneClickTweak.Service.Services;

namespace OneClickTweak.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly LoaderService loaderService;

    public Worker(
        ILogger<Worker> logger,
        LoaderService loaderService)
    {
        this.logger = logger;
        this.loaderService = loaderService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);
        }

        var waitUntil = await loaderService.DetectTickAsync(stoppingToken);
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Initial run finished at: {time}", DateTimeOffset.Now);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Waiting until: {time}", waitUntil);
            }

            var diff = (int)(waitUntil - DateTime.UtcNow).TotalSeconds;
            await Task.Delay(Math.Max(diff, 1) * 1000, stoppingToken);
            await loaderService.DetectTickAsync(stoppingToken);
        }
    }
}