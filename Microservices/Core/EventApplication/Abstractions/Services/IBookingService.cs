using EventApplication.Bookings.DTOs;
using EventApplication.Events.DTOs;
using EventDomain.Entities;

namespace EventApplication.Abstractions.Services;

/// <summary>
/// Бронирование бронирований
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Создание брони для указанного события
    /// </summary>
    /// <param name="eventId">Идентификатор события</param>
    /// <param name="user">Текущий пользователь</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AddBookingResult?> CreateBookingAsync(Guid eventId, UserContext user, CancellationToken cancellationToken = default);
    /// <summary>
    /// Получение брони по идентификатору
    /// </summary>
    /// <param name="bookingId">Идентификатор брони</param>
    /// <param name="user">Текущий пользователь</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Объектная модель Booking</returns>
    Task<Booking> GetBookingByIdAsync(Guid bookingId, UserContext user, CancellationToken cancellationToken = default);
    /// <summary>
    /// Отмена бронирования
    /// </summary>
    /// <param name="bookingId"></param>
    /// <param name="user">Текущий пользователь</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Cancel(Guid bookingId, UserContext user, CancellationToken cancellationToken = default);
}
