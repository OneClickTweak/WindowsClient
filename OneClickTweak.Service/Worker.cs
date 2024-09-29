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
        await loaderService.Load();
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}