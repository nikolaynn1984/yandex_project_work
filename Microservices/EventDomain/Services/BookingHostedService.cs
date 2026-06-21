using EventDomain.Interfaces;
using EventDomain.Extentions;
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

                    await Task.Delay(TimeSpan.FromSeconds(Funcs.ProcessBookingDelaySecond), stoppingToken);

                    await _processingSemaphore.WaitAsync();

                    await this.eventService.GetAsync(booking.EventId, stoppingToken);

                    booking.Confirm();

                    logger.LogInformation("Бронирование {Id} обработано", booking.Id);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Сервис фоновой обработки бронирования остановлен");
                booking.Reject();
                this.eventService.ReleaseSeats(booking.EventId);
                return;
            }
            catch (EventException ex)
            {
                logger.LogWarning(ex, "Ошибка при обработки бронирования");
                booking.Reject();
                this.eventService.ReleaseSeats(booking.EventId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при обработки бронирования");
                booking.Reject();
                this.eventService.ReleaseSeats(booking.EventId);
            }
            finally
            {
                if(_processingSemaphore.CurrentCount == 0)
                   this._processingSemaphore.Release();
            }
        }
    }
}
