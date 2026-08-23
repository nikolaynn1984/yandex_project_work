using Bookings.Application.Abstractions.Repositories;
using Bookings.Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bookings.Infrastructure.Services;

/// <summary>
/// Сервис фоновой обработки бронирований
/// </summary>
public class BookingHostedService : BackgroundService
{
    private readonly ILogger<BookingHostedService> logger;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IMessageBroker messageBroker;
    private const int ProcessBookingDelaySecond = 5;
    public BookingHostedService(IServiceScopeFactory scopeFactory, IMessageBroker messageBroker, ILogger<BookingHostedService> logger)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
        this.messageBroker  = messageBroker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Сервис фоновой обработки бронирования запущен");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                   var  outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

                    var messages = await outbox.GetNoProcessed(stoppingToken);

                    foreach(var message in messages)
                    {
                        try
                        {
                            message.IsProcessed =  await this.messageBroker.PublishAsync(message.Type, message.Body);

                        }catch (Exception)
                        {
                            message.IsProcessed = false;
                        }
                    }

                    await outbox.SaveChangesAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Сервис фоновой обработки бронирования остановлен");
            }

            await Task.Delay(TimeSpan.FromSeconds(ProcessBookingDelaySecond), stoppingToken);
        }

    }
}
