using EventApplication.Abstractions.Repositories;
using EventApplication.Abstractions.Services;
using EventApplication.Bookings.DTOs;
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
    public async Task<AddBookingResult?> CreateBookingAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        try
        {
            await Funcs.bookingLock.WaitAsync();

            var eventItem = await this.eventRepository.GetById(eventId, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return null;


#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
            if (eventItem.TryReserveSeats(1) == false)
                throw new NoAvailableSeatsException("Свободных мест на это мероприятие нет.");
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

            await this.eventRepository.SaveChangesAsync(cancellationToken);

            var booking = await Add(eventId, cancellationToken);


            return new AddBookingResult(booking.Id, eventId, booking.Status);
        }
        finally
        {
            Funcs.bookingLock.Release();
        }
    }

    /// <summary>
    /// Добавление планирования
    /// </summary>
    /// <param name="eventId">Идентификатор события</param>
    /// <returns>Объектная модель бронирования</returns>
    private async Task<Booking> Add(Guid eventId, CancellationToken cancellationToken = default)
    {

        var id = Guid.NewGuid();

        var booking = new Booking(id, eventId);

        await this.bookingRepository.Add(booking, cancellationToken);

        this.bookingQueueService.Add(booking);

        return booking;
    }

    /// <inheritdoc/>
    public async Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await this.bookingRepository.GetById(bookingId, cancellationToken);
    }
}
