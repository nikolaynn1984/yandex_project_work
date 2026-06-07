using Event.Domain.Interfaces;
using EventDomain.Extentions;
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
        private readonly IEventService eventService;
        private readonly ILogger<BookingHostedService> logger;
        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);
        public BookingHostedService(IBookingQueueService bookingQueueService, IEventService eventService, ILogger<BookingHostedService> logger)
        {
            this.bookingQueueService = bookingQueueService;
            this.eventService = eventService;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Сервис фоновой обработки бронирования запущен");

            this.bookingQueueService.OnNextEvent += (bookings) => Bookingshandler(bookings, stoppingToken);
            
        }

        private async void Bookingshandler(List<Booking> bookings, CancellationToken stoppingToken)
        {
            var tasks = bookings.Select(b => ProcessBookingAsync(b, stoppingToken));
            await Task.WhenAll(tasks);

            await this.bookingQueueService.Next();

        }

        private async Task ProcessBookingAsync(Booking booking, CancellationToken stoppingToken)
        {

            try
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation("Начата обработка бронирования {Id}", booking.Id);

                    await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

                    await _processingSemaphore.WaitAsync();

                    await this.eventService.Get(booking.EventId, stoppingToken);

                    booking.Status = BookingStatus.Confirmed;
                    booking.ProcessedAt = DateTime.Now;


                    logger.LogInformation("Бронирование {Id} обработано", booking.Id);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Сервис фоновой обработки бронирования остановлен");
                booking.Status = BookingStatus.Rejected;
                booking.ProcessedAt = DateTime.Now;
                return;
            }
            catch (EventException ex)
            {
                logger.LogWarning(ex, "Ошибка при обработки бронирования");
                booking.Status = BookingStatus.Rejected;
                booking.ProcessedAt = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при обработки бронирования");
            }
            finally
            {
                this._processingSemaphore.Release();
            }
        }
    }
}
