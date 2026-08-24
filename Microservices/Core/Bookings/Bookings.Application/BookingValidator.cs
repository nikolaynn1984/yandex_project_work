using Bookings.Application.Abstractions.Repositories;
using Bookings.Application.Abstractions.Services;
using Bookings.Application.DTOs;
using Bookings.Domain;
using Bookings.Domain.Entities;
using Bookings.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Bookings.Application;

/// <summary>
/// Валидатор добавления события
/// </summary>
public class BookingValidator : IBookingValidator
{
    private readonly IBookingRepository bookingRepository;

    public BookingValidator(IBookingRepository bookingRepository)
    {
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
        catch(BookingException ex)
        {
            return null;
        }
    }


    public async Task CanceledValild(Booking booking, UserContext user, CancellationToken cancellationToken = default)
    {
        if (booking.Status == BookingStatus.Cancelled)
            throw new ValidationException("Бронирование уже отменено");

        if (user.Id != booking.UserId && user.Role != "Admin")
            throw new ForbiddenExeption("Не достаточно прав");
     
    }
}
