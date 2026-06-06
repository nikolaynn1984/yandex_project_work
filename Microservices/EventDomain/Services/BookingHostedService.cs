using EventDomain.Interfaces;
using EventDomain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventDomain.Services
{
    /// <summary>
    /// Сервис фоновой обработки бронирований
    /// </summary>
    public class BookingHostedService : BackgroundService
    {
        private readonly IBookingQueueService bookingQueueService;
        private readonly ILogger<BookingHostedService> logger;
        public BookingHostedService(IBookingQueueService bookingQueueService, ILogger<BookingHostedService> logger)
        {
            this.bookingQueueService = bookingQueueService;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Сервис фоновой обработки бронирования запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if(this.bookingQueueService.TryDequeue(out Booking booking))
                    {
                        logger.LogInformation("Начата обработка бронирования {Id}", booking.Id);

                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);


                        booking.Status = BookingStatus.Confirmed;
                        booking.ProcessedAt = DateTime.Now;


                        logger.LogInformation("Бронирование {Id} обработано", booking.Id);
                    }
                }catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break; 
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при обработки бронирования");
                }
            }

            logger.LogInformation("Сервис фоновой обработки бронирования остановлен");
        }
    }
}
