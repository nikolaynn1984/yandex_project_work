using EventDomain.DataAccess;
using EventDomain.Extentions;
using EventDomain.Interfaces;
using EventDomain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventDomain.Services;

/// <summary>
/// Сервис бронирования билетов
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingQueueService bookingQueueService;
    private readonly AppDbContext context;
    private readonly SemaphoreSlim _bookingLock = new SemaphoreSlim(1,1);

    public BookingService(IBookingQueueService bookingQueueService, AppDbContext context)
    {
        this.bookingQueueService = bookingQueueService;
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<AddBookingResult?> CreateBookingAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _bookingLock.WaitAsync();

            var eventItem = await this.context.Events.FirstOrDefaultAsync(s => s.Id == eventId, cancellationToken);

            if (eventItem == null)
                throw new EventException($"Событие с идентификатором {eventId} не найден");

            if (cancellationToken.IsCancellationRequested)
                return null;

            if (eventItem.TryReserveSeats(1) == false)
                throw new NoAvailableSeatsException("No available seats for this event");

            var booking = await Add(eventId, cancellationToken);


            return new AddBookingResult(booking.Id, eventId, booking.Status);
        }
        finally
        {
            _bookingLock.Release();
        } 
    }

    /// <summary>
    /// Добавление планирования
    /// </summary>
    /// <param name="eventId">Идентификатор события</param>
    /// <returns>Объектная модель бронирования</returns>
    private async Task< Booking> Add(Guid eventId, CancellationToken cancellationToken = default)
    {

        var id = Guid.NewGuid();

        var booking = new Booking(id, eventId);

        await this.context.Bookings.AddAsync(booking, cancellationToken);

        await this.context.SaveChangesAsync(cancellationToken);
        this.bookingQueueService.Add(booking);

        return booking;
    }

    /// <inheritdoc/>
    public async Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var book = await this.context.Bookings.FirstOrDefaultAsync(s => s.Id == bookingId, cancellationToken);
        if(book == null)
        {
            throw new EventException($"Бронирование с идентификатором {bookingId} не найден");
        }

        return book;
    }
}
