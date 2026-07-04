using EventDomain.DataAccess;
using EventDomain.Extentions;
using EventDomain.Interfaces;
using EventDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventDomain.Services;

/// <summary>
/// Сервис фоновой обработки бронирований
/// </summary>
public class BookingHostedService : BackgroundService
{
    private readonly IBookingQueueService bookingQueueService;
    private readonly ILogger<BookingHostedService> logger;
    private readonly IServiceScopeFactory scopeFactory;
    public BookingHostedService(IBookingQueueService bookingQueueService, IServiceScopeFactory scopeFactory, ILogger<BookingHostedService> logger)
    {
        this.bookingQueueService = bookingQueueService;
        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Сервис фоновой обработки бронирования запущен");

        this.bookingQueueService.OnNextEvent += (bookings) => Bookingshandler(bookings, stoppingToken);
        
    }

    private async void Bookingshandler(List<Booking> bookings, CancellationToken stoppingToken)
    {
        var tasks = bookings.Select(b => ProcessBookingAsync(b.Id, stoppingToken));
        await Task.WhenAll(tasks);

        await this.bookingQueueService.Next();

    }

    private async Task ProcessBookingAsync(Guid bookingId, CancellationToken stoppingToken)
    {

        try
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Начата обработка бронирования {Id}", bookingId);

                await Task.Delay(TimeSpan.FromSeconds(Funcs.ProcessBookingDelaySecond), stoppingToken);
                using (var scope = this.scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();


                    var booking = await context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, stoppingToken);
                    if (booking == null || booking.Status != BookingStatus.Pending)
                        return;

                    var @event = await context.Events.AsNoTracking().FirstOrDefaultAsync(s => s.Id == booking.EventId, stoppingToken);
                    if (@event == null)
                        throw new EventException($"Событие с идентификатором {booking.EventId} не найден");


                    booking.Confirm();

                    await context.SaveChangesAsync(stoppingToken);

                    logger.LogInformation("Бронирование {Id} обработано", booking.Id);
                    
                }  
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Сервис фоновой обработки бронирования остановлен");
            await CancelBooking(bookingId, stoppingToken);
            return;
        }
        catch (EventException ex)
        {
            logger.LogWarning(ex, "Ошибка при обработки бронирования");
            await CancelBooking(bookingId, stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обработки бронирования");
            await CancelBooking(bookingId, stoppingToken);
        }
        
    }

    private async Task CancelBooking(Guid bookingId, CancellationToken cancelationToken)
    {
        try
        {
            using (var scope = this.scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var booking = await context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, cancelationToken);
                if (booking != null)
                {
                    booking.Reject();

                    var @event = await context.Events.FirstOrDefaultAsync(e => e.Id == booking.EventId, cancelationToken);
                    if (@event != null)
                        @event.ReleaseSeats();

                    await context.SaveChangesAsync(cancelationToken);
                }
            }
        }catch(Exception ex)
        {
            logger.LogError(ex, $"Не удалось отклонить бронирование {bookingId} после ошибки.");
        }
    }
}
