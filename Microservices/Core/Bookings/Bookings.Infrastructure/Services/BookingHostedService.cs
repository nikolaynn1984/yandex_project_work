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

    //private async void Bookingshandler(List<Booking> bookings, CancellationToken stoppingToken)
    //{
    //    var tasks = bookings.Select(b => ProcessBookingAsync(b.Id, stoppingToken));
    //    await Task.WhenAll(tasks);

    //}

    //private async Task ProcessBookingAsync(Guid bookingId, CancellationToken stoppingToken)
    //{

    //    try
    //    {
    //            logger.LogInformation("Начата обработка бронирования {Id}", bookingId);

    //            await Task.Delay(TimeSpan.FromSeconds(ProcessBookingDelaySecond), stoppingToken);
    //            using (var scope = this.scopeFactory.CreateScope())
    //            {
    //                //var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
    //                var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

    //                var booking = await bookingRepository.GetById(bookingId, stoppingToken);
    //                if (booking == null || booking.Status != BookingStatus.Pending)
    //                    return;

    //                //await eventRepository.GetById(booking.EventId, stoppingToken);

    //                booking.Confirm();

    //                await bookingRepository.SaveChangesAsync(stoppingToken);

    //                logger.LogInformation("Бронирование {Id} обработано", booking.Id);

    //            }
            
    //    }
    //    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
    //    {
    //        logger.LogInformation("Сервис фоновой обработки бронирования остановлен");
    //        await CancelBooking(bookingId, stoppingToken);
    //        return;
    //    }
    //    catch (BookingException ex)
    //    {
    //        logger.LogWarning(ex, "Ошибка при обработки бронирования");
    //        await CancelBooking(bookingId, stoppingToken);
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "Ошибка при обработки бронирования");
    //        await CancelBooking(bookingId, stoppingToken);
    //    }

    //}

    //private async Task CancelBooking(Guid bookingId, CancellationToken cancelationToken)
    //{
    //    try
    //    {
    //        using (var scope = this.scopeFactory.CreateScope())
    //        {
    //            //var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //            //var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
    //            var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

    //            var booking = await bookingRepository.GetById(bookingId, cancelationToken);
    //            if (booking != null)
    //            {
    //                booking.Reject();

    //                //var @event = await eventRepository.GetById(booking.EventId, cancelationToken);

    //                //if (@event != null)
    //                //    @event.ReleaseSeats();

    //                //await eventRepository.SaveChangesAsync(cancelationToken);

    //                await bookingRepository.SaveChangesAsync(cancelationToken);
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, $"Не удалось отклонить бронирование {bookingId} после ошибки.");
    //    }
    //}
}
