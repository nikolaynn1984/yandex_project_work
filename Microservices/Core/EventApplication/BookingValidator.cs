using EventApplication.Abstractions.Repositories;
using EventApplication.Abstractions.Services;
using EventApplication.Events.DTOs;
using EventDomain;
using EventDomain.Entities;
using EventDomain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace EventApplication;

/// <summary>
/// Валидатор добавления события
/// </summary>
public class BookingValidator : IBookingValidator
{
    private readonly IEventRepository eventRepository;
    private readonly IBookingRepository bookingRepository;

    public BookingValidator(IEventRepository eventRepository, IBookingRepository bookingRepository)
    {
        this.eventRepository = eventRepository;
        this.bookingRepository = bookingRepository;
    }

    public async Task<bool> UserSeatsCount( UserContext user, CancellationToken cancellationToken = default)
    {

        var bookings = await GetBookings(user.Id, cancellationToken);
        if (bookings == null)
            return true;

        if (bookings.Count >= 10)
            throw new NoAvailableSeatsException("Превышен лимит (10) бронированй для пользователя");


        return true;
    }

    private async Task<List<Booking>?> GetBookings(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await this.bookingRepository.GetByUser(userId, cancellationToken);

        }
        catch
        {
            return null;
        }
    }

    public async Task<Event?> EventHandler(Guid eventId, CancellationToken cancellationToken = default)
    {
        var eventItem = await this.eventRepository.GetById(eventId, cancellationToken);


#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
        if (eventItem.TryReserveSeats(1) == false)
            throw new NoAvailableSeatsException("Свободных мест на это мероприятие нет.");
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

        if (eventItem.StartAt >= DateTime.UtcNow)
            throw new ValidationException("Событие уже началось");


        await eventRepository.SaveChangesAsync(cancellationToken);

        return eventItem;
    }

    public async Task CanceledValild(Booking booking, UserContext user, CancellationToken cancellationToken = default)
    {
        if (booking.Status == BookingStatus.Cancelled)
            throw new ValidationException("Бронирование уже отменено");

        if (user.Id != booking.UserId && user.Role != "Admin")
            throw new ForbiddenExeption("Не достаточно прав");


        var eventItem = await this.eventRepository.GetById(booking.EventId, cancellationToken);
        if(eventItem != null)
        {
            eventItem.ReleaseSeats(1);
        }

        await eventRepository.SaveChangesAsync(cancellationToken);
     
    }
}
