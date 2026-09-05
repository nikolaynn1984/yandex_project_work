using Bookings.Application.Abstractions.Repositories;
using Bookings.Application.Abstractions.Services;
using Bookings.Application.DTOs;
using Bookings.Domain.Entities;
using System.Text.Json;

namespace Bookings.Application;

/// <summary>
/// Сервис бронирования билетов
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository bookingRepository;
    private readonly IBookingValidator bookingValidator;
    private readonly IOutboxRepository outboxRepository;

    public BookingService(IBookingRepository bookingRepository, IBookingValidator bookingValidator, IOutboxRepository outboxRepository)
    {
        this.bookingRepository = bookingRepository;
        this.bookingValidator = bookingValidator;
        this.outboxRepository = outboxRepository;
    }

    /// <inheritdoc/>
    public async Task<AddBookingResult?> CreateBookingAsync(Guid eventId, UserContext user, CancellationToken cancellationToken = default)
    {
        try
        {
            await Funcs.bookingLock.WaitAsync();

            

            if (cancellationToken.IsCancellationRequested)
                return null;

            //var eventItem = await this.bookingValidator.EventHandler(eventId, cancellationToken);

            await this.bookingValidator.UserSeatsCount(user, cancellationToken);


            var booking = await Add(eventId, user.Id, cancellationToken);


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
    private async Task<Booking> Add(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {

        var id = Guid.NewGuid();

        var booking = new Booking(id, eventId)
        {
            UserId = userId
        };

        await this.outboxRepository.Add(new OutboxMessage(id, booking.CreatedAt, "BookingCreated", new BookingBody(id, eventId, 1).ToString()));

        await this.bookingRepository.Add(booking, cancellationToken);

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


        await this.bookingValidator.CanceledValild(booking, user, cancellationToken);

        if(booking != null)
        {
            booking.Cancelled();

            await this.bookingRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
