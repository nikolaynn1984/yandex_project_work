using EventApplication.Abstractions.Repositories;
using EventApplication.Abstractions.Services;
using EventApplication.Bookings.DTOs;
using EventApplication.Events.DTOs;
using EventDomain.Entities;
using EventDomain.Exceptions;

namespace EventApplication;

/// <summary>
/// Сервис бронирования билетов
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingQueueService bookingQueueService;
    private readonly IEventRepository eventRepository;
    private readonly IBookingRepository bookingRepository;

    public BookingService(IBookingQueueService bookingQueueService, IEventRepository eventRepository, IBookingRepository bookingRepository)
    {
        this.bookingQueueService = bookingQueueService;
        this.eventRepository = eventRepository;
        this.bookingRepository = bookingRepository;
    }

    /// <inheritdoc/>
    public async Task<AddBookingResult?> CreateBookingAsync(Guid eventId, UserContext user, CancellationToken cancellationToken = default)
    {
        try
        {
            await Funcs.bookingLock.WaitAsync();

            var eventItem = await this.eventRepository.GetById(eventId,  cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return null;

            await Valid(eventId, user, cancellationToken);

#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
            if (eventItem.TryReserveSeats(1) == false)
                throw new NoAvailableSeatsException("Свободных мест на это мероприятие нет.");
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

            await this.eventRepository.SaveChangesAsync(cancellationToken);

            var booking = await Add(eventId, user.Id, cancellationToken);


            return new AddBookingResult(booking.Id, eventId, booking.Status);
        }
        finally
        {
            Funcs.bookingLock.Release();
        }
    }

    private async Task<bool> Valid(Guid eventId, UserContext user, CancellationToken cancellationToken = default)
    {
        try
        {
            var bookings = await this.bookingRepository.GetByEventId(eventId, cancellationToken);
            
            var userBookings = bookings.Where(s => s.UserId == user.Id).ToList();
            if(bookings.Count  >= 10)
                throw new NoAvailableSeatsException("Превышен лимит бронированй для пользователя");

            return true;
        }
        catch
        {
            return true;
        }
        
    }

    /// <summary>
    /// Добавление планирования
    /// </summary>
    /// <param name="eventId">Идентификатор события</param>
    /// <returns>Объектная модель бронирования</returns>
    private async Task<Booking> Add(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {

        var id = Guid.NewGuid();

        var booking = new Booking(id, eventId);
        booking.UserId = userId;

        await this.bookingRepository.Add(booking, cancellationToken);

        this.bookingQueueService.Add(booking);

        return booking;
    }

    /// <inheritdoc/>
    public async Task<Booking> GetBookingByIdAsync(Guid bookingId, UserContext user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await this.bookingRepository.GetById(bookingId, cancellationToken);
    }


    public async Task Cancel(Guid bookingId, UserContext user, CancellationToken cancellationToken = default)
    {
        var booking = await this.bookingRepository.GetById(bookingId, cancellationToken);

        if(booking != null)
        {
            booking.Cancelled();

            await this.bookingRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
